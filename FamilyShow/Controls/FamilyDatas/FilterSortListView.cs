using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FamilyShow.Controls.FamilyDatas
{
    public class FilterSortListView : SortListView
    {
        private delegate void FilterDelegate();
        private Filter filter = new Filter();

        /// <summary>
        /// Get the filter for this control.
        /// </summary>
        protected Filter Filter
        {
            get { return this.filter; }
        }

        /// <summary>
        /// Filter the data using the specified filter text.
        /// </summary>
        public void FilterList(string text)
        {
            // Setup the filter object.
            filter.Parse(text);

            // Start an async operation that filters the list.
            this.Dispatcher.BeginInvoke(
                DispatcherPriority.ApplicationIdle,
                new FilterDelegate(FilterWorker));
        }


    }
}
