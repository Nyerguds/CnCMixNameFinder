using System;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    public class HashObscure : HashMethod
    {
        public override string DisplayName => "Poor Man's (SETUP.MIX)";
        public override string SimpleName => "PoorMans";

        private const uint OBSCURE_MAGIC_NUM = 0x516150;

        public override uint GetNameIdCorrectCase(string name)
        {
            return GetNameIdCorrectCase(Encoding.ASCII.GetBytes(name));
        }

        public override uint GetNameIdCorrectCase(byte[] data)
        {
            if (data.Length < 7)
            {
                byte[] values2 = new byte[7];
                Array.Copy(data, values2, data.Length);
                data = values2;
            }
            byte v8 = data[0]; // [sp+1Ch] [bp-104h]@1
            byte v9 = data[1]; // [sp+1Dh] [bp-103h]@3
            byte v10 = data[3]; // [sp+1Fh] [bp-101h]@3
            byte v11 = data[4]; // [sp+20h] [bp-100h]@3
            byte v12 = data[5]; // [sp+21h] [bp-FFh]@3
            byte v13 = data[6]; // [sp+22h] [bp-FEh]@3
            long v3 = v13 + 0xA * (v12 + 0xA * (v11 + 0xA * (v10 + 0xA * (v9 + 0xA * v8)))) - 0x516150;
            return (uint)v3;
        }

        private uint Testfnc(string name)
        {
            byte[] values = Encoding.ASCII.GetBytes(name.ToUpperInvariant());
            if (values.Length < 7)
            {
                byte[] values2 = new byte[7];
                Array.Copy(values, values2, values.Length);
                values = values2;
            }
            long res = values[6] + 10 * (values[5] + 10 * (values[4] + 10 * (values[3] + 10 * (values[1] + 10 * values[0])))) - OBSCURE_MAGIC_NUM;
            return (uint)res;
        }
    }
}
