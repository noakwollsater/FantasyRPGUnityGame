// Copyright (c) 2024 Synty Studios Limited. All rights reserved.
//
// Use of this software is subject to the terms and conditions of the Synty Studios End User Licence Agreement (EULA)
// available at: https://syntystore.com/pages/end-user-licence-agreement
//
// For additional details, see the LICENSE.MD file bundled with this software.

using SqlCipher4Unity3D;
using SQLite.Attributes;
using System.Collections.Generic;

namespace Synty.SidekickCharacters.Database.DTO
{
    [Table("sk_part_filter_row")]
    public class SidekickPartFilterRow
    {
        private SidekickPartFilter _filter;
        private SidekickPart _part;

        [PrimaryKey, AutoIncrement, Column("id")]
        public int ID { get; set; }
        [Column("ptr_filter")]
        public int PtrFilter { get; set; }
        [Column("ptr_part")]
        public int PtrPart { get; set; }

        [Ignore]
        public SidekickPartFilter Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                PtrFilter = value.ID;
            }
        }

        [Ignore]
        public SidekickPart Part
        {
            get => _part;
            set
            {
                _part = value;
                PtrPart = value.ID;
            }
        }

        /// <summary>
        ///     Gets a specific Part Filter by its database ID.
        /// </summary>
        /// <param name="dbManager">The Database Manager to use.</param>
        /// <param name="id">The id of the required Part Filter.</param>
        /// <returns>The specific Part Filter if it exists; otherwise null.</returns>
        public static SidekickPartFilterRow GetByID(DatabaseManager dbManager, int id)
        {
            SidekickPartFilterRow filterRow = dbManager.GetCurrentDbConnection().Find<SidekickPartFilterRow>(id);
            Decorate(dbManager, filterRow);
            return filterRow;
        }

        /// <summary>
        ///     Gets a list of all the part filters in the database.
        /// </summary>
        /// <param name="dbManager">The Database Manager to use.</param>
        /// <returns>A list of all part filters in the database.</returns>
        public static List<SidekickPartFilterRow> GetAll(DatabaseManager dbManager)
        {
            List<SidekickPartFilterRow> filterRows = dbManager.GetCurrentDbConnection().Table<SidekickPartFilterRow>().ToList();

            foreach (SidekickPartFilterRow row in filterRows)
            {
                Decorate(dbManager, row);
            }

            return filterRows;
        }

        /// <summary>
        ///     Gets a list of all the part filter rows in the database for a given part filter.
        /// </summary>
        /// <param name="dbManager">The Database Manager to use.</param>
        /// <param name="filter">The Part Filter to get all the part filter rows for.</param>
        /// <returns>A list of all part filter rows for the given Part Filter in the database.</returns>
        public static List<SidekickPartFilterRow> GetAllForFilter(DatabaseManager dbManager, SidekickPartFilter filter)
        {
            List<SidekickPartFilterRow> filterRows = dbManager.GetCurrentDbConnection().Table<SidekickPartFilterRow>().Where(filterRow => filterRow
                .PtrFilter == filter.ID).ToList();

            foreach (SidekickPartFilterRow row in filterRows)
            {
                Decorate(dbManager, row);
            }

            return filterRows;
        }

        /// <summary>
        ///     Gets a list of all the part filter rows in the database for a given part filter.
        /// </summary>
        /// <param name="dbManager">The Database Manager to use.</param>
        /// <param name="filter">The Part Filter to get all the part filter row for.</param>
        /// <param name="part">The part to get the row for.</param>
        /// <returns>The part filter row for the given Part Filter and Part in the database.</returns>
        public static SidekickPartFilterRow GetForFilterAndPart(DatabaseManager dbManager, SidekickPartFilter filter, SidekickPart part)
        {
            SidekickPartFilterRow row = dbManager.GetCurrentDbConnection().Table<SidekickPartFilterRow>().FirstOrDefault(filterRow => filterRow
                    .PtrFilter == filter.ID && filterRow.PtrPart == part.ID
            );

            Decorate(dbManager, row);

            return row;
        }

        /// <summary>
        ///     Ensures that the given part filter row has its nice DTO class properties set
        /// </summary>
        /// <param name="dbManager">The Database Manager to use.</param>
        /// <param name="filterRow">The part filter row to decorate</param>
        private static void Decorate(DatabaseManager dbManager, SidekickPartFilterRow filterRow)
        {
            filterRow.Filter ??= SidekickPartFilter.GetByID(dbManager, filterRow.PtrFilter);
            filterRow.Part ??= SidekickPart.GetByID(dbManager, filterRow.PtrPart);
        }

        /// <summary>
        ///     Updates or Inserts this item in the Database.
        /// </summary>
        /// <param name="dbManager">The database manager to use.</param>
        public int Save(DatabaseManager dbManager)
        {
            if (ID <= 0)
            {
                dbManager.GetCurrentDbConnection().Insert(this);
                // in theory this could return a different ID, but in practice it's highly unlikely
                ID = (int) SQLite3.LastInsertRowid(dbManager.GetCurrentDbConnection().Handle);
            }
            dbManager.GetCurrentDbConnection().Update(this);
            return ID;
        }

        /// <summary>
        ///     Deletes this item from the database
        /// </summary>
        /// <param name="dbManager">The database manager to use.</param>
        public void Delete(DatabaseManager dbManager)
        {
            dbManager.GetCurrentDbConnection().Delete<SidekickPartFilterRow>(ID);
        }
    }
}
