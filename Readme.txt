Lock Free Hash Table / CAS Hash Table / ������ϣ��

2015��8�·ݣ���΢�Ź��ں��Ͽ�����ʷ����ǿ�㷨��ս���벻Ҫ���������ǹ�ϣ�� http://chuansong.me/n/1489885
һ���в������Ͻ�����Ʊ����ϵͳ�ĺ������ݽṹ���㷨ʮ�������ˣ�������һ���µ�ҵ��ʱ������һ��C#��ʵ�֣�����
˳����Ӧ�õ���˾��ĳ��Ʒ�Ĳ��Խ׶Ρ�

����ʵ���������£�
When key is a Int64 integer and value is 256 bytes, tested on a DELL Z440 work station with 16 GB memory and 8 core CPU:
a.	Load 3,000,000 key value pairs one by one into memory in 4 seconds with one thread.
b.	Process 7,113,400 random get requests / 8,927,000 random update requests / 13,566,040 random delete requests in 1 second simultaneously.
c.	Its capability should be able to increases in a linear manner along with CPU core quantity increasing

����˵����
CASHashTable�������Ǻ��Ĵ��롣
Test�ǲ��Թ��̡�

ѹ�����Էֱ���������key�����ܺ͹̶�һ��key����ɾ�Ĳ����ܡ����Խ�����֤�������key����ɾ�Ĳ�����Զ����.Net�ṩ��Concurrent Dictionary��
�̶�key����ɾ�Ĳ������������ȡ�����ѹ�����Խ�����ο���CASHashTable/PerfTestingResults.xlsx

�����ʵ�⡢ʹ�ã���Ҫ�޸ģ�
1. Utility.cs �л�ȡ���ݵ�SQL�ֶΣ������������key/value���ԡ�
2. �޸�KeyIn54BitCASHashTable.cs���ṩ���Լ���TrySet��TryGet��TryDelete�� GenerateKey������