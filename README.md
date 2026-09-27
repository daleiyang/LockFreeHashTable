# Technology showcase: microservices, docker compose, event-driven architecture with RabbitMQ, REST WebApi and my implimentation of lock-free hash table as a in-memory cache, unit test, performance test.

## How To Use
- Environment: VS Code, .NET 8.0, Docker Desktop, RabbitMQ.Client 7.0
- git clone https://github.com/daleiyang/LockFreeHashTable
- Open local floder "LockFreeHashTable" with VS Code.
- Start a Terminal and execute: " docker compose up --build ".

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/3.jpg)

- If all goes well, you should see four microservices successfully built.

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/1.jpg)

- If all goes well, you should see four microservices successfully deployed in Docker Desktop, and they should be able to maintain continuous communication with other microservices.

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/2.jpg)

## Architecture

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/4.jpg)

$${\color{red}Step\ 4}$$ Load 3 million records into lock-free hash table. See [Program.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/WebApi/Program.cs#L3) (line 3 to 4) and [CAS.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/WebApi/CAS.cs#L104) (line 104 to 130). For example: records[1] has properties "linkId = 2 clid = 2 sbp = 2" and value = "http://www.microsoft.com/abc.asp+1" 

$${\color{red}Step\ 1}$$ The RPC Client continuously randomly selects "Get" or "Update" or "Delete" records[1] and sends requests to RabbitMQ. See [RPCClient.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/RPCClient/RPCClient.cs#L105) (line 105 to 122)

$${\color{red}Step\ 2}$$ The callback function of the RPC Server obtains requests from RabbitMQ. See [RPCServer.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/RPCServer/RPCServer.cs#L36) (line 36 to 37)

$${\color{red}Step\ 3}$$ The RPC Server calls corresponding REST Web Api. See [RPCServer.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/RPCServer/RPCServer.cs#L39) (line 39 to 51)

$${\color{red}Step\ 5}$$ The Web API receives requests and performs operations on lock-free hash table. See [Program.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/WebApi/Program.cs#L10) (line 10 to 44)

$${\color{red}Step\ 6}$$ The RPC Server receives response from Web Api. See [RPCServer.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/RPCServer/RPCServer.cs#L43) (line 43 to 49)

$${\color{red}Step\ 7}$$ The RPC Server sends responses to RabbitMQ. See [RPCServer.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/RPCServer/RPCServer.cs#L62) (line 62 to 65)

$${\color{red}Step\ 8}$$ The callback function of the RPC Client obtains responses from RabbitMQ. See [RPCClient.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/RPCClient/RPCClient.cs#L128) (line 128)

# Implementation of lock-free hash table

## How To Use 
- git clone https://github.com/daleiyang/LockFreeHashTable
- Open solution "CASHashTable.sln", execute the unit tests.
- Environment: VS 2022 and .NET 8.0

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/ut.jpg)

## First, the conclusion
- If you can combine the required keys into a 64-bit integer, using .Net's Concurrent Dictionary is a good option.

## Background
- Saw an [article](https://cloud.tencent.com/developer/article/1130969) outlining the core data structures and algorithms used in the Shanghai Stock Exchange's securities trading system.
- Since 2010, the Shanghai Stock Exchange has been using this core algorithm, and even in the face of the bull market in 2015 and the explosive growth of daily trading volume exceeding one trillion RMB, the system has continued to operate smoothly.
- Implemented in C# as a candidate solution for MS's short link service.

## Data Structure
- The key value in the hash table is a 64-bit integer:
- 54 bytes are reserved for the business logic to set the real key value; 
- 1 byte is used to mark whether the "writer" has obtained an exclusive lock; 
- 1 byte is used to mark whether this record has been deleted or not; 
- 8 byte are used to record the number of "readers". 

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/DataStructure.png)

- The combination of linkId, clcId, sbp in the figure above becomes the key value of the business logic, with a size of 54 bytes.
- The code in the figure below is the process of generating a 54-byte key value based on business logic, refer to [KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs#L44) [line 44 to 47].

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/KeyGen.png)

- In securities trading system, the value is 64-bit integer instead of 256 byte array in my demo. For performance reasons, 64-bit integers are the most efficient choice.
- According to the number of keys in the business logic, select a prime number as the length of the hash table so that the load factor is 0.5. This can control the average number of hash table lookups to 1.1.

## Algorithms
- The TrySet, TryGet and TryDelete functions in [KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs#L11) [line 11 to 42] are the entry points.
- [KeyIn54BitCASHashTableBase.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTableBase.cs)  contains detailed comments explaining the principles of each bit operation and how to use the CAS API to read, add, update, and delete data.
- The "do... . while" loop in the figure below is a typical CAS API usage. 

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/CAS.png)

## Performance Test Report [[PerformanceReport.xlsx]](https://github.com/daleiyang/LockFreeHashTable/raw/refs/heads/master/PerformanceReport.xlsx) 

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/perf.jpg)

## Test Project
- [KeyIn54BitCASHashTableFunctionalTest.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTableTest/KeyIn54BitCASHashTableFunctionalTest.cs) is unit tests for lock-free hash table.

- [KeyIn54BitCASHashTablePerfTest.cs ](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTableTest/KeyIn54BitCASHashTablePerfTest.cs) is preformance tests for lock-free hash table. Please see [[Report]](https://github.com/daleiyang/LockFreeHashTable/raw/refs/heads/master/PerformanceReport.xlsx) .

- [ConcurrentDictionaryPerfTesting.cs ](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTableTest/ConcurrentDictionaryPerfTesting.cs) is preformance tests for .Net Concurrent Dictionary. Please see [[Report]](https://github.com/daleiyang/LockFreeHashTable/raw/refs/heads/master/PerformanceReport.xlsx) .

# Code Analysis by Claude Code (2026-09)

An independent review of the lock-free hash table (`CASHashTable`), its tests (`CASHashTableTest`) and `PerformanceReport.xlsx`. The WebApi / RPC / Docker / RabbitMQ demo skeleton is out of scope. Every conclusion was verified by actually running the code on the same laptop model as the performance report (i7-1065G7, 16GB, .NET 8).

- **[View the full report (rendered HTML)](https://raw.githack.com/daleiyang/LockFreeHashTable/master/Analysis/LockFreeHashTable_Analysis.html)** (in Chinese) · [alternative viewer](https://htmlpreview.github.io/?https://github.com/daleiyang/LockFreeHashTable/blob/master/Analysis/LockFreeHashTable_Analysis.html) · [HTML source](Analysis/LockFreeHashTable_Analysis.html)
- The report contains a class diagram, flowcharts of TrySet / Update / TryGet / TryDelete / HashSearch, a slot state diagram, the 64-bit key layout, a decoded table of every bit mask, a findings list with evidence, suggested fixes with sample code, and an overall score.
- Experiment code (E1 to E14) and raw logs: [Analysis/experiments](Analysis/experiments)

## Key findings
- All 15 functional tests and 4 performance tests pass.
- **The concurrency protocol is correct.** A stress test with versioned payloads (4 readers, 2 writers, 2 deleters) did 21,174,067 successful reads with 0 torn reads.
- **Strictly speaking, it is not lock-free.** It is a per-slot reader-writer spin lock implemented with CAS: if a thread holding the write bit is preempted or throws, other threads spin on that slot.
- **Three critical bugs, all reproduced:**
  - When the table is full, `TryGet` / `TryDelete` / `TrySet` on a missing key loop forever.
  - Deleted slots (tombstones) are never reclaimed, so after enough distinct keys have ever been inserted, inserts loop forever even if every key was deleted.
  - An exception inside the write critical section (no `try/finally`) leaves the write bit set, so the key can never be read or written again.
- **Other issues:**
  - A valid input (`linkId = 2097152, clcId = 0, sbp = 0`) produces `long.MinValue`, and `Math.Abs` throws `OverflowException`.
  - The public base-class API bypasses all validation.
  - The 8-bit reader counter overflows under thread oversubscription.
  - Writers starve: with 7 readers, p99.9 write latency grows from 0.5 µs to 12.5 ms.
- **Tests:**
  - No concurrency correctness assertions.
  - Performance tests have no assertions.
  - A Stopwatch pair per call costs about 34 ns.
  - The ConcurrentDictionary comparison does not use the same semantics (`TryAdd` vs upsert-with-copy, references vs copies).
- **Revisiting "First, the conclusion":** it holds when values are immutable and can be shared by reference (reads: ConcurrentDictionary 19.1 vs CAS 10.8 M ops/s). With the same copy-on-write semantics, the CAS table is much faster for updates (9.48 vs 0.49 M ops/s) because preallocated in-place buffers avoid GC card-marking costs.
- **Overall score: 61 / 100.** The hard part (the concurrency protocol) is right; the routine defenses (probe limits, `try/finally`, space reclamation) and concurrency tests are missing. With the minimal fixes in the report, it would rate 75 to 80.
