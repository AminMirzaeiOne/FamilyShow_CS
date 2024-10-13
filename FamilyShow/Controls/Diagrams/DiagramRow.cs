using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FamilyShow.Controls.Diagrams
{
    public class DiagramRow
    {
        #region fields

        // Space between each group.
        private double groupSpace = 80;

        // Location of the row, relative to the diagram.
        private Point location = new Point();

        // List of groups in the row.
        private List<DiagramGroup> groups = new List<DiagramGroup>();

        #endregion
    }
}
