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

        /// <summary>
        /// Update the marriage / divorce information for the two people.
        /// </summary>
        private static void ImportMarriage(Person husband, Person wife, XmlNode node)
        {
            // Return right away if there are not two people.
            if (husband == null || wife == null)
                return;

            // See if a marriage (or divorce) is specified.
            if (node.SelectSingleNode("MARR") != null || node.SelectSingleNode("DIV") != null)
            {
                // Get dates.
                DateTime? marriageDate = GetValueDate(node, "MARR/DATE");
                DateTime? divorceDate = GetValueDate(node, "DIV/DATE");
                SpouseModifier modifier = GetDivorced(node) ? SpouseModifier.Former : SpouseModifier.Current;

                // Add info to husband.
                if (husband.GetSpouseRelationship(wife) == null)
                {
                    SpouseRelationship husbandMarriage = new SpouseRelationship(wife, modifier);
                    husbandMarriage.MarriageDate = marriageDate;
                    husbandMarriage.DivorceDate = divorceDate;
                    husband.Relationships.Add(husbandMarriage);
                }

                // Add info to wife.
                if (wife.GetSpouseRelationship(husband) == null)
                {
                    SpouseRelationship wifeMarriage = new SpouseRelationship(husband, modifier);
                    wifeMarriage.MarriageDate = marriageDate;
                    wifeMarriage.DivorceDate = divorceDate;
                    wife.Relationships.Add(wifeMarriage);
                }
            }
        }

        /// <summary>
        /// Import photo information from the GEDCOM XML file.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void ImportPhotos(Person person, XmlNode node)
        {
            try
            {
                // Get list of photos for this person.
                string[] photos = GetPhotos(node);
                if (photos == null || photos.Length == 0)
                    return;

                // Import each photo. Make the first photo specified
                // the default photo (avatar).
                for (int i = 0; i < photos.Length; i++)
                {
                    Photo photo = new Photo(photos[i]);
                    photo.IsAvatar = (i == 0) ? true : false;
                    person.Photos.Add(photo);
                }
            }
            catch
            {
                // There was an error importing a photo, ignore 
                // and continue processing the GEDCOM XML file.
            }
        }

        /// <summary>
        /// Import the note info from the GEDCOM XMl file.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void ImportNote(Person person, XmlNode node)
        {
            try
            {
                string value = GetValue(node, "NOTE");
                if (!string.IsNullOrEmpty(value))
                {
                    person.Story = new Story();
                    string storyFileName = new StringBuilder(person.Name).Append(".rtf").ToString();
                    person.Story.Save(value, storyFileName);
                }
            }
            catch
            {
                // There was an error importing the note, ignore
                // and continue processing the GEDCOM XML file.
            }
        }

        /// <summary>
        /// Import the birth info from the GEDCOM XML file.
        /// </summary>
        private static void ImportBirth(Person person, XmlNode node)
        {
            person.BirthDate = GetValueDate(node, "BIRT/DATE");
            person.BirthPlace = GetValue(node, "BIRT/PLAC");
        }

        /// <summary>
        /// Import the death info from the GEDCOM XML file.
        /// </summary>
        private static void ImportDeath(Person person, XmlNode node)
        {
            person.IsLiving = (node.SelectSingleNode("DEAT") == null) ? true : false;
            person.DeathDate = GetValueDate(node, "DEAT/DATE");
            person.DeathPlace = GetValue(node, "DEAT/PLAC");
        }

        /// <summary>
        /// Return a list of photos specified in the GEDCOM XML file.
        /// </summary>
        private static string[] GetPhotos(XmlNode node)
        {
            string[] photos;
            XmlNodeList list = node.SelectNodes("OBJE");
            photos = new string[list.Count];

            for (int i = 0; i < list.Count; i++)
                photos[i] = GetFile(list[i]);

            return photos;
        }

        private static string GetSuffix(XmlNode node)
        {
            return GetValue(node, "NAME/NPFX");
        }

        private static string GetMarriedName(XmlNode node)
        {
            return GetValue(node, "NAME/_MARNM");
        }


    }
}
