using System;
using System.Threading;
using log4net;
using System.Runtime.InteropServices;

namespace CAS
{
    public abstract class LongKeyCASHashTableBase
    {
        private ILog log = LogFactory.Configure();
        private struct Entity
        {
            public long flag;
            public byte[] key;
            public byte[] content;
            public int keyLength;
            public int contentLength;
            public bool occupy;
        }
        private Entity[] entityArray;
        private int arrayLength;
        private static UIntPtr forMemcmp = new UIntPtr(32);

        private int keyByteArrayLength;
        public int KeyByteArrayLength
        {
            get { return keyByteArrayLength; }
        }

        private int contentByteArrayLength;
        public int ContentByteArrayLength
        {
            get { return contentByteArrayLength; }
        }

        public LongKeyCASHashTableBase(int arrayLength, int KeyByteArrayLength, int contentByteArrayLength)
        {
            if (arrayLength < 1) throw new ArgumentOutOfRangeException("arrayLength");
            if (contentByteArrayLength < 1) throw new ArgumentOutOfRangeException("contentByteArrayLength");
            if (KeyByteArrayLength < 1) throw new ArgumentOutOfRangeException("KeyByteArrayLength");

            this.arrayLength = arrayLength;
            this.keyByteArrayLength = KeyByteArrayLength;
            this.contentByteArrayLength = contentByteArrayLength;

            entityArray = new Entity[this.arrayLength];

            for (int i = 0; i < this.arrayLength; i++)
            {
                entityArray[i] = new Entity
                {
                    flag = 0,
                    key = new byte[keyByteArrayLength],
                    content = new byte[contentByteArrayLength],
                    contentLength = 0,
                    occupy = false
                };
            }
        }

        public int TrySet(int keyHashCode, byte[] key, byte[] content)
        {
            var _keyHashCode = Math.Abs(keyHashCode);
            try
            {
                long initCompondKey = 0; // for add a new entity only
                long finalCompondKey = 0; // for add a new entity only

                GenerateKeysForAdd(0, ref initCompondKey, ref finalCompondKey);
                /*
                    "Set" function could be used as "Add An New Entity",
                    if hash table don't have the same key.

                    Or it could be used as "Update An Existing Entity",
                    if hash table has a key with the same value already.
                */
                return AddOrUpdate(Hash(_keyHashCode), key, _keyHashCode, initCompondKey, finalCompondKey, content);
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                return -4;
            }
        }

        private int AddOrUpdate(int index, byte[] key, int keyHashCode, long initCompondKey, long finalCompondKey, byte[] content)
        {
            int i = 1;
            if (entityArray[index].occupy && !KeysAreEqual(key, entityArray[index].key, entityArray[index].keyLength))
            {
                do
                {
                    index = Collision(keyHashCode, i++);
                } while (entityArray[index].occupy && !KeysAreEqual(key, entityArray[index].key, entityArray[index].keyLength));
            }

            //if (i > 1) Console.WriteLine(index + "\t" + i);

            if (entityArray[index].occupy)
            {
                //Find the key in hash table, so the prupose is to update url.
                return Update(index, content);
            }
            else
            {
                //No matched key found and there is an empty item, so we will try to add a new entity there.
                if (Interlocked.CompareExchange(ref entityArray[index].flag, initCompondKey, 0) == 0)
                {
                    //Only try once, if fail, go to next "eles" clause.
                    Buffer.BlockCopy(key, 0, entityArray[index].key, 0, key.Length);
                    entityArray[index].keyLength = key.Length;
                    Buffer.BlockCopy(content, 0, entityArray[index].content, 0, content.Length);
                    entityArray[index].contentLength = content.Length;
                    entityArray[index].occupy = true;

                    Interlocked.CompareExchange(ref entityArray[index].flag, finalCompondKey, initCompondKey);
                    return 0;
                }
                else
                {
                    /*
                        If we get into here, it means other thread already set "compondKey" for that empty location,
                        We have to re-check content in the same location which means recursion call happens.
                        The reson to pass the same index is to handle the same key already been set in this location and we have to update it.
                    */
                    return AddOrUpdate(index, key, keyHashCode, initCompondKey, finalCompondKey, content);
                }
            }
        }

        private int Update(int index, byte[] content)
        {
            long comparand, value;
            do
            {
                comparand = GenerateComparandForUpdate(entityArray[index].flag);
                value = GenerateValueForUpdate(comparand);
            } while (Interlocked.CompareExchange(ref entityArray[index].flag, value, comparand) != comparand);

            //We have setup doWrite bit to 1 successfully, no reader and isDelete is 0 , now it's safe to update the value.
            Buffer.BlockCopy(content, 0, entityArray[index].content, 0, content.Length);
            entityArray[index].contentLength = content.Length;

            //Set doWrite bit to 0 to allow other threads to change the compondKey.
            Interlocked.CompareExchange(ref entityArray[index].flag, SetWriteBitToZero(value), value);
            return 1;
        }

        public int TryGet(int keyHashCode, byte[] key, out byte[] output)
        {
            var _keyHashCode = Math.Abs(keyHashCode);
            output = null;
            int index;

            if (!HashSearch(_keyHashCode, key, out index))
                return -2;

            try
            {
                long comparand, originalValue;
                do
                {
                    comparand = GenerateComparandBeforeSearch(entityArray[index].flag);
                    originalValue = Interlocked.CompareExchange(ref entityArray[index].flag, comparand + 1, comparand);
                    //if isDelete bit to 1 before current thread, we need to return false;
                    if ((originalValue != comparand) && !CheckDeleteBitIsZero(originalValue))
                        return -3;
                } while ((originalValue != comparand) && CheckDeleteBitIsZero(originalValue));

                //We have setup readerCount = readerCount + 1 successfully, now it's safe to read the value in the URL.
                output = new byte[entityArray[index].contentLength];
                Buffer.BlockCopy(entityArray[index].content, 0, output, 0, entityArray[index].contentLength);

                do
                {
                    comparand = entityArray[index].flag;
                } while (Interlocked.CompareExchange(ref entityArray[index].flag, comparand - 1, comparand) != comparand);
                //We have setup readerCount = readerCount - 1 successfully.

                return 0;
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                return -4;
            }
        }

        public int TryDelete(int keyHashCode, byte[] key)
        {
            var _keyHashCode = Math.Abs(keyHashCode);
            int index;

            if (!HashSearch(_keyHashCode, key, out index))
                return -2;

            try
            {
                long comparand, value, originalValue;
                do
                {
                    comparand = GenerateComparandForDelete(entityArray[index].flag);
                    value = GenerateValueForDelete(comparand);
                    originalValue = Interlocked.CompareExchange(ref entityArray[index].flag, value, comparand);
                    //if other thread already set isDelete bit to 1 before current thread, we need to return false;
                    if ((originalValue != comparand) && !CheckDeleteBitIsZero(originalValue))
                        return -3;
                } while ((originalValue != comparand) && CheckDeleteBitIsZero(originalValue));

                //We have set isDelete bit to 1 successfully.
                return 0;
            }
            catch (Exception ex)
            {
                if (log.IsFatalEnabled) log.Fatal(ex.Message + "\t" + ex.StackTrace);
                return -4;
            }
        }

        private bool HashSearch(int keyHashCode, byte[] key, out int index)
        {
            index = Hash(keyHashCode);

            int i = 1;
            if (!entityArray[index].occupy) return false;
            else if (KeysAreEqual(key, entityArray[index].key, entityArray[index].keyLength)) return true;
            else
            {
                do
                {
                    index = Collision(keyHashCode, i++);
                } while (entityArray[index].occupy && !KeysAreEqual(key, entityArray[index].key, entityArray[index].keyLength));

                if (!entityArray[index].occupy) return false;
                else return true;
            }
        }

        public int Hash(int keyHashCode)
        {
            return keyHashCode % arrayLength;
        }

        public int Collision(int keyHashCode, int i)
        {
            return (Hash(keyHashCode) + i) % arrayLength;
        }

        private long GenerateValueForUpdate(long compondKey)
        {
            /*
                Purpose: Set doWrite bit to 1， isDelete to 0 in comparand.

                0x200: doWrite bit are 1, other are 0.
                0x7FFFFFFFFFFFFEFF: first bit is 0 ,isDelete bit is 0.

                If the current comparand already been makked as deleted, update method will set isDelete to 0 to
                bring it back.
            */
            return (compondKey | 0x200) & 0x7FFFFFFFFFFFFEFF;
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
                Purpose: Set first/doWrite/readerCount bits to 0 for the key.
                0x7FFFFFFFFFFFFD00: first bit, doWrite bit, readerCount bit is 0, others are 1.
                = 9223372036854775040
            */
            return compondKey & 0x7FFFFFFFFFFFFD00;
        }

        private long GenerateComparandForDelete(long compondKey)
        {
            /*
                Purpose: Set first/doWrite/isDelete bits to 0 for the key.
                0x7FFFFFFFFFFFFCFF: first bit, doWrite bit, others are 1.
            */
            return compondKey & 0x7FFFFFFFFFFFFCFF;
        }

        private long GenerateComparandBeforeSearch(long compondKey)
        {
            /*
              Purpose: Set first bit, doWrite bit, isDeleted bit to 0.
              0x7FFFFFFFFFFFFCFF: first bit, doWrite bit isDeleted bit are 0, others are 1;
            */
            return compondKey & 0x7FFFFFFFFFFFFCFF;
        }

        private bool CheckDeleteBitIsZero(long compondKey)
        {
            /*
               Purpose: Check whether isDelete bits is 0 in the compondKey.
               0x100: isDelete bit is 1, others are 0;
            */
            return (compondKey & 0x100) == 0;
        }

        private long SetWriteBitToZero(long compondKey)
        {
            /*
               Purpose: Set doWrite bits to 0 in the compondKey.
               0x7FFFFFFFFFFFFDFF: first bit is 0, doWrite bit is 0, others are 1;
           */
            return compondKey & 0x7FFFFFFFFFFFFDFF;
        }

        public bool KeysAreEqual(byte[] key, byte[] targetKey, int targetKeyLength)
        {
            if (key.Length != targetKeyLength) return false;
            if(memcmp(key, targetKey, new UIntPtr(Convert.ToUInt32(key.Length))) == 0) return true;
            return false;
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

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern int memcmp(byte[] b1, byte[] b2, UIntPtr count);
    }
}