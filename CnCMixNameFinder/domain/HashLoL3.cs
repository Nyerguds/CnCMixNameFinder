using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.domain
{
    public class HashLoL3 : HashMethod
    {
        public override uint GetNameId(String name)
        {
            return 0;
        }

        public override string GetMethodName()
        {
            return "Unknown (Lands of Lore 3)";
        }
    }
}
