namespace FX.Core.Common.DataModel
{
    public class NDescriptor : BaseDescriptor<long>
    {
        public NDescriptor() : base(-1, "")
        { }

        public NDescriptor(long value, string desc) : base(value, desc)
        {

        }
    }
}
