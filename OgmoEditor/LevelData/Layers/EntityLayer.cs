using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.Definitions.LayerDefinitions;
using System.Xml;
using OgmoEditor.LevelEditors.LayerEditors;
using OgmoEditor.LevelEditors.Resizers;
using Newtonsoft.Json.Linq;

namespace OgmoEditor.LevelData.Layers
{
    public class EntityLayer : Layer
    {
        public new EntityLayerDefinition Definition { get; private set; }
        public List<Entity> Entities { get; private set; }

        public EntityLayer(Level level, EntityLayerDefinition definition)
            : base(level, definition)
        {
            Definition = definition;

            Entities = new List<Entity>();
        }

        public override XmlElement GetXML(XmlDocument doc)
        {
            XmlElement xml = doc.CreateElement(Definition.Name);

            foreach (Entity e in Entities)
                xml.AppendChild(e.GetXML(doc));

            return xml;
        }

        public override bool SetXML(XmlElement xml)
        {
            foreach (XmlElement e in xml.ChildNodes)
            {
                if (Ogmo.Project.EntityDefinitions.Find(d => d.Name == e.Name) != null)
                    Entities.Add(new Entity(this, e));
            }
            return true;
        }

        public override JObject GetJSON()
        {
            JArray entityArray = new JArray();

            foreach (Entity e in Entities)
                entityArray.Add(e.GetJSON());

            JObject json = new JObject();
            json.Add("name", Definition.Name);
            json.Add("entities", entityArray);
            return json;
        }

        public override bool SetJSON(JToken json)
        {
            JArray jArray = json.Value<JArray>("entities");
            foreach (JObject e in jArray.Children())
            {
                string eName = (string)e.GetValue("name");
                if (Ogmo.Project.EntityDefinitions.Find(d => d.Name == eName) != null)
                    Entities.Add(new Entity(this, e));
            }
            return true;
        }

        public override LayerEditor GetEditor(LevelEditors.LevelEditor editor)
        {
            return new EntityLayerEditor(editor, this);
        }

        public uint GetNewEntityID()
        {
            uint id = 0;
            while (Entities.Find(e => e.ID == id) != null)
                id++;
            return id;
        }
    }
}
