using System.Linq;

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
                new HashObfuscateSole(), // Sole Survivor fixed version of hidden options
                //new HashBR(),          // Blade Runner (not implemented)
            };

        /// <summary>
        /// The hashing method. Assumes that the input is already formatted to the correct case according to NeedsUpperCase.
        /// </summary>
        /// <param name="name">String to hash.</param>
        /// <returns>The hashed value.</returns>
        public abstract uint GetNameIdCorrectCase(string name);

        /// <summary>
        /// The hashing method. Assumes that the input is already formatted to the correct case according to NeedsUpperCase.
        /// </summary>
        /// <param name="name">String to hash, as byte array.</param>
        /// <returns>The hashed value.</returns>
        public abstract uint GetNameIdCorrectCase(byte[] data);

        /// <summary>
        /// Returns the display name of the hashing method.
        /// </summary>
        /// <returns>The display name of the hashing method.</returns>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Returns the short name of the hashing method.
        /// </summary>
        /// <returns>The short name of the hashing method.</returns>
        public abstract string SimpleName { get; }

        /// <summary>
        /// Allows supporting methods that are not case insensitive.
        /// </summary>
        public virtual bool NeedsUpperCase
        {
            get { return true; }
        }

        public uint GetNameId(string name)
        {
            if (name == null)
                name = string.Empty;
            return GetNameIdCorrectCase(NeedsUpperCase ? name.ToUpperInvariant() : name);
        }

        public string GetNameIdHexString(string name)
        {
            return GetNameId(name).ToString("X4").PadLeft(8, '0');
        }

        public string GetNameIdHexString(string name, bool assumeCorrectCase)
        {
            return (assumeCorrectCase ? GetNameIdCorrectCase(name) : GetNameId(name)).ToString("X4").PadLeft(8, '0');
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public static HashMethod[] GetRegisteredMethods()
        {
            return registeredMethods.ToArray();
        }

    }
}
