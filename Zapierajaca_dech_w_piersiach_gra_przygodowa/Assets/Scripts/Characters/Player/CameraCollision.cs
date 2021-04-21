using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public float minDistance = 1f;
    public float maxDistance = 4f;
    public float smooth = 150f;
    public Vector3 dollyDirAdjusted;
    

    Vector3 dollyDir;
    static float x;
    static float y;
    static float z;
    private float distance;

    void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
        
        x = Mathf.Tan(Camera.main.fieldOfView / 2) * Camera.main.nearClipPlane;
        y = x / Camera.main.aspect;
        z = Camera.main.nearClipPlane;
    }

    void Update()
    {
        Vector3 desireCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        distance = maxDistance;

        if (Physics.Linecast(transform.parent.position, desireCameraPos + new Vector3(x, y, z), out hit))
        {
            if (distance > Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance))
                distance = Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance);
        }
        if (Physics.Linecast(transform.parent.position, desireCameraPos + new Vector3(x, -y, z), out hit))
        {
            if (distance > Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance))
                distance = Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance);
        }
        if (Physics.Linecast(transform.parent.position, desireCameraPos + new Vector3(-x, y, z), out hit))
        {
            if (distance > Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance))
                distance = Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance);
        }
        if (Physics.Linecast(transform.parent.position, desireCameraPos + new Vector3(-x, -y, z), out hit))
        {
            if (distance > Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance))
                distance = Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance);
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
}