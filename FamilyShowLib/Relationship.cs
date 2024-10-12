using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyShowLib
{
    public abstract class Relationship
    {
        /// <summary>
        /// Describes the kinship between person objects
        /// </summary>
        [Serializable]

        private RelationshipType relationshipType;
        private Person relationTo;

        // The person's Id will be serialized instead of the relationTo person object to avoid
        // circular references during Xml Serialization. When family data is loaded, the corresponding
        // person object will be assigned to the relationTo property (please see app.xaml.cs).
        private string personId;

        // Store the person's name with the Id to make the xml file more readable
        private string personFullname;


    }
}
