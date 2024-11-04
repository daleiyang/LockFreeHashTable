# Lock-Free Hash Table / CAS Hash Table / ������ϣ��

## ����
- �ڹ��ںš���������ժ���Ͽ���[��ʷ����ǿ�㷨��ս���벻Ҫ���������ǹ�ϣ��](https://cloud.tencent.com/developer/article/1130969)һ�ģ��˽⵽�ˡ��Ϻ�֤ȯ���������ġ�֤ȯ����ϵͳ��ʹ�õĺ������ݽṹ���㷨��
- 2010�����Ͻ���һֱʹ�����׺����㷨����ʹ���2015��ţ�����顢�ʱ�ըʽ������ÿ�չ����ڵĽ�������ϵͳ����ƽ�����С�
- ��ʹ��C#����һ��ʵ�֣�����Ӧ������MS�Ķ����ӷ����Ԥ���������С�

## Background
- Saw an [article](https://cloud.tencent.com/developer/article/1130969) outlining the core data structures and algorithms used in the Shanghai Stock Exchange's securities trading system.
- Since 2010, the Shanghai Stock Exchange has been using this core algorithm, and even in the face of the bull market in 2015 and the explosive growth of daily trading volume exceeding one trillion RMB, the system has continued to operate smoothly.
- Implemented it in C# and applied it to a pre-production environment for the MS short link service.

## Data Structure
- The key value in the hash table is a 64-bit integer:
- 54 bytes are reserved for the business logic to set the real key value; 
- 1 byte is used to mark whether the ��writer�� has obtained an exclusive lock; 
- 1 byte is used to mark whether this record has been deleted or not; 
- 8 byte are used to record the number of ��readers��. 

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/DataStructure.png)

- The combination of linkId, clcId, sbp in the figure above becomes the key value of the business logic, with a size of 54 bytes.
- The code in the figure below is the process of generating a 54-byte key value based on business logic, refer to [KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs).

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/KeyGen.png)

- In securities trading system, the value is 64-bit integer instead of 256 byte array in my demo. For performance reasons, 64-bit integers are the most efficient choice.
- According to the number of keys in the business logic, select a prime number as the length of the hash table so that the load factor is 0.5. This can control the average number of hash table lookups to 1.1.

## Algorithms
- The TrySet, TryGet and TryDelete functions in [KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs) are the entry points.
- [KeyIn54BitCASHashTableBase.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTableBase.cs)  contains detailed comments explaining the principles of each bit operation and how to use the CAS API to read, add, update, and delete data.
- The ��do... . while�� loop in the figure below is a typical CAS API usage. 

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/CAS.png)

## Performance Test Report [[PerformanceReport.xlsx]](https://github.com/daleiyang/LockFreeHashTable/raw/refs/heads/master/PerformanceReport.xlsx) 

![alt tag](https://raw.githubusercontent.com/daleiyang/LockFreeHashTable/refs/heads/master/Images/perf.jpg)

## Test Project
- [KeyIn54BitCASHashTableFunctionalTest.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTableFunctionalTest.cs) in the test project is a unit test that includes the verification of the correctness of addition, deletion, modification, and query in various situations.

- [KeyIn54BitCASHashTablePerfTest.cs ](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTablePerfTest.cs) is a preformance test for lock-free hash table. For the test method and result analysis, please refer to the section ��Performance Test Report�� above.

- [ConcurrentDictionaryPerfTesting.cs ](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/ConcurrentDictionaryPerfTesting.cs) is a preformance test for .Net Concurrent Dictionary. For the test method and result analysis, please refer to the section ��Performance Test Report�� above.