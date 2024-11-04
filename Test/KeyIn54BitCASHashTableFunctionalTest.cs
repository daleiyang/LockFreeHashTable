using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CAS;
using System.Threading;
using System.Text;

namespace Test
{
    [TestClass]
    public class KeyIn54BitCASHashTableFunctionalTest
    {
        //private int arrayLength = 4139;
        private int arrayLength = 10107313;
        private int contentLength = 256;
        private int dataNumber = 3000000;

        [TestMethod]
        public void KeyIn54BitInitialize()
        {
           var hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
        }

        [TestMethod]
        public void KeyIn54BitCheckAddTotalSync()
        {
            KeyIn54BitCASHashTable hashTable = null;
            hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);

            var result = Utility.GetData(dataNumber);
            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, record.sbp, record.url), 0);
            }

            var u8 = Encoding.UTF8;
            byte[] output = null;
            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, record.sbp, out output), 0);
                Assert.AreEqual(u8.GetString(record.url), u8.GetString(output));
                output = null;
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckAddEachSync()
        {
            KeyIn54BitCASHashTable hashTable = null;
            hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);

            var result = Utility.GetData(dataNumber);

            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, record.sbp, record.url), 0);
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckAddInvildInput()
        {
            KeyIn54BitCASHashTable hashTable = null;
            hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);

            var temp0 = new byte[hashTable.ContentLength];
            for (int i = 0; i < hashTable.ContentLength; i++)
                temp0[i] = 0;

            var temp = new byte[hashTable.ContentLength + 1];
            for (int i = 0; i < hashTable.ContentLength + 1; i++)
                temp[i] = 0;

            string message = null;
            try { hashTable.TrySet(-1, 1, 1, temp0); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("linkId"));

            message = null;
            try { hashTable.TrySet(4194304, 1, 1, temp0); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("linkId"));

            message = null;
            try { hashTable.TrySet(1, -1, 1, temp0); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("clcId"));

            message = null;
            try { hashTable.TrySet(1, 262144, 1, temp0); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("clcId"));

            message = null;
            try { hashTable.TrySet(1, 1, -1, temp0); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("sbp"));

            message = null;
            try { hashTable.TrySet(1, 1, 16384, temp0); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("sbp"));

            message = null;
            try { hashTable.TrySet(0, 0, 0, temp0); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("KeyIsZero"));

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

        [TestMethod]
        public void KeyIn54BitCheckGet()
        {
            KeyIn54BitCASHashTable hashTable = null;
            hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);

            var result = Utility.GetData(dataNumber);
            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, record.sbp, record.url), 0);
            }

            var u8 = Encoding.UTF8;
            byte[] output = null;
            foreach (var record in result)
            {
                Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, record.sbp, out output), 0);
                Assert.AreEqual(u8.GetString(record.url), u8.GetString(output));
                output = null;
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckGetInvlidInput()
        {
            KeyIn54BitCASHashTable hashTable = null;
            hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);

            byte[] output;
            string message = null;
            try { hashTable.TryGet(-1, 1, 1, out output); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("linkId"));

            message = null;
            try { hashTable.TryGet(4194304, 1, 1, out output); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("linkId"));

            message = null;
            try { hashTable.TryGet(1, -1, 1, out output); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("clcId"));

            message = null;
            try { hashTable.TryGet(1, 262144, 1, out output); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("clcId"));

            message = null;
            try { hashTable.TryGet(1, 1, -1, out output); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("sbp"));

            message = null;
            try { hashTable.TryGet(1, 1, 16384, out output); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("sbp"));

            message = null;
            try { hashTable.TryGet(0, 0, 0, out output); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("KeyIsZero"));
        }

        [TestMethod]
        public void KeyIn54BitCheckGetNotFound()
        {
             var hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
             byte[] output;
             Assert.AreEqual(hashTable.TryGet(2097151, 262143, 16383, out output), -2);
             Assert.IsNull(output);
        }

        [TestMethod]
        public void KeyIn54BitCheckGetDeleted()
        {
            KeyIn54BitCASHashTable hashTable = null;
            hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);

            var result = Utility.GetData(dataNumber);

            int i = 0;

            long linkIdTemp = 0;
            long clcIdTemp = 0;
            long SBPIdTemp = 0;

            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, record.sbp, record.url), 0);

                if (i++ == 1)
                {
                    linkIdTemp = record.linkId;
                    clcIdTemp = record.clcId;
                    SBPIdTemp = record.sbp;
                }
            }

            Assert.AreEqual(hashTable.TryDelete(linkIdTemp, clcIdTemp, SBPIdTemp), 0);
            byte[] output = null;
            Assert.AreEqual(hashTable.TryGet(linkIdTemp, clcIdTemp, SBPIdTemp, out output), -3);
            Assert.IsNull(output);
        }

        [TestMethod]
        public void KeyIn54BitCheckDelete()
        {
            KeyIn54BitCASHashTable hashTable = null;
            hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);

            var result = Utility.GetData(dataNumber);
            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, record.sbp, record.url), 0);
            }

            string message = string.Empty;
            byte[] output;
            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TryDelete(record.linkId, record.clcId, record.sbp), 0);
                output = null;
                Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, record.sbp, out output), -3);
                Assert.IsNull(output);
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckDeleteInvildInput()
        {
            KeyIn54BitCASHashTable hashTable = null;
            hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);

            string message = null;
            try { hashTable.TryDelete(-1, 1, 1); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("linkId"));

            message = null;
            try { hashTable.TryDelete(4194304, 1, 1); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("linkId"));

            message = null;
            try { hashTable.TryDelete(1, -1, 1); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("clcId"));

            message = null;
            try { hashTable.TryDelete(1, 262144, 1); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("clcId"));

            message = null;
            try { hashTable.TryDelete(1, 1, -1); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("sbp"));

            message = null;
            try { hashTable.TryDelete(1, 1, 16384); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("sbp"));

            message = null;
            try { hashTable.TryDelete(0, 0, 0); }
            catch (ArgumentOutOfRangeException e)
            { message = e.Message; }
            Assert.IsTrue(message.Contains("KeyIsZero"));
        }

        [TestMethod]
        public void KeyIn54BitCheckDeleteNotFound()
        {
            var hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            Assert.AreEqual(hashTable.TryDelete(2097151, 262143, 16383), -2);
        }

        [TestMethod]
        public void KeyIn54BitCheckDeleteAgain()
        {
            KeyIn54BitCASHashTable hashTable = null;
            hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);

            var result = Utility.GetData(dataNumber);
            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, record.sbp, record.url), 0);
            }

            byte[] output;
            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TryDelete(record.linkId, record.clcId, record.sbp), 0);
                output = null;
                Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, record.sbp, out output), -3);
                Assert.IsNull(output);
            }

            output = null;
            //Delete all the records again and nothing should happens.
            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TryDelete(record.linkId, record.clcId, record.sbp), -3);
                output = null;
                Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, record.sbp, out output), -3);
                Assert.IsNull(output);
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckUpdate()
        {
            KeyIn54BitCASHashTable hashTable = null;
            hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);

            var result = Utility.GetData(dataNumber);
            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, record.sbp, record.url), 0);
            }

            foreach(var record in result.EmptyIfNull())
            {
                for (var j = 0; j < record.url.Length; j++)
                {
                    record.url[j] = 0xFF;
                }
            }

            var u8 = Encoding.UTF8;
            byte[] output = null;

            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, record.sbp, out output), 0);

                if (record.url.Length == 0)
                {
                    Assert.AreNotEqual("haha", u8.GetString(output));
                }
                else
                {
                    Assert.AreNotEqual(u8.GetString(record.url), u8.GetString(output));
                }
                output = null;
            }

            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, record.sbp, record.url), 1);
            }

            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, record.sbp, out output), 0);
                Assert.AreEqual(u8.GetString(record.url), u8.GetString(output));
                output = null;
            }
        }

        [TestMethod]
        public void KeyIn54BitCheckUpdateBringDeletedBack()
        {
            KeyIn54BitCASHashTable hashTable = null;
            hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);

            var result = Utility.GetData(dataNumber);
            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, record.sbp, record.url), 0);
            }

            byte[] output;
            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TryDelete(record.linkId, record.clcId, record.sbp), 0);
                output = null;
                Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, record.sbp, out output), -3);
                Assert.IsNull(output);
            }
            
            var u8 = Encoding.UTF8;
            //Update all the records again.
            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TrySet(record.linkId, record.clcId, record.sbp, record.url), 1);
            }

            //We should get all the URL again.
            output = null;
            foreach (var record in result.EmptyIfNull())
            {
                Assert.AreEqual(hashTable.TryGet(record.linkId, record.clcId, record.sbp, out output), 0);
                Assert.AreEqual(u8.GetString(record.url), u8.GetString(output));
            }
        }

        [TestMethod]
        public void InterlockTesting()
        {
            //Change the value successfully.
            long target = 0;
            long value = 1;
            long ret = Interlocked.CompareExchange(ref target, value, target);
            Assert.AreEqual(target, value);
            Assert.AreEqual(ret, 0);

            //Failed to change the value.
            target = 0;
            value = 1;
            ret = Interlocked.CompareExchange(ref target, value, target+1);
            Assert.AreEqual(target, 0);
            Assert.AreEqual(ret, 0);
        }
    }
}
