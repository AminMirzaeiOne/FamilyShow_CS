using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FamilyShowLib
{
    public class GedcomImport
    {
        // The collection to add entries.
        private PeopleCollection people;

        // Convert the GEDCOM file to an XML file which is easier 
        // to parse, this contains the GEDCOM info in an XML format.
        private XmlDocument doc;

        public void Import(PeopleCollection peopleCollection, string gedcomFilePath)
        {
            // Clear current content.
            peopleCollection.Clear();

            // First convert the GEDCOM file to an XML file so it's easier to parse,
            // the temp XML file is deleted when importing is complete.
            string xmlFilePath = Path.GetTempFileName();

            try
            {
                this.people = peopleCollection;

                // Convert the GEDCOM file to a temp XML file.
                GedcomConverter.ConvertToXml(gedcomFilePath, xmlFilePath, true);
                doc = new XmlDocument();
                doc.Load(xmlFilePath);

                // Import data from the temp XML file to the people collection.
                ImportPeople();
                ImportFamilies();

                // The collection requires a primary-person, use the first
                // person added to the collection as the primary-person.
                if (peopleCollection.Count > 0)
                    peopleCollection.Current = peopleCollection[0];
            }
            finally
            {
                // Delete the temp XML file.
                File.Delete(xmlFilePath);
            }
        }

        /// <summary>
        /// Imports the individuals (INDI tags) from the GEDCOM XML file.
        /// </summary>
        private void ImportPeople()
        {
            // Get list of people.
            XmlNodeList list = doc.SelectNodes("/root/INDI");

            foreach (XmlNode node in list)
            {
                // Create a new person that will be added to the collection.
                Person person = new Person();

                // Import details about the person.
                person.FirstName = GetFirstName(node);
                person.LastName = GetLastName(node);
                person.NickName = GetNickName(node);
                person.Suffix = GetSuffix(node);
                person.MarriedName = GetMarriedName(node);

                person.Id = GetId(node);
                person.Gender = GetGender(node);

                ImportBirth(person, node);
                ImportDeath(person, node);

                ImportPhotos(person, node);
                ImportNote(person, node);

                people.Add(person);
            }
        }

        /// <summary>
        /// Imports the families (FAM tags) from the GEDCOM XML file.
        /// </summary>
        private void ImportFamilies()
        {
            // Get list of families.
            XmlNodeList list = doc.SelectNodes("/root/FAM");
            foreach (XmlNode node in list)
            {
                // Get family (husband, wife and children) IDs from the GEDCOM file.
                string husband = GetHusbandID(node);
                string wife = GetWifeID(node);
                string[] children = GetChildrenIDs(node);

                // Get the Person objects for the husband and wife,
                // required for marriage info and adding children.
                Person husbandPerson = people.Find(husband);
                Person wifePerson = people.Find(wife);

                // Add any marriage / divoirce details.
                ImportMarriage(husbandPerson, wifePerson, node);

                // Import the children.
                foreach (string child in children)
                {
                    // Get the Person object for the child.
                    Person childPerson = people.Find(child);

                    // Calling RelationshipHelper.AddChild hooks up all of the
                    // child relationships for the husband and wife. Also hooks up
                    // the sibling relationships.
                    if (husbandPerson != null && childPerson != null)
                        RelationshipHelper.AddChild(people, husbandPerson, childPerson);

                    if (husbandPerson == null && wifePerson != null & childPerson != null)
                        RelationshipHelper.AddChild(people, wifePerson, childPerson);
                }
            }
        }


    }
}
