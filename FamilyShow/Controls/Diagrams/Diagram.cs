using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FamilyShowLib;
using System.Windows.Threading;

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


    }
}
