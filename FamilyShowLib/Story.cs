using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FamilyShowLib
{
    public class Story
    {
        // The constants specific to this class
        public static class Const
        {
            // Name of the folder
            public const string StoriesFolderName = "Stories";
        }

        private string relativePath;

        #region Properties

        /// <summary>
        /// The relative path to the story file.
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

        /// <summary>
        /// The fully qualified path to the story.
        /// </summary>
        [XmlIgnore]
        public string AbsolutePath
        {
            get
            {
                string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    App.ApplicationFolderName);
                tempFolder = Path.Combine(tempFolder, App.AppDataFolderName);

                if (relativePath != null)
                    return Path.Combine(tempFolder, relativePath);
                else
                    return string.Empty;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Empty constructor is needed for serialization
        /// </summary>
        public Story() { }

        #endregion

    }
}
