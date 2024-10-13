using System;
using System.Collections.Generic;
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

    }
}
