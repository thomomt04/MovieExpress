/// <reference path="Singular.Core.js" /> 
/// <reference path="Singular.Validation2.js" /> 

/*******************
Mapping Extensions
KO Object Helpers

*******************/

//By default, Objects will be dirty if they are new from the server.
Singular.DirtyIfNew = true;

//Sets up an object.
function SetupKOObject(Object, Parent, CallBack) {
  Object.SInfo = {
    Mapping: {},
    Parent: Parent,
    Properties: [],
    MarkOld: function () {
      Object.IsClientNew(false);
      Object.IsSelfDirty(Singular.DirtyIfNew ? Object.IsNew() : false); //Only mark clean if the object is old.
    },
    MarkSaved: function () {
      Object.SInfo.SuspendChanges = true;
      Object.IsNew(false);
      Object.SInfo.MarkOld();
      Object.SInfo.SuspendChanges = false;
    }
  };
  Object.GetParent = function () {
    return this.SInfo.Parent;
  };
  CreateProperty(Object, 'Guid', Singular.NewGuid());
  CreateROProperty(Object, 'IsClientNew', true);
  CreateProperty(Object, 'IsNew', true);
  CreateROProperty(Object, 'IsSelfDirty', true); //New objects are dirty until they are saved.
  
  //Tell the caller to create the properties
  if (CallBack) CallBack(Object);

  //Rule setup
  var RuleDiscoveryPhase = true;//!Singular.Validation.HasValidated();
  //if the validation has already run, then the rules will be checked below.
  Object.SInfo.RulesChecked = false;// Singular.Validation.HasValidated();

  for (var i = 0; i < Object.SInfo.Properties.length; i++) {
    var Property = Object.SInfo.Properties[i];

    //Create the functions on this property
    if (Property.Rules.length == 0) {
      Property.Rules = undefined;
    } else {
      Property.Rules.BrokenRulesString = ko.computed(function () {
        var BrokenRulesString = '';
        for (var i = 0; i < this.Rules.length; i++) {
          if (this.Rules[i].Property == this.Rules[i].AffectedProperty) {
            var Error = this.Rules[i].RuleError();
            BrokenRulesString += (BrokenRulesString == '' || Error == '' ? '' : ', ') + Error;
          }
        }
        return BrokenRulesString;
      }, Property, { deferEvaluation: true });
      Property.Rules.BrokenRulesStringIncludingSecondary = ko.computed(function () {
        var BrokenRulesString = '';
        for (var i = 0; i < this.Rules.length; i++) {
          var Error = this.Rules[i].RuleError();
          BrokenRulesString += (BrokenRulesString == '' || Error == '' ? '' : ', ') + Error;
        }
        return BrokenRulesString;
      }, Property, { deferEvaluation: true });
      Property.Rules.Severity = ko.computed(function () {
        var MaxSeverity = 4;
        for (var j = 0; j < this.Rules.length; j++) {
          if (this.Rules[j].RuleError()) MaxSeverity = Math.min(MaxSeverity, this.Rules[j].Severity());
        }
        return MaxSeverity;
      }, Property, { deferEvaluation: true });
      Property.Rules.PendingASyncOperation = ko.computed(function () {
        var InAsync = false;
        for (var i = 0; i < this.Rules.length; i++) {
          if (this.Rules[i].InAsync()) InAsync = true;
        }
        return InAsync;
      }, Property, { deferEvaluation: true });
      Property.Rules.IconClass = function () {
        return this.Severity() == 1 ? 'ImgValidation' : (this.Severity() == 2 ? 'ImgWarning' : 'ImgInfo');
      };
      Property.Rules.BorderClass = function () {
        return this.Severity() == 1 ? 'ValBorderErr' : (this.Severity() == 2 ? 'ValBorderWarn' : 'ValBorderInfo');
      };

      //Rule event handler setup
      for (var j = 0; j < Property.Rules.length; j++) {

        Property.Rules[j].SubcribeHandler = ko.computed(function () {
          if (RuleDiscoveryPhase || this.Property.AttachedTo.SInfo.SuspendChanges) {
            try {
              this.DiscoverRuleProperties();
            } catch (e) {

            }

          } else {
            this.CheckRule();
          }
        }, Property.Rules[j]);

      }
    }

  }
  RuleDiscoveryPhase = false;

  Object.IsDirty = ko.computed(function () {

    //If this object is dirty return true.
    if (Object.IsSelfDirty()) return true;

    //If not, then check if the children are dirty.
    for (prop in Object.SInfo.Mapping) {
      //ignore readonly objects.
      if (Object[prop].PropertyType == 1) {
        //check child object.
        if (Object.SInfo.Mapping[prop].IsArray) {
          if (Object[prop].IsDirty()) return true;
        } else {
          var ChildObj = ko.utils.unwrapObservable(Object[prop]);
          if (ChildObj && ChildObj.IsDirty()) return true;
        }
      }
    }

    return false;
  }, Object, { deferEvaluation: true });

  Object.IsSelfValid = ko.computed(function () {
    for (var i = 0; i < Object.SInfo.Properties.length; i++) {
      var prop = Object.SInfo.Properties[i];
      if (prop.Rules) {
        if (prop.Rules.Severity() == 1 && prop.Rules.BrokenRulesString() != '') {
          return false;
        }
      }
    };
    return true;
  }, Object, { deferEvaluation: true });

  Object.IsValid = ko.computed(function () {

    if (!Object.IsSelfValid()) {
      return false;
    }

    for (prop in Object.SInfo.Mapping) {
      var ChildObj = ko.utils.unwrapObservable(Object[prop]);
      if (ChildObj) {
        if (Object.SInfo.Mapping[prop].IsArray) {
          for (var i = 0; i < ChildObj.length; i++) {
            if (!ChildObj[i].IsValid()) return false;
          }
        } else {
          if (!ChildObj.IsValid()) return false;
        }
      }
    }
    return true;
  }, Object, { deferEvaluation: true });

  Object._Severity = function (SelfOnly) {
    var Severity = 4;
    for (var i = 0; i < Object.SInfo.Properties.length; i++) {
      var prop = Object.SInfo.Properties[i];
      if (prop.Rules) Severity = Math.min(Severity, prop.Rules.Severity());
    };

    if (!SelfOnly) {
      for (prop in Object.SInfo.Mapping) {
        var ChildObj = ko.utils.unwrapObservable(Object[prop]);
        if (ChildObj) {
          if (Object.SInfo.Mapping[prop].IsArray) {
            for (var i = 0; i < ChildObj.length; i++) {
              Severity = Math.min(Severity, ChildObj[i]._Severity());
            }
          } else if (ChildObj._Severity) {
            Severity = Math.min(Severity, ChildObj._Severity());
          }
        }
      }
    }
    return Severity;
  };

  Object.IsSelfBusy = ko.computed(function () {
    for (var i = 0; i < Object.SInfo.Properties.length; i++) {
      var prop = Object.SInfo.Properties[i];
      if (prop.Rules) {
        if (prop.Rules.PendingASyncOperation()) {
          return true;
        }
      }
    };
    return false;
  }, Object, { deferEvaluation: true });

  Object.IsBusy = ko.computed(function () {

    if (Object.IsSelfBusy()) {
      return true;
    }

    for (prop in Object.SInfo.Mapping) {
      var ChildObj = ko.utils.unwrapObservable(Object[prop]);
      if (ChildObj) {
        if (Object.SInfo.Mapping[prop].IsArray) {
          for (var i = 0; i < ChildObj.length; i++) {
            if (ChildObj[i].IsBusy()) return true;
          }
        } else {
          if (ChildObj.IsBusy()) return true;
        }
      }
    }
    return false;
  }, Object, { deferEvaluation: true });

  Object.Serialise = function (DirtyOnly) {
    if (DirtyOnly) {
      return KOFormatter.Serialise(Object);
    } else {
      return KOFormatterFull.Serialise(Object);
    }
  }
  Object.ProtectedKey = function () {
    return Object.SInfo.KeyProperty.KeyValue;
  }

  if (Object.constructor.LocalisedProperties) {
    //if the type is localised, create a localisation storage object on this object.
    Object.SInfo.LocalisedData = {};
  }

  return Object;
};

Singular.HandleResponse = function (result, Callback) {
  if (result.Success) {
    Callback();
  } else {
    Singular.OnServerMethodError('', '', result.ErrorText);
  }
}
Singular.OnServerMethodError = function(MethodName, OrigArgs, ErrorText) {
  Singular.ShowMessage('', ErrorText);
}
function SetupType(Type, TypeName) {
  Type.Type = TypeName;

  Type.CallServerMethod = function (MethodName, NamedArgs, OnComplete, SuccessOnly) {
    Singular.CallServerMethod(TypeName, MethodName, NamedArgs, function (result) {
      if (SuccessOnly) {
        if (result.Success) { OnComplete(result.Data) } else { Singular.OnServerMethodError(MethodName, NamedArgs, result.ErrorText); }
      } else {
        if (OnComplete) OnComplete(result);
      }
    });
  }

  Type.CallServerMethodPromise = function (MethodName, NamedArgs, serialiseEverything) {
    if (serialiseEverything) {
      StatelessFormatter.IncludeChildren = true;
      StatelessFormatter.IncludeClean = true;
      StatelessFormatter.IncludeCleanInArray = true;
      StatelessFormatter.IncludeCleanProperties = true;
      StatelessFormatter.IncludeIsNew = true;
    }
    return Singular.CallServerMethodPromise(TypeName, MethodName, NamedArgs);
  }

  Type.DownloadFile = function (MethodName, NamedArgs, OnComplete) {
    
    Singular.DownloadFile(TypeName, MethodName, NamedArgs, OnComplete);

  }

  //Allow the static methods to be called on the types instances
  Type.prototype.CallServerMethod = function () {
    if (Type.prototype.constructor != ViewModel.constructor) {
      arguments[1]._Instance = StatelessFormatter.Serialise(this);
    }
    Type.CallServerMethod.apply(Type, arguments);
  }
  Type.prototype.CallServerMethodPromise = function () {
    if (Type.prototype.constructor != ViewModel.constructor) {
      arguments[1]._Instance = StatelessFormatter.Serialise(this);

    }
    return Type.CallServerMethodPromise.apply(Type, arguments);
  }
  Type.prototype.DownloadFile = Type.DownloadFile;
}

Singular.LateObservable = function (obj, name, defaultValue) {
  if (obj[name] == undefined) {
    obj[name] = ko.observable(defaultValue);
  }
  return obj[name]();
}
Singular.OnAnyPropertyChanged = null;

var primitiveTypes = { 'undefined': true, 'boolean': true, 'number': true, 'string': true };

//Hack to allow cancel of value changed.
ko.observable.fn.equalityComparer = function (a, b) {

  if (this.RaiseChangeEvents() && this.OnBeforeChange != undefined) {
    //If the initial binding has taken place, and this observable has a BeforeChanged event.
    //Then call the BeforeChanged event, and if Cancel is set to true, then trick knockout and say that this value hasn't actually changed.
    var Args = { OldValue: a, NewValue: b, Cancel: false, ResetElement: true, Prop: this };
    this.OnBeforeChange(Args);
    if (Args.Cancel) {
      //Change the elements back to what they were
      if (Args.ResetElement) {
        for (var i = 0; this.BoundElements && i < this.BoundElements.length; i++) {
          if (this.BoundElements[i].value == b) this.BoundElements[i].value = a;
        }
      }
      return true;
    }

  }
  //Copied from knockout:
  var oldValueIsPrimitive = (a === null) || (typeof (a) in primitiveTypes);
  if (oldValueIsPrimitive ? (a === b) : false) {
    return true;
  } else {
    if (this.RaiseChangeEvents()) {
      if (this.AttachedTo && this.PropertyType == 1 && !this._AlwaysClean) {
        if (!this.ChildType) this.AttachedTo.IsSelfDirty(true); //mark object as dirty. if this is a child list / object, then isSelf dirty must not change.
        if (Singular.OnAnyPropertyChanged) Singular.OnAnyPropertyChanged(this, b);
      }
    }
    return false;
  }
};

ko.observable.fn.RaiseChangeEvents = function () {
  return this.AttachedTo && !this.AttachedTo.SInfo.SuspendChanges;
}

//BeforeChange Event, Raised before the property changes, can be cancelled.
ko.subscribable.fn.BeforeChange = function (callback) {
  this.OnBeforeChange = callback;
};
//OnValueChanged - Occurs when the value of the property changes.
ko.observable.fn.OnValueChanged = function (BeforeChanged, CallBack, Delay) {
  var obs = this;

  if (BeforeChanged) {
    this.BeforeChange(function (args) {
      if (obs.RaiseChangeEvents()) {
        CallBack(args);
      }
    });
  } else {
    this.subscribe(function (args) {
      if (obs.RaiseChangeEvents()) {

        if (Delay) {
          if (obs._DelayTimer) clearTimeout(obs._DelayTimer);
          obs._DelayTimer = setTimeout(function () { CallBack(args) }, Delay);
        } else {
          CallBack(args);
        }
      }
    });
  }
  return this;
};
//this is used for computed properties. The callback is only run if the return value of the computed changes.
ko.subscribable.fn.OnReturnValueChanged = function (CallBack) {
  var obs = this;
  obs._PreviousValue = obs.peek();
  this.subscribe(function (newVal) {
    if (obs._PreviousValue != newVal) {
      CallBack({ OldValue: obs._PreviousValue, NewValue: newVal });
    }
    obs._PreviousValue = newVal;
  });
};
ko.observable.fn.Nullable = function () {
  this.AllowNulls = true;
  return this;
}
ko.observable.fn.NoRules = function () {
  this._NoRules = true;
  return this;
}
ko.observable.fn.AlwaysClean = function () {
  this._AlwaysClean = true;
  return this;
}
ko.observable.fn.IsPrimary = function () {
  Singular.PrimaryProperties.push(this.PropertyName);
}


function SetupObservable(Observable, obj, PropertyName, PropertyType) {
  Observable.AttachedTo = obj;
  Observable.PropertyName = PropertyName;
  Observable.PropertyType = PropertyType ? PropertyType : 1;
  Observable.Rules = [];
  Observable.Rules.PendingRefresh = false;
  Observable.BoundElements = [];

  //if (Observable.PropertyType == 1) {
  obj.SInfo.Properties.push(Observable);
  //}
}


//Registers an observable that will contain a typed object, or typed array.
//Adds an entry to ko.mapping so that it creates objects of the correct type when creating the view model.
function CreateTypedROProperty(obj, PropertyName, ChildType, IsArray, CreateNew) {
  return CreateTypedProperty(obj, PropertyName, ChildType, IsArray, CreateNew, 2);
}
function CreateTypedProperty(obj, PropertyName, ChildType, IsArray, CreateNew, PropertyType) {

  var obs;

  if (IsArray) {
    obs = ko.observableArray();
    obs.ProcessOptions = { SortOptions: ko.observable({ SortProperty: '', SortAsc: true }) };
    if (PropertyType == 1 || PropertyType == undefined) {
      //if this is a writeable array, then create a deleted list.
      obs.DeletedList = ko.observableArray([]);
    }
    obs.IsDirty = ko.computed(function () {
      if (this.DeletedList && this.DeletedList().length > 0) return true;
      var innerList = obs();
      for (var i = 0; i < innerList.length; i++) {
        if (innerList[i].IsDirty()) return true;
      }
      return false;
    }, obs, { deferEvaluation: true });
  } else {
    var Child = null;
    if (CreateNew && !obj.SInfo.SuspendChanges) {
      //if the object must be created new, then set a new object as the default value.
      Child = new ChildType(obj);
    }
    obs = ko.observable(Child);
    obs.Clear = function () {
      var obj = this();
      MarkObjectDeleted(obj);
      this(null);
      //Singular.Validation.Validate();
    }

  }

  //For single objects, creates a new object of the correct type if null is passed in.
  //If an object is passed in then we check if its a proper KO object, or a js object.
  //If a js object, then we deserialise it before setting the property.
  obs.Set = function (NewObj, ClearArray) {
    if (ClearArray === undefined) ClearArray = true;
    if (!IsArray) {
      if (NewObj === undefined) {
        NewObj = obj.SInfo.Mapping[PropertyName].create();
        obs(NewObj);
      } else if (NewObj && ChildType == NewObj.constructor) {
        NewObj.__ContainerProperty = obs;
        obs(NewObj);
      } else {
        KOFormatter.Deserialise(NewObj, obs, !ClearArray);
        NewObj = obs.peek();
      }

      if (NewObj && !NewObj.SInfo.RulesChecked) {
        Singular.Validation.CheckRules(NewObj);
      }
      return NewObj;
    } else {
      //if ((ClearArray == undefined || ClearArray) && obs().length) obs([]);
      KOFormatter.Deserialise(NewObj, obs, !ClearArray);
      if (NewObj.length > 0) Singular.Validation.CheckRules(obs.peek()[0].GetParent());
    }

  }
  obs.Update = function (NewObj) {
    //TODO: this wont work any more with deserialisemode = stateless gone.
    obs.Set(NewObj, false);
  }

  obs.Create = function () {
    var NewObj = obj.SInfo.Mapping[PropertyName].create();
    Singular.Validation.CheckRules(NewObj);
    return NewObj;
  }
  obs.Serialise = function (DirtyOnly) {
    if (DirtyOnly) {
      return KOFormatter.Serialise(obs);
    } else {
      return KOFormatterFull.Serialise(obs);
    }
  }

  obs.IsBusy = ko.observable(false);
  var StartingBusy = false;
  obs.StartBusy = function (Delay) {
    StartingBusy = true;
    setTimeout(function () {
      if (StartingBusy) {
        StartingBusy = false;
        obs.IsBusy(true);
      }
    }, Delay ? Delay : 200)
  }
  obs.EndBusy = function () {
    StartingBusy = false;
    obs.IsBusy(false);
  }

  obj[PropertyName] = obs;
  SetupObservable(obs, obj, PropertyName, PropertyType);

  if (ChildType) {
    obj[PropertyName].ChildType = ChildType;
    obj.SInfo.Mapping[PropertyName] = { IsArray: IsArray };
    obj.SInfo.Mapping[PropertyName].create = function () {
      return CreateChild(ChildType, obj[PropertyName]);
    };
  }
  return obs;

};
function CreateChild(ChildType, Property) {
  var ParentObj = Property.AttachedTo;
  var ParentList = Property.peek();

  var NewObj = new ChildType(ParentObj);

  if (ParentList instanceof Array) {
    NewObj.SInfo.ParentList = Property;
  }
  NewObj.__ContainerProperty = Property;
  return NewObj;
}
//Registers an observable on the object, and adds a reference to the object the observable is attached to.
function CreateKeyProperty(obj, PropertyName, IsSafe) {
  var Key = CreatePropertyN(obj, PropertyName, null);
  Key.IsSafeKey = IsSafe;
  obj.SInfo.KeyProperty = Key;
  return Key;
};

function CreateProperty(obj, PropertyName, DefaultValue) {
  return RegisterObservable(obj, PropertyName, 1, 's', DefaultValue);
};
function CreatePropertyN(obj, PropertyName, DefaultValue) {
  return RegisterObservable(obj, PropertyName, 1, 'n', DefaultValue);
};
function CreatePropertyB(obj, PropertyName, DefaultValue) {
  return RegisterObservable(obj, PropertyName, 1, 'b', DefaultValue);
};
function CreatePropertyD(obj, PropertyName, DefaultValue) {
  return RegisterObservable(obj, PropertyName, 1, 'd', DefaultValue);
};
function CreateROProperty(obj, PropertyName, DefaultValue) {
  return RegisterObservable(obj, PropertyName, 2, 's', DefaultValue);
};
function CreateROPropertyN(obj, PropertyName, DefaultValue) {
  return RegisterObservable(obj, PropertyName, 2, 'n', DefaultValue);
};
function CreateROPropertyB(obj, PropertyName, DefaultValue) {
  return RegisterObservable(obj, PropertyName, 2, 'b', DefaultValue);
};
function CreateROPropertyD(obj, PropertyName, DefaultValue) {
  return RegisterObservable(obj, PropertyName, 2, 'd', DefaultValue);
};
function CreateComputedProperty(obj, PropertyName, Body) {
  obj[PropertyName] = ko.computed(Body);
  obj[PropertyName].AttachedTo = obj;
  obj[PropertyName].PropertyName = PropertyName;
  return obj[PropertyName];
};
function RegisterObservable(obj, PropertyName, PropertyType, DataType, DefaultValue) {
  var obs = ko.observable(DefaultValue);
  obj[PropertyName] = obs;
  SetupObservable(obs, obj, PropertyName, PropertyType);
  obs.DataType = DataType;

  //Raise global Value changed event, if handled.
  if (PropertyType == 1 && Singular.AnyValueChanged) {
    obs.subscribe(function (args) {
      if (obs.RaiseChangeEvents()) {
        Singular.AnyValueChanged(obj[PropertyName], args);
      }
    });
  };

  return obs;
};
//Checks if the item can be deleted by asking the server view model. Adds the item to the deleted list, and removes the item.
ko.observableArray.fn.Remove = function (Item, Event, options) {
  var Self = this;

  var Prompt = '';
  if (Event && Event.currentTarget) {
    Prompt = $(Event.currentTarget).attr('data-prompt');
  }
  if (!Prompt && Singular.DeleteMode == 2) {
    Prompt = 'Are you sure you want to permanently delete this item?';
  }

  Singular.CanDeleteItem(Item, Prompt, function () {
    Self.RemoveNoCheck(Item, options);
  }, Singular.DeleteMode == 2);

};
ko.observableArray.fn.RemoveNoCheck = function (Item, options) {
  if (Singular.DeleteMode == 1) {
    //for delete immediate, dont mark dirty cause the list doesnt need to be saved, the object is gone from the database.
    if (this.DeletedList) this.DeletedList.push(Item);
    Item.IsSelfDirty(true);
  }
  this.remove(Item);
  MarkObjectDeleted(Item);
  if (options && options.afterItemRemoved) {
    options.afterItemRemoved.call(this, Item)
  }
};
//Removes all items in the list and adds them to the deleted list.
ko.observableArray.fn.DeleteAll = function () {
  var arr = this();
  for (var i = arr.length - 1; i >= 0; i--) {
    this.RemoveNoCheck(arr[i]);
  }
};
ko.observableArray.fn.RemoveSelectedItem = function (Item) {
  if (Singular.DeleteMode == 1) {
    //for delete immediate, dont mark dirty cause the list doesnt need to be saved, the object is gone from the database.
    if (this.DeletedList) this.DeletedList.push(Item);
    Item.IsSelfDirty(true);
  }
  this.remove(Item);
};
MarkObjectDeleted = function (obj) {
  if (obj == undefined) return;
  obj.__Deleted = true;
  for (var name in obj.SInfo.Mapping) {

    var Val = obj[name]();
    if (obj.SInfo.Mapping[name].IsArray) {
      //Array
      for (var i = 0; i < Val.length; i++) {
        MarkObjectDeleted(Val[i]);
      }
    } else {
      //Object     
      MarkObjectDeleted(Val)
    };

  };
  Singular.PendingFileOperations.RemoveItem(obj);
  Singular.Validation.CheckRules(obj.GetParent());

};
//Creates an object of the correct type and Adds it.
ko.observableArray.fn.AddNew = function (DontCheckRules) {
  var ChildObj = CreateChild(this.ChildType, this);
  this.push(ChildObj);

  if (!this.AttachedTo.SInfo.SuspendChanges && !DontCheckRules && !ChildObj.SInfo.RulesChecked) {
    Singular.Validation.CheckRules(ChildObj, true);
  }

  return ChildObj;
}
//Item can be a KO object or JS object. Check rules defaults to true and only applies if the object is a plain JS object.
ko.observableArray.fn.Add = function (Item, CheckRules) {
  if (!Item.SInfo) {
    var NewItem = CreateChild(this.ChildType, this);
    KOFormatter.Deserialise(Item, NewItem);
    NewItem.IsClientNew(true);
    if (CheckRules !== false) Singular.Validation.CheckRules(NewItem);
    Item = NewItem;
  }
  Item.SInfo.Parent = this.AttachedTo;
  this.push(Item);
  return Item;
}

function AddRule(Property, RuleHandler, Arguments, AffectedProperties) {

  if (typeof (RuleHandler) == 'string') {
    //Built in rule handlers.
    RuleHandler = Singular.Validation.Rules[RuleHandler];
  };

  //Create the rule and add it to the properties rules array.
  var Rule = new Singular.Validation.RuleObject(Property, RuleHandler, Arguments, Property);
  Property.Rules.push(Rule);

  if (AffectedProperties && AffectedProperties.length > 0) {
    for (var i = 0; i < AffectedProperties.length; i++) {
      var SecondaryRule = new Singular.Validation.RuleObject(Property, RuleHandler, Arguments, AffectedProperties[i]);
      AffectedProperties[i].Rules.push(SecondaryRule);
    }
  }

};
Singular.GetRootObject = function (Object) {

  var Parent = Object.GetParent();
  while (Parent) {
    Object = Parent;
    Parent = Parent.GetParent();
  }
  return Object;

};

function ToStringHelper(Obj, Text, Name) {
  if (Text.trim() == '') {
    if (Obj.IsNew()) {
      return 'New ' + Name;
    } else {
      return 'Blank ' + Name;
    }
  } else {
    return Text;
  }
};


/**************************************
Simpler and better JSON de-serialise
Uses above functions to create objects
with correct type etc.
*************************************/

var KOFormatterObject = function () {
  var self = {};
  var private = {};

  //Options
  self.IncludeChildren = true;
  self.IncludeIsNew = Singular.IsStateless;
  self.IncludeClean = !Singular.IsStateless; //Not used anymore, use IncludeCleanInArray
  self.IncludeCleanProperties = !Singular.IsStateless;
  self.IncludeCleanInArray = false;
	
  self.Deserialise = function (From, To, Update) {
    private.GuidHashTable = {};
    if (From == null) {
      To(null);
    } else {
      if (From.Guid) {
        private.GuidHashTable[From.Guid] = true;
      }
      //Allow Deserialising into a property instead of an object.
      if (typeof (To) == 'function') {
        NewFrom = {};
        NewFrom[To.PropertyName] = From;
        From = NewFrom;
        To = To.AttachedTo;
      }
      private.DeserialiseInternal(From, To, Update);
    }
  };

  private.DeserialiseInternal = function (From, To, Update) {

    To.SInfo.SuspendChanges = true;
    for (var name in From) {

      if (To[name]) {
        if (name != 'IsDirty' /*&& name != 'IsNew'*/ && ko.isWriteableObservable(To[name])) {
          //Make sure its observable

          if (From[name] instanceof Array) {
            //Array
            var CreateObject = To.SInfo.Mapping[name] ? To.SInfo.Mapping[name].create : null;
            private.DeserialiseArray(From[name], To[name], CreateObject, Update);

          } else if (From[name] instanceof Object) {
            //Object     
            var ToObject = To[name]();
            var Created = false;
            var Deserialise = true;
            //Check if the from object is a reference to an existing object
            if (From[name]._Reference) {
              Created = true;
              Deserialise = false;
              ToObject = private.GuidHashTable[From[name].Guid];
            }
            if (!To.SInfo.Mapping[name]) {
              //not 'strongly typed' property, just set the plain obj later
              ToObject = From[name];
              Created = true;
              Deserialise = false;
            }
            //if the to value is null, then create the object.
            if (ToObject == null) {
              ToObject = To.SInfo.Mapping[name].create();
              //ToObject.IsNew = false;
              Created = true;
            }
            if (Deserialise) {
              private.DeserialiseInternal(From[name], ToObject, Update);
            }

            if (Created) {
              To[name](ToObject);
            }
            if (Deserialise && ToObject.Guid()) {
              private.GuidHashTable[ToObject.Guid()] = ToObject;
            }

          } else {
            //Simple
            To[name](From[name]);
            if (To[name].IsSafeKey) To[name].KeyValue = From._Key;
          };
        };
      } else {
        //There is no observable, just create a plain field.
        To[name] = From[name];
      }
      
    };

		//if (From.IsNew === false) To.IsNew(false);
    if (From.IsSelfDirty === false) {
      To.SInfo.MarkOld();
    } else {
      To.IsClientNew(false);
    }

    if (From.__LocalisedData) To.SInfo.LocalisedData = From.__LocalisedData;

    To.SInfo.SuspendChanges = false;
  };

  private.DeserialiseArray = function (From, To, Create, Update) {

    var ToArray = To();
    var ToIndex = {};

    if (Create) {
      for (var i = 0; i < ToArray.length; i++) {
        ToArray[i].__DontDelete = undefined;
        ToIndex[ToArray[i].Guid.peek()] = ToArray[i];
      }
    } else {
      //for simple arrays, just clear the to array.
      if (ToArray) {
        ToArray.length = 0;
      } else {
        //if this is not a proper array property, it will not have a value.
        //just set the JS array as the value.
        To(From);
        return;
      }
    }


    for (var i = 0; i < From.length; i++) {

      if (Create) {
        //Working with Proper object
        var FoundItem = null;
        var Created = false;
        var Deserialise = true;

        FoundItem = ToIndex[From[i].Guid];

        if (FoundItem) {
          //Mark the item so that it is not deleted later.
          FoundItem.__DontDelete = true;
        } else {
          //Check if the from object is a reference to an existing object
          if (From[i]._Reference) {
            FoundItem = private.GuidHashTable[From[i].Guid];
            FoundItem.__DontDelete = true;
            Created = true;
            Deserialise = false;
          } else {
            //If it does not, create it.
            FoundItem = Create();
            FoundItem.__DontDelete = true;
            Created = true;
          }
        }

        if (Deserialise) {
          //Populate the new / existing object
          private.DeserialiseInternal(From[i], FoundItem, Update);
        }

        if (i == 0 && From[i].__TotalRows) {
          To.TotalRows = From[i].__TotalRows;
        }

        //Add the item to the list after its been populated.
        if (Created) {
          if (From[i]._AddToTop) {
            ToArray.splice(0, 0, FoundItem);
          } else {
            ToArray.push(FoundItem);
          }
					
          ToIndex[FoundItem.Guid.peek()] = FoundItem;
        }
        private.GuidHashTable[FoundItem.Guid()] = FoundItem;
      } else {
        //Working with simple array.
        ToArray.push(From[i]);
      }
    };

    //go through the To List, every item that isn't marked, doesn't have an entry in the from list, and must be deleted.
    if (Create) {
      for (var i = ToArray.length - 1; i >= 0; i--) {
        if (ToArray[i].__DontDelete == undefined) {
          private.GuidHashTable[ToArray[i].Guid()] = false;
          if (!Update) {
            //If updating, dont remove old items that werent in the new array.
            ko.utils.arrayRemoveItem(ToArray, ToArray[i]);
          }
        }
      };
    }

    if (To.DeletedList) {
      To.DeletedList([]);
    }
    To.valueHasMutated();
  };

  private.SerialiseInternal = function (From, To) {

    if (From.Guid) {
      if (private.GuidHashTable[From.Guid.peek()]) {
        //This object has been serialised already, reference it with the guid.
        To.Guid = From.Guid.peek();
        To._Reference = true;
        return true;
      } else {
        //record that this object has been serialised.
        private.GuidHashTable[From.Guid.peek()] = From;
      }
    }
       
    var IsPlain = From.SInfo === undefined;

    if (!IsPlain && From.SInfo.LocalisedData) To.__LocalisedData = From.SInfo.LocalisedData;

    for (var name in From) {
      var IsObs = ko.isObservable(From[name]);

      if (IsPlain || (IsObs && !name.indexOf('__') == 0 && From[name].PropertyType == 1)
        || (name == 'IsNew' && self.IncludeIsNew) || (name == 'IsSelfDirty' && self.IncludeIsNew)) {

        var Value = ko.utils.peekObservable(From[name]);
        var ToName = name == 'IsSelfDirty' ? 'IsDirty' : name;

        if (Value instanceof Array) {
          //Array

          if (self.IncludeChildren) {
            //Check if the array has a deleted list, and if there are any items to delete.
            var DeletedInfoList = [];
            if (From[name].DeletedList) {
              var InnerList = From[name].DeletedList.peek();
              for (var i = 0; i < InnerList.length; i++) {
                //ignore new clientside items that have been deleted
                if (!InnerList[i].IsClientNew.peek()) {

                  //send the full object in the deletedlist if the setting is enabled
                  if (Singular.SendFullDeletedObjects) {
                    var FullDelInfo = KOFormatter.Serialise(InnerList[i]),
                        FullKeyProp = InnerList[i].SInfo.KeyProperty;
                    FullDelInfo.ID = (FullKeyProp.KeyValue ? FullKeyProp.KeyValue : ko.utils.peekObservable(FullKeyProp));
                    FullDelInfo._IsFullObject = true
                    DeletedInfoList.push(FullDelInfo);
                  }
                  else {
                    //otherwise, send only the guid and ID
                    var DelInfo = { Guid: InnerList[i].Guid.peek() },
							          KeyProp = InnerList[i].SInfo.KeyProperty;
                    if (KeyProp) {
                      DelInfo.ID = KeyProp.KeyValue ? KeyProp.KeyValue : ko.utils.peekObservable(KeyProp);
                    }
                    DeletedInfoList.push(DelInfo);
                  }

                }
              }
            }

            //If there are items to delete, create an object to hold the array items, and deleted list.
            var ToArray = [];
            if (DeletedInfoList.length > 0) {
              To[name] = { Deleted: DeletedInfoList, Items: ToArray };
            } else {
              To[name] = ToArray;
            }
            //Dont include clean items in an array.
            //var OldIncludeClean = self.IncludeClean;
            //self.IncludeClean = self.IncludeCleanInArray;

            for (var i = 0; i < Value.length; i++) {
              if (Value[i] instanceof Object && !(Value[i] instanceof Date)) {

                if (!Value[i].IsDirty || Value[i].IsDirty() || self.IncludeCleanInArray) {
                  var Obj = {};
                  private.SerialiseInternal(Value[i], Obj)
                  ToArray.push(Obj);
                }

              } else {
                ToArray.push(Value[i]);
              }
            }
            //self.IncludeClean = OldIncludeClean;
          }
        } else if (Value instanceof Date) {
          //Date
          To[name] = Value.format('dd MMM yyyy HH:mm:ss');

        } else if (Value instanceof Object) {
          //Object     
          if (self.IncludeChildren) {
            var Obj = {};
            private.SerialiseInternal(Value, Obj)
            To[name] = Obj;
          }

        } else {
          //Simple
          if (IsObs && From[name].IsSafeKey) {
            //protected key.
            if (!From.IsNew.peek()) {
              To[name] = From[name].KeyValue;
            }
          } else {
            To[ToName] = Value;
          }
						
        };
      }
    }

  }

  self.Serialise = function (ViewModel, /* optional */To) {
    private.GuidHashTable = {};

    if (To == null) {
      To = {};
    }
    if (ko.isObservable(ViewModel)) {
      var From = { Obj: ViewModel };
      From.PropertyType = 1;
      var TempObj = self.Serialise(From);
      return TempObj.Obj;
    } else if (ViewModel instanceof Array) {
      //For arrays, create a dummy base object, and serialise that. 
      var From = { List: ko.observableArray(ViewModel) };
      From.List.PropertyType = 1;
      var TempObj = self.Serialise(From);
      return TempObj.List;
    }

    private.SerialiseInternal(ViewModel, To);
    return To;

  };

  return self;
}

//Global serialiser / deserialiser.
var KOFormatter = new KOFormatterObject();
var StatelessFormatter = new KOFormatterObject();
StatelessFormatter.IncludeIsNew = true;
//StatelessFormatter.IncludeClean = false;
StatelessFormatter.IncludeCleanProperties = false;

var KOFormatterFull = new KOFormatterObject();
KOFormatterFull.IncludeIsNew = true;
//KOFormatterFull.IncludeClean = true;
KOFormatterFull.IncludeCleanInArray = true;

var ModelBinder = KOFormatter; //used to be called this.
