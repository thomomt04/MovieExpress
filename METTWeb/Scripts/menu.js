var toggleMaintenanceSubMenu = function () {
	//$("#MaintenanceMenu").collapse("show");
	//$("#maintenance").removeClass("collapse");
	//$("#maintenance").attr("aria-expanded", "true");
	//$("#MaintenanceMenu").removeAttr("style");
	$("#maintenance").toggle();

	$('#sidebar').removeClass('active');
	$('#sidebarWrapper').removeClass('active');
}


var hideEverything = function () {
	$("#maintenance").toggle();

}