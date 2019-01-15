using System.Drawing;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors.LayerEditors;

namespace OgmoEditor.LevelEditors.Resizers
{
	public class TileResizer : Resizer
	{
		public new TileLayerEditor Editor { get; private set; }

		public int[,] oldTiles;

		public TileResizer(TileLayerEditor tileEditor)
			: base(tileEditor)
		{
			Editor = tileEditor;
		}

		public override void Resize(Size oldSize, bool fromRight, bool fromBottom)
		{
			TileLayer layer = Editor.Layer;

			oldTiles = layer.Tiles.Clone() as int[,];

			int tileWidth = layer.Level.Size.Width / layer.Definition.Grid.Width + (layer.Level.Size.Width % layer.Definition.Grid.Width != 0 ? 1 : 0);
			int tileHeight = layer.Level.Size.Height / layer.Definition.Grid.Height + (layer.Level.Size.Height % layer.Definition.Grid.Height != 0 ? 1 : 0);
			int dx = tileWidth - oldTiles.GetLength(0);
			int dy = tileHeight - oldTiles.GetLength(1);

			int[,] newTiles = new int[tileWidth, tileHeight];

			// Clear all tiles
			for (int i = 0; i < tileWidth; i++)
				for (int j = 0; j < tileHeight; j++)
					newTiles[i, j] = -1;

			for (int i = 0; i < newTiles.GetLength(0); i++)
			{
				for (int j = 0; j < newTiles.GetLength(1); j++)
				{
					// Old grid coordinates to pull from
					int x = -1;
					int y = -1;

					// Horizontal bounds checks
					if (fromRight)
					{
						if (i < oldTiles.GetLength(0))
						{
							x = i;
						}
					}
					else
					{
						if (i - dx >= 0)
						{
							x = i - dx;
						}
					}

					// Vertical bounds checks
					if (fromBottom)
					{
						if (j < oldTiles.GetLength(1))
						{
							y = j;
						}
					}
					else
					{
						if (j - dy >= 0)
						{
							y = j - dy;
						}
					}

					// Calculate the final value for the tile
					int val = -1;
					if (x != -1 && y != -1)
					{
						val = oldTiles[x, y];
					}
					newTiles[i, j] = val;
				}
			}

			layer.Tiles = newTiles;
		}

		public override void Undo()
		{
			Editor.Layer.Tiles = oldTiles;
		}
	}
}
