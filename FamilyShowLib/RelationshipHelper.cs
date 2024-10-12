﻿using System;
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

        /// <summary>
        /// Performs the business logic for adding the Parent relationship between the person and the parents.
        /// </summary>
        public static void AddParent(PeopleCollection family, Person person, ParentSet parentSet)
        {
            // First add child to parents.
            family.AddChild(parentSet.FirstParent, person, ParentChildModifier.Natural);
            family.AddChild(parentSet.SecondParent, person, ParentChildModifier.Natural);

            // Next update the siblings. Get the list of full siblings for the person. 
            // A full sibling is a sibling that has both parents in common. 
            List<Person> siblings = GetChildren(parentSet);
            foreach (Person sibling in siblings)
            {
                if (sibling != person)
                    family.AddSibling(person, sibling);
            }
        }

        /// <summary>
        /// Return a list of children for the parent set.
        /// </summary>
        private static List<Person> GetChildren(ParentSet parentSet)
        {
            // Get list of both parents.
            List<Person> firstParentChildren = new List<Person>(parentSet.FirstParent.Children);
            List<Person> secondParentChildren = new List<Person>(parentSet.SecondParent.Children);

            // Combined children list that is returned.
            List<Person> children = new List<Person>();

            // Go through and add the children that have both parents.            
            foreach (Person child in firstParentChildren)
            {
                if (secondParentChildren.Contains(child))
                    children.Add(child);
            }

            return children;
        }
    }
}
