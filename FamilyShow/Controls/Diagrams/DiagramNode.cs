using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
