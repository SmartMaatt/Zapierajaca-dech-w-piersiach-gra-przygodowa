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
    public bool enablePortal;
    [SerializeField] GameObject portal;

    private bool _open = false;
    private AudioManager _audioManager;

    void Start()
    {
        dPos = new Vector3(0, transform.position.y - transform.localScale.y - 0.5f, 0);
        error.SetActive(false);
        _audioManager = GetComponent<AudioManager>();
    }

    public void Operate()
    {
        if (requireKey)
        {
            if (Managers.Inventory.equippedSpecial == keyName)
            {
                if (_open)
                {
                    _audioManager.Play("Start");
                    StartCoroutine(doorOpenClose(transform.position, dPos, true));
                }
                else
                {
                    _audioManager.Play("Start");
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
                _audioManager.Play("Start");
                StartCoroutine(doorOpenClose(transform.position, dPos, true));
            }
            else
            {
                _audioManager.Play("Start");
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
        while (transform.position.y > endPos.y)
        {
            _audioManager.Play("Slide", 0.5f, 1f, false, true);

            if (up)
                transform.position += new Vector3(0, Time.deltaTime * speed, 0);
            else
                transform.position -= new Vector3(0, Time.deltaTime * speed, 0);

            yield return new WaitForEndOfFrame();
        }

        _audioManager.Stop("Slide");
        _audioManager.Play("End");
        portal.SetActive(enablePortal);
    }

   
}