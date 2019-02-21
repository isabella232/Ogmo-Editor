using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OgmoEditor.Definitions;
using System.IO;
using OgmoEditor.Definitions.GroupDefinitions;
using System.Linq;

namespace OgmoEditor.ProjectEditors
{
	public partial class TilesetsEditor : UserControl, IProjectChanger
	{
		private const string DEFAULT_NAME = "NewTileset";

		private List<Tileset> tilesets;
        private CommonGroupDefinition groups;
        private string directory;

		public TilesetsEditor()
		{
			InitializeComponent();
		}

		public void LoadFromProject(Project project)
		{
			tilesets = project.Tilesets;
			foreach (var t in tilesets)
				listBox.Items.Add(t.Name);

			directory = project.SavedDirectory;

            groups = project.TilesetGroups;
            tilesetGroupComboBox.DataSource = groups.groupNames;
            itemGroupComboBox.DataSource = groups.GetValidGroupNames();
		}

		private void setControlsFromTileset(Tileset t)
		{
			removeButton.Enabled = true;
			moveUpButton.Enabled = listBox.SelectedIndex > 0;
			moveDownButton.Enabled = listBox.SelectedIndex < listBox.Items.Count - 1;

			imagePreviewer.Enabled = true;
			nameTextBox.Enabled = true;
			imageFileTextBox.Enabled = true;
			imageFileButton.Enabled = true;
			imageFileWarningLabel.Enabled = true;
			tileSizeXTextBox.Enabled = true;
			tileSizeYTextBox.Enabled = true;
			tileSpacingTextBox.Enabled = true;
            itemGroupComboBox.Enabled = true;


            nameTextBox.Text = t.Name;
			imageFileTextBox.Text = t.FilePath;
			tileSizeXTextBox.Text = t.TileSize.Width.ToString();
			tileSizeYTextBox.Text = t.TileSize.Height.ToString();
			tileSpacingTextBox.Text = t.TileSep.ToString();

			LoadPreview();

            // Get index of group from group name
            var gntemp = groups.groupNames.Where(ob => ob == t.GroupName);
            itemGroupComboBox.SelectedIndex = (gntemp.Count() > 0 && gntemp.Single() != "") ? itemGroupComboBox.Items.IndexOf(gntemp.Single()) : 0;
        }

		private void disableControls()
		{
			removeButton.Enabled = false;
			moveUpButton.Enabled = false;
			moveDownButton.Enabled = false;

			imagePreviewer.Enabled = false;
			nameTextBox.Enabled = false;
			imageFileTextBox.Enabled = false;
			imageFileButton.Enabled = false;
			imageFileWarningLabel.Enabled = false;
			tileSizeXTextBox.Enabled = false;
			tileSizeYTextBox.Enabled = false;
			tileSpacingTextBox.Enabled = false;
            itemGroupComboBox.Enabled = false;


            imageFileWarningLabel.Visible = false;
			clearPreview();
		}

		private string getNewName()
		{
			int i = 0;
			string name;

			do
			{
				name = DEFAULT_NAME + i.ToString();
				i++;
			}
			while (tilesets.Find(t => t.Name == name) != null);


			return name;
		}

		private void LoadPreview()
		{
			tilesets[GetTilesetIDFromSelectedGroup()].GenerateBitmap();
			Bitmap bitmap = tilesets[GetTilesetIDFromSelectedGroup()].GetBitmap();
			if (bitmap == null)
			{
				imageSizeLabel.Visible = false;
				totalTilesLabel.Visible = false;
				imagePreviewer.ClearImage();
			}
			else
			{
				imagePreviewer.LoadImage(bitmap);
				imageSizeLabel.Visible = true;
				imageSizeLabel.Text = "Image Size: " + tilesets[GetTilesetIDFromSelectedGroup()].Size.Width + " x " + tilesets[GetTilesetIDFromSelectedGroup()].Size.Height;
				totalTilesLabel.Visible = true;
				updateTotalTiles();
			}

			imageFileWarningLabel.Visible = !imagePreviewer.BitmapValid;
		}

		private void clearPreview()
		{
			imagePreviewer.ClearImage();
			imageSizeLabel.Visible = false;
			totalTilesLabel.Visible = false;
		}

		private void updateTotalTiles()
		{
			totalTilesLabel.Text = "Tiles: " + tilesets[GetTilesetIDFromSelectedGroup()].TilesAcross.ToString() + " x " + tilesets[GetTilesetIDFromSelectedGroup()].TilesDown.ToString() + " (" + tilesets[GetTilesetIDFromSelectedGroup()].TilesTotal.ToString() + " total)";
		}

        #region Events

        private void addButton_Click(object sender, EventArgs e)
		{
			Tileset t = new Tileset();
			t.Name = getNewName();

            if (tilesetGroupComboBox.SelectedIndex > 1)
                t.GroupName = tilesetGroupComboBox.SelectedItem.ToString();

            tilesets.Add(t);
            listBox.SelectedIndex = -1;

            disableControls();
            RefreshListBoxItems();
            
            if (tilesetGroupComboBox.SelectedIndex == 0) // If the "All" group is selected
                listBox.SelectedIndex = tilesets.IndexOf(t);
            else if (tilesetGroupComboBox.SelectedIndex > 0)
                listBox.SelectedIndex = GetTilesetListForCurrentSelection().Count() - 1;

            if (tilesets.Count == 0)
                removeButton.Enabled = true;
        }

		private void removeButton_Click(object sender, EventArgs e)
		{
			int index = listBox.SelectedIndex;
            if (index < 0)
                return;

            int tID = GetTilesetIDFromSelectedGroup();
            tilesets.RemoveAt(tID);

            if (tilesets.Count == 0)
                removeButton.Enabled = false;

            RefreshListBoxItems();
            listBox.SelectedIndex = Math.Min(listBox.Items.Count - 1, index);
        }

		private void moveUpButton_Click(object sender, EventArgs e)
		{
            int index = listBox.SelectedIndex;
            if (index == -1)
                return;

            if (tilesetGroupComboBox.SelectedIndex > 0)
            { // A specific group is selected
                int prev, cur;
                prev = cur = 0;
                int num = 0;
                foreach (var ent in tilesets)
                    if (ent.GroupName == tilesetGroupComboBox.SelectedItem.ToString())
                    {
                        if (num == index - 1)
                            prev = tilesets.IndexOf(ent);
                        else if (num == index)
                            cur = tilesets.IndexOf(ent);
                        num++;
                    }

                Tileset tmp = tilesets[cur];
                tilesets[cur] = tilesets[prev];
                tilesets[prev] = tmp;
            }
            else
            { // The "All" group is selected
                Tileset temp = tilesets[index];
                tilesets[index] = tilesets[index - 1];
                tilesets[index - 1] = temp;
            }

            RefreshListBoxItems();
            listBox.SelectedIndex = index - 1;
        }

		private void moveDownButton_Click(object sender, EventArgs e)
		{
            int index = listBox.SelectedIndex;
            if (index == -1)
                return;

            if (tilesetGroupComboBox.SelectedIndex > 0)
            { // A specific group is selected
                int cur, next;
                cur = next = 0;
                int num = 0;
                foreach (var ent in tilesets)
                    if (ent.GroupName == tilesetGroupComboBox.SelectedItem.ToString())
                    {
                        if (num == index)
                            cur = tilesets.IndexOf(ent);
                        else if (num == index + 1)
                            next = tilesets.IndexOf(ent);
                        num++;
                    }

                Tileset tmp = tilesets[next];
                tilesets[next] = tilesets[cur];
                tilesets[cur] = tmp;
            }
            else
            { // The "All" group is selected
                Tileset temp = tilesets[index];
                tilesets[index] = tilesets[index + 1];
                tilesets[index + 1] = temp;
            }

            RefreshListBoxItems();
            listBox.SelectedIndex = index + 1;
        }

		private void listBox_SelectedIndexChanged(object sender, EventArgs e)
		{
            int index = listBox.SelectedIndex;
            if (index == -1)
            {
                disableControls();
                return;
            }

            var tempEntities = GetTilesetListForCurrentSelection();
            setControlsFromTileset(tempEntities[index]); // For some reason, the selected item was being deselected here sometimes
            listBox.SelectedIndex = index; // This is a fix so it forces the selection (if deselected)
        }

		private void nameTextBox_Validated(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;

			tilesets[GetTilesetIDFromSelectedGroup()].Name = nameTextBox.Text;
			listBox.Items[listBox.SelectedIndex] = nameTextBox.Text;
		}

		private void tileSizeXTextBox_Validated(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;

			OgmoParse.Parse(ref tilesets[GetTilesetIDFromSelectedGroup()].TileSize, tileSizeXTextBox, tileSizeYTextBox);
			updateTotalTiles();
		}

		private void tileSpacingTextBox_Validated(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1)
				return;

			OgmoParse.Parse(ref tilesets[GetTilesetIDFromSelectedGroup()].TileSep, tileSpacingTextBox);
			updateTotalTiles();
		}

		private void imageFileButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = Ogmo.IMAGE_FILE_FILTER;
			dialog.CheckFileExists = true;

			if (File.Exists(Path.Combine(directory, imageFileTextBox.Text)))
				dialog.InitialDirectory = Path.Combine(directory, imageFileTextBox.Text);
			else
				dialog.InitialDirectory = directory;

			if (dialog.ShowDialog() == DialogResult.Cancel)
				return;

			imageFileTextBox.Text = Util.RelativePath(directory, dialog.FileName);
			tilesets[GetTilesetIDFromSelectedGroup()].SetFilePath(imageFileTextBox.Text);

			LoadPreview();
		}

        private void addEntityGroupButton_Click(object sender, EventArgs e)
        {
            AddNewGroupForm frm = new AddNewGroupForm(groups);
            frm.ShowDialog();
            ResetGroupComboBoxes();
        }

        private void removeTilesetGroupButton_Click(object sender, EventArgs e)
        {
            if (tilesetGroupComboBox.SelectedIndex < 2) // If "All" or "default" is selected, don't remove it
                return;

            var name = tilesetGroupComboBox.SelectedItem.ToString();
            groups.groupNames.Remove(name);
            foreach (var ent in tilesets)
            {
                if (ent.GroupName == name)
                    ent.GroupName = "default";
            }

            tilesetGroupComboBox.SelectedIndex = 1; // Go to the "default" layer when removed
            ResetGroupComboBoxes();
            RefreshListBoxItems();
        }

        private void tilesetGroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = tilesetGroupComboBox.SelectedIndex;
            if (index == -1)
                return;
            if (index < 2) // Disable the remove button on "All" and "default"
                removeTilesetGroupButton.Enabled = false;
            else
                removeTilesetGroupButton.Enabled = true;
            RefreshListBoxItems();
        }

        private void itemGroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            tilesets[GetTilesetIDFromSelectedGroup()].GroupName = itemGroupComboBox.SelectedItem.ToString();

            RefreshListBoxItems();
        }
        
        private void TilesetsEditor_Click(object sender, EventArgs e)
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
            tilesetGroupComboBox.DataSource = null; // This is needed so the changes take effect
            tilesetGroupComboBox.DataSource = groups.groupNames;
            itemGroupComboBox.DataSource = null;
            itemGroupComboBox.DataSource = groups.GetValidGroupNames();
        }

        /// <summary>
        /// Resets the items in the list box for the currently selected group.
        /// </summary>
        private void RefreshListBoxItems()
        {
            int index = tilesetGroupComboBox.SelectedIndex;
            listBox.Items.Clear();
            foreach (var e in GetTilesetListForCurrentSelection())
                listBox.Items.Add(e.Name);
        }

        /// <summary>
        /// Returns the filtered list of tilesets by their group name (simulating groups).
        /// </summary>
        /// <returns>The filtered list of tilesets of the same group.</returns>
        private List<Tileset> GetTilesetListForCurrentSelection()
        {
            int gIndex = tilesetGroupComboBox.SelectedIndex;
            List<Tileset> temp;

            if (gIndex < 1) // Show all (when gIndex is 0 or -1)
                temp = tilesets;
            else if (gIndex == 1) // Show default items
                temp = tilesets.Where(ob => ob.GroupName == "" || ob.GroupName == "default").ToList();
            else // Show items from specific group
                temp = tilesets.Where(ob => ob.GroupName == tilesetGroupComboBox.SelectedItem.ToString()).ToList();

            return temp;
        }

        /// <summary>
        /// This will return the appropriate tileset when a group is selected.
        /// </summary>
        /// <returns>The correct tileset inside the group</returns>
        private Tileset GetTilesetFromSelectedGroup()
        {
            int selectIndex = listBox.SelectedIndex;
            if (selectIndex < 0)
                return null;

            if (tilesetGroupComboBox.SelectedIndex == 0) // If "All" selected
                return tilesets[selectIndex];

            int num = 0;
            Tileset result = null;

            foreach (Tileset e in tilesets)
                if (e.GroupName == tilesetGroupComboBox.SelectedItem.ToString())
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
        /// This will return the position of the tielset inside the tileset list based on the current selection inside a group.
        /// </summary>
        /// <returns>The correct id of tileset inside the group</returns>
        private int GetTilesetIDFromSelectedGroup()
        {
            int selectIndex = listBox.SelectedIndex;
            if (selectIndex < 0)
                return -1;

            if (tilesetGroupComboBox.SelectedIndex == 0) // If "All" selected
                return selectIndex;

            int num = 0;
            int result = -1;

            foreach (Tileset e in tilesets)
                if (e.GroupName == tilesetGroupComboBox.SelectedItem.ToString())
                {
                    if (num == selectIndex)
                    {
                        result = tilesets.IndexOf(e);
                        break;
                    }
                    num++;
                }

            return result;
        }


        #endregion

    }
}
