using System;
using System.Collections.Generic;
using System.Drawing;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors.LayerEditors;

namespace OgmoEditor.LevelEditors.Resizers
{
	public class EntityResizer : Resizer
	{
		public new EntityLayerEditor Editor { get; private set; }

		public List<Entity> oldEntities = new List<Entity>();

		public EntityResizer(EntityLayerEditor entityEditor)
			: base(entityEditor)
		{
			Editor = entityEditor;
		}

		public override void Resize(Size oldSize, bool fromRight, bool fromBottom)
		{
			EntityLayer layer = Editor.Layer;

			// Clone all the entities into the oldEntities list
			Editor.Layer.Entities.ForEach(item => oldEntities.Add(item.Clone()));

			foreach (Entity e in Editor.Layer.Entities)
			{
				if (!fromRight)
				{
					e.Position.X += layer.Level.Size.Width - oldSize.Width;
				}
				if (!fromBottom)
				{
					e.Position.Y += layer.Level.Size.Height - oldSize.Height;
				}
			}
		}

		public override void Undo()
		{
			Editor.Layer.Entities.Clear();
			oldEntities.ForEach(item => Editor.Layer.Entities.Add(item.Clone()));
		}
	}
}
