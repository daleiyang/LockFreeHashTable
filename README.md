# Lock Free Hash Table / CAS Hash Table / ������ϣ��

### ����
2015��8�·ݣ���΢�Ź��ںš���������ժ���Ͽ�����ʷ����ǿ�㷨��ս���벻Ҫ���������ǹ�ϣ�� http://chuansong.me/n/1489885 һ���в������Ͻ�����Ʊ����ϵͳ�ĺ������ݽṹ���㷨ʮ�������ˣ�������һ���µ�ҵ��ʱ������һ��C#��ʵ�֣������ڹ�˾ĳ��Ʒ�Ĳ��Խ׶ν���ʵ�⡣

ϣ���ҵ����ʵ���ܹ�����λ���ڵ��ճ����������������������л֮�飬���л��֪��Ƽ����ġ������ϡ���û�����ķ�����û���ҵ���һ��ʵ�֡�

### ����ʵ����
���Ի�ΪDell Z440 ����վ��16GB�ڴ棬8��CPU����keyΪ64λ���Ρ�valueΪ256 bytes ʱ�����Խ�����£�
- ������װ��3�����ֵ����ʱ4�롣
- CPUѹ������£�һ���Ӵ���711,3400 ��� ����ȡ�� ����892,7004 ��� �����¡� �����Լ�1356,6043��� ��ɾ���� ���󡣴˴������ָ���Ǵ�װ������3����Key�����ѡȡ��
- �������ܹ�����CPU�������ӣ�ͬ����������

��ͬ�������£�Lock-Free Hash Table �� .Net Concurrent Dictionary ���ܶԱ�
|��������|Lock-Free Hash Table|.Net Concurrent Dictionary|���ưٷֱ�|
|:----------|----------:|----------:|----------:|
|Get|7,113,400|1,681,929|<font color="red">422.93%</font>|
|Add/Update|8,927,004|240,321|<font color="red">3714.61%</font>|
|Delete|13,566,043|245,884|<font color="red">5517.26%</font>|

### ���ܶԱȽ����ѹ�����Ա���[PerfTestingResults.xlsx](https://github.com/daleiyang/LockFreeHashTable/raw/master/CASHashTable/PerfTestingResults.xlsx)���

#### �����ȡ���ݵķ�ʽ���в��ԣ�ͬʱ10�����̡���ȡ��������10�����̡����/���¡�������10�����̡�ɾ��������
10������ȡ�����̲��Խ��

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

10�������¡����̲��Խ��

![alt tag](https://github.com/daleiyang/LockFreeHashTable/raw/master/Update%20Random.jpg)
ÿ�к���:
- Update Attemps�����ԡ����¡����ܴ�����ÿ�ε���ʱ��װ�ص�3����ļ�ֵ�����ѡ��һ����
- API Call Elapsed Time������TrySet��������������ʱ�䣬�ų��������������߼���ʱ�����ġ�
- Update RPS/Thread��ÿ��ִ�е�TrySet�������ô�����
- Update API Call Elapsed Time/100,000,000 Attemps��ÿ1�ڴ�TrySet���������ĵ�ʱ�䡣
- Update Successfully�����³ɹ��Ĵ�����
- Test Elapsed Time����������ʱ��

10����ɾ�������̲��Խ��

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

#### ��30�����̵����ֲ�ͬ��������ȡ/����/ɾ����ͬʱ����ͬһ�����ݵļ����������

### Դ����˵����
- CASHashTable�����е�[KeyIn54BitCASHashTableBase.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTableBase.cs)�Ǻ��Ĵ��롢���࣬�������ṩ����ϸ��ע�ͣ�������ÿ��λ�����ԭ����Ӧ��ͬ����(set/update,get,delete)ʱCAS����Ӧ�ó��ֵ�λ�ú�ԭ��

- CASHashTable�����е�[KeyIn54BitCASHashTable.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/CASHashTable/KeyIn54BitCASHashTable.cs)��ʹ�û�������ӣ�ʹ������Ҫ�ṩTrySet��TryGet��TryDelete��GenerateKey��ʵ�֡������Ҫ�����룬���Դ��������֡�

- Test���Թ����е�[KeyIn54BitCASHashTableFunctionalTest.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTableFunctionalTest.cs)�ǹ��ܲ��ԣ�������������µ���ɾ�Ĳ����ȷ����֤��

- Test���Թ����е�[KeyIn54BitCASHashTablePerfTest.cs ](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTablePerfTest.cs)��ѹ�����ԣ����Է�����ͬʱʹ��30������ѹ�⣬����10�����̽����������ȡ��������10�����̽��������ɾ����������10�����̽�����������/���¡�������������ÿ�ֲ������е��ܴ����Ǿ��ĵ������ģ���֤��30�������ܹ���ͬʱ�����������ܹ��õ�һ��������Ľ����

- �����е�λ�����߼��������߼������˴��������Ĳ��ԣ�Ӧ��û��Bug������㷢���ˣ��뷢pull request��

- �����߼���ȷ�Ķ����ǻ��ڴ������Զ�û�з�������ľ��飬�������ϸ��֤�������ڶ���̡��߲������ϸ���Է���������ѧϰ�о�Paxos�㷨ʱ���ֿ��Բ���[Leslie Lamport](http://www.lamport.org/)����ķ��������Ȱ��㷨��[PlusCal](http://lamport.azurewebsites.net/tla/tla.html)���Խ���������Ȼ����[TLA+](http://lamport.azurewebsites.net/tla/tla.html)�������ʱ��Ĳ��ԡ�Leslie��[Specifying Systems](https://www.microsoft.com/en-us/research/publication/specifying-systems-the-tla-language-and-tools-for-hardware-and-software-engineers/?from=http%3A%2F%2Fresearch.microsoft.com%2Fusers%2Flamport%2Ftla%2Fbook.html)�Ȿ����ϸ���������ַ������Ҷ���ǰ9�¡�Ŀǰ��û��ʵʩ���ֲ��ԡ�

### ʹ�÷�����
- Utility.cs �л�ȡ���ݵ�SQL�ֶΣ������������key/value���ԡ�

- �޸�KeyIn54BitCASHashTable.cs���ṩ���Լ���TrySet��TryGet��TryDelete�� GenerateKey������

- ��Test���Թ����еĹ��ܲ����ļ�[KeyIn54BitCASHashTableFunctionalTest.cs](https://github.com/daleiyang/LockFreeHashTable/blob/master/Test/KeyIn54BitCASHashTableFunctionalTest.cs)������ȷ�ԡ�