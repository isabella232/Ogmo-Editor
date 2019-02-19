using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OgmoEditor.Definitions;
using System.IO;
using OgmoEditor.Definitions.GroupDefinitions;
using System.Linq;

namespace OgmoEditor.ProjectEditors
{
	public partial class EntityDefinitionsEditor : UserControl, IProjectChanger
	{
		private const string NEW_NAME = "NewObject";

		private List<EntityDefinition> entities;
        private CommonGroupDefinition groups;
		private string directory;

		public EntityDefinitionsEditor()
		{
			InitializeComponent();
		}

		public void LoadFromProject(Project project)
		{
			entities = project.EntityDefinitions;
			foreach (var o in entities)
				listBox.Items.Add(o.Name);

			directory = project.SavedDirectory;

            groups = project.EntityGroups;
            entityGroupComboBox.DataSource = groups.groupNames;
            itemGroupComboBox.DataSource = groups.GetValidGroupNames();
        }

		private void SetControlsFromObject(EntityDefinition def)
		{
			removeButton.Enabled = true;
			moveUpButton.Enabled = listBox.SelectedIndex > 0;
			moveDownButton.Enabled = listBox.SelectedIndex < listBox.Items.Count - 1;

			nameTextBox.Enabled = true;
			limitTextBox.Enabled = true;
			sizeXTextBox.Enabled = true;
			sizeYTextBox.Enabled = true;
			originXTextBox.Enabled = true;
			originYTextBox.Enabled = true;
			resizableXCheckBox.Enabled = true;
			resizableYCheckBox.Enabled = true;
			rotatableCheckBox.Enabled = true;
			valuesEditor.Enabled = true;
			nodesCheckBox.Enabled = true;
			graphicTypeComboBox.Enabled = true;
            itemGroupComboBox.Enabled = true;

            //Basics
            nameTextBox.Text = def.Name;
			limitTextBox.Text = def.Limit.ToString();
			sizeXTextBox.Text = def.Size.Width.ToString();
			sizeYTextBox.Text = def.Size.Height.ToString();
			originXTextBox.Text = def.Origin.X.ToString();
			originYTextBox.Text = def.Origin.Y.ToString();

			//Resizable/rotation
			resizableXCheckBox.Checked = def.ResizableX;
			resizableYCheckBox.Checked = def.ResizableY;
			rotatableCheckBox.Checked = def.Rotatable;
			rotationIncrementTextBox.Text = def.RotateIncrement.ToString();
			RotationFieldsVisible = def.Rotatable;

			//Nodes
			nodesCheckBox.Checked = def.NodesDefinition.Enabled;
			nodeLimitTextBox.Text = def.NodesDefinition.Limit.ToString();
			nodeDrawComboBox.SelectedIndex = (int)def.NodesDefinition.DrawMode;
			nodeGhostCheckBox.Checked = def.NodesDefinition.Ghost;
			NodesFieldsVisible = def.NodesDefinition.Enabled;

			//Values
			valuesEditor.SetList(def.ValueDefinitions);

			//Graphic
			graphicTypeComboBox.SelectedIndex = (int)def.ImageDefinition.DrawMode;
			GraphicFieldsVisibility = (int)def.ImageDefinition.DrawMode;
			rectangleColorChooser.Color = def.ImageDefinition.RectColor;
			imageFileTextBox.Text = def.ImageDefinition.ImagePath;
			imageFileTiledCheckBox.Checked = def.ImageDefinition.Tiled;
			imageFileWarningLabel.Visible = !CheckImageFile();
			LoadImageFilePreview();

            // Get index of group from group name
            var gntemp = groups.groupNames.Where(ob => ob == def.GroupName);
            itemGroupComboBox.SelectedIndex = (gntemp.Count()>0 && gntemp.Single()!="") ? itemGroupComboBox.Items.IndexOf(gntemp.Single()) : 0;

        }

		private void DisableControls()
		{
			removeButton.Enabled = false;
			moveUpButton.Enabled = false;
			moveDownButton.Enabled = false;

			nameTextBox.Enabled = false;
			limitTextBox.Enabled = false;
			sizeXTextBox.Enabled = false;
			sizeYTextBox.Enabled = false;
			originXTextBox.Enabled = false;
			originYTextBox.Enabled = false;
			resizableXCheckBox.Enabled = false;
			resizableYCheckBox.Enabled = false;
			rotatableCheckBox.Enabled = false;
			nodesCheckBox.Enabled = false;
			valuesEditor.Enabled = false;
			graphicTypeComboBox.Enabled = false;
            itemGroupComboBox.Enabled = false;

            RotationFieldsVisible = false;
			NodesFieldsVisible = false;
			GraphicFieldsVisibility = -1;
			ClearImageFilePreview();
		}

		private EntityDefinition GetDefault()
		{
			EntityDefinition def = new EntityDefinition();

			int i = 0;
			string name;

			do
			{
				name = NEW_NAME + i.ToString();
				i++;
			}
			while (entities.Find(o => o.Name == name) != null);

			def.Name = name;

            // Add to appropriate group
            if (entityGroupComboBox.SelectedIndex > 1)
                def.GroupName = entityGroupComboBox.SelectedItem.ToString();

			return def;
		}

		private bool RotationFieldsVisible
		{
			set
			{
				rotationIncrementLabel.Visible = rotationIncrementTextBox.Enabled = rotationIncrementTextBox.Visible = value;
			}
		}

		private bool NodesFieldsVisible
		{
			set
			{
				nodeLimitTextBox.Visible = nodeLimitTextBox.Enabled = value;
				nodeLimitLabel.Visible = value;
				nodeDrawComboBox.Visible = nodeDrawComboBox.Enabled = value;
				nodeDrawLabel.Visible = value;
				nodeGhostCheckBox.Visible = value;
			}
		}

		private int GraphicFieldsVisibility
		{
			set
			{
				rectangleGraphicPanel.Visible = rectangleGraphicPanel.Enabled = (value == 0);
				imageFileGraphicPanel.Visible = imageFileGraphicPanel.Enabled = (value == 1);
			}
		}

		private bool CheckImageFile()
		{
			return File.Exists(Path.Combine(directory, imageFileTextBox.Text));
		}

		private void LoadImageFilePreview()
		{
			imagePreviewer.LoadImage(Path.Combine(directory, imageFileTextBox.Text));
		}

		private void ClearImageFilePreview()
		{
			imagePreviewer.ClearImage();
		}

		#region Selector Events

		private void listBox_SelectedIndexChanged(object sender, EventArgs e)
		{
            int index = listBox.SelectedIndex;
            if (index==-1)
                return;
            
            var tempEntities = GetEntityListForCurrentSelection(); 
            SetControlsFromObject(tempEntities[index]); // For some reason, the selected item was being deselected here sometimes
            listBox.SelectedIndex = index; // This is a fix so it forces the selection (if deselected)
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			EntityDefinition def = GetDefault();
			entities.Add(def);
            listBox.SelectedIndex = -1;

            DisableControls();
            RefreshListBoxItems();
            
            if (entityGroupComboBox.SelectedIndex == 0) // If the "All" group is selected
                listBox.SelectedIndex = entities.IndexOf(def);
            else if (entityGroupComboBox.SelectedIndex > 0)
                listBox.SelectedIndex = GetEntityListForCurrentSelection().Count() - 1;

            if (entities.Count == 0)
                removeButton.Enabled = true;

        }

		private void removeButton_Click(object sender, EventArgs e)
		{
			int index = listBox.SelectedIndex;
            if (index < 0)
                return;
            
            int entID = GetEntityIDFromSelectedGroup();
            entities.RemoveAt(entID);

            if (entities.Count == 0)
                removeButton.Enabled = false;

            RefreshListBoxItems();
			listBox.SelectedIndex = Math.Min(listBox.Items.Count - 1, index);
		}

		private void moveUpButton_Click(object sender, EventArgs e)
		{
			int index = listBox.SelectedIndex;
            if (index == -1)
                return;

            if (entityGroupComboBox.SelectedIndex > 0)
            { // A specific group is selected
                int prev, cur;
                prev = cur = 0;
                int num = 0;
                foreach (var ent in entities)
                    if (ent.GroupName == entityGroupComboBox.SelectedItem.ToString())
                    {
                        if (num == index - 1)
                            prev = entities.IndexOf(ent);
                        else if (num == index)
                            cur = entities.IndexOf(ent);
                        num++;
                    }

                EntityDefinition tmp = entities[cur];
                entities[cur] = entities[prev];
                entities[prev] = tmp;
            }
            else
            { // The "All" group is selected
                EntityDefinition temp = entities[index];
                entities[index] = entities[index - 1];
                entities[index - 1] = temp;
            }
            
            RefreshListBoxItems();
            listBox.SelectedIndex = index - 1;
		}

		private void moveDownButton_Click(object sender, EventArgs e)
		{
			int index = listBox.SelectedIndex;
            if (index == -1)
                return;

            if (entityGroupComboBox.SelectedIndex > 0)
            { // A specific group is selected
                int cur, next;
                cur = next = 0;
                int num = 0;
                foreach (var ent in entities)
                    if (ent.GroupName == entityGroupComboBox.SelectedItem.ToString())
                    {
                        if (num == index)
                            cur = entities.IndexOf(ent);
                        else if (num == index+1)
                            next = entities.IndexOf(ent);
                        num++;
                    }

                EntityDefinition tmp = entities[next];
                entities[next] = entities[cur];
                entities[cur] = tmp;
            }
            else
            { // The "All" group is selected
                EntityDefinition temp = entities[index];
                entities[index] = entities[index + 1];
                entities[index + 1] = temp;
            }

            RefreshListBoxItems();
            listBox.SelectedIndex = index + 1;
		}

        private void entityGroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = entityGroupComboBox.SelectedIndex;
            if (index == -1)
                return;
            if (index < 2) // Disable the remove button on "All" and "default"
                removeEntityGroupButton.Enabled = false;
            else
                removeEntityGroupButton.Enabled = true;
            RefreshListBoxItems();
        }

        private void addEntityGroupButton_Click(object sender, EventArgs e)
        {
            AddNewGroupForm frm = new AddNewGroupForm(groups);
            frm.ShowDialog();
            ResetGroupComboBoxes();
        }

        private void removeEntityGroupButton_Click(object sender, EventArgs e)
        {
            if (entityGroupComboBox.SelectedIndex < 2) // If "All" or "default" is selected, don't remove it
                return;

            var name = entityGroupComboBox.SelectedItem.ToString();
            groups.groupNames.Remove(name);
            foreach(var ent in entities)
            {
                if (ent.GroupName == name)
                    ent.GroupName = "default";
            }

            entityGroupComboBox.SelectedIndex = 1; // Go to the "default" layer when removed
            ResetGroupComboBoxes();
            RefreshListBoxItems();
        }

        private void itemGroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;
            
            entities[GetEntityIDFromSelectedGroup()].GroupName = itemGroupComboBox.SelectedItem.ToString();

            RefreshListBoxItems();
        }
        
        private void EntityDefinitionsEditor_Click(object sender, EventArgs e)
        {
            // If we click on the form, deselect the currently selected item
            listBox.SelectedIndex = -1;
            DisableControls();
        }
        
        #endregion

        #region Basic Settings Events

        private void nameTextBox_Validated(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;

            if (String.IsNullOrWhiteSpace(nameTextBox.Text))
                return;

            entities[GetEntityIDFromSelectedGroup()].Name = nameTextBox.Text;
			listBox.Items[listBox.SelectedIndex] = nameTextBox.Text;
		}

        private void nameTextBox_Leave(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            if (String.IsNullOrWhiteSpace(nameTextBox.Text))
                return;

            entities[GetEntityIDFromSelectedGroup()].Name = nameTextBox.Text;
            listBox.Items[listBox.SelectedIndex] = nameTextBox.Text;
        }

        private void limitTextBox_Validated(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;
            
			OgmoParse.Parse(ref entities[GetEntityIDFromSelectedGroup()].Limit, limitTextBox);
		}

		private void sizeXTextBox_Validated(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;
            
            OgmoParse.Parse(ref entities[GetEntityIDFromSelectedGroup()].Size, sizeXTextBox, sizeYTextBox);
		}

		private void originXTextBox_Validated(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;
            
            OgmoParse.Parse(ref entities[GetEntityIDFromSelectedGroup()].Origin, originXTextBox, originYTextBox);
		}

		#endregion

		#region Size/Rotate Events

		private void resizableXCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;
            
			entities[GetEntityIDFromSelectedGroup()].ResizableX = resizableXCheckBox.Checked;
		}

		private void resizableYCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;

			entities[GetEntityIDFromSelectedGroup()].ResizableY = resizableYCheckBox.Checked;
		}

		private void rotatableCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;

			entities[GetEntityIDFromSelectedGroup()].Rotatable = rotatableCheckBox.Checked;
			RotationFieldsVisible = rotatableCheckBox.Checked;
		}

		private void rotationIncrementTextBox_Validated(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;

			OgmoParse.Parse(ref entities[GetEntityIDFromSelectedGroup()].RotateIncrement, rotationIncrementTextBox);
		}

		#endregion

		#region Nodes Events

		private void nodesCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;

			entities[GetEntityIDFromSelectedGroup()].NodesDefinition.Enabled = nodesCheckBox.Checked;
			NodesFieldsVisible = nodesCheckBox.Checked;
		}

		private void nodeLimitTextBox_Validated(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;

			OgmoParse.Parse(ref entities[GetEntityIDFromSelectedGroup()].NodesDefinition.Limit, nodeLimitTextBox);
		}

		private void nodeDrawComboBox_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;

			entities[GetEntityIDFromSelectedGroup()].NodesDefinition.DrawMode = (EntityNodesDefinition.PathMode)nodeDrawComboBox.SelectedIndex;
		}

		private void nodeGhostCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			entities[GetEntityIDFromSelectedGroup()].NodesDefinition.Ghost = nodeGhostCheckBox.Checked;
		}

		#endregion

		#region Graphics Events

		private void graphicTypeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;

			entities[GetEntityIDFromSelectedGroup()].ImageDefinition.DrawMode = (EntityImageDefinition.DrawModes)graphicTypeComboBox.SelectedIndex;
			GraphicFieldsVisibility = graphicTypeComboBox.SelectedIndex;
		}

		private void rectangleColorChooser_ColorChanged(OgmoColor color)
		{
			if (listBox.SelectedIndex == -1)
				return;

			entities[GetEntityIDFromSelectedGroup()].ImageDefinition.RectColor = color;
		}

		private void imageFileTiledCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;

			entities[GetEntityIDFromSelectedGroup()].ImageDefinition.Tiled = imageFileTiledCheckBox.Checked;
		}

		private void imageFileButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = Ogmo.IMAGE_FILE_FILTER;
			dialog.CheckFileExists = true;

			if (CheckImageFile())
				dialog.InitialDirectory = Util.DirectoryPath(Path.Combine(directory, imageFileTextBox.Text));
			else
				dialog.InitialDirectory = directory;

			if (dialog.ShowDialog() == DialogResult.Cancel)
				return;

			imageFileTextBox.Text = Util.RelativePath(directory, dialog.FileName);
			imageFileWarningLabel.Visible = !CheckImageFile();
			LoadImageFilePreview();

			entities[GetEntityIDFromSelectedGroup()].ImageDefinition.ImagePath = imageFileTextBox.Text;
		}

        #endregion

        #region Misc Functions

        /// <summary>
        /// Reasigns values to combo boxes to update them.
        /// </summary>
        private void ResetGroupComboBoxes()
        {
            entityGroupComboBox.DataSource = null; // This is needed so the changes take effect
            entityGroupComboBox.DataSource = groups.groupNames;
            itemGroupComboBox.DataSource = null;
            itemGroupComboBox.DataSource = groups.GetValidGroupNames();
        }

        /// <summary>
        /// Resets the items in the list box for the currently selected group.
        /// </summary>
        private void RefreshListBoxItems()
        {
            int index = entityGroupComboBox.SelectedIndex;
            listBox.Items.Clear();
            foreach (var e in GetEntityListForCurrentSelection())
                listBox.Items.Add(e.Name);
        }

        /// <summary>
        /// Returns the filtered list of entities by their group name (simulating groups).
        /// </summary>
        /// <returns>The filtered list of entities of the same group.</returns>
        private List<EntityDefinition> GetEntityListForCurrentSelection()
        {
            int gIndex = entityGroupComboBox.SelectedIndex;
            List<EntityDefinition> tempEntities;

            if (gIndex < 1) // Show all (when gIndex is 0 or -1)
                tempEntities = entities;
            else if (gIndex == 1) // Show default items
                tempEntities = entities.Where(ob => ob.GroupName == "" || ob.GroupName == "default").ToList();
            else // Show items from specific group
                tempEntities = entities.Where(ob => ob.GroupName == entityGroupComboBox.SelectedItem.ToString()).ToList();

            return tempEntities;
        }

        /// <summary>
        /// This will return the appropriate entity when a group is selected.
        /// </summary>
        /// <returns>The correct entity inside the group</returns>
        private EntityDefinition GetEntityFromSelectedGroup()
        {
            int selectIndex = listBox.SelectedIndex;
            if (selectIndex < 0)
                return null;

            if (entityGroupComboBox.SelectedIndex == 0) // If "All" selected
                return entities[selectIndex];

            int num = 0;
            EntityDefinition result = null;

            foreach (EntityDefinition e in entities)
                if (e.GroupName == entityGroupComboBox.SelectedItem.ToString())
                {
                    if (num == selectIndex)
                    {
                        result = e;
                        break;
                    }
                    num++;
                }

            return result;
        }

        /// <summary>
        /// This will return the position of the entity inside the entities list based on the current selection inside a group.
        /// </summary>
        /// <returns>The correct entity inside the group</returns>
        private int GetEntityIDFromSelectedGroup()
        {
            int selectIndex = listBox.SelectedIndex;
            if (selectIndex < 0)
                return -1;

            if (entityGroupComboBox.SelectedIndex == 0) // If "All" selected
                return selectIndex;

            int num = 0;
            int result = -1;

            foreach (EntityDefinition e in entities)
                if (e.GroupName == entityGroupComboBox.SelectedItem.ToString())
                {
                    if (num == selectIndex)
                    {
                        result = entities.IndexOf(e);
                        break;
                    }
                    num++;
                }

            return result;
        }


        #endregion

    }
}

