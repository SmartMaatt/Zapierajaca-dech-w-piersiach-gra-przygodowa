using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine;

public class LookAroundRigging : MonoBehaviour
{
    public Vector3 defaultTargetPos;
    public Vector3 headHeight;
    [SerializeField] GameObject Target;
    [SerializeField] GameObject HeadAim;
    [SerializeField] GameObject ChestAim;

    private void Awake()
    {
        changeWeight(0.0f, 0.0f);    
    }

    public void changeWeight(float headWeight, float chestWeight)
    {
        if (headWeight >= 0 && headWeight <= 1)
            HeadAim.GetComponent<MultiAimConstraint>().weight = headWeight;
        else
            Debug.Log("Given argument: " + headWeight + " - HeadWeight has to be in range <0,1>!");

        if (chestWeight >= 0 && chestWeight <= 1)
            ChestAim.GetComponent<MultiAimConstraint>().weight = chestWeight;
        else
            Debug.Log("Given argument: " + chestWeight + " - ChestWeight has to be in range <0,1>!");
    }

    public void changeCurrentTargetPos(Vector3 targetPos)
    {
        if (targetPos != Vector3.zero)
            Target.transform.position = targetPos + transform.forward * 1.5f;
        else
            Target.transform.position = defaultTargetPos;
    }

    public IEnumerator lookAroundAnimation(float time)
    {
        float elapsedTime = 0;
        changeWeight(1.0f, 1.0f);
        Vector3 startingPos = new Vector3(transform.position.x + headHeight.x, transform.position.y + headHeight.y, transform.position.z + headHeight.z) + transform.forward * 1.0f;

        int index = 0;
        float[] x = new float[] { 0.0f, -1.0f, 0.0f, 1.0f };
        float[] y = new float[] { -1.0f, 0.0f, 1.0f, 0.0f };

        while (elapsedTime < 4)
        {
            Debug.Log(elapsedTime);
            Debug.Log(ChestAim.GetComponent<MultiAimConstraint>().weight);
            elapsedTime += Time.deltaTime / (time / 4);
            changeCurrentTargetPos(startingPos + transform.right * Mathf.Lerp(x[index], y[index], elapsedTime % 1));
            index = (int)(elapsedTime);
            yield return new WaitForEndOfFrame();
        }

        changeWeight(0.0f, 0.0f);
    }
}
