using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

        /// <summary>
        /// The Type of relationship.  Parent, child, sibling, or spouse
        /// </summary>
        public RelationshipType RelationshipType
        {
            get { return relationshipType; }
            set { relationshipType = value; }
        }

        /// <summary>
        /// The person id the relationship is to. See comment on personId above.
        /// </summary>
        [XmlIgnore]
        public Person RelationTo
        {
            get { return relationTo; }
            set
            {
                relationTo = value;
                personId = ((Person)value).Id;
                personFullname = ((Person)value).FullName;
            }
        }

        public string PersonId
        {
            get { return personId; }
            set { personId = value; }
        }

        public string PersonFullname
        {
            get { return personFullname; }
            set { personFullname = value; }
        }


    }

    /// <summary>
    /// Describes the kinship between a child and parent.
    /// </summary>
    [Serializable]
    public class ParentRelationship : Relationship
    {
        // The ParentChildModifier are not currently used by the application
        private ParentChildModifier parentChildModifier;
        public ParentChildModifier ParentChildModifier
        {
            get { return parentChildModifier; }
            set { parentChildModifier = value; }
        }

        // Paramaterless constructor required for XML serialization
        public ParentRelationship() { }

        public ParentRelationship(Person personId, ParentChildModifier parentChildType)
        {
            RelationshipType = RelationshipType.Parent;
            this.RelationTo = personId;
            this.parentChildModifier = parentChildType;
        }
    }
}
