using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepeatingBackground : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _resetWidth;

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        transform.position += Vector3.left * _speed * Time.deltaTime;

        if (transform.position.x <= _resetWidth)
        {
            transform.position = _startPosition;
        }
    }
}
