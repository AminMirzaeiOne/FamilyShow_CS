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

        /// <summary>
        /// Adds the speficied file to the package as document part
        /// </summary>
        private static void CreateDocumentPart(Package package, FileInfo file, string contentType, bool storeInDirectory)
        {
            Uri partUriDocument;

            // Convert system path and file names to Part URIs.
            if (storeInDirectory)
                partUriDocument = PackUriHelper.CreatePartUri(new Uri(Path.Combine(file.Directory.Name, file.Name), UriKind.Relative));
            else
                partUriDocument = PackUriHelper.CreatePartUri(new Uri(file.Name, UriKind.Relative));

            // Add the Document part to the Package
            PackagePart packagePartDocument = package.CreatePart(
                partUriDocument, contentType);

            // Copy the data to the Document Part
            using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                CopyStream(fileStream, packagePartDocument.GetStream());
            }

            // Add a Package Relationship to the Document Part
            package.CreateRelationship(packagePartDocument.Uri, TargetMode.Internal, PackageRelationshipType);
        }


        /// <summary>
        ///   Extracts content and resource parts from a given Package
        ///   zip file to a specified target directory.</summary>
        /// <param name="packagePath">
        ///   The relative path and filename of the Package zip file.</param>
        /// <param name="targetDirectory">
        ///   The relative path from the current directory to the targer folder.
        /// </param>
        public static void ExtractPackage(string packagePath, string targetDirectory)
        {
            try
            {
                // Create a new Target directory.  If the Target directory
                // exists, first delete it and then create a new empty one.
                DirectoryInfo directoryInfo = new DirectoryInfo(targetDirectory);
                if (directoryInfo.Exists)
                    directoryInfo.Delete(true);
                directoryInfo.Create();
            }
            catch
            {
                // ignore errors
            }

            // Open the Package.
            using (Package package = Package.Open(packagePath, FileMode.Open, FileAccess.Read))
            {
                PackagePart documentPart = null;
                PackagePart resourcePart = null;

                // Get the Package Relationships and look for the Document part based on the RelationshipType
                Uri uriDocumentTarget = null;
                foreach (PackageRelationship relationship in package.GetRelationshipsByType(PackageRelationshipType))
                {
                    // Resolve the Relationship Target Uri so the Document Part can be retrieved.
                    uriDocumentTarget = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), relationship.TargetUri);

                    // Open the Document Part, write the contents to a file.
                    documentPart = package.GetPart(uriDocumentTarget);
                    ExtractPart(documentPart, targetDirectory);
                }

                // Get the Document part's Relationships, and look for required resources.
                Uri uriResourceTarget = null;
                foreach (PackageRelationship relationship in documentPart.GetRelationshipsByType(ResourceRelationshipType))
                {
                    // Resolve the Relationship Target Uri so the Resource Part can be retrieved.
                    uriResourceTarget = PackUriHelper.ResolvePartUri(documentPart.Uri, relationship.TargetUri);

                    // Open the Resource Part and write the contents to a file.
                    resourcePart = package.GetPart(uriResourceTarget);
                    ExtractPart(resourcePart, targetDirectory);
                }
            }
        }


    }
}
