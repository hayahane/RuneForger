using UnityEngine;

namespace RuneForger.GravityField
{
    public interface IGravity
    {
        public void OnGravityChanged(in Vector3 oldValue, in Vector3 newValue);

        public void OnForceFieldChanged(in Vector3 oldValue, in Vector3 newValue);
    }
}