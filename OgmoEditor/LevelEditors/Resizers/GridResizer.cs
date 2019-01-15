using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors.LayerEditors;

namespace OgmoEditor.LevelEditors.Resizers
{
	public class GridResizer : Resizer
	{
		public new GridLayerEditor Editor { get; private set; }

		public bool[,] oldGrid;

		public GridResizer(GridLayerEditor gridEditor)
			: base(gridEditor)
		{
			Editor = gridEditor;
		}

		public override void Resize(bool fromRight, bool fromBottom)
		{
			GridLayer layer = Editor.Layer;

			oldGrid = layer.Grid;
			int tileWidth = layer.Level.Size.Width / layer.Definition.Grid.Width + (layer.Level.Size.Width % layer.Definition.Grid.Width != 0 ? 1 : 0);
			int tileHeight = layer.Level.Size.Height / layer.Definition.Grid.Height + (layer.Level.Size.Height % layer.Definition.Grid.Height != 0 ? 1 : 0);
			layer.Grid = new bool[tileWidth, tileHeight];

			int dx = tileWidth - oldGrid.GetLength(0);
			int dy = tileHeight - oldGrid.GetLength(1);

			for (int i = 0; i < layer.Grid.GetLength(0); i++)
			{
				for (int j = 0; j < layer.Grid.GetLength(1); j++)
				{
					int x = -1;
					int y = -1;

					// Horizontal
					if (fromRight)
					{
						if (i < oldGrid.GetLength(0))
							x = i;
					}
					else
					{
						if (i - dx >= 0)
							x = i - dx;
					}

					// Vertical
					if (fromBottom)
					{
						if (j < oldGrid.GetLength(1))
							y = j;
					}
					else
					{
						if (j - dy >= 0)
							y = j - dy;
					}

					bool val = false;

					if (y != -1 && x != -1)
						val = oldGrid[x, y];

					layer.Grid[i, j] = val;
				}
			}
		}

		public override void Undo()
		{
			Editor.Layer.Grid = oldGrid;
		}
	}
}
