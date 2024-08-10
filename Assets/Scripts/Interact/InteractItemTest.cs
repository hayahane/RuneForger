using RuneForger.Interact;
using UnityEngine;

public class InteractItemTest : InteractItem
{
    [SerializeField]
    private string _interactText = "Interact with Torus";
    public override string InteractText => _interactText;
    public override bool CanInteract(Transform trans)
    {
        return true;
    }

    public override void OnInteract()
    {
        Debug.Log("Character Interacted");
    }
}