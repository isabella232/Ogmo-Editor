using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OgmoEditor.Definitions.GroupDefinitions;
using OgmoEditor.Definitions.LayerDefinitions;

namespace OgmoEditor.ProjectEditors
{
	public partial class LayerDefinitionsEditor : UserControl, IProjectChanger
    {
        private const string NEW_LAYER_NAME = "NewLayer";

        private List<LayerDefinition> layerDefinitions;
        private UserControl layerEditor;
        private CommonGroupDefinition groups;
        private bool indexChangeable = true;

        public LayerDefinitionsEditor()
        {
            InitializeComponent();
            layerEditor = null;

            //Init the type combobox items
            foreach (var s in LayerDefinition.LAYER_NAMES)
                typeComboBox.Items.Add(s);
        }

        public void LoadFromProject(Project project)
        {
            layerDefinitions = project.LayerDefinitions;
            foreach (LayerDefinition d in layerDefinitions)
                listBox.Items.Add(d.Name);

            groups = project.LayerGroups;
            layerGroupComboBox.DataSource = groups.groupNames;
            itemGroupComboBox.DataSource = groups.GetValidGroupNames();
        }

        private void setControlsFromDefinition(LayerDefinition definition)
        {
            //Enabled stuff
            removeButton.Enabled = true;
            moveUpButton.Enabled = listBox.SelectedIndex > 0;
            moveDownButton.Enabled = listBox.SelectedIndex < listBox.Items.Count - 1;
            nameTextBox.Enabled = true;
            gridXTextBox.Enabled = true;
            gridYTextBox.Enabled = true;
            scrollXTextBox.Enabled = true;
            scrollYTextBox.Enabled = true;
            typeComboBox.Enabled = true;
            itemGroupComboBox.Enabled = true;

            //Load properties
            nameTextBox.Text = definition.Name;
            gridXTextBox.Text = definition.Grid.Width.ToString();
            gridYTextBox.Text = definition.Grid.Height.ToString();
            scrollXTextBox.Text = definition.ScrollFactor.X.ToString();
            scrollYTextBox.Text = definition.ScrollFactor.Y.ToString();
            typeComboBox.SelectedIndex = LayerDefinition.LAYER_TYPES.FindIndex(e => e == definition.GetType());

            //Remove the old layer editor
            if (layerEditor != null)
                Controls.Remove(layerEditor);

            //Add the new one
            layerEditor = definition.GetEditor();
            if (layerEditor != null)
            {
                layerEditor.TabIndex = 6;
                Controls.Add(layerEditor);
            }

            // Get index of group from group name
            var gntemp = groups.groupNames.Where(ob => ob == definition.GroupName);
            itemGroupComboBox.SelectedIndex = (gntemp.Count() > 0 && gntemp.Single() != "") ? itemGroupComboBox.Items.IndexOf(gntemp.Single()) : 0;
        }

        private void disableControls()
        {
            //Disable all
            removeButton.Enabled = false;
            moveUpButton.Enabled = false;
            moveDownButton.Enabled = false;
            nameTextBox.Enabled = false;
            gridXTextBox.Enabled = false;
            gridYTextBox.Enabled = false;
            scrollXTextBox.Enabled = false;
            scrollYTextBox.Enabled = false;
            typeComboBox.Enabled = false;
            itemGroupComboBox.Enabled = false;

            if (layerEditor != null)
            {
                Controls.Remove(layerEditor);
                layerEditor = null;
            }
        }

        private LayerDefinition getDefaultLayer()
        {
            int i = 0;
            string name;

            do
            {
                name = NEW_LAYER_NAME + i.ToString();
                i++;
            }
            while (layerNameTaken(name));

            GridLayerDefinition grid = new GridLayerDefinition();
            grid.Name = name;
            grid.Grid = new Size(16, 16);

            if (layerGroupComboBox.SelectedIndex > 1)
                grid.GroupName = layerGroupComboBox.SelectedItem.ToString();

            return grid;
        }

        private bool layerNameTaken(string name)
        {
            return layerDefinitions.Find(e => e.Name == name) != null;
        }

        #region Events

        private void addButton_Click(object sender, EventArgs e)
        {
            LayerDefinition def = getDefaultLayer();

            layerDefinitions.Add(def);
            listBox.SelectedIndex = -1;

            disableControls();
            RefreshListBoxItems();
            
            if (layerGroupComboBox.SelectedIndex == 0) // If the "All" group is selected
                listBox.SelectedIndex = layerDefinitions.IndexOf(def);
            else if (layerGroupComboBox.SelectedIndex > 0)
                listBox.SelectedIndex = GetLayerListForCurrentSelection().Count() - 1;

            if (layerDefinitions.Count == 0)
                removeButton.Enabled = true;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            int index = listBox.SelectedIndex;

            if (index < 0)
                return;

            int layID = GetLayerIDFromSelectedGroup();
            layerDefinitions.RemoveAt(layID);

            if (layerDefinitions.Count == 0)
                removeButton.Enabled = false;

            RefreshListBoxItems();
            listBox.SelectedIndex = Math.Min(listBox.Items.Count - 1, index);
        }

        private void moveUpButton_Click(object sender, EventArgs e)
        {
            int index = listBox.SelectedIndex;
            if (index == -1)
                return;

            if (layerGroupComboBox.SelectedIndex > 0)
            { // A specific group is selected
                int prev, cur;
                prev = cur = 0;
                int num = 0;
                foreach (var ent in layerDefinitions)
                    if (ent.GroupName == layerGroupComboBox.SelectedItem.ToString())
                    {
                        if (num == index - 1)
                            prev = layerDefinitions.IndexOf(ent);
                        else if (num == index)
                            cur = layerDefinitions.IndexOf(ent);
                        num++;
                    }

                LayerDefinition tmp = layerDefinitions[cur];
                layerDefinitions[cur] = layerDefinitions[prev];
                layerDefinitions[prev] = tmp;
            }
            else
            { // The "All" group is selected
                LayerDefinition temp = layerDefinitions[index];
                layerDefinitions[index] = layerDefinitions[index - 1];
                layerDefinitions[index - 1] = temp;
            }

            RefreshListBoxItems();
            listBox.SelectedIndex = index - 1;
        }

        private void moveDownButton_Click(object sender, EventArgs e)
        {
            int index = listBox.SelectedIndex;
            if (index == -1)
                return;

            if (layerGroupComboBox.SelectedIndex > 0)
            { // A specific group is selected
                int cur, next;
                cur = next = 0;
                int num = 0;
                foreach (var ent in layerDefinitions)
                    if (ent.GroupName == layerGroupComboBox.SelectedItem.ToString())
                    {
                        if (num == index)
                            cur = layerDefinitions.IndexOf(ent);
                        else if (num == index + 1)
                            next = layerDefinitions.IndexOf(ent);
                        num++;
                    }

                LayerDefinition tmp = layerDefinitions[next];
                layerDefinitions[next] = layerDefinitions[cur];
                layerDefinitions[cur] = tmp;
            }
            else
            { // The "All" group is selected
                LayerDefinition temp = layerDefinitions[index];
                layerDefinitions[index] = layerDefinitions[index + 1];
                layerDefinitions[index + 1] = temp;
            }

            RefreshListBoxItems();
            listBox.SelectedIndex = index + 1;
        }

        private void nameTextBox_Validated(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            indexChangeable = false;
            layerDefinitions[GetLayerIDFromSelectedGroup()].Name = nameTextBox.Text;
            listBox.Items[listBox.SelectedIndex] = (nameTextBox.Text == "" ? "(blank)" : nameTextBox.Text);
            indexChangeable = true;
        }

        private void gridXTextBox_Validated(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            OgmoParse.Parse(ref layerDefinitions[GetLayerIDFromSelectedGroup()].Grid, gridXTextBox, gridYTextBox);
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!indexChangeable)
                return;
            
            int index = listBox.SelectedIndex;
            if (index == -1)
            {
                disableControls();
                return;
            }

            var tempEntities = GetLayerListForCurrentSelection();
            setControlsFromDefinition(layerDefinitions[index]); // For some reason, the selected item was being deselected here sometimes
            listBox.SelectedIndex = index; // This is a fix so it forces the selection (if deselected)
        }

        private void typeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            LayerDefinition oldDef = layerDefinitions[GetLayerIDFromSelectedGroup()];
            LayerDefinition newDef;

            newDef = (LayerDefinition)Activator.CreateInstance(LayerDefinition.LAYER_TYPES[typeComboBox.SelectedIndex]);

            newDef.Name = oldDef.Name;
            newDef.Grid = oldDef.Grid;
            layerDefinitions[GetLayerIDFromSelectedGroup()] = newDef;
            setControlsFromDefinition(newDef);
        }

        private void scrollXTextBox_Validated(object sender, EventArgs e)
        {
            if (GetLayerIDFromSelectedGroup() == -1)
                return;

            OgmoParse.Parse(ref layerDefinitions[GetLayerIDFromSelectedGroup()].ScrollFactor, scrollXTextBox, scrollYTextBox);
        }

        private void addEntityGroupButton_Click(object sender, EventArgs e)
        {
            AddNewGroupForm frm = new AddNewGroupForm(groups);
            frm.ShowDialog();
            ResetGroupComboBoxes();
        }
        
        private void removeEntityGroupButton_Click(object sender, EventArgs e)
        {
            if (layerGroupComboBox.SelectedIndex < 2) // If "All" or "default" is selected, don't remove it
                return;

            var name = layerGroupComboBox.SelectedItem.ToString();
            groups.groupNames.Remove(name);
            foreach (var ent in layerDefinitions)
            {
                if (ent.GroupName == name)
                    ent.GroupName = "default";
            }

            layerGroupComboBox.SelectedIndex = 1; // Go to the "default" layer when removed
            ResetGroupComboBoxes();
            RefreshListBoxItems();
        }
        
        private void layerGroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = layerGroupComboBox.SelectedIndex;
            if (index == -1)
                return;
            if (index < 2) // Disable the remove button on "All" and "default"
                removeLayerGroupButton.Enabled = false;
            else
                removeLayerGroupButton.Enabled = true;
            RefreshListBoxItems();
        }

        private void itemGroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            layerDefinitions[GetLayerIDFromSelectedGroup()].GroupName = itemGroupComboBox.SelectedItem.ToString();

            RefreshListBoxItems();
        }

        private void LayerDefinitionsEditor_Click(object sender, EventArgs e)
        {
            listBox.SelectedIndex = -1;
            disableControls();
        }
        
        #endregion

        #region Misc functions

        /// <summary>
        /// Reasigns values to combo boxes to update them.
        /// </summary>
        private void ResetGroupComboBoxes()
        {
            layerGroupComboBox.DataSource = null; // This is needed so the changes take effect
            layerGroupComboBox.DataSource = groups.groupNames;
            itemGroupComboBox.DataSource = null;
            itemGroupComboBox.DataSource = groups.GetValidGroupNames();
        }

        /// <summary>
        /// Resets the items in the list box for the currently selected group.
        /// </summary>
        private void RefreshListBoxItems()
        {
            int index = layerGroupComboBox.SelectedIndex;
            listBox.Items.Clear();
            foreach (var e in GetLayerListForCurrentSelection())
                listBox.Items.Add(e.Name);
        }

        /// <summary>
        /// Returns the filtered list of layers by their group name (simulating groups).
        /// </summary>
        /// <returns>The filtered list of layers of the same group.</returns>
        private List<LayerDefinition> GetLayerListForCurrentSelection()
        {
            int gIndex = layerGroupComboBox.SelectedIndex;
            List<LayerDefinition> temp;

            if (gIndex < 1) // Show all (when gIndex is 0 or -1)
                temp = layerDefinitions;
            else if (gIndex == 1) // Show default items
                temp = layerDefinitions.Where(ob => ob.GroupName == "" || ob.GroupName == "default").ToList();
            else // Show items from specific group
                temp = layerDefinitions.Where(ob => ob.GroupName == layerGroupComboBox.SelectedItem.ToString()).ToList();

            return temp;
        }

        /// <summary>
        /// This will return the appropriate layer when a group is selected.
        /// </summary>
        /// <returns>The correct layer inside the group</returns>
        private LayerDefinition GetLayerFromSelectedGroup()
        {
            int selectIndex = listBox.SelectedIndex;
            if (selectIndex < 0)
                return null;

            if (layerGroupComboBox.SelectedIndex == 0) // If "All" selected
                return layerDefinitions[selectIndex];

            int num = 0;
            LayerDefinition result = null;

            foreach (LayerDefinition e in layerDefinitions)
                if (e.GroupName == layerGroupComboBox.SelectedItem.ToString())
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
        /// This will return the position of the layer inside the layers list based on the current selection inside a group.
        /// </summary>
        /// <returns>The correct id of layer inside the group</returns>
        private int GetLayerIDFromSelectedGroup()
        {
            int selectIndex = listBox.SelectedIndex;
            if (selectIndex < 0)
                return -1;

            if (layerGroupComboBox.SelectedIndex == 0) // If "All" selected
                return selectIndex;

            int num = 0;
            int result = -1;

            foreach (LayerDefinition e in layerDefinitions)
                if (e.GroupName == layerGroupComboBox.SelectedItem.ToString())
                {
                    if (num == selectIndex)
                    {
                        result = layerDefinitions.IndexOf(e);
                        break;
                    }
                    num++;
                }

            return result;
        }

        #endregion

    }
}
