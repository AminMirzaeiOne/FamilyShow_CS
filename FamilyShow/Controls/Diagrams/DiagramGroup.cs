using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FamilyShow.Controls.Diagrams
{
    public class DiagramGroup
    {
        #region fields

        // Space between each node.
        private const double NodeSpace = 10;

        // Location of the group, relative to the row.
        private Point location = new Point();

        // List of nodes in the group.
        private List<DiagramNode> nodes = new List<DiagramNode>();

        #endregion

        #region properties

        /// <summary>
        /// Location of the group, relative to the row.
        /// </summary>
        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        /// <summary>
        /// List of nodes in the group.
        /// </summary>
        public ReadOnlyCollection<DiagramNode> Nodes
        {
            get { return new ReadOnlyCollection<DiagramNode>(nodes); }
        }

        #endregion

    }
}
