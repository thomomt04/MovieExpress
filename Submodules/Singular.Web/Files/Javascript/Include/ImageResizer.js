var ImageResizeHelper = (function () {
  var self = this;

  var ImgWidth,
      ImgHeight,
      ImgScale,
      _BackColor;

  self.Current = ko.observable();
  self.IsVisible = ko.observable(false);
  self.OpenCount = ko.observable(0);
  self.CurrentDoc = ko.observable();

  self.GetCanvasAttributes = function () {
    self.OpenCount();
    return { width: ImgWidth, height: ImgHeight };
  }
  self.GetCanvasStyle = function () {
    self.OpenCount();
    return { width: (ImgWidth * ImgScale) + 2 + 'px', height: (ImgHeight * ImgScale) + 2 + 'px' };
  }
  self.GetDialogWidth = function () {
    return ((ImgWidth * ImgScale) + 22) + 'px';
  }
  self.GetImageURL = function (ID) {
    if (self.CurrentDoc() && self.CurrentDoc().ExistsOnServer() == 2) {
      return Singular.RootPath + '/Singular/Images/LoadingSmall.gif';
    } else {
      if (ID) {
        return Singular.RootPath + '/Library/FileDownloader.ashx?ImageID=' + ID;
      } else {
        return '';
      }
    }
  }

  self.Close = function () {
    self.IsVisible(false);
    self.Current(null);
  }

  var _CanShow = function () {
    if (self.CurrentDoc()) {
      Singular.ShowMessage('Choose Image', 'There is a document busy uploading, please wait for this to complete');
      return false;
    } else {
      return true;
    }
  }

  self.LoadFromURL = function (URL, Width, Height, Scale, DocIDProp, BackColor, QSParams, Options) {

    var fReq = new XMLHttpRequest();
    fReq.open("GET", URL, true);
    fReq.responseType = "blob";
    fReq.onload = function (oEvent) {
      fReq.response.name = URL;
      self.ImageChosen({ files: [fReq.response] }, Width, Height, Scale, DocIDProp, BackColor, QSParams, Options);
    };
    fReq.send();

  };

  self.Browse = function (Width, Height, Scale, DocIDProp, BackColor, QSParams, Options) {
    if (_CanShow()) {
      Singular.ShowFileDialog(function () { self.ImageChosen(this, Width, Height, Scale, DocIDProp, BackColor, QSParams, Options) }, 'image/*');
    }
  }

  self.ImageChosen = function (fu, Width, Height, Scale, DocIDProp, BackColor, UserQSParams, Options) {
    if (_CanShow()) {
      ImgWidth = Width;
      ImgHeight = Height;
      ImgScale = Scale;
      _BackColor = BackColor;
      self.OpenCount(self.OpenCount() + 1);
      
      var _Complete = function (result) {
        Singular.PendingFileOperations.RemoveItem(self.CurrentDoc());

        self.CurrentDoc(null);
        if (result.Success) {
          DocIDProp(result.DocumentID);
          self.Close();
        } else {
          alert('Error uploading image: ' + result.Response);
        }
      }

      var _SetDoc = function () {
        self.CurrentDoc({
          Guid: ko.observable(Singular.NewGuid()), DocumentName: ko.observable(), DocumentID: ko.observable(), ExistsOnServer: ko.observable(2),
          UploadPercent: ko.observable(0), IsNew: ko.observable(true), GetParent: function () { return null; }
        });
      }

      var _UploadFile = function (file, qsParams) {
        _SetDoc();
        Singular.UploadFile(file, self.CurrentDoc(), 'png,jpg,jpeg', _Complete, qsParams);
      }

      if (fu.files && ImgHeight != 0) {
        self.Current(new ImageResizer(fu.files[0], _BackColor, function (IsBlob, FileName, Data, fileAttributes) {
          if (IsBlob) {
            Data.name = FileName;
            _UploadFile(Data, UserQSParams);
          } else {
            //data is base64 string
            _SetDoc();
            Singular.PendingFileOperations.push(self.CurrentDoc());
            Singular.RawAjaxCall('FileUpload.ashx?Type=Base64&FileName=' + FileName + '&DocumentGuid=' + self.CurrentDoc().Guid() + (UserQSParams ? '&' + UserQSParams : ''), null, Data,
              Singular.Data.HandleComplete(true, _Complete),
              Singular.Data.HandleComplete(false, _Complete), null, 'Text')
          }
        }, Options));
        setTimeout(function () {
          self.IsVisible(true);
        }, 0);
      } else {
        //doesnt support file api, just upload file.
        if (ImgHeight == 0)
        {
          _UploadFile(fu, UserQSParams);
        }
        else
        {
          _UploadFile(fu, (UserQSParams == null || UserQSParams == "" ? "" : UserQSParams + "&")
                            + 'ImgScaleParams=' + Width + ',' + Height);
        }
      }
    }
  }

  return self;
})();

/*
  Options: { AfterDraw: function, InitialSize: 'Fit|Fill', Clamp: false }
*/
var ImageResizer = function (File, BackColor, OnComplete, Options) {
  var self = this,
      _BackColor = BackColor;

  var jCanvas = $('#IRCanvas'),
      canvas = jCanvas[0],
      ctx = canvas.getContext('2d'),
      IAspect, CAspect,
      img = new Image(),
      SaveMimeType,
      SaveFileName,
      zoomLevel = 1;

  Options = Options || {};

  var SmallImg,
    cClicked,
    mLastPos = { x: 0, y: 0 },
    Offset = { x: 0, y: 0 };

  var GetSmallImageSize = function () {
    //if (img.width < img.height) {
    //  return { width: max, height: max / img.width * img.height };
    //} else {
    //  return { width: max / img.height * img.width, height: max };
    //}
    return { width: img.width * zoomLevel, height: img.height * zoomLevel };
  }
  var GetSmallImage = function () {
    //resizes in 2 steps to improve quality.
    SmallImg = document.createElement('canvas');
    var osc = document.createElement('canvas'),
        osg = osc.getContext('2d'),
        gr = SmallImg.getContext('2d');

    var size = GetSmallImageSize();
    osc.width = size.width * 2;
    osc.height = size.height * 2;
    SmallImg.width = size.width;
    SmallImg.height = size.height;
    osg.drawImage(img, 0, 0, osc.width, osc.height);
    gr.drawImage(osc, 0, 0, SmallImg.width, SmallImg.height);
  }

  self.Zoom = function (factor) {
    var delta = factor - 1,
        increase = zoomLevel * delta,
        newZoom = zoomLevel + increase;

    Offset.x += (Offset.x - canvas.width / 2) * delta;
    Offset.y += (Offset.y - canvas.height / 2) * delta;
    SetZoom(newZoom);
    
  }
  var SetZoom = function (NewZoom) {
    zoomLevel = NewZoom;
    GetSmallImage(zoomLevel);
    ClampOffset();
    Draw();
  }

  self.Fit = function () {
    if (IAspect > CAspect) {
      var zoom = canvas.width / img.width;
      if (_BackColor) {
          Offset.x = 0;
          Offset.y = (canvas.height - img.height * zoom) / 2;
        }
      SetZoom(zoom);
    } else {
      var zoom = canvas.height / img.height;
      if (_BackColor) {
        Offset.x = (canvas.width - img.width * zoom) / 2;
        Offset.y = 0;
      }
      SetZoom(canvas.height / img.height);
    }
  }
  self.Fill = function () {
    Offset.x = 0;
    Offset.y = 0;
    if (IAspect > CAspect) {
      SetZoom(canvas.height / img.height);
    } else {
      SetZoom(canvas.width / img.width);
    }
  }

  self.Save = function () {

    Options.AfterDraw = null;
    Draw();

    var saveCanvas;
    if ((_BackColor || (SmallImg.width > canvas.width && SmallImg.height > canvas.height)) && zoomLevel <= 1) {

      //image is bigger than container in both dimensions, upload container image.
      saveCanvas = canvas;

    } else if (_BackColor || SmallImg.width > canvas.width || SmallImg.height > canvas.height) {
      //image partially fits in container, create intersection of image and container.

      var sc = document.createElement('canvas'),
          sg = sc.getContext('2d');

      sc.width = (_BackColor ? canvas.width : Math.min(canvas.width, SmallImg.width)) / Math.max(1, zoomLevel);
      sc.height = (_BackColor ? canvas.height : Math.min(canvas.height, SmallImg.height)) / Math.max(1, zoomLevel);

      if (_BackColor) {
        sg.fillStyle = _BackColor;
        sg.fillRect(0, 0, sc.width, sc.height);
      }

      if (zoomLevel <= 1) {
        //draw the already reduced image onto the canvas
        sg.drawImage(SmallImg, Offset.x, Offset.y);
      } else {
        //draw the original dimension image onto the smaller canvas.
        sg.drawImage(img, parseInt(Offset.x / zoomLevel), parseInt(Offset.y / zoomLevel));
      }
      
      saveCanvas = sc;

    } else {
      //image fits inside container

      if (zoomLevel < 1) {
        saveCanvas = SmallImg;
      } else {
        //upload the original image.
        OnComplete(true, File.name, File);
        return;
      }
           
    }

    //toBlob is not well supported yet, so use base64 if not supported.
    if (saveCanvas.toBlob) {
      saveCanvas.toBlob(function (blob) {
        OnComplete(true, SaveFileName, blob)
      }, SaveMimeType, 0.95);
    } else {
      OnComplete(false, SaveFileName, saveCanvas.toDataURL(SaveMimeType, 0.95).split(',')[1]);
    }

  }
  var ClampOffset = function () {
    if (!_BackColor || Options.Clamp) {
      Offset.x = Math.min(0, Math.max(canvas.width - SmallImg.width, Offset.x));
      Offset.y = Math.min(0, Math.max(canvas.height - SmallImg.height, Offset.y));
    }
  }
  img.onload = function () {
    URL.revokeObjectURL(img.src);
    IAspect = img.width / img.height;
    CAspect = canvas.width / canvas.height;
    GetSmallImage();

    if (_BackColor) {
      if (Options.InitialSize == 'Fill' || Options.Clamp) {
        self.Fill();
      } else {
        self.Fit();
      }
    } else {
      Draw();
    }
  }
  img.src = URL.createObjectURL(File);
  if (File.type.toLowerCase().indexOf("png") < 0) {
    SaveMimeType = 'image/jpeg';
    SaveFileName = 'image.jpg';
  } else {
    SaveMimeType = 'image/png';
    SaveFileName = 'image.png';
  }

  $(document).mousedown(function (e) {
    cClicked = false;
    if (e.target == canvas) {
      cClicked = true;
      mLastPos.x = e.screenX;
      mLastPos.y = e.screenY;
      return false;
    }
  }).mousemove(function (e) {
    if (cClicked && e.buttons == 1) {
      Offset.x += e.screenX - mLastPos.x;
      Offset.y += e.screenY - mLastPos.y;
      mLastPos.x = e.screenX;
      mLastPos.y = e.screenY;

      ClampOffset();
      Draw();
    }

  });

  var Draw = function () {
    if (_BackColor) {
      ctx.fillStyle = _BackColor;
      ctx.fillRect(0, 0, canvas.width, canvas.height);
    } else {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
    }
    
    if (SmallImg) {
      ctx.drawImage(SmallImg, Offset.x, Offset.y);

      if (Options.AfterDraw) Options.AfterDraw(ctx, Offset);
    } else {
      ctx.fillStyle = '#fff';
      ctx.font = 'normal normal 600 15px Calibri';
      ctx.fillText('Loading Image', 10.5, 20.5);
    }

  }
  Draw();
}