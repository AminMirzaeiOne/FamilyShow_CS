using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyShowLib;
using System.Windows;

namespace FamilyShow.Controls.Diagrams
{
    public class DiagramLogic
    {
        #region fields

        // List of the connections, specify connections between two nodes.
        private List<DiagramConnector> connections = new List<DiagramConnector>();

        // Map that allows quick lookup of a Person object to connection information.
        // Used when setting up the connections between nodes.
        private Dictionary<Person, DiagramConnectorNode> personLookup =
            new Dictionary<Person, DiagramConnectorNode>();

        // List of people, global list that is shared by all objects in the application.
        private PeopleCollection family;

        // Callback when a node is clicked.
        private RoutedEventHandler nodeClickHandler;

        // Filter year for nodes and connectors.
        private double displayYear;

        #endregion
    }
}
