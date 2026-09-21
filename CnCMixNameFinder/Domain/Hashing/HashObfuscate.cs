using System;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    public class HashObfuscateSole : HashObfuscate
    {
        public override string DisplayName => "Obfuscate Sole (hidden options)";
        public override string SimpleName => "ObfuscateSole";

        public HashObfuscateSole()
        {
            //
        }

        protected override int GetDataLength(byte[] data)
        {
            int len = data.Length;
            for (int i = 0; i < len; ++i)
            {
                if (data[i] == 0)
                {
                    return i;
                }
            }
            return len;
        }
    }

    public class HashObfuscate : HashRol
    {

        public override string DisplayName => "Obfuscate (hidden options)";
        public override string SimpleName => "Obfuscate";

        private static bool[] isGraph = new bool[256];

        static HashObfuscate()
        {
            char[] graphChars = new char[] { '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?', '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_', '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', '|', '}', '~' };
            for (int i = 0; i < graphChars.Length; ++i)
            {
                isGraph[graphChars[i]] = true;
            }
        }

        public HashObfuscate()
        {
            //
        }

        /*
        **    Only upper case letters are significant.
        */
        public override bool NeedsUpperCase => true;

        public override uint GetNameIdCorrectCase(string name)
        {
            // With encoding 1252, all unknown chars get treated as non-graphs and converted as expected.
            return GetNameIdCorrectCase(name == null ? null : Encoding.GetEncoding(1252).GetBytes(name));
        }

        public override uint GetNameIdCorrectCase(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return 0;
            }
#if DEBUG
            string curBufferStr;
            string curCodeStr;
#endif
            /*
            **    Copy key phrase into a working buffer. This hides any transformation done
            **    to the string.
            */
            int length = data.Length;

            /*
            **    Ensure that only visible ASCII characters compose the key phrase. This
            **    discourages the direct forced illegal character input method of attack.
            */
            for (int index = 0; index < length; index++)
            {
                if (!isGraph[data[index]])
                {
                    data[index] = (byte)('A' + (index % 26));
                }
            }

            /*
            **    Increase the strength of even short pass phrases by extending the
            **    length to be at least a minimum number of characters. This helps prevent
            **    a weak pass phrase from compromising the obfuscation process. This
            **    process also forces the key phrase to be an even multiple of four.
            **    This is necessary to support the cypher process that occurs later.
            */
            if (length < 16 || (length & 0x03) != 0)
            {
                int maxlen = 16;
                if (((length + 3) & 0x00FC) > maxlen)
                {
                    maxlen = (length + 3) & 0x00FC;
                }
                byte[] newBuffer = new byte[maxlen];
                for (int i = length; i < newBuffer.Length; ++i)
                    newBuffer[i] = 0xA5;
                Array.Copy(data, 0, newBuffer, 0, length);
                int index;
                for (index = length; index < maxlen; index++)
                {
                    newBuffer[index] = (byte)('A' + ((('?' ^ newBuffer[index - length]) + index) % 26));
                }
                length = index;
                data = newBuffer;
            }
#if DEBUG
            curBufferStr = Encoding.ASCII.GetString(data);
            curCodeStr = GetNameId(data, GetDataLength(data), 1).ToString("X8");
#endif
            /*
            **    Transform the buffer into a number. This transformation is character
            **    order dependant.
            */
            //uint code = GetNameId(buffer, 1);

            /*
            **    Record a copy of this initial transformation to be used in a later
            **    self referential transformation.
            */
            //uint copy = code;

            /*
            **    Reverse the character string and combine with the previous transformation.
            **    This doubles the workload of trying to reverse engineer the CRC calculation.
            */
            Array.Reverse(data);
            int code = (int)GetNameId(data, GetDataLength(data), 1);
#if DEBUG
            curBufferStr = String.Join(", ", data.Select(b => String.Format("{0:X2}", b)).ToArray());
            curCodeStr = code.ToString("X8");
#endif
            //code ^= code_rev;

            /*
            **    Perform a self referential transformation. This makes a reverse engineering
            **    by using a cause and effect attack more difficult.
            */
            //code ^= copy;

            // Above steps are pointless; they result in the same thing as just the reversed crc.


            /*
            **    Unroll and combine the code value into the pass phrase and then perform
            **    another self referential transformation. Although this is a trivial cypher
            **    process, it gives the sophisticated hacker false hope since the strong
            **    cypher process occurs later.
            */
            Array.Reverse(data);     // Restore original string order.
            for (int index = 0; index < length; index++)
            {
                code ^= data[index];
                byte temp = (byte)code;
                data[index] ^= temp;
                code = code >> 8 | (temp << 24);
            }
#if DEBUG
            curBufferStr = String.Join(", ", data.Select(b => String.Format("{0:X2}", b)).ToArray());
            curCodeStr = code.ToString("X8");
#endif
            /*
            **    Introduce loss into the vector. This strengthens the key against traditional
            **    cryptographic attack engines. Since this also weakens the key against
            **    unconventional attacks, the loss is limited to less than 10%.
            */
            byte[] _lossbits = { 0x00, 0x08, 0x00, 0x20, 0x00, 0x04, 0x10, 0x00 };
            int _lossbitsLen = _lossbits.Length;
            byte[] _addbits = { 0x10, 0x00, 0x00, 0x80, 0x40, 0x00, 0x00, 0x04 };
            int _addbitsLen = _addbits.Length;
            for (int index = 0; index < length; index++)
            {
                data[index] |= _addbits[index % _addbitsLen];
                data[index] &= (byte)~_lossbits[index % _lossbitsLen];
            }
#if DEBUG
            curBufferStr = String.Join(", ", data.Select(b => String.Format("{0:X2}", b)).ToArray());
#endif
            /*
            **    Perform a general cypher transformation on the vector
            **    and use the vector itself as the cypher key. This is a variation on the
            **    cypher process used in PGP. It is a very strong cypher process with no known
            **    weaknesses. However, in this case, the cypher key is the vector itself and this
            **    opens up a weakness against attacks that have access to this transformation
            **    algorithm. The sheer workload of reversing this transformation should be enough
            **    to discourage even the most determined hackers.
            */
            for (int index = 0; index < length; index += 4)
            {
                /*/
                Int16 key1 = data[index];
                Int16 key2 = data[index + 1];
                Int16 key3 = data[index + 2];
                Int16 key4 = data[index + 3];
                Int16 val1 = (Int16)(key1 * key1);
                Int16 val2 = (Int16)(2 * key2);
                Int16 val3 = (Int16)(2 * key3);
                Int16 val4 = (Int16)(key4 * key4);

                Int16 s3 = val3;
                val3 ^= val1;
                val3 *= key1;
                Int16 s2 = val2;
                val2 ^= val4;
                val2 += val3;
                val2 *= key3;
                val3 += val2;

                val1 ^= val2;
                val4 ^= val3;

                val2 ^= s3;
                val3 ^= s2;

                data[index] = (byte)val1;
                data[index + 1] = (byte)val2;
                data[index + 2] = (byte)val3;
                data[index + 3] = (byte)val4;
                //*/

                // Tomsons26's rewrite. Not sure if faster.
                short ch1 = data[index + 0];
                short ch2 = data[index + 1];
                short ch3 = data[index + 2];
                short ch4 = data[index + 3];
                short val_1 = (short)(ch1 * ch1);
                short val_2 = (short)(2 * ch2);
                short val_3 = (short)(2 * ch3);
                short val_4 = (short)(ch4 * ch4);
                
                short tmp0 = (short)(ch1 * (val_1 ^ val_3));
                short tmp1 = (short)(ch3 * (tmp0 + (val_4 ^ val_2)));
                short tmp2 = (short)(tmp1 + tmp0);

                data[index + 0] = (byte)(tmp1 ^ val_1);
                data[index + 1] = (byte)(val_3 ^ tmp1);
                data[index + 2] = (byte)(val_2 ^ tmp2);
                data[index + 3] = (byte)(tmp2 ^ val_4);
                //*/
            }
#if DEBUG
            curBufferStr = String.Join(", ", data.Select(b => String.Format("{0:X2}", b)).ToArray());
#endif
            /*
            **    Convert this final vector into a cypher key code to be
            **    returned by this routine.
            */
            code = (int)GetNameId(data, GetDataLength(data), 1);
#if DEBUG
            curCodeStr = code.ToString("X8");
#endif
            /*
            **    Return the final code value.
            */
            return (uint)code;
        }

        protected virtual int GetDataLength(byte[] data)
        {
            return data.Length;
        }
    }
}