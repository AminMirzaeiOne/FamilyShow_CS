using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        #region properties

        /// <summary>
        /// Space between each group.
        /// </summary>
        public double GroupSpace
        {
            get { return groupSpace; }
            set { groupSpace = value; }
        }

        /// <summary>
        /// Location of the row, relative to the diagram.
        /// </summary>
        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        /// <summary>
        /// List of groups in the row.
        /// </summary>
        public ReadOnlyCollection<DiagramGroup> Groups
        {
            get { return new ReadOnlyCollection<DiagramGroup>(groups); }
        }

        public int NodeCount
        {
            get
            {
                int count = 0;
                foreach (DiagramGroup group in groups)
                    count += group.Nodes.Count;
                return count;
            }
        }

        #endregion
    }
}
