using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace CAS
{
    public static class Extensions
    {
        public static int ParseToInt32(this string input, int defaultValue = 0)
        {
            int output;
            return Int32.TryParse(input, out output) ? output : defaultValue;
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> src)
        {
            return src ?? Enumerable.Empty<T>();
        }

        public static string GetString(this SqlDataReader reader, string column, string defaultValueIfNull = null, HashSet<string> columnList = null)
        {
            if (columnList == null)
            {
                return reader[column] == DBNull.Value ? defaultValueIfNull : (string)reader[column];
            }
            return columnList.Contains(column)
                ? (reader[column] == DBNull.Value ? defaultValueIfNull : (string)reader[column])
                : defaultValueIfNull;
        }

        public static long GetLong(this SqlDataReader reader, string column, long deaultValueIfNull = 0L, HashSet<string> columnList = null)
        {
            if (columnList == null)
            {
                return reader[column] == DBNull.Value ? deaultValueIfNull : (long)reader[column];
            }
            return columnList.Contains(column)
                ? (reader[column] == DBNull.Value ? deaultValueIfNull : (long)reader[column])
                : deaultValueIfNull;
        }

        public static int GetInt(this SqlDataReader reader, string column, int defaultValueIfNull = 0, HashSet<string> columnList = null)
        {
            if (columnList == null)
            {
                return reader[column] == DBNull.Value ? defaultValueIfNull : (int)reader[column];
            }
            return columnList.Contains(column)
                ? (reader[column] == DBNull.Value ? defaultValueIfNull : (int)reader[column])
                : defaultValueIfNull;
        }
    }

    public static class Utilities
    {
        public static HashSet<string> GetColumnNames(SqlDataReader sdr)
        {
            var ret = new HashSet<string>();
            for (int i = 0; i < sdr.FieldCount; i++)
                ret.Add(sdr.GetName(i));
            return ret;
        }
    }
}