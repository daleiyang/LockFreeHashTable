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
    public class CASHashTablePerfTest
    {
        private int arrayLength = 4139;
        private int keyByteArrayLength = 256;
        private int contentByteArrayLength = 256;
        private int perfTestAttempts = 2000000;
        private int perfTestAttemptsForUpdate = 300000;
        private int perfTestAttemptsForDelete = 400000;

        //private int arrayLength = 10522403;
        //private int keyByteArrayLength = 256;
        //private int contentByteArrayLength = 256;
        //private int perfTestAttempts = 200000000;
        //private int perfTestAttemptsForUpdate = 300000000;
        //private int perfTestAttemptsForDelete = 400000000;

        [TestMethod]
        public void LongKeyPerfTestingRandomVerifyResult()
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
            byte[] content = null;

            stopWatch.Restart();
            foreach (var record in result.EmptyIfNull())
            {
                var key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                content = u8.GetBytes(record.url);
                Assert.AreEqual(hashTable.TrySet(key, content), 0);
            }
            stopWatch.Stop();
            Console.WriteLine("Add all records" + "\t" + stopWatch.Elapsed.ToString());

            var mre = new ManualResetEvent(false);

            bool getEnabled = true;
            bool deleteEnabled = true;
            bool updateEnabled = true;

            Task tGet1 = null, tGet2 = null, tGet3 = null, tGet4 = null, tGet5 = null, tGet6 = null, tGet7 = null, tGet8 = null, tGet9 = null, tGet10 = null;
            Task tDelete1 = null, tDelete2 = null, tDelete3 = null, tDelete4 = null, tDelete5 = null, tDelete6 = null, tDelete7 = null, tDelete8 = null, tDelete9 = null, tDelete10 = null;
            Task tUpdate1 = null, tUpdate2 = null, tUpdate3 = null, tUpdate4 = null, tUpdate5 = null, tUpdate6 = null, tUpdate7 = null, tUpdate8 = null, tUpdate9 = null, tUpdate10 = null;

            if (getEnabled)
                tGet1 = Task.Run(() => { LongKeyperfGetRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate1 = Task.Run(() => { LongKeyperfUpdateRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete1 = Task.Run(() => { LongKeyperfDeleteRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet2 = Task.Run(() => { LongKeyperfGetRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate2 = Task.Run(() => { LongKeyperfUpdateRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete2 = Task.Run(() => { LongKeyperfDeleteRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet3 = Task.Run(() => { LongKeyperfGetRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate3 = Task.Run(() => { LongKeyperfUpdateRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete3 = Task.Run(() => { LongKeyperfDeleteRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet4 = Task.Run(() => { LongKeyperfGetRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate4 = Task.Run(() => { LongKeyperfUpdateRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete4 = Task.Run(() => { LongKeyperfDeleteRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet5 = Task.Run(() => { LongKeyperfGetRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate5 = Task.Run(() => { LongKeyperfUpdateRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete5 = Task.Run(() => { LongKeyperfDeleteRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet6 = Task.Run(() => { LongKeyperfGetRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate6 = Task.Run(() => { LongKeyperfUpdateRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete6 = Task.Run(() => { LongKeyperfDeleteRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet7 = Task.Run(() => { LongKeyperfGetRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate7 = Task.Run(() => { LongKeyperfUpdateRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete7 = Task.Run(() => { LongKeyperfDeleteRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet8 = Task.Run(() => { LongKeyperfGetRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate8 = Task.Run(() => { LongKeyperfUpdateRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete8 = Task.Run(() => { LongKeyperfDeleteRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet9 = Task.Run(() => { LongKeyperfGetRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate9 = Task.Run(() => { LongKeyperfUpdateRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete9 = Task.Run(() => { LongKeyperfDeleteRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet10 = Task.Run(() => { LongKeyperfGetRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate10 = Task.Run(() => { LongKeyperfUpdateRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete10 = Task.Run(() => { LongKeyperfDeleteRandomVerifyResult(mre, hashTable, result); });


            mre.Set();

            stopWatch.Restart();

            Task.WaitAll(
                tGet1, tGet2, tGet3, tGet4, tGet5, tGet6, tGet7, tGet8, tGet9, tGet10
                , tDelete1, tDelete2, tDelete3, tDelete4, tDelete5, tDelete6, tDelete7, tDelete8, tDelete9, tDelete10
                , tUpdate1, tUpdate2, tUpdate3, tUpdate4, tUpdate5, tUpdate6, tUpdate7, tUpdate8, tUpdate9, tUpdate10
            );

            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            Console.WriteLine("Finish all tast in" + "\t" + ts.ToString());
        }

        private void LongKeyperfGetRandomVerifyResult(ManualResetEvent mre, CAS.LongKeyCASHashTable hashTable, List<RecordString> record)
        {
            mre.WaitOne();

            int totoalRecord = record.Count;
            var rndTemp = new Random();
            int seed = rndTemp.Next(totoalRecord);

            var rnd = new Random(seed);
            int index = 0;

            var u8 = Encoding.UTF8;

            int InputParametersError = 0;
            int NoExists = 0;
            int IsDeleted = 0;
            int ExceptionError = 0;
            int GetSuccessfully = 0;
            int ResultMatch = 0;
            int ResultNotMatch = 0;
            int Zero = 0;

            int result;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            string key = null;
            string url = null;
            byte[] content = null;

            for (var i = 0; i < perfTestAttempts; i++)
            {
                index = rnd.Next(totoalRecord);
                key = record[index].linkId.ToString() + "-" + record[index].clcId.ToString() + "-" + record[index].sbp;
                url = record[index].url;

                internalStopWatch.Start();
                result = hashTable.TryGet(key, out content);
                internalStopWatch.Stop();

                switch (result)
                {
                    case 0:
                        Interlocked.Increment(ref GetSuccessfully);
                        if (String.Compare(url, u8.GetString(content)) == 0)
                        {
                            Interlocked.Increment(ref ResultMatch);
                        }
                        else
                        {
                            Interlocked.Increment(ref ResultNotMatch);
                        }
                        break;
                    case -1:
                        Interlocked.Increment(ref InputParametersError);
                        break;
                    case -2:
                        Interlocked.Increment(ref NoExists);
                        break;
                    case -3:
                        Interlocked.Increment(ref IsDeleted);
                        break;
                    case -4:
                        Interlocked.Increment(ref ExceptionError);
                        break;
                    default:
                        Interlocked.Increment(ref Zero);
                        break;
                };
            }

            var internalTs = internalStopWatch.Elapsed;

            externalStopWatch.Stop();
            var externalTs = externalStopWatch.Elapsed;

            Console.WriteLine(
               "TaskId" + "\t" + Task.CurrentId + "\t" +
               "perfGetRandomVerifyResult" + "\t" + perfTestAttempts + "\t" +
               "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
               "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
               "GetSuccessfully" + "\t" + GetSuccessfully + "\t" +
               "ResultMatch" + "\t" + ResultMatch + "\t" +
               "ResultNotMatch" + "\t" + ResultNotMatch + "\t" +
               "InputParametersError" + "\t" + InputParametersError + "\t" +
               "NoExists" + "\t" + NoExists + "\t" +
               "IsDeleted" + "\t" + IsDeleted + "\t" +
               "ExceptionError" + "\t" + ExceptionError + "\t" +
               "Zero" + "\t" + Zero + "\t"
            );
        }

        private void LongKeyperfDeleteRandomVerifyResult(ManualResetEvent mre, CAS.LongKeyCASHashTable hashTable, List<RecordString> record)
        {
            mre.WaitOne();

            int totoalRecord = record.Count;
            var rndTemp = new Random();
            int seed = rndTemp.Next(totoalRecord);

            var rnd = new Random(seed);
            int index = 0;

            int InputParametersError = 0;
            int NoExists = 0;
            int IsDeleted = 0;
            int ExceptionError = 0;
            int DeleteSuccessfully = 0;
            int Zero = 0;

            int result;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            string key = null;

            for (var i = 0; i < perfTestAttemptsForDelete; i++)
            {
                index = rnd.Next(totoalRecord);
                key = record[index].linkId.ToString() + "-" + record[index].clcId.ToString() + "-" + record[index].sbp;

                internalStopWatch.Start();
                result = hashTable.TryDelete(key);
                internalStopWatch.Stop();

                switch (result)
                {
                    case 0:
                        Interlocked.Increment(ref DeleteSuccessfully);
                        break;
                    case -1:
                        Interlocked.Increment(ref InputParametersError);
                        break;
                    case -2:
                        Interlocked.Increment(ref NoExists);
                        break;
                    case -3:
                        Interlocked.Increment(ref IsDeleted);
                        break;
                    case -4:
                        Interlocked.Increment(ref ExceptionError);
                        break;
                    default:
                        Interlocked.Increment(ref Zero);
                        break;
                };
            }

            var internalTs = internalStopWatch.Elapsed;

            externalStopWatch.Stop();
            var externalTs = externalStopWatch.Elapsed;

            Console.WriteLine(
                "TaskId" + "\t" + Task.CurrentId + "\t" +
                "perfDeleteRandomVerifyResult" + "\t" + perfTestAttemptsForDelete + "\t" +
                "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
                "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
                "DeleteSuccessfully" + "\t" + DeleteSuccessfully + "\t" +
                "InputParametersError" + "\t" + InputParametersError + "\t" +
                "NoExists" + "\t" + NoExists + "\t" +
                "IsDeleted" + "\t" + IsDeleted + "\t" +
                "ExceptionError" + "\t" + ExceptionError + "\t" +
                "Zero" + "\t" + Zero + "\t"
            );
        }

        private void LongKeyperfUpdateRandomVerifyResult(ManualResetEvent mre, CAS.LongKeyCASHashTable hashTable, List<RecordString> record)
        {
            mre.WaitOne();

            int totoalRecord = record.Count;
            var rndTemp = new Random();
            int seed = rndTemp.Next(totoalRecord);

            var rnd = new Random(seed);
            int index = 0;

            var u8 = Encoding.UTF8;

            int InputParametersError = 0;
            int ExceptionError = 0;
            int AddSuccessfully = 0;
            int UpdateSuccessfully = 0;
            int Zero = 0;

            int result;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            string key = null;
            string url = null;

            for (var i = 0; i < perfTestAttemptsForUpdate; i++)
            {
                index = rnd.Next(totoalRecord);
                key = record[index].linkId.ToString() + "-" + record[index].clcId.ToString() + "-" + record[index].sbp;
                url = record[index].url;

                internalStopWatch.Start();
                result = hashTable.TrySet(key, u8.GetBytes(url));
                internalStopWatch.Stop();

                switch (result)
                {
                    case 0:
                        Interlocked.Increment(ref AddSuccessfully);
                        break;
                    case 1:
                        Interlocked.Increment(ref UpdateSuccessfully);
                        break;
                    case -1:
                        Interlocked.Increment(ref InputParametersError);
                        break;
                    case -4:
                        Interlocked.Increment(ref ExceptionError);
                        break;
                    default:
                        Interlocked.Increment(ref Zero);
                        break;
                };
            }

            var internalTs = internalStopWatch.Elapsed;

            externalStopWatch.Stop();
            var externalTs = externalStopWatch.Elapsed;

            Console.WriteLine(
                "TaskId" + "\t" + Task.CurrentId + "\t" +
                "perfUpdateRandomVerifyResult" + "\t" + perfTestAttemptsForUpdate + "\t" +
                "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
                "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
                "AddSuccessfully" + "\t" + AddSuccessfully + "\t" +
                "UpdateSuccessfully" + "\t" + UpdateSuccessfully + "\t" +
                "InputParametersError" + "\t" + InputParametersError + "\t" +
                "ExceptionError" + "\t" + ExceptionError + "\t" +
                "Zero" + "\t" + Zero + "\t"
            );
        }

        [TestMethod]
        public void LongKeyPerfTestingOneRecordVerifyResult()
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
            byte[] content = null;

            stopWatch.Restart();
            foreach (var record in result.EmptyIfNull())
            {
                var key = record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp;
                content = u8.GetBytes(record.url);
                Assert.AreEqual(hashTable.TrySet(key, content), 0);
            }
            stopWatch.Stop();
            Console.WriteLine("Add all records" + "\t" + stopWatch.Elapsed.ToString());

            var mre = new ManualResetEvent(false);

            bool getEnabled = true;
            bool deleteEnabled = false;
            bool updateEnabled = false;

            Task tGet1 = null, tGet2 = null, tGet3 = null, tGet4 = null, tGet5 = null, tGet6 = null, tGet7 = null, tGet8 = null, tGet9 = null, tGet10 = null;
            Task tDelete1 = null, tDelete2 = null, tDelete3 = null, tDelete4 = null, tDelete5 = null, tDelete6 = null, tDelete7 = null, tDelete8 = null, tDelete9 = null, tDelete10 = null;
            Task tUpdate1 = null, tUpdate2 = null, tUpdate3 = null, tUpdate4 = null, tUpdate5 = null, tUpdate6 = null, tUpdate7 = null, tUpdate8 = null, tUpdate9 = null, tUpdate10 = null;

            int totoalRecord = result.Count;
            var rndTemp = new Random();
            int seed = rndTemp.Next(totoalRecord);

            var rnd = new Random(seed);
            var index = rnd.Next(totoalRecord);

            if (getEnabled)
                tGet1 = Task.Run(() => { LongKeyperfGetOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate1 = Task.Run(() => { LongKeyperfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete1 = Task.Run(() => { LongKeyperfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet2 = Task.Run(() => { LongKeyperfGetOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate2 = Task.Run(() => { LongKeyperfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete2 = Task.Run(() => { LongKeyperfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet3 = Task.Run(() => { LongKeyperfGetOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate3 = Task.Run(() => { LongKeyperfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete3 = Task.Run(() => { LongKeyperfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet4 = Task.Run(() => { LongKeyperfGetOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate4 = Task.Run(() => { LongKeyperfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete4 = Task.Run(() => { LongKeyperfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet5 = Task.Run(() => { LongKeyperfGetOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate5 = Task.Run(() => { LongKeyperfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete5 = Task.Run(() => { LongKeyperfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet6 = Task.Run(() => { LongKeyperfGetOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate6 = Task.Run(() => { LongKeyperfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete6 = Task.Run(() => { LongKeyperfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet7 = Task.Run(() => { LongKeyperfGetOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate7 = Task.Run(() => { LongKeyperfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete7 = Task.Run(() => { LongKeyperfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet8 = Task.Run(() => { LongKeyperfGetOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate8 = Task.Run(() => { LongKeyperfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete8 = Task.Run(() => { LongKeyperfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet9 = Task.Run(() => { LongKeyperfGetOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate9 = Task.Run(() => { LongKeyperfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete9 = Task.Run(() => { LongKeyperfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet10 = Task.Run(() => { LongKeyperfGetOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate10 = Task.Run(() => { LongKeyperfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete10 = Task.Run(() => { LongKeyperfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });


            mre.Set();

            stopWatch.Restart();

            Task.WaitAll(
                tGet1, tGet2, tGet3, tGet4, tGet5, tGet6, tGet7, tGet8, tGet9, tGet10
            //, tDelete1, tDelete2, tDelete3, tDelete4, tDelete5, tDelete6, tDelete7, tDelete8, tDelete9, tDelete10
            //, tUpdate1, tUpdate2, tUpdate3, tUpdate4, tUpdate5, tUpdate6, tUpdate7, tUpdate8, tUpdate9, tUpdate10
            );

            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            Console.WriteLine("Finish all tast in" + "\t" + ts.ToString());
        }

        private void LongKeyperfGetOneRecordVerifyResult(ManualResetEvent mre, CAS.LongKeyCASHashTable hashTable, List<RecordString> record, int index)
        {
            mre.WaitOne();
            var u8 = Encoding.UTF8;

            int InputParametersError = 0;
            int NoExists = 0;
            int IsDeleted = 0;
            int ExceptionError = 0;
            int GetSuccessfully = 0;
            int ResultMatch = 0;
            int ResultNotMatch = 0;
            int Zero = 0;

            int result;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            string key = null;
            string url = null;
            byte[] content = null;

            key = record[index].linkId.ToString() + "-" + record[index].clcId.ToString() + "-" + record[index].sbp;
            url = record[index].url;

            for (var i = 0; i < perfTestAttempts; i++)
            {
                content = null;

                internalStopWatch.Start();
                result = hashTable.TryGet(key, out content);
                internalStopWatch.Stop();

                switch (result)
                {
                    case 0:
                        Interlocked.Increment(ref GetSuccessfully);
                        if (String.Compare(url, u8.GetString(content)) == 0)
                        {
                            Interlocked.Increment(ref ResultMatch);
                        }
                        else
                        {
                            Interlocked.Increment(ref ResultNotMatch);
                        }
                        break;
                    case -1:
                        Interlocked.Increment(ref InputParametersError);
                        break;
                    case -2:
                        Interlocked.Increment(ref NoExists);
                        break;
                    case -3:
                        Interlocked.Increment(ref IsDeleted);
                        break;
                    case -4:
                        Interlocked.Increment(ref ExceptionError);
                        break;
                    default:
                        Interlocked.Increment(ref Zero);
                        break;
                };
            }

            var internalTs = internalStopWatch.Elapsed;

            externalStopWatch.Stop();
            var externalTs = externalStopWatch.Elapsed;

            Console.WriteLine(
               "TaskId" + "\t" + Task.CurrentId + "\t" +
               "perfGetOneRecordVerifyResult" + "\t" + perfTestAttempts + "\t" +
               "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
               "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
               "GetSuccessfully" + "\t" + GetSuccessfully + "\t" +
               "ResultMatch" + "\t" + ResultMatch + "\t" +
               "ResultNotMatch" + "\t" + ResultNotMatch + "\t" +
               "InputParametersError" + "\t" + InputParametersError + "\t" +
               "NoExists" + "\t" + NoExists + "\t" +
               "IsDeleted" + "\t" + IsDeleted + "\t" +
               "ExceptionError" + "\t" + ExceptionError + "\t" +
               "Zero" + "\t" + Zero + "\t"
            );
        }

        private void LongKeyperfDeleteOneRecordVerifyResult(ManualResetEvent mre, CAS.LongKeyCASHashTable hashTable, List<RecordString> record, int index)
        {
            mre.WaitOne();
            var u8 = Encoding.UTF8;

            int InputParametersError = 0;
            int NoExists = 0;
            int IsDeleted = 0;
            int ExceptionError = 0;
            int DeleteSuccessfully = 0;
            int Zero = 0;

            int result;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            string key = record[index].linkId.ToString() + "-" + record[index].clcId.ToString() + "-" + record[index].sbp;

            for (var i = 0; i < perfTestAttemptsForDelete; i++)
            {
                internalStopWatch.Start();
                result = hashTable.TryDelete(key);
                internalStopWatch.Stop();

                switch (result)
                {
                    case 0:
                        Interlocked.Increment(ref DeleteSuccessfully);
                        break;
                    case -1:
                        Interlocked.Increment(ref InputParametersError);
                        break;
                    case -2:
                        Interlocked.Increment(ref NoExists);
                        break;
                    case -3:
                        Interlocked.Increment(ref IsDeleted);
                        break;
                    case -4:
                        Interlocked.Increment(ref ExceptionError);
                        break;
                    default:
                        Interlocked.Increment(ref Zero);
                        break;
                };
            }

            var internalTs = internalStopWatch.Elapsed;

            externalStopWatch.Stop();
            var externalTs = externalStopWatch.Elapsed;

            Console.WriteLine(
                "TaskId" + "\t" + Task.CurrentId + "\t" +
                "perfDeleteOneRecordVerifyResult" + "\t" + perfTestAttemptsForDelete + "\t" +
                "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
                "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
                "DeleteSuccessfully" + "\t" + DeleteSuccessfully + "\t" +
                "InputParametersError" + "\t" + InputParametersError + "\t" +
                "NoExists" + "\t" + NoExists + "\t" +
                "IsDeleted" + "\t" + IsDeleted + "\t" +
                "ExceptionError" + "\t" + ExceptionError + "\t" +
                "Zero" + "\t" + Zero + "\t"
            );
        }

        private void LongKeyperfUpdateOneRecordVerifyResult(ManualResetEvent mre, CAS.LongKeyCASHashTable hashTable, List<RecordString> record, int index)
        {
            mre.WaitOne();
            var u8 = Encoding.UTF8;

            int InputParametersError = 0;
            int ExceptionError = 0;
            int AddSuccessfully = 0;
            int UpdateSuccessfully = 0;
            int Zero = 0;

            int result;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            string key = record[index].linkId.ToString() + "-" + record[index].clcId.ToString() + "-" + record[index].sbp;
            string url = record[index].url;

            for (var i = 0; i < perfTestAttemptsForUpdate; i++)
            {
                internalStopWatch.Start();
                result = hashTable.TrySet(key, u8.GetBytes(url));
                internalStopWatch.Stop();

                switch (result)
                {
                    case 0:
                        Interlocked.Increment(ref AddSuccessfully);
                        break;
                    case 1:
                        Interlocked.Increment(ref UpdateSuccessfully);
                        break;
                    case -1:
                        Interlocked.Increment(ref InputParametersError);
                        break;
                    case -4:
                        Interlocked.Increment(ref ExceptionError);
                        break;
                    default:
                        Interlocked.Increment(ref Zero);
                        break;
                };
            }

            var internalTs = internalStopWatch.Elapsed;

            externalStopWatch.Stop();
            var externalTs = externalStopWatch.Elapsed;

            Console.WriteLine(
                "TaskId" + "\t" + Task.CurrentId + "\t" +
                "perfUpdateOneRecordVerifyResult" + "\t" + perfTestAttemptsForUpdate + "\t" +
                "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
                "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
                "AddSuccessfully" + "\t" + AddSuccessfully + "\t" +
                "UpdateSuccessfully" + "\t" + UpdateSuccessfully + "\t" +
                "InputParametersError" + "\t" + InputParametersError + "\t" +
                "ExceptionError" + "\t" + ExceptionError + "\t" +
                "Zero" + "\t" + Zero + "\t"
            );
        }
    }
}