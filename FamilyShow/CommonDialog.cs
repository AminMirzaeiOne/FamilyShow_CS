using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyShow
{
    public class CommonDialog
    {
        #region fields

        // Structure used when displaying Open and SaveAs dialogs.
        private OpenFileName ofn = new OpenFileName();

        // List of filters to display in the dialog.
        private List<FilterEntry> filter = new List<FilterEntry>();

        #endregion

        #region properties

        public List<FilterEntry> Filter
        {
            get { return filter; }
        }

        public string Title
        {
            set { ofn.title = value; }
        }

        public string InitialDirectory
        {
            set { ofn.initialDir = value; }
        }

        public string DefaultExtension
        {
            set { ofn.defExt = value; }
        }

        public string FileName
        {
            get { return ofn.file; }
        }

        #endregion
    }
}
