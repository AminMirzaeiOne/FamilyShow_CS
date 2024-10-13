using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

namespace FamilyShow.Controls.FamilyDatas
{

    public class Filter
    {
        // Parsed data from the filter string.
        private string filterText;
        private int? maximumAge;
        private int? minimumAge;
        private DateTime? filterDate;

        /// <summary>
        /// Indicates if the filter is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(this.filterText); }
        }

        /// <summary>
        /// Return true if the filter contains the specified text.
        /// </summary>
        public bool Matches(string text)
        {
            return (this.filterText != null && text != null &&
                text.ToLower(CultureInfo.CurrentCulture).Contains(this.filterText));
        }

    }

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

        /// <summary>
        /// Worker method that filters the list.
        /// </summary>
        private void FilterWorker()
        {
            // Get the data the ListView is bound to.
            ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemsSource);

            // Clear the list if the filter is empty, otherwise filter the list.
            view.Filter = filter.IsEmpty ? null :
                new Predicate<object>(FilterCallback);
        }

        /// <summary>
        /// This is called for each item in the list. The derived classes 
        /// override this method.
        /// </summary>
        virtual protected bool FilterCallback(object item)
        {
            return false;
        }


    }
}
