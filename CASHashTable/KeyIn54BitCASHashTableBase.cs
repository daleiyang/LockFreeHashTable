using System;
using System.Threading;

namespace CAS
{
    public abstract class KeyIn54BitCASHashTableBase
    {
        private int contentLength;
        public int ContentLength
        {
            get { return contentLength; }
        }
        private struct Entity
        {
            public long compondKey;
            public byte[] content;
            public int contentLength;
        }
        private Entity[] entityArray;
        private int arrayLength;

        public KeyIn54BitCASHashTableBase(int arrayLength, int contentLength)
        {
            if (arrayLength < 1) throw new ArgumentOutOfRangeException("arrayLength");
            if (contentLength < 1) throw new ArgumentOutOfRangeException("contentLength");

            this.arrayLength = arrayLength;
            this.contentLength = contentLength;

            entityArray = new Entity[this.arrayLength];

            for (int i = 0; i < this.arrayLength; i++)
            {
                entityArray[i] = new Entity { content = new byte[this.contentLength] };
            }
        }

        public int TrySet(long key, byte[] content)
        {
            long initCompondKey = 0; // for add a new entity only
            long finalCompondKey = 0; // for add a new entity only

            GenerateKeysForAdd(key, ref initCompondKey, ref finalCompondKey);
            /*
                "Set" function could be used as "Add An New Entity",
                if hash table don't have the same key.

                Or it could be used as "Update An Existing Entity",
                if hash table has a key with the same value already.
            */
            return AddOrUpdate(Hash(key), key, initCompondKey, finalCompondKey, content);
        }

        private int AddOrUpdate(int index, long key, long initCompondKey, long finalCompondKey, byte[] content)
        {
            int i = 1;
            long targetKey = entityArray[index].compondKey;
            if (targetKey != 0 && !KeysAreEqual(key, targetKey))
            {
                do
                {
                    index = Collision(key, i++);
                    targetKey = entityArray[index].compondKey;
                } while (targetKey != 0 && !KeysAreEqual(key, targetKey));
            }

            //if (i > 1) Console.WriteLine(index + "\t" + i);

            if (targetKey != 0)
            {
                //Find the key in hash table, so the prupose is to update url.
                return Update(index, content);
            }
            else
            {
                //No matched key found and there is an empty item, so we will try to add a new entity there.
                if (Interlocked.CompareExchange(ref entityArray[index].compondKey, initCompondKey, 0) == 0)
                {
                    //Only try once, if fail, go to next "eles" clause.
                    Buffer.BlockCopy(content, 0, entityArray[index].content, 0, content.Length);
                    entityArray[index].contentLength = content.Length;
                    Interlocked.CompareExchange(ref entityArray[index].compondKey, finalCompondKey, initCompondKey);
                    //if (Interlocked.CompareExchange(ref entityArray[index].compondKey, finalCompondKey, initCompondKey) != initCompondKey)
                    //throw new CASFailureException("Set write bit back to 0 failed after a new item was inserted successfully. Need to call ResetDoWriteBit() function to make it accessible.");
                    return 0;
                }
                else
                {
                    /*
                        If we get into here, it means other thread already set "compondKey" for that empty location,
                        We have to re-check content in the same location which means recursion call happens.
                        The reson to pass the same index is to handle the same key already been set in this location and we have to update it.
                    */
                    return AddOrUpdate(index, key, initCompondKey, finalCompondKey, content);
                }
            }
        }

        private int Update(int index, byte[] content)
        {
            long comparand, value;
            do
            {
                comparand = GenerateComparandForUpdate(entityArray[index].compondKey);
                value = GenerateValueForUpdate(comparand);
                //ToDo:  value should be the same in each loop, can move out.
            } while (Interlocked.CompareExchange(ref entityArray[index].compondKey, value, comparand) != comparand);

            //We have setup doWrite bit to 1 successfully, no reader and isDelete is 0 , now it's safe to update the value.
            Buffer.BlockCopy(content, 0, entityArray[index].content, 0, content.Length);
            entityArray[index].contentLength = content.Length;

            //Set doWrite bit to 0 to allow other threads to change the compondKey.
            Interlocked.CompareExchange(ref entityArray[index].compondKey, SetWriteBitToZero(value), value);
            //if (Interlocked.CompareExchange(ref entityArray[index].compondKey, SetWriteBitToZero(value), value) != value)
            //throw new CASFailureException("Set write bit back to 0 failed after content was updated successfully. Need to call ResetDoWriteBit() function to make it accessible.");
            return 1;
        }

        public int TryGet(long key, out byte[] output)
        {
            output = null;
            int index;

            if (!HashSearch(key, out index))
                return -2;

            long comparand, originalValue;
            do
            {
                comparand = GenerateComparandBeforeSearch(entityArray[index].compondKey);
                if (CheckReaderCounterIsMaximum(comparand))
                    throw new ReaderCounterOverflowException("Reader count reach the maximun. Try to get it later.");
                originalValue = Interlocked.CompareExchange(ref entityArray[index].compondKey, comparand + 1, comparand);
                //if isDelete bit to 1 before current thread, we need to return false;
                if ((originalValue != comparand) && !CheckDeleteBitIsZero(originalValue))
                    return -3;
            } while ((originalValue != comparand) && CheckDeleteBitIsZero(originalValue));

            //We have setup readerCount = readerCount + 1 successfully, now it's safe to read the value in the URL.
            output = new byte[entityArray[index].contentLength];
            Buffer.BlockCopy(entityArray[index].content, 0, output, 0, entityArray[index].contentLength);

            do
            {
                comparand = entityArray[index].compondKey;
                //if (CheckReaderCounterIsMinimum(comparand))
                //throw new ReaderCounterOverflowException("Reader count reach the minimum. Must be a logic bug.");
            } while (Interlocked.CompareExchange(ref entityArray[index].compondKey, comparand - 1, comparand) != comparand);
            //We have setup readerCount = readerCount - 1 successfully.

            return 0;
        }

        public int TryDelete(long key)
        {
            int index;

            if (!HashSearch(key, out index))
                return -2;

            long comparand, value, originalValue;
            do
            {
                comparand = GenerateComparandForDelete(entityArray[index].compondKey);
                value = GenerateValueForDelete(comparand);
                originalValue = Interlocked.CompareExchange(ref entityArray[index].compondKey, value, comparand);
                //if other thread already set isDelete bit to 1 before current thread, we need to return false;
                if ((originalValue != comparand) && !CheckDeleteBitIsZero(originalValue))
                    return -3;
            } while ((originalValue != comparand) && CheckDeleteBitIsZero(originalValue));

            //We have set isDelete bit to 1 successfully.
            return 0;
        }

        private bool HashSearch(long key, out int index)
        {
            index = Hash(key);
            long targetKey = entityArray[index].compondKey;

            int i = 0;
            if (targetKey == 0) return false;
            else if (KeysAreEqual(key, targetKey)) return true;
            else
            {
                do
                {
                    index = Collision(key, i++);
                    targetKey = entityArray[index].compondKey;
                } while (targetKey != 0 && !KeysAreEqual(key, targetKey));

                if (targetKey == 0) return false;
                else return true;
            }
        }

        public int Hash(long key)
        {
            return (int)(Math.Abs(key) % arrayLength);
        }

        public int Collision(long key, int i)
        {
            return (Hash(key) + i) % arrayLength;
        }

        private long GenerateValueForUpdate(long compondKey)
        {
            /*
                Purpose: Set doWrite bit to 1， isDelete to 0 in comparand.

                0x200: doWrite bit are 1, other are 0.
                0xFFFFFFFFFFFFFEFF: isDelete bit is 0 other are 1.
                0xFFFFFFFFFFFFFEFF = -257.

                If the current comparand already been makked as deleted, update method will set isDelete to 0 to
                bring it back.
            */
            return (compondKey | 0x200) & -257;
        }

        private long GenerateValueForDelete(long compondKey)
        {
            /*
                Purpose: Set isDelete bit to 1 in comparand.
                0x100: isDelete bit are 1, others are 0.
           */
            return compondKey | 0x100;
        }

        private long GenerateComparandForUpdate(long compondKey)
        {
            /*
                Purpose: Set doWrite/readerCount bits to 0 for the key.
                0xFFFFFFFFFFFFFD00: doWrite bit, readerCount bit is 0, others are 1.
                0xFFFFFFFFFFFFFD00 = -768
            */
            return compondKey & -768;
        }

        private long GenerateComparandForDelete(long compondKey)
        {
            /*
                Purpose: Set doWrite/isDelete bits to 0 for the key.
                0xFFFFFFFFFFFFFCFF: doWrite bit, isDelete bits are 0, others are 1.
                0xFFFFFFFFFFFFFCFF = -769;
            */
            return compondKey & -769;
        }

        private long GenerateComparandBeforeSearch(long compondKey)
        {
            /*
              Purpose: Set doWrite bit, isDeleted bit to 0.
              0xFFFFFFFFFFFFFCFF: doWrite bit, isDeleted bit are 0, others are 1;
              0xFFFFFFFFFFFFFCFF = -769;
            */
            return compondKey & -769;
        }

        private bool CheckWriteBitIsZero(long compondKey)
        {
            /*
               Purpose: Check whether doWrite bits are 0 in the compondKey.
               0x200: doWrite bit is 1, others are 0;
           */
            return (compondKey & 0x200) == 0;
        }

        private bool CheckDeleteBitIsZero(long compondKey)
        {
            /*
               Purpose: Check whether isDelete bits is 0 in the compondKey.
               0x100: isDelete bit is 1, others are 0;
            */
            return (compondKey & 0x100) == 0;
        }

        private bool CheckReaderCounterIsMaximum(long compondKey)
        {
            /*
               Purpose: Check whether reader counter is 255 in the compondKey.
               0xFF: readerCount bit is 1, others are 0;
            */
            return (compondKey & 0xFF) == 0xFF;
        }

        private bool CheckReaderCounterIsMinimum(long compondKey)
        {
            /*
               Purpose: Check whether reader counter is 0 in the compondKey.
               0xFF: readerCount bit is 1, others are 0;
            */
            return (compondKey & 0xFF) == 0x0;
        }

        private long SetWriteBitToZero(long compondKey)
        {
            /*
               Purpose: Set doWrite bits to 0 in the compondKey.
               0xFFFFFFFFFFFFFDFF: doWrite bit is 0, others are 1;
               0xFFFFFFFFFFFFFDFF = -513;
           */
            return compondKey & -513;
        }

        public bool KeysAreEqual(long key, long targetKey)
        {
            /*
                Purpose: Compare linkid + clcid + sbp are equal in these two keys or not.
                0xFFFFFFFFFFFFFC00: doWrite/isDelete/readerCount bits are 0, others are 1;
                0xFFFFFFFFFFFFFC00 = -1024;
            */
            return key == (targetKey & -1024);
        }

        private void GenerateKeysForAdd(long key, ref long initCompondKey, ref long finalCompondKey)
        {
            long doWrite = 0x1 << 9;
            long notDoWrite = 0x0 << 9;
            long notDeleted = 0x0 << 8;
            long readerCounter = 0x0;

            initCompondKey = key + doWrite + notDeleted + readerCounter;
            //Purpose: Set doWrite bit to 0 in initCompondKey 
            finalCompondKey = key + notDoWrite + notDeleted + readerCounter;
        }
    }

    public class CASFailureException : Exception
    {
        public CASFailureException(string message) : base(message) { }
    }

    public class ReaderCounterOverflowException : Exception
    {
        public ReaderCounterOverflowException(string message) : base(message) { }
    }
}

//ToDo 最终的测试还是要根据实际情况的get/update/delete来分配压力，压力可以放大，但是比例要合乎实际情况。
//对于某一种特定的应用场景，根据实际的比例，对于算法还可以微调，优化。

//ToDo 在来回Try的CAS操作中应该加入一个阈值，当一次API call的总重试次数超过了这个阈值，要用exception或者log的方式通知，
//这样你就能够知道在这种压力情况下，实际重试次数的分布情况，有利于掌握实际的循环情况。