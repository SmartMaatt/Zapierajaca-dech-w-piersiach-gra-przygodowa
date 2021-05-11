using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    public List<HandingPlace> handingPlaces;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeHandingPlace(ActivityType index)
    {
        int i = (int)index;
        if (i > handingPlaces.Count)
            return;
        transform.parent = handingPlaces[i].handingParent.transform;
        transform.localPosition = handingPlaces[i].handingPosition;
        transform.localEulerAngles = handingPlaces[i].handingRotation;
    }

    [System.Serializable]
    public class HandingPlace
    {
        public GameObject handingParent;
        public Vector3 handingPosition;
        public Vector3 handingRotation;
        public ActivityType workType;
    }

    public enum ActivityType
    {
        WORKING = 0,
        WALKING = 1,
        STANDING = 2,
        SITTING = 3
    }
}
