// Copyright (c) 2024 Synty Studios Limited. All rights reserved.
//
// Use of this software is subject to the terms and conditions of the Synty Studios End User Licence Agreement (EULA)
// available at: https://syntystore.com/pages/end-user-licence-agreement
//
// For additional details, see the LICENSE.MD file bundled with this software.

using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Synty.SidekickCharacters.Filters
{
    public class FilterItem
    {
        public DatabaseManager DbManager;
        public SidekickPartFilter Filter;
        public FilterCombineType CombineType;

        private List<SidekickPart> _filteredParts;

        public FilterItem(DatabaseManager dbManager, SidekickPartFilter filter, FilterCombineType combineType)
        {
            DbManager = dbManager;
            Filter = filter;
            CombineType = combineType;
        }

        /// <summary>
        ///     Gets a list of all the parts for this filter item.
        /// </summary>
        /// <returns>A list of all parts for this filter item.</returns>
        public List<SidekickPart> GetFilteredParts()
        {
            if (_filteredParts == null || _filteredParts.Count < 1)
            {
                _filteredParts = SidekickPartFilterRow.GetAllForFilter(DbManager, Filter).Select(row => row.Part).ToList();
            }

            return _filteredParts;
        }
    }
}
