using UnityEngine;

namespace UnityUtils
{
    public static class LineEquations
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
    }
}