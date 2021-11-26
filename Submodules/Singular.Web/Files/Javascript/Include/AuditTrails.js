var ATHelper = (function () {

  var self = this;

  Singular.PreBind(function () {
    CreateUserIndex();
    ProcessData(ClientData.ViewModel.AuditTrailLookup.ROAuditTrailTableList);
  }, true);

  self.GetCurrentDescription = function() {
    var sh = ViewModel.AuditTrailLookup.peek().SelectedHeader();
    return 'Change history for ' + sh.Description() + ' (' + sh.GetParent().TableName() + ')';
  }

  

  function CreateUserIndex() {
    ClientData.ATUserList.Iterate(function (atu) {
      ClientData.ATUserList['ATU' + atu.AuditTrailUserID] = atu;
    });
  }
  function UpdateUserIndex(NewUserList) {
    NewUserList.Iterate(function (atu) {
      var Key = 'ATU' + atu.AuditTrailUserID;
      if (!ClientData.ATUserList[Key]) {
        ClientData.ATUserList.push(atu);
        ClientData.ATUserList[Key] = atu;
      }
    });
  }

  function ProcessData(attl) {
    if (attl) {
      attl.Iterate(function (att) {
        var athl = att.ROAuditTrailHeaderList;
        if (athl) {
          athl.Iterate(function (ath) {
            //Set user info
            var atl = ath.ROAuditTrailList;
            atl.Iterate(function (at) {
              var atu = ClientData.ATUserList['ATU' + at.ATUserID];
              if (atu) {
                at.UserName = atu.UserName;
              } else {
                at.UserName = '?';
              }
            });
            if (atl.length) {
              ath.LastChangeDate = atl[0].ChageDateTime;
              ath.LastChangedBy = atl[0].UserName;
            }

            //recurse down
            ProcessData(ath.ROAuditTrailTableList);
          });
          if (athl.length == 1) athl[0].IsExpanded = athl[0].FetchedChildren;
        }

      });
      if (attl.length == 1) attl[0].IsExpanded = true;
    }
  }

  function FetchChildren(att, ath) {
    ath.IsLoading(true);
    ROAuditTrailHeaderObject.CallServerMethod('FetchChildren', { ParentTableID: att.AuditTrailTableID(), ParentRowID: ath.RowID() }, function (data) {
      ath.IsLoading(false);
      UpdateUserIndex(data.Item2);
      ProcessData(data.Item1);
      ath.ROAuditTrailTableList.Set(data.Item1);
      ath.FetchedChildren(true);
      ath.IsExpanded(data.Item1.length > 0);
    }, true);
  }

  self.HeaderExpandClass = function(ath) {
    return ath.FetchedChildren() ? '' : (ath.IsLoading() ? 'fa fa-refresh fa-spin' : 'fa fa-binoculars');
  }

  self.ShowHeaderExpand = function(att, ath) {
    return att.HasChildren() && (!ath.FetchedChildren() || ath.ROAuditTrailTableList().length > 0);
  }

  self.HeaderExpand = function(att, ath, e) {
    e.stopPropagation();
    if (!ath.IsLoading()) {
      if (!ath.FetchedChildren()) {
        FetchChildren(att, ath);
      } else {
        ath.IsExpanded(!ath.IsExpanded());
      }
    }
  }

  self.GetColumnList = function() {
    var sh = ViewModel.AuditTrailLookup.peek().SelectedHeader();
    if (sh) {
      if (!sh.ColList) {
        sh.ColList = [];
        sh.ROAuditTrailList.peek().Iterate(function (at) {
          at.ColIndex = {};
          at.ROAuditTrailDetailList.peek().Iterate(function (atd) {
            var Key = 'Col' + atd.Column.peek();
            at.ColIndex[Key] = true;
            if (!sh.ColList[Key]) {
              sh.ColList.push({ Column: atd.Column.peek() });
              sh.ColList[Key] = true;
            }
          });
        });
        sh.ColList.sort(function (a, b) { return a.Column < b.Column ? -1 : 1 });
      }
      return sh.ColList;
    }
    return [];
  }

  self.ApplyColumnFilter = function() {
    var Col = ViewModel.AuditTrailLookup.peek().ColumnFilter();
    ViewModel.AuditTrailLookup.peek().SelectedHeader().ROAuditTrailList.peek().Iterate(function (at) {
      if (Col && !at.ColIndex['Col' + Col]) {
        at.IsFiltered(true);
      } else {
        at.IsFiltered(false);
      }

    });
  }

  self.GetOperation = function(at) {
    if (at.Type.peek() == 1) {
      return 'Inserted';
    } else if (at.Type.peek() == 3) {
      return 'Deleted';
    } else {
      var atdl = at.ROAuditTrailDetailList.peek();
      if (atdl.length > 1) {
        return 'Updated ' + atdl.length + ' fields';
      } else {
        return 'Updated ' + atdl[0].Column.peek();
      }
    }
  }

  self.SelectRow = function (Header) {
    if (Header.ROAuditTrailList()[0].Type() == 4) {
      Singular.ShowMessage(Header.Description(), 'This record has never changed.');
    } else {
      ViewModel.AuditTrailLookup.peek().SelectedHeader(Header);
    }
  }

  return self;

})();