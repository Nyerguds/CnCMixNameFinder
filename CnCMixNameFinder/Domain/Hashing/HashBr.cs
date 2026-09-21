using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    internal class HashBr : HashMethod
    {
        public override UInt32 GetNameIdCorrectCase(String name)
        {
            return 0;
        }

        public override UInt32 GetNameIdCorrectCase(Byte[] data)
        {
            return 0;
        }

        public override String GetDisplayName()
        {
            return "Unknown (Blade Runner)";
        }

        public override String GetSimpleName()
        {
            return "Blade Runner";
        }
    }
}
