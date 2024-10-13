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

        protected override Size MeasureOverride(Size availableSize)
        {
            // Let each node determine how large they want to be.
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (DiagramNode node in nodes)
                node.Measure(size);

            // Return the total size of the group.
            return this.ArrangeNodes(false);
        }

    }
}
