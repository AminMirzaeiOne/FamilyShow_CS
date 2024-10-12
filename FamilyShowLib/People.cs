using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FamilyShowLib
{
    public class ContentChangedEventArgs : EventArgs
    {
        private Person newPerson;

        public Person NewPerson
        {
            get { return newPerson; }
        }

        public ContentChangedEventArgs(Person newPerson)
        {
            this.newPerson = newPerson;
        }

    }



    /// <summary>
    /// Contains the collection of person nodes and which person in the list is the currently
    /// selected person. This class exists mainly because of xml serialization limitations.
    /// Properties are not serialized in a class that is derived from a collection class 
    /// (as the PeopleCollection class is). Therefore the People collection is contained in 
    /// this class, along with other important properties that need to be serialized.
    /// </summary>
    [XmlRoot("Family")]
    [XmlInclude(typeof(ParentRelationship))]
    [XmlInclude(typeof(ChildRelationship))]
    [XmlInclude(typeof(SpouseRelationship))]
    [XmlInclude(typeof(SiblingRelationship))]
    public class People
    {
        // The constants specific to this class
        private static class Const
        {
            public const string DataFileName = "default.family";
        }

        // Fields
        private PeopleCollection peopleCollection;

        // The current person's Id will be serialized instead of the current person object to avoid
        // circular references during Xml Serialization. When family data is loaded, the corresponding
        // person object will be assigned to the current property (please see app.xaml.cs).
        // The currentPersonId is set in the Save method of this class.
        private string currentPersonId;

        // Store the person's name with the Id to make the xml file more readable.
        // The currentPersonName is set in the Save method of this class.
        private string currentPersonName;

        // The fully qualified path and filename for the family file.
        private string fullyQualifiedFilename;

        // Version of the file. This is not used at this time, but allows a future
        // version of the application to handle previous file formats.
        private string fileVersion = "1.0";

        private string OPCContentFileName = "content.xml";


        /// <summary>
        /// Collection of people.
        /// </summary>
        public PeopleCollection PeopleCollection
        {
            get { return peopleCollection; }
        }

        /// <summary>
        /// Id of currently selected person.
        /// </summary>
        [XmlAttribute(AttributeName = "Current")]
        public string CurrentPersonId
        {
            get { return currentPersonId; }
            set { currentPersonId = value; }
        }

        // Name of current selected person (included for readability in xml file).
        [XmlAttribute(AttributeName = "CurrentName")]
        public string CurrentPersonName
        {
            get { return currentPersonName; }
            set { currentPersonName = value; }
        }

        // Version of the file.
        [XmlAttribute(AttributeName = "FileVersion")]
        public string Version
        {
            get { return fileVersion; }
            set { fileVersion = value; }
        }


        [XmlIgnore]
        public static string ApplicationFolderPath
        {
            get
            {
                return Path.Combine(
                  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                  App.ApplicationFolderName);
            }
        }

        [XmlIgnore]
        public static string DefaultFullyQualifiedFilename
        {
            get
            {
                // Absolute path to the application folder
                string appLocation = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    App.ApplicationFolderName);

                // Create the directory if it doesn't exist
                if (!Directory.Exists(appLocation))
                    Directory.CreateDirectory(appLocation);

                return Path.Combine(appLocation, Const.DataFileName);
            }
        }

        /// <summary>
        /// Fully qualified filename (absolute pathname and filename) for the data file
        /// </summary>
        [XmlIgnore]
        public string FullyQualifiedFilename
        {
            get { return fullyQualifiedFilename; }

            set { fullyQualifiedFilename = value; }
        }

        public People()
        {
            this.peopleCollection = new PeopleCollection();
        }

        /// <summary>
        /// Persist the current list of people to disk.
        /// </summary>
        public void Save()
        {
            // Return right away if nothing to save.
            if (this.PeopleCollection == null || this.PeopleCollection.Count == 0)
                return;

            // Set the current person id and name before serializing
            this.CurrentPersonName = this.PeopleCollection.Current.FullName;
            this.CurrentPersonId = this.PeopleCollection.Current.Id;

            // Use the default path and filename if none was provided
            if (string.IsNullOrEmpty(this.FullyQualifiedFilename))
                this.FullyQualifiedFilename = People.DefaultFullyQualifiedFilename;

            // Setup temp folders for this family to be packaged into OPC later
            string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                App.ApplicationFolderName);
            tempFolder = Path.Combine(tempFolder, App.AppDataFolderName);

            // Create the necessary directories
            Directory.CreateDirectory(tempFolder);

            // Create xml content file
            XmlSerializer xml = new XmlSerializer(typeof(People));
            using (Stream stream = new FileStream(Path.Combine(tempFolder, OPCContentFileName), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xml.Serialize(stream, this);
            }

            // save to file package
            OPCUtility.CreatePackage(FullyQualifiedFilename, tempFolder);

            this.PeopleCollection.IsDirty = false;
        }


    }
}
