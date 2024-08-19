using RuneForger.Gameplay;
using UnityEngine;

namespace RuneForger.Player
{
    public class ForceFieldEmitter : MonoBehaviour
    {
        [SerializeField]
        private LayerMask emitLayer;
        [SerializeField]
        private float emitDistance = 10f;
        public bool IsAiming { get; private set; }
        public bool IsFieldActive { get; private set; }
        [SerializeField]
        private GameObject forceField;
        [SerializeField]
        private GameObject forceFieldIndicator;

        public void BeginAiming()
        {
            IsAiming = true;
            enabled = true;
            forceFieldIndicator.SetActive(true);
        }

        public void EndAiming()
        {
            IsAiming = false;
            enabled = false;
            forceFieldIndicator.SetActive(false);
        }

        public void Emit()
        {
            IsAiming = false;
            enabled = false;
            forceFieldIndicator.SetActive(false);
            IsFieldActive = true;
            forceField.transform.position = forceFieldIndicator.transform.position;
            forceField.SetActive(true);
        }

        public void Dissolve()
        {
            forceField.SetActive(false);
            IsFieldActive = false;
        }
        
        private void Update()
        {
            var ray = new Ray(GameplayManager.Instance.ViewCamera.transform.position, GameplayManager.Instance.ViewCamera.transform.forward);
            forceFieldIndicator.transform.position = Physics.Raycast(ray, out var hit, emitDistance,  emitLayer) ? hit.point : ray.GetPoint(emitDistance);
        }
    }
}