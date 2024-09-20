# Lock-Free Hash Table / CAS Hash Table 

## Background
- Saw an [article](http://chuansong.me/n/1489885) outlining the core data structures and algorithms of the trading system used by the Shanghai Stock Exchange. 
- Implemented it in C# and applied it to a pre-production environment for the MS short link service.

## Core Data Structure
- The key value in the hash table is a 64-bit integer:
- 54 bytes are reserved for the business logic to set the real key value; 
- 1 byte is used to mark whether the “writer” has obtained an exclusive lock; 
- 1 byte is used to mark whether this record has been deleted or not; 
- 8 byte are used to record the number of “readers”. 

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/DataStructure.png)

- The combination of linkId, clcId, sbp in the figure below becomes the key value of the business logic, with a size of 54 bytes.

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/KeyGen.png)

## 性能实测结果汇总
测试机为Dell Z440 工作站，16GB内存，8核CPU；当key为64位整形、value为256 bytes 时，测试结果如下
- 单进程装载3百万键值对用时4秒。
- CPU压满情况下，一秒钟处理711,3400 随机“读取” 请求，892,7004 随机 “更新” 请求以及1356,6043随机 “删除” 请求。此处的随机指的是从装载入表的3百万Key中随机选取。
- 测试时在内存中保存了3百万键值对当作数据源，用来装载Hash表以及随机挑选测试数据。
- “删除”操作会从Hash表中将指定的Key标记为“已删除”，“更新”操作会重置标记为“已删除”数据的“已删除”标签，将这条数据重新恢复。
- 我的实现和原文中的实现区别在于：上交所hash表的value是int64，我的实现中hash表的value是256 bytes，这样的话，性能上就是Buffer.BlockCopy()拖了后腿。其实尽量避免memcpy在任何语言中都是一个很重要的优化方向，上交所key和value都是int64，也确实是做到了极致。
- 由于是CPU密集型计算，实际吞吐量能够随着CPU Core的数量增加而同比例增长。

在同样条件下，Lock-Free Hash Table 与 .Net Concurrent Dictionary 性能对比

|操作类型|Lock-Free Hash Table|.Net Concurrent Dictionary|优势百分比|
|:----------|----------:|----------:|----------:|
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