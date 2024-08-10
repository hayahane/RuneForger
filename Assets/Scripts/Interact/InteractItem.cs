using UnityEngine;

namespace RuneForger.Interact
{
    public abstract class InteractItem : MonoBehaviour
    {
        public abstract string InteractText {get;}
        public abstract void OnInteract();
        public abstract bool CanInteract(Transform trans);
    }
}