function Undo() {
  ViewModel.CallServerMethod('FetchList', { ShowLoadingBar: true, ListType: ViewModel.ListType() }, function (result) {
    if (result.Success) {
      ViewModel.CurrentList.Set(result.Data);
      Singular.AddMessage(0, "Undo", "Changes have been undone.").Fade(2000);
    } else {
      alert(result.ErrorText);
    }
  });
}

function Save() {
  Singular.Validation.IfValid(ViewModel, function(){
    ViewModel.CallServerMethod('SaveList', { ShowLoadingBar: true, ListType: ViewModel.ListType(), List: ViewModel.CurrentList.Serialise(true) }, function (result) {
      if (!result.ErrorText) {
        ViewModel.CurrentList.Update(result.Data);
        Singular.AddMessage(result.MessageType - 1, "Save", result.Message).Fade(2000);
      } else {
        alert(result.ErrorText);
      }
    });
  });
}