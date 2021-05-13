using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DoorOpenDevice : MonoBehaviour, InteractOperator
{
    [SerializeField] private Vector3 dPos;
    public bool requireKey;
    public string keyName;

    private bool _open = false;
    
    void Start()
    {
        dPos = new Vector3(0, 0.1f -transform.lossyScale.y, 0);
    }

    public void Operate()
    {
        if(_open)
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

[CustomEditor(typeof(DoorOpenDevice))]
public class MyScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        var myScript = target as DoorOpenDevice;

        myScript.requireKey = GUILayout.Toggle(myScript.requireKey, " Key required");

        if (myScript.requireKey)
            myScript.keyName = EditorGUILayout.TextField("Key name", myScript.keyName);

    }
}