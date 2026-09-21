using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LarchenkoCRC32;

namespace CnCMixNameFinder.Domain
{
    public class HashCrc32 : HashMethod
    {
        public override UInt32 GetNameIdCorrectCase(String name)
        {
            Byte[] data = Encoding.ASCII.GetBytes(name);
            return GetNameIdCorrectCase(data);
        }

        /*
        public override UInt32 GetNameIdCorrectCase(String name)
        {
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
        }
        */

        public override UInt32 GetNameIdCorrectCase(Byte[] data)
        {
            int l1 = data.Length;
            // Fill buffer up to next multiple of 4 bytes
            if ((l1 & 3) != 0)
            {
                Int32 l2 = (l1 + 3) & ~3;
                Byte[] data2 = new Byte[l2];
                Array.Copy(data, 0, data2, 0, l1);
                Int32 a = l1 >> 2;
                data2[l1] = (byte)(l1 - (a << 2));
                for (int i = l1 + 1; i < l2; i++)
                {
                    data2[i] = data2[a << 2];
                }
                data = data2;
            }
            return ParallelCRC.Compute(new ArraySegment<Byte>(data));
        }


        public override String GetDisplayName()
        {
            return "CRC32 (TS/RA2)";
        }

        public override String GetSimpleName()
        {
            return "CRC32";
        }
    }
}
