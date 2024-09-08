using UnityEngine;

namespace RuneForger.Battle
{
    public class BattleZone : MonoBehaviour
    {
        [SerializeField] private float zoneRadius = 10f;
        [SerializeField] private int minerCount;

        [SerializeField] private int wormCount;

        private void OnValidate()
        {
            transform.localScale = new Vector3(zoneRadius / 4f, 1, zoneRadius / 4f);
        }
    }
}