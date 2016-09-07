using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using CAS;
using System.Text;

namespace Test
{
    class Utility
    {
        public static async Task<List<Record>> GetData()
        {
            string connectionString = "Data Source=tcp:XXXXX;Initial Catalog=XXXXX;User ID=XXXXX;Password=XXXXX;Trusted_Connection=False;Encrypt=True;Connection Timeout=3600";
            string query = " SELECT Top 2000 LinkId, CLCID, SBP, MAX(URL) AS URL " +
                           " FROM (" +
                           " SELECT LinkId, CLCID," +
                           " CASE " +
                           "    WHEN LEFT(SBP, 3) = 'pc=' THEN SBP " +
                           "    ELSE '''' " +
                           " END AS SBP, " +
                           " URL " +
                           " FROM [dbo].[Links] WHERE deleted = 0 ) AS  A " +
                           " GROUP BY LinkId, CLCID, SBP";

            var result = new List<Record>();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var command = new SqlCommand(query, conn);
                    command.CommandTimeout = 0;
                    var reader = await command.ExecuteReaderAsync();
                    HashSet<string> cl;
                    var u8 = Encoding.UTF8;

                    if (reader.HasRows)
                    {
                        cl = Utilities.GetColumnNames(reader);
                        while (reader.Read())
                        {
                            result.Add(new Record{
                                linkId = reader.GetLong("LinkId", columnList: cl),
                                clcId = reader.GetInt("CLCID", columnList: cl),
                                sbp = reader.GetString("SBP", columnList: cl),
                                url = u8.GetBytes(reader.GetString("URL", columnList: cl))
                            });
                        }
                    }
                }
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
            return result;
        }

        public static async Task<List<RecordString>> GetDataString()
        {
            string connectionString = "Data Source=tcp:XXXXX;Initial Catalog=XXXXX;User ID=XXXXX;Password=XXXXX;Trusted_Connection=False;Encrypt=True;Connection Timeout=3600";
            string query = " SELECT  Top 2000 LinkId, CLCID, SBP, MAX(URL) AS URL " +
                           " FROM (" +
                           " SELECT LinkId, CLCID," +
                           " CASE " +
                           "    WHEN LEFT(SBP, 3) = 'pc=' THEN SBP " +
                           "    ELSE '''' " +
                           " END AS SBP, " +
                           " URL " +
                           " FROM [dbo].[Links] WHERE deleted = 0 ) AS  A " +
                           " GROUP BY LinkId, CLCID, SBP";

            var result = new List<RecordString>();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var command = new SqlCommand(query, conn);
                    command.CommandTimeout = 0;
                    var reader = await command.ExecuteReaderAsync();
                    HashSet<string> cl;

                    if (reader.HasRows)
                    {
                        cl = Utilities.GetColumnNames(reader);
                        while (reader.Read())
                        {
                            result.Add(new RecordString
                            {
                                linkId = reader.GetLong("LinkId", columnList: cl),
                                clcId = reader.GetInt("CLCID", columnList: cl),
                                sbp = reader.GetString("SBP", columnList: cl),
                                url = reader.GetString("URL", columnList: cl)
                            });
                        }
                    }
                }
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
            return result;
        }

        public static async Task<ArrayList> GetDataArrayList()
        {
            string connectionString = "Data Source=tcp:XXXXX;Initial Catalog=XXXXX;User ID=XXXXX;Password=XXXXX;Trusted_Connection=False;Encrypt=True;Connection Timeout=3600";
            string query = " SELECT  Top 2000 LinkId, CLCID, SBP, MAX(URL) AS URL " +
                           " FROM (" +
                           " SELECT LinkId, CLCID," +
                           " CASE " +
                           "    WHEN LEFT(SBP, 3) = 'pc=' THEN SBP " +
                           "    ELSE '''' " +
                           " END AS SBP, " +
                           " URL " +
                           " FROM [dbo].[Links] WHERE deleted = 0 ) AS  A " +
                           " GROUP BY LinkId, CLCID, SBP";

            var result = new ArrayList();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var command = new SqlCommand(query, conn);
                    command.CommandTimeout = 0;
                    var reader = await command.ExecuteReaderAsync();
                    HashSet<string> cl;
                    var u8 = Encoding.UTF8;

                    if (reader.HasRows)
                    {
                        cl = Utilities.GetColumnNames(reader);
                        while (reader.Read())
                        {
                            result.Add(new Record
                            {
                                linkId = reader.GetLong("LinkId", columnList: cl),
                                clcId = reader.GetInt("CLCID", columnList: cl),
                                sbp = reader.GetString("SBP", columnList: cl),
                                url = u8.GetBytes(reader.GetString("URL", columnList: cl))
                            });
                        }
                    }
                }
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
            return result;
        }
    }

    public struct Record
    {
        public long linkId;
        public long clcId;
        public string sbp;
        public byte[] url;
    }

    public struct RecordString
    {
        public long linkId;
        public long clcId;
        public string sbp;
        public string url;
    }
}