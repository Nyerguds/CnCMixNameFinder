using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    public abstract class HashMethod
    {
        /// <summary>
        /// The hashing method. The "assumeCorrectCase" arg is an optimization for brute-force;
        /// if the hash method needs uppercase text, this can be done with the original input data
        /// instead of having to apply it to each string during the brute force operation itself.
        /// </summary>
        /// <param name="name">String to hash.</param>
        /// <param name="assumeCorrectCase">Assume input is in the case specified by the hashing method's NeedsUpperCase property.</param>
        /// <returns>The hashed value.</returns>
        public abstract UInt32 GetNameId(String name, Boolean assumeCorrectCase);
        
        /// <summary>
        /// Returns the name of the hashing method.
        /// </summary>
        /// <returns>The name of the hashing method.</returns>
        public abstract String GetMethodName();

        /// <summary>
        /// Allows supporting methods that are not case insensitive.
        /// </summary>
        public virtual Boolean NeedsUpperCase
        {
            get { return true; }
        }

        public UInt32 GetNameId(String name)
        {
            if (name == null)
                name = String.Empty;
            name = name.Trim();
            return GetNameId(this.NeedsUpperCase ? name.ToUpperInvariant() : name, true);
        }

        public String GetNameIdHexString(String name)
        {
            return this.GetNameId(name).ToString("X4").PadLeft(8, '0');
        }

        public String GetNameIdHexString(String name, Boolean assumeUppercase)
        {
            return this.GetNameId(name, assumeUppercase).ToString("X4").PadLeft(8, '0');
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
                new HashROR(),     // Lands of Lore 3
                new HashObscure(), // setup mix files
                //new HashBR(),      // Blade Runner (not implemented)
            };
        }


    }
}
