using UnityEngine;

namespace UnityUtils.ScriptableReference
{
    [CreateAssetMenu(menuName = "FloatVariable")]
    public class FloatVariable : ScriptableObject
    {
#if UNITY_EDITOR
        // ReSharper disable once NotAccessedField.Local
        [Multiline] [SerializeField] private string developerDescription = "Self Descriptive Name";
#endif
        [SerializeField] private float value;

        public float Value => value;

        public static implicit operator float(FloatVariable variable)
        {
            return variable.Value;
        }
    }
}