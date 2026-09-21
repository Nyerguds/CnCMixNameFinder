using System;
using System.Collections.Generic;
using System.Text;

namespace CnCMixNameFinder.Domain
{


    public class HashRol1 : HashRol
    {
        public override UInt32 GetNameIdCorrectCase(String name)
        {
            return GetNameId(name, 1);
        }
        public override String GetMethodName()
        {
            return "ROL (TD/RA)";
        }
    }


    public class HashRol3 : HashRol
    {
        public override UInt32 GetNameIdCorrectCase(String name)
        {
            return GetNameId(name, 3);
        }

        public override String GetMethodName()
        {
            return "ROL3 (setup TS/RA2/...)";
        }
    }

    public abstract class HashRol : HashMethod
    {

        protected UInt32 GetNameId(String name, Int32 rot)
        {
            Byte[] values = Encoding.ASCII.GetBytes(name);
            Int32 i = 0;
            UInt32 id = 0;
            Int32 l = values.Length;          // length of the filename
            while (i < l)
            {
                // get next uint32 chunk
                UInt32 buffer = this.GetUInt32FromBuffer(values, l, ref i);
                if (i <= l)
                    id = BitFunctions.RotateLeft(id, rot) + buffer;
                else
                    id = BitFunctions.RotateLeft(id, 1) + buffer;
            }
            return id;
        }

        protected UInt32 GetUInt32FromBuffer(Byte[] values, int length, ref Int32 i)
        {
            UInt32 a = 0;
            for (Int32 j = 0; j < 4; j++)
            {
                a >>= 8;
                if (i < length)
                    a += ((UInt32)values[i] << 24);
                i++;
            }
            return a;
        }

    }
}
