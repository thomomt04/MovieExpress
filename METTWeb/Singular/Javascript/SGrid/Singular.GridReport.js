// Version 1.0.9
Singular.GridReport = (function () {
  var self = {};

  self.ReportDataPage = '../Reports/ReportData.aspx';
  
  //Open grid report tab (used on reports screen.)
  self.OpenGridReportTab = function (LayoutName, ReportType, Criteria) {

    var ReportID;
    if (!ReportType) {
      ReportType = ViewModel.Report.ChildType.Type;
      Criteria = ViewModel.Report().ReportCriteriaGeneric();
      ReportID = ViewModel.Report().UniqueKey();
    }
   

    var PostObject = {
      ObjectType: 'Singular.Reporting.GridInfo',
      LayoutName: LayoutName,
      GetDataParams: 
        {
          ReportID: ReportID,
          ReportType: ReportType,
          Criteria: KOFormatter.Serialise(Criteria)
        }
    }
   
    if (Singular.DebugMode && $('[name=hModelData').length) {
      //open in same window so debugger is still attached.
      var form = $(document).find('form');
      form.attr('action', self.ReportDataPage);
      $('[name=hModelData').val(JSON.stringify(PostObject));
      form[0].submit();
    } else {
      //open in new window.
      var popup = window.open('', '_blank');
      popup.document.write('<!DOCTYPE html><html><head><title>Report Data - Loading...</title></head><body style="font-variant: small-caps; font-family: Arial; font-size: 20px; font-weight: 100;">Loading data...<form method="post"></form></body></html>');

      var form = $(popup.document).find('form');
      form.attr('action', self.ReportDataPage);
      form.append('<input type="hidden" name="hModelData"/>');
      form.append('<input type="hidden" name="hCSRF" value="' + $('#hCSRF').val() + '"/>');
      form.children('[name=hModelData]').val(JSON.stringify(PostObject));
      form[0].submit();
    }
    
  }
  //Show report layouts (used on reports screen.)
  self.ShowOptions = function (e) {

    if (!window.CanvasRenderingContext2D) {
      alert('Sorry, your browser is too old to view grid reports, please upgrade');
    } else {
      if (ViewModel.IsValid()) {
        if (ClientData.GridLayoutList.length == 1) {
          self.OpenGridReportTab(ClientData.GridLayoutList[0].LayoutName);
        } else {

          var List = [];
          for (var i = 0; i < ClientData.GridLayoutList.length; i++) {
            List.push({ Text: ClientData.GridLayoutList[i].LayoutName });
          }
          Singular.ContextMenu.Show(List, $(e.target).offset().left, -$(e.target).offset().top, function (Item) {
            self.OpenGridReportTab(Item.Text);
          })
        }
      } else {
        Singular.ShowMessage('View Data', 'Please fill in all required fields.');
      }
    }

  }
      
  return self;
})();


var DynamicCriteriaObject = function (Parent) {
  SetupKOObject(this, Parent, function (self) {

    CreateTypedProperty(self, 'ParameterList', ROParameterObject, true, false);

    ClientData.ViewModel.Report.ReportCriteriaGeneric.ParameterList.Iterate(function (Item, i) {

      var prop = CreateROProperty(self, Item.ParameterName, Item.DefaultValue);
      prop.subscribe(function (NewValue) {
        var Param = self.ParameterList.peek().Find('ParameterName', Item.ParameterName);
        Param.SelectedValue(NewValue);
      });

      if (Item.RequiredInd) {
        AddRule(prop, 'Required', { Description: Item.DisplayName + ' is Required' });
      } else {
        prop.Nullable();
      }

    });

    self.ToString = function () {
      return ClientData.ViewModel.Report.ReportName;
    }

  });
};
