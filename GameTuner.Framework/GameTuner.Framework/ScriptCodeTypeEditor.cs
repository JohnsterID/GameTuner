using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class ScriptCodeTypeEditor : UITypeEditor
	{
		private ScriptCodeForm scriptDialog;

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (scriptDialog == null)
			{
				scriptDialog = new ScriptCodeForm();
			}
			IScriptCodeHandlerProvider scriptCodeHandlerProvider = context.Instance as IScriptCodeHandlerProvider;
			if (scriptCodeHandlerProvider != null)
			{
				scriptDialog.CompileHandler = scriptCodeHandlerProvider.CompileHandler;
			}
			if (value is ObjectWrapper)
			{
				scriptDialog.ScriptCode = (string)((ObjectWrapper)value).Object;
			}
			else
			{
				scriptDialog.ScriptCode = (string)value;
			}
			if (scriptDialog.ShowDialog() == DialogResult.OK)
			{
				if (value is ObjectWrapper)
				{
					return new ObjectWrapper(scriptDialog.ScriptCode, value.ToString());
				}
				return scriptDialog.ScriptCode;
			}
			return value;
		}
	}
}
