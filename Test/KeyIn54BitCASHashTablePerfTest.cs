using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CAS;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using log4net;

namespace Test
{
    [TestClass]
    public class KeyIn54BitCASHashTablePerfTest
    {
        private ILog log = LogFactory.Configure();

        private int arrayLength = 10177;
        private int contentLength = 256;
        private int perfTestAttempts = 1000;
        private int perfTestAttemptsForUpdate = 6000;
        private int perfTestAttemptsForDelete = 1000;

        //private int arrayLength = 10522403;
        //private int contentLength = 256;
        //private int perfTestAttempts = 200000000;
        //private int perfTestAttemptsForUpdate = 300000000;
        //private int perfTestAttemptsForDelete = 400000000;

        private int perfTestAttemptsForGetOneKeyword = 700000000;
        private int perfTestAttemptsForUpdateOneKeyword = 187000000;
        private int perfTestAttemptsForDeleteOneKeyword = 870000000;

        [TestMethod]
        public void KeyIn54BitPerfTestingRandomVerifyResult()
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
                    hashTable.TrySet(record.linkId, record.clcId, SBPId, record.url);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
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
                tGet1 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (updateEnabled)
                tUpdate1 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (deleteEnabled)
                tDelete1 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, SBPHashTable, result); });


            if (getEnabled)
                tGet2 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (updateEnabled)
                tUpdate2 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (deleteEnabled)
                tDelete2 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, SBPHashTable, result); });


            if (getEnabled)
                tGet3 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (updateEnabled)
                tUpdate3 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (deleteEnabled)
                tDelete3 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, SBPHashTable, result); });


            if (getEnabled)
                tGet4 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (updateEnabled)
                tUpdate4 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (deleteEnabled)
                tDelete4 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, SBPHashTable, result); });


            if (getEnabled)
                tGet5 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (updateEnabled)
                tUpdate5 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (deleteEnabled)
                tDelete5 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, SBPHashTable, result); });


            if (getEnabled)
                tGet6 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (updateEnabled)
                tUpdate6 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (deleteEnabled)
                tDelete6 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, SBPHashTable, result); });


            if (getEnabled)
                tGet7 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (updateEnabled)
                tUpdate7 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (deleteEnabled)
                tDelete7 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, SBPHashTable, result); });


            if (getEnabled)
                tGet8 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (updateEnabled)
                tUpdate8 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (deleteEnabled)
                tDelete8 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, SBPHashTable, result); });


            if (getEnabled)
                tGet9 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (updateEnabled)
                tUpdate9 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (deleteEnabled)
                tDelete9 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, SBPHashTable, result); });


            if (getEnabled)
                tGet10 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (updateEnabled)
                tUpdate10 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, SBPHashTable, result); });

            if (deleteEnabled)
                tDelete10 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, SBPHashTable, result); });


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

        private void perfGetRandomVerifyResult(ManualResetEvent mre, CAS.KeyIn54BitCASHashTable hashTable, ConcurrentDictionary<string, int> SBPHashTable, List<Record> record)
        {
            mre.WaitOne();
            byte[] output = null;

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

            int result = -100;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttempts; i++)
            {
                index = rnd.Next(totoalRecord);

                try
                {
                    internalStopWatch.Start();
                    result = hashTable.TryGet(record[index].linkId, record[index].clcId, SBPHashTable[record[index].sbp], out output);
                    internalStopWatch.Stop();
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref ExceptionError);
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }

                switch (result)
                {
                    case 0:
                        Interlocked.Increment(ref GetSuccessfully);
                        if (String.Compare(u8.GetString(record[index].url), u8.GetString(output)) == 0)
                        {
                            Interlocked.Increment(ref ResultMatch);
                        }
                        else
                        {
                            Interlocked.Increment(ref ResultNotMatch);
                            Console.WriteLine("Target results in string:" + u8.GetString(record[index].url));
                            Console.WriteLine("Miss match result in string:" + u8.GetString(output));
                            Console.WriteLine("Target results in byte:" + BitConverter.ToString(record[index].url));
                            Console.WriteLine("Miss match result in byte:" + BitConverter.ToString(output));
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

        private void perfDeleteRandomVerifyResult(ManualResetEvent mre, CAS.KeyIn54BitCASHashTable hashTable, ConcurrentDictionary<string, int> SBPHashTable, List<Record> record)
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
            int DeleteSuccessfully = 0;
            int Zero = 0;

            int result = -100;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttemptsForDelete; i++)
            {
                index = rnd.Next(totoalRecord);

                try
                {
                    internalStopWatch.Start();
                    result = hashTable.TryDelete(record[index].linkId, record[index].clcId, SBPHashTable[record[index].sbp]);
                    internalStopWatch.Stop();
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref ExceptionError);
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }

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

        private void perfUpdateRandomVerifyResult(ManualResetEvent mre, CAS.KeyIn54BitCASHashTable hashTable, ConcurrentDictionary<string, int> SBPHashTable, List<Record> record)
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

            int result = -100;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttemptsForUpdate; i++)
            {
                index = rnd.Next(totoalRecord);

                try
                {
                    internalStopWatch.Start();
                    result = hashTable.TrySet(record[index].linkId, record[index].clcId, SBPHashTable[record[index].sbp], record[index].url);
                    internalStopWatch.Stop();
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref ExceptionError);
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }

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
        public void KeyIn54BitPerfTestingOneRecordVerifyResult()
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
                    hashTable.TrySet(record.linkId, record.clcId, SBPId, record.url);
                }
                catch (Exception ex)
                {
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
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
                tGet1 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (updateEnabled)
                tUpdate1 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (deleteEnabled)
                tDelete1 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });


            if (getEnabled)
                tGet2 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (updateEnabled)
                tUpdate2 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (deleteEnabled)
                tDelete2 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });


            if (getEnabled)
                tGet3 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (updateEnabled)
                tUpdate3 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (deleteEnabled)
                tDelete3 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });


            if (getEnabled)
                tGet4 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (updateEnabled)
                tUpdate4 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (deleteEnabled)
                tDelete4 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });


            if (getEnabled)
                tGet5 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (updateEnabled)
                tUpdate5 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (deleteEnabled)
                tDelete5 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });


            if (getEnabled)
                tGet6 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (updateEnabled)
                tUpdate6 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (deleteEnabled)
                tDelete6 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });


            if (getEnabled)
                tGet7 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (updateEnabled)
                tUpdate7 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (deleteEnabled)
                tDelete7 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });


            if (getEnabled)
                tGet8 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (updateEnabled)
                tUpdate8 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (deleteEnabled)
                tDelete8 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });


            if (getEnabled)
                tGet9 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (updateEnabled)
                tUpdate9 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (deleteEnabled)
                tDelete9 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });


            if (getEnabled)
                tGet10 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (updateEnabled)
                tUpdate10 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });

            if (deleteEnabled)
                tDelete10 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, SBPHashTable, result, index); });


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

        private void perfGetOneRecordVerifyResult(ManualResetEvent mre, CAS.KeyIn54BitCASHashTable hashTable, ConcurrentDictionary<string, int> SBPHashTable, List<Record> record, int index)
        {
            mre.WaitOne();
            byte[] output = null;
            var u8 = Encoding.UTF8;

            int InputParametersError = 0;
            int NoExists = 0;
            int IsDeleted = 0;
            int ExceptionError = 0;
            int GetSuccessfully = 0;
            int ResultMatch = 0;
            int ResultNotMatch = 0;
            int Zero = 0;

            int result = -100;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttemptsForGetOneKeyword; i++)
            {
                try
                {
                    internalStopWatch.Start();
                    result = hashTable.TryGet(record[index].linkId, record[index].clcId, SBPHashTable[record[index].sbp], out output);
                    internalStopWatch.Stop();
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref ExceptionError);
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }

                switch (result)
                {
                    case 0:
                        Interlocked.Increment(ref GetSuccessfully);
                        if (String.Compare(u8.GetString(record[index].url), u8.GetString(output)) == 0)
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
               "perfGetOneRecordVerifyResult" + "\t" + perfTestAttemptsForGetOneKeyword + "\t" +
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

        private void perfDeleteOneRecordVerifyResult(ManualResetEvent mre, CAS.KeyIn54BitCASHashTable hashTable, ConcurrentDictionary<string, int> SBPHashTable, List<Record> record, int index)
        {
            mre.WaitOne();
            var u8 = Encoding.UTF8;

            int InputParametersError = 0;
            int NoExists = 0;
            int IsDeleted = 0;
            int ExceptionError = 0;
            int DeleteSuccessfully = 0;
            int Zero = 0;

            int result = -100;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttemptsForDeleteOneKeyword; i++)
            {
                try
                {
                    internalStopWatch.Start();
                    result = hashTable.TryDelete(record[index].linkId, record[index].clcId, SBPHashTable[record[index].sbp]);
                    internalStopWatch.Stop();
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref ExceptionError);
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }
           
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
                "perfDeleteOneRecordVerifyResult" + "\t" + perfTestAttemptsForDeleteOneKeyword + "\t" +
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

        private void perfUpdateOneRecordVerifyResult(ManualResetEvent mre, CAS.KeyIn54BitCASHashTable hashTable, ConcurrentDictionary<string, int> SBPHashTable, List<Record> record, int index)
        {
            mre.WaitOne();
            var u8 = Encoding.UTF8;

            int InputParametersError = 0;
            int ExceptionError = 0;
            int AddSuccessfully = 0;
            int UpdateSuccessfully = 0;
            int Zero = 0;

            int result = -100;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttemptsForUpdateOneKeyword; i++)
            {
                try
                {
                    internalStopWatch.Start();
                    result = hashTable.TrySet(record[index].linkId, record[index].clcId, SBPHashTable[record[index].sbp], record[index].url);
                    internalStopWatch.Stop();
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref ExceptionError);
                    if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                }

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
                "perfUpdateOneRecordVerifyResult" + "\t" + perfTestAttemptsForUpdateOneKeyword + "\t" +
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