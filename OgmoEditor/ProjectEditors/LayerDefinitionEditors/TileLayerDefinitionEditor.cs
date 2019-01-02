using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OgmoEditor.Definitions.LayerDefinitions;

namespace OgmoEditor.ProjectEditors.LayerDefinitionEditors
{
    public partial class TileLayerDefinitionEditor : UserControl
    {
        private TileLayerDefinition def;
        private List<TileLayerDefinition.TileExportMode> xmlExportModes;
        private List<TileLayerDefinition.TileExportMode> jsonExportModes;
        private Dictionary<TileLayerDefinition.TileExportMode, string> exportModeDescriptions;

        public TileLayerDefinitionEditor(TileLayerDefinition def)
        {
            this.def = def;
            InitializeComponent();
            Location = new Point(206, 128);

            // Compile a list of all the export modes for each project type
            xmlExportModes = new List<TileLayerDefinition.TileExportMode>();
            xmlExportModes.Add(TileLayerDefinition.TileExportMode.CSV);
            xmlExportModes.Add(TileLayerDefinition.TileExportMode.TrimmedCSV);
            xmlExportModes.Add(TileLayerDefinition.TileExportMode.XML);
            xmlExportModes.Add(TileLayerDefinition.TileExportMode.XMLCoords);

            jsonExportModes = new List<TileLayerDefinition.TileExportMode>();
            jsonExportModes.Add(TileLayerDefinition.TileExportMode.Array2D);
            jsonExportModes.Add(TileLayerDefinition.TileExportMode.Array1D);
            jsonExportModes.Add(TileLayerDefinition.TileExportMode.CSV);
            jsonExportModes.Add(TileLayerDefinition.TileExportMode.TrimmedCSV);
            jsonExportModes.Add(TileLayerDefinition.TileExportMode.JSON);
            jsonExportModes.Add(TileLayerDefinition.TileExportMode.JSONCoords);

            // Initialize a dictionary containing the descriptions for each export mode
            exportModeDescriptions = new Dictionary<TileLayerDefinition.TileExportMode, string>();
            exportModeDescriptions.Add(TileLayerDefinition.TileExportMode.CSV, "CSV");
            exportModeDescriptions.Add(TileLayerDefinition.TileExportMode.TrimmedCSV, "Trimmed CSV");
            exportModeDescriptions.Add(TileLayerDefinition.TileExportMode.XML, "XML (IDs)");
            exportModeDescriptions.Add(TileLayerDefinition.TileExportMode.XMLCoords, "XML (Co-ords)");
            exportModeDescriptions.Add(TileLayerDefinition.TileExportMode.JSON, "JSON (IDs)");
            exportModeDescriptions.Add(TileLayerDefinition.TileExportMode.JSONCoords, "JSON (Co-ords)");
            exportModeDescriptions.Add(TileLayerDefinition.TileExportMode.Array2D, "2D Array");
            exportModeDescriptions.Add(TileLayerDefinition.TileExportMode.Array1D, "1D Array");

            // Add the Export Mode dropdown options
            if (Ogmo.Project.ProjectType == Ogmo.ProjectType.XML)
            {
                foreach (var mode in xmlExportModes)
                {
                    exportModeComboBox.Items.Add(exportModeDescriptions[mode]);
                }
            }
            else if (Ogmo.Project.ProjectType == Ogmo.ProjectType.JSON)
            {
                foreach (var mode in jsonExportModes)
                {
                    exportModeComboBox.Items.Add(exportModeDescriptions[mode]);
                }
            }

            // Automatically select the current export mode from the dropdown
            exportModeComboBox.SelectedIndex = GetIndexForExportMode(def.ExportMode);
        }

        private void exportModeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            def.ExportMode = GetExportModeForIndex(exportModeComboBox.SelectedIndex);
        }

        private int GetIndexForExportMode(TileLayerDefinition.TileExportMode mode)
        {
            int index = 0;

            if (Ogmo.Project.ProjectType == Ogmo.ProjectType.XML)
            {
                index = xmlExportModes.IndexOf(mode);
            }
            else if (Ogmo.Project.ProjectType == Ogmo.ProjectType.JSON)
            {
                index = jsonExportModes.IndexOf(mode);
            }

            return index;
        }

        private TileLayerDefinition.TileExportMode GetExportModeForIndex(int index)
        {
            TileLayerDefinition.TileExportMode mode = TileLayerDefinition.TileExportMode.CSV;

            if (Ogmo.Project.ProjectType == Ogmo.ProjectType.XML)
            {
                mode = xmlExportModes[index];
            }
            else if (Ogmo.Project.ProjectType == Ogmo.ProjectType.JSON)
            {
                mode = jsonExportModes[index];
            }

            return mode;
        }
    }
}
