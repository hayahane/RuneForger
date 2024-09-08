using RuneForger.Interact;
using UnityEngine;

namespace RuneForger.GravityField.GravityItem
{
    [SelectionBase]
    public class FieldSwitcher : InteractItem
    {
        public GravityField GravityField { get; set; }
        public override string InteractText => "改变重力";

        public override void OnInteract()
        {
            GravityField.FieldDirection *= -1;
        }

        public override bool CanInteract(Transform trans)
        {
            return true;
        }
    }
}