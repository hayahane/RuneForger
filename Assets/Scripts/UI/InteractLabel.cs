using System;
using RuneForger.Gameplay;
using UnityEngine;

namespace RuneForger.UI
{
    public class InteractLabel : MonoBehaviour
    {
        private TMPro.TextMeshProUGUI _text;
        private CanvasGroup _canvasGroup;
        private Interact.InteractItem _interactItem;

        private void Awake()
        {
            _text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
        }

        private void OnEnable()
        {
            GameplayManager.Instance.CharacterInteract.OnItemChangedEvent += OnItemChanged;
        }
        

        private void OnItemChanged(Interact.InteractItem item)
        {
            _interactItem = item;
            _canvasGroup.alpha = item == null ? 0 : 1;
            if (_interactItem != null)
                _text.text = _interactItem.InteractText;
        }
    }
}