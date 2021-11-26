var MEHelpers = {

	getUrlParameter: function (sParam) {
		var sPageURL = window.location.search.substring(1);
		var sURLVariables = sPageURL.split('&');
		for (var i = 0; i < sURLVariables.length; i++) {
			var sParameterName = sURLVariables[i].split('=');
			if (sParameterName[0] == sParam) {
				return sParameterName[1];
			}
		}
		return null;
	},

	Notification: function (Text, LayoutPosition, Type, Timeout) {
		var n = noty({
			text: Text,
			type: (Type) ? Type.toLowerCase() : 'information',
			dismissQueue: true,
			//layout: LayoutPosition,
			theme: 'defaultTheme',
			timeout: (Timeout) ? Timeout : 3000
		});
	},

	QuestionDialogYesNo: function (Text, LayoutPosition, YesFunction, NoFunction) {
		var n = noty({
			text: Text,
			type: 'alert',
			dismissQueue: true,
			//layout: LayoutPosition,
			theme: 'defaultTheme',
			buttons: [
				{
					addClass: 'btn btn-primary', text: 'Yes', onClick: function ($noty) {
						$noty.close();
						YesFunction();
					}
				},
				{
					addClass: 'btn btn-danger', text: 'No', onClick: function ($noty) {
						$noty.close();
						NoFunction();
					}
				}
			]
		});
	},

	QuestionDialogAssessment: function (Text, LayoutPosition, YesFunction, NoFunction) {
		var n = noty({
			text: Text,
			type: 'alert',
			dismissQueue: true,
			//layout: LayoutPosition,
			theme: 'defaultTheme',
			buttons: [
				{
					addClass: 'btn btn-primary btn btn-outline', text: 'Blank', onClick: function ($noty) {
						$noty.close();
						YesFunction();
					}
				},
				{
					addClass: 'btn btn-primary', text: 'Create a Copy', onClick: function ($noty) {
						$noty.close();
						NoFunction();
					}
				}
			]
		});
	},

	QuestionDialogAssessmentYear: function (Text, LayoutPosition, YesFunction, NoFunction) {
		var n = noty({
			text: Text,
			type: 'alert',
			dismissQueue: true,
			theme: 'defaultTheme',
			buttons: [
				{
					addClass: 'btn btn-primary btn btn-outline', text: ViewModel.PreviousYear(), onClick: function ($noty) {
						$noty.close();
						YesFunction();
					}
				},
				{
					addClass: 'btn btn-primary', text: ViewModel.CurrentYear(), onClick: function ($noty) {
						$noty.close();
						NoFunction();
					}
				}
			]
		});
	},

	ErrorNotificationOk: function (Text, LayoutPosition, YesFunction) {
		var n = noty({
			text: Text,
			type: 'alert',
			dismissQueue: true,
			//layout: LayoutPosition,
			theme: 'defaultTheme',
			buttons: [
				{
					addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
						$noty.close();
						YesFunction();
					}
				}

			]
		});
	},

	horizontalTabControl: function (className) {
		var ulLevel = $("." + className + " > div[data-tabcontrol] > ul");
		var liLevel = $("." + className + " > div[data-tabcontrol] > ul > li");
		var aLevel = $("." + className + " > div[data-tabcontrol] > ul > li > a");

		$("." + className + " > .ui-tabs").removeClass();
		$("." + className + " > .ui-tabs-nav").removeClass();
		$("." + className + " > .ui-tabs-tab").removeClass();
		ulLevel.addClass("nav nav-tabs").removeAttr("role");
		liLevel.removeAttr("data-tab-key").removeAttr("role").removeAttr("tabindex").removeAttr("aria-controls").removeAttr("aria-labelledby").removeAttr("aria-selected").removeAttr("aria-expanded");
		aLevel.removeAttr("role").removeAttr("tabindex").removeAttr("class").removeAttr("id").attr("data-toggle", "tab");
		ulLevel.next().remove();

		var i = 0;
		liLevel.each(function () {
			$(this).attr("id", className + i);
			$(this).appendTo(ulLevel[0]);
			i++;
		})

		ulLevel.each(function () {
			$(this).insertBefore($(this).parent());
		})

		$("." + className + " > div[data-tabcontrol]").remove();

		$("." + className + " > ul:not(:first-child)").remove();

		$("<span class='pull-right'><i class='fa fa-angle-right'></i></span>").insertBefore("." + className + " > ul > li > a > span");

		$("." + className + " > ul.nav-tabs > li > a").click(function () {
			$("." + className + " > ul.nav-tabs > li").removeClass("active");
			$(this).parent().addClass("active");

			$(".tab-pane").removeClass("active");

			var selector = $(this).attr("href");

			$(selector).addClass("active");
		})
	},


	SideMenuActive: function (pagenum) {

		switch (pagenum.value) {
			case "1":
				$($("#menuItem2ChildItem0").parent()[0]).addClass("in")

				break;
			case "2":
				$($("#menuItem2ChildItem1").parent()[0]).addClass("in")

				break;
			case "3":
				$($("#menuItem2ChildItem2").parent()[0]).addClass("in")

				break;
			case "4":
				$($("#menuItem2ChildItem3").parent()[0]).addClass("in")

				break;
			case "5":
				$($("#menuItem2ChildItem4").parent()[0]).addClass("in")

				break;
			case "6":
				$($("#menuItem2ChildItem5").parent()[0]).addClass("in")
				break;
			default:
				break;
		}

	},

	SplitURLVars: function (url) {
		var queryparams = url.split('?')[1];
		var params = queryparams.split('&');
		var pair = null,
			data = [];
		params.forEach(function (d) {
			pair = d.split('=');
			data.push({ key: pair[0], value: pair[1] });
		});

		return data;
  }

}