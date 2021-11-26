function GetSecurityRoles(Group) {
  
  return window.GetCustomSecurityRoles ? window.GetCustomSecurityRoles(Group) : ViewModel.ROSecurityRoleHeaderList();
}

function IterateRoles(Group, Callback) {
  var RoleList = GetSecurityRoles(Group),
    GroupRoleList = Group.SecurityGroupRoleList();

  RoleList.Iterate(function (srh) {
    srh.ROSecurityRoleList().Iterate(function (sr) {
      var sgr = GroupRoleList.Find('SecurityRoleID', sr.SecurityRoleID());
      Callback(sgr, sr);
    });
  });
}

function EditRoles(Group) {
  if (Group.IsNew()) {
    Singular.ShowMessage('Edit Roles', 'Please save this group before editing its roles.');
  } else {

    //Populate the SecurityRole list with the groups roles.
    PopulateRoleList(Group);
    ViewModel.CurrentGroup(Group);
    
  }
}

function Undo() {
  ViewModel.CurrentGroup(null);
}

function PopulateRoleList(Group) {
  //Start Edit Roles
  IterateRoles(Group, function (sgr, sr) {
    sr.SelectedInd(sgr && sgr.IsSelected());
  });
}

function UpdateFromSecurityRoleList() {
  //End Edit Roles
  var Group = ViewModel.CurrentGroup(),
      sgrl = Group.SecurityGroupRoleList;
  IterateRoles(Group, function (sgr, sr) {
    if (sgr) {
      sgr.IsSelected(sr.SelectedInd());
    } else if (sr.SelectedInd()) {
      sgrl.Add({ SecurityRoleID: sr.SecurityRoleID(), IsSelected: true });
    }
  });
}

function Save() {
  if (ViewModel.CurrentGroup()) UpdateFromSecurityRoleList();
  var Group = ViewModel.CurrentGroup();

  Singular.ShowLoadingBar();
  ViewModel.CallServerMethod('SaveGroups', { GroupList: ViewModel.SecurityGroupList.Serialise(true) }, function (result) {
    Singular.HideLoadingBar();
    if (result.Success) {
      ViewModel.SecurityGroupList.Update(result.Data);
      ViewModel.CurrentGroup.Clear();
    }
    Singular.AddMessage(result.MessageType - 1, 'Save ', result.Message ? result.Message : result.ErrorText).Fade(2000);
  });
}