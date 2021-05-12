using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float CameraMoveSpeed = 120f;
    public GameObject CameraFollowObj;
    Vector3 FollowPOS;
    public float clampAngle = 80f;
    public float inputSensitivity = 150f;
    public GameObject CameraObj;
    public GameObject PlayerObj;
    public float camDistanceXToPlayer;
    public float camDistanceYToPlayer;
    public float camDistanceZToPlayer;
    public float mouseX;
    public float mouseY;
    public float finalInputX;
    public float finalInputZ;
    public float smoothX;
    public float smoothY;

    public bool isDead;
    public bool deadAnimationPlaying;

    private float rotY = 0.0f;
    private float rotX = 0.0f;

    [SerializeField] private bool cursorDebugMode = false;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        if (!cursorDebugMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }


    void Update()
    {
        if (!isDead)
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            finalInputX = mouseX;
            finalInputZ = -mouseY;

            rotY += finalInputX * inputSensitivity * Time.deltaTime;
            rotX += finalInputZ * inputSensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            transform.rotation = localRotation;
        }
    }

    void LateUpdate()
    {
        CameraUpdater();
    }

    void CameraUpdater()
    {
        if (!isDead && !deadAnimationPlaying)
        {
            // set the target object to follow
            Transform target = CameraFollowObj.transform;

            // move toword game object that is the target
            float step = CameraMoveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
        else if(isDead && !deadAnimationPlaying)
        {
            StartCoroutine(deadAnimation(15f));
        }
    }


    IEnumerator deadAnimation(float time)
    {
        float elapsedTime = 0.0f;
        deadAnimationPlaying = true;

        transform.LookAt(CameraFollowObj.transform);
        transform.eulerAngles = new Vector3(90, 180, 0);

        float min = 0.5f;
        float max = 5f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime / time;
            transform.position = CameraFollowObj.transform.position + new Vector3(0, Mathf.Lerp(min,max,elapsedTime), 0);
            transform.Rotate(0, 0, 0.05f);

            yield return new WaitForEndOfFrame();
        }
    }
}
