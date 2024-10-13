﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FamilyShowLib;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls;

namespace FamilyShow.Controls.Diagrams
{
    /// <summary>
    /// Diagram that lays out and displays the nodes.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public class Diagram : FrameworkElement
    {
        #region fields

        private static class Const
        {
            // Duration to pause before displaying new nodes.
            public static double AnimationPauseDuration = 600;

            // Duration for nodes to fade in when the diagram is repopulated.
            public static double NodeFadeInDuration = 500;

            // Duration for the new person animation.
            public static double NewPersonAnimationDuration = 250;

            // Stop adding new rows when the number of nodes exceeds the max node limit.
            public static int MaximumNodes = 50;

            // Group space.
            public static double PrimaryRowGroupSpace = 20;
            public static double ChildRowGroupSpace = 20;
            public static double ParentRowGroupSpace = 40;

            // Amount of space between each row.
            public static double RowSpace = 40;

            // Scale multiplier for spouse and siblings.
            public static double RelatedMultiplier = 0.8;

            // Scale multiplier for each future generation row.
            public static double GenerationMultiplier = 0.9;
        }

        // List of rows in the diagram. Each row contains groups, and each group contains nodes.
        private List<DiagramRow> rows = new List<DiagramRow>();

        // Populates the rows with nodes.
        private DiagramLogic logic;

        // Size of the diagram. Used to layout all of the nodes before the
        // control gets an actual size.
        private Size totalSize = new Size(0, 0);

        // Zoom level of the diagram.
        private double scale = 1.0;

        // Bounding area of the selected node, the selected node is the 
        // non-primary node that is selected, and will become the primary node.
        private Rect selectedNodeBounds = Rect.Empty;

        // Flag if currently populating or not. Necessary since diagram populate 
        // contains several parts and animations, request to update the diagram
        // are ignored when this flag is set.
        private bool populating;

        // The person that has been added to the diagram.
        private Person newPerson;

        // Timer used with the repopulating animation.
        private DispatcherTimer animationTimer = new DispatcherTimer();

#if DEBUG
        // Flag if the row and group borders should be drawn.
        bool displayBorder;
#endif

        #endregion

        #region events

        public event EventHandler DiagramUpdated;
        private void OnDiagramUpdated()
        {
            if (DiagramUpdated != null)
                DiagramUpdated(this, EventArgs.Empty);
        }

        public event EventHandler DiagramPopulated;
        private void OnDiagramPopulated()
        {
            if (DiagramPopulated != null)
                DiagramPopulated(this, EventArgs.Empty);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the zoom level of the diagram.
        /// </summary>
        public double Scale
        {
            get { return this.scale; }
            set
            {
                if (this.scale != value)
                {
                    scale = value;
                    this.LayoutTransform = new ScaleTransform(scale, scale);
                }
            }
        }

        /// <summary>
        /// Sets the display year filter.
        /// </summary>
        public double DisplayYear
        {
            set
            {
                // Filter nodes and connections based on the year.
                logic.DisplayYear = value;
                this.InvalidateVisual();
            }
        }

        /// <summary>
        /// Gets the minimum year specified in the nodes and connections.
        /// </summary>
        public double MinimumYear
        {
            get { return logic.MinimumYear; }
        }

        /// <summary>
        /// Gets the bounding area (relative to the diagram) of the primary node.
        /// </summary>
        public Rect PrimaryNodeBounds
        {
            get { return logic.GetNodeBounds(logic.Family.Current); }
        }

        /// <summary>
        /// Gets the bounding area (relative to the diagram) of the selected node.
        /// The selected node is the non-primary node that was previously selected
        /// to be the primary node.
        /// </summary>
        public Rect SelectedNodeBounds
        {
            get { return selectedNodeBounds; }
        }

        /// <summary>
        /// Gets the number of nodes in the diagram.
        /// </summary>
        public int NodeCount
        {
            get { return logic.PersonLookup.Count; }
        }

        #endregion

        public Diagram()
        {
            // Init the diagram logic, which handles all of the layout logic.
            logic = new DiagramLogic();
            logic.NodeClickHandler = new RoutedEventHandler(OnNodeClick);

            // Can have an empty People collection when in design tools such as Blend.
            if (logic.Family != null)
            {
                logic.Family.ContentChanged += new EventHandler<ContentChangedEventArgs>(OnFamilyContentChanged);
                logic.Family.CurrentChanged += new EventHandler(OnFamilyCurrentChanged);
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
#if DEBUG
            // Context menu so can display row and group borders.
            this.ContextMenu = new ContextMenu();
            MenuItem item = new MenuItem();
            this.ContextMenu.Items.Add(item);
            item.Header = "Show Diagram Outline";
            item.Click += new RoutedEventHandler(OnToggleBorderClick);
            item.Foreground = SystemColors.MenuTextBrush;
            item.Background = SystemColors.MenuBrush;
#endif

            UpdateDiagram();
            base.OnInitialized(e);
        }

        protected override int VisualChildrenCount
        {
            // Return the number of rows.
            get { return rows.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            // Return the requested row.
            return rows[index];
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Let each row determine how large they want to be.
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (DiagramRow row in rows)
                row.Measure(size);

            // Return the total size of the diagram.
            return ArrangeRows(false);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // Arrange the rows in the diagram, return the total size.
            return ArrangeRows(true);
        }

        /// <summary>
        /// Arrange the rows in the diagram, return the total size.
        /// </summary>
        private Size ArrangeRows(bool arrange)
        {
            // Location of the row.
            double pos = 0;

            // Bounding area of the row.
            Rect bounds = new Rect();

            // Total size of the diagram.
            Size size = new Size(0, 0);

            foreach (DiagramRow row in rows)
            {
                // Row location, center the row horizontaly.
                bounds.Y = pos;
                bounds.X = (totalSize.Width == 0) ? 0 :
                    bounds.X = (totalSize.Width - row.DesiredSize.Width) / 2;

                // Row Size.
                bounds.Width = row.DesiredSize.Width;
                bounds.Height = row.DesiredSize.Height;

                // Arrange the row, save the location.
                if (arrange)
                {
                    row.Arrange(bounds);
                    row.Location = bounds.TopLeft;
                }

                // Update the size of the diagram.
                size.Width = Math.Max(size.Width, bounds.Width);
                size.Height = pos + row.DesiredSize.Height;

                pos += bounds.Height;
            }

            // Store the size, this is necessary so the diagram
            // can be laid out without a valid Width property.
            totalSize = size;
            return size;
        }

        /// <summary>
        /// Draw the connector lines at a lower level (OnRender) instead
        /// of creating visual tree objects.
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
#if DEBUG
            if (displayBorder)
            {
                // Draws borders around the rows and groups.
                foreach (DiagramRow row in rows)
                {
                    // Display row border.
                    Rect bounds = new Rect(row.Location, row.DesiredSize);
                    drawingContext.DrawRectangle(null, new Pen(Brushes.DarkKhaki, 1), bounds);

                    foreach (DiagramGroup group in row.Groups)
                    {
                        // Display group border.
                        bounds = new Rect(group.Location, group.DesiredSize);
                        bounds.Offset(row.Location.X, row.Location.Y);
                        bounds.Inflate(-1, -1);
                        drawingContext.DrawRectangle(null, new Pen(Brushes.Gray, 1), bounds);
                    }
                }
            }
#endif

            // Draw child connectors first, so marriage information appears on top.
            foreach (DiagramConnector connector in logic.Connections)
            {
                if (connector.IsChildConnector)
                    connector.Draw(drawingContext);
            }

            // Draw all other non-child connectors.
            foreach (DiagramConnector connector in logic.Connections)
            {
                if (!connector.IsChildConnector)
                    connector.Draw(drawingContext);
            }
        }

#if DEBUG
        void OnToggleBorderClick(object sender, RoutedEventArgs e)
        {
            // Display or hide the row and group borders.
            displayBorder = !displayBorder;

            // Update check on menu.
            MenuItem menuItem = this.ContextMenu.Items[0] as MenuItem;
            menuItem.IsChecked = displayBorder;

            this.InvalidateVisual();
        }
#endif

        /// <summary>
        /// Reset all of the data associated with the diagram.
        /// </summary>
        private void Clear()
        {
            foreach (DiagramRow row in rows)
            {
                row.Clear();
                this.RemoveVisualChild(row);
            }

            rows.Clear();
            logic.Clear();
        }


    }
}
