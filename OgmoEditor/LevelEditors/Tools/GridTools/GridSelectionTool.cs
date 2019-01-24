using System.Drawing;
using OgmoEditor.LevelEditors.Actions.GridActions;

namespace OgmoEditor.LevelEditors.Tools.GridTools
{
	public class GridSelectionTool : GridTool
	{
		private bool drawing;
		private Point drawStart;
		private Point drawTo;

		private bool moving;
		private Point mouseStart;
		private Point moved;

		public GridSelectionTool()
			: base("Select / Move", "selection.png")
		{
			moving = false;
		}

		public override void Draw(Graphics graphics)
		{
			if (drawing)
			{
				Rectangle draw = LayerEditor.Layer.GetGridRectangle(drawStart, drawTo);
				if (LevelEditor.Level.Bounds.IntersectsWith(draw))
					graphics.DrawSelectionRectangle(draw);
			}
		}

		public override void OnMouseLeftDown(Point location)
		{
			if (LayerEditor.Layer.Selection != null &&
				LayerEditor.Layer.Selection.ScaledArea.Contains(location))
			{
				moving = true;
				mouseStart = location;
				moved = Point.Empty;
				LevelEditor.StartBatch();
			}
			else
			{
				drawTo = drawStart = LayerEditor.MouseSnapPosition;
				drawing = true;
			}
		}

		public override void OnMouseLeftUp(Point location)
		{
			if (drawing)
			{
				drawing = false;
				drawTo = LayerEditor.MouseSnapPosition;

				Rectangle rect = LayerEditor.Layer.GetGridRectangle(drawStart, drawTo);
				rect.X /= LayerEditor.Layer.Definition.Grid.Width;
				rect.Width /= LayerEditor.Layer.Definition.Grid.Width;
				rect.Y /= LayerEditor.Layer.Definition.Grid.Height;
				rect.Height /= LayerEditor.Layer.Definition.Grid.Height;

				if (rect.IntersectsWith(new Rectangle(0, 0, LayerEditor.Layer.GridCellsX, LayerEditor.Layer.GridCellsY)))
					LevelEditor.Perform(new GridSelectAction(LayerEditor.Layer, rect));
			}

			if (moving)
			{
				moving = false;
				LevelEditor.EndBatch();
			}
		}

		public override void OnMouseMove(Point location)
		{
			if (drawing)
			{
				drawTo = LayerEditor.MouseSnapPosition;
			}

			if (moving)
			{
				Point move = new Point(location.X - mouseStart.X, location.Y - mouseStart.Y);
				move = LayerEditor.Layer.Definition.ConvertToGrid(move);
				move.X -= moved.X;
				move.Y -= moved.Y;

				if (move.X != 0 || move.Y != 0)
				{
					LevelEditor.BatchPerform(LayerEditor.Layer.Selection.GetMoveAction(move));
					moved = new Point(move.X + moved.X, move.Y + moved.Y);
				}
			}
		}

		public override void OnMouseRightClick(Point location)
		{
			if (!drawing)
				LevelEditor.Perform(new GridClearSelectionAction(LayerEditor.Layer));
		}
	}
}
