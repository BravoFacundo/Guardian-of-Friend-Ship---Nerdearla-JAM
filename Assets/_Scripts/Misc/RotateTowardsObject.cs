using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsObject : MonoBehaviour
{
    private Transform target;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer targetSpriteRenderer;
    private float dist;
    private Camera cam;

    private void Start()
    {
        target = transform.parent.GetComponent<RotatePivotTowardsObject>().target;
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetSpriteRenderer = target.GetComponent<SpriteRenderer>();
        cam = Camera.main;

        //Rotate Towards Object
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        //Hide or Show Arrow
        if (!IsVisibleToCamera(target.transform) && IsGettingCloser())
            spriteRenderer.enabled = true;
        else
            spriteRenderer.enabled = false;
    }

    void Update()
    {
        if (target != null)
        {
            //Rotate Towards Object
            Vector3 dir = target.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

            //Hide or Show Arrow
            if (!IsVisibleToCamera(target.transform) && IsGettingCloser())
                spriteRenderer.enabled = true;
            else
                spriteRenderer.enabled = false;
        }
    }

    public bool IsVisibleToCamera(Transform transform)
    {
        Vector3 visTest = Camera.main.WorldToViewportPoint(transform.position);
        return (visTest.x >= 0 && visTest.y >= 0) && (visTest.x <= 1 && visTest.y <= 1) && visTest.z >= 0;
    }
    public bool IsGettingCloser()
    {
        float distTemp = Vector3.Distance(cam.transform.position, target.position);
        if (distTemp < dist)
        {
            dist = distTemp;
            return true;
        }
        else if (distTemp > dist)
        { // rigorous checking
            dist = distTemp;
            return false;
        }
        else
            return true;
    }
}
