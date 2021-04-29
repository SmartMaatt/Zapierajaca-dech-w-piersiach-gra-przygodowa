using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine;

public class TargetHeadAim : MonoBehaviour
{
    public Vector3 defaultTargetPos;
    [SerializeField] GameObject Target;
    [SerializeField] GameObject HeadAim;
    [SerializeField] GameObject ChestAim;
    Transform player;

    void Start()
    {
        player = FindObjectsOfType<RelativeMovement>()[0].transform;
    }

    public void changeWeight(float headWeight, float chestWeight) {

       if(headWeight >= 0 && headWeight <= 1) {
            HeadAim.GetComponent<MultiAimConstraint>().weight = headWeight;
        }
        else{
            Debug.Log("Given argument: " + headWeight + " - HeadWeight has to be in range <0,1>!");
        }

        if (chestWeight >= 0 && chestWeight <= 1) {
            ChestAim.GetComponent<MultiAimConstraint>().weight = chestWeight;
        }
        else {
            Debug.Log("Given argument: " + chestWeight + " - ChestWeight has to be in range <0,1>!");
        }
    }

    public void changeCurrentTargetPos(Vector3 targetPos)
    {
        if (targetPos != Vector3.zero) {
            Target.transform.position = targetPos + transform.forward * 1.5f;
        }
        else {
            Target.transform.position = defaultTargetPos;
        }
    }

    public void changeTargetToPlayer(float playerHead)
    {
        Target.transform.position = new Vector3(player.position.x, player.position.y + playerHead, player.position.z);
    }
}
