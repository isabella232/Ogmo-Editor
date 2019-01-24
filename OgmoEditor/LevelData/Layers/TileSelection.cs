using System.Drawing;
using OgmoEditor.LevelEditors.Actions.TileActions;

namespace OgmoEditor.LevelData.Layers
{
	public class TileSelection
	{
		public TileLayer Layer;
		public Rectangle Area;
		public int[,] Under;

		public Rectangle ScaledArea
		{
			get
			{
				return new Rectangle(
					Area.X * Layer.Definition.Grid.Width,
					Area.Y * Layer.Definition.Grid.Height,
					Area.Width * Layer.Definition.Grid.Width,
					Area.Height * Layer.Definition.Grid.Height
				);
			}
		}

		public TileSelection(TileLayer layer, Rectangle area)
		{
			Layer = layer;
			Area = area;

			Under = new int[area.Width, area.Height];
			for (int i = 0; i < area.Width; i++)
				for (int j = 0; j < area.Height; j++)
					Under[i, j] = -1;
		}

		public void SetUnderFromTiles()
		{
			for (int i = 0; i < Area.Width; i++)
				for (int j = 0; j < Area.Height; j++)
					Under[i, j] = Layer[i + Area.X, j + Area.Y];
		}

		public int[,] GetDataFromTiles()
		{
			int[,] data = new int[Area.Width, Area.Height];
			for (int i = 0; i < Area.Width; i++)
				for (int j = 0; j < Area.Height; j++)
					data[i, j] = Layer[i + Area.X, j + Area.Y];
			return data;
		}

		public TileMoveSelectionAction GetMoveAction(Point move)
		{
			if (Area.X + move.X >= 0 && Area.Y + move.Y >= 0 && Area.X + move.X + Area.Width <= Layer.TileCellsX && Area.Y + move.Y + Area.Height <= Layer.TileCellsY)
				return new TileMoveSelectionAction(Layer, move);
			else
				return null;
		}
	}
}
