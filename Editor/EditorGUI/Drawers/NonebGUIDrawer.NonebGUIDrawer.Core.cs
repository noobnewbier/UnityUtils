namespace UnityUtils.Editor
{
    public partial class NonebGUIDrawer
    {
        /// <summary>
        /// Call this before you do anything
        /// </summary>
        public bool Update() => _serializedObject.UpdateIfRequiredOrScript();

        /// <summary>
        /// Call this before OnGui exits
        /// </summary>
        public bool Apply() => _serializedObject.ApplyModifiedProperties();
    }
}