using OgmoEditor.LevelEditors.LayerEditors;

namespace OgmoEditor.LevelEditors.Tools.EntityTools
{
	public abstract class EntityTool : Tool
	{
		public EntityTool(string name, string image)
			: base(name, image)
		{

		}

		public EntityLayerEditor LayerEditor
		{
			get { return (EntityLayerEditor)LevelEditor.LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex]; }
		}
	}
}
