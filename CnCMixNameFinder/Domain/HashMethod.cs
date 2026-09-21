using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    public abstract class HashMethod
    {
        private static HashMethod[] registeredMethods = {
                new HashRol1(),        // TD/RA
                new HashCrc32(),       // TS/RA2
                new HashRol3(),        // TS/RA2 setup
                new HashRor(),         // Lands of Lore 3
                new HashObscure(),     // setup mix files
                new HashObfuscate(),   // hidden options
                //new HashBR(),          // Blade Runner (not implemented)
            };

        /// <summary>
        /// The hashing method. Assumes that the input is already formatted to the correct case according to NeedsUpperCase.
        /// </summary>
        /// <param name="name">String to hash.</param>
        /// <returns>The hashed value.</returns>
        public abstract UInt32 GetNameIdCorrectCase(String name);

        /// <summary>
        /// The hashing method. Assumes that the input is already formatted to the correct case according to NeedsUpperCase.
        /// </summary>
        /// <param name="name">String to hash, as byte array.</param>
        /// <returns>The hashed value.</returns>
        public abstract UInt32 GetNameIdCorrectCase(Byte[] data);

        /// <summary>
        /// Returns the display name of the hashing method.
        /// </summary>
        /// <returns>The display name of the hashing method.</returns>
        public abstract String GetDisplayName();

        /// <summary>
        /// Returns the short name of the hashing method.
        /// </summary>
        /// <returns>The short name of the hashing method.</returns>
        public abstract String GetSimpleName();

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
            return GetNameIdCorrectCase(this.NeedsUpperCase ? name.ToUpperInvariant() : name);
        }

        public String GetNameIdHexString(String name)
        {
            return this.GetNameId(name).ToString("X4").PadLeft(8, '0');
        }

        public String GetNameIdHexString(String name, Boolean assumeCorrectCase)
        {
            return (assumeCorrectCase ? this.GetNameIdCorrectCase(name) : GetNameId(name)).ToString("X4").PadLeft(8, '0');
        }

        public override String ToString()
        {
            return GetDisplayName();
        }

        public static HashMethod[] GetRegisteredMethods()
        {
            return registeredMethods.ToArray();
        }

    }
}
