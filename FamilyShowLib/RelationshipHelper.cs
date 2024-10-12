using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyShowLib
{
    /// <summary>
    /// Contains the relationship logic rules for adding people and how they relate to each other.
    /// </summary>
    public class RelationshipHelper
    {
        /// <summary>
        /// Performs the business logic for adding the Child relationship between the person and the child.
        /// </summary>
        public static void AddChild(PeopleCollection family, Person person, Person child)
        {
            // Add the new child as a sibling to any existing children
            foreach (Person existingSibling in person.Children)
            {
                family.AddSibling(existingSibling, child);
            }

            switch (person.Spouses.Count)
            {
                // Single parent, add the child to the person
                case 0:
                    family.AddChild(person, child, ParentChildModifier.Natural);
                    break;

                // Has existing spouse, add the child to the person's spouse as well.
                case 1:
                    family.AddChild(person, child, ParentChildModifier.Natural);
                    family.AddChild(person.Spouses[0], child, ParentChildModifier.Natural);
                    break;
            }
        }
    }
}
