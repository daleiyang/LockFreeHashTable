Data structure

struct Entity
{
    /*      
        linkId         0  ~ 21 = 22 bit
        clcId          22 ~ 39 = 18 bit
        sbp            40 ~ 53 = 14 bit
        doWrite        54      = 1 bit
        isDelete       55      = 1 bit
        readerCounter  56 ~ 63 = 8 bit
    */
    public long key;
    public byte[] url;  //256 bytes
}

linkId			: 0 ~ 4,194,303
clcId			: 0 ~   262,143
sbp				: 0 ~    16,383
doWrite			: 0 ~ 1
isDelete		: 0 ~ 1
readerCounter	: 0 ~ 255

linkId，clcId，sbp 组成哈希表的key

Algorithm

Hash表设计：

目前（20150724），linkId，clcId，sbp 三者共有 3,040,000 种不同组合。
Hash表选择 6,080,023 这个质数作为长度，这样装填因子是 3,080,000 / 6,080,023 = 0.5
在使用线性探测再散列方法时，查找成功时的平均查找长度的数学期望是 (1+1/(1-0.5))/2 = 1.5
查找不成功时的数学期望是 (1+1/(1-0.5)^2)/2 = 2.5

http://www.primos.mat.br/primeiros_10000_primos.txt

Hash表初始化：

1. new Entity[6080023]
2. key = 0
3. url = new byte[256]，写每一字节写 0x00


Hash函数：

key % 6080023


Hash冲突(线性探测再散列)：

(Hash(key) + i) / 6080023;  其中i为冲突的次数。


Hash添加和更新：

Set(), 返回值是新添加的或者更新的数组下标。具体逻辑参考代码中注释。
更新时会将isDelete标记为0，也就是通过Set函数，可以将已经标记为"已删除"
的重新enable.

Hash查找：

Get(), 返回值是URL，或者null (当Hash表中没有查找项时)。具体逻辑参考代码中注释。