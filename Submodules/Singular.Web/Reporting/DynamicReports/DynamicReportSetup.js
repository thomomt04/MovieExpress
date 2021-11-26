function AddReport(parent) {
  if (ViewModel.IsValid()) {
    ViewModel.EditReport.Set(new ReportObject(parent));
  } else {
    alert('Please fix any validation errors first.');
  }
}
function EditReport(Report) {
  if (ViewModel.IsValid()) {
    ViewModel.EditReport(Report);
    Report.ReportParameterList([]);
    GetReport(Report.DynamicReportID(), Report.StoredProcedureName());
  } else {
    alert('Please fix any validation errors first.');
  }
}

function SelectProc() {
  var rpt = ViewModel.EditReport();
  if (ViewModel.IsValid()) {
    GetReport(0, rpt.StoredProcedureName());
  } else {
    alert('Please select a data source.');
  }
}

function GetReport(ReportID, ProcName) {
  Singular.ShowLoadingBar();
  Singular.GetDataStateless('Singular.Reporting.Dynamic.ReportList, Singular.Silverlight', { ReportID: ReportID, ObjectName: ProcName }, function (result) {
    Singular.HideLoadingBar();
    if (result.Success) {
      ViewModel.EditReport.Set(result.Data[0]);
      if (ReportID == 0) ViewModel.EditReport().IsSelfDirty(true);
    } else {
      alert('Error getting report properties');
    }
  });
}

function SaveReport() {
  var rpt = ViewModel.EditReport();
  if (rpt.IsDirty()) {
    //var WasNew = rpt.DynamicReportID() == 0;
    var grp = rpt.GetParent();
    if (ViewModel.IsValid()) {

      Singular.ShowLoadingBar();
      ViewModel.CallServerMethod('SaveReport', { Report: rpt.Serialise(), GroupID: grp.DynamicReportGroupID(), GroupName: grp.GroupName() }, function (result) {
        Singular.HideLoadingBar();
        if (result.Success) {
          ViewModel.EditReport.Set(result.Data);
          grp.DynamicReportGroupID(result.Data.DynamicReportGroupID);
          if (grp.ReportList().Find('DisplayName', ViewModel.EditReport().DisplayName()) == null) {
            grp.ReportList.push(ViewModel.EditReport());
          }
          ViewModel.EditReport.Clear();
        } else {
          alert('Error saving report.');
        }
      });

    } else {
      alert('Please fix all validation errors first.');
    }
  } else {
    ViewModel.EditReport.Clear();
  }
}

function SaveAll() {
  if (ViewModel.IsValid()) {

    Singular.ShowLoadingBar();
    ViewModel.CallServerMethod('SaveGroups', { GroupList: KOFormatter.Serialise(ViewModel.ReportGroupList) }, function (result) {
      Singular.HideLoadingBar();
      if (result.Success) {
        KOFormatter.Deserialise(result.Data, ViewModel.ReportGroupList);
        Singular.AddMessage(3, 'Saved', 'Saved Successfully').Fade(1000);
      } else {
        alert('Error saving.');
      }
    });

  } else {
    alert('Please fix all validation errors first.');
  }
}

function GetDefinedDefaults(Param) {
  var fList = [];
  ClientData.SpecialDefaultValue.Iterate(function (Item) {
    if (Param.ParamDataType() == 'n' && Item.Val >= 100 && Item.Val < 200) {
      fList.push(Item);
    } else if (Param.ParamDataType() == 'd' && Item.Val >= 200 && Item.Val < 300) {
      fList.push(Item);
    }
  });
  return fList;
}

function OnDrop(from, to) {

  if (from.GetParent() != to) {
    from.IsSelfDirty(true);
    from.GetParent().ReportList.remove(from);
    to.ReportList.Add(from);
    to.IsExpanded(true);
  }
}

function DragHover(e, ui, from, to) {
  ui.helper.html('Move "' + from.DisplayName() + '" to group "' + to.GroupName() + '"');
}

function GroupRenamed(Group) {
  if (Group.Auto()) {
    Group.ReportList().Iterate(function (Report) {
      Report.IsSelfDirty(true);
    });
  }
}