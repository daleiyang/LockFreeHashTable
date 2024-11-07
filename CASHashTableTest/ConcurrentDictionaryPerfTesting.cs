using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;

namespace Test
{
    [TestClass]
    public class ConcurrentDictionaryPerfTesting
    {
        private int perfTestAttempts = 4000000;
        private int perfTestAttemptsForUpdate = 12000000;
        private int perfTestAttemptsForDelete = 18000000;
        private int dataNumber = 3000000;

        private int perfTestAttemptsForGetOneKeyword = 14000000;
        private int perfTestAttemptsForUpdateOneKeyword = 20000000;
        private int perfTestAttemptsForDeleteOneKeyword = 25000000;

        private long GenerateKey(long linkId, long clcId, long sbp)
        {
            return (linkId << 42) | (clcId << 24) | (sbp << 10);
        }

        [TestMethod]
        public void ConcurrentDictionaryPerfTestingConcurrentDictionaryRandomVerifyResult()
        {
            var hashTable = new ConcurrentDictionary<long, byte[]>();

            var stopWatch = new Stopwatch();
            stopWatch.Restart();
            var result = Utility.GetData(dataNumber);
            stopWatch.Stop();
            Console.WriteLine("Get " + result.Count + " data from database" + "\t" + stopWatch.Elapsed.ToString());

            stopWatch.Restart();
            foreach (var record in result.EmptyIfNull())
            {
                long key = GenerateKey(record.linkId, record.clcId, record.sbp);
                hashTable.TryAdd(key, record.url);
            }
            stopWatch.Stop();
            Console.WriteLine("Add all records" + "\t" + stopWatch.Elapsed.ToString());

            var mre = new ManualResetEvent(false);

            bool getEnabled = true;
            bool deleteEnabled = true;
            bool updateEnabled = true;

            Task tGet1 = null, tGet2 = null, tGet3 = null, tGet4 = null;
            Task tDelete1 = null, tDelete2 = null, tDelete3 = null, tDelete4 = null;
            Task tUpdate1 = null, tUpdate2 = null, tUpdate3 = null, tUpdate4 = null;

            if (getEnabled)
            {
                tGet1 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });
                tGet2 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });
                tGet3 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });
                tGet4 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });
            }

            if (updateEnabled)
            {
                tUpdate1 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });
                tUpdate2 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });
                tUpdate3 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });
                tUpdate4 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });
            }

            if (deleteEnabled)
            {
                tDelete1 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });
                tDelete2 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });
                tDelete3 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });
                tDelete4 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });
            }

            mre.Set();

            stopWatch.Restart();

            Task.WaitAll(
                tGet1, tGet2, tGet3, tGet4,
                tDelete1, tDelete2, tDelete3, tDelete4,
                tUpdate1, tUpdate2, tUpdate3, tUpdate4
            );

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            Console.WriteLine("Finish all tast in" + "\t" + ts.ToString());
        }

        private void PerfGetConcurrentDictionaryRandomVerifyResult(ManualResetEvent mre, ConcurrentDictionary<long, byte[]> hashTable, List<Record> record)
        {
            mre.WaitOne();
            byte[] output;

            int seed = Math.Abs(Guid.NewGuid().ToString().GetHashCode());
            var rnd = new Random(seed);
            int totoalRecord = record.Count;
            int index = 0;

            int KeyFound = 0;
            int KeyNotFound = 0;
            int ResultMatch = 0;
            int ResultNotMatch = 0;
            bool result = false;
            long key = 0;

            var u8 = Encoding.UTF8;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttempts; i++)
            {
                index = rnd.Next(totoalRecord);

                internalStopWatch.Start();
                key = GenerateKey(record[index].linkId, record[index].clcId, record[index].sbp);
                result = hashTable.TryGetValue(key, out output);
                internalStopWatch.Stop();

                if (result)
                {
                    Interlocked.Increment(ref KeyFound);
                    if (String.Compare(u8.GetString(record[index].url), u8.GetString(output)) == 0)
                    {
                        Interlocked.Increment(ref ResultMatch);
                    }
                    else
                    {
                        Interlocked.Increment(ref ResultNotMatch);
                        Console.WriteLine("linkId:" + record[index].linkId.ToString());
                        Console.WriteLine("clcId:" + record[index].clcId.ToString());
                        Console.WriteLine("sbp:" + record[index].sbp.ToString());
                        Console.WriteLine("Target results in string:" + u8.GetString(record[index].url));
                        Console.WriteLine("Miss match result in string:" + u8.GetString(output));
                        Console.WriteLine("Target results in byte:" + BitConverter.ToString(record[index].url));
                        Console.WriteLine("Miss match result in byte:" + BitConverter.ToString(output));
                    }
                }
                else
                {
                    Interlocked.Increment(ref KeyNotFound);
                };
            }

            var internalTs = internalStopWatch.Elapsed;

            externalStopWatch.Stop();
            var externalTs = externalStopWatch.Elapsed;

            Console.WriteLine(
               "TaskId" + "\t" + Task.CurrentId + "\t" +
               "PerfGetConcurrentDictionaryRandomVerifyResult" + "\t" + perfTestAttempts + "\t" +
               "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
               "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
               "KeyFound" + "\t" + KeyFound + "\t" +
               "KeyNotFound" + "\t" + KeyNotFound + "\t" +
               "ResultMatch" + "\t" + ResultMatch + "\t" +
               "ResultNotMatch" + "\t" + ResultNotMatch + "\t"
            );
        }

        private void PerfDeleteConcurrentDictionaryRandomVerifyResult(ManualResetEvent mre, ConcurrentDictionary<long, byte[]> hashTable, List<Record> record)
        {
            mre.WaitOne();
            byte[] output;

            int seed = Math.Abs(Guid.NewGuid().ToString().GetHashCode());
            var rnd = new Random(seed);
            int totoalRecord = record.Count;
            int index = 0;
            long key = 0;

            var u8 = Encoding.UTF8;

            int DeleteSuccessfully = 0;
            int DeleteFailed = 0;
            int ResultMatch = 0;
            int ResultNotMatch = 0;
            bool result = false;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttemptsForDelete; i++)
            {
                index = rnd.Next(totoalRecord);

                internalStopWatch.Start();
                key = GenerateKey(record[index].linkId, record[index].clcId, record[index].sbp);
                result = hashTable.TryRemove(key, out output);
                internalStopWatch.Stop();

                if (result)
                {
                    Interlocked.Increment(ref DeleteSuccessfully);
                    if (String.Compare(u8.GetString(record[index].url), u8.GetString(output)) == 0)
                    {
                        Interlocked.Increment(ref ResultMatch);
                    }
                    else
                    {
                        Interlocked.Increment(ref ResultNotMatch);
                        Console.WriteLine("linkId:" + record[index].linkId.ToString());
                        Console.WriteLine("clcId:" + record[index].clcId.ToString());
                        Console.WriteLine("sbp:" + record[index].sbp.ToString());
                        Console.WriteLine("Target results in string:" + u8.GetString(record[index].url));
                        Console.WriteLine("Miss match result in string:" + u8.GetString(output));
                        Console.WriteLine("Target results in byte:" + BitConverter.ToString(record[index].url));
                        Console.WriteLine("Miss match result in byte:" + BitConverter.ToString(output));
                    }
                }
                else
                {
                    Interlocked.Increment(ref DeleteFailed);
                };
            }

            var internalTs = internalStopWatch.Elapsed;

            externalStopWatch.Stop();
            var externalTs = externalStopWatch.Elapsed;

            Console.WriteLine(
                "TaskId" + "\t" + Task.CurrentId + "\t" +
                "PerfDeleteConcurrentDictionaryRandomVerifyResult" + "\t" + perfTestAttemptsForDelete + "\t" +
                "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
                "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
                "DeleteSuccessfully" + "\t" + DeleteSuccessfully + "\t" +
                "DeleteFailed" + "\t" + DeleteFailed + "\t" +
                "ResultMatch" + "\t" + ResultMatch + "\t" +
                "ResultNotMatch" + "\t" + ResultNotMatch + "\t"
            );
        }

        private void PerfAddConcurrentDictionaryRandomVerifyResult(ManualResetEvent mre, ConcurrentDictionary<long, byte[]> hashTable, List<Record> record)
        {
            mre.WaitOne();

            int seed = Math.Abs(Guid.NewGuid().ToString().GetHashCode());
            var rnd = new Random(seed);
            int totoalRecord = record.Count;
            int index = 0;

            var u8 = Encoding.UTF8;

            int AddSuccess = 0;
            int AlreayExists = 0;
            bool result = false;
            long key = 0;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttemptsForUpdate; i++)
            {
                index = rnd.Next(totoalRecord);

                internalStopWatch.Start();
                key = GenerateKey(record[index].linkId, record[index].clcId, record[index].sbp);
                result = hashTable.TryAdd(key, record[index].url);
                internalStopWatch.Stop();

                if (result)
                {
                    Interlocked.Increment(ref AddSuccess);
                }
                else
                {
                    Interlocked.Increment(ref AlreayExists);
                };
            }

            var internalTs = internalStopWatch.Elapsed;

            externalStopWatch.Stop();
            var externalTs = externalStopWatch.Elapsed;

            Console.WriteLine(
                "TaskId" + "\t" + Task.CurrentId + "\t" +
                "PerfAddConcurrentDictionaryRandomVerifyResult" + "\t" + perfTestAttemptsForUpdate + "\t" +
                "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
                "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
                "AddSuccess" + "\t" + AddSuccess + "\t" +
                "AlreayExists" + "\t" + AlreayExists + "\t"
            );
        }

        [TestMethod]
        public void ConcurrentDictionaryPerfTestingOneRecordVerifyResult()
        {
            var hashTable = new ConcurrentDictionary<long, byte[]>();

            var stopWatch = new Stopwatch();
            stopWatch.Restart();
            var result = Utility.GetData(dataNumber);
            stopWatch.Stop();
            Console.WriteLine("Get " + result.Count + " data from database" + "\t" + stopWatch.Elapsed.ToString());

            stopWatch.Restart();
            foreach (var record in result.EmptyIfNull())
            {
                long key = GenerateKey(record.linkId, record.clcId, record.sbp);
                hashTable.TryAdd(key, record.url);
            }
            stopWatch.Stop();
            Console.WriteLine("Add all records" + "\t" + stopWatch.Elapsed.ToString());

            var mre = new ManualResetEvent(false);

            bool getEnabled = true;
            bool deleteEnabled = true;
            bool updateEnabled = true;

            Task tGet1 = null, tGet2 = null, tGet3 = null, tGet4 = null;
            Task tDelete1 = null, tDelete2 = null, tDelete3 = null, tDelete4 = null;
            Task tUpdate1 = null, tUpdate2 = null, tUpdate3 = null, tUpdate4 = null;

            int seed = Math.Abs(Guid.NewGuid().ToString().GetHashCode());
            var rnd = new Random(seed);
            var index = rnd.Next(result.Count);

            if (getEnabled)
            {
                tGet1 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });
                tGet2 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });
                tGet3 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });
                tGet4 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });
            }

            if (updateEnabled)
            {
                tUpdate1 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });
                tUpdate2 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });
                tUpdate3 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });
                tUpdate4 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });
            }

            if (deleteEnabled)
            {
                tDelete1 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });
                tDelete2 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });
                tDelete3 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });
                tDelete4 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });
            }

            mre.Set();

            stopWatch.Restart();

            Task.WaitAll(
                tGet1, tGet2, tGet3, tGet4,
                tDelete1, tDelete2, tDelete3, tDelete4,
                tUpdate1, tUpdate2, tUpdate3, tUpdate4
            );

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            Console.WriteLine("Finish all tast in" + "\t" + ts.ToString());
        }

        private void PerfGetConcurrentDictionaryOneRecordVerifyResult(ManualResetEvent mre, ConcurrentDictionary<long, byte[]> hashTable, List<Record> record, int index)
        {
            mre.WaitOne();
            byte[] output;

            int KeyFound = 0;
            int KeyNotFound = 0;
            int ResultMatch = 0;
            int ResultNotMatch = 0;
            var u8 = Encoding.UTF8;
            bool result = false;
            long key = 0;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttemptsForGetOneKeyword; i++)
            {
                output = null;

                internalStopWatch.Start();
                key = GenerateKey(record[index].linkId, record[index].clcId, record[index].sbp);
                result = hashTable.TryGetValue(key, out output);
                internalStopWatch.Stop();

                if (result)
                {
                    Interlocked.Increment(ref KeyFound);
                    if (String.Compare(u8.GetString(record[index].url), u8.GetString(output)) == 0)
                    {
                        Interlocked.Increment(ref ResultMatch);
                    }
                    else
                    {
                        Interlocked.Increment(ref ResultNotMatch);
                    }
                }
                else
                {
                    Interlocked.Increment(ref KeyNotFound);
                }
            }

            var internalTs = internalStopWatch.Elapsed;

            externalStopWatch.Stop();
            var externalTs = externalStopWatch.Elapsed;

            Console.WriteLine(
               "TaskId" + "\t" + Task.CurrentId + "\t" +
               "PerfGetConcurrentDictionaryOneRecordVerifyResult" + "\t" + perfTestAttemptsForGetOneKeyword + "\t" +
               "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
               "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
               "KeyFound" + "\t" + KeyFound + "\t" +
               "KeyNotFound" + "\t" + KeyNotFound + "\t" +
               "ResultMatch" + "\t" + ResultMatch + "\t" +
               "ResultNotMatch" + "\t" + ResultNotMatch + "\t"
            );
        }

        private void PerfDeleteConcurrentDictionaryOneRecordVerifyResult(ManualResetEvent mre, ConcurrentDictionary<long, byte[]> hashTable, List<Record> record, int index)
        {
            mre.WaitOne();
            byte[] output;

            int DeleteSuccessfully = 0;
            int DeleteFailed = 0;
            int ResultMatch = 0;
            int ResultNotMatch = 0;
            var u8 = Encoding.UTF8;
            bool result = false;
            long key = 0;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttemptsForDeleteOneKeyword; i++)
            {
                internalStopWatch.Start();
                key = GenerateKey(record[index].linkId, record[index].clcId, record[index].sbp);
                result = hashTable.TryRemove(key, out output);
                internalStopWatch.Stop();

                if (result)
                {
                    Interlocked.Increment(ref DeleteSuccessfully);
                    if (String.Compare(u8.GetString(record[index].url), u8.GetString(output)) == 0)
                    {
                        Interlocked.Increment(ref ResultMatch);
                    }
                    else
                    {
                        Interlocked.Increment(ref ResultNotMatch);
                    }
                }
                else
                {
                    Interlocked.Increment(ref DeleteFailed);
                }
            }

            var internalTs = internalStopWatch.Elapsed;

            externalStopWatch.Stop();
            var externalTs = externalStopWatch.Elapsed;

            Console.WriteLine(
                "TaskId" + "\t" + Task.CurrentId + "\t" +
                "PerfDeleteConcurrentDictionaryOneRecordVerifyResult" + "\t" + perfTestAttemptsForDeleteOneKeyword + "\t" +
                "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
                "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
                "DeleteSuccessfully" + "\t" + DeleteSuccessfully + "\t" +
                "DeleteFailed" + "\t" + DeleteFailed + "\t" +
                "ResultMatch" + "\t" + ResultMatch + "\t" +
                "ResultNotMatch" + "\t" + ResultNotMatch + "\t"
            );
        }

        private void PerfAddConcurrentDictionaryOneRecordVerifyResult(ManualResetEvent mre, ConcurrentDictionary<long, byte[]> hashTable, List<Record> record, int index)
        {
            mre.WaitOne();

            var u8 = Encoding.UTF8;
            int AddSuccess = 0;
            int AlreadyExists = 0;
            bool result = false;
            long key = 0;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttemptsForUpdateOneKeyword; i++)
            {
                internalStopWatch.Start();
                key = GenerateKey(record[index].linkId, record[index].clcId, record[index].sbp);
                result = hashTable.TryAdd(key, record[index].url);
                internalStopWatch.Stop();

                if (result)
                {
                    Interlocked.Increment(ref AddSuccess);
                }
                else
                {
                    Interlocked.Increment(ref AlreadyExists);
                }
            }

            var internalTs = internalStopWatch.Elapsed;

            externalStopWatch.Stop();
            var externalTs = externalStopWatch.Elapsed;

            Console.WriteLine(
                "TaskId" + "\t" + Task.CurrentId + "\t" +
                "PerfAddConcurrentDictionaryOneRecordVerifyResult" + "\t" + perfTestAttemptsForUpdateOneKeyword + "\t" +
                 "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
                "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
                "AddSuccess" + "\t" + AddSuccess + "\t" +
                "AlreadyExists" + "\t" + AlreadyExists + "\t"
            );
        }
    }
}