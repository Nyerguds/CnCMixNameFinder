using System;
using System.Collections.Generic;
using System.Text;

namespace MissionNameChecker.domain
{
    class NameGenerator
    {

        /// <summary>
        /// Generates an ID as it is stored in a C&amp;C1 type mixfile.
        /// The input is always changed to upper case,
        /// making the algorithm case insensitive.
        /// Original C++ code: http://xhp.xwis.net/documents/MIX_Format.html
        /// </summary>
        /// <param name="name">Filename to encode</param>
        /// <returns>the hashed ID of the filename</returns>
        public static UInt32 getNameId(String name)
        {
            name = name.ToUpper();              // convert to uppercase
            int i = 0;
            UInt32 id = 0;
            int l = name.Length;          // length of the filename
            while (i < l)
            {
                UInt32 a = 0;
                for (int j = 0; j < 4; j++)
                {
                    a >>= 8;
                    if (i < l)
                        a += ((UInt32)name[i] << 24);
                    i++;
                }
                id = (id << 1 | id >> 31) + a;
            }
            return id;
        }

        public static String getNameIdHexString(String name)
        {
            return getNameId(name).ToString("X4").PadLeft(8, '0');
        }
    }
}
