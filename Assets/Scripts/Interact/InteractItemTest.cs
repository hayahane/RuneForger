using RuneForger.GravityField;
using RuneForger.Interact;
using UnityEngine;

public class InteractItemTest : InteractItem
{
    [field: SerializeField]
    public GravityField GravityField { get; set; }
    [SerializeField]
    private string _interactText = "改变重力";
    public override string InteractText => _interactText;
    public override bool CanInteract(Transform trans)
    {
        return true;
    }
    // 交互时改变重力方向
    public override void OnInteract()
    {
        Debug.Log("Character Interacted");
        GravityField.FieldDirection *= -1;
    }
}