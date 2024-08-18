using System;
using UnityEngine;
using UnityEngine.Events;

namespace RuneForger.Interact
{
    public class GenericSwitch : InteractItem
    {
        [SerializeField]
        private string switchText = "Press E to interact";
        public override string InteractText => switchText;
        [SerializeField]
        private UnityEvent onSwitchOn;
        
        public override void OnInteract()
        {
            onSwitchOn?.Invoke();
        }

        public override bool CanInteract(Transform trans)
        {
            return true;
        }

        private void OnDrawGizmos()
        {
            if (onSwitchOn == null) return;
            Gizmos.color = new Color(0.7f, 0.5f, 0.2f, 0.5f);
            for (var i = 0; i < onSwitchOn.GetPersistentEventCount(); i++)
            {
                if (onSwitchOn.GetPersistentTarget(i) is Component target)
                {
                    Gizmos.DrawLine(transform.position, target.transform.position);
                }
            }
        }
    }
}