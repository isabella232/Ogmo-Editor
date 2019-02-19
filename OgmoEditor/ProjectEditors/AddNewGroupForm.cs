using OgmoEditor.Definitions.GroupDefinitions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OgmoEditor.ProjectEditors
{
    public partial class AddNewGroupForm : Form
    {
        #region Fields

        private CommonGroupDefinition Groups;

        #endregion

        #region Setup

        public AddNewGroupForm(CommonGroupDefinition groups)
        {
            InitializeComponent();
            Groups = groups;
        }

        public AddNewGroupForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void AddGroupButton_Click(object sender, EventArgs e)
        {
            string groupName = groupNameTextBox.Text;

            if (String.IsNullOrWhiteSpace(groupName))
                return;
            if (Groups.groupNames.Select(ob => ob.ToLower()).Where(ob => ob == groupName.ToLower()).Count() > 0)
            {
                MessageBox.Show("Group name already exists.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Groups.groupNames.Add(groupName);
            Close();
        }

        #endregion

    }
}
