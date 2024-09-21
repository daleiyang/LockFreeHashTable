# Lock-Free Hash Table / CAS Hash Table / ������ϣ��

## ����
- �ڹ��ںš���������ժ���Ͽ���[��ʷ����ǿ�㷨��ս���벻Ҫ���������ǹ�ϣ��](https://cloud.tencent.com/developer/article/1130969)һ�ģ��˽⵽�ˡ��Ϻ�֤ȯ���������ġ�֤ȯ����ϵͳ��ʹ�õĺ������ݽṹ���㷨��
- 2010�����Ͻ���һֱʹ�����׺����㷨����ʹ���2015��ţ�����顢�ʱ�ըʽ������ÿ�չ����ڵĽ�������ϵͳ����ƽ�����С�
- ��ʹ��C#����һ��ʵ�֣�����Ӧ������MS�Ķ����ӷ����Ԥ���������С�

## Background
- Saw an [article](https://cloud.tencent.com/developer/article/1130969) outlining the core data structures and algorithms used in the Shanghai Stock Exchange's securities trading system.
- Since 2010, the Shanghai Stock Exchange has been using this core algorithm, and even in the face of the bull market in 2015 and the explosive growth of daily trading volume exceeding one trillion RMB, the system has continued to operate smoothly.
- Implemented it in C# and applied it to a pre-production environment for the MS short link service.

## Core Data Structure
- The key value in the hash table is a 64-bit integer:
- 54 bytes are reserved for the business logic to set the real key value; 
- 1 byte is used to mark whether the ��writer�� has obtained an exclusive lock; 
- 1 byte is used to mark whether this record has been deleted or not; 
- 8 byte are used to record the number of ��readers��. 

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/DataStructure.png)

- The combination of linkId, clcId, sbp in the figure above becomes the key value of the business logic, with a size of 54 bytes.
- The code in the figure below is the process of generating a 54-byte key value based on business logic, refer to [KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs).

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/KeyGen.png)

- In securities trading system, the value is 64-bit integer instead of 256 byte array in my demo. For performance reasons, 64-bit integers are the most efficient choice.
- According to the number of keys in the business logic, select a prime number as the length of the hash table so that the load factor is 0.5. This can control the average number of hash table lookups to 1.1.

## Algorithms
- The TrySet, TryGet and TryDelete functions in [KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs) are the entry points.
- [KeyIn54BitCASHashTableBase.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTableBase.cs)  contains detailed comments explaining the principles of each bit operation and how to use the CAS API to read, add, update, and delete data.
- The ��do... . while�� loop in the figure below is a typical CAS API usage. 

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/CAS.png)

## Performance Test Results
- Envrionment: Dell Z440 work station, 16 GB memory, 8 core CPU; 
- Hash table setting: key is int64, value is 256 byte array.
- Loads 3 million key-value pairs into a lock-free hash table in 4 seconds using a single process.
- CPUѹ������£�һ���Ӵ���711,3400 �������ȡ�� ����892,7004 ��� �����¡� �����Լ�1356,6043��� ��ɾ���� ���󡣴˴������ָ���Ǵ�װ������3����Key�����ѡȡ��

Compare the performance of "Lock-Free Hash Tables" and ".Net Concurrent Dictionary Classes".

|Operation|Lock-Free Hash Table(operations per second)|.Net Concurrent Dictionary(operations per second)|Performance Improvement|
|:----------|-----------|-----------|----------:|
|Get|7,113,400|1,681,929|<font color="red">422.93%</font>|
|Add/Update|8,927,004|240,321|<font color="red">3714.61%</font>|
|Delete|13,566,043|245,884|<font color="red">5517.26%</font>|

## ���ܶԱȽ����ѹ�����Ա���[PerfTestingResults.xlsx](https://github.com/daleiyang/LockFreeHashTable/raw/master/CASHashTable/PerfTestingResults.xlsx)���

��������������У�ÿ������е����ֲ������ܴ����Ǿ������������ģ�Ŀ����ʹ�ò�����ʹ�õ�30�����̾�����ͬһʱ������������ܹ��������ͼ�еĸ��ֲ��Խ�������塣

### �����ȡ���ݵķ�ʽ���ԣ�ͬʱ10�����̡���ȡ����10�����̡����¡���10�����̡�ɾ����

#### 10������ȡ�����̲��Խ��

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Get%20Random.jpg)
ÿ�к���:
- Get Attemps�����ԡ���ȡ�����ܴ�����ÿ�ε���ʱ��װ�ص�3����ļ�ֵ�����ѡ��һ����
- API Call Elapsed Time������TryGet��������������ʱ�䣬�ų��������������߼���ʱ�����ġ�
- Get RPS/One Thread��ÿ��ִ�е�TryGet�������ô�����
- Get API Call Elapsed Time/100,000,000 Attemps��ÿ1�ڴ�TryGet���������ĵ�ʱ�䡣
- Get Successfully���ɹ�ȡ��ֵ�Ĵ�����
- Get Successfully Percentage���ɹ�ȡ��ֵ�Ĵ���ռ�ܳ��Դ����İٷֱȡ�
- Is Deleted��Ŀ�������Ѿ�ɾ���Ĵ�����
- Is Deleted Percentage��Ŀ�������Ѿ�ɾ������ռ�ܳ��Դ����ٷֱȡ�
- Result Match��������ȷ����֤��ȷ��ȡ����ֵ��ԭʼֵһ�¡�
- Result Match Percentage��������ȷ�԰ٷֱȡ�
- Test Elapsed Time����������ʱ��

#### 10�������¡����̲��Խ��

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Update%20Random.jpg)
ÿ�к���:
- Update Attemps�����ԡ����¡����ܴ�����ÿ�ε���ʱ��װ�ص�3����ļ�ֵ�����ѡ��һ����
- API Call Elapsed Time������TrySet��������������ʱ�䣬�ų��������������߼���ʱ�����ġ�
- Update RPS/Thread��ÿ��ִ�е�TrySet�������ô�����
- Update API Call Elapsed Time/100,000,000 Attemps��ÿ1�ڴ�TrySet���������ĵ�ʱ�䡣
- Update Successfully�����³ɹ��Ĵ�����
- Test Elapsed Time����������ʱ��

#### 10����ɾ�������̲��Խ��

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Delete%20Random.jpg)
ÿ�к���:
- Delete Attemps�����ԡ�ɾ�������ܴ�����ÿ�ε���ʱ��װ�ص�3����ļ�ֵ�����ѡ��һ����
- API Call Elapsed Time������TryDelete��������������ʱ�䣬�ų��������������߼���ʱ�����ġ�
- Delete RPS/Thread��ÿ��ִ�е�TryDelete�������ô�����
- Delete API Call Elapsed Time/100,000,000 Attemps��ÿ1�ڴ�TryDelete���������ĵ�ʱ�䡣
- Delete Successfully��ɾ���ɹ��Ĵ�����
- Delete Successfully Percentage��ɾ���ɹ�����ռ�ܳ��Դ����İٷֱȡ�
- Is Deleted��Ŀ�������Ѿ�ɾ���Ĵ�����
- Is Deleted Percentage��Ŀ�������Ѿ�ɾ������ռ�ܳ��Դ����ٷֱȡ�
- Test Elapsed Time����������ʱ��

### ��ͬһ�����ݽ��в��ԣ�ͬʱ10�����̡���ȡ����10�����̡����¡���10�����̡�ɾ����

#### 10������ȡ�����̲��Խ��

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Get%20One.jpg)
ÿ�к���:ͬ�ϣ�����30�����Խ���ѡ��ͬ��һ�����ݽ��в��ԡ�

#### 10�������¡����̲��Խ��

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Update%20One.jpg)
ÿ�к���:ͬ�ϣ�����30�����Խ���ѡ��ͬ��һ�����ݽ��в��ԡ�

#### 10����ɾ�������̲��Խ��

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Delete%20One.jpg)
ÿ�к���:ͬ�ϣ�����30�����Խ���ѡ��ͬ��һ�����ݽ��в��ԡ�

## Դ����˵����
- CASHashTable�����е�[KeyIn54BitCASHashTableBase.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTableBase.cs)�Ǻ��Ĵ��롢���࣬�������ṩ����ϸ��ע�ͣ�������ÿ��λ�����ԭ����Ӧ��ͬ����(set/update,get,delete)ʱCAS����Ӧ�ó��ֵ�λ�ú�ԭ��

- CASHashTable�����е�[KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs)��ʹ�û�������ӣ�ʹ������Ҫ�ṩTrySet��TryGet��TryDelete��GenerateKey��ʵ�֡������Ҫ�����룬���Դ��������֡�

- Test���Թ����е�[KeyIn54BitCASHashTableFunctionalTest.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTableFunctionalTest.cs)�ǹ��ܲ��ԣ�������������µ���ɾ�Ĳ����ȷ����֤��

- Test���Թ����е�[KeyIn54BitCASHashTablePerfTest.cs ](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTablePerfTest.cs)��ѹ�����ԣ����Է����ͽ��������ο����桰���ܶԱȽ����ѹ�����Ա��桱һ�ڵ����ݡ�

- �����е�λ�����߼��������߼������˴��������Ĳ��ԣ�û�з���Bug��

## ʹ�÷�����
- Utility.cs �л�ȡ���ݵ�SQL�ֶΣ������������key/value���ԡ�

- �޸�KeyIn54BitCASHashTable.cs���ṩ���Լ���TrySet��TryGet��TryDelete�� GenerateKey������

- ��Test���Թ����еĹ��ܲ����ļ�[KeyIn54BitCASHashTableFunctionalTest.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTableFunctionalTest.cs)������ȷ�ԡ�