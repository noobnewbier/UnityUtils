using UnityEngine;

namespace UnityUtils.FloatProvider
{
    public class RandomFloatProvider : FloatProvider
    {
        [SerializeField] private float maximum;
        [SerializeField] private float minimum;

        private void OnValidate()
        {
            if (minimum > maximum)
            {
                Debug.Log(gameObject.name + "'s RandomFloatProvider could not have minimum greater than maximum");
                minimum = maximum;
            }
        }

        public override float ProvideFloat() => Random.Range(minimum, maximum);
    }
}