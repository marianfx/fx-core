namespace FX.Core.Common.DataModel
{
    public class Descriptor : BaseDescriptor<string>
    {
        public Descriptor() : base("", "")
        {
        }

        public Descriptor(string value, string desc) : base(value, desc)
        {

        }
    }
}
