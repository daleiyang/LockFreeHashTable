# Lock-Free Hash Table / CAS Hash Table 

## Background
- Saw an [article](http://chuansong.me/n/1489885) outlining the core data structures and algorithms of the trading system used by the Shanghai Stock Exchange. 
- Implemented it in C# and applied it to a pre-production environment for the MS short link service.

## Core Data Structure
- The key value in the hash table is a 64-bit integer:
- 54 bytes are reserved for the business logic to set the real key value; 
- 1 byte is used to mark whether the ��writer�� has obtained an exclusive lock; 
- 1 byte is used to mark whether this record has been deleted or not; 
- 8 byte are used to record the number of ��readers��. 

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/DataStructure.png)

- The combination of linkId, clcId, sbp in the figure below becomes the key value of the business logic, with a size of 54 bytes.

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/KeyGen.png)

## ����ʵ��������
���Ի�ΪDell Z440 ����վ��16GB�ڴ棬8��CPU����keyΪ64λ���Ρ�valueΪ256 bytes ʱ�����Խ������
- ������װ��3�����ֵ����ʱ4�롣
- CPUѹ������£�һ���Ӵ���711,3400 �������ȡ�� ����892,7004 ��� �����¡� �����Լ�1356,6043��� ��ɾ���� ���󡣴˴������ָ���Ǵ�װ������3����Key�����ѡȡ��
- ����ʱ���ڴ��б�����3�����ֵ�Ե�������Դ������װ��Hash���Լ������ѡ�������ݡ�
- ��ɾ�����������Hash���н�ָ����Key���Ϊ����ɾ�����������¡����������ñ��Ϊ����ɾ�������ݵġ���ɾ������ǩ���������������»ָ���
- �ҵ�ʵ�ֺ�ԭ���е�ʵ���������ڣ��Ͻ���hash���value��int64���ҵ�ʵ����hash���value��256 bytes�������Ļ��������Ͼ���Buffer.BlockCopy()���˺��ȡ���ʵ��������memcpy���κ������ж���һ������Ҫ���Ż������Ͻ���key��value����int64��Ҳȷʵ�������˼��¡�
- ������CPU�ܼ��ͼ��㣬ʵ���������ܹ�����CPU Core���������Ӷ�ͬ����������

��ͬ�������£�Lock-Free Hash Table �� .Net Concurrent Dictionary ���ܶԱ�

|��������|Lock-Free Hash Table|.Net Concurrent Dictionary|���ưٷֱ�|
|:----------|----------:|----------:|----------:|
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