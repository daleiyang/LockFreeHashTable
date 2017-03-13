# Lock Free Hash Table / CAS Hash Table / 无锁哈希表

背景：2015年8月份，在微信公众号上看到《史上最强算法论战：请不要嘻哈，这是哈希》 http://chuansong.me/n/1489885 一文中阐述的上交所股票交易系统的核心数据结构和算法十分吸引人，我用了一个月的业余时间做了一份C#的实现，并且在公司某产品的测试阶段进行实测。

### 性能实测结果：
测试机为Dell Z440 工作站，16GB内存，8核CPU；当key为64位整形、value为256 bytes 时，测试结果如下：
> 1. 单进程装载3百万键值对用时4秒。
> 2. CPU压满情况下，一秒钟处理711,3400 随机 “读取” 请求，892,7004 随机 “更新” 请求以及1356,6043随机 “删除” 请求。
> 3. 吞吐量能够随着CPU数量增加，同比例增长。

### 在同样条件下，Lock-Free Hash Table 与 .Net Concurrent Dictionary 性能对比
|操作类型|Lock-Free Hash Table|.Net Concurrent Dictionary|优势百分比|
|:----------|----------:|----------:|:---------:|
|Get|7,113,400|1,681,929|<font color="red">422.93%</font>|
|Add/Update|8,927,004|240,321|<font color="red">3714.61%</font>|
|Delete|13,566,043|245,884|<font color="red">5517.26%</font>|

性能对比结果报告完整版参考：CASHashTable目录下的PerfTestingResults.xlsx

## 源代码说明：
CASHashTable工程中是核心代码；Test是测试工程。


## 使用方法：

如果想实测、使用，需要修改：
- Utility.cs 中获取数据的SQL字段，可以随机生成key/value测试。
- 修改KeyIn54BitCASHashTable.cs，提供你自己的TrySet，TryGet，TryDelete， GenerateKey函数。