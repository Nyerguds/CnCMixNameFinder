using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    public class HashROR : HashMethod
    {
        public override UInt32 GetNameId(String name, Boolean assumeCorrectCase)
        {
            if (!assumeCorrectCase)
                name = name.ToUpperInvariant();
            UInt32 id = 0;
            for (Int32 i = 0; i < name.Length; i++)
            {
                UInt32 rotatedValue = BitFunctions.RotateRight(id, 6);
                id = (UInt32)((name[i] - 48) & 63) + rotatedValue;
            }
            return id;
        }

        public UInt32 GetNameId2(String name, Boolean assumeCorrectCase)
        {
            return 0;
        }

        public override String GetMethodName()
        {
            return "ROR (Lands of Lore 3)";
        }
    }
}