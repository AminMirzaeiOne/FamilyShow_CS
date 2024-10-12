using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    }
}
