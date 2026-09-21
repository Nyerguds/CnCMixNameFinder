using Misc.Blowfish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LarchenkoCRC32;

namespace CnCMixNameFinder.Domain
{
    public class HashCRC32 : HashMethod
    {
        public override UInt32 GetNameId(String name)
        {
            // Assume all input is upper case. This will save time.
            //name = name.ToUpperInvariant();
            Int32 l = name.Length;
            Int32 a = l >> 2;
            if ((l & 3) != 0)
            {
                name += (Char)(l - (a << 2));
                Int32 i = 3 - (l & 3);
                while (i-- != 0)
                    name += name[a << 2];
            }
            return ParallelCRC.Compute(new ArraySegment<Byte>(Encoding.ASCII.GetBytes(name)));
            //return CRC32.Calculate(Encoding.ASCII.GetBytes(name));
        }

        public override String GetMethodName()
        {
            return "CRC32 (TS/RA2)";
        }
    }
}
