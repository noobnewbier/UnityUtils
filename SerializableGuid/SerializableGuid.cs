using System;
using UnityEngine;

namespace UnityUtils.SerializableGuid
{
    [Serializable]
    public struct SerializableGuid : ISerializationCallbackReceiver
    {
        public SerializableGuid(Guid guid) : this()
        {
            Value = guid;
        }

        public SerializableGuid(byte[] guidAsBytes) : this()
        {
            Value = new Guid(guidAsBytes);
        }

        public Guid Value
        {
            get => _value;
            set
            {
                _value = value;
                serializedGuid = _value.ToByteArray();
            }
        }

        public static implicit operator Guid(SerializableGuid? serializableGuid) => serializableGuid?.Value ?? Guid.Empty;

        #region Serialization

        [SerializeField] private byte[]? serializedGuid; //16 is the guid's length in bytes
        private Guid _value;

        public void OnBeforeSerialize()
        {
            if (Value != Guid.Empty) serializedGuid = Value.ToByteArray();
        }

        public void OnAfterDeserialize()
        {
            if (serializedGuid is { Length: 16 }) Value = new Guid(serializedGuid);
        }

        #endregion
    }
}