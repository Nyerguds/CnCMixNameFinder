using System;

namespace CnCMixNameFinder.Domain
{
    public class BitFunctions
    {
        public static UInt32 RotateLeft(UInt32 value, Int32 count)
        {
            return (value << count) | (value >> (32 - count));
        }

        public static UInt32 RotateRight(UInt32 value, Int32 count)
        {
            return (value >> count) | (value << (32 - count));
        }
    }
}
