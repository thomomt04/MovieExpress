// Singular Systems JavaScript Library.
// This file contains code relating to both the ASPNet library, and the Mobile Library.

//Singular Namespace.
var Singular = (function () {

  var self = {};
  self.IsStateless = true;
  //self.DeserialiseMode = 1; //1=Normal (Delete items not returned from server. //2=Stateless (Leave items, only update / add items returned from server).
  self._pvt = {}; //'private' variables used outside this file.

  //Creates 4 random characters.
  var S4 = function () {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
  };

  //Creates a random string in GUID format.
  self.NewGuid = function () {
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
  };

  self.ConvertToDate = function (DateAsJSon) {
    if (DateAsJSon == undefined)
      return null;
    else
      return new Date(parseInt(DateAsJSon.match(/\d+/)[0]));
  };

  self.DateMonthStart = function (date) {
    return new Date(date.getFullYear(), date.getMonth(), 1);
  };
  self.DateMonthEnd = function (date) {
    return new Date(date.getFullYear(), date.getMonth() + 1, 0);
  }

  var _SupportsLocalStorage;
  self.SupportsLocalStorage = function () {
    if (_SupportsLocalStorage == undefined) {
      _SupportsLocalStorage = false;
      try {
        if (typeof (Storage) !== "undefined") {
          localStorage.setItem('lsTest', 1);
          localStorage.removeItem('lsTest');
          _SupportsLocalStorage = true;
        }
      } catch (e) {
      }
    }
    return _SupportsLocalStorage;
  }

  self.TryStoreLocal = function (key, value) {
    if (self.SupportsLocalStorage()) {
      localStorage.setItem(key, value);
    } else {
      sessionStorage.setItem(key, value);
    }
  }

  self.TryRetrieveLocal = function (key) {
    if (self.SupportsLocalStorage()) {
      return localStorage.getItem(key);
    } else {
      return sessionStorage.getItem(key);
    }
  }

  self.lzw_encode = function (s) {
    var dict = {};
    var data = (s + "").split("");
    var out = [];
    var currChar;
    var phrase = data[0];
    var code = 256;
    for (var i = 1; i < data.length; i++) {
      currChar = data[i];
      if (dict[phrase + currChar] != null) {
        phrase += currChar;
      }
      else {
        out.push(phrase.length > 1 ? dict[phrase] : phrase.charCodeAt(0));
        dict[phrase + currChar] = code;
        code++;
        phrase = currChar;
      }
    }
    out.push(phrase.length > 1 ? dict[phrase] : phrase.charCodeAt(0));
    for (var i = 0; i < out.length; i++) {
      out[i] = String.fromCharCode(out[i]);
    }
    return out.join("");
  }

  self.SetSValue = function (obj, Key, Value) {

    obj.SInfo = obj.SInfo || {};
    obj.SInfo[Key] = Value;

  };

  self.GetSValue = function (obj, Key) {
    if (obj.SInfo) {
      return obj.SInfo[Key];
    }
    return null;
  };

  self.PrimaryProperties = [];

  self.UnwrapFunction = function (FunctionOrValue) {
    if (FunctionOrValue == null) return null;
    if (ko.isObservable(FunctionOrValue)) {
      return FunctionOrValue();
    } else if (typeof FunctionOrValue == 'function') {
      return FunctionOrValue.apply(window, Array.prototype.slice.call(arguments, 1));
    } else {
      return FunctionOrValue;
    }
  }
  
  self.TimeProcess = function (FunctionToCall, Description) {

    Description = Description || 'Process';
    var start = new Date();

    setTimeout(function () {

      FunctionToCall();
      var end = new Date();
      alert(Description + ' took ' + (end - start) + ' ms');
    }, 0)


  };

  self.FDTokenValue = '';
  self.OnDownloadComplete = function (CallBack) {

    var FDTimer = setInterval(function () {
      var FDTokenIndex = document.cookie.indexOf('FDToken=');
      if (FDTokenIndex >= 0) {
        var ThisFDTokenValue = document.cookie.substring(FDTokenIndex + 8, FDTokenIndex + 16);
        if (ThisFDTokenValue != Singular.FDTokenValue) {
          Singular.FDTokenValue = ThisFDTokenValue;
          window.clearInterval(FDTimer);
          CallBack();
        }

      }

    }, 500);

  };

  self.HasAccess = function (Role) {
    if (self.ClientRoles) {
      return self.ClientRoles.indexOf(Role) >= 0;
    }
  };

  self.FilterSecurityGroupList = function (List) {
    //This function can be overridden in the users page.
    return List;
  }

  self.Log = '';
  self.AddLogEntry = function (Text) {
    Singular.Log += '\n' + Text;
  }

  self.SendFullDeletedObjects = false;

  return self;

})();

var ObjectSchema = (function () {
  var self = {};

  var SchemaList = {};
  self.Register = function (TypeName, SchemaObject) {
    SchemaList[TypeName] = SchemaObject;
  }
  self.Get = function (TypeName) {
    return SchemaList[TypeName]
  }
  self.Apply = function (TypeName, List) {
    var Schema = SchemaList[TypeName];
    if (Schema) {
      List.Properties = [];
      for (var key in Schema.Properties) {
        var prop = Schema.Properties[key];
        prop.Name = key;
        if (prop.Display === undefined) prop.Display = key;
        if (prop.Type === undefined) prop.Type = 's';
        if (prop.IDProperty) List.IDProperty = key;
        List.Properties.push(prop)
      }
      if (!List.IDProperty) List.IDProperty = List.Properties[0].Name;
    }
  }

  return self;
})();


Singular.FilterList = function (List, PropertyName, FilterValue) {

  var Ret = [];

  if (List.length > 0 && typeof List[0][PropertyName] == 'function') {
    for (var i = 0; i < List.length; i++)
      if (List[i][PropertyName]() == FilterValue)
        Ret.push(List[i]);
  } else {
    for (var i = 0; i < List.length; i++)
      if (List[i][PropertyName] == FilterValue)
        Ret.push(List[i]);
  }
  return Ret;
};
//Allows text to be selected on focus without chrome unselecting it again.
Singular.OnFocus = function (element, OnFocus) {

  var MouseFocus = false;
  $(element).mousedown(function () {
    if (document.activeElement != element) MouseFocus = true;
  });
  $(element).focus(function () {
    OnFocus();
  });
  $(element).mouseup(function (e) {
    if (MouseFocus) e.preventDefault();
    MouseFocus = false;
  });

};

window.HasAccess = Singular.HasAccess;



var LocalText = function (Key) {
  var Translated = LocalStrings[Key];
  if (Translated && Translated.length > 0) {
    if (arguments.length > 1) {
      return Translated.format.apply(Translated, Array.prototype.slice.call(arguments, 1));
    } else {
      return Translated;
    }
  } else {
    return Key;
  }
}

var LocalStrings = (function () {
  var self = {};
  self.Set = function (Key, Value) {
    self[Key] = Value;
  }
  //[JS Key Definition Start]
  self.Set('Submit', 'Submit')
  self.Set('JSValidation', 'Validation');
  self.Set('JSValidationPrompt', 'Please make sure all validation errors are fixed before you continue.');
  self.Set('JSValidationError', 'Please fix the following validation errors:');
  self.Set('JSValidationError2', 'Fix all validation errors first.');
  self.Set('JSValidationError3', 'Please wait for all files to upload.');
  self.Set('JSValidationSuccess', 'Validation Successful');
  self.Set('Yes', 'Yes');
  self.Set('No', 'No');
  self.Set('Cancel', 'Cancel');
  self.Set('OldBrowser', 'You are using an old browser. Some functionality may not work correctly. Please <a href="http://www.whatbrowser.org/">upgrade</a>.');
  self.Set('Combo_Choose', 'Choose');
  self.Set('ASyncRule', 'Checking Rule...');
  self.Set('ASyncRules', 'Checking Rules...');
  self.Set('PleaseNote', 'Please Note');
  
  //[JS Key Definition End]
  return self;
})();


