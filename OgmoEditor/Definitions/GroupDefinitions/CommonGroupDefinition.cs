using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OgmoEditor.Definitions.GroupDefinitions
{
    public class CommonGroupDefinition
    {
        [XmlArrayItem("GroupName")]
        public List<string> groupNames;

        public CommonGroupDefinition()
        {
            groupNames = new List<string>
            {
                "All", // Used for displaying all the items (grids, entities, tiles)
                "default" // Initial group (if the user doesn't choose a group)
            };
        }

        /// <summary>
        /// Returns a list of all group names except the "All" group, as it does not represent any group name
        /// </summary>
        /// <returns>A new list of group names</returns>
        public List<string> GetValidGroupNames()
        {
            return groupNames.Where(ob => ob != "All").ToList();
        }

    }
}
