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
    public Canvas dialoguseCanvas = null;
    public Image dialogueArea = null;
    public GameObject responseButtonPrefab = null;
    public GameObject exitButtonPrefab = null;
    public List<GameObject> responseButtons = null;
    public TextMeshProUGUI characterNameText = null;
    public TextMeshProUGUI characterSentenceText = null;
    public GameObject player = null;
    public new GameObject camera = null;

    public void Startup()
    {
        Debug.Log("Uruchomienie menadżera dialogów...");
        // ODKOMENTOWAĆ TO
        //dialoguseCanvas.enabled = false;
        status = ManagerStatus.Started;
    }

    public void StartDialogue(Dialogue firstDialogue, string peasantName)
    {
        if (!dialoguseCanvas.enabled)
        {
            player.GetComponent<RelativeMovement>().enabled = false;
            player.GetComponent<Animator>().SetFloat("Speed", 0f);
            camera.GetComponent<CameraFollow>().enabled = false;
            dialoguseCanvas.enabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            characterNameText.text = peasantName;
            ShowDialogue(firstDialogue);
        }
    }

    public void ShowDialogue(Dialogue newDialogue)
    {
        foreach(GameObject button in responseButtons)
        {
            //responseButtons.Remove(button);
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
        camera.GetComponent<CameraFollow>().enabled = true;
        dialoguseCanvas.enabled = false;
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
}
