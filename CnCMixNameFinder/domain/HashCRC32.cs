using Misc.Blowfish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.domain
{
    class HashCRC32 : HashMethod
    {
        public override UInt32 GetNameId(String name)
        {
            name = name.ToUpperInvariant();
            var l = name.Length;
            var a = l >> 2;
            if ((l & 3) != 0)
            {
                name += (char)(l - (a << 2));
                var i = 3 - (l & 3);
                while (i-- != 0)
                    name += name[a << 2];
            }

            return CRC32.Calculate(Encoding.ASCII.GetBytes(name));
        }

        public override string GetMethodName()
        {
            return "CRC32 (TS/RA2)";
        }
    }
}
