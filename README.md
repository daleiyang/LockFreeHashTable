# Lock Free Hash Table / CAS Hash Table / 无锁哈希表

背景：2015年8月份，在微信公众号上看到《史上最强算法论战：请不要嘻哈，这是哈希》 http://chuansong.me/n/1489885 一文中阐述的上交所股票交易系统的核心数据结构和算法十分吸引人，我用了一个月的业余时间做了一份C#的实现，并且在公司某产品的测试阶段进行实测。

### 性能实测结果：
测试机为Dell Z440 工作站，16GB内存，8核CPU；当key为64位整形、value为256 bytes 时，测试结果如下：
- 单进程装载3百万键值对用时4秒。
- CPU压满情况下，一秒钟处理711,3400 随机 “读取” 请求，892,7004 随机 “更新” 请求以及1356,6043随机 “删除” 请求。
- 吞吐量能够随着CPU数量增加，同比例增长。

### 在同样条件下，Lock-Free Hash Table 与 .Net Concurrent Dictionary 性能对比
|操作类型|Lock-Free Hash Table|.Net Concurrent Dictionary|优势百分比|
|:----------|----------:|----------:|----------:|
|Get|7,113,400|1,681,929|<font color="red">422.93%</font>|
|Add/Update|8,927,004|240,321|<font color="red">3714.61%</font>|
|Delete|13,566,043|245,884|<font color="red">5517.26%</font>|

性能对比结果报告完整版参考：CASHashTable目录下的[PerfTestingResults.xlsx](https://github.com/daleiyang/LockFreeHashTable/raw/master/CASHashTable/PerfTestingResults.xlsx)

## 源代码说明：
- CASHashTable工程中的[KeyIn54BitCASHashTableBase.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTableBase.cs)是核心代码、基类。
- CASHashTable工程中的[KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs)是使用基类的例子，使用者需要提供TrySet，TryGet，TryDelete和GenerateKey的实现。如果需要理解代码，可以从这里入手。
- Test测试工程中的[KeyIn54BitCASHashTableFunctionalTest.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTableFunctionalTest.cs)是功能测试，包括各种情况下的增删改查的正确性验证。
- Test测试工程中的[KeyIn54BitCASHashTablePerfTest.cs ](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTablePerfTest.cs)是压力测试，测试方法是同时使用30个进程压测，其中10个进程进行随机“读取”操作、10个进程进行随机“删除”操作、10个进程进行随机“添加/更新”操作。代码中每种操作进行的总次数是精心调整过的，保证这30个进程能够在同时结束，这样能够得到一个有意义的结果。

## 使用方法：
- Utility.cs 中获取数据的SQL字段，可以随机生成key/value测试。
- 修改KeyIn54BitCASHashTable.cs，提供你自己的TrySet，TryGet，TryDelete， GenerateKey函数。
- 跑Test测试工程中的功能测试文件[KeyIn54BitCASHashTableFunctionalTest.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTableFunctionalTest.cs)测试正确性。