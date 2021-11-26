using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Singular.Web.MaintenanceHelpers;
using Singular.Web;

namespace MEWeb.Controls
{
	public class FactorEditor : MaintenanceEditor
	{
		private Action<Singular.Web.Controls.HelperControls.HelperAccessors<MaintenanceVM>, MaintenancePage> mOnRenderButton;
		public FactorEditor(MaintenanceVM VM) : base(VM) { }

		public FactorEditor(MaintenanceVM VM, Action<Singular.Web.Controls.HelperControls.HelperAccessors<MaintenanceVM>, MaintenancePage> OnRenderButton) : base(VM, OnRenderButton) {
			mOnRenderButton = OnRenderButton;
		}

		
		protected override void Setup()
		{
			base.NoContentSetup();

			MaintenanceVM VM = (MaintenanceVM)base.Model;

			using (var h = this.Helpers)
			{
				

				h.MessageHolder();

				if(MaintenancePage.InfoText != "")
				{
					h.HTML(MaintenancePage.InfoText);
				}

				if(MaintenancePage.EditMode == MaintenancePage.EditModeType.Grid)
				{
					AddTable(h);
				}
				else if (MaintenancePage.EditMode == MaintenancePage.EditModeType.Form)
				{
					AddForm(h);
				}
				else
				{
					AddCustomControl(h);
				}

				if(VM.IsStateless)
				{
					Singular.Web.PageBase BasePage = (Singular.Web.PageBase)Page;
					//BasePage.AddLateResource(Singular.Web.Scripts.LibraryIncludes.MaintenanceScript.ScriptTag.ToString());
				}

				if (!(VM.CurrentList != null) && !(VM.CurrentObject != null))
				{
					h.HTML("<p>Please add a new record, or click find to edit an existing record.</p>");
					h.FindScreen<object>("Find", "Find", false, MaintenancePage.ListType);
					if (MaintenancePage.AllowAdd)
					{
						h.Button(DefinedButtonType.New);
					}
				}
				else
				{
					if (VM.CanEdit)
					{
						var saveButton = h.Button(DefinedButtonType.Save);
						if (VM.IsStateless)
						{
							saveButton.AddBinding(KnockoutBindingString.click, "Save()");
						}

						var undoButton = h.Button(DefinedButtonType.Undo);
						undoButton.AddBinding(KnockoutBindingString.visible, c => c.IsDirty);
						undoButton.ButtonText.Text = c => c.IsDirty ? "Undo" : "Back";
						if (VM.IsStateless)
						{
							undoButton.AddBinding(KnockoutBindingString.click, "Undo()");
						}

						if (MaintenancePage.AllowExport)
						{
							h.Button(DefinedButtonType.Export).PostBackType = PostBackType.Full;
						}

						if (MaintenancePage.AllowRemove == true && MaintenancePage.EditMode == MaintenancePage.EditModeType.Form)
						{
							var deleteButton = h.Button("Delete", "Delete");
							deleteButton.Image.SrcDefined = DefinedImageType.TrashCan;
							deleteButton.PromptText = "Are you sure you want to delete this Record?";
						}

						foreach (var btn in MaintenancePage.ButtonList)
						{
							h.Control(btn);
						}
					}
				}

				if (VM.CurrentObject != null)
				{
					var backButton = h.Button("Back", "Back");
					backButton.Image.SrcDefined = DefinedImageType.Left;
					backButton.Image.Glyph = FontAwesomeIcon.back;
				}

				if (mOnRenderButton != null)
				{
					mOnRenderButton.Invoke(h, MaintenancePage);
				}

			}

		}
	}
}