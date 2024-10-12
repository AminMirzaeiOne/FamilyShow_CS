using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

        /// The fully qualified path to the photo.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
        public string FullyQualifiedPath
        {
            get
            {
                string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    App.ApplicationFolderName);
                tempFolder = Path.Combine(tempFolder, App.AppDataFolderName);

                return Path.Combine(tempFolder, relativePath);
            }
            set
            {
                // This empty setter is needed for serialization.
            }
        }

        /// <summary>
        /// Whether the photo is the avatar photo or not.
        /// </summary>
        public bool IsAvatar
        {
            get { return isAvatar; }
            set
            {
                if (isAvatar != value)
                {
                    isAvatar = value;
                    OnPropertyChanged("IsAvatar");
                }
            }
        }

    }
}
