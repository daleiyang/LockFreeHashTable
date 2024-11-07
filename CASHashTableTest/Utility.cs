using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

namespace Test
{
    class Utility
    {
        public static List<Record> GetData(int cnt)
        {
            var result = new List<Record>();
            var ran = new Random();
            var u8 = Encoding.UTF8;
            int linkId = 1;
            int clcId = 1;
            int sbp = 1;

            for (int i = 0; i < cnt; i++)
            {
                result.Add(new Record
                {
                    linkId = linkId % 4194303,
                    clcId = clcId % 262143,
                    sbp = sbp % 16383,
                    url = u8.GetBytes("http://www.microsoft.com/abc.asp+" + i.ToString())
                });

                linkId++; clcId++; sbp++;
            }

            return result;
        }
    }

    public struct Record
    {
        public long linkId;
        public long clcId;
        public long sbp;
        public byte[] url;
    }


    public static class Extensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> src)
        {
            return src ?? Enumerable.Empty<T>();
        }
    }
}