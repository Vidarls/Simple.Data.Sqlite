﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using Simple.Data.Ado;

namespace Simple.Data.Sqlite
{
    [Export(typeof(IQueryPager))]
    public class SqliteQueryPager : IQueryPager
    {
        private static readonly Regex ColumnExtract = new Regex(@"SELECT\s*(.*)\s*(FROM.*)", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        public IEnumerable<string> ApplyLimit(string sql, int take)
        {
            sql = AddMissingOrderBy(sql);
            yield return string.Format("{0} LIMIT {1}", sql, take);
        }

        public IEnumerable<string> ApplyPaging(string sql, int skip, int take)
        {
            sql = AddMissingOrderBy(sql);

            yield return string.Format("{0} LIMIT {1},{2}", sql, skip, take);
        }

        private string AddMissingOrderBy(string sql)
        {
            if (sql.IndexOf("order by", StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                var match = ColumnExtract.Match(sql);
                var columns = match.Groups[1].Value.Trim();
                sql += " ORDER BY " + columns.Split(',').First().Trim();
            }
            return sql;
        }
    }
}