using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.domain
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
    }
}
