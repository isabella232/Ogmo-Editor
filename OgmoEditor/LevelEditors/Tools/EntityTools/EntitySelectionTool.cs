using System;
using System.Collections.Generic;
using System.Drawing;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors.Actions.EntityActions;

namespace OgmoEditor.LevelEditors.Tools.EntityTools
{
	public class EntitySelectionTool : EntityTool
	{
		private bool drawing;
		private bool moving;
		private Point mouseStart;
		private EntityMoveAction moveAction;
		private Point moved;

		public EntitySelectionTool()
			: base("Select / Move", "selection.png")
		{
			moveAction = null;
		}

		public override void Draw(Graphics graphics)
		{
			if (drawing)
			{
				int x = Math.Min(mouseStart.X, LevelEditor.MousePosition.X);
				int y = Math.Min(mouseStart.Y, LevelEditor.MousePosition.Y);
				int w = Math.Max(mouseStart.X, LevelEditor.MousePosition.X) - x;
				int h = Math.Max(mouseStart.Y, LevelEditor.MousePosition.Y) - y;

				graphics.DrawSelectionRectangle(new Rectangle(x, y, w, h));
			}
		}

		public override void OnMouseLeftDown(Point location)
		{
			if (Ogmo.EntitySelectionWindow.Selected.Count > 0 &&
				Ogmo.EntitySelectionWindow.Selected.Exists(e => e.Bounds.Contains(location)))
			{
				moving = true;
				mouseStart = location;
				moved = Point.Empty;
			}
			else
			{
				drawing = true;
				mouseStart = location;
			}
		}

		public override void OnMouseMove(Point location)
		{
			if (moving)
			{
				Point move = new Point(location.X - mouseStart.X, location.Y - mouseStart.Y);
				if (!Util.Ctrl)
					move = LayerEditor.Layer.Definition.SnapToGrid(move);

				move = new Point(move.X - moved.X, move.Y - moved.Y);
				if (move.X != 0 || move.Y != 0)
				{
					if (moveAction != null)
						moveAction.DoAgain(move);
					else
						LevelEditor.Perform(moveAction = new EntityMoveAction(LayerEditor.Layer, Ogmo.EntitySelectionWindow.Selected, move));
					moved = new Point(move.X + moved.X, move.Y + moved.Y);
					Ogmo.EntitySelectionWindow.RefreshPosition();
				}
			}
		}

		public override void OnMouseRightDown(Point location)
		{
			if (!drawing)
				Ogmo.EntitySelectionWindow.ClearSelection();
		}

		public override void OnMouseLeftUp(Point location)
		{
			if (drawing)
			{
				drawing = false;

				int x = Math.Min(mouseStart.X, LevelEditor.MousePosition.X);
				int y = Math.Min(mouseStart.Y, LevelEditor.MousePosition.Y);
				int w = Math.Max(mouseStart.X, LevelEditor.MousePosition.X) - x;
				int h = Math.Max(mouseStart.Y, LevelEditor.MousePosition.Y) - y;
				Rectangle r = new Rectangle(x, y, w, h);

				List<Entity> hit = LayerEditor.Layer.Entities.FindAll(e => e.Bounds.IntersectsWith(r));

				if (Util.Ctrl)
					Ogmo.EntitySelectionWindow.ToggleSelection(hit);
				else
					Ogmo.EntitySelectionWindow.SetSelection(hit);
			}

			if (moving)
			{
				moving = false;
				moveAction = null;
			}
		}
	}
}
