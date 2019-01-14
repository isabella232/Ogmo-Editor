using OgmoEditor.LevelEditors.Actions.TileActions;
using System.Drawing;
using System.Windows.Forms;

namespace OgmoEditor.LevelEditors.Tools.TileTools
{
	public class TileFloodTool : TileTool
	{
		bool randomizing;

		public TileFloodTool()
			: base("Flood Fill", "flood.png")
		{

		}

		public override void OnKeyDown(Keys key)
		{
			if (key == Keys.ShiftKey)
			{
				randomizing = true;
			}
		}

		public override void OnKeyUp(Keys key)
		{
			if (key == Keys.ShiftKey)
			{
				randomizing = false;
			}
		}

		public override void OnMouseLeftClick(Point location)
		{
			location = LayerEditor.Layer.Definition.ConvertToGrid(location);
			if (IsValidTileCell(location) && LayerEditor.Layer[location.X, location.Y] != Ogmo.TilePaletteWindow.TilesStartID)
				LevelEditor.Perform(new TileFloodAction(LayerEditor.Layer, location, Ogmo.TilePaletteWindow.Tiles, randomizing));
		}

		public override void OnMouseRightClick(Point location)
		{
			location = LayerEditor.Layer.Definition.ConvertToGrid(location);
			if (IsValidTileCell(location) && LayerEditor.Layer[location.X, location.Y] != -1)
				LevelEditor.Perform(new TileFloodAction(LayerEditor.Layer, location, null));
		}
	}
}
