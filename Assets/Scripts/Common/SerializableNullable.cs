using System;
using UnityEngine;

namespace LunarCube
{
    [Serializable]
    public struct SerializableNullable<T> where T : struct
    {
        [SerializeField] private bool hasValue;
        [SerializeField] private T value;

        public SerializableNullable(T value)
        {
            hasValue = true;
            this.value = value;
        }

        public bool HasValue => hasValue;

        public T Value
        {
            get
            {
                if (!HasValue) throw new InvalidOperationException("SerializableNullable object must have a value.");
                return value;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SerializableNullable<T> other)) return false;
            else return Equals(other);
        }

        public bool Equals(SerializableNullable<T> other)
        {
            if (!hasValue && !other.hasValue) return true;
            if (hasValue != other.hasValue) return false;
            return value.Equals(other.value);
        }

        public override int GetHashCode() => HasValue ? Value.GetHashCode() : 0;
        public T GetValueOrDefault() => HasValue ? Value : default;
        public T GetValueOrDefault(T defaultValue) => HasValue ? Value : defaultValue;
        public override string ToString() => hasValue ? Value.ToString() : string.Empty;

        public static implicit operator SerializableNullable<T>(T value) => new SerializableNullable<T>(value);
        public static implicit operator SerializableNullable<T>(DBNull _) => new SerializableNullable<T>();
        public static implicit operator T(SerializableNullable<T> value) => value.Value;

        public static bool operator ==(SerializableNullable<T> left, SerializableNullable<T> right) => left.Equals(right);
        public static bool operator !=(SerializableNullable<T> left, SerializableNullable<T> right) => !left.Equals(right);
    }
}
