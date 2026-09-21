using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    public class HashObscure : HashMethod
    {
        private const UInt32 OBSCURE_MAGIC_NUM = 0x516150;

        public override UInt32 GetNameIdCorrectCase(String name)
        {
            return GetNameIdCorrectCase(Encoding.ASCII.GetBytes(name));
        }

        public override UInt32 GetNameIdCorrectCase(Byte[] data)
        {
            if (data.Length < 7)
            {
                Byte[] values2 = new Byte[7];
                Array.Copy(data, values2, data.Length);
                data = values2;
            }
            Byte v8 = data[0]; // [sp+1Ch] [bp-104h]@1
            Byte v9 = data[1]; // [sp+1Dh] [bp-103h]@3
            Byte v10 = data[3]; // [sp+1Fh] [bp-101h]@3
            Byte v11 = data[4]; // [sp+20h] [bp-100h]@3
            Byte v12 = data[5]; // [sp+21h] [bp-FFh]@3
            Byte v13 = data[6]; // [sp+22h] [bp-FEh]@3
            Int64 v3 = v13 + 0xA * (v12 + 0xA * (v11 + 0xA * (v10 + 0xA * (v9 + 0xA * v8)))) - 0x516150;
            return (UInt32)v3;
        }

        private UInt32 Testfnc(String name)
        {
            Byte[] values = Encoding.ASCII.GetBytes(name.ToUpperInvariant());
            if (values.Length < 7)
            {
                Byte[] values2 = new Byte[7];
                Array.Copy(values, values2, values.Length);
                values = values2;
            }
            Int64 res = values[6] + 10 * (values[5] + 10 * (values[4] + 10 * (values[3] + 10 * (values[1] + 10 * values[0])))) - OBSCURE_MAGIC_NUM;
            return (UInt32)res;
        }

        public override String GetDisplayName()
        {
            return "Poor Man's (SETUP.MIX)";
        }

        public override String GetSimpleName()
        {
            return "Poor Man's";
        }
    }
}
