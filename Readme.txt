Lock Free Hash Table / CAS Hash Table / 无锁哈希表

2015年8月份，在微信公众号上看到《史上最强算法论战：请不要嘻哈，这是哈希》 http://chuansong.me/n/1489885
一文中阐述的上交所股票交易系统的核心数据结构和算法十分吸引人，我用了一个月的业余时间做了一份C#的实现，并且
顺利的应用到公司的某产品的测试阶段。

性能实测数据如下：
When key is a Int64 integer and value is 256 bytes, tested on a DELL Z440 work station with 16 GB memory and 8 core CPU:
a.	Load 3,000,000 key value pairs one by one into memory in 4 seconds with one thread.
b.	Process 7,113,400 random get requests / 8,927,000 random update requests / 13,566,040 random delete requests in 1 second simultaneously.
c.	Its capability should be able to increases in a linear manner along with CPU core quantity increasing

代码说明：
CASHashTable工程中是核心代码。
Test是测试工程。

压力测试分别测试了随机key的性能和固定一个key的增删改查性能。测试结果充分证明，随机key的增删改查性能远高于.Net提供的Concurrent Dictionary。
固定key的增删改查性能依旧领先。具体压力测试结果，参考：CASHashTable/PerfTestingResults.xlsx

如果想实测、使用，需要修改：
1. Utility.cs 中获取数据的SQL字段，可以随机生成key/value测试。
2. 修改KeyIn54BitCASHashTable.cs，提供你自己的TrySet，TryGet，TryDelete， GenerateKey函数。