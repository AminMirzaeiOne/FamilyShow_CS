using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyShowLib
{
    /// <summary>
    /// Simple representation of a serializable photo associated with the Person class
    /// </summary>
    [Serializable]
    public class Photo : INotifyPropertyChanged
    {
        // The constants specific to this class
        public static class Const
        {
            public const string PhotosFolderName = "Images";
        }

        private string relativePath;
        private bool isAvatar;

        /// <summary>
        /// The relative path to the photo.
        /// </summary>
        public string RelativePath
        {
            get { return relativePath; }
            set
            {
                if (relativePath != value)
                {
                    relativePath = value;
                    OnPropertyChanged("relativePath");
                }
            }
        }

    }
}
