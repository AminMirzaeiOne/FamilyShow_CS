using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace FamilyShow.Controls.Diagrams
{
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


    }
}
