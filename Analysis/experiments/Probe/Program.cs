using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using CAS;

// Probe experiments for KeyIn54BitCASHashTable. Usage: Probe <experiment>|all
static class P
{
    static void Log(string s) { Console.WriteLine(s); Console.Out.Flush(); }

    // Runs action on a background thread; returns true if it finished within timeoutMs.
    static bool RunWithTimeout(Action a, int timeoutMs, out Exception err)
    {
        Exception e0 = null;
        var t = new Thread(() => { try { a(); } catch (Exception e) { e0 = e; } }) { IsBackground = true };
        t.Start();
        bool done = t.Join(timeoutMs);
        err = e0;
        return done;
    }

    static byte[] Bytes(string s) => Encoding.UTF8.GetBytes(s);

    static List<(long l, long c, long s, byte[] url)> GetData(int cnt)
    {
        // identical to test Utility.GetData
        var r = new List<(long, long, long, byte[])>(cnt);
        int linkId = 1, clcId = 1, sbp = 1;
        for (int i = 0; i < cnt; i++)
        {
            r.Add((linkId % 4194303, clcId % 262143, sbp % 16383, Bytes("http://www.microsoft.com/abc.asp+" + i)));
            linkId++; clcId++; sbp++;
        }
        return r;
    }

    static void E1_MinValueKey()
    {
        Log("## E1 linkId=2^21, clcId=0, sbp=0 -> key == long.MinValue");
        var t = new KeyIn54BitCASHashTable(101, 64);
        Log($"GenerateKey = {t.GenerateKey(2097152, 0, 0)} (long.MinValue={long.MinValue})");
        try { var r = t.TrySet(2097152, 0, 0, Bytes("x")); Log("TrySet returned " + r); }
        catch (Exception e) { Log("TrySet threw " + e.GetType().Name + ": " + e.Message); }
        try { var r = t.TryGet(2097152, 0, 0, out _); Log("TryGet returned " + r); }
        catch (Exception e) { Log("TryGet threw " + e.GetType().Name + ": " + e.Message); }
    }

    static void E2_FullTable()
    {
        Log("## E2 table full (arrayLength=7, 7 keys inserted) then query/insert a missing key");
        var t = new KeyIn54BitCASHashTable(7, 16);
        for (int i = 1; i <= 7; i++) t.TrySet(i, 0, 0, Bytes("v" + i));
        bool ok = RunWithTimeout(() => t.TryGet(8, 0, 0, out _), 3000, out _);
        Log("TryGet(missing) finished within 3s: " + ok + (ok ? "" : "  => INFINITE LOOP (HashSearch never meets an empty slot)"));
        ok = RunWithTimeout(() => t.TryDelete(8, 0, 0), 3000, out _);
        Log("TryDelete(missing) finished within 3s: " + ok);
        ok = RunWithTimeout(() => t.TrySet(8, 0, 0, Bytes("v8")), 3000, out _);
        Log("TrySet(new key) finished within 3s: " + ok + (ok ? "" : "  => INFINITE LOOP (AddOrUpdate probes forever)"));
    }

    static void E3_Tombstones()
    {
        Log("## E3 tombstones are never reclaimed (arrayLength=11): insert+delete 11 distinct keys, live count = 0");
        var t = new KeyIn54BitCASHashTable(11, 16);
        for (int i = 1; i <= 11; i++) { t.TrySet(i, 1, 1, Bytes("v")); t.TryDelete(i, 1, 1); }
        bool ok = RunWithTimeout(() => t.TrySet(12, 1, 1, Bytes("v12")), 3000, out _);
        Log("TrySet(12th distinct key) finished within 3s: " + ok + (ok ? "" : "  => table 'full' of deleted slots, INFINITE LOOP"));
    }

    static void E4_LeakedWriteBit()
    {
        Log("## E4 exception inside write critical section leaves doWrite bit set forever");
        var t = new KeyIn54BitCASHashTable(101, 16);
        KeyIn54BitCASHashTableBase b = t; // public base API has no validation
        long key = t.GenerateKey(1, 1, 1);
        try { b.TrySet(key, new byte[17]); Log("base.TrySet returned normally"); }
        catch (Exception e) { Log("base.TrySet(content 17 bytes > 16) threw " + e.GetType().Name); }
        bool ok = RunWithTimeout(() => t.TryGet(1, 1, 1, out _), 3000, out _);
        Log("TryGet same key finished within 3s: " + ok + (ok ? "" : "  => spins forever (slot permanently write-locked)"));
        ok = RunWithTimeout(() => t.TrySet(1, 1, 1, Bytes("ok")), 3000, out _);
        Log("TrySet same key (valid content) finished within 3s: " + ok);

        Log("-- same thing on the Update path");
        var t2 = new KeyIn54BitCASHashTable(101, 16);
        t2.TrySet(2, 2, 2, Bytes("abc"));
        try { ((KeyIn54BitCASHashTableBase)t2).TrySet(t2.GenerateKey(2, 2, 2), new byte[17]); }
        catch (Exception e) { Log("base.TrySet (update) threw " + e.GetType().Name); }
        ok = RunWithTimeout(() => t2.TryGet(2, 2, 2, out _), 3000, out _);
        Log("TryGet after failed update finished within 3s: " + ok);

        Log("-- key 0 via public base API");
        var t3 = new KeyIn54BitCASHashTable(101, 16);
        KeyIn54BitCASHashTableBase b3 = t3;
        Log("base.TrySet(0, ...) returned " + b3.TrySet(0, Bytes("zero")) + " (0 = 'added')");
        Log("base.TryGet(0, ...) returned " + b3.TryGet(0, out _) + " (-2 = 'not found') => silent data loss");
    }

    static void E5_PowerOfTwo()
    {
        Log("## E5 arrayLength = power of two vs prime (keys always have low 10 bits = 0)");
        var data = GetData(20000);
        foreach (int len in new[] { 65536, 65537 })
        {
            var t = new KeyIn54BitCASHashTable(len, 64);
            var homes = new HashSet<int>();
            foreach (var d in data) homes.Add(t.Hash(t.GenerateKey(d.l, d.c, d.s)));
            var sw = Stopwatch.StartNew();
            foreach (var d in data) t.TrySet(d.l, d.c, d.s, d.url);
            sw.Stop();
            Log($"len={len}: distinct home slots={homes.Count} for {data.Count} keys, insert 20k keys took {sw.ElapsedMilliseconds} ms");
        }
    }

    static void E6_ProbeStats()
    {
        Log("## E6 probe length (linear probing) with the test's config: arrayLength=10107313, 3M keys");
        const int len = 10107313;
        var data = GetData(3_000_000);
        void Sim(string name, IEnumerable<long> keys)
        {
            var occ = new bool[len];
            long total = 0; int max = 0; int n = 0;
            var hist = new int[8];
            foreach (var k in keys)
            {
                int idx = (int)(Math.Abs(k) % len); int p = 1;
                while (occ[idx]) { idx = (idx + 1) % len; p++; }
                occ[idx] = true; total += p; n++; if (p > max) max = p;
                hist[Math.Min(p, 7)]++;
            }
            Log($"{name}: avg probes per successful lookup={(double)total / n:F3}, max={max}, histogram(1..7+)={string.Join(",", hist.Skip(1))}");
        }
        Sim("test data (linkId=clcId=sbp sequential)", data.Select(d => (d.l << 42) | (d.c << 24) | (d.s << 10)));
        var rnd = new Random(42);
        var set = new HashSet<long>();
        while (set.Count < 3_000_000)
            set.Add(((long)rnd.Next(1, 4194304) << 42) | ((long)rnd.Next(262144) << 24) | ((long)rnd.Next(16384) << 10));
        Sim("uniform random keys", set);
    }

    static void E7_TornReadStress()
    {
        Log("## E7 torn-read / consistency stress (versioned payload, 4 readers + 2 writers + 2 deleters, 16 hot keys, 5 s)");
        var t = new KeyIn54BitCASHashTable(1009, 64);
        for (int k = 1; k <= 16; k++) t.TrySet(k, 1, 1, Payload(0, 64));
        long reads = 0, torn = 0, writes = 0, deletes = 0, deletedSeen = 0;
        var stop = false;
        var threads = new List<Thread>();
        for (int r = 0; r < 4; r++) threads.Add(new Thread(() =>
        {
            var rnd = new Random(Environment.CurrentManagedThreadId);
            long lr = 0, lt = 0, ld = 0;
            while (!Volatile.Read(ref stop))
            {
                int res = t.TryGet(rnd.Next(1, 17), 1, 1, out var o);
                if (res == 0) { lr++; if (!Check(o)) lt++; } else ld++;
            }
            Interlocked.Add(ref reads, lr); Interlocked.Add(ref torn, lt); Interlocked.Add(ref deletedSeen, ld);
        }));
        for (int w = 0; w < 2; w++) threads.Add(new Thread(() =>
        {
            var rnd = new Random(Environment.CurrentManagedThreadId); long lw = 0;
            while (!Volatile.Read(ref stop))
            {
                long v = rnd.NextInt64();
                t.TrySet(rnd.Next(1, 17), 1, 1, Payload(v, 8 * rnd.Next(1, 9))); lw++;
            }
            Interlocked.Add(ref writes, lw);
        }));
        for (int d = 0; d < 2; d++) threads.Add(new Thread(() =>
        {
            var rnd = new Random(Environment.CurrentManagedThreadId); long ld = 0;
            while (!Volatile.Read(ref stop)) { t.TryDelete(rnd.Next(1, 17), 1, 1); ld++; }
            Interlocked.Add(ref deletes, ld);
        }));
        threads.ForEach(x => x.Start());
        Thread.Sleep(5000); Volatile.Write(ref stop, true);
        threads.ForEach(x => x.Join());
        Log($"successful reads={reads:N0}, torn/inconsistent reads={torn}, reads of deleted={deletedSeen:N0}, writes={writes:N0}, deletes={deletes:N0}");

        static byte[] Payload(long v, int len)
        {
            var b = new byte[len];
            for (int i = 0; i < len; i += 8) BitConverter.TryWriteBytes(b.AsSpan(i), v);
            return b;
        }
        static bool Check(byte[] o)
        {
            if (o.Length == 0 || o.Length % 8 != 0) return false;
            long v = BitConverter.ToInt64(o, 0);
            for (int i = 8; i < o.Length; i += 8) if (BitConverter.ToInt64(o, i) != v) return false;
            return true;
        }
    }

    static void E8_WriterStarvation()
    {
        Log("## E8 writer latency on a hot key vs number of concurrent readers (content 256B)");
        foreach (int readers in new[] { 0, 1, 3, 7 })
        {
            var t = new KeyIn54BitCASHashTable(101, 256);
            var content = new byte[256];
            t.TrySet(1, 1, 1, content);
            bool stop = false;
            var rs = Enumerable.Range(0, readers).Select(ix => new Thread(() => { while (!Volatile.Read(ref stop)) t.TryGet(1, 1, 1, out _); })).ToList();
            rs.ForEach(x => x.Start());
            Thread.Sleep(200);
            const int N = 20000;
            var lat = new double[N];
            var sw = new Stopwatch();
            var total = Stopwatch.StartNew();
            for (int i = 0; i < N; i++)
            {
                sw.Restart(); t.TrySet(1, 1, 1, content); sw.Stop();
                lat[i] = sw.Elapsed.TotalMicroseconds;
                if (total.ElapsedMilliseconds > 20000) { Array.Resize(ref lat, i + 1); break; }
            }
            Volatile.Write(ref stop, true); rs.ForEach(x => x.Join());
            Array.Sort(lat);
            Log($"readers={readers}: writes={lat.Length}, p50={lat[lat.Length / 2]:F2}us p99={lat[(int)(lat.Length * 0.99)]:F2}us p99.9={lat[(int)(lat.Length * 0.999)]:F2}us max={lat[^1]:F0}us");
        }
    }

    static void E9_ReaderOverflow()
    {
        Log("## E9 8-bit reader counter: 400 threads reading ONE key for 5 s (oversubscribed, 8 logical cores)");
        var t = new KeyIn54BitCASHashTable(101, 256);
        t.TrySet(1, 1, 1, new byte[256]);
        long overflow = 0, ok = 0; bool stop = false;
        var ths = Enumerable.Range(0, 400).Select(ix => new Thread(() =>
        {
            long lo = 0, lk = 0;
            while (!Volatile.Read(ref stop))
            {
                try { t.TryGet(1, 1, 1, out _); lk++; }
                catch (ReaderCounterOverflowException) { lo++; }
            }
            Interlocked.Add(ref overflow, lo); Interlocked.Add(ref ok, lk);
        })).ToList();
        ths.ForEach(x => x.Start());
        Thread.Sleep(5000); Volatile.Write(ref stop, true); ths.ForEach(x => x.Join());
        Log($"successful gets={ok:N0}, ReaderCounterOverflowException={overflow:N0}");
    }

    static void E10_FairBench()
    {
        Log("## E10 fair micro-benchmark, 3M keys, 8 threads, whole-loop timing (no per-call Stopwatch)");
        var data = GetData(3_000_000);
        var keys = data.Select(d => (d.l << 42) | (d.c << 24) | (d.s << 10)).ToArray();
        int threads = Environment.ProcessorCount;
        const int opsPerThread = 3_000_000;

        // Stopwatch overhead used by original tests
        var swo = new Stopwatch(); var outer = Stopwatch.StartNew();
        for (int i = 0; i < 10_000_000; i++) { swo.Start(); swo.Stop(); }
        Log($"Stopwatch Start()+Stop() pair overhead ≈ {outer.Elapsed.TotalNanoseconds / 10_000_000:F1} ns (the original tests wrap every call in one)");

        double Run(Action<int, Random> op)
        {
            var bar = new Barrier(threads + 1);
            var ths = Enumerable.Range(0, threads).Select(ti => new Thread(() =>
            {
                var rnd = new Random(ti * 7919 + 1); bar.SignalAndWait();
                for (int i = 0; i < opsPerThread; i++) op(ti, rnd);
                bar.SignalAndWait();
            })).ToList();
            ths.ForEach(x => x.Start());
            bar.SignalAndWait(); var sw = Stopwatch.StartNew(); bar.SignalAndWait(); sw.Stop();
            ths.ForEach(x => x.Join());
            return (double)threads * opsPerThread / sw.Elapsed.TotalSeconds / 1e6;
        }

        {
            var t = new KeyIn54BitCASHashTable(10107313, 256);
            foreach (var d in data) t.TrySet(d.l, d.c, d.s, d.url);
            GC.Collect();
            Log($"CAS  TryGet (copy out, alloc)        : {Run((ti, r) => { var d = data[r.Next(data.Count)]; t.TryGet(d.l, d.c, d.s, out _); }):F2} M ops/s");
            Log($"CAS  TrySet (update, copy in)        : {Run((ti, r) => { var d = data[r.Next(data.Count)]; t.TrySet(d.l, d.c, d.s, d.url); }):F2} M ops/s");
            Log($"CAS  mixed 80% get/15% set/5% delete : {Run((ti, r) => { var d = data[r.Next(data.Count)]; int x = r.Next(100); if (x < 80) t.TryGet(d.l, d.c, d.s, out _); else if (x < 95) t.TrySet(d.l, d.c, d.s, d.url); else t.TryDelete(d.l, d.c, d.s); }):F2} M ops/s");
            t = null; GC.Collect();
        }
        {
            var cd = new ConcurrentDictionary<long, byte[]>(Environment.ProcessorCount, 6_000_000);
            for (int i = 0; i < keys.Length; i++) cd[keys[i]] = (byte[])data[i].url.Clone();
            GC.Collect();
            Log($"CD   TryGetValue (reference, no copy) : {Run((ti, r) => { int i = r.Next(keys.Length); cd.TryGetValue(keys[i], out _); }):F2} M ops/s");
            Log($"CD   TryGetValue + copy out (same sem.): {Run((ti, r) => { int i = r.Next(keys.Length); if (cd.TryGetValue(keys[i], out var v)) { var o = new byte[v.Length]; Buffer.BlockCopy(v, 0, o, 0, v.Length); } }):F2} M ops/s");
            Log($"CD   TryAdd (as original test, mostly no-op): {Run((ti, r) => { int i = r.Next(keys.Length); cd.TryAdd(keys[i], data[i].url); }):F2} M ops/s");
            Log($"CD   indexer set (real upsert, new copy): {Run((ti, r) => { int i = r.Next(keys.Length); cd[keys[i]] = (byte[])data[i].url.Clone(); }):F2} M ops/s");
            Log($"CD   mixed 80/15/5 (copy semantics)   : {Run((ti, r) => { int i = r.Next(keys.Length); int x = r.Next(100); if (x < 80) { if (cd.TryGetValue(keys[i], out var v)) { var o = new byte[v.Length]; Buffer.BlockCopy(v, 0, o, 0, v.Length); } } else if (x < 95) cd[keys[i]] = (byte[])data[i].url.Clone(); else cd.TryRemove(keys[i], out _); }):F2} M ops/s");
        }
    }

    static void E11_Memory()
    {
        Log("## E11 memory & construction cost of the test configuration");
        GC.Collect(); long before = GC.GetTotalMemory(true);
        var sw = Stopwatch.StartNew();
        var t = new KeyIn54BitCASHashTable(10107313, 256);
        sw.Stop();
        long after = GC.GetTotalMemory(true);
        Log($"new KeyIn54BitCASHashTable(10107313, 256): {sw.ElapsedMilliseconds} ms, managed heap +{(after - before) / 1024.0 / 1024 / 1024:F2} GB, {10107313:N0} separate byte[] objects");
        sw.Restart(); GC.Collect(2, GCCollectionMode.Forced, true); sw.Stop();
        Log($"one full blocking Gen2 GC with the table alive: {sw.ElapsedMilliseconds} ms");
        GC.KeepAlive(t);
        t = null; GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
        before = GC.GetTotalMemory(true);
        var data = GetData(3_000_000);
        var cd = new ConcurrentDictionary<long, byte[]>();
        foreach (var d in data) cd[(d.l << 42) | (d.c << 24) | (d.s << 10)] = d.url;
        after = GC.GetTotalMemory(true);
        Log($"ConcurrentDictionary with the same 3M entries (incl. value arrays): +{(after - before) / 1024.0 / 1024 / 1024:F2} GB");
        GC.KeepAlive(cd);
    }

    static void E12_Oversubscription()
    {
        Log("## E12 one hot key, mixed get/set, 8 vs 64 threads, 3 s each (spin-without-backoff under preemption)");
        foreach (int n in new[] { 8, 64 })
        {
            var t = new KeyIn54BitCASHashTable(101, 256);
            var c = new byte[256]; t.TrySet(1, 1, 1, c);
            long ops = 0; bool stop = false;
            var ths = Enumerable.Range(0, n).Select(i => new Thread(() =>
            {
                long l = 0; var r = new Random(i);
                while (!Volatile.Read(ref stop)) { if (r.Next(4) == 0) t.TrySet(1, 1, 1, c); else t.TryGet(1, 1, 1, out _); l++; }
                Interlocked.Add(ref ops, l);
            })).ToList();
            ths.ForEach(x => x.Start()); Thread.Sleep(3000); Volatile.Write(ref stop, true); ths.ForEach(x => x.Join());
            var p = Process.GetCurrentProcess();
            Log($"threads={n}: {ops / 3.0 / 1e6:F2} M ops/s");
        }
    }


    static void E13_CdMemory()
    {
        Log("## E13 ConcurrentDictionary memory for the same 3M entries");
        var data = GetData(3_000_000);
        GC.Collect(); long before = GC.GetTotalMemory(true);
        var cd = new ConcurrentDictionary<long, byte[]>();
        foreach (var d in data) cd[(d.l << 42) | (d.c << 24) | (d.s << 10)] = (byte[])d.url.Clone();
        long after = GC.GetTotalMemory(true);
        Log($"CD 3M entries incl. private value copies: +{(after - before) / 1024.0 / 1024:F0} MB");
        GC.KeepAlive(cd); GC.KeepAlive(data);
    }

    static void E14_CdAttribution()
    {
        Log("## E14 why is CD upsert-with-copy slow? 8 threads x 3M ops, 3M keys");
        var data = GetData(3_000_000);
        var keys = data.Select(d => (d.l << 42) | (d.c << 24) | (d.s << 10)).ToArray();
        var cd = new ConcurrentDictionary<long, byte[]>(Environment.ProcessorCount, 6_000_000);
        for (int i = 0; i < keys.Length; i++) cd[keys[i]] = (byte[])data[i].url.Clone();
        int threads = Environment.ProcessorCount; const int ops = 3_000_000;
        (double mops, int gc0, int gc2) Run(Action<Random> op)
        {
            GC.Collect(); int g0 = GC.CollectionCount(0), g2 = GC.CollectionCount(2);
            var sw = Stopwatch.StartNew();
            Parallel.For(0, threads, new ParallelOptions { MaxDegreeOfParallelism = threads }, ti => { var r = new Random(ti + 1); for (int i = 0; i < ops; i++) op(r); });
            sw.Stop();
            return (threads * (double)ops / sw.Elapsed.TotalSeconds / 1e6, GC.CollectionCount(0) - g0, GC.CollectionCount(2) - g2);
        }
        var a = Run(r => { int i = r.Next(keys.Length); cd[keys[i]] = data[i].url; });
        Log($"CD indexer set, reuse existing reference (no alloc): {a.mops:F2} M ops/s, gen0 GCs={a.gc0}, gen2 GCs={a.gc2}");
        var b = Run(r => { int i = r.Next(keys.Length); var c = new byte[data[i].url.Length]; });
        Log($"allocation only (new byte[~40], not stored)        : {b.mops:F2} M ops/s, gen0 GCs={b.gc0}, gen2 GCs={b.gc2}");
        var c2 = Run(r => { int i = r.Next(keys.Length); cd[keys[i]] = (byte[])data[i].url.Clone(); });
        Log($"CD indexer set with fresh copy (old->young refs)   : {c2.mops:F2} M ops/s, gen0 GCs={c2.gc0}, gen2 GCs={c2.gc2}");
        GC.KeepAlive(cd);
    }

    static int Main(string[] args)
    {
        var map = new Dictionary<string, Action>
        {
            ["e1"] = E1_MinValueKey, ["e2"] = E2_FullTable, ["e3"] = E3_Tombstones, ["e4"] = E4_LeakedWriteBit,
            ["e5"] = E5_PowerOfTwo, ["e6"] = E6_ProbeStats, ["e7"] = E7_TornReadStress, ["e8"] = E8_WriterStarvation,
            ["e9"] = E9_ReaderOverflow, ["e10"] = E10_FairBench, ["e11"] = E11_Memory, ["e12"] = E12_Oversubscription, ["e13"] = E13_CdMemory, ["e14"] = E14_CdAttribution,
        };
        foreach (var a in args)
        {
            var sw = Stopwatch.StartNew();
            map[a.ToLowerInvariant()]();
            Log($"   ({a} took {sw.Elapsed.TotalSeconds:F1}s)\n");
        }
        Environment.Exit(0); // kill spinning background threads left by hang experiments
        return 0;
    }
}
