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

        /// <summary>
        /// Return true if the filter contains the specified date.
        /// </summary>
        public bool Matches(DateTime? date)
        {
            return (date != null && date.Value.ToShortDateString().Contains(this.filterText));
        }

        /// <summary>
        /// Return true if the filter contains the year in the specified date.
        /// </summary>
        public bool MatchesYear(DateTime? date)
        {
            return (date != null && date.Value.Year.ToString(CultureInfo.CurrentCulture).Contains(this.filterText));
        }

        /// <summary>
        /// Return true if the filter contains the month in the specified date.
        /// </summary>
        public bool MatchesMonth(DateTime? date)
        {
            return (date != null && this.filterDate != null &&
                date.Value.Month == this.filterDate.Value.Month);
        }

        /// <summary>
        /// Return true if the filter contains the day in the specified date.
        /// </summary>
        public bool MatchesDay(DateTime? date)
        {
            return (date != null && this.filterDate != null &&
                date.Value.Day == this.filterDate.Value.Day);
        }

        /// <summary>
        /// Return true if the filter contains the specified age. The filter can 
        /// represent a single age (10), a range (10-20), or an ending (10+).
        /// </summary>
        public bool Matches(int? age)
        {
            if (age == null)
                return false;

            // Check single age.
            if (this.minimumAge != null && age.Value == this.minimumAge.Value)
                return true;

            // Check for a range.
            if (this.minimumAge != null && this.maximumAge != null &&
                age.Value >= this.minimumAge && age <= this.maximumAge)
                return true;

            // Check for an ending age.
            if (this.minimumAge == null && this.maximumAge != null && age.Value >= this.maximumAge)
                return true;

            return false;
        }

        /// <summary>
        /// Parse the specified filter text.
        /// </summary>
        public void Parse(string text)
        {
            // Initialize fields.
            this.filterText = "";
            this.filterDate = null;
            this.minimumAge = null;
            this.maximumAge = null;

            // Store the filter text.
            this.filterText = string.IsNullOrEmpty(text) ? "" : text.ToLower(CultureInfo.CurrentCulture).Trim();

            // Parse date and age.
            ParseDate();
            ParseAge();
        }

        /// <summary>
        /// Parse the filter date.
        /// </summary>
        private void ParseDate()
        {
            DateTime date;
            if (DateTime.TryParse(this.filterText, out date))
                this.filterDate = date;
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
