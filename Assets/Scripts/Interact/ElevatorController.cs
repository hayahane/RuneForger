using System;
using DG.Tweening;
using RuneForger.Scene;
using UnityEngine;

namespace RuneForger.Interact
{
    public class ElevatorController : InteractItem
    {
        public enum ButtonType
        {
            Operator,
            Caller
        }

        [SerializeField] 
        private ButtonType buttonType = ButtonType.Caller;
        [SerializeField]
        private int callerLevel;
        [SerializeField]
        private Elevator elevator;
        [SerializeField]
        private string text = "操作电梯";

        private Transform _controller;

        public override string InteractText => text;
        public override void OnInteract()
        {
            if (elevator == null)
            {
                Debug.LogError("No Valid Elevator");
                return;
            }
            switch (buttonType)
            {
                case ButtonType.Caller:
                    elevator.ChangeToLevel(callerLevel);
                    break;
                case ButtonType.Operator:
                    elevator.NextLevel();
                    break;
            }

            var seq = DOTween.Sequence();
            seq.Append(_controller.DOLocalRotate(new Vector3(0, 0, 20f), 0.5f));
            seq.Append(_controller.DOLocalRotate(new Vector3(0, 0, -20f), 0.6f));
            seq.SetLoops(1);
        }

        public override bool CanInteract(Transform trans)
        {
            return true;
        }

        private void Awake()
        {
            _controller = transform.GetChild(0);
        }
    }
}