using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    internal class HashBR : HashMethod
    {
        public override uint GetNameId(string name)
        {
            return 0;
        }

        public override string GetMethodName()
        {
            return "Unknown (Blade Runner)";
        }
    }
}
