using UnityEngine;

namespace UnityUtils
{
    public static class LinearEquations
    {
        //reference: https://math.stackexchange.com/questions/404440/what-is-the-equation-for-a-3d-line symmetric form
        public static (float x, float z) GetXzGivenY(float targetY, Vector3 origin, Vector3 endPoint)
        {
            var direction = endPoint - origin;

            var yEquationAnswer = (targetY - origin.y) / direction.y;

            var x = direction.x * yEquationAnswer + origin.x;
            var z = direction.z * yEquationAnswer + origin.z;

            return (x, z);
        }

        public class LinearEquation2D
        {
            private readonly float _constantValue;
            private readonly float _xCoefficient;
            private readonly float _yCoefficient;

            public LinearEquation2D(float xCoefficient, float yCoefficient, float constantValue)
            {
                _xCoefficient = xCoefficient;
                _yCoefficient = yCoefficient;
                _constantValue = constantValue;
            }

            public float Solve(Vector2 input) => _xCoefficient * input.x + _yCoefficient * input.y - _constantValue;
        }
    }
}