using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CAS;
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
        private int perfTestAttempts = 1000;
        private int perfTestAttemptsForUpdate = 1000;
        private int perfTestAttemptsForDelete = 1000;

        //private int perfTestAttempts = 120000000;
        //private int perfTestAttemptsForUpdate = 50000000;
        //private int perfTestAttemptsForDelete = 50000000;

        [TestMethod]
        public void ConcurrentDictionaryPerfTestingConcurrentDictionaryRandomVerifyResult()
        {
            var hashTable = new ConcurrentDictionary<string, byte[]>();

            var stopWatch = new Stopwatch();
            stopWatch.Restart();
            var result = Utility.GetData().Result;
            stopWatch.Stop();
            Console.WriteLine("Get " + result.Count + " data from database" + "\t" + stopWatch.Elapsed.ToString());

            stopWatch.Restart();
            foreach (var record in result.EmptyIfNull())
            {
                hashTable.TryAdd(record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp.ToString(), record.url);
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
                tGet1 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate1 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete1 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet2 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate2 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete2 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet3 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate3 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete3 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet4 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate4 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete4 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet5 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate5 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete5 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet6 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate6 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete6 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet7 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate7 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete7 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet8 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate8 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete8 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet9 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate9 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete9 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });


            if (getEnabled)
                tGet10 = Task.Run(() => { PerfGetConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (updateEnabled)
                tUpdate10 = Task.Run(() => { PerfAddConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });

            if (deleteEnabled)
                tDelete10 = Task.Run(() => { PerfDeleteConcurrentDictionaryRandomVerifyResult(mre, hashTable, result); });


            mre.Set();

            stopWatch.Restart();

            Task.WaitAll(
                tGet1, tGet2, tGet3, tGet4, tGet5, tGet6, tGet7, tGet8, tGet9, tGet10
                , tDelete1, tDelete2, tDelete3, tDelete4, tDelete5, tDelete6, tDelete7, tDelete8, tDelete9, tDelete10
                , tUpdate1, tUpdate2, tUpdate3, tUpdate4, tUpdate5, tUpdate6, tUpdate7, tUpdate8, tUpdate9, tUpdate10
            );

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            Console.WriteLine("Finish all tast in" + "\t" + ts.ToString());
        }

        private void PerfGetConcurrentDictionaryRandomVerifyResult(ManualResetEvent mre, ConcurrentDictionary<string, byte[]> hashTable, List<Record> record)
        {
            mre.WaitOne();
            byte[] output;

            int totoalRecord = record.Count;
            var rndTemp = new Random();
            int seed = rndTemp.Next(totoalRecord);

            var rnd = new Random(seed);
            int index = 0;

            int KeyFound = 0;
            int KeyNotFound = 0;
            int ResultMatch = 0;
            int ResultNotMatch = 0;
            bool result = false;

            var u8 = Encoding.UTF8;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttempts; i++)
            {
                index = rnd.Next(totoalRecord);

                internalStopWatch.Start();
                result = hashTable.TryGetValue(record[index].linkId.ToString() + "-" + record[index].clcId.ToString() + "-" + record[index].sbp.ToString(), out output);
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

        private void PerfDeleteConcurrentDictionaryRandomVerifyResult(ManualResetEvent mre, ConcurrentDictionary<string, byte[]> hashTable, List<Record> record)
        {
            mre.WaitOne();
            byte[] output;

            int totoalRecord = record.Count;
            var rndTemp = new Random();
            int seed = rndTemp.Next(totoalRecord);

            var rnd = new Random(seed);
            int index = 0;

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
                result = hashTable.TryRemove(record[index].linkId.ToString() + "-" + record[index].clcId.ToString() + "-" + record[index].sbp.ToString(), out output);
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

        private void PerfAddConcurrentDictionaryRandomVerifyResult(ManualResetEvent mre, ConcurrentDictionary<string, byte[]> hashTable, List<Record> record)
        {
            mre.WaitOne();

            int totoalRecord = record.Count;
            var rndTemp = new Random();
            int seed = rndTemp.Next(totoalRecord);

            var rnd = new Random(seed);
            int index = 0;

            var u8 = Encoding.UTF8;

            int AddSuccess = 0;
            int AlreayExists = 0;
            bool result = false;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttemptsForUpdate; i++)
            {
                index = rnd.Next(totoalRecord);

                internalStopWatch.Start();
                result = hashTable.TryAdd(record[index].linkId.ToString() + "-" + record[index].clcId.ToString() + "-" + record[index].sbp.ToString(), record[index].url);
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
            var hashTable = new ConcurrentDictionary<string, byte[]>();

            var stopWatch = new Stopwatch();
            stopWatch.Restart();
            var result = Utility.GetData().Result;
            stopWatch.Stop();
            Console.WriteLine("Get " + result.Count + " data from database" + "\t" + stopWatch.Elapsed.ToString());

            stopWatch.Restart();
            foreach (var record in result.EmptyIfNull())
            {
                hashTable.TryAdd(record.linkId.ToString() + "-" + record.clcId.ToString() + "-" + record.sbp.ToString(), record.url);
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
                tGet1 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate1 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete1 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet2 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate2 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete2 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet3 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate3 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete3 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet4 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate4 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete4 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet5 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate5 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete5 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet6 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate6 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete6 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet7 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate7 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete7 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet8 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate8 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete8 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet9 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate9 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete9 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });


            if (getEnabled)
                tGet10 = Task.Run(() => { PerfGetConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (updateEnabled)
                tUpdate10 = Task.Run(() => { PerfAddConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            if (deleteEnabled)
                tDelete10 = Task.Run(() => { PerfDeleteConcurrentDictionaryOneRecordVerifyResult(mre, hashTable, result, index); });

            mre.Set();

            stopWatch.Restart();

            Task.WaitAll(
                tGet1, tGet2, tGet3, tGet4, tGet5, tGet6, tGet7, tGet8, tGet9, tGet10
                //, tDelete1, tDelete2, tDelete3, tDelete4, tDelete5, tDelete6, tDelete7, tDelete8, tDelete9, tDelete10
                //, tUpdate1, tUpdate2, tUpdate3, tUpdate4, tUpdate5, tUpdate6, tUpdate7, tUpdate8, tUpdate9, tUpdate10
            );

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            Console.WriteLine("Finish all tast in" + "\t" + ts.ToString());
        }

        private void PerfGetConcurrentDictionaryOneRecordVerifyResult(ManualResetEvent mre, ConcurrentDictionary<string, byte[]> hashTable, List<Record> record, int index)
        {
            mre.WaitOne();
            byte[] output;

            int KeyFound = 0;
            int KeyNotFound = 0;
            int ResultMatch = 0;
            int ResultNotMatch = 0;
            var u8 = Encoding.UTF8;
            bool result = false;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttempts; i++)
            {
                internalStopWatch.Start();
                result = hashTable.TryGetValue(record[index].linkId.ToString() + "-" + record[index].clcId.ToString() + "-" + record[index].sbp.ToString(), out output);
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
               "PerfGetConcurrentDictionaryOneRecordVerifyResult" + "\t" + perfTestAttempts + "\t" +
               "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
               "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
               "KeyFound" + "\t" + KeyFound + "\t" +
               "KeyNotFound" + "\t" + KeyNotFound + "\t" +
               "ResultMatch" + "\t" + ResultMatch + "\t" +
               "ResultNotMatch" + "\t" + ResultNotMatch + "\t"
            );
        }

        private void PerfDeleteConcurrentDictionaryOneRecordVerifyResult(ManualResetEvent mre, ConcurrentDictionary<string, byte[]> hashTable, List<Record> record, int index)
        {
            mre.WaitOne();
            byte[] output;

            int DeleteSuccessfully = 0;
            int DeleteFailed = 0;
            int ResultMatch = 0;
            int ResultNotMatch = 0;
            var u8 = Encoding.UTF8;
            bool result = false;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttemptsForDelete; i++)
            {
                internalStopWatch.Start();
                result = hashTable.TryRemove(record[index].linkId.ToString() + "-" + record[index].clcId.ToString() + "-" + record[index].sbp.ToString(), out output);
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
                "PerfDeleteConcurrentDictionaryOneRecordVerifyResult" + "\t" + perfTestAttemptsForDelete + "\t" +
                "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
                "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
                "DeleteSuccessfully" + "\t" + DeleteSuccessfully + "\t" +
                "DeleteFailed" + "\t" + DeleteFailed + "\t" +
                "ResultMatch" + "\t" + ResultMatch + "\t" +
                "ResultNotMatch" + "\t" + ResultNotMatch + "\t"
            );
        }

        private void PerfAddConcurrentDictionaryOneRecordVerifyResult(ManualResetEvent mre, ConcurrentDictionary<string, byte[]> hashTable, List<Record> record, int index)
        {
            mre.WaitOne();

            var u8 = Encoding.UTF8;
            int AddSuccess = 0;
            int AlreadyExists = 0;
            bool result = false;

            var externalStopWatch = new Stopwatch();
            externalStopWatch.Start();

            var internalStopWatch = new Stopwatch();

            for (var i = 0; i < perfTestAttemptsForUpdate; i++)
            {
                internalStopWatch.Start();
                result = hashTable.TryAdd(record[index].linkId.ToString() + "-" + record[index].clcId.ToString() + "-" + record[index].sbp.ToString(), record[index].url);
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
                "PerfAddConcurrentDictionaryOneRecordVerifyResult" + "\t" + perfTestAttemptsForUpdate + "\t" +
                 "InternalAPICallTotalTime" + "\t" + internalTs.ToString() + "\t" +
                "TestingTotalTime" + "\t" + externalTs.ToString() + "\t" +
                "AddSuccess" + "\t" + AddSuccess + "\t" +
                "AlreadyExists" + "\t" + AlreadyExists + "\t"
            );
        }
    }
}