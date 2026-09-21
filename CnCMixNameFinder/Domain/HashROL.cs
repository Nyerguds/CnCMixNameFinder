using System;
using System.Collections.Generic;
using System.Text;
using Misc.Blowfish;

namespace CnCMixNameFinder.Domain
{
    public class HashROL : HashMethod
    {
        public override UInt32 GetNameId(String name)
        {
            Byte[] values = Encoding.ASCII.GetBytes(name.ToUpperInvariant());
            Int32 i = 0;
            UInt32 id = 0;
            Int32 l = values.Length;          // length of the filename
            while (i < l)
            {
                UInt32 a = 0;
                for (Int32 j = 0; j < 4; j++)
                {
                    a >>= 8;
                    if (i < l)
                        a += ((UInt32)values[i] << 24);
                    i++;
                }
                id = (id << 1 | id >> 31) + a;
            }
            return id;
        }

        public override String GetMethodName()
        {
            return "ROL (TD/RA)";
        }
    }
}
