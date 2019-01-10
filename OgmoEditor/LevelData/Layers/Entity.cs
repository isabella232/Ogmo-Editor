using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OgmoEditor.Definitions;
using OgmoEditor.Definitions.ValueDefinitions;

namespace OgmoEditor.LevelData.Layers
{
    public class Entity
    {
        public EntityLayer Layer { get; private set; }
        public EntityDefinition Definition { get; private set; }
        public Point Position;
        public Size Size;
        public float Angle;
        public List<Value> Values { get; private set; }
        public List<Point> Nodes;
        public uint ID { get; private set; }

        public Entity(EntityLayer layer, EntityDefinition def, Point position)
        {
            Layer = layer;
            Definition = def;
            ID = layer.GetNewEntityID();

            Position = position;
            Size = def.Size;
            Angle = 0;

            //Nodes
            if (def.NodesDefinition.Enabled)
                Nodes = new List<Point>();

            //Values
            if (def.ValueDefinitions.Count > 0)
            {
                Values = new List<Value>(def.ValueDefinitions.Count);
                foreach (var d in def.ValueDefinitions)
                    Values.Add(new Value(d));
            }
        }

        public Entity(EntityLayer layer, XmlElement xml)
        {
            Layer = layer;
            Definition = Ogmo.Project.EntityDefinitions.Find(d => d.Name == xml.Name);

            //ID
            if (xml.Attributes["id"] != null)
                ID = Convert.ToUInt32(xml.Attributes["id"].InnerText);
            else
                ID = layer.GetNewEntityID();                

            //Position
            Position = new Point(Convert.ToInt32(xml.Attributes["x"].InnerText), Convert.ToInt32(xml.Attributes["y"].InnerText));

            //Size
            if (Definition.ResizableX && xml.Attributes["width"] != null)
                Size.Width = Convert.ToInt32(xml.Attributes["width"].InnerText);
            else
                Size.Width = Definition.Size.Width;
            if (Definition.ResizableY && xml.Attributes["height"] != null)
                Size.Height = Convert.ToInt32(xml.Attributes["height"].InnerText);
            else
                Size.Height = Definition.Size.Height;

            //Rotation
            if (Definition.Rotatable && xml.Attributes["angle"] != null)
                Angle = Ogmo.Project.ImportAngle(xml.Attributes["angle"].InnerText);

            //Nodes
            if (Definition.NodesDefinition.Enabled)
            {
                Nodes = new List<Point>();
                foreach (XmlElement node in xml.GetElementsByTagName("node"))
                    Nodes.Add(new Point(Convert.ToInt32(node.Attributes["x"].InnerText), Convert.ToInt32(node.Attributes["y"].InnerText)));
            }

            //Values
            if (Definition.ValueDefinitions.Count > 0)
            {
                Values = new List<Value>(Definition.ValueDefinitions.Count);
                foreach (var d in Definition.ValueDefinitions)
                    Values.Add(new Value(d));
                OgmoParse.ImportValues(Values, xml);
            } 
        }

        public Entity(EntityLayer layer, Entity e)
        {
            Layer = layer;
            Definition = e.Definition;
            ID = layer.GetNewEntityID();

            Position = e.Position;
            Size = e.Size;
            Angle = e.Angle;

            //Nodes
            if (Definition.NodesDefinition.Enabled)
            {
                Nodes = new List<Point>();
                foreach (var p in e.Nodes)
                    Nodes.Add(p);
            }

            //Values
            if (Definition.ValueDefinitions.Count > 0)
            {
                Values = new List<Value>(e.Values.Count);
                foreach (var v in e.Values)
                    Values.Add(new Value(v));
            }
        }

        public Entity(EntityLayer layer, JObject json)
        {
            Layer = layer;
            Definition = Ogmo.Project.EntityDefinitions.Find(d => d.Name == (string)json.GetValue("name"));

            //ID
            if (json.Property("id") != null)
                ID = Convert.ToUInt32(json.GetValue("id"));
            else
                ID = layer.GetNewEntityID();

            //Position
            Position = new Point(Convert.ToInt32(json.GetValue("x")), Convert.ToInt32(json.GetValue("y")));

            //Size
            if (Definition.ResizableX && json.Property("width") != null)
                Size.Width = Convert.ToInt32(json.GetValue("width"));
            else
                Size.Width = Definition.Size.Width;

            if (Definition.ResizableY && json.Property("height") != null)
                Size.Height = Convert.ToInt32(json.GetValue("height"));
            else
                Size.Height = Definition.Size.Height;

            //Rotation
            if (Definition.Rotatable && json.Property("angle") != null)
                Angle = Ogmo.Project.ImportAngle((string)json.GetValue("angle"));

            //Nodes
            if (Definition.NodesDefinition.Enabled)
            {
                Nodes = new List<Point>();
                if (json.Property("nodes") != null)
                {
                    foreach (JObject node in json.GetValue("nodes"))
                    {
                        Nodes.Add(new Point(Convert.ToInt32(node.GetValue("x")), Convert.ToInt32(node.GetValue("y"))));
                    }
                }
            }

            //Values
            if (Definition.ValueDefinitions.Count > 0)
            {
                Values = new List<Value>(Definition.ValueDefinitions.Count);
                foreach (var d in Definition.ValueDefinitions)
                    Values.Add(new Value(d));
                OgmoParse.ImportValues(Values, json);
            }
        }

        public XmlElement GetXML(XmlDocument doc)
        {
            XmlElement xml = doc.CreateElement(Definition.Name);
            XmlAttribute a;

            //ID
            a = doc.CreateAttribute("id");
            a.InnerText = ID.ToString();
            xml.Attributes.Append(a);

            //Position
            a = doc.CreateAttribute("x");
            a.InnerText = Position.X.ToString();
            xml.Attributes.Append(a);
            a = doc.CreateAttribute("y");
            a.InnerText = Position.Y.ToString();
            xml.Attributes.Append(a);

            //Size
            if (Definition.ResizableX)
            {
                a = doc.CreateAttribute("width");
                a.InnerText = Size.Width.ToString();
                xml.Attributes.Append(a);
            }
            if (Definition.ResizableY)
            {
                a = doc.CreateAttribute("height");
                a.InnerText = Size.Height.ToString();
                xml.Attributes.Append(a);
            }

            //Rotation
            if (Definition.Rotatable)
            {
                a = doc.CreateAttribute("angle");
                a.InnerText = Ogmo.Project.ExportAngle(Angle).ToString();
                xml.Attributes.Append(a);
            }

            //Nodes
            if (Nodes != null)
            {
                foreach (var p in Nodes)
                {
                    XmlElement node = doc.CreateElement("node");
                    a = doc.CreateAttribute("x");
                    a.InnerText = p.X.ToString();
                    node.Attributes.Append(a);
                    a = doc.CreateAttribute("y");
                    a.InnerText = p.Y.ToString();
                    node.Attributes.Append(a);
                    xml.AppendChild(node);
                }
            }

            //Values
            if (Values != null)
                foreach (var v in Values)
                    xml.Attributes.Append(v.GetXML(doc));

            return xml;
        }

        public void WriteJSON(JsonTextWriter jw)
        {
            jw.WriteStartObject();

            jw.WritePropertyName("name");
            jw.WriteValue(Definition.Name);

            jw.WritePropertyName("id");
            jw.WriteValue(ID);

            jw.WritePropertyName("x");
            jw.WriteValue(Position.X);
            jw.WritePropertyName("y");
            jw.WriteValue(Position.Y);

            if (Definition.ResizableX)
            {
                jw.WritePropertyName("width");
                jw.WriteValue(Size.Width);
            }
            if (Definition.ResizableY)
            {
                jw.WritePropertyName("height");
                jw.WriteValue(Size.Height);
            }

            if (Definition.Rotatable)
            {
                jw.WritePropertyName("angle");
                jw.WriteValue(Ogmo.Project.ExportAngle(Angle));
            }

            if (Nodes != null)
            {
                jw.WritePropertyName("nodes");
                jw.WriteStartArray();
                foreach (var p in Nodes)
                {
                    jw.WriteStartObject();
                    jw.WriteRaw($@"""x"": {p.X}, ""y"": {p.Y}");
                    jw.WriteEndObject();
                }
                jw.WriteEndArray();
            }

            if (Values != null)
            {
                foreach (var v in Values)
                {
                    jw.WritePropertyName(v.Definition.Name);

                    // Make sure to save it as the right type
                    Type defType = v.Definition.GetType();
                    if (defType == typeof(IntValueDefinition))
                    {
                        jw.WriteValue(Convert.ToInt32(v.Content));
                    }
                    else if (defType == typeof(BoolValueDefinition))
                    {
                        jw.WriteValue(Convert.ToBoolean(v.Content));
                    }
                    else if (defType == typeof(FloatValueDefinition))
                    {
                        jw.WriteValue(Convert.ToSingle(v.Content));
                    }
                    else
                    {
                        // Treat it as a string
                        jw.WriteValue(v.Content);
                    }
                }
            }

            jw.WriteEndObject();
        }

        public void Draw(Graphics graphics, bool current, bool fullAlpha)
        {
            Definition.Draw(graphics, Position, Size, Angle, fullAlpha ? DrawUtil.AlphaMode.Full : DrawUtil.AlphaMode.Half);

            //Selection box
            if (current && Ogmo.EntitySelectionWindow.IsSelected(this))
                graphics.DrawSelectionRectangle(Bounds);

            //Draw Nodes
            if (Nodes != null)
            {
                //Node ghost images
                if (Definition.NodesDefinition.Ghost)
                {
                    DrawUtil.AlphaMode alphaMode = fullAlpha ? DrawUtil.AlphaMode.Half : DrawUtil.AlphaMode.Quarter;
                    foreach (var p in Nodes)
                        Definition.Draw(graphics, p, Size, Angle, alphaMode);
                }

                switch (Definition.NodesDefinition.DrawMode)
                {
                    case EntityNodesDefinition.PathMode.None:
                        foreach (var p in Nodes)
                            graphics.DrawNode(p);
                        break;

                    case EntityNodesDefinition.PathMode.Path:
                        if (Nodes.Count > 0)
                        {
                            graphics.DrawLine(DrawUtil.NodePathPen, Position, Nodes[0]);
                            graphics.DrawNode(Nodes[0]);
                        }

                        for (int i = 1; i < Nodes.Count; i++)
                        {
                            graphics.DrawLine(DrawUtil.NodePathPen, Nodes[i - 1], Nodes[i]);
                            graphics.DrawNode(Nodes[i]);
                        }
                        break;

                    case EntityNodesDefinition.PathMode.Circuit:
                        if (Nodes.Count > 0)
                        {
                            graphics.DrawLine(DrawUtil.NodePathPen, Position, Nodes[0]);
                            graphics.DrawNode(Nodes[0]);
                        }

                        for (int i = 1; i < Nodes.Count; i++)
                        {
                            graphics.DrawLine(DrawUtil.NodePathPen, Nodes[i - 1], Nodes[i]);
                            graphics.DrawNode(Nodes[i]);
                        }

                        if (Nodes.Count > 1)
                            graphics.DrawLine(DrawUtil.NodePathPen, Nodes[Nodes.Count - 1], Position);
                        break;

                    case EntityNodesDefinition.PathMode.Fan:
                        foreach (var p in Nodes)
                        {
                            graphics.DrawLine(DrawUtil.NodePathPen, Position, p);
                            graphics.DrawNode(p);
                        }
                        break;
                }
            }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle(Position.X - Definition.Origin.X, Position.Y - Definition.Origin.Y, Size.Width, Size.Height); }
        }

        public Entity Clone()
        {
            return new Entity(Layer, this);
        }

        public void MoveNodes(Point move)
        {
            if (Nodes != null)
                for (int i = 0; i < Nodes.Count; i++)
                    Nodes[i] = new Point(Nodes[i].X + move.X, Nodes[i].Y + move.Y);
        }

    }
}
