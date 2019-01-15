using System.Collections.Generic;
using OgmoEditor.LevelData;
using System.Drawing;
using OgmoEditor.LevelEditors.Resizers;

namespace OgmoEditor.LevelEditors.Actions.LevelActions
{
	public class LevelResizeAction : LevelAction
	{
		private List<Resizer> resizers;
		private Size oldSize;
		private Size newSize;
		private bool fromRight;
		private bool fromBottom;

		public LevelResizeAction(Level level, Size newSize, bool fromRight = true, bool fromBottom = true)
			: base(level)
		{
			oldSize = level.Size;
			this.newSize = newSize;

			this.fromRight = fromRight;
			this.fromBottom = fromBottom;

			Resizer r;
			resizers = new List<Resizer>(level.Layers.Count);
			foreach (var l in Ogmo.MainWindow.LevelEditors[Ogmo.CurrentLevelIndex].LayerEditors)
			{
				r = l.GetResizer();
				if (r != null)
					resizers.Add(r);
			}
		}

		public override void Do()
		{
			base.Do();

			Level.Size = newSize;
			foreach (var r in resizers)
				r.Resize(oldSize, fromRight, fromBottom);
		}

		public override void Undo()
		{
			base.Undo();

			Level.Size = oldSize;
			foreach (var r in resizers)
				r.Undo();
		}
	}
}
