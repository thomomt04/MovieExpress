function Point(x, y) {
  var self = this;
  self.X = x;
  self.Y = y;
}
if (!Array.prototype.insert) {
  Array.prototype.insert = function (index) {
    index = Math.min(index, this.length);
    arguments.length > 1
        && this.splice.apply(this, [index, 0].concat([].pop.call(arguments)))
        && this.insert.apply(this, arguments);
    return this;
  };
}

var Rectangle = (function () {
  function Rectangle(x, y, w, h) {
    var self = this;
    self.X = parseInt(x);
    self.Y = parseInt(y);
    self.Width = parseInt(w);
    self.Height = parseInt(h);

  }
  Rectangle.prototype.Bottom = function () {
    return this.Y + this.Height;
  }
  Rectangle.prototype.Right = function () {
    return this.X + this.Width;
  }
  Rectangle.prototype.Fill = function (Context, Colour) {
    if (Colour) Context.fillStyle = Colour;
    Context.fillRect(this.X, this.Y, this.Width, this.Height);
  }
  Rectangle.prototype.Draw = function (Context, Colour) {
    if (Colour) Context.strokeStyle = Colour;
    Context.strokeRect(this.X, this.Y, this.Width, this.Height);
  }
  Rectangle.prototype.Contains = function (x, y) {
    return x >= this.X && x <= this.Right() && y >= this.Y && y <= this.Bottom();
  }
  Rectangle.FromLTRB = function (l, t, r, b) {
    return new Rectangle(l, t, r - l, b - t);
  }
  Rectangle.prototype.Expand = function (Pixels) {
    return new Rectangle(this.X - Pixels, this.Y - Pixels, this.Width + Pixels + Pixels, this.Height + Pixels + Pixels);
  }
  Rectangle.prototype.MakeRounded = function (Context, Radius) {
    Context.beginPath();
    Context.moveTo(this.X + Radius, this.Y);
    Context.arcTo(this.X + this.Width, this.Y, this.X + this.Width, this.Y + this.Height, Radius);
    Context.arcTo(this.X + this.Width, this.Y + this.Height, this.X, this.Y + this.Height, Radius);
    Context.arcTo(this.X, this.Y + this.Height, this.X, this.Y, Radius);
    Context.arcTo(this.X, this.Y, this.X + this.Width, this.Y, Radius);
    Context.closePath();
    return Context;
  }

  return Rectangle;
})();

var DrawUtils = (function () {
  var self = this,
      _Ellipsis = '...',
      _EllipsesWidths = {};

  function GetEllipsisWidth (Context) {
    var font = Context.font;
    var width = _EllipsesWidths[font];
    if (width == undefined) {
      width = Context.measureText(_Ellipsis).width;
      _EllipsesWidths[font] = width;
    }
    return width;
  }

  self.CanvasSupported = function () {
    return !!window.CanvasRenderingContext2D;
  }

  //Shortens the text to fit in the max width, and adds ... to the end.
  self.GetSizedText = function (Context, Text, MaxWidth, IsNumber) {

    var width = Context.measureText(Text).width,
        ellipsisWidth = GetEllipsisWidth(Context),
        returnValue = '';

    if (width <= MaxWidth || width <= ellipsisWidth) {
      //the text will all fit.
      return Text;
    } else {
      //needs to be shortened.
      if (IsNumber) {
        //numbers get turned into #'s
        while (true) {
          if (Context.measureText(returnValue + '#').width <= MaxWidth) {
            returnValue += '#'
          } else {
            return returnValue;
          }
        }
      } else {
        //shorten
        MaxWidth = Math.max(MaxWidth - ellipsisWidth, ellipsisWidth);
        var len = Text.length;
        //guess how short it should be based on average char width
        len = Math.ceil(len * (MaxWidth / width));

        //measure, and keep shortening till its short enough.
        do {
          returnValue = Text.substring(0, len).toString();
          width = Context.measureText(returnValue).width;
          len -= 1;
        } while (width >= MaxWidth)
       
        return returnValue.toString() + _Ellipsis;
      }

    }
  }

  //Returns an array of lines
  self.WrapText = function (Context, Text, MaxWidth, FirstLineSubtractWidth) {
    
    var inputLines = Text.split('\n'),
        outputLines = [];

    function PushLine(line) {
      outputLines.push(self.GetSizedText(Context, line.substr(0, line.length - 1), MaxWidth, false));
    }

    for (var i = 0; i < inputLines.length; i++) {
      var words = inputLines[i].split(' '),
          line = '';

      for (var j = 0; j < words.length; j++) {
        var testLine = line + words[j] + ' ';
        var testWidth = Context.measureText(testLine).width;
        if (testWidth > MaxWidth - (outputLines == 0 ? FirstLineSubtractWidth : 0) && j > 0) {
          PushLine(line);
          line = words[j] + ' ';
        } else {
          line = testLine;
        }
      }
      PushLine(line);
    }
    return outputLines;
  }

  self.FlipImage = function (Img, Horizontal) {

    var canvas = document.createElement('canvas'),
                  ctx = canvas.getContext('2d');
    canvas.width = Img.width;
    canvas.height = Img.height;
    ctx.translate(0, Img.height);
    ctx.scale(Horizontal ? -1 : 1, Horizontal ? 1 : -1);
    ctx.drawImage(Img, 0, 0);

    var newImage = new Image();
    newImage.src = canvas.toDataURL();
    return newImage;
  }

  self.FixEvent = function (e, offsetPos) {
    //target element
    e.srcElement = e.target || e.srcElement;

    //touch
    if (e.touches && e.touches.length > 0) {
      e.ctlX = e.touches[0].pageX - offsetPos.left;
      e.ctlY = e.touches[0].pageY - offsetPos.top;
    } else {
      //offset
      if (offsetPos) {
        e.ctlX = e.pageX - offsetPos.left;
        e.ctlY = e.pageY - offsetPos.top
      } else if (!e.hasOwnProperty('offsetX')) {
        var curleft = curtop = 0;
        var obj = e.srcElement;
        do {
          curleft += obj.offsetLeft;
          curtop += obj.offsetTop;
        } while (obj = obj.offsetParent);

        e.offsetX = e.layerX - curleft;
        e.offsetY = e.layerY - curtop;
      }
    }
    e.ctlXRoot = e.ctlX;
    e.ctlYRoot = e.ctlY;
    
    return e;
  }

  self.HexToHSL = function (color) {
    var r = parseInt(color.substr(1, 2), 16) / 255;
    var g = parseInt(color.substr(3, 2), 16) / 255;
    var b = parseInt(color.substr(5, 2), 16) / 255;

    var max = Math.max(r, g, b), min = Math.min(r, g, b);
    var h, s, l = (max + min) / 2;

    if (max == min) {
      h = s = 0; // achromatic
    }
    else {
      var d = max - min;
      s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
      switch (max) {
        case r: h = (g - b) / d + (g < b ? 6 : 0); break;
        case g: h = (b - r) / d + 2; break;
        case b: h = (r - g) / d + 4; break;
      }
      h /= 6;
    }
    return new Color(h, s, l);
  }

  return self;
})();

var Color = function (h, s, l) {
  this.h = h;
  this.s = s;
  this.l = l;

  this.GetCSSColor = function () {
    return 'hsl(' + (this.h * 360).toFixed(0) + ', ' + (this.s * 100).toFixed(0) + '%, ' + (this.l * 100).toFixed(0) + '%)';
  }
}

if (DrawUtils.CanvasSupported()) {

  window.CanvasRenderingContext2D.prototype.CreateShadow = function (x, y, blur, colour, save) {
    if (save) this.save();
    if (x != undefined) this.shadowOffsetX = x;
    if (y != undefined) this.shadowOffsetY = y;
    if (blur != undefined) this.shadowBlu = blur;
    if (colour != undefined) this.shadowColor = colour;
  };

  if (!window.CanvasRenderingContext2D.prototype.setLineDash) {
    window.CanvasRenderingContext2D.prototype.setLineDash = function (param) {

    };
  }

}

var ScrollBar = function (Horizontal) {
  var self = {};
    
  //Private vars
  var TopRect,
    BottomRect,
    TrackRect,
    ThumbRect,
		_Thickness = 17,
    _Length = 200,
    _OffsetX = 0,
    ButtonSize = 32,
    _MinThumbSize = 16,
		_Value = 0,
    _Maximum = 10,
    _LargeChange = 10,
		_Canvas = document.createElement('canvas'),
    _Context = _Canvas.getContext('2d'),
    _IgnoreThumbPos = false,
    _IsDirty = false;

  var Parts = {
    None: 0,
    TopButton: 1,
    BottomButton: 2,
    TrackTop: 3,
    TrackBottom: 4,
    Thumb: 5
  }

  //if touchscreen, make the scrollbar a bit bigger.
  try
  {
    if (('ontouchstart' in window) ||
        (navigator.maxTouchPoints > 0) ||
        (navigator.msMaxTouchPoints > 0)) {

      _Thickness = 24;
      ButtonSize = 36;
      _MinThumbSize = 32;
    }
  } catch (e) { }
    
    
  var MaxValue = function () {
    return Math.max(_Maximum - _LargeChange + 1, 0)
  }

  self.Maximum = MaxValue;

  var ResizeControl = function () {
    if (Horizontal) {
      TopRect = new Rectangle(0, 0, ButtonSize, _Thickness);
      BottomRect = new Rectangle(_Length - ButtonSize, 0, ButtonSize, _Thickness);
      TrackRect = new Rectangle(ButtonSize + 1, 0, _Length - (ButtonSize * 2) - 2, _Thickness);
    } else {
      TopRect = new Rectangle(0, 0, _Thickness, ButtonSize);
      BottomRect = new Rectangle(0, _Length - ButtonSize, _Thickness, ButtonSize);
      TrackRect = new Rectangle(0, ButtonSize + 1, _Thickness, _Length - (ButtonSize * 2) - 2);
    }

    _Canvas.width = Horizontal ? _Length : _Thickness;
    _Canvas.height = Horizontal ? _Thickness : _Length;
    _IsDirty = true;
  }
  var RecalcThumb = function () {
    
    if (Horizontal) {
      var x, w;
      w = parseInt(Math.max((TrackRect.Width * Math.min(_LargeChange / (_Maximum + 1), 1)), _MinThumbSize));
      x = MaxValue() == 0 ? TrackRect.X : ((Math.min(_Value, MaxValue()) / MaxValue() * (TrackRect.Width - w)) + TrackRect.X);
      ThumbRect = new Rectangle(x, 0, w, _Thickness);
    } else {
      var y, h;
      h = parseInt(Math.max((TrackRect.Height * Math.min(_LargeChange / (_Maximum + 1), 1)), _MinThumbSize));
      y = MaxValue() == 0 ? TrackRect.Y : ((Math.min(_Value, MaxValue()) / MaxValue() * (TrackRect.Height - h)) + TrackRect.Y);
      ThumbRect = new Rectangle(0, y, _Thickness, h);
    }
   
    _IsDirty = true;
  }

  var Draw = function () {

    _Context.clearRect(0, 0, _Thickness, _Length);

    _Context.fillStyle = '#ddd';
    TrackRect.Fill(_Context);

    _Context.fillStyle = '#aaa';
    TopRect.Fill(_Context);
    BottomRect.Fill(_Context);
    ThumbRect.Fill(_Context);

  }

  self.Resize = function (Length) {
    _Length = Length;
    ResizeControl();
    RecalcThumb();
  }

  self.SetLargeChange = function (LargeChange) {
    _LargeChange = LargeChange;
    self.SetValues(_Value);
  }

  self.Thickness = function () {
    return _Thickness;
  }
  self.SetValues = function (value, max) {
    var OldValue = _Value;
    if (value != undefined) _Value = parseInt(value);
    if (max != undefined) _Maximum = max;

    _Value = Math.max(0, Math.min(_Value, MaxValue()));

    if (!_IgnoreThumbPos) RecalcThumb();
    if (_Value != OldValue) self.ValueChanged();
  }

  self.ValueChanged = function () {

  }

  self.LargeChange = function () {
    return _LargeChange;
  }

  self.GetCanvas = function () {
    if (_IsDirty) {
      Draw();
      _IsDirty = false;
    }
    return _Canvas;
  }

  self.Value = function () {
    return _Value;
  }

  var GetPart = function () {
    var x = MousePos.X, y = MousePos.Y;
    if (TopRect.Contains(x, y)) return Parts.TopButton;
    if (BottomRect.Contains(x, y)) return Parts.BottomButton;
    if (ThumbRect.Contains(x, y)) return Parts.Thumb;
    if (Horizontal) {
      if (TrackRect.Contains(x, y)) return x < ThumbRect.X ? Parts.TrackTop : Parts.TrackBottom;
    } else {
      if (TrackRect.Contains(x, y)) return y < ThumbRect.Y ? Parts.TrackTop : Parts.TrackBottom;
    }
    
    return Parts.None;
  }
  var PartWithMouseCapture, ThumbMouseDifference, CurrentPartWithMouseCapture, IsMouseDown, MousePos;

  self.MouseDown = function (e) {
    IsMouseDown = true;
    MousePos = new Point(e.ctlX, e.ctlY);
    PartWithMouseCapture = GetPart();
    CurrentPartWithMouseCapture = PartWithMouseCapture;
    ThumbMouseDifference = Horizontal ? (e.ctlX - ThumbRect.X) : (e.ctlY - ThumbRect.Y);

    switch (PartWithMouseCapture) {
      case Parts.TopButton:
        self.SetValues(_Value - 1);
        StartTimer();
        break;
      case Parts.BottomButton:
        self.SetValues(_Value + 1);
        StartTimer();
        break;
      case Parts.TrackTop:
        self.SetValues(_Value - _LargeChange);
        StartTimer();
        break;
      case Parts.TrackBottom:
        self.SetValues(_Value + _LargeChange);
        StartTimer();
        break;
    }

  }

  self.MouseMove = function (e) {
    MousePos = new Point(e.ctlX, e.ctlY);
    if (PartWithMouseCapture == Parts.Thumb) {

      var ThumbLocaion;
      
      if (Horizontal){
        ThumbLocaion = Math.max(TrackRect.X, Math.min(TrackRect.Right() - ThumbRect.Width, e.ctlX - ThumbMouseDifference));
        ThumbRect.X = ThumbLocaion;
      }else{
        ThumbLocaion = Math.max(TrackRect.Y, Math.min(TrackRect.Bottom() - ThumbRect.Height, e.ctlY - ThumbMouseDifference));
        ThumbRect.Y = ThumbLocaion;
      }
            
      _IsDirty = true;

      var ScollSpace = Horizontal ? (TrackRect.Width - ThumbRect.Width) : (TrackRect.Height - ThumbRect.Height);
      if (ScollSpace > 0) {
        var NewValue = Math.round(((ThumbLocaion - (Horizontal ? TrackRect.X : TrackRect.Y)) / ScollSpace * MaxValue()));
        if (NewValue != _Value) {
          _IgnoreThumbPos = true;
          self.SetValues(NewValue);
          _IgnoreThumbPos = false;
        }
      }
    }
  }

  self.MouseUp = function (e) {
    IsMouseDown = false;
    if (PartWithMouseCapture == Parts.Thumb) {
      RecalcThumb();
    }
    PartWithMouseCapture = Parts.None;
    clearInterval(Timer);
    Timer = null;
  }

  var Timer;
  var StartTimer = function () {

    Timer = setTimeout(function () {
      if (IsMouseDown) {
        Timer_Tick();
        Timer = setInterval(Timer_Tick, 50);
      }
    }, 400);

  }

  var Timer_Tick = function () {
    if (IsMouseDown) {
      CurrentPartWithMouseCapture = GetPart();

      switch (PartWithMouseCapture) {
        case Parts.TopButton:
          if (CurrentPartWithMouseCapture == Parts.TopButton) self.SetValues(_Value - 1);
          break;
        case Parts.BottomButton:
          if (CurrentPartWithMouseCapture == Parts.BottomButton) self.SetValues(_Value + 1);
          break;
        case Parts.TrackTop:
          if (CurrentPartWithMouseCapture == Parts.TrackTop) self.SetValues(_Value - _LargeChange);
          break;
        case Parts.TrackBottom:
          if (CurrentPartWithMouseCapture == Parts.TrackBottom) self.SetValues(_Value + _LargeChange);
          break;
      }
    } else {
      clearInterval(Timer);
    }

  };


  //Setup
  self.Resize(100);

  return self;
};

