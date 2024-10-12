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

        // <summary>
        /// Create the family section (the FAM tags) in the GEDCOM file.
        /// </summary>
        private void ExportFamilies()
        {
            // Exporting families is more difficult since need to export each
            // family group. A family group consists of one or more parents,
            // marriage / divorce information and children. The FamilyMap class
            // creates a list of family groups from the People collection.
            FamilyMap map = new FamilyMap();
            map.Create(people);

            // Created the family groups, now export each family.
            foreach (Family family in map.Values)
                ExportFamily(family);
        }

        /// <summary>
        /// Export one family group to the GEDCOM file.
        /// </summary>
        private void ExportFamily(Family family)
        {
            // Return right away if this is only a single person without any children.
            if (family.ParentRight == null && family.Children.Count == 0)
                return;

            // Start of new family record.
            WriteLine(0, string.Format(CultureInfo.InvariantCulture, "@F{0}@", familyId++), "FAM");

            // Marriage info.
            ExportMarriage(family.ParentLeft, family.ParentRight, family.Relationship);

            // Children.
            foreach (Person child in family.Children)
            {
                WriteLine(1, "CHIL", string.Format(
                    CultureInfo.InvariantCulture, "@{0}@", idMap.Get(child.Id)));
            }
        }

        /// <summary>
        /// Export marriage / divorce information.
        /// </summary>
        private void ExportMarriage(Person partnerLeft, Person partnerRight, SpouseRelationship relationship)
        {
            // PartnerLeft.
            if (partnerLeft != null && partnerLeft.Gender == Gender.Male)
                WriteLine(1, "HUSB", string.Format(CultureInfo.InvariantCulture,
                "@{0}@", idMap.Get(partnerLeft.Id)));

            if (partnerLeft != null && partnerLeft.Gender == Gender.Female)
                WriteLine(1, "WIFE", string.Format(CultureInfo.InvariantCulture,
                "@{0}@", idMap.Get(partnerLeft.Id)));

            // PartnerRight.
            if (partnerRight != null && partnerRight.Gender == Gender.Male)
                WriteLine(1, "HUSB", string.Format(CultureInfo.InvariantCulture,
                "@{0}@", idMap.Get(partnerRight.Id)));

            if (partnerRight != null && partnerRight.Gender == Gender.Female)
                WriteLine(1, "WIFE", string.Format(CultureInfo.InvariantCulture,
                "@{0}@", idMap.Get(partnerRight.Id)));

            if (relationship == null)
                return;

            // Marriage.
            if (relationship.SpouseModifier == SpouseModifier.Current)
            {
                WriteLine(1, "MARR", "Y");

                // Date if it exist.
                if (relationship.MarriageDate != null)
                    WriteLine(2, "DATE", relationship.MarriageDate.Value.ToShortDateString());
            }

            // Divorce.
            if (relationship.SpouseModifier == SpouseModifier.Former)
            {
                WriteLine(1, "DIV", "Y");

                // Date if it exist.
                if (relationship.DivorceDate != null)
                    WriteLine(2, "DATE", relationship.DivorceDate.Value.ToShortDateString());
            }
        }


    }
}
