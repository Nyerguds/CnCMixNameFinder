namespace CnCMixNameFinder.Domain
{
    internal class HashBr : HashMethod
    {
        public override string DisplayName => "Unknown (Blade Runner)";
        public override string SimpleName => "BladeRunner";

        public override uint GetNameIdCorrectCase(string name)
        {
            return 0;
        }

        public override uint GetNameIdCorrectCase(byte[] data)
        {
            return 0;
        }
    }
}
