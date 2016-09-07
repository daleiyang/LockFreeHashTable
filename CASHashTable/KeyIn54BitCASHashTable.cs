using System;

namespace CAS
{
    public class KeyIn54BitCASHashTable: KeyIn54BitCASHashTableBase
    {
        public KeyIn54BitCASHashTable(int arrayLength, int contentLength)
            : base(arrayLength, contentLength)
        {}

        public int TrySet(long linkId, long clcId, long sbp, byte[] content)
        {
            if (linkId < 1 || linkId > 4194303) throw new ArgumentOutOfRangeException("linkId");
            if (clcId < 1 || clcId > 262143) throw new ArgumentOutOfRangeException("clcId");
            if (sbp < 1 || sbp > 16383) throw new ArgumentOutOfRangeException("sbp");
            if (content == null) throw new ArgumentNullException("contentLength");
            if (content.Length > base.ContentLength) throw new ArgumentOutOfRangeException("contentLength");

            return base.TrySet(GenerateKey(linkId, clcId, sbp), content);
        }

        public int TryGet(long linkId, long clcId, long sbp, out byte[] output)
        {
            if (linkId < 1 || linkId > 4194303) throw new ArgumentOutOfRangeException("linkId");
            if (clcId < 1 || clcId > 262143) throw new ArgumentOutOfRangeException("clcId");
            if (sbp < 1 || sbp > 16383) throw new ArgumentOutOfRangeException("sbp");
            output = null;

            return base.TryGet(GenerateKey(linkId, clcId, sbp), out output);
        }

        public int TryDelete(long linkId, long clcId, long sbp)
        {

            if (linkId < 1 || linkId > 4194303) throw new ArgumentOutOfRangeException("linkId");
            if (clcId < 1 || clcId > 262143) throw new ArgumentOutOfRangeException("clcId");
            if (sbp < 1 || sbp > 16383) throw new ArgumentOutOfRangeException("sbp");

            return base.TryDelete(GenerateKey(linkId, clcId, sbp));
        }

        public virtual long GenerateKey(long linkId, long clcId, long sbp)
        {
            return (linkId << 42) | (clcId << 24) | (sbp << 10);
        }
    }
}