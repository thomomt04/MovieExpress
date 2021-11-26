/// <reference path="Singular.Core.js" /> 

//#region  Ajax / Get / Send Data 

Singular.AjaxTimeout = 45;
Singular.AlwaysShowLoadingBar = false;

//Calls and Ajax method on the current page.
Singular.RawAjaxCall = function (Handler, Method, Args, Success, Fail, PostType, DataType) {

  var RootURL = Singular.RootPath;
  if (Args && Args.Args && Args.Args._RootURL) RootURL = Args.Args._RootURL;

  var PostUrl = RootURL + '/Library/' + Handler;
  if (!PostType) PostType = "POST";
  Args = Args || {};

  var ShowLoadingBar = Singular.AlwaysShowLoadingBar;
  if (Args.ShowLoadingBar) ShowLoadingBar = Args.ShowLoadingBar;
  if (Args.Args && Args.Args.ShowLoadingBar) ShowLoadingBar = Args.Args.ShowLoadingBar;

  if (Singular.OnSendData) Singular.OnSendData(Args);

  if (Method) { //Non Stateless HandleCommand
    Args = { Method: Method, Args: Args };
  }
  if (ShowLoadingBar) {
    if (Singular.LoadingCount == 0) {
      Singular.ShowLoadingBar();
    } else {
      /*if the loading bar is already displayed, cancel this request.
        the user may have double clicked before he loading bar was shown.*/
      return;
    }
  }

  //Cross site request forgery
  var CSRF = document.getElementById('hCSRF');
  CSRF = CSRF ? CSRF.value : null;

  var Params = (DataType == 'Text' ? Args : (PostType == 'POST' ? JSON.stringify(Args) : Args.Args));

  $.ajax({
    type: PostType,
    url: PostUrl,
    data: Params,
    contentType: PostType == 'POST' && !DataType ? "application/json; charset=utf-8" : null,
    //dataType: "text",
    timeout: (Singular.DebugMode ? 0 : Singular.AjaxTimeout * 1000),
    success: function (Data) {

      if (Singular.OnReceiveData && Singular.OnReceiveData(Data)) {
        Singular.HideLoadingBar();
        return;
      }

      if (Data && typeof Args == 'object' && Data.AjaxProgressInfo) {
        //handle progress update.
        var pi = Data.AjaxProgressInfo;
        var OnProgress = Args.Args ? Args.Args.OnProgress : undefined;
        if (OnProgress) {
          OnProgress(pi);
        } else {
          Singular.UpdateLoadingBar(pi.CurrentStatus, pi.CurrentStep, pi.MaxSteps);
        }

        Singular.RawAjaxCall(Singular.Data.StatelessHandlerName, null, { Method: 'ProgressPoll', Args: { OnProgress: OnProgress }, ProgressGuid: pi.Guid, ShowLoadingBar: false, ProgressEnd: true }, Success, Fail);


      } else {
        //handle proper complete callback.
        if (ShowLoadingBar || (Args && Args.ProgressEnd)) Singular.HideLoadingBar();
        Success(Data);
      }

    },
    error: function (jqXHR, textStatus, error) {
      if (ShowLoadingBar || (Args && Args.ProgressEnd)) Singular.HideLoadingBar();
      Fail(jqXHR, textStatus, error);
    },
    headers: { hCSRF: CSRF }
  });

};
Singular.AJAXCall = function (Method, Args, Success, Fail, HandlerID) {

  Args = typeof Args == 'string' ? { "Data": Args } : Args;
  var GuidElement = document.getElementById('hPageGuid');
  if (GuidElement) {
    Args.PageGuid = GuidElement.value;
  } else {
    Args.VMType = ViewModel.constructor.Type;
  }


  Singular.RawAjaxCall(HandlerID == 2 ? Singular.Data.StatelessHandlerName : Singular.Data.SessionHandlerName, Method, Args,
  //Success
  function (Data) {
    Success(Data);
  },
  //Fail
  function (jqXHR, textStatus, error) {
    if (Fail) {
      Fail(jqXHR, textStatus, error);
    } else {

      if (error == 'timeout') {
        Singular.ShowMessage('Error', 'The request has taken too long to complete. Click ok and the page will reload.', 2, function () { window.location.reload(); });
      } else {
        if (jqXHR.responseText) {
          error += '<br/>' + jqXHR.responseText;
        }
        Singular.ShowMessage('Error', error, 2);
      }
    }
  });

};

Singular.Data = (function () {
  var self = this;
  self.UseGETRequests = false;
  self.getHTTPMethod = function () {
    return self.UseGETRequests ? 'GET' : 'POST';
  }

  self.StatelessHandlerName = 'StatelessHandler';
  self.SessionHandlerName = 'DataHandler.ashx';

  self.HandleComplete = function (IsSuccess, OnComplete) {
    var CompleteArgs = { Data: null, Success: IsSuccess, ErrorText: '' };
    if (IsSuccess) {
      return function (data) {
        //if the data returns an object that looks like CompleteArgs, then return that object.
        //otherwise, return the completeargs object, after setting the data property.
        if (data != null && data.Success !== undefined && data.ErrorText !== undefined) {
          if (OnComplete) OnComplete(data);
        } else {
          CompleteArgs.Data = data;
          if (OnComplete) OnComplete(CompleteArgs);
        }
      }
    } else {
      return function (jqXHR, textStatus, error) {
        CompleteArgs.ErrorText = error + ' - ' + jqXHR.responseText;
        if (OnComplete) OnComplete(CompleteArgs);
      }
    }
  }

  return self;
})();

Singular.StatelessAJAXCall = function (Method, Type, BaseArgs, OnComplete, HTTPMethod) {
  if (!HTTPMethod) HTTPMethod = 'POST';

  var Handler = Singular.Data.StatelessHandlerName;

  if (HTTPMethod == 'POST') {
    BaseArgs = (!BaseArgs ? {} : BaseArgs)
    BaseArgs.Method = Method;
    BaseArgs.Type = Type;
    BaseArgs.Args = (!BaseArgs.Args ? {} : BaseArgs.Args);

    //if this is in a statefull page, send the page guid.
    var GuidElement = document.getElementById('hPageGuid');
    if (GuidElement) {
      BaseArgs.PageGuid = GuidElement.value;
    }
  } else {
    //Get
    Handler = Method + '/' + Type;
  }

  Singular.RawAjaxCall(Handler, null, BaseArgs,
    Singular.Data.HandleComplete(true, OnComplete),
    Singular.Data.HandleComplete(false, OnComplete), HTTPMethod);

}

Singular.GetDataStateless = function (Type, Args, OnComplete) {

  Singular.StatelessAJAXCall('GetData', Type, { Args: Args }, OnComplete, Singular.Data.getHTTPMethod());

}
Singular.SaveDataStateless = function (ObjectToSave, OnComplete) {

  Singular.StatelessAJAXCall('SaveData', ObjectToSave.constructor.Type, { Object: ModelBinder.Serialise(ObjectToSave) }, OnComplete);

}
Singular.CallServerMethod = function (Type, Method, Args, OnComplete) {

  Singular.StatelessAJAXCall('Command', Type, { CallMethod: Method, Args: Args }, OnComplete);

};
Singular.GetDataStatelessPromise = function (Type, Args) {

  if (!(typeof Promise !== "undefined" && Promise.toString().indexOf("[native code]") !== -1)) {
    console.log('Promises are not supported :(');
    return
  }

  return new Promise(function (resolve, reject) {
    Singular.StatelessAJAXCall('GetData', Type,
    { Args: Args },
    function (response) {
      if (response.Success) {
        resolve(response.Data)
      }
      else {
        reject(response.ErrorText)
      }
    }, Singular.Data.getHTTPMethod())
  })

}
Singular.CallServerMethodPromise = function (Type, Method, Args) {

  if (!(typeof Promise !== "undefined" && Promise.toString().indexOf("[native code]") !== -1)) {
    console.log('Promises are not supported :(');
    return
  }

  return new Promise(function (resolve, reject) {
    Singular.StatelessAJAXCall('Command', Type, { CallMethod: Method, Args: Args },
    function (response) {
      if (response.Success) {
        resolve(response.Data)
      }
      else {
        Singular.OnServerMethodError(Method, Args, response.ErrorText);
        reject(response.ErrorText)
      }
    })
  })

};

//Gets JSON data from the server. For search screen / autocomplete text boxes etc.
Singular.GetServerData = function (Args, OnComplete) {
  Args = Args ? Args : {};
  Args.ShowLoadingBar = Args.ShowLoadingBar == undefined ? true : Args.ShowLoadingBar;
  Singular.AJAXCall('VM_GetData', Args, OnComplete, undefined, 2);
};

Singular.SendROCommand = function (CommandName, Args, OnComplete) {
  Singular.SendCommand(CommandName, { SendModel: false, TextArgument: Args }, OnComplete);
};

//Causes HandleCommand to be called on the current ViewModel on the server.
Singular.SendCommand = function (CommandName, Args, OnComplete, OnError) {

  Args = (typeof Args == 'object' && !(Args instanceof Date)) ? Args : { TextArgument: Args };
  Args.ShowLoadingBar = Args.ShowLoadingBar == undefined ? true : Args.ShowLoadingBar;

  if (Args.FetchModel == undefined) {
    Args.FetchModel = true;
  }
  Args.CommandName = CommandName;
  var LoadingText = undefined;
  if (Args.LoadingText) {
    LoadingText = Args.LoadingText;
  }
  if (Singular.LastButtonClicked && Singular.LastButtonClicked.ButtonArgument) {
    Args.ButtonArgument = Singular.LastButtonClicked.ButtonArgument;
  }

  function SendCommandWorker() {

    if (Args.SendModel == undefined || Args.SendModel) {
      var Serialiser = new KOFormatterObject();
      Serialiser.IncludeCleanProperties = true;

      Args.Model = Serialiser.Serialise(ViewModel);
    }

    Singular.AJAXCall('HandleCommandAsync', Args, function (Data) {
      var ReturnData = jQuery.parseJSON(Data);

      Singular.CreateMessages(ReturnData.MessageList);

      //Update the model if we got given one.
      if (ReturnData.ViewModel != undefined) {
        Singular.DeserialiseModel(ReturnData.ViewModel, false);
        Singular.ConvertControls();
      }

      if (OnComplete) {
        OnComplete(ReturnData.ReturnData);
      }

    }, OnError);

  };

  if (Args.Validate) {
    Singular.CheckPendingFileOperation(function () {
      Singular.Validation.Validate(ViewModel, function (IsValid) {
        if (IsValid) {
          SendCommandWorker();
        } else {
          if ((Singular.Validation.ValidationDisplayMode & 2) != 0) {
            Singular.ShowMessage(LocalStrings.Submit, LocalStrings.JSValidationError2 + '<br/><br/>' + Singular.Validation.GetBrokenRulesHTML(ViewModel), 2);
          }
        }
      });
    });

  } else {
    SendCommandWorker();
  }

};

//Allow manipulation of pre and post ajax calls
Singular.OnSendData = null;
Singular.OnReceiveData = null;

//#endregion

//#region  Common Data 

var CommonData = (function () {
  var self = {};

  var _FetchCount = 0,
      _Ready = false,
      _ToLoad = {};

  window.ClientData = {};

  self.FetchClientData = function (OnComplete) {
    _Ready = true;

    Singular.AddLogEntry('Data fetch count: ' + _FetchCount);
    if (_FetchCount == 0) {
      OnComplete();
    } else {
      for (var Name in _ToLoad) {

        FetchList(Name, _ToLoad[Name].TypeOrName, _ToLoad[Name].Source, _ToLoad[Name].Hash, function () {
          if (_ToLoad[Name]) delete _ToLoad[Name];
          _FetchCount -= 1;
          if (_FetchCount == 0) {
            OnComplete();
          }
        });
      }
    }
  }

  self.RegisterList = function (Name, TypeOrName, Source, Hash) {
    if (_Ready) {
      FetchList(Name, TypeOrName, Source, Hash);
    } else {
      _FetchCount += 1;
      _ToLoad[Name] = { Source: Source, TypeOrName: TypeOrName, Hash: Hash };
    }
  }

  var FetchList = function (ListName, TypeOrName, Source, Hash, OnComplete) {

    var Key = "CommonData_" + Source + "_" + ListName;

    if (Singular.SupportsLocalStorage()) {
      if (localStorage[Key]) {
        //Get the stored object.
        var Obj;
        try {
          Obj = JSON.parse(localStorage[Key]);
        } catch (e) {
          Obj = null;
        }

        if (Obj && Obj.Hash == Hash) {
          //if the hash is equal, return the local list 
          try {
            ClientData[ListName] = Singular.CreateList(Obj.List);
            if (OnComplete) OnComplete();
            return;
          } catch (e) { } //Data might be corrupt from old version of library, so ignore errors and re-fetch.
        };
      };
    };

    //if the hash is different, or the local storage is empty, then list needs to be re-fetched.
    var Obj = {};
    Obj.Hash = Hash;
    Obj.List = [];

    var Args = { SourceName: ListName, TypeOrName: TypeOrName, Source: Source };

    Singular.ShowLoadingBar(1000, 'Loading data. Please wait...');
    Singular.AJAXCall('VM_GetCommonData', Args, function (Data) {
      Singular.HideLoadingBar();
      Obj.List = $.parseJSON(Data);
      if (Singular.SupportsLocalStorage()) {
        try {
          localStorage[Key] = JSON.stringify(Obj);
        } catch (e) {
          localStorage.clear();
        }

      }
      ClientData[ListName] = Singular.CreateList(Obj.List);
      if (OnComplete) OnComplete();
    }, undefined, 2);

  }


  return self;
})();

function RegisterLookupData(Name, TypeOrName, Source, Hash) {

  CommonData.RegisterList(Name, TypeOrName, Source, Hash);

};

//#endregion

//#region  List functionality  



//Singular List Base
Singular.CreateList = function (BaseList) {

  //check if its a real array, or a json array.
  if (typeof (BaseList) == 'string') {
    BaseList = jQuery.parseJSON(arguments[0]);
  };
  //Check if its an array, not a list of object.
  if (BaseList.length > 0 && !(BaseList[0] instanceof Object)) {
    for (var i = 0; i < BaseList.length; i++) {
      BaseList[i] = { Val: BaseList[i] };
    }
  }
  BaseList.Version = ko.observable(0);
  BaseList.Refresh = function (NewList) {
    if (NewList) {
      BaseList.length = 0;
      BaseList.push.apply(BaseList, NewList);
    }
    BaseList.Version(BaseList.Version() + 1);
  }
  return BaseList;
};

//Makes sure the same drop down value cant be selected twice in one list.
Singular.FilterUnique = function (List, ListMember, Property) {

  //Get the values that are in the list already.
  var ParentList = Property.AttachedTo.SInfo.ParentList();
  var Values = {};
  for (var i = 0; i < ParentList.length; i++) {
    var obj = ParentList[i];
    if (obj != Property.AttachedTo) {
      Values[obj[Property.PropertyName]()] = true;
    }
  }

  var fList = [];
  for (var i = 0; i < List.length; i++) {
    if (!Values[List[i][ListMember]]) {
      fList.push(List[i]);
    }
  }
  return fList;
};

Singular.FilterOld = function (List, ObjValue, ValueMember, ChildListName /* [, FilterNames] */) {

  var fList = [];

  if (!ChildListName || ChildListName == '') {
    //Easy single level list. (Andrew Cronwright -- Added '!ChildListName || ' to check for the case when 'ChildListName' is undefined)
    for (var i = 0; i < List.length; i++) {
      if (!List[i][arguments[4]] || List[i][ValueMember] == ObjValue) {
        fList.push(List[i]);
      } else {
        //its inactive and not selected.
      }
    }
  } else {
    //2 level list.

    for (var i = 0; i < List.length; i++) {
      var Item = List[i];
      var Add = false;
      if (arguments[4] != '' && Item[arguments[4]]) {
        //inactive - check if an item in the child list is selected.
        for (var j = 0; j < Item[ChildListName].length; j++) {
          if (Item[ChildListName][j][ValueMember] == ObjValue) {
            Add = true;
            break;
          }
        }
      } else {
        Add = true;
      }
      if (Add) {
        //active - filter the child list.

        var fChildList = Singular.FilterOld(Item[ChildListName], ObjValue, ValueMember, '', arguments[5]);
        if (fChildList.length > 0) {
          if (fChildList.length != Item[ChildListName].length) {

            //Create a copy of the object.
            Item = jQuery.extend({}, Item);
            Item[ChildListName] = fChildList;
          }
          fList.push(Item);
        }
      };
    };
  };
  return fList;

};

//Sorts / Filters a list.
Singular.ProcessList = function (List, ListProperty) {

  if (arguments.length == 1) {
    //old method signature
    ListProperty = List;
    List = Singular.UnwrapFunction(ListProperty);
  } else {
    List = Singular.UnwrapFunction(List);
  }

  var SortOptions = ListProperty.ProcessOptions ? ListProperty.ProcessOptions.SortOptions() : null;

  if (SortOptions && SortOptions.SortProperty) {

    //Register a dependency on every item in the list. this lets us use peek in the sort accessor.
    //for (var i = 0; i < List.length; i++) {
    //  var tmp = ko.utils.unwrapObservable(List[i][SortOptions.SortProperty]);
    //}
    //Sort the list.
    var SortedList = Singular.MergeSort(List, function (Item) {
      var Prop = Item[SortOptions.SortProperty];

      var Value = ko.utils.peekObservable(Prop);
      if (Value == null) return 'zzzz';

      if (Prop.DropDownText) {
        return Prop.DropDownText.toLowerCase();
      }

      switch (Prop.DataType) {
        case 'n':
          if (typeof (Value) == 'number') return Value;
          Value = Value.replace('(', '-').replace(')', '');
          return parseFloat(Value.replace(/[^0-9\.\-]+/g, ''));
          break;
        case 'd':
          if (Value instanceof Date) {
            return Value;
          } else {
            return new Date(Value.replace(/&nbsp;/g, ' '));
          }
          break;
        default:
          return Value.toString().toLowerCase();
      }

    }, SortOptions.SortAsc);

    return SortedList;

  } else {
    return List;
  }

}

Singular.MergeSort = function (input, ValueAccessor, Direction, TempID) {
  var mid = 0;

  if (input.length > 1) {
    var mid = parseInt(input.length / 2);

    //Create the left list
    var left = [];
    for (var i = 0; i < mid; i++)
      left[i] = input[i];
    //Create the right list
    var right = [];
    for (var i = mid; i < input.length; i++)
      right[i - mid] = input[i];

    var SortedLeft = Singular.MergeSort(left, ValueAccessor, Direction, TempID);
    var SortedRight = Singular.MergeSort(right, ValueAccessor, Direction, TempID);

    //Merge the left and right lists.
    var result = [], LeftValue, RightValue;
    var tempName = '__ArraySortValue' + TempID;

    while (SortedLeft.length && SortedRight.length) {

      if (TempID && !SortedLeft[0][tempName]) {
        SortedLeft[0][tempName] = ValueAccessor(SortedLeft[0]);
      }
      if (TempID && !SortedRight[0][tempName]) {
        SortedRight[0][tempName] = ValueAccessor(SortedRight[0]);
      }
      LeftValue = TempID ? SortedLeft[0][tempName] : ValueAccessor(SortedLeft[0]);
      RightValue = TempID ? SortedRight[0][tempName] : ValueAccessor(SortedRight[0])

      if ((Direction == 1 && LeftValue <= RightValue)
          || (Direction == 0 && LeftValue > RightValue)) {

        result.push(SortedLeft.shift());
      } else {
        result.push(SortedRight.shift());
      };
    };
    while (SortedLeft.length) {
      result.push(SortedLeft.shift());
    };
    while (SortedRight.length) {
      result.push(SortedRight.shift());
    };
    return result;

  } else {
    return input;
  };

};

//#endregion

//#region  Can Delete 

Singular.CanDeleteItem = function (Item, Prompt, DeleteFunction, DeleteImmediate) {
  //If the item was created on the client, then allow the delete.
  if (Item.IsClientNew()) {
    DeleteFunction();
  } else {

    if (Prompt) {
      Singular.ShowMessageQuestion('Delete', Prompt, ExecuteCheckDelete);
    } else {
      ExecuteCheckDelete()
    }

  }

  function ExecuteCheckDelete() {
    //Ask the server object if it can be deleted.

    var ServerArgs;

    function ServerCallComplete(cdi) {
      if (cdi.CanDeleteResult != 1) {
        DeleteFunction();
      } else {
        Singular.ShowMessage('Delete', cdi.Detail, 2);
      }
    }

    if (Singular.IsStateless) {

      var Formatter = new KOFormatterObject();
      Formatter.IncludeChildren = false;
      Formatter.IncludeClean = true;
      Formatter.IncludeCleanProperties = true;
      Singular.StatelessAJAXCall('Delete', Item.constructor.Type, { Object: Formatter.Serialise(Item), DeleteNow: DeleteImmediate }, function (Args) {
        if (Args.Success) {
          ServerCallComplete(Args.Data);
        } else {
          Singular.ShowMessage('Delete', 'Error: ' + Args.ErrorText);
        }
      });


    } else {
      Singular.AJAXCall('VM_CanDelete', Item.Guid(), function (Data) {
        ServerCallComplete(jQuery.parseJSON(Data));
      }, undefined, 2);
    }

  };

};

//#endregion

//#region  Immediate Save 

//Object to keep track of things.
var DelayedSaveInfo = {
  SendingCount: 0,
  PendingCount: 0,
  ObjectsToSend: {},
  Remove: function (Guid) {
    var Object = this.ObjectsToSend[Guid].Object
    delete this.ObjectsToSend[Guid];
    this.PendingCount -= 1;
    return Object;
  }
};
var SavingDiv;

Singular.SaveObjectDelayed = function (Obj, Delay) {

  function CheckDelay(ObjData) {
    if (DelayedSaveInfo.ObjectsToSend[ObjData.Guid]) {
      if (new Date().getTime() - ObjData.LastTime.getTime() < Delay || ObjData.Object.IsBusy()) {
        setTimeout(function () {
          CheckDelay(ObjData);
        }, Delay);
      } else {
        Singular.SaveObject(DelayedSaveInfo.Remove(ObjData.Guid));
      }
    };
  };

  var Guid = Obj.Guid.peek();
  if (DelayedSaveInfo.ObjectsToSend[Guid]) {
    DelayedSaveInfo.ObjectsToSend[Guid].LastTime = new Date();
  } else {
    DelayedSaveInfo.PendingCount += 1;
    DelayedSaveInfo.ObjectsToSend[Guid] = { Object: Obj, Guid: Guid, LastTime: new Date() };
    CheckDelay(DelayedSaveInfo.ObjectsToSend[Guid]);
  }

}

Singular.SaveObject = function (Object) {

  var OnComplete, lReturnProperty;
  for (var i = 1; i < arguments.length; i++) {
    if (typeof arguments[i] == "function") {
      OnComplete = arguments[i];
    } else if (typeof arguments[i] == "string") {
      lReturnProperty = arguments[i];
    }
  }


  if ((Object.IsValid()) && !Object.__Deleted) {

    var Args = {
      Object: ModelBinder.Serialise(Object),
      ObjectType: Object.constructor.Type,
      ReturnProperty: lReturnProperty
    }
    if (Object != ViewModel) {
      Args.ContainerGuid = Object.__ContainerProperty.AttachedTo.Guid.peek();
      Args.ContainerProperty = Object.__ContainerProperty.PropertyName;
    }
    if (DelayedSaveInfo.SendingCount == 0) {
      if (!SavingDiv) {
        SavingDiv = document.createElement('div');
        SavingDiv.innerHTML = 'Saving...';
        SavingDiv.style.position = 'fixed';
        $(SavingDiv).addClass('SavingDiv');
        document.body.appendChild(SavingDiv);
      } else {
        $(SavingDiv).show();
      }

    }
    DelayedSaveInfo.SendingCount += 1;
    Singular.AJAXCall('ObjectUpdate', Args, function (d) {

      var Result = jQuery.parseJSON(d);
      if (!Result.Success) {
        Singular.ShowMessage('Error saving', Result.ErrorText);
      }

      Object.SInfo.MarkSaved();
      DelayedSaveInfo.SendingCount -= 1;
      if (DelayedSaveInfo.SendingCount == 0) {
        $(SavingDiv).fadeOut();
      }
      if (OnComplete) {
        OnComplete(Result);
      }
    });
  };
};
Singular.FlushSavingObjects = function (OnComplete) {
  for (var name in DelayedSaveInfo.ObjectsToSend) {
    Singular.SaveObject(DelayedSaveInfo.Remove(name));
  }
  if (OnComplete) OnComplete();
};
Singular.HasPendingSave = function () {
  return DelayedSaveInfo.PendingCount > 0 || DelayedSaveInfo.SendingCount > 0;
}

//#endregion

