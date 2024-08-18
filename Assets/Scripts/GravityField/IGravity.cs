using UnityEngine;

namespace RuneForger.GravityField
{
    public interface IGravity
    {
        public void OnGravityChanged(in Vector3 oldDir, in Vector3 newDir);

        public void OnForceFieldEnter(in Vector3 fieldPos);
        public void OnForceFieldExit(in Vector3 fieldPos);
        public void OnForceFieldChanged(in Vector3 fieldPos);
    }
}