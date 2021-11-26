function RunAllCheckQueries() {

  var OnProgress = function (ap) {
    ViewModel.Status(ap.CurrentStatus);
    if (ap.StreamData) {
      ap.StreamData.Iterate(function (Item) {
        ViewModel.CheckQueryList().Find('Description', Item.Item1).Status(Item.Item2);
      });
    }

  }

  ViewModel.IsLoading(true);
  ViewModel.CallServerMethod('RunAllCheckQueries', { OnProgress: OnProgress }, function (result) {
    OnProgress(result.Data);
    ViewModel.IsLoading(false);
    ViewModel.Status('');
  });
}

function RunCheckQuery(CQ) {
  ViewModel.CallServerMethod('RunCheckQuery', { ShowLoadingBar: true, Description: CQ.Description() }, function (result) {
    CQ.Status(result.Data);
  });
}

function ViewCQ(cq) {
  ViewModel.CurrentCQ(cq.Name());
  var cqr = ViewModel.CQResult();
  cqr.FriendlyName(cq.Name());
  cqr.GetDataParams(cq.Description());
  cqr.FetchData();
  
}

function CloseCQView() {
  ViewModel.CurrentCQ('');
}
