/// <reference path="Intellisense.js" />
Singular.DateEditorOnReadOnly = true;


var LogEvent = function (msg) {
  console.log(msg);
}

var ScriptLoader = (function () {
  var self = this;

  self.LoadCordovaScript = function (OnComplete) {

    //Phonegap Build puts this file in the www folder. 
    //If it fails to load, then the app is being run in browser.
    self.AppendScript('cordova.js', OnComplete);

  }

  self.AppendScript = function (Src, OnComplete) {

    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = Src;

    script.addEventListener('load', function () {
      OnComplete({ Success: true });
    });
    script.addEventListener('error', function () {
      OnComplete({ Success: false });
    });
    document.head.appendChild(script);
  }

  return self;
})();

var AppLoader = (function () {
  var self = {};

  var _OnReadyCallBack,
      _OnUIReadyCallBack,
      _Resources = [],
      _ResourcesToLoad = 0;

  self.Ready = false;

  self.Initialise = function (OnReadyCallBack, OnUIReadyCallBack) {

    _OnReadyCallBack = OnReadyCallBack;
    _OnUIReadyCallBack = OnUIReadyCallBack;

    $.support.cors = true;

    //Load the cordova script from the correct location:
    LogEvent('Loading cordova script');
    ScriptLoader.LoadCordovaScript(function (args) {

      //Add a handler for the device ready event, or document ready for desktop.
      if (args.Success) {
        AppLoader._IsDevice = true;
        LogEvent('attaching deviceready event');
        document.addEventListener('deviceready', _OnDeviceReady, true);
      } else {
        LogEvent('attaching documentready event');
        $(document).ready(_OnDeviceReady);
      }

      if (IsDevice()) ko.bindingHandlers.NValue.ClearOnFocus = true;

    });

  };

  self.LoadTemplate = function (TemplateFileName) {
    _LoadResource(TemplateFileName, _ResourceType.View);
  };

  self.LoadModel = function (ModelFileName) {
    _LoadResource(ModelFileName, _ResourceType.Model);
  };

  var _ResourceType = { View: 1, Model: 2 };

  var _LoadResource = function (ResourceFileName, Type) {

    var Resource;

    if (typeof (ResourceFileName) == 'string') {
      // check if we already have this template
      var FindKey = Type + '_' + ResourceFileName;
      Resource = _Resources.Find('FindKey', FindKey);

      // if not then create it
      if (!Resource) {
        Resource = { FileName: ResourceFileName, Type: Type, IsLoaded: false, FindKey: FindKey }
        _Resources.push(Resource);
        _ResourcesToLoad += 1;
      }
    } else {
      //Resource was passed in as first parameter.
      Resource = ResourceFileName;
    }

    // if it is already loaded then abort
    if (Resource.IsLoaded) return;

    // if the app is ready, then load the Resource now
    if (self.Ready) {

      LogEvent('Loading ' + (Type == _ResourceType.View ? 'View: ' : 'Model: ') + Resource.FileName);
      if (Type == _ResourceType.View) {
        _LoadView(Resource);
      } else {
        _LoadModel(Resource);
      }
    }
  }

  var _LoadView = function (View) {

    //Get the full path
    var FullPath = window.location.pathname.substring(0, window.location.pathname.length - 10) + 'Views/' + View.FileName + '.htm';

    $.ajax({
      url: FullPath,
      type: 'GET',
      dataType: 'html',
      success: function (ResponseText) {
        View.IsLoaded = true;
        try {
          $('body').append(ResponseText);
          LogEvent(View.FileName + " template loaded");

          _ResourcesToLoad -= 1;

        } catch (e) {
          LogEvent(View.FileName + " template failed to load");
          alert('Error loading template file: ' + FullPath + ': ' + e);
        }

        if (_ResourcesToLoad == 0) {
          _OnAllResourcesLoaded();
        }

      },
      error: function () {
        alert('Error loading template file: ' + FullPath);
      }

    });

  }

  var _LoadModel = function (Model) {
    //Get the full path
    var FullPath = window.location.pathname.substring(0, window.location.pathname.length - 10) + 'Models/' + Model.FileName + '.js';

    ScriptLoader.AppendScript(FullPath, function () {

      LogEvent(Model.FileName + " model loaded");

      Model.IsLoaded = true;
      _ResourcesToLoad -= 1;
      if (_ResourcesToLoad == 0) {
        _OnAllResourcesLoaded();
      }

    });

  }

  var _OnDeviceReady = function () {

    LogEvent("Device Ready, UserAgent: " + navigator.userAgent);

    //Singular library setup
    Singular.DetermineClickEvent();
    Singular.CreateLoadingBar();

    self.Ready = true;

    //Load templates that have requested to load
    for (var i = 0; i < _Resources.length; i++) {
      _LoadResource(_Resources[i], _Resources[i].Type);
    }

  }

  var _OnAllResourcesLoaded = function () {

    //Run the user ready code.
    try {
      _OnReadyCallBack();
    } catch (e) {
      LogEvent('Error in OnReady: ' + e);
    }

    ko.applyBindings(Application);

    if (Singular._pvt.OnLoadCallbacks) {
      for (var i = 0; i < Singular._pvt.OnLoadCallbacks.length; i++) {
        Singular._pvt.OnLoadCallbacks[i]();
      }
    }

    if (_OnUIReadyCallBack) {
      _OnUIReadyCallBack();
    }

    //Check if re-login is required.
    Singular.Security.Refetch();

    document.addEventListener("backbutton", function () {
      Application.GoBack();
      return false;
    }, false);

    //Hide splashscreen.
    if (navigator.splashscreen) {
      setTimeout(function () {
        navigator.splashscreen.hide();
      }, 1000);
    }
  }

  return self;
})();

AppLoader._IsDevice = false;
function IsDevice() {
  return AppLoader._IsDevice;// navigator.userAgent.match(/(iPhone|iPod|iPad|Android|BlackBerry|IEMobile)/);
}

Singular.OnPageLoad = function (CallBack) {
  if (!Singular._pvt.OnLoadCallbacks) Singular._pvt.OnLoadCallbacks = [];
  Singular._pvt.OnLoadCallbacks.push(CallBack);
}


//#region  Loading bar 

Singular.CreateLoadingBar = function () {

  var Container = document.createElement('div');


  $(Container).attr('data-bind', 'template: {if: $root.CurrentModule().ViewModel.IsLoading, afterRender: CreateLoadingCanvas }');
  document.body.appendChild(Container);

  var Overlay = document.createElement('div');
  $(Overlay).css({ position: 'fixed', left: '0', top: '0', background: 'rgba(0,0,0,0.1)', width: '100%', height: '100%' });
  Container.appendChild(Overlay);

  var InnerContainer = document.createElement('div');
  $(InnerContainer).css({ position: 'fixed', left: (($(window).width() - 100) / 2) + 'px', top: '200px', background: 'rgba(255,255,255,0.9)', 'border-radius': '30px', padding: '10px' });
  InnerContainer.setAttribute('id', 'LoadingContainer');
  Overlay.appendChild(InnerContainer);

};
var CLInstance;

function CreateLoadingCanvas() {

  ko.utils.domNodeDisposal.addDisposeCallback(document.getElementById('LoadingContainer'), function () {
    CLInstance.kill();
  });

  CLInstance = new CanvasLoader('LoadingContainer');

  //CLInstance.setColor('#000'); // default is '#000000'
  CLInstance.setShape('spiral'); // default is 'oval'
  CLInstance.setDiameter(80); // default is 40
  CLInstance.setDensity(13); // default is 40
  CLInstance.setRange(1.2); // default is 1.3
  CLInstance.setSpeed(1); // default is 2
  CLInstance.setFPS(15); // default is 24
  CLInstance.show(); // Hidden by default
}

//#endregion

//#region  View Model Base 


function SetupViewModel(ViewModel, InheritingViewModel, SetupCallback) {
  ko.utils.extend(ViewModel, new InheritingViewModel());
  SetupKOObject(ViewModel, null, SetupCallback);
}


//ViewModel 
var ViewModelBase = function () {
  var self = this;
  this.IsLoading = ko.observable(false);
  this.CanGoBack = ko.observable(false);
  this.GoBack = function () { };
  this.GoForward = function () { };
  this.OnNavigateTo = function () { };
  this.AfterRender = function () { };
  this.RequiresLogin = function () { return false; };
  this.RequiresRole = function () { return null; };

  this.CallServerMethod = function (Method, Args, Complete) {
    if (!this.constructor.Type) {
      throw 'ViewModel Type not set.';
    }
    self.IsLoading(true);
    Singular.CallServerMethod(this.constructor.Type, Method, Args, function (Data) {
      self.IsLoading(false);
      Complete(Data);
    });
  }
  this.CallServerMethodPromise = function (Method, Args) {
    if (!(typeof Promise !== "undefined" && Promise.toString().indexOf("[native code]") !== -1)) {
      console.log('Promises are not supported :(');
      return
    }
    if (!this.constructor.Type) {
      throw 'ViewModel Type not set.';
    }
    self.IsLoading(true);
    return new Promise(function (resolve, reject) {
      Singular.CallServerMethod(this.constructor.Type, Method, Args, function (response) {
        self.IsLoading(false);
        if (response.Success) {
          resolve(response.Data)
        }
        else {
          reject(response.ErrorText)
        }
      })
    })
  }
}


//StageViewModel 
var StageViewModelBase = function () {
  var self = this;
  ko.utils.extend(this, new ViewModelBase());
  this.Stage = ko.observable(0);

  this.CanGoBack = ko.computed(function () {
    return this.Stage() > 0;
  }, self);
  this.GoBack = function () {
    self.Stage(Math.max(self.Stage() - 1, 0))
  };
  this.GoForward = function () {
    self.Stage(self.Stage() + 1)
  };

};

//Module
function ModuleObject(MainMenuName, Image, ViewModel) {
  var self = this;
  this.MainMenuName = MainMenuName;
  this.Image = ko.observable(Image);
  this.ViewModel = ViewModel;

  // commands
  this.Select = function () {
    Application.NavigateTo(self);
  }

}

//Main Application Veiw Model
var Application = (function () {
  var self = {};

  //All the modules in the app.
  self.ModuleList = ko.observableArray([]);


  self.ShowDebugInfo = ko.observable(false);

  self.AddModule = function (MainMenuName, Image, ViewModel) {
    /// <summary>Adds a menu item to the main menu</summary>
    /// <param name="Name" type="string">The name to appear on the menu item, and in the page header.</param>
    /// <param name="Image" type="string">The image to appear on the icon.</param>
    /// <param name="ViewModel" type="string">The instance of the ViewModel.</param>

    LogEvent('Adding module: ' + MainMenuName);

    var AddModule = new ModuleObject(MainMenuName, Image, ViewModel);
    self.ModuleList.push(AddModule);

    if (!self.CurrentModule()) {
      self.NavigateTo(AddModule);
    }
    return AddModule;
  }

  self.CurrentModule = ko.observable();
  var NavigatingFrom = ko.observable(null);
  var NavigatingTo = null;

  self.IsRedirected = ko.computed(function () {
    return NavigatingFrom();
  });
  self.CheckRedirect = function (Forward) {
    if (Forward) {
      if (NavigatingTo) {
        self.NavigateTo(NavigatingTo);
        return;
      }
    } else {
      if (NavigatingFrom.peek()) {
        self.NavigateTo(NavigatingFrom.peek());
        return;
      }
    }
  };

  self.LoginModule = null;

  self.NavigateTo = function (Module) {

    NavigatingFrom(null);

    //Check security
    if (Singular.UnwrapFunction(Module.ViewModel.RequiresLogin) && !Singular.Security.IsAuthenticated()) {
      //Remember where we wanted to go.
      NavigatingFrom(self.CurrentModule());
      NavigatingTo = Module;
      //Go to the login module.
      self.CurrentModule(self.LoginModule);
    } else {
      self.CurrentModule(Module);
    }
    self.CurrentModule().ViewModel.OnNavigateTo();

    if (Singular.Validation.ValidationMode == 2) {
      Singular.Validation.CheckRules(self.CurrentModule().ViewModel);
    }

  };

  self.GoBack = function () {

    if (self.CurrentModule().ViewModel.CanGoBack()) {
      self.CurrentModule().ViewModel.GoBack();
    } else {
      navigator.notification.confirm('Are you sure you want to exit?', function (arg) {
        if (parseInt(arg) == 2) {
          navigator.app.exitApp();
        }
      }, 'Exit', 'No,Yes')
    }
  };


  return self;
})();



//#endregion

//#region  Storage 

var TempStorage = {};

Singular.SaveSetting = function (Key, Value) {
  if (Singular.SupportsLocalStorage()) {
    localStorage[Key] = Value;
  } else {
    TempStorage[Key] = Value;
  }
}
Singular.GetSetting = function (Key) {
  if (Singular.SupportsLocalStorage()) {
    return localStorage[Key];
  } else {
    return TempStorage[Key];
  }
}
Singular.ClearSetting = function (Key) {
  if (Singular.SupportsLocalStorage()) {
    delete localStorage[Key];
  } else {
    delete TempStorage[Key];
  }
}

//#endregion

//#region  Security 

Singular.IdentityObject = function () {
  SetupKOObject(this, null, function (self) {

    CreateProperty(self, 'Name', '');
    CreateProperty(self, 'UserNameReadable', '');
    CreateProperty(self, 'AuthenticationToken', '');
    CreateProperty(self, 'ValidUntil', '');

  });
}

Singular.Security = (function () {
  var self = {};
  var AuthToken = '',
      ExpiryDate,
      CurrentUser = ko.observable(null),
      AfterLoginEvents = [];

  var CheckExpiry = function () {
    if (!ExpiryDate || new Date().getTime() > ExpiryDate.getTime()) {
      CurrentUser(null);
    }
  }

  self.IsAuthenticated = ko.computed(function () {
    CheckExpiry();
    return CurrentUser() != null;
  });

  self.Identity = ko.computed(function () {
    CheckExpiry();
    return CurrentUser();
  });

  self.GetAuthToken = function () {
    return AuthToken;
  }

  self.AfterLogin = function (CompleteEvent) {
    AfterLoginEvents.push(CompleteEvent);
  };

  self.LoginError = ko.observable('');
  self.IsBusy = ko.observable(false);

  function AuthInternal(UserName, Password, Remember, Refreshing, CompleteCallback) {

    self.IsBusy(true);
    Singular.AJAXCall('Login', { UserName: UserName, Password: Password, Remember: Remember, Refreshing: Refreshing }, function (args) {
      self.IsBusy(false);

      if (args.Success) {

        if (args.Data.Identity) {
          //Authenticated
          AuthToken = args.Data.AuthToken;
          ExpiryDate = new Date(args.Data.ExpiryDate);
          CurrentUser(new Singular.IdentityObject());
          KOFormatter.Deserialise(args.Data.Identity, CurrentUser());

          for (var i = 0; i < AfterLoginEvents.length; i++) AfterLoginEvents[i]();

        } else {
          //Not Authenticated
          AuthToken = '';
          CurrentUser(null);
        }
        self.LoginError(args.Data.LoginError);
        if (CompleteCallback) CompleteCallback();

        //Redirent if required.
        if (CurrentUser.peek() != null) {
          Application.CheckRedirect(true);
        }

      } else {
        //Failed
        self.LogOut();
        self.LoginError(args.ErrorText);
        if (CompleteCallback) CompleteCallback(args.ErrorText);
      }

    });
  };

  //If remember me is true, only remember the auth token.
  //The next time the app runs, it will log on in the background with the token.
  self.Authenticate = function (UserName, Password, RememberMe, CompleteCallback) {

    AuthInternal(UserName, Password, RememberMe, false, function (Data) {
      if (RememberMe) {
        Singular.SaveSetting('AuthToken', AuthToken);
      }
      if (CompleteCallback) CompleteCallback(Data);
    });
  };

  self.LogOut = function () {
    CurrentUser(null);
    AuthToken = '';
    Singular.ClearSetting('AuthToken');
  };

  self.Refetch = function () {

    if (AuthToken) {
      AuthInternal('', '', true, true);
    }

  }

  //On app startup, get the auth token from storage.
  var StoredToken = Singular.GetSetting('AuthToken');
  if (StoredToken) AuthToken = StoredToken;

  return self;
})();

//#endregion


