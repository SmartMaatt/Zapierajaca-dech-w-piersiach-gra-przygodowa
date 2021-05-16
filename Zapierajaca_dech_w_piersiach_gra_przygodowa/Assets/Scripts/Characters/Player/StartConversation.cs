using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartConversation : MonoBehaviour
{
    public float radius = 1.5f;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider hitCollider in hitColliders)
            {
                Vector3 direction = hitCollider.transform.position - transform.position;
                if (Vector3.Dot(transform.forward, direction) > 0.5f)
                {
                    PeasantCharacter target = hitCollider.GetComponent<PeasantCharacter>();
                    if (target != null)
                    {
                        Managers.Dialogue.StartDialogue(target.dialogue, target.peasantName);
                        //Working targetWorking = target.GetComponent<Working>();
                        //targetWorking.StartTalking();
                    }
                }
            }
        }
    }
}
