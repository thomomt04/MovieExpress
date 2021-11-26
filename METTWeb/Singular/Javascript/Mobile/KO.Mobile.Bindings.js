/// <reference path="Intellisense.js" />

Singular.DetermineClickEvent = function () {
  Singular.ClickEvent = 'click';
  if ('ontouchstart' in document.documentElement) {
    // for android and ?safari?
    Singular.ClickEvent = 'touchstart'
    console.log('click event changed to touchstart');
 
  }
  else if ('onmousedown' in document.documentElement) {
    // for WP8
    Singular.ClickEvent = 'mousedown'
    console.log('click event changed to mousedown');
   
  }
};

ko.bindingHandlers.click = {
  init: function (element, valueAccessor) {
    var binding = ko.utils.unwrapObservable(valueAccessor());
    $(element).on(Singular.ClickEvent, function (e) {

      if (this.nodeName == 'BUTTON') {
        $(document.activeElement).change();
      }
     
      //Make sure this doesnt still appear in another page.
      Singular.Validation.HideErrorPopup();

      binding();
    });
  }
};


