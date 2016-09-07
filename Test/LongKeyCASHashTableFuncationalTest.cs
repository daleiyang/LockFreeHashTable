using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CAS;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;

namespace Test
{
    [TestClass]
    public class LongKeyCASHashTableFuncationalTest
    {
        private int arrayLength = 4139;
        //private int arrayLength = 10368739;
        private int keyByteArrayLength = 256;
        private int contentByteArrayLength = 256;

        [TestMethod]
        public void LongKeyCASHashTableInitialize()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var hashTable = new CAS.LongKeyCASHashTable(arrayLength, keyByteArrayLength, contentByteArrayLength);

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            Console.WriteLine("Create hash table" + "\t" + ts.ToString());
        }

        [TestMethod]
        public void LongKeyCASHashTableCheckAddTotalSync()
        {
            var stopWatch = new Stopwatch();
            var hashTable = new CAS.LongKeyCASHashTable(arrayLength, keyByteArrayLength, contentByteArrayLength);
            var ts = stopWatch.Elapsed;
            Console.WriteLine("Create hash table" + "\t" + ts.ToString());

            stopWatch.Restart();
            var result = Utility.GetDataString().Result;
            stopWatch.Stop();
            Console.WriteLine("Get " + result.Count + " data from database" + "\t" + stopWatch.Elapsed.ToString());

            var u8 = Encoding.UTF8;
            string key = null;
            byte[] content = null;

            stopWatch.Restart();
            foreach (var record in result.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                content = u8.GetBytes(record.url);
                //stopWatch.Restart();
                Assert.AreEqual(hashTable.TrySet(key, content), 0);
                //stopWatch.Stop();
                //Console.WriteLine("Add one records" + "\t" + stopWatch.Elapsed.ToString());
            }
            stopWatch.Stop();
            Console.WriteLine("Add all records" + "\t" + stopWatch.Elapsed.ToString());

            content = null;

            foreach (var record in result.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                Assert.AreEqual(hashTable.TryGet(key, out content), 0);
                Assert.AreEqual(record.url, u8.GetString(content));
                content = null;
            }
        }

        [TestMethod]
        public void LongKeyCASHashTableCheckAddInvildInput()
        {
            var hashTable = new CAS.LongKeyCASHashTable(arrayLength, keyByteArrayLength, contentByteArrayLength);

            string message = null;
            byte[] content = new byte[] { 0xff };

            try { hashTable.TrySet(message, content); }
            catch (ArgumentNullException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("key"));

            message = Encoding.UTF8.GetString(new byte[keyByteArrayLength+1]);
            content = null;

            try { hashTable.TrySet(message, content); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("Key length shouldn't longer than init value when you create hash table."));

            message = "";
            content = null;

            try { hashTable.TrySet(message, content); }
            catch (ArgumentNullException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("value"));

            message = "";
            content = new byte[contentByteArrayLength + 1];

            try { hashTable.TrySet(message, content); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("Content length shouldn't longer than init value when you create hash table."));
        }

        [TestMethod]
        public void LongKeyCASHashTableCheckGet()
        {
            var hashTable = new CAS.LongKeyCASHashTable(arrayLength, keyByteArrayLength, contentByteArrayLength);
            var result = Utility.GetDataString().Result;

            var u8 = Encoding.UTF8;
            string key = null;
            byte[] content = null;

            foreach (var record in result.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                content = u8.GetBytes(record.url);
                Assert.AreEqual(hashTable.TrySet(key, content), 0);
            }

            content = null;
            foreach (var record in result)
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                Assert.AreEqual(hashTable.TryGet(key, out content), 0);
                Assert.AreEqual(record.url, u8.GetString(content));
                content = null;
            }
        }

        [TestMethod]
        public void LongKeyCASHashTableCheckGetInvlidInput()
        {
            var hashTable = new CAS.LongKeyCASHashTable(arrayLength, keyByteArrayLength, contentByteArrayLength);

            string message = null;
            byte[] content = null;

            try { hashTable.TryGet(message, out content); }
            catch (ArgumentNullException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("key"));

            message = Encoding.UTF8.GetString(new byte[keyByteArrayLength + 1]);
            content = null;

            try { hashTable.TryGet(message, out content); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("Key length shouldn't longer than init value when you create hash table."));
        }

        [TestMethod]
        public void LongKeyCASHashTableCheckGetNotFound()
        {
            var hashTable = new CAS.LongKeyCASHashTable(arrayLength, keyByteArrayLength, contentByteArrayLength);

            string message = "";
            byte[] content = null;

            Assert.AreEqual(hashTable.TryGet(message, out content), -2);
            Assert.IsNull(content);
        }

        [TestMethod]
        public void LongKeyCASHashTableCheckGetDeleted()
        {
            var hashTable = new CAS.LongKeyCASHashTable(arrayLength, keyByteArrayLength, contentByteArrayLength);

            string message = "";
            byte[] content = new byte[] { 0xff };

            Assert.AreEqual(hashTable.TrySet(message, content), 0);
            Assert.AreEqual(hashTable.TryDelete(message), 0);

            content = null;
            Assert.AreEqual(hashTable.TryGet(message, out content), -3);
            Assert.IsNull(content);
        }

        [TestMethod]
        public void LongKeyCASHashTableCheckDelete()
        {
            var hashTable = new CAS.LongKeyCASHashTable(arrayLength, keyByteArrayLength, contentByteArrayLength);
            var result = Utility.GetDataString().Result;

            var u8 = Encoding.UTF8;
            string key = null;
            byte[] content = null;

            foreach (var record in result.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                content = u8.GetBytes(record.url);
                Assert.AreEqual(hashTable.TrySet(key, content), 0);
            }

            key = null;
            content = null;

            foreach (var record in result.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                Assert.AreEqual(hashTable.TryDelete(key), 0);
                Assert.AreEqual(hashTable.TryGet(key, out content), -3);
                Assert.IsNull(content);
                content = null;
            }
        }

        [TestMethod]
        public void LongKeyCASHashTableCheckDeleteInvildInput()
        {
            var hashTable = new CAS.LongKeyCASHashTable(arrayLength, keyByteArrayLength, contentByteArrayLength);

            string message = null;

            try { hashTable.TryDelete(message); }
            catch (ArgumentNullException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("key"));

            message = Encoding.UTF8.GetString(new byte[keyByteArrayLength + 1]);

            try { hashTable.TryDelete(message); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("Key length shouldn't longer than init value when you create hash table."));
        }

        [TestMethod]
        public void LongKeyCASHashTableCheckDeleteNotFound()
        {
            var hashTable = new CAS.LongKeyCASHashTable(arrayLength, keyByteArrayLength, contentByteArrayLength);
            string message = "";

            Assert.AreEqual(hashTable.TryDelete(message), -2);
        }

        [TestMethod]
        public void LongKeyCASHashTableCheckDeleteAgain()
        {
            var stopWatch = new Stopwatch();
            var hashTable = new CAS.LongKeyCASHashTable(arrayLength, keyByteArrayLength, contentByteArrayLength);
            var result = Utility.GetDataString().Result;

            var u8 = Encoding.UTF8;
            string key = null;
            byte[] content = null;

            foreach (var record in result.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                content = u8.GetBytes(record.url);
                Assert.AreEqual(hashTable.TrySet(key, content), 0);
            }

            key = null;
            content = null;

            foreach (var record in result.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                Assert.AreEqual(hashTable.TryDelete(key), 0);
                Assert.AreEqual(hashTable.TryGet(key, out content), -3);
                Assert.IsNull(content);
                content = null;
            }

            key = null;
            content = null;
            //Delete all the records again and nothing should happens.
            foreach (var record in result.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                Assert.AreEqual(hashTable.TryDelete(key), -3);
                Assert.AreEqual(hashTable.TryGet(key, out content), -3);
                Assert.IsNull(content);
                content = null;
            }
        }

        [TestMethod]
        public void LongKeyCASHashTableCheckUpdate()
        {
            var hashTable = new CAS.LongKeyCASHashTable(arrayLength, keyByteArrayLength, contentByteArrayLength);
            var result = Utility.GetDataString().Result;

            var u8 = Encoding.UTF8;
            string key = null;
            byte[] content = null;

            foreach (var record in result.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                content = u8.GetBytes(record.url);
                Assert.AreEqual(hashTable.TrySet(key, content), 0);
            }

            var result2 = new List<RecordString>();

            foreach (var record in result.EmptyIfNull())
            {
                result2.Add(new RecordString { linkId = record.linkId, clcId = record.clcId, sbp = record.sbp, url = "hahahahahahaha" });
            }

            key = null;
            content = null;

            foreach (var record in result2.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                Assert.AreEqual(hashTable.TryGet(key, out content), 0);
                Assert.AreNotEqual(record.url, u8.GetString(content));
                content = null;
            }

            key = null;
            content = null;

            foreach (var record in result2.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                content = u8.GetBytes(record.url);
                Assert.AreEqual(hashTable.TrySet(key, content), 1);
            }

            key = null;
            content = null;

            foreach (var record in result2.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                Assert.AreEqual(hashTable.TryGet(key, out content), 0);
                Assert.AreEqual(record.url, u8.GetString(content));
                content = null;
            }
        }

        [TestMethod]
        public void LongKeyCASHashTableCheckUpdateBringDeletedBack()
        {
            var hashTable = new CAS.LongKeyCASHashTable(arrayLength, keyByteArrayLength, contentByteArrayLength);
            var result = Utility.GetDataString().Result;

            var u8 = Encoding.UTF8;
            string key = null;
            byte[] content = null;

            foreach (var record in result.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                content = u8.GetBytes(record.url);
                Assert.AreEqual(hashTable.TrySet(key, content), 0);
            }

            key = null;
            content = null;

            foreach (var record in result.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                Assert.AreEqual(hashTable.TryDelete(key), 0);
                Assert.AreEqual(hashTable.TryGet(key, out content), -3);
                Assert.IsNull(content);
                content = null;
            }

            //Update all the records again.
            key = null;
            content = null;

            foreach (var record in result.EmptyIfNull())
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                content = u8.GetBytes(record.url);
                Assert.AreEqual(hashTable.TrySet(key, content), 1);
            }

            //We should get all the URL again.
            key = null;
            content = null;
            foreach (var record in result)
            {
                key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                Assert.AreEqual(hashTable.TryGet(key, out content), 0);
                Assert.AreEqual(record.url, u8.GetString(content));
                content = null;
            }
        }
    }
}