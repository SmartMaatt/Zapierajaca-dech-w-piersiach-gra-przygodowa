using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioManager))]
public class Working : MonoBehaviour
{
    public List<WorkTask> workTasks;

    private Animator _animator;
    private NavMeshAgent _agent;
    private WorkTask currenTask;
    private bool working;
    private bool talking;
    private int taskNumber = 0;
    private int activity = 0;
    private float timeCount = 0.0f;
    private Transform playerTransform;

    private AudioManager _audioManager = null;
    private bool canPlayWorkSound = true;
    private string _workName = null;

    [Header("Audio times")]
    public float startWorkTime;
    public float repeatWorkTime;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _audioManager = GetComponent<AudioManager>();
        currenTask = workTasks[0];
    }

    void Update()
    {
        if (!working)
        {
            switch (activity)
            {
            case 0:
                FindWorkingPlace();
                break;
            case 1:
                GoToWorkingPlace();
                break;
            case 2:
                RotateToWork();
                break;
            case 3:
                StartWork();
                break;
            }
        }
        else if(talking)
        {
            transform.LookAt(playerTransform);
        }
        else if(working && !talking)
        {
           if(canPlayWorkSound)
            {
                StartCoroutine(PlayWorkSound(_workName, repeatWorkTime));
            }
        }
    }

    public void SetWorkType(bool set)
    {
        switch (currenTask.workType)
        {
            case WorkType.STANDING:
                currenTask.workingTool.GetComponent<ToolManager>().ChangeHandingPlace(ToolManager.ActivityType.STANDING);
                _workName = "Standing";
                break;
            case WorkType.FARMING:
                _animator.SetBool("Farming", set);
                _workName = "Farming";
                currenTask.workingTool.GetComponent<ToolManager>().ChangeHandingPlace(ToolManager.ActivityType.WORKING);
                break;
            case WorkType.FISHING:
                _animator.SetBool("Fishing", set);
                currenTask.workingTool.GetComponent<ToolManager>().ChangeHandingPlace(ToolManager.ActivityType.WORKING);
                break;
            case WorkType.GATHERING:
                _animator.SetBool("Gathering", set);
                currenTask.workingTool.GetComponent<ToolManager>().ChangeHandingPlace(ToolManager.ActivityType.WORKING);
                break;
            case WorkType.HAMMERWORKING:
                _animator.SetBool("HammerWorking", set);
                _workName = "HammerWorking";
                currenTask.workingTool.GetComponent<ToolManager>().ChangeHandingPlace(ToolManager.ActivityType.WORKING);
                break;
            case WorkType.MINING:
                _animator.SetBool("Mining", set);
                _workName = "Mining";
                currenTask.workingTool.GetComponent<ToolManager>().ChangeHandingPlace(ToolManager.ActivityType.WORKING);
                break;
            case WorkType.SITTING:
                _animator.SetBool("Sitting", set);
                currenTask.workingTool.GetComponent<ToolManager>().ChangeHandingPlace(ToolManager.ActivityType.SITTING);
                _workName = "sitting";
                break;
            default:
                break;
        }
    }

    private void FindWorkingPlace()
    {
        _audioManager.Stop(_workName);
        _agent.SetDestination(currenTask.workingPlace);
        _animator.SetBool("Walking", true);
        currenTask.workingTool.GetComponent<ToolManager>().ChangeHandingPlace(ToolManager.ActivityType.WALKING);
        activity++;
    }

    private void GoToWorkingPlace()
    {
        Vector3 distanceToWorkingPlace = transform.position - currenTask.workingPlace;
        _audioManager.Play("Step", UnityEngine.Random.Range(0.2f, 0.4f), 1f, false, true);

        if (distanceToWorkingPlace.magnitude < 0.5f)
        {
            _agent.SetDestination(transform.position);
            _animator.SetBool("Walking", false);
            activity++;
        }
    }

    private void RotateToWork()
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        Vector3 pointToRotate = currenTask.workingRotation;
        if (currentRotation.y < 0)
        {
            currentRotation.y += 360;
        }
        if (pointToRotate.y < 0)
        {
            pointToRotate.y += 360;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(pointToRotate), timeCount);
        timeCount += Time.deltaTime / 4;

        Vector3 rotationToWorkingRotation = currentRotation - pointToRotate;
        if (rotationToWorkingRotation.magnitude < 0.2f)
        {
            timeCount = 0;
            activity++;
        }
    }

    public void StartTalking(GameObject player)
    {
        talking = true;
        working = true;
        StopAllCoroutines();
        SetWorkType(false);
        _animator.SetBool("Walking", false);
        _agent.SetDestination(transform.position);
        playerTransform = player.transform;
        transform.LookAt(playerTransform);
    }

    public void StopTalking()
    {
        talking = false;
        activity = 0;
        working = false;
    }

    private void StartWork()
    {
        currenTask.workingTime = UnityEngine.Random.Range(currenTask.minWorkingTime, currenTask.maxWorkingTime);
        SetWorkType(true);
        working = true;

        StartCoroutine(JustWork(currenTask.workingTime));
        StartCoroutine(PlayWorkSound(_workName, startWorkTime));
    }

    private IEnumerator JustWork(float workingTime)
    {
        yield return new WaitForSeconds(workingTime);
        SetWorkType(false);
        activity = 0;
        taskNumber++;
        if (taskNumber >= workTasks.Count)
        {
            taskNumber = 0;
        }
        currenTask = workTasks[taskNumber];

        working = false;
        yield return null;
    }

    private IEnumerator PlayWorkSound(string workName, float time)
    {
        if (workName == "HammerWorking" || workName == "Mining" || workName == "Farming")
        {
            canPlayWorkSound = false;

            yield return new WaitForSeconds(time);
            _audioManager.Play(workName);

            canPlayWorkSound = true;
        }
    }

    [System.Serializable]
    public class WorkTask
    {
        public GameObject workingTool;
        public Vector3 workingPlace;
        public Vector3 workingRotation;
        public WorkType workType;
        public float minWorkingTime;
        public float maxWorkingTime;
        public float workingTime;
    }

    public enum WorkType
    {
        WALKING = 0,
        STANDING = 1,
        FARMING = 2,
        FISHING = 3,
        GATHERING = 4,
        HAMMERWORKING = 5,
        MINING = 6,
        SITTING = 7
    }
}
