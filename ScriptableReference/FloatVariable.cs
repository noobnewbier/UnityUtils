using UnityEngine;

namespace UnityUtils.ScriptableReference
{
    [CreateAssetMenu(menuName = "FloatVariable")]
    public class FloatVariable : ScriptableObject
    {
#if UNITY_EDITOR
#pragma warning disable 0414
        // ReSharper disable once NotAccessedField.Local
        [Multiline] [SerializeField] private string developerDescription = "Self Descriptive Name";
#pragma warning restore 0414
#endif
        [SerializeField] private float value;

        public float Value => value;

        public static implicit operator float(FloatVariable variable) => variable.Value;
    }
}