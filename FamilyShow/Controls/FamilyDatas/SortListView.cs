using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        protected override void OnInitialized(EventArgs e)
        {
            // Handle the event when a header is clicked.
            this.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(OnHeaderClicked));
            base.OnInitialized(e);
        }

    }
}
