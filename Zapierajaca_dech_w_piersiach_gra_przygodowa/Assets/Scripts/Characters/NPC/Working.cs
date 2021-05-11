using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Working : MonoBehaviour
{
    public List<WorkTask> workTasks;

    private Animator _animator;
    private NavMeshAgent _agent;
    private WorkTask currenTask;
    private bool working;
    private int taskNumber = 0;
    private int activity = 0;
    private float timeCount = 0.0f;

    private const float minWorkingTime = 2f;
    private const float maxWorkingTime = 5f;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
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
    }

    public void SetWorkType(bool set)
    {
        switch (currenTask.workType)
        {
            case WorkType.STANDING:
                currenTask.workingTool.GetComponent<ToolManager>().ChangeHandingPlace(ToolManager.ActivityType.STANDING);
                break;
            case WorkType.FARMING:
                _animator.SetBool("Farming", set);
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
                currenTask.workingTool.GetComponent<ToolManager>().ChangeHandingPlace(ToolManager.ActivityType.WORKING);
                break;
            case WorkType.MINNING:
                _animator.SetBool("Minning", set);
                currenTask.workingTool.GetComponent<ToolManager>().ChangeHandingPlace(ToolManager.ActivityType.WORKING);
                break;
            case WorkType.SITTING:
                _animator.SetBool("Sitting", set);
                currenTask.workingTool.GetComponent<ToolManager>().ChangeHandingPlace(ToolManager.ActivityType.SITTING);
                break;
            default:
                break;
        }
    }

    private void FindWorkingPlace()
    {
        _agent.SetDestination(currenTask.workingPlace);
        _animator.SetBool("Walking", true);
        currenTask.workingTool.GetComponent<ToolManager>().ChangeHandingPlace(ToolManager.ActivityType.WALKING);
        activity++;
    }

    private void GoToWorkingPlace()
    {
        Vector3 distanceToWorkingPlace = transform.position - currenTask.workingPlace;
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
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(pointToRotate), timeCount);
        timeCount += Time.deltaTime / 4;
        if (currentRotation == pointToRotate)
        {
            timeCount = 0;
            activity++;
        }
    }

    private void StartWork()
    {
        currenTask.workingTime = UnityEngine.Random.Range(minWorkingTime, maxWorkingTime);
        SetWorkType(true);
        working = true;
        StartCoroutine(JustWork(currenTask.workingTime));
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

    [System.Serializable]
    public class WorkTask
    {
        public GameObject workingTool;
        public Vector3 workingPlace;
        public Vector3 workingRotation;
        public WorkType workType;
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
        MINNING = 6,
        SITTING = 7
    }
}
