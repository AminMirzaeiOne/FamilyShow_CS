using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyShowLib;
using System.Windows;
using System.Collections.ObjectModel;

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

        #region properties

        /// <summary>
        /// Sets the callback that is called when a node is clicked.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public RoutedEventHandler NodeClickHandler
        {
            set { nodeClickHandler = value; }
        }

        /// <summary>
        /// Gets the list of people in the family.
        /// </summary>
        public PeopleCollection Family
        {
            get { return family; }
        }

        /// <summary>
        /// Gets the list of connections between nodes.
        /// </summary>
        public List<DiagramConnector> Connections
        {
            get { return connections; }
        }

        /// <summary>
        /// Gets the person lookup list. This includes all of the 
        /// people and nodes that are displayed in the diagram.
        /// </summary>
        public Dictionary<Person, DiagramConnectorNode> PersonLookup
        {
            get { return personLookup; }
        }

        /// <summary>
        /// Sets the year filter that filters nodes and connectors.
        /// </summary>
        public double DisplayYear
        {
            set
            {
                if (this.displayYear != value)
                {
                    this.displayYear = value;
                    foreach (DiagramConnectorNode connectorNode in personLookup.Values)
                        connectorNode.Node.DisplayYear = this.displayYear;
                }
            }
        }

        /// <summary>
        /// Gets the minimum year in all nodes and connectors.
        /// </summary>
        public double MinimumYear
        {
            get
            {
                // Init to current year.
                double minimumYear = DateTime.Now.Year;

                // Check birth years.
                foreach (DiagramConnectorNode connectorNode in personLookup.Values)
                {
                    DateTime? date = connectorNode.Node.Person.BirthDate;
                    if (date != null)
                        minimumYear = Math.Min(minimumYear, date.Value.Year);
                }

                // Check marriage years.
                foreach (DiagramConnector connector in connections)
                {
                    // Marriage date.
                    DateTime? date = connector.MarriedDate;
                    if (date != null)
                        minimumYear = Math.Min(minimumYear, date.Value.Year);

                    // Previous marriage date.
                    date = connector.PreviousMarriedDate;
                    if (date != null)
                        minimumYear = Math.Min(minimumYear, date.Value.Year);
                }

                return minimumYear;
            }
        }

        #endregion

        public DiagramLogic()
        {
            // The list of people, this is a global list shared by the application.
            family = App.Family;

            Clear();
        }

        /// <summary>
        /// Return a list of parents for the people in the specified row.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Collection<Person> GetParents(DiagramRow row)
        {
            // List that is returned.
            Collection<Person> list = new Collection<Person>();

            // Get possible children in the row.
            List<Person> rowList = GetPrimaryAndRelatedPeople(row);

            // Add each parent to the list, make sure the parent is only added once.
            foreach (Person person in rowList)
            {
                foreach (Person parent in person.Parents)
                {
                    if (!list.Contains(parent))
                        list.Add(parent);
                }
            }

            return list;
        }

    }
}
