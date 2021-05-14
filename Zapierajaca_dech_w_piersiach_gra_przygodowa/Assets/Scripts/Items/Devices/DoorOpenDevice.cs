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
                    transform.position = transform.position - dPos;
                }
                else
                {
                    transform.position = transform.position + dPos;
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
                transform.position = transform.position - dPos;
            }
            else
            {
                transform.position = transform.position + dPos;
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
}

//[CustomEditor(typeof(DoorOpenDevice))]
//public class MyScriptEditor : Editor
//{
//    override public void OnInspectorGUI()
//    {
//        GameObject gameObject;
//        var myScript = target as DoorOpenDevice;
        
//        myScript.requireKey = GUILayout.Toggle(myScript.requireKey, " Key required");

//        if (myScript.requireKey)
//            myScript.keyName = EditorGUILayout.TextField("Key name", myScript.keyName);

//    }
//}