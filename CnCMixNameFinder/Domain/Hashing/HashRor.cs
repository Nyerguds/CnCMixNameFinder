namespace CnCMixNameFinder.Domain
{
    public class HashRor : HashMethod
    {
        public override string DisplayName => "ROR (Lands of Lore 3)";
        public override string SimpleName => "ROR";

        public override uint GetNameIdCorrectCase(byte[] data)
        {
            uint id = 0;
            for (int i = 0; i < data.Length; i++)
            {
                uint rotatedValue = BitFunctions.RotateRight(id, 6);
                id = (uint)((data[i] - 48) & 63) + rotatedValue;
            }
            return id;
        }

        public override uint GetNameIdCorrectCase(string name)
        {
            uint id = 0;
            for (int i = 0; i < name.Length; i++)
            {
                uint rotatedValue = BitFunctions.RotateRight(id, 6);
                id = (uint)((name[i] - 48) & 63) + rotatedValue;
            }
            return id;
        }
    }
}