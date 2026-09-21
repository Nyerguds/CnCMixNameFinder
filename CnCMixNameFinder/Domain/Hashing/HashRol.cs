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
        
        public override UInt32 GetNameIdCorrectCase(Byte[] data)
        {
            return GetNameId(data, 1);
        }

        public override String GetDisplayName()
        {
            return "ROL (TD/RA)";
        }

        public override String GetSimpleName()
        {
            return "ROL";
        }
    }


    public class HashRol3 : HashRol
    {
        public override UInt32 GetNameIdCorrectCase(String name)
        {
            return GetNameId(name, 3);
        }

        public override UInt32 GetNameIdCorrectCase(Byte[] data)
        {
            return GetNameId(data, 3);
        }

        public override String GetDisplayName()
        {
            return "ROL3 (setup TS/RA2/...)";
        }

        public override String GetSimpleName()
        {
            return "ROL3";
        }
    }

    public abstract class HashRol : HashMethod
    {

        protected UInt32 GetNameId(String name, Int32 rot)
        {
            return GetNameId(Encoding.ASCII.GetBytes(name), rot);
        }
        
        protected UInt32 GetNameId(Byte[] values, Int32 rot)
        {
            Int32 i = 0;
            UInt32 id = 0;
            // length of the filename
            Int32 len = values.Length;
            while (i < len)
            {
                // get next uint32 chunk
                UInt32 buffer = this.GetUInt32FromBuffer(values, len, ref i);
                id = BitFunctions.RotateLeft(id, i <= len ? rot : 1) + buffer;
            }
            return id;
        }

        protected UInt32 GetUInt32FromBuffer(Byte[] values, Int32 length, ref Int32 index)
        {
            UInt32 a = 0;
            for (Int32 i = 0; i < 4; ++i)
            {
                a >>= 8;
                if (index < length)
                    a += ((UInt32)values[index] << 24);
                index++;
            }
            return a;
        }

    }
}
