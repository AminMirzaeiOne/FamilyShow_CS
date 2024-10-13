using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyShowLib;
using System.Windows;

namespace FamilyShow.Controls.Diagrams
{

    /// <summary>
    /// The type of node.
    /// </summary>
    public enum NodeType
    {
        Primary,
        Related,
        Spouse,
        Sibling,
        SiblingLeft,
        SiblingRight
    }

    public class DiagramNode
    {
        private static class Const
        {
            public static double OpacityFiltered = 0.15;
            public static double OpacityNormal = 1.0;
            public static double AnimationDuration = 300;
        }

        #region fields

        // Person object associated with the node.
        private Person person;

        // Location of the node, relative to its parent group.
        private Point location = new Point();

        // The type of node.
        private NodeType type = NodeType.Related;

        // The amount to scale the node. 
        private double scale = 1;

        // The current display year, this is used for the time filter.
        private double displayYear = DateTime.Now.Year;

        // Flag, true if this node is currently filtered. This means
        // its still displayed but in a dim state.
        private bool isFiltered;

        #endregion
    }
}
