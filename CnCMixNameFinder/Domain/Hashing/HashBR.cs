using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    internal class HashBR : HashMethod
    {
        public override UInt32 GetNameId(String name, Boolean assumeCorrectCase)
        {
            if (!assumeCorrectCase)
                name = name.ToUpperInvariant();
            return 0;
        }

        public override String GetMethodName()
        {
            return "Unknown (Blade Runner)";
        }
    }
}
