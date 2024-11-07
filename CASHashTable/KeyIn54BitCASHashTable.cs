using System;

namespace CAS
{
    public class KeyIn54BitCASHashTable : KeyIn54BitCASHashTableBase
    {
        public KeyIn54BitCASHashTable(int arrayLength, int contentLength)
            : base(arrayLength, contentLength)
        { }

        public int TrySet(long linkId, long clcId, long sbp, byte[] content)
        {
            if (linkId < 0 || linkId > 4194303) throw new ArgumentOutOfRangeException("linkId");
            if (clcId < 0 || clcId > 262143) throw new ArgumentOutOfRangeException("clcId");
            if (sbp < 0 || sbp > 16383) throw new ArgumentOutOfRangeException("sbp");
            if (linkId == 0 && clcId == 0 && sbp == 0) throw new ArgumentOutOfRangeException("KeyIsZero Error. Must > 0.");
            if (content == null) throw new ArgumentNullException("contentLength");
            if (content.Length > base.ContentLength) throw new ArgumentOutOfRangeException("contentLength");

            return base.TrySet(GenerateKey(linkId, clcId, sbp), content);
        }

        public int TryGet(long linkId, long clcId, long sbp, out byte[] output)
        {
            if (linkId < 0 || linkId > 4194303) throw new ArgumentOutOfRangeException("linkId");
            if (clcId < 0 || clcId > 262143) throw new ArgumentOutOfRangeException("clcId");
            if (sbp < 0 || sbp > 16383) throw new ArgumentOutOfRangeException("sbp");
            if (linkId == 0 && clcId == 0 && sbp == 0) throw new ArgumentOutOfRangeException("KeyIsZero Error. Must > 0.");

            output = null;
            return base.TryGet(GenerateKey(linkId, clcId, sbp), out output);
        }

        public int TryDelete(long linkId, long clcId, long sbp)
        {
            if (linkId < 0 || linkId > 4194303) throw new ArgumentOutOfRangeException("linkId");
            if (clcId < 0 || clcId > 262143) throw new ArgumentOutOfRangeException("clcId");
            if (sbp < 0 || sbp > 16383) throw new ArgumentOutOfRangeException("sbp");
            if (linkId == 0 && clcId == 0 && sbp == 0) throw new ArgumentOutOfRangeException("KeyIsZero Error. Must > 0.");

            return base.TryDelete(GenerateKey(linkId, clcId, sbp));
        }

        public long GenerateKey(long linkId, long clcId, long sbp)
        {
            return (linkId << 42) | (clcId << 24) | (sbp << 10);
        }
    }
}