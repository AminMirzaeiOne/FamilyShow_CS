using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyShowLib
{
    public class GedcomExport
    {
        // Writes the text (GEDCOM) file.
        private TextWriter writer;

        // Maps GUID IDs (which are too long for GEDCOM) to smaller IDs.
        private GedcomIdMap idMap = new GedcomIdMap();

        // The people collection that is being exported.
        private PeopleCollection people;

        // Family group counter.
        private int familyId = 1;

        /// <summary>
        /// Export the data from the People collection to the specified GEDCOM file.
        /// </summary>
        public void Export(PeopleCollection peopleCollection, string gedcomFilePath)
        {
            this.people = peopleCollection;

            using (writer = new StreamWriter(gedcomFilePath))
            {
                WriteLine(0, "HEAD", "");
                ExportPeople();
                ExportFamilies();
                WriteLine(0, "TRLR", "");
            }
        }

        /// <summary>
        /// Export each person to the GEDCOM file.
        /// </summary>
        private void ExportPeople()
        {
            foreach (Person person in people)
            {
                // Start of a new individual record.
                WriteLine(0, string.Format(CultureInfo.InvariantCulture,
                    "@{0}@", idMap.Get(person.Id)), "INDI");

                // Export details.

                // Name.
                ExportName(person);

                // Nickname.
                if (!string.IsNullOrEmpty(person.NickName))
                    WriteLine(2, "NICK", person.NickName);

                // Prefix.
                if (!string.IsNullOrEmpty(person.Suffix))
                    WriteLine(2, "NPFX", person.Suffix);

                // Married name.
                if (!string.IsNullOrEmpty(person.MarriedName))
                    WriteLine(2, "_MARNM", person.MarriedName);

                // Gender.
                ExportGender(person);

                // Birth and death info.
                ExportEvent("BIRT", person.BirthDate, person.BirthPlace);
                ExportEvent("DEAT", person.DeathDate, person.DeathPlace);

                // Photos.
                ExportPhotos(person);
            }
        }
    }
}
