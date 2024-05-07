using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace StartledSeal
{
    public class PlatformMover : MonoBehaviour
    {
        [SerializeField] private Vector3 _moveTo = Vector3.zero;
        [SerializeField] private float _moveTime = 1f;
        [SerializeField] private Ease _ease = Ease.InOutQuad;

        private Vector3 _startPosition;

        private void Awake()
        {
            _startPosition = transform.position;
            Move();
        }

        private void Move()
        {
            transform.DOMove(_startPosition + _moveTo, _moveTime)
                .SetEase(_ease)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}