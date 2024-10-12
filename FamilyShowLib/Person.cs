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

    }
}
