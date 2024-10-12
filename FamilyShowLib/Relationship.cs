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


    }
}
