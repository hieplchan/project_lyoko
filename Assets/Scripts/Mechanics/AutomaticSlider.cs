using Cysharp.Threading.Tasks;
using StartledSeal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class AutomaticSlider : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _duration = 1f, _delayBeforeReverse = 0.5f;
    [SerializeField] private bool _useAutoReverse = false, _useSmoothStep = false;
    [SerializeField] private UnityEvent<float> _valueChanged;
    
    public bool IsReversed { get; set; }
    public bool UseAutoReverse
    {
        get => _useAutoReverse;
        set => _useAutoReverse = value;
    }

    private float _value;
    
    // Animate physic object so use fixed update
    private async void FixedUpdate()
    {
        float delta = Time.deltaTime / _duration;
        if (IsReversed)
        {
            _value -= delta;
            if (_value <= 0f)
            {
                if (_useAutoReverse)
                {
                    _value = Mathf.Min(1f, -_value);
                    await UniTask.Delay((int)(_delayBeforeReverse * 1000));
                    IsReversed = false;
                }
                else
                {
                    _value = 0f;
                    enabled = false;
                }
            }
        }
        else
        {
            _value += delta;
            if (_value >= 1f)
            {
                if (_useAutoReverse)
                {
                    _value = Mathf.Max(0f, 2f - _value);
                    await UniTask.Delay((int)(_delayBeforeReverse * 1000));
                    IsReversed = true;
                }
                else
                {
                    _value = 1f;
                    enabled = false;
                }
            }
        }

        _valueChanged?.Invoke(_useSmoothStep ? _value.SmoothStep() : _value);
    }
}
