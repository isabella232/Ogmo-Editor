using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OgmoEditor.Definitions.LayerDefinitions;
using System.Diagnostics;

namespace OgmoEditor.ProjectEditors.LayerDefinitionEditors
{
    public partial class TileLayerDefinitionEditor : UserControl
    {
        private TileLayerDefinition def;

        public TileLayerDefinitionEditor(TileLayerDefinition def)
        {
            this.def = def;
            InitializeComponent();
            Location = new Point(206, 128);

            // Add the options for the dropdown
            exportModeComboBox.Items.Add("CSV");
            exportModeComboBox.Items.Add("Trimmed CSV");

            if (Ogmo.Project.ProjectType == Ogmo.ProjectType.XML)
            {
                exportModeComboBox.Items.Add("XML (IDs)");
                exportModeComboBox.Items.Add("XML (Co-ords)");
            }
            else if (Ogmo.Project.ProjectType == Ogmo.ProjectType.JSON)
            {
                exportModeComboBox.Items.Add("JSON (IDs)");
                exportModeComboBox.Items.Add("JSON (Co-ords)");
            }

            int index = (int)def.ExportMode;

            // Offset for JSON-specific options
            if (Ogmo.Project.ProjectType == Ogmo.ProjectType.JSON)
            {
                if (index == 4) index = 2;
                if (index == 5) index = 3;
            }

            exportModeComboBox.SelectedIndex = index;
        }

        private void exportModeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int index = exportModeComboBox.SelectedIndex;

            // Offset for JSON-specific options
            if (Ogmo.Project.ProjectType == Ogmo.ProjectType.JSON)
            {
                if (index == 2) index = 4;
                if (index == 3) index = 5;
            }

            def.ExportMode = (TileLayerDefinition.TileExportMode)index;
        }
    }
}
