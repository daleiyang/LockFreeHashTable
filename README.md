# Lock-Free Hash Table / CAS Hash Table / 无锁哈希表

## 背景
- 在公众号“大数据文摘”上看到[《史上最强算法论战：请不要嘻哈，这是哈希》](https://cloud.tencent.com/developer/article/1130969)一文，了解到了“上海证券交易所”的“证券交易系统”使用的核心数据结构和算法。
- 2010年起，上交所一直使用这套核心算法，即使面对2015年牛市行情、呈爆炸式增长的每日过万亿的交易量，系统依旧平稳运行。
- 我使用C#做了一份实现，并且应用在了MS的短链接服务的预生产环境中。

## Background
- Saw an [article](https://cloud.tencent.com/developer/article/1130969) outlining the core data structures and algorithms used in the Shanghai Stock Exchange's securities trading system.
- Since 2010, the Shanghai Stock Exchange has been using this core algorithm, and even in the face of the bull market in 2015 and the explosive growth of daily trading volume exceeding one trillion RMB, the system has continued to operate smoothly.
- Implemented it in C# and applied it to a pre-production environment for the MS short link service.

## Data Structure
- The key value in the hash table is a 64-bit integer:
- 54 bytes are reserved for the business logic to set the real key value; 
- 1 byte is used to mark whether the “writer” has obtained an exclusive lock; 
- 1 byte is used to mark whether this record has been deleted or not; 
- 8 byte are used to record the number of “readers”. 

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/DataStructure.png)

- The combination of linkId, clcId, sbp in the figure above becomes the key value of the business logic, with a size of 54 bytes.
- The code in the figure below is the process of generating a 54-byte key value based on business logic, refer to [KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs).

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/KeyGen.png)

- In securities trading system, the value is 64-bit integer instead of 256 byte array in my demo. For performance reasons, 64-bit integers are the most efficient choice.
- According to the number of keys in the business logic, select a prime number as the length of the hash table so that the load factor is 0.5. This can control the average number of hash table lookups to 1.1.

## Algorithms
- The TrySet, TryGet and TryDelete functions in [KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs) are the entry points.
- [KeyIn54BitCASHashTableBase.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTableBase.cs)  contains detailed comments explaining the principles of each bit operation and how to use the CAS API to read, add, update, and delete data.
- The “do... . while” loop in the figure below is a typical CAS API usage. 

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/CAS.png)

## Performance
- Envrionment: Dell Z440 work station, 16 GB memory, 8 core CPU; 
- Hash table setting: key is int64, value is 256 byte array.
- Loads 3 million key-value pairs into a lock-free hash table in 4 seconds using a single process.
- 30 threads are started simultaneously. 10 are used to randomly get a record, 10 are used to randomly update a record, and 10 are used to randomly delete a record.

|Operation|Lock-Free Hash Table (operations per second)|.Net Concurrent Dictionary (operations per second)|Comparison|
|:----------|----------:|----------:|----------:|
|Get|7,113,400|1,681,929|<font color="red">4.2X</font>|
|Add/Update|8,927,004|240,321|<font color="red">37.1X</font>|
|Delete|13,566,043|245,884|<font color="red">55.2X</font>|

## Performance test report [[PerfTestingResults.xlsx]](https://github.com/daleiyang/LockFreeHashTable/raw/master/CASHashTable/PerfTestingResults.xlsx) summary

### Test Case One:  
- A key is randomly selected for each call across all processes.
- 10 “read” processes, 10 “update” processes and 10 "delete” processes are started and ended at the same time.

#### Test result for 10 “read” processes

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Get%20Random.jpg)
- [Get Attemps]：Total number of “TryGet” calls. Each time “TryGet”  is called, a random key is selected from the 3 million keys.
- [API Call Elapsed Time]：The time consumed by the “TryGet” function, excluding the time consumed by other auxiliary logic.
- [Get RPS/One Thread]：Number of times the "TryGet" function is executed per second.
- [Get API Call Elapsed Time/100,000,000 Attemps]：Time consumed per 100 million "TryGet" calls.
- [Get Successfully]：The number of successful value retrievals.
- [Get Successfully Percentage]：The number of successful value retrievals as a percentage of the total number of attempts.
- [Is Deleted]：The number of times the target key has been deleted.
- [Is Deleted Percentage]：The number of times the target key has been deleted as a percentage of the total number of attempts.
- [Result Match]：Data correctness verification ensures that the value obtained is consistent with the original value.
- [Result Match Percentage]：Percentage of data that is correct.
- [Test Elapsed Time]：Total test duration.

#### Test result for 10 “update” processes

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Update%20Random.jpg)
- [Update Attemps]：Total number of “TrySet” calls. Each time “TrySet”  is called, a random key is selected from the 3 million keys.
- [API Call Elapsed Time]：The time consumed by the “TrySet” function, excluding the time consumed by other auxiliary logic.
- [Update RPS/Thread]：Number of times the "TrySet" function is executed per second.
- [Update API Call Elapsed Time/100,000,000 Attemps]：Time consumed per 100 million "TrySet" calls.
- [Update Successfully]：Number of successful "TrySet".
- [Test Elapsed Time]：Total test duration.

#### Test result for 10 “delete” processes

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Delete%20Random.jpg)
- [Delete Attemps]：Total number of “TryDelete" calls. Each time “TryDelete”  is called, a random key is selected from the 3 million keys.
- [API Call Elapsed Time]：The time consumed by the “TryDelete” function, excluding the time consumed by other auxiliary logic.
- [Delete RPS/Thread]：Number of times the "TryDelete" function is executed per second.
- [Delete API Call Elapsed Time/100,000,000 Attemps]：Time consumed per 100 million "TryDelete" calls.
- [Delete Successfully]：Number of successful "TryDelete".
- [Delete Successfully Percentage]：The number of times the target key has been deleted as a percentage of the total number of attempts.
- [Is Deleted]：The number of times the target key has been deleted.
- [Is Deleted Percentage]：The target key has been deleted times as a percentage of the total number of attempts.
- [Test Elapsed Time]：Total test duration.

### ### Test Case two:  
- All processes select the same key on every call.
- 10 “read” processes, 10 “update” processes and 10 "delete” processes are started and ended at the same time.

#### Test result for 10 “read” processes

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Get%20One.jpg)

#### Test result for 10 “update” processes

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Update%20One.jpg)

#### Test result for 10 “delete” processes

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Delete%20One.jpg)

## Test Code
- [KeyIn54BitCASHashTableFunctionalTest.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTableFunctionalTest.cs) in the test project is a functional test that includes the verification of the correctness of addition, deletion, modification, and query in various situations.

- [KeyIn54BitCASHashTablePerfTest.cs ](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTablePerfTest.cs) is a stress test. For the test method and result analysis, please refer to the section “Performance test report summary” above.
