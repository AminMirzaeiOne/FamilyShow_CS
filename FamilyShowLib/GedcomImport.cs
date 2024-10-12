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


    }
}
