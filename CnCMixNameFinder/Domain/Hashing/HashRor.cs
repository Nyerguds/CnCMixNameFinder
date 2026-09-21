using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    public class HashRor : HashMethod
    {

        public override UInt32 GetNameIdCorrectCase(Byte[] data)
        {
            UInt32 id = 0;
            for (Int32 i = 0; i < data.Length; i++)
            {
                UInt32 rotatedValue = BitFunctions.RotateRight(id, 6);
                id = (UInt32)((data[i] - 48) & 63) + rotatedValue;
            }
            return id;
        }

        public override UInt32 GetNameIdCorrectCase(String name)
        {
            UInt32 id = 0;
            for (Int32 i = 0; i < name.Length; i++)
            {
                UInt32 rotatedValue = BitFunctions.RotateRight(id, 6);
                id = (UInt32)((name[i] - 48) & 63) + rotatedValue;
            }
            return id;
        }

        public override String GetDisplayName()
        {
            return "ROR (Lands of Lore 3)";
        }

        public override String GetSimpleName()
        {
            return "ROR";
        }
    }
}