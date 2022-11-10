using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePivotTowardsObject : MonoBehaviour
{
    public Transform target;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        transform.position = cam.transform.position;
        if (target != null)
        {
            //Point in Direction to the Target
            Vector3 dir = target.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

            //Hide when object is visible on screen
            //if target distance > distance then ocultar renderer, else mostrar;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
