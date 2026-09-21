using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    public abstract class HashMethod
    {
        public abstract UInt32 GetNameId(String name);
        public abstract String GetMethodName();

        public String GetNameIdHexString(String name)
        {
            return this.GetNameId(name).ToString("X4").PadLeft(8, '0');
        }

        public override String ToString()
        {
            return GetMethodName();
        }

        public static HashMethod[] GetRegisteredMethods()
        {
            return new HashMethod[] {
                new HashROL(),     // TD/RA
                new HashCRC32(),   // TS/RA2
                new HashObscure(), // setup mix files
                //new HashBR(),      // Blade Runner (not implemented)
                //new HashLoL3()     // Lands of Lore 3 (not implemented)
            };
        }


    }
}
