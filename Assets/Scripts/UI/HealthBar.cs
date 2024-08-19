using System;
using DG.Tweening;
using RuneForger.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace RuneForger.UI
{
    public class HealthBar : MonoBehaviour
    {
        private Image _bar;
        private Image _barBackground;

        private void Awake()
        {
            _bar = transform.GetChild(2).GetComponent<Image>();
            _barBackground = transform.GetChild(1).GetComponent<Image>();
        }
        
        private void OnEnable()
        {
            GameplayManager.Instance.Status.HitPointChangeEvent += OnHitPointChanged;
        }

        private void OnDisable()
        {
            GameplayManager.Instance.Status.HitPointChangeEvent -= OnHitPointChanged;
        }

        private void OnHitPointChanged(float currentHp, float maxHp)
        {
            _bar.fillAmount = currentHp / maxHp;
            _barBackground.DOFillAmount(currentHp / maxHp, 0.5f);
        }
    }
}