using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CAS;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;
using System.Text;
using log4net;

namespace Test
{
    [TestClass]
    public class KeyIn54BitCASHashTableFunctionalTest
    {
        private ILog log = LogFactory.Configure();

        private int arrayLength = 4139;
        //private int arrayLength = 10107313;
        private int contentLength = 256;

        [TestMethod]
        public void KeyIn54BitInitialize()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                var hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            Console.WriteLine("Create hash table" + "\t" + ts.ToString());
        }

        [TestMethod]
        public void KeyIn54BitCheckAddTotalSync()
        {
            var stopWatch = new Stopwatch();
            KeyIn54BitCASHashTable hashTable = null;
            try
            {
                hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }
            var ts = stopWatch.Elapsed;
            Console.WriteLine("Create hash table" + "\t" + ts.ToString());

            stopWatch.Restart();
            var result = Utility.GetData().Result;
            stopWatch.Stop();
            Console.WriteLine("Get " + result.Count + " data from database" + "\t" + stopWatch.Elapsed.ToString());

            int SBPCounter = 0;
            var SBPHashTable = new ConcurrentDictionary<string, int>();
            int SBPId = 0;

            stopWatch.Restart();
            foreach (var record in result.EmptyIfNull())
            {
                if (!SBPHashTable.ContainsKey(record.sbp))
                {
                    SBPId = Interlocked.Increment(ref SBPCounter);
                    SBPHashTable.TryAdd(record.sbp, SBPId);
                }
                else
                {
                    SBPId = SBPHashTable[record.sbp];
                }
                try
                {
                    Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, SBPId, record.url), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
            }
            stopWatch.Stop();
            Console.WriteLine("Add all records" + "\t" + stopWatch.Elapsed.ToString());

            var u8 = Encoding.UTF8;
            byte[] output = null;

            foreach (var record in result)
            {
                SBPId = SBPHashTable[record.sbp];
                try
                {
                    Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, SBPId, out output), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
                Assert.AreEqual(u8.GetString(record.url), u8.GetString(output));
                output = null;
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckAddEachSync()
        {
            KeyIn54BitCASHashTable hashTable = null;
            try
            {
                hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }

            var result = Utility.GetData().Result;

            int SBPCounter = 0;
            var SBPHashTable = new ConcurrentDictionary<string, int>();
            int SBPId = 0;

            //var stopWatch = new Stopwatch();
            foreach (var record in result.EmptyIfNull())
            {
                if (!SBPHashTable.ContainsKey(record.sbp))
                {
                    SBPId = Interlocked.Increment(ref SBPCounter);
                    SBPHashTable.TryAdd(record.sbp, SBPId);
                }
                else
                {
                    SBPId = SBPHashTable[record.sbp];
                }

                //stopWatch.Restart();
                try
                {
                    Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, SBPId, record.url), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
                //stopWatch.Stop();
                //Console.WriteLine("Add one records" + "\t" + stopWatch.Elapsed.ToString());
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckAddInvildInput()
        {
            KeyIn54BitCASHashTable hashTable = null;
            try
            {
                hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }

            try
            {
                var temp0 = new byte[hashTable.ContentLength];
                for (int i = 0; i < hashTable.ContentLength; i++)
                    temp0[i] = 0;

                var temp = new byte[hashTable.ContentLength + 1];
                for (int i = 0; i < hashTable.ContentLength + 1; i++)
                    temp[i] = 0;

                string message = null;
                try { hashTable.TrySet(0, 1, 1, temp0); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("linkId"));

                message = null;
                try { hashTable.TrySet(4194304, 1, 1, temp0); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("linkId"));

                message = null;
                try { hashTable.TrySet(1, 0, 1, temp0); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("clcId"));

                message = null;
                try { hashTable.TrySet(1, 262144, 1, temp0); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("clcId"));

                message = null;
                try { hashTable.TrySet(1, 1, 0, temp0); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("sbp"));

                message = null;
                try { hashTable.TrySet(1, 1, 16384, temp0); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("sbp"));

                message = null;
                try { hashTable.TrySet(4194303, 262143, 16383, null); }
                catch (ArgumentNullException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("contentLength"));

                message = null;
                try { hashTable.TrySet(4194303, 262143, 16383, temp); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("contentLength"));
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckGet()
        {
            KeyIn54BitCASHashTable hashTable = null;

            try
            {
                hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }

            var result = Utility.GetData().Result;

            int SBPCounter = 0;
            var SBPHashTable = new ConcurrentDictionary<string, int>();
            int SBPId = 0;

            foreach (var record in result.EmptyIfNull())
            {
                if (!SBPHashTable.ContainsKey(record.sbp))
                {
                    SBPId = Interlocked.Increment(ref SBPCounter);
                    SBPHashTable.TryAdd(record.sbp, SBPId);
                }
                else
                {
                    SBPId = SBPHashTable[record.sbp];
                }
                try
                {
                    Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, SBPId, record.url), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
            }

            var u8 = Encoding.UTF8;
            byte[] output = null;
            //var stopWatch = new Stopwatch();
            foreach (var record in result)
            {
                SBPId = SBPHashTable[record.sbp];
                //stopWatch.Restart();
                try
                {
                    Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, SBPId, out output), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
                //stopWatch.Stop();
                //Console.WriteLine("Get one records" + "\t" + stopWatch.Elapsed.ToString());
                Assert.AreEqual(u8.GetString(record.url), u8.GetString(output));
                output = null;
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckGetInvlidInput()
        {
            KeyIn54BitCASHashTable hashTable = null;

            try
            {
                hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }

            byte[] output;

            try
            {
                string message = null;
                try { hashTable.TryGet(0, 1, 1, out output); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("linkId"));

                message = null;
                try { hashTable.TryGet(4194304, 1, 1, out output); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("linkId"));

                message = null;
                try { hashTable.TryGet(1, 0, 1, out output); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("clcId"));

                message = null;
                try { hashTable.TryGet(1, 262144, 1, out output); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("clcId"));

                message = null;
                try { hashTable.TryGet(1, 1, 0, out output); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("sbp"));

                message = null;
                try { hashTable.TryGet(1, 1, 16384, out output); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("sbp"));
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckGetNotFound()
        {
            try
            {
                var hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
                byte[] output;
                Assert.AreEqual(hashTable.TryGet(2097151, 262143, 16383, out output), -2);
                Assert.IsNull(output);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckGetDeleted()
        {
            KeyIn54BitCASHashTable hashTable = null;
            try
            {
                hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }
            var result = Utility.GetData().Result;

            int SBPCounter = 0;
            var SBPHashTable = new ConcurrentDictionary<string, int>();
            int SBPId = 0;
            int i = 0;

            long linkIdTemp = 0;
            long clcIdTemp = 0;
            long SBPIdTemp = 0;

            foreach (var record in result.EmptyIfNull())
            {
                if (!SBPHashTable.ContainsKey(record.sbp))
                {
                    SBPId = Interlocked.Increment(ref SBPCounter);
                    SBPHashTable.TryAdd(record.sbp, SBPId);
                }
                else
                {
                    SBPId = SBPHashTable[record.sbp];
                }

                try
                {
                    Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, SBPId, record.url), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }

                if (i++ == 1)
                {
                    linkIdTemp = record.linkId;
                    clcIdTemp = record.clcId;
                    SBPIdTemp = SBPId;
                }
            }

            try
            {
                Assert.AreEqual(hashTable.TryDelete(linkIdTemp, clcIdTemp, SBPIdTemp), 0);
                byte[] output = null;
                Assert.AreEqual(hashTable.TryGet(linkIdTemp, clcIdTemp, SBPIdTemp, out output), -3);
                Assert.IsNull(output);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckDelete()
        {
            KeyIn54BitCASHashTable hashTable = null;
            try
            {
                hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }
            var result = Utility.GetData().Result;

            int SBPCounter = 0;
            var SBPHashTable = new ConcurrentDictionary<string, int>();
            int SBPId = 0;

            foreach (var record in result.EmptyIfNull())
            {
                if (!SBPHashTable.ContainsKey(record.sbp))
                {
                    SBPId = Interlocked.Increment(ref SBPCounter);
                    SBPHashTable.TryAdd(record.sbp, SBPId);
                }
                else
                {
                    SBPId = SBPHashTable[record.sbp];
                }
                try
                {
                    Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, SBPId, record.url), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
            }

            string message = string.Empty;
            byte[] output;

            foreach (var record in result.EmptyIfNull())
            {
                SBPId = SBPHashTable[record.sbp];
                try
                {
                    Assert.AreEqual(hashTable.TryDelete(record.linkId, record.clcId, SBPId), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }

                output = null;
                try
                {
                    Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, SBPId, out output), -3);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
                Assert.IsNull(output);
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckDeleteInvildInput()
        {
            KeyIn54BitCASHashTable hashTable = null;
            try
            {
                hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }

            try
            {
                string message = null;
                try { hashTable.TryDelete(0, 1, 1); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("linkId"));

                message = null;
                try { hashTable.TryDelete(4194304, 1, 1); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("linkId"));

                message = null;
                try { hashTable.TryDelete(1, 0, 1); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("clcId"));

                message = null;
                try { hashTable.TryDelete(1, 262144, 1); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("clcId"));

                message = null;
                try { hashTable.TryDelete(1, 1, 0); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("sbp"));

                message = null;
                try { hashTable.TryDelete(1, 1, 16384); }
                catch (ArgumentOutOfRangeException e)
                { message = e.Message; }
                Assert.IsTrue(message.Contains("sbp"));
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckDeleteNotFound()
        {
            try
            {
                var hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
                Assert.AreEqual(hashTable.TryDelete(2097151, 262143, 16383), -2);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckDeleteAgain()
        {
            var stopWatch = new Stopwatch();
            KeyIn54BitCASHashTable hashTable = null;
            try
            {
                hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }
            var result = Utility.GetData().Result;

            int SBPCounter = 0;
            var SBPHashTable = new ConcurrentDictionary<string, int>();
            int SBPId = 0;

            foreach (var record in result.EmptyIfNull())
            {
                if (!SBPHashTable.ContainsKey(record.sbp))
                {
                    SBPId = Interlocked.Increment(ref SBPCounter);
                    SBPHashTable.TryAdd(record.sbp, SBPId);
                }
                else
                {
                    SBPId = SBPHashTable[record.sbp];
                }
                try
                {
                    Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, SBPId, record.url), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
            }

            byte[] output;

            foreach (var record in result.EmptyIfNull())
            {
                SBPId = SBPHashTable[record.sbp];
                try
                {
                    Assert.AreEqual(hashTable.TryDelete(record.linkId, record.clcId, SBPId), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
                output = null;

                try
                {
                    Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, SBPId, out output), -3);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
                Assert.IsNull(output);
            }

            output = null;
            //Delete all the records again and nothing should happens.
            foreach (var record in result.EmptyIfNull())
            {
                SBPId = SBPHashTable[record.sbp];
                try
                {
                    Assert.AreEqual(hashTable.TryDelete(record.linkId, record.clcId, SBPId), -3);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }

                output = null;
                try
                {
                    Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, SBPId, out output), -3);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
                Assert.IsNull(output);
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckUpdate()
        {
            KeyIn54BitCASHashTable hashTable = null;
            try
            {
                hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }
            var result = Utility.GetDataArrayList().Result;

            int SBPCounter = 0;
            var SBPHashTable = new ConcurrentDictionary<string, int>();
            int SBPId = 0;

            for (var i = 0; i < result.Count; i++)
            {
                if (!SBPHashTable.ContainsKey(((Record)result[i]).sbp))
                {
                    SBPId = Interlocked.Increment(ref SBPCounter);
                    SBPHashTable.TryAdd(((Record)result[i]).sbp, SBPId);
                }
                else
                {
                    SBPId = SBPHashTable[((Record)result[i]).sbp];
                }
                try
                {
                    Assert.AreEqual(hashTable.TrySet(((Record)result[i]).linkId, ((Record)result[i]).clcId, SBPId, ((Record)result[i]).url), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
            }

            var temp = new byte[hashTable.ContentLength];
            for (int i = 0; i < hashTable.ContentLength; i++)
                temp[i] = 0;

            for (var i = 0; i < result.Count; i++)
            {
                for (var j = 0; j < ((Record)result[i]).url.Length; j++)
                {
                    ((Record)result[i]).url[j] = 0xFF;
                }
            }

            var u8 = Encoding.UTF8;
            byte[] output = null;

            for (var i = 0; i < result.Count; i++)
            {
                SBPId = SBPHashTable[((Record)result[i]).sbp];
                try
                {
                    Assert.AreEqual(hashTable.TryGet(((Record)result[i]).linkId, ((Record)result[i]).clcId, SBPId, out output), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
                if (((Record)result[i]).url.Length == 0)
                {
                    Assert.AreNotEqual("haha", u8.GetString(output));
                }
                else
                {
                    Assert.AreNotEqual(u8.GetString(((Record)result[i]).url), u8.GetString(output));
                }
                output = null;
            }

            for (var i = 0; i < result.Count; i++)
            {
                SBPId = SBPHashTable[((Record)result[i]).sbp];
                try
                {
                    Assert.AreEqual(hashTable.TrySet(((Record)result[i]).linkId, ((Record)result[i]).clcId, SBPId, ((Record)result[i]).url), 1);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
            }

            for (var i = 0; i < result.Count; i++)
            {
                SBPId = SBPHashTable[((Record)result[i]).sbp];
                try
                {
                    Assert.AreEqual(hashTable.TryGet(((Record)result[i]).linkId, ((Record)result[i]).clcId, SBPId, out output), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
                Assert.AreEqual(u8.GetString(((Record)result[i]).url), u8.GetString(output));
                output = null;
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckUpdateBringDeletedBack()
        {
            KeyIn54BitCASHashTable hashTable = null;
            try
            {
                hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
            }

            var result = Utility.GetData().Result;

            int SBPCounter = 0;
            var SBPHashTable = new ConcurrentDictionary<string, int>();
            int SBPId = 0;

            foreach (var record in result.EmptyIfNull())
            {
                if (!SBPHashTable.ContainsKey(record.sbp))
                {
                    SBPId = Interlocked.Increment(ref SBPCounter);
                    SBPHashTable.TryAdd(record.sbp, SBPId);
                }
                else
                {
                    SBPId = SBPHashTable[record.sbp];
                }
                try
                {
                    Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, SBPId, record.url), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
            }

            string message = string.Empty;
            byte[] output;

            foreach (var record in result.EmptyIfNull())
            {
                SBPId = SBPHashTable[record.sbp];
                try
                {
                    Assert.AreEqual(hashTable.TryDelete(record.linkId, record.clcId, SBPId), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
                output = null;
                try
                {
                    Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, SBPId, out output), -3);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
                Assert.IsNull(output);
            }

            var u8 = Encoding.UTF8;
            //Update all the records again.
            foreach (var record in result.EmptyIfNull())
            {
                SBPId = SBPHashTable[record.sbp];
                try
                {
                    Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, SBPId, record.url), 1);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
            }

            //We should get all the URL again.
            output = null;
            foreach (var record in result.EmptyIfNull())
            {
                SBPId = SBPHashTable[record.sbp];
                try
                {
                    Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, SBPId, out output), 0);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
                Assert.AreEqual(u8.GetString(record.url), u8.GetString(output));
            }
        }

        [TestMethod]
        public void InterlockTesting()
        {
            long target = 0;
            long comparand = 0;
            long value = 9223372036854775807;
            long value2 = -9223372036854775808;
            ulong value3 = 18446744073709551615;

            Interlocked.CompareExchange(ref target, value, comparand);
        }
    }
}