# Lock-Free Hash Table / CAS Hash Table / 无锁哈希表

## 背景
- 在公众号“大数据文摘”上看到[《史上最强算法论战：请不要嘻哈，这是哈希》](https://cloud.tencent.com/developer/article/1130969)一文，了解到了“上海证券交易所”的“证券交易系统”使用的核心数据结构和算法。
- 2010年起，上交所一直使用这套核心算法，即使面对2015年牛市行情、呈爆炸式增长的每日过万亿的交易量，系统依旧平稳运行。
- 我使用C#做了一份实现，并且应用在了MS的短链接服务的预生产环境中。

## Background
- Saw an [article](https://cloud.tencent.com/developer/article/1130969) outlining the core data structures and algorithms used in the Shanghai Stock Exchange's securities trading system.
- Since 2010, the Shanghai Stock Exchange has been using this core algorithm, and even in the face of the bull market in 2015 and the explosive growth of daily trading volume exceeding one trillion RMB, the system has continued to operate smoothly.
- Implemented it in C# and applied it to a pre-production environment for the MS short link service.

## Core Data Structure
- The key value in the hash table is a 64-bit integer:
- 54 bytes are reserved for the business logic to set the real key value; 
- 1 byte is used to mark whether the “writer” has obtained an exclusive lock; 
- 1 byte is used to mark whether this record has been deleted or not; 
- 8 byte are used to record the number of “readers”. 

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/DataStructure.png)

- The combination of linkId, clcId, sbp in the figure above becomes the key value of the business logic, with a size of 54 bytes.
- The code in the figure below is the process of generating a 54-byte key value based on business logic, refer to [KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs).

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/KeyGen.png)

- In securities trading system, the value is 64-bit integer instead of 256 byte array in my demo. For performance reasons, 64-bit integers are the most efficient choice.
- According to the number of keys in the business logic, select a prime number as the length of the hash table so that the load factor is 0.5. This can control the average number of hash table lookups to 1.1.

## Algorithms
- The TrySet, TryGet and TryDelete functions in [KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs) are the entry points.
- [KeyIn54BitCASHashTableBase.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTableBase.cs)  contains detailed comments explaining the principles of each bit operation and how to use the CAS API to read, add, update, and delete data.
- The “do... . while” loop in the figure below is a typical CAS API usage. 

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/CAS.png)

## Performance Test Results
- Envrionment: Dell Z440 work station, 16 GB memory, 8 core CPU; 
- Hash table setting: key is int64, value is 256 byte array.
- Loads 3 million key-value pairs into a lock-free hash table in 4 seconds using a single process.
- CPU压满情况下，一秒钟处理711,3400 随机“读取” 请求，892,7004 随机 “更新” 请求以及1356,6043随机 “删除” 请求。此处的随机指的是从装载入表的3百万Key中随机选取。

Compare the performance of "Lock-Free Hash Tables" and ".Net Concurrent Dictionary Classes".

|Operation|Lock-Free Hash Table(operations per second)|.Net Concurrent Dictionary(operations per second)|Performance Improvement|
|:----------|-----------|-----------|----------:|
|Get|7,113,400|1,681,929|<font color="red">422.93%</font>|
|Add/Update|8,927,004|240,321|<font color="red">3714.61%</font>|
|Delete|13,566,043|245,884|<font color="red">5517.26%</font>|

## 性能对比结果、压力测试报告[PerfTestingResults.xlsx](https://github.com/daleiyang/LockFreeHashTable/raw/master/CASHashTable/PerfTestingResults.xlsx)详解

在下面两组测试中，每组测试中的三种操作的总次数是经过反复调整的，目的是使得测试中使用的30个进程尽量在同一时间结束，这样能够让下面截图中的各种测试结果有意义。

### 随机抽取数据的方式测试，同时10个进程“读取”、10个进程“更新”、10个进程“删除”

#### 10个“读取”进程测试结果

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Get%20Random.jpg)
每列含义:
- Get Attemps：尝试“读取”的总次数。每次调用时在装载的3百万的键值中随机选择一个。
- API Call Elapsed Time：单纯TryGet函数调用所消耗时间，排除其他辅助测试逻辑的时间消耗。
- Get RPS/One Thread：每秒执行的TryGet函数调用次数。
- Get API Call Elapsed Time/100,000,000 Attemps：每1亿次TryGet调用所消耗的时间。
- Get Successfully：成功取到值的次数。
- Get Successfully Percentage：成功取到值的次数占总尝试次数的百分比。
- Is Deleted：目标数据已经删除的次数。
- Is Deleted Percentage：目标数据已经删除次数占总尝试次数百分比。
- Result Match：数据正确性验证，确保取到的值和原始值一致。
- Result Match Percentage：数据正确性百分比。
- Test Elapsed Time：测试总用时。

#### 10个“更新”进程测试结果

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Update%20Random.jpg)
每列含义:
- Update Attemps：尝试“更新”的总次数。每次调用时在装载的3百万的键值中随机选择一个。
- API Call Elapsed Time：单纯TrySet函数调用所消耗时间，排除其他辅助测试逻辑的时间消耗。
- Update RPS/Thread：每秒执行的TrySet函数调用次数。
- Update API Call Elapsed Time/100,000,000 Attemps：每1亿次TrySet调用所消耗的时间。
- Update Successfully：更新成功的次数。
- Test Elapsed Time：测试总用时。

#### 10个“删除”进程测试结果

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Delete%20Random.jpg)
每列含义:
- Delete Attemps：尝试“删除”的总次数。每次调用时在装载的3百万的键值中随机选择一个。
- API Call Elapsed Time：单纯TryDelete函数调用所消耗时间，排除其他辅助测试逻辑的时间消耗。
- Delete RPS/Thread：每秒执行的TryDelete函数调用次数。
- Delete API Call Elapsed Time/100,000,000 Attemps：每1亿次TryDelete调用所消耗的时间。
- Delete Successfully：删除成功的次数。
- Delete Successfully Percentage：删除成功次数占总尝试次数的百分比。
- Is Deleted：目标数据已经删除的次数。
- Is Deleted Percentage：目标数据已经删除次数占总尝试次数百分比。
- Test Elapsed Time：测试总用时。

### 对同一个数据进行测试，同时10个进程“读取”、10个进程“更新”、10个进程“删除”

#### 10个“读取”进程测试结果

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Get%20One.jpg)
每列含义:同上，除了30个测试进程选择同样一条数据进行测试。

#### 10个“更新”进程测试结果

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Update%20One.jpg)
每列含义:同上，除了30个测试进程选择同样一条数据进行测试。

#### 10个“删除”进程测试结果

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Delete%20One.jpg)
每列含义:同上，除了30个测试进程选择同样一条数据进行测试。

## 源代码说明：
- CASHashTable工程中的[KeyIn54BitCASHashTableBase.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTableBase.cs)是核心代码、基类，代码中提供了详细的注释，解释了每种位运算的原理、对应不同操作(set/update,get,delete)时CAS操作应该出现的位置和原理。

- CASHashTable工程中的[KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs)是使用基类的例子，使用者需要提供TrySet，TryGet，TryDelete和GenerateKey的实现。如果需要理解代码，可以从这里入手。

- Test测试工程中的[KeyIn54BitCASHashTableFunctionalTest.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTableFunctionalTest.cs)是功能测试，包括各种情况下的增删改查的正确性验证。

- Test测试工程中的[KeyIn54BitCASHashTablePerfTest.cs ](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTablePerfTest.cs)是压力测试，测试方法和结果分析请参考上面“性能对比结果、压力测试报告”一节的内容。

- 代码中的位运算逻辑和其他逻辑经过了大量反复的测试，没有发现Bug。

## 使用方法：
- Utility.cs 中获取数据的SQL字段，可以随机生成key/value测试。

- 修改KeyIn54BitCASHashTable.cs，提供你自己的TrySet，TryGet，TryDelete， GenerateKey函数。

- 跑Test测试工程中的功能测试文件[KeyIn54BitCASHashTableFunctionalTest.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTableFunctionalTest.cs)测试正确性。