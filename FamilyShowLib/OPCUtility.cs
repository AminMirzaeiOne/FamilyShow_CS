using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace FamilyShowLib
{
    public class OPCUtility
    {
        private const string PackageRelationshipType =
           @"http://schemas.microsoft.com/opc/2006/sample/document";
        private const string ResourceRelationshipType =
            @"http://schemas.microsoft.com/opc/2006/sample/required-resource";


        /// <summary>
        /// Creates a package file containing the content from the specified directory.
        /// </summary>
        /// <param name="TargetDirectory">Path to directory containing the content to package</param>
        public static void CreatePackage(string PackageFileName, string TargetDirectory)
        {
            using (Package package = Package.Open(PackageFileName, FileMode.Create))
            {
                // Package the contents of the top directory
                DirectoryInfo mainDirectory = new DirectoryInfo(TargetDirectory);
                CreatePart(package, mainDirectory, false);

                // Package the contents of the sub-directories
                foreach (DirectoryInfo di in mainDirectory.GetDirectories())
                {
                    CreatePart(package, di, true);
                }
            }
        }

        /// <summary>
        /// Adds files from the specified directory as parts of the package
        /// </summary>
        private static void CreatePart(Package package, DirectoryInfo directoryInfo, bool storeInDirectory)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                // Only Add files for the following known types
                switch (file.Extension.ToLower())
                {
                    case ".xml":
                        CreateDocumentPart(package, file, MediaTypeNames.Text.Xml, storeInDirectory);
                        break;
                    case ".jpg":
                        CreateDocumentPart(package, file, MediaTypeNames.Image.Jpeg, storeInDirectory);
                        break;
                    case ".gif":
                        CreateDocumentPart(package, file, MediaTypeNames.Image.Gif, storeInDirectory);
                        break;
                    case ".rtf":
                        CreateDocumentPart(package, file, MediaTypeNames.Text.RichText, storeInDirectory);
                        break;
                    case ".txt":
                        CreateDocumentPart(package, file, MediaTypeNames.Text.Plain, storeInDirectory);
                        break;
                    case ".html":
                        CreateDocumentPart(package, file, MediaTypeNames.Text.Html, storeInDirectory);
                        break;
                }
            }
        }
    }
}
