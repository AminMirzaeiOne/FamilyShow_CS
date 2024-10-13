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

        /// <summary>
        /// Get or set the pen that specifies the connector line.
        /// </summary>
        protected Pen ResourcePen
        {
            get { return resourcePen; }
            set { resourcePen = value; }
        }

        /// <summary>
        /// Create the connector line pen. The opacity is set based on
        /// the current filtered state. The pen contains an animation
        /// if the filtered state has changed.
        /// </summary>
        protected Pen Pen
        {
            get
            {
                // Make a copy of the resource pen so it can 
                // be modified, the resource pen is frozen.
                Pen connectorPen = this.ResourcePen.Clone();

                // Set opacity based on the filtered state.
                connectorPen.Brush.Opacity = (this.isFiltered) ? Const.OpacityFiltered : Const.OpacityNormal;

                // Create animation if the filtered state has changed.
                if (animation != null)
                    connectorPen.Brush.BeginAnimation(Brush.OpacityProperty, animation);

                return connectorPen;
            }
        }

        /// <summary>
        /// Return true if the connection is currently filtered.
        /// </summary>
        private bool IsFiltered
        {
            set { isFiltered = value; }
            get { return isFiltered; }
        }

        /// <summary>
        /// Get the new filtered state of the connection. This depends
        /// on the connection nodes, marriage date and previous marriage date.
        /// </summary>
        virtual protected bool NewFilteredState
        {
            get
            {
                // Connection is filtered if any of the nodes are filtered.
                if (start.Node.IsFiltered || end.Node.IsFiltered)
                    return true;

                // Connection is not filtered.
                return false;
            }
        }

        /// <summary>
        /// Consturctor that specifies the two nodes that are connected.
        /// </summary>
        protected DiagramConnector(DiagramConnectorNode startConnector,
            DiagramConnectorNode endConnector)
        {
            this.start = startConnector;
            this.end = endConnector;
        }

        /// <summary>
        /// Return true if should continue drawing, otherwise false.
        /// </summary>
        virtual public bool Draw(DrawingContext drawingContext)
        {
            // Don't draw if either of the nodes are filtered.
            if (start.Node.Visibility != Visibility.Visible ||
                end.Node.Visibility != Visibility.Visible)
                return false;

            // First check if the filtered state has changed, an animation
            // if created if the state has changed which is used for all 
            // connection drawing.
            CheckIfFilteredChanged();

            return true;
        }

        /// <summary>
        /// Create the specified brush. The opacity is set based on the 
        /// current filtered state. The brush contains an animation if 
        /// the filtered state has changed.
        /// </summary>
        protected SolidColorBrush GetBrush(Color color)
        {
            // Create the brush.
            SolidColorBrush brush = new SolidColorBrush(color);

            // Set the opacity based on the filtered state.
            brush.Opacity = (this.isFiltered) ? Const.OpacityFiltered : Const.OpacityNormal;

            // Create animation if the filtered state has changed.
            if (animation != null)
                brush.BeginAnimation(Brush.OpacityProperty, animation);

            return brush;
        }

        /// <summary>
        /// Determine if the filtered state has changed, and create
        /// the animation that is used to draw the connection.
        /// </summary>
        protected void CheckIfFilteredChanged()
        {
            // See if the filtered state has changed.
            bool newFiltered = this.NewFilteredState;
            if (newFiltered != this.IsFiltered)
            {
                // Filtered state did change, create the animation.
                this.IsFiltered = newFiltered;
                animation = new DoubleAnimation();
                animation.From = isFiltered ? Const.OpacityNormal : Const.OpacityFiltered;
                animation.To = isFiltered ? Const.OpacityFiltered : Const.OpacityNormal;
                animation.Duration = App.GetAnimationDuration(Const.AnimationDuration);
            }
            else
            {
                // Filtered state did not change, clear the animation.
                animation = null;
            }
        }

        /// <summary>
        /// Connector between a parent and child.
        /// </summary>
        public class ChildDiagramConnector : DiagramConnector
        {
            public ChildDiagramConnector(DiagramConnectorNode startConnector,
                DiagramConnectorNode endConnector) : base(startConnector, endConnector)
            {
                // Get the pen that is used to draw the connection line.
                this.ResourcePen = (Pen)Application.Current.TryFindResource("ChildConnectionPen");
            }

            /// <summary>
            /// Draw the connection between the two nodes.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
            override public bool Draw(DrawingContext drawingContext)
            {
                if (!base.Draw(drawingContext))
                    return false;

                drawingContext.DrawLine(this.Pen, this.StartNode.Center, this.EndNode.Center);
                return true;
            }
        }



    }
}
