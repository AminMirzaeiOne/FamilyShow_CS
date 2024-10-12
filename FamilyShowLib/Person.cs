using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyShowLib
{
    /// <summary>
    /// Representation for a single serializable Person.
    /// INotifyPropertyChanged allows properties of the Person class to
    /// participate as source in data bindings.
    /// </summary>
    [Serializable]
    public class Person : INotifyPropertyChanged, IEquatable<Person>, IDataErrorInfo
    {
        #region Fields and Constants

        // The constants specific to this class
        private static class Const
        {
            public const string DefaultFirstName = "Unknown";
        }

        private string id;
        private string firstName;
        private string lastName;
        private string middleName;
        private string suffix;
        private string nickName;
        private string marriedName;
        private Gender gender;
        private DateTime? birthDate;
        private string birthPlace;
        private DateTime? deathDate;
        private string deathPlace;
        private bool isLiving;
        private PhotoCollection photos;
        private Story story;
        private RelationshipCollection relationships;

        #endregion
    }
}
