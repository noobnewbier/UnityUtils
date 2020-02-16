namespace UnityUtils.BooleanProviders
{
    public class StateRepresenter : BooleanProvider
    {
        private bool _isStateTrue;

        public override bool ProvideBoolean()
        {
            return _isStateTrue;
        }

        public void SetState(bool currentState)
        {
            _isStateTrue = currentState;
        }
    }
}