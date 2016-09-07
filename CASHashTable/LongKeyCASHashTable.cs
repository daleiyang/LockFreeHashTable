using System;
using System.Security.Cryptography;
using System.Text;

namespace CAS
{
    public class LongKeyCASHashTable: LongKeyCASHashTableBase
    {
        public LongKeyCASHashTable(int arrayLength, int keyByteArrayLength, int contentByteArrayLength)
            : base(arrayLength, keyByteArrayLength, contentByteArrayLength)
        { }

        public int TrySet(string key, byte[] value)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (key.Length > base.KeyByteArrayLength) throw new ArgumentOutOfRangeException("Key length shouldn't longer than init value when you create hash table.");
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length > base.ContentByteArrayLength) throw new ArgumentOutOfRangeException("Content length shouldn't longer than init value when you create hash table.");
            return base.TrySet(key.GetHashCode(), Encoding.UTF8.GetBytes(key), value);
        }

        public int TryGet(string key, out byte[] output)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (key.Length > base.KeyByteArrayLength) throw new ArgumentOutOfRangeException("Key length shouldn't longer than init value when you create hash table.");
            output = null;
            return base.TryGet(key.GetHashCode(), Encoding.UTF8.GetBytes(key), out output);
        }

        public int TryDelete(string key)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (key.Length > base.KeyByteArrayLength) throw new ArgumentOutOfRangeException("Key length shouldn't longer than init value when you create hash table.");
            return base.TryDelete(key.GetHashCode(), Encoding.UTF8.GetBytes(key));
        }
    }
}