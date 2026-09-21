using System;

namespace CnCMixNameFinder.Domain
{
    public class HashObfuscate : HashMethod
    {
        public override UInt32 GetNameIdCorrectCase(String name)
        {
            throw new NotImplementedException();
        }

        public override String GetMethodName()
        {
            return "Obsfuscate (hidden options)";
        }
    }
}