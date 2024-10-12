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

        /// <summary>
        /// Performs the business logic for adding the Parent relationship between the person and the parent.
        /// </summary>
        public static void AddParent(PeopleCollection family, Person person, Person parent)
        {
            // A person can only have 2 parents, do nothing
            if (person.Parents.Count == 2)
                return;

            // Add the parent to the main collection of people.
            family.Add(parent);

            switch (person.Parents.Count)
            {
                // No exisitng parents
                case 0:
                    family.AddChild(parent, person, ParentChildModifier.Natural);
                    break;

                // An existing parent
                case 1:
                    family.AddChild(parent, person, ParentChildModifier.Natural);
                    family.AddSpouse(parent, person.Parents[0], SpouseModifier.Current);
                    break;
            }

            // Handle siblings
            if (person.Siblings.Count > 0)
            {
                // Make siblings the child of the new parent
                foreach (Person sibling in person.Siblings)
                {
                    family.AddChild(parent, sibling, ParentChildModifier.Natural);
                }
            }

            // Setter for property change notification
            person.HasParents = true;
        }
    }
}
