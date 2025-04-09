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
using UnityEngine;

namespace Synty.SidekickCharacters.Filters
{
    public class FilterGroup
    {
        public DatabaseManager DbManager;
        public FilterCombineType CombineType;

        private List<SidekickPart> _filteredParts;
        private List<FilterItem> _filterItems = new List<FilterItem>();
        private List<FilterGroup> _subGroups = new List<FilterGroup>();

        /// <summary>
        ///     Adds a sub group of filters to this group.
        /// </summary>
        /// <param name="subGroup">The group to add as a sub group.</param>
        public void AddFilterSubGroup(FilterGroup subGroup)
        {
            if (_subGroups.Count < 1 || _subGroups[0].CombineType == subGroup.CombineType)
            {
                _subGroups.Add(subGroup);
            }
            else
            {
                Debug.LogWarning("Unable to add sub group as sub groups cannot have different combine types.");
            }
        }

        /// <summary>
        ///     Adds a filter item to this group.
        /// </summary>
        /// <param name="filterItem">The filter item to add.</param>
        public void AddFilterItem(FilterItem filterItem)
        {
            if (_filterItems.Count < 1 || _filterItems[0].CombineType == filterItem.CombineType)
            {
                _filterItems.Add(filterItem);
            }
            else
            {
                Debug.LogWarning("Unable to add filter as filters cannot have different combine types.");
            }
        }

        /// <summary>
        ///     Removes the given filter item from this group, if it exists.
        /// </summary>
        /// <param name="filterItem">The filter item to remove.</param>
        public void RemoveFilterItem(FilterItem filterItem)
        {
            _filterItems.RemoveAll(fi => fi.Filter == filterItem.Filter);
        }

        /// <summary>
        ///     Gets a list of all parts that the filters within this group evaluate to.
        /// </summary>
        /// <returns>The list of all filtered parts</returns>
        public List<SidekickPart> GetFilteredParts()
        {
            _filteredParts = new List<SidekickPart>();

            // If there are no filter items, it means that we aren't filtering any parts out. Return all parts.
            if (_filterItems.Count < 1)
            {
                return SidekickPart.GetAll(DbManager);
            }

            if (_filterItems.Count > 1)
            {
                FilterCombineType filterCombineType = _filterItems[1].CombineType;
                _filteredParts = _filterItems[0].CombineType == FilterCombineType.Or ? new List<SidekickPart>() : SidekickPart.GetAll(DbManager);

                foreach (FilterItem item in _filterItems)
                {
                    List<SidekickPart> newFilteredParts = item.GetFilteredParts();
                    List<SidekickPart> toRemove = new List<SidekickPart>();

                    switch (filterCombineType)
                    {
                        case FilterCombineType.And:
                            toRemove = new List<SidekickPart>();

                            foreach (SidekickPart part in _filteredParts)
                            {
                                if (!newFilteredParts.Contains(part))
                                {
                                    toRemove.Add(part);
                                }
                            }

                            _filteredParts.RemoveAll(part => toRemove.Contains(part));
                            break;
                        case FilterCombineType.Or:
                            foreach (SidekickPart part in newFilteredParts)
                            {
                                if (!_filteredParts.Contains(part))
                                {
                                    _filteredParts.Add(part);
                                }
                            }
                            break;
                        case FilterCombineType.Not:
                            toRemove = new List<SidekickPart>();

                            foreach (SidekickPart part in _filteredParts)
                            {
                                if (newFilteredParts.Contains(part))
                                {
                                    toRemove.Add(part);
                                }
                            }

                            _filteredParts.RemoveAll(part => toRemove.Contains(part));
                            break;
                    }
                }
            }
            else
            {
                _filteredParts = _filterItems[0].GetFilteredParts();
            }

            foreach (FilterGroup group in _subGroups)
            {
                FilterCombineType filterCombineType = group.CombineType;
                List<SidekickPart> newFilteredParts = group.GetFilteredParts();
                List<SidekickPart> toRemove = new List<SidekickPart>();

                switch (filterCombineType)
                {
                    case FilterCombineType.And:
                        toRemove = new List<SidekickPart>();

                        foreach (SidekickPart part in _filteredParts)
                        {
                            if (!newFilteredParts.Contains(part))
                            {
                                toRemove.Add(part);
                            }
                        }

                        _filteredParts.RemoveAll(part => toRemove.Contains(part));
                        break;
                    case FilterCombineType.Or:
                        foreach (SidekickPart part in newFilteredParts)
                        {
                            if (!_filteredParts.Contains(part))
                            {
                                _filteredParts.Add(part);
                            }
                        }
                        break;
                    case FilterCombineType.Not:
                        toRemove = new List<SidekickPart>();

                        foreach (SidekickPart part in _filteredParts)
                        {
                            if (newFilteredParts.Contains(part))
                            {
                                toRemove.Add(part);
                            }
                        }

                        _filteredParts.RemoveAll(part => toRemove.Contains(part));
                        break;
                }
            }

            return _filteredParts;
        }
    }
}
