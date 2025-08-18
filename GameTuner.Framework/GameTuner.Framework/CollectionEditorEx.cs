using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class CollectionEditorEx : CollectionEditor, IPropertyGridAssociation
	{
		public PropertyGrid PropertyGrid { get; set; }

		public CollectionEditorEx(Type type)
			: base(type)
		{
		}

		protected override CollectionForm CreateCollectionForm()
		{
			CollectionForm collectionForm = base.CreateCollectionForm();
			TableLayoutPanel tableLayoutPanel = (TableLayoutPanel)collectionForm.Controls.Find((Control a) => a is TableLayoutPanel);
			if (tableLayoutPanel != null)
			{
				PropertyGrid propertyGrid = (PropertyGrid)FindControl(tableLayoutPanel, (Control a) => a is PropertyGrid);
				if (propertyGrid != null)
				{
					propertyGrid.HelpVisible = true;
				}
				Button button = (Button)FindControl(collectionForm, (Control a) => a is Button && ((Button)a).DialogResult == DialogResult.OK);
				if (button != null)
				{
					button.Click += button_Click;
				}
			}
			return collectionForm;
		}

		protected Control FindControl(Control control, Predicate<Control> predicate)
		{
			if (predicate(control))
			{
				return control;
			}
			foreach (Control control4 in control.Controls)
			{
				Control control3 = FindControl(control4, predicate);
				if (control3 != null)
				{
					return control3;
				}
			}
			return null;
		}

		private void button_Click(object sender, EventArgs e)
		{
		}
	}
}
