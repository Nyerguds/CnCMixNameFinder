using System;
using System.Collections.Generic;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    public class HashROL : HashMethod
    {
        public override UInt32 GetNameId(String name, Boolean assumeCorrectCase)
        {
            if (!assumeCorrectCase)
                name = name.ToUpperInvariant();
            Byte[] values = Encoding.ASCII.GetBytes(name);
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
                id = BitFunctions.RotateLeft(id,1) + a;
            }
            return id;
        }

        public override String GetMethodName()
        {
            return "ROL (TD/RA)";
        }
    }
}
