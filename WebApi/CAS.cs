using CAS;
using System.Text;

namespace WebApi
{
    public struct Record
    {
        public long linkId;
        public long clcId;
        public long sbp;
        public byte[] url;
    }

    public class Result
    {
        public int status { get; set; }
        public string value { get; set; }
        public string message { get; set; }
    }

    public class DB
    {
        private int _arrayLength = 10107313;
        private int _contentLength = 256;
        private int _dataNumber = 3000000;
        private KeyIn54BitCASHashTable _hashTable;
        private Encoding u8 = Encoding.UTF8;

        public DB()
        {
            _hashTable = new KeyIn54BitCASHashTable(_arrayLength, _contentLength);
            var records = GetData(_dataNumber);
            foreach(var record in records)
            {
                _hashTable.TrySet(record.linkId, record.clcId, record.sbp, record.url);
            }
        }

        public Result TryGet(long linkId, long clcId, long sbp)
        {
            byte[] output;
            int status = _hashTable.TryGet(linkId, clcId, sbp, out output);
            var result = new Result();
            result.status = status;
            result.value = "";
            string key = "linkId = " + linkId.ToString() + " clcId = " + clcId.ToString() + " sbp = " + sbp.ToString() + " ";
            switch (status)
            {
                case 0:
                    result.value = u8.GetString(output);
                    result.message = key + " get successfully.";
                    break;
                case -2:
                    result.message = key + " is not exist.";
                    break;
                case -3:
                    result.message = key + " has been deleted.";
                    break;
            }
            return result;
        }

        public Result TrySet(long linkId, long clcId, long sbp, byte[] url)
        {
            int status = _hashTable.TrySet(linkId, clcId, sbp, url);
            var result = new Result();
            result.status = status;
            result.value = u8.GetString(url);
            string key = "linkId = " + linkId.ToString() + " clcId = " + clcId.ToString() + " sbp = " + sbp.ToString() + " ";
            switch (status)
            {
                case 0:
                    result.message = key + " added successfully.";
                    break;
                case 1:
                    result.message = key + " updated successfully.";
                    break;
            }
            return result;
        }

        public Result TryDelete(long linkId, long clcId, long sbp)
        {
            int status = _hashTable.TryDelete(linkId, clcId, sbp);
            var result = new Result();
            result.status = status;
            result.value = "";
            string key = "linkId = " + linkId.ToString() + " clcId = " + clcId.ToString() + " sbp = " + sbp.ToString() + " ";
            switch (status)
            {
                case 0:
                    result.message = key + " deleted successfully.";
                    break;
                case -2:
                    result.message = key + " is not exist.";
                    break;
                case -3:
                    result.message = key + " has been delete by other process.";
                    break;
            }
            return result;
        }

        public List<Record> Data()
        {
            return GetData(_dataNumber);
        }

        private List<Record> GetData(int cnt)
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
}
