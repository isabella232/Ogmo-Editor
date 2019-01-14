using System.Drawing;
using System.Windows.Forms;
using OgmoEditor.Definitions.ValueDefinitions;

namespace OgmoEditor.ProjectEditors.ValueDefinitionEditors
{
	public partial class ColorValueDefinitionEditor : UserControl
	{
		private ColorValueDefinition def;

		public ColorValueDefinitionEditor(ColorValueDefinition def)
		{
			this.def = def;
			InitializeComponent();
			Location = new Point(99, 53);

			defaultColorChooser.Color = def.Default;
		}

		private void defaultColorChooser_ColorChanged(OgmoColor color)
		{
			def.Default = color;
		}
	}
}
