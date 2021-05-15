using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DoorOpenDevice : MonoBehaviour, InteractOperator
{
    private Vector3 dPos;
    public GameObject error;
    public bool requireKey;
    public string keyName;

    private bool _open = false;
    
    void Start()
    {
        dPos = new Vector3(0, 0.1f -transform.lossyScale.y, 0);
        error.SetActive(false);
    }

    public void Operate()
    {
        if (requireKey)
        {
            if (Managers.Inventory.equippedSpecial == keyName)
            {
                if (_open)
                {
                    StartCoroutine(doorOpenClose(transform.position, dPos, true));
                }
                else
                {
                    StartCoroutine(doorOpenClose(transform.position, dPos, false));
                }
                _open = !_open;
            }
            else
                StartCoroutine(errorPopup());
        }
        else 
        {
            if (_open)
            {
                StartCoroutine(doorOpenClose(transform.position, dPos, true));
            }
            else
            {
                StartCoroutine(doorOpenClose(transform.position, dPos, false));
            }
            _open = !_open;
        }
    }

    private IEnumerator errorPopup()
    {
        error.SetActive(true);
        yield return new WaitForSeconds(3);
        error.SetActive(false);
    }

    private IEnumerator doorOpenClose(Vector3 startPos, Vector3 endPos, bool up)
    {
        float speed = 1;
        while (startPos != startPos - endPos)
        {
            if (up)
                transform.position += new Vector3(0, Time.deltaTime * speed, 0);
            else
                transform.position -= new Vector3(0, Time.deltaTime * speed, 0);

            yield return new WaitForEndOfFrame();
        }
    }
}