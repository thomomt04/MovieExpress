// ==========================================================================================
// Author: Hendrik de Wet
// Create date: 2018 - 11 - 6
// Description: Check the security groups and roles against Organisation Protected Area List
// ==========================================================================================

function userGroupRolesCheck(obj, sectionName, securityRole) {
	var result = null;
	if (ViewModel.ROSecurityOrganisationProtectedAreaGroupUserList() !== null) {
		result = JSLINQ(ViewModel.ROSecurityOrganisationProtectedAreaGroupUserList()).Any(function (item) {

			return item.OrganisationID() === obj.OrganisationID && item.SectionName() === sectionName &&
				item.SecurityRole() === securityRole

		});
	}
	return result;
}
