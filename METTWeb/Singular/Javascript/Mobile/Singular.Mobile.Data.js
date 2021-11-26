Singular.DataUrl = 'http://localhost/';


Singular.ServiceTypes = {
  WCF: 1,
  HTTPHandler: 2
}
Singular.ServiceType = Singular.ServiceTypes.WCF;

Singular.AJAXCall = function (Method, Args, OnComplete) {

  var PostUrl = Singular.DataUrl;
  
  if (Singular.ServiceType == Singular.ServiceTypes.WCF) {
    PostUrl += 'DataService.svc/' + Method;
  } else {
    PostUrl += 'StatelessHandler.ashx';
    Args.Method = Method;
  }
  
  var JSonArgs = JSON.stringify(Args);

  //LogEvent('Sending ' + JSonArgs.length + ' bytes of data');

  var CompleteArgs = { Data: null, Success: true, ErrorText: '' };
  
  $.ajax({
    type: "POST",
    url: PostUrl,
    data: JSonArgs,
    timeout: 45000,
    headers: { 'AuthToken': Singular.Security.GetAuthToken() },
    success: function (data, textStatus, jqXHR) {

      //LogEvent('Received ' + JSON.stringify(data).length + ' bytes of data');

      //if the data returns an object that looks like CompleteArgs, then return that object.
      //otherwise, return the completeargs object, after setting the data property.
      if (data != null && data.Success !== undefined && data.ErrorText !== undefined) {
        OnComplete(data);
      } else {
        CompleteArgs.Data = data;
        OnComplete(CompleteArgs);
      }

    },
    error: function (jqXHR, textStatus, error) {
            
      console.log('Error getting data on method "' + Method + '": ' + error + ' - ' + jqXHR.responseText);

      if (error == '' && jqXHR.responseText == '') {
        error = 'Connection Failed';
      } else {
        if (jqXHR.responseText) {
          error += ' - ' + jqXHR.responseText
        }
      }

      CompleteArgs.Success = false;
      CompleteArgs.ErrorText = error;
      OnComplete(CompleteArgs);
      
    }
  });

};
Singular.GetData = function (Type, Args, OnComplete) {
  
  Singular.AJAXCall('GetData', { Type: Type, Args: (!Args ? {} : Args) }, OnComplete);

};
Singular.SaveData = function (ObjectToSave, Args, OnComplete) {
  
  var BaseType;
  //Get the type of the object.
  if (ObjectToSave instanceof Array) {
    if (ObjectToSave.length == 0) {
      return;
    } else {
      BaseType = ObjectToSave[0].constructor.Type;
    }
  } else {
    BaseType = ObjectToSave.constructor.Type;
  }

  var SaveArgs = {
    Type: BaseType,
    Args: Args,
    Object: KOFormatter.Serialise(ObjectToSave)
  }
  //Tell the server to save this object.
  Singular.AJAXCall('SaveData', SaveArgs, function (args) {

    //If the server succeeded, then populate the client object with the new server object.
    if (args.Success && args.SavedObject) {
      KOFormatter.Deserialise(args.SavedObject, ObjectToSave);
    }
    //Complete handler
    if (OnComplete) {
      OnComplete(args);
    }
  });

}
Singular.CallServerMethod = function (Type, Method, Args, OnComplete) {
  
  Singular.AJAXCall('Command', { Type: Type, CallMethod: Method, Args: Args }, OnComplete);

}