using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLight : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _targetOffset;
    [SerializeField] private bool _reset;

    private void Update()
    {
    }

    private void LookAtTarget()
    {
        if (_target != null)
        {
            transform.LookAt(_target.position + _targetOffset);
        }
    }

    private void OnValidate()
    {
        LookAtTarget();
    }
}
