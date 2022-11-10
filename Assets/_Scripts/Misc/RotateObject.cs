using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool startAtRandomRotation;

    private void Start()
    {
        if (startAtRandomRotation) transform.Rotate(0,0, Random.Range(-180f, 180f));
    }
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed);
    }
}
