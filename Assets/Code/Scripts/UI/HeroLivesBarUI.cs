using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroLivesBarUI : MonoBehaviour
{
    [SerializeField] private Hero _hero;
    [SerializeField] private float _heartImageWidth = 8;

    private Image _heartsImage;

    void Awake()
    {
        TryGetComponent(out _heartsImage);
    }

    void Start()
    {
        UpdateLivesBar();
    }
    
    void OnEnable()
    {
        UpdateLivesBar();
        _hero.OnHeroDied.AddListener(UpdateLivesBar);
    }

    private void OnDisable()
    {
        _hero.OnHeroDied.RemoveListener(UpdateLivesBar);
    }

    void UpdateLivesBar()
    {
        if (_hero != null)
        {
            _heartsImage.rectTransform.sizeDelta = new Vector2(_heartImageWidth * _hero.Lives, _heartImageWidth);
        }
    }
}
