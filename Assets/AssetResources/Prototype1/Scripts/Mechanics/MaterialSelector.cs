using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSelector : MonoBehaviour
{
    [SerializeField] Material[] _materials = default;
    [SerializeField] MeshRenderer _meshRenderer = default;

    public void Select(int index)
    {
        if (_meshRenderer && _materials != null &&
            index >= 0 && index < _materials.Length)
        {
            _meshRenderer.material = _materials[index];
        }
    }
}
