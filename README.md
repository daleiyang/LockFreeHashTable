# Lock Free Hash Table / CAS Hash Table / ������ϣ��

������2015��8�·ݣ���΢�Ź��ں��Ͽ�����ʷ����ǿ�㷨��ս���벻Ҫ���������ǹ�ϣ�� http://chuansong.me/n/1489885 һ���в������Ͻ�����Ʊ����ϵͳ�ĺ������ݽṹ���㷨ʮ�������ˣ�������һ���µ�ҵ��ʱ������һ��C#��ʵ�֣������ڹ�˾ĳ��Ʒ�Ĳ��Խ׶ν���ʵ�⡣

### ����ʵ������
���Ի�ΪDell Z440 ����վ��16GB�ڴ棬8��CPU����keyΪ64λ���Ρ�valueΪ256 bytes ʱ�����Խ�����£�
- ������װ��3�����ֵ����ʱ4�롣
- CPUѹ������£�һ���Ӵ���711,3400 ��� ����ȡ�� ����892,7004 ��� �����¡� �����Լ�1356,6043��� ��ɾ���� ����
- �������ܹ�����CPU�������ӣ�ͬ����������

### ��ͬ�������£�Lock-Free Hash Table �� .Net Concurrent Dictionary ���ܶԱ�
|��������|Lock-Free Hash Table|.Net Concurrent Dictionary|���ưٷֱ�|
|:----------|----------:|----------:|----------:|
|Get|7,113,400|1,681,929|<font color="red">422.93%</font>|
|Add/Update|8,927,004|240,321|<font color="red">3714.61%</font>|
|Delete|13,566,043|245,884|<font color="red">5517.26%</font>|

���ܶԱȽ������������ο���CASHashTableĿ¼�µ�[PerfTestingResults.xlsx](https://github.com/daleiyang/LockFreeHashTable/raw/master/CASHashTable/PerfTestingResults.xlsx)

## Դ����˵����
- CASHashTable�����е�[KeyIn54BitCASHashTableBase.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTableBase.cs)�Ǻ��Ĵ��롢���ࡣ
- CASHashTable�����е�[KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs)��ʹ�û�������ӣ�ʹ������Ҫ�ṩTrySet��TryGet��TryDelete��GenerateKey��ʵ�֡������Ҫ�����룬���Դ��������֡�
- Test���Թ����е�[KeyIn54BitCASHashTableFunctionalTest.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTableFunctionalTest.cs)�ǹ��ܲ��ԣ�������������µ���ɾ�Ĳ����ȷ����֤��
- Test���Թ����е�[KeyIn54BitCASHashTablePerfTest.cs ](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTablePerfTest.cs)��ѹ�����ԣ����Է�����ͬʱʹ��30������ѹ�⣬����10�����̽����������ȡ��������10�����̽��������ɾ����������10�����̽�����������/���¡�������������ÿ�ֲ������е��ܴ����Ǿ��ĵ������ģ���֤��30�������ܹ���ͬʱ�����������ܹ��õ�һ��������Ľ����

## ʹ�÷�����
- Utility.cs �л�ȡ���ݵ�SQL�ֶΣ������������key/value���ԡ�
- �޸�KeyIn54BitCASHashTable.cs���ṩ���Լ���TrySet��TryGet��TryDelete�� GenerateKey������
- ��Test���Թ����еĹ��ܲ����ļ�[KeyIn54BitCASHashTableFunctionalTest.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTableFunctionalTest.cs)������ȷ�ԡ�