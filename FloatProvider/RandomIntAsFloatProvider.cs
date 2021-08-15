using UnityEngine;

namespace UnityUtils.FloatProvider
{
    public class RandomIntAsFloatProvider : FloatProvider
    {
        [SerializeField] private int maximum;
        [SerializeField] private int minimum;

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