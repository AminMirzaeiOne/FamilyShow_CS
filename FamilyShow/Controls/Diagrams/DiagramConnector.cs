using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;

namespace FamilyShow.Controls.Diagrams
{
    /// <summary>
    /// One of the nodes in a connection.
    /// </summary>
    public class DiagramConnectorNode
    {
        #region fields

        // Node location in the diagram.
        private DiagramRow row;
        private DiagramGroup group;
        private DiagramNode node;

        #endregion

        #region properties

        /// <summary>
        /// Node for this connection point.
        /// </summary>
        public DiagramNode Node
        {
            get { return node; }
        }

        /// <summary>
        /// Center of the node relative to the diagram.
        /// </summary>
        public Point Center
        {
            get { return GetPoint(this.node.Center); }
        }

        /// <summary>
        /// LeftCenter of the node relative to the diagram.
        /// </summary>
        public Point LeftCenter
        {
            get { return GetPoint(this.node.LeftCenter); }
        }

        /// <summary>
        /// RightCenter of the node relative to the diagram.
        /// </summary>
        public Point RightCenter
        {
            get { return GetPoint(this.node.RightCenter); }
        }

        /// <summary>
        /// TopCenter of the node relative to the diagram.
        /// </summary>
        public Point TopCenter
        {
            get { return GetPoint(this.node.TopCenter); }
        }

        /// <summary>
        /// TopRight of the node relative to the diagram.
        /// </summary>
        public Point TopRight
        {
            get { return GetPoint(this.node.TopRight); }
        }

        /// <summary>
        /// TopLeft of the node relative to the diagram.
        /// </summary>
        public Point TopLeft
        {
            get { return GetPoint(this.node.TopLeft); }
        }

        #endregion

        public DiagramConnectorNode(DiagramNode node, DiagramGroup group, DiagramRow row)
        {
            this.node = node;
            this.group = group;
            this.row = row;
        }

        /// <summary>
        /// Return the point shifted by the row and group location.
        /// </summary>
        private Point GetPoint(Point point)
        {
            point.Offset(
                this.row.Location.X + this.group.Location.X,
                this.row.Location.Y + this.group.Location.Y);

            return point;
        }
    }

    public class DiagramConnector
    {
        private static class Const
        {
            // Filtered settings.
            public static double OpacityFiltered = 0.15;
            public static double OpacityNormal = 1.0;
            public static double AnimationDuration = 300;
        }

        #region fields

        // The two nodes that are connected.
        private DiagramConnectorNode start;
        private DiagramConnectorNode end;

        // Flag, if the connection is currently filtered. The
        // connection is drawn in a dim-state when filtered.
        private bool isFiltered;

        // Animation if the filtered state has changed.
        private DoubleAnimation animation;

        // Pen to draw connector line.
        private Pen resourcePen;

        #endregion

        /// <summary>
        /// Return true if this is a child connector.
        /// </summary>
        virtual public bool IsChildConnector
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the married date for the connector. Can be null.
        /// </summary>
        virtual public DateTime? MarriedDate
        {
            get { return null; }
        }

        /// <summary>
        /// Get the previous married date for the connector. Can be null.
        /// </summary>
        virtual public DateTime? PreviousMarriedDate
        {
            get { return null; }
        }

        /// <summary>
        /// Get the starting node.
        /// </summary>
        protected DiagramConnectorNode StartNode
        {
            get { return start; }
        }

        /// <summary>
        /// Get the ending node.
        /// </summary>
        protected DiagramConnectorNode EndNode
        {
            get { return end; }
        }



    }
}
