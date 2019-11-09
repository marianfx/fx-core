using System;

namespace FX.Core.Common.DataModel
{
    public class BaseDescriptor<T> : ICloneable
    {
        public T EmptyValue { get; set; } = default(T);
        public T Value { get; set; }
        public string Description { get; set; }

        public BaseDescriptor(T value, string description)
        {
            Value = value;
            Description = description;
        }

        public bool IsEmpty()
        {
            return Value.Equals(EmptyValue);
        }

        public override bool Equals(object obj)
        {
            var descriptor = obj as BaseDescriptor<T>;
            return descriptor != null &&
                   Value.Equals(descriptor.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
