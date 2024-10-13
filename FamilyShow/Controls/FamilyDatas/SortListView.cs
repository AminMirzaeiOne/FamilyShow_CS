using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FamilyShow.Controls.FamilyDatas
{
    public class SortListView : ListView
    {
        // The current column that is sorted.
        private SortListViewColumn sortColumn;

        // The previous column that was sorted.
        private SortListViewColumn previousSortColumn;

        // The current direction the header is sorted;
        private ListSortDirection sortDirection;

    }
}
