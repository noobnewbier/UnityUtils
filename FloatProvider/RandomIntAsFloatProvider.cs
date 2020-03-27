using UnityEngine;

namespace UnityUtils.FloatProvider
{
    public class RandomIntAsFloatProvider : FloatProvider
    {
        [SerializeField] private int maximum;
        [SerializeField] private int minimum;

        public override float ProvideFloat()
        {
            return Random.Range(minimum, maximum);
        }

        private void OnValidate()
        {
            if (minimum > maximum)
            {
                Debug.Log(gameObject.name + "'s RandomFloatProvider could not have minimum greater than maximum");
                minimum = maximum;
            }
        }
    }
}