using UnityEngine;
using UnityEngine.Events;

namespace RuneForger.Interact
{
    public class InteractGetAbilityBall : InteractItem
    {
        [SerializeField] private string text = "获取能力";
        public override string InteractText => text;

        [SerializeField] private UnityEvent _event;
        public override void OnInteract()
        {
            Debug.Log("解锁能力");
            _event?.Invoke();
        }

        public override bool CanInteract(Transform trans)
        {
            return true;
        }
    }
}