using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    // ZABRAĆ STĄD NULL
    public GameObject dialoguseCanvas = null;
    public GameObject dialogueArea = null;
    public GameObject responseButtonPrefab = null;
    public GameObject exitButtonPrefab = null;
    public List<GameObject> responseButtons = null;
    public TextMeshProUGUI characterNameText = null;
    public TextMeshProUGUI characterSentenceText = null;
    public GameObject profilePart = null;
    public GameObject skillsPart = null;
    public GameObject player = null;
    public GameObject cameraBase = null;
    public List<Dialogue> allTalkableOnceDialogues;
    private PeasantCharacter peasantTalkingTo;

    public void Startup()
    {
        Debug.Log("Uruchomienie menadżera dialogów...");
        // ODKOMENTOWAĆ TO
        if (dialoguseCanvas)
        {
            dialoguseCanvas.SetActive(false);
        }
        ResetAllDialogues();
        status = ManagerStatus.Started;
    }

    public void StartDialogue(PeasantCharacter peasant)
    {
        if (!dialoguseCanvas.activeSelf)
        {
            player.GetComponent<RelativeMovement>().enabled = false;
            player.GetComponent<Animator>().SetFloat("Speed", 0f);
            cameraBase.GetComponent<CameraFollow>().enabled = false;
            dialoguseCanvas.SetActive(true);
            profilePart.SetActive(false);
            skillsPart.SetActive(false);
            
            peasantTalkingTo = peasant;
            Working targetWorking = peasant.GetComponent<Working>();
            targetWorking.StartTalking(player);
            //targetWorking.enabled = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            characterNameText.text = peasant.peasantName;
            ShowDialogue(peasant.dialogue);
        }
    }

    public void ShowDialogue(Dialogue newDialogue)
    {
        foreach(GameObject button in responseButtons)
        {
            Destroy(button);
        }

        characterSentenceText.text = newDialogue.sentence;
        foreach (Dialogue.Response response in newDialogue.responses)
        {
            if(response.talkable != Dialogue.Talkable.DONE)
            {
                if (response.talkable == Dialogue.Talkable.QUEST && !Managers.Quest.DoesThisDialoguInQuestExist(response.nextDialogue))
                { }
                else
                {
                    GameObject button = Instantiate(responseButtonPrefab);
                    GameObject buttonText = button.transform.GetChild(0).gameObject;
                    buttonText.GetComponent<TextMeshProUGUI>().text = response.sentence;
                    switch (response.talkable)
                    {
                        case Dialogue.Talkable.INFINITE:
                            button.GetComponent<Button>().onClick.AddListener(delegate { ShowDialogue(response.nextDialogue); });
                            break;
                        case Dialogue.Talkable.ONCE:
                            button.GetComponent<Button>().onClick.AddListener(delegate { ResponseOnce(response); });
                            break;
                        case Dialogue.Talkable.QUEST:
                            button.GetComponent<Button>().onClick.AddListener(delegate { ResponseQuest(response); });
                            break;
                    }
                    button.transform.SetParent(dialogueArea.transform);
                    responseButtons.Add(button);
                }
            }
        }

        if (newDialogue.canExit)
        {
            GameObject exitButton = Instantiate(exitButtonPrefab);
            exitButton.GetComponent<Button>().onClick.AddListener(delegate { EndDialogue(); });
            exitButton.transform.SetParent(dialogueArea.transform);
            responseButtons.Add(exitButton);
        }
    }

    public void EndDialogue()
    {
        player.GetComponent<RelativeMovement>().enabled = true;
        cameraBase.GetComponent<CameraFollow>().enabled = true;
        dialoguseCanvas.SetActive(false);
        profilePart.SetActive(true);
        skillsPart.SetActive(true);

        Working targetWorking = peasantTalkingTo.GetComponent<Working>();
        //targetWorking.enabled = true;
        targetWorking.StopTalking();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ResponseOnce(Dialogue.Response response)
    {
        response.talkable = Dialogue.Talkable.DONE;
        ShowDialogue(response.nextDialogue);
    }
    public void ResponseQuest(Dialogue.Response response)
    {
        Managers.Quest.CheckTalkToQuest(response.nextDialogue);
        ShowDialogue(response.nextDialogue);
    }

    public void ResetAllDialogues()
    {
        foreach (Dialogue dialogue in allTalkableOnceDialogues)
        {
            foreach (Dialogue.Response response in dialogue.responses)
            {
                if (response.talkable == Dialogue.Talkable.DONE)
                {
                    response.talkable = Dialogue.Talkable.ONCE;
                }
            }
        }
    }
}
