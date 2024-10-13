﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyShowLib;

namespace FamilyShow.Controls.FamilyDatas
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public class FamilyDisplayListView : FilterSortListView
    {
        /// <summary>
        /// Called for each item in the list. Return true if the item should be in
        /// the current result set, otherwise return false to exclude the item.
        /// </summary>
        protected override bool FilterCallback(object item)
        {
            Person person = item as Person;
            if (person == null)
                return false;

            if (this.Filter.Matches(person.Name) ||
                this.Filter.MatchesYear(person.BirthDate) ||
                this.Filter.MatchesYear(person.DeathDate) ||
                this.Filter.Matches(person.Age))
                return true;

            return false;
        }
    }
}
