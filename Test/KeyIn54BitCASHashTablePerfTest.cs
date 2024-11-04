using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CAS;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

namespace Test
{
    [TestClass]
    public class KeyIn54BitCASHashTablePerfTest
    {
        //private int arrayLength = 4139;
        private int arrayLength = 10107313;
        private int contentLength = 256;
        private int dataNubmer = 3000000;

        private int perfTestAttempts = 3000000;
        private int perfTestAttemptsForUpdate = 6000000;
        private int perfTestAttemptsForDelete = 15000000;

        private int perfTestAttemptsForGetOneKeyword = 14000000;
        private int perfTestAttemptsForUpdateOneKeyword = 14000000;
        private int perfTestAttemptsForDeleteOneKeyword = 70000000;

        [TestMethod]
        public void KeyIn54BitPerfTestingRandomVerifyResult()
        {
            var stopWatch = new Stopwatch();
            KeyIn54BitCASHashTable hashTable = null;

            hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            var ts = stopWatch.Elapsed;
            Console.WriteLine("Create hash table" + "\t" + ts.ToString());

            stopWatch.Restart();
            var result = Utility.GetData(dataNubmer);
            stopWatch.Stop();
            Console.WriteLine("Get " + result.Count + " data from memory" + "\t" + stopWatch.Elapsed.ToString());

            stopWatch.Restart();
            foreach (var record in result.EmptyIfNull())
            {
                hashTable.TrySet(record.linkId, record.clcId, record.sbp, record.url);
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
                tGet1 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, result); });
                tGet2 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, result); });
                tGet3 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, result); });
                tGet4 = Task.Run(() => { perfGetRandomVerifyResult(mre, hashTable, result); });
            }
            
            if (updateEnabled)
            {
                tUpdate1 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, result); });
                tUpdate2 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, result); });
                tUpdate3 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, result); });
                tUpdate4 = Task.Run(() => { perfUpdateRandomVerifyResult(mre, hashTable, result); });
            }
            
            if (deleteEnabled)
            {
                tDelete1 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, result); });
                tDelete2 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, result); });
                tDelete3 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, result); });
                tDelete4 = Task.Run(() => { perfDeleteRandomVerifyResult(mre, hashTable, result); });
            }

            mre.Set();
            stopWatch.Restart();

            Task.WaitAll(
                tGet1, tGet2, tGet3, tGet4,
                tDelete1, tDelete2, tDelete3, tDelete4,
                tUpdate1, tUpdate2, tUpdate3, tUpdate4
            );

            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            Console.WriteLine("Finish all tast in" + "\t" + ts.ToString());
        }

        private void perfGetRandomVerifyResult(ManualResetEvent mre, CAS.KeyIn54BitCASHashTable hashTable, List<Record> record)
        {
            mre.WaitOne();
            byte[] output = null;

            int seed = Math.Abs(Guid.NewGuid().ToString().GetHashCode());
            var rnd = new Random(seed);
            int totoalRecord = record.Count;

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
                    result = hashTable.TryGet(record[index].linkId, record[index].clcId, record[index].sbp, out output);
                    internalStopWatch.Stop();
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref ExceptionError);
                    Console.WriteLine(ex.Message + "\t" + ex.StackTrace);
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

        private void perfDeleteRandomVerifyResult(ManualResetEvent mre, CAS.KeyIn54BitCASHashTable hashTable, List<Record> record)
        {
            mre.WaitOne();

            int seed = Math.Abs(Guid.NewGuid().ToString().GetHashCode());
            var rnd = new Random(seed);
            int totoalRecord = record.Count;

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
                    result = hashTable.TryDelete(record[index].linkId, record[index].clcId, record[index].sbp);
                    internalStopWatch.Stop();
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref ExceptionError);
                    Console.WriteLine(ex.Message + "\t" + ex.StackTrace);
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

        private void perfUpdateRandomVerifyResult(ManualResetEvent mre, CAS.KeyIn54BitCASHashTable hashTable, List<Record> record)
        {
            mre.WaitOne();

            int seed = Math.Abs(Guid.NewGuid().ToString().GetHashCode());
            var rnd = new Random(seed);
            int totoalRecord = record.Count;

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
                    result = hashTable.TrySet(record[index].linkId, record[index].clcId, record[index].sbp, record[index].url);
                    internalStopWatch.Stop();
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref ExceptionError);
                    Console.WriteLine(ex.Message, ex.StackTrace);
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
            KeyIn54BitCASHashTable hashTable = new KeyIn54BitCASHashTable(arrayLength, contentLength);
            var ts = stopWatch.Elapsed;
            Console.WriteLine("Create hash table" + "\t" + ts.ToString());

            stopWatch.Restart();
            var result = Utility.GetData(dataNubmer);
            stopWatch.Stop();
            Console.WriteLine("Get " + result.Count + " data from memory" + "\t" + stopWatch.Elapsed.ToString());

            stopWatch.Restart();
            foreach (var record in result.EmptyIfNull())
            {
                hashTable.TrySet(record.linkId, record.clcId, record.sbp, record.url);
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
                tGet1 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, result, index); });
                tGet2 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, result, index); });
                tGet3 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, result, index); });
                tGet4 = Task.Run(() => { perfGetOneRecordVerifyResult(mre, hashTable, result, index); });
            }

            if (updateEnabled)
            {
                tUpdate1 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });
                tUpdate2 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });
                tUpdate3 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });
                tUpdate4 = Task.Run(() => { perfUpdateOneRecordVerifyResult(mre, hashTable, result, index); });
            }
                
            if (deleteEnabled)
            {
                tDelete1 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });
                tDelete2 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });
                tDelete3 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });
                tDelete4 = Task.Run(() => { perfDeleteOneRecordVerifyResult(mre, hashTable, result, index); });
            }
                
            mre.Set();
            stopWatch.Restart();
            Task.WaitAll(
                tGet1, tGet2, tGet3, tGet4,
                tDelete1, tDelete2, tDelete3, tDelete4,
                tUpdate1, tUpdate2, tUpdate3, tUpdate4
            );
            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            Console.WriteLine("Finish all tast in" + "\t" + ts.ToString());
        }

        private void perfGetOneRecordVerifyResult(ManualResetEvent mre, CAS.KeyIn54BitCASHashTable hashTable, List<Record> record, int index)
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
                    result = hashTable.TryGet(record[index].linkId, record[index].clcId, record[index].sbp, out output);
                    internalStopWatch.Stop();
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref ExceptionError);
                    Console.WriteLine(ex.Message + "\t" + ex.StackTrace);
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

        private void perfDeleteOneRecordVerifyResult(ManualResetEvent mre, CAS.KeyIn54BitCASHashTable hashTable, List<Record> record, int index)
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
                    result = hashTable.TryDelete(record[index].linkId, record[index].clcId, record[index].sbp);
                    internalStopWatch.Stop();
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref ExceptionError);
                    Console.WriteLine(ex.Message + "\t" + ex.StackTrace);
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

        private void perfUpdateOneRecordVerifyResult(ManualResetEvent mre, CAS.KeyIn54BitCASHashTable hashTable, List<Record> record, int index)
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
                    result = hashTable.TrySet(record[index].linkId, record[index].clcId, record[index].sbp, record[index].url);
                    internalStopWatch.Stop();
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref ExceptionError);
                    Console.WriteLine(ex.Message + "\t" + ex.StackTrace);
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
