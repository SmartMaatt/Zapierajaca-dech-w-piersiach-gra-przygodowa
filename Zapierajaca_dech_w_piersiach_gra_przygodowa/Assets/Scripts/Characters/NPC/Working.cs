using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Working : MonoBehaviour
{
    public List<WorkTask> workTasks;

    private Animator _animator;
    private WorkTask currenTask;
    private bool working;
    private int taskNumber = 0;

    private const float minWorkingTime = 2f;
    private const float maxWorkingTime = 5f;
    private float rotSpeed = 5.0f;
    private float Speed = 0.15f;

    void Start()
    {
        _animator = GetComponent<Animator>();
        currenTask = workTasks[0];
    }

    void Update()
    {
        if (!working)
        {
            if (taskNumber >= workTasks.Count)
            {
                taskNumber = 0;
            }

            SetWorkType(false);
            currenTask = workTasks[taskNumber];

            if (transform.position != currenTask.workingPlace)
            {
                transform.position = currenTask.workingPlace;
                if(transform.position == currenTask.workingPlace)
                {
                    Quaternion direction = Quaternion.Euler(currenTask.workingRotation);
                    transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
                }
                //transform.position = Vector3.Lerp(transform.position, currenTask.workingPlace, Speed * Time.deltaTime);
            }
            else
            {
                currenTask.workingTime = UnityEngine.Random.Range(minWorkingTime, maxWorkingTime);
                _animator.SetBool("Walking", false);
                SetWorkType(true);
                working = true;
                StartCoroutine(JustWork(currenTask.workingTime));
            }
        }
    }

    private IEnumerator JustWork(float workingTime)
    {
        yield return new WaitForSeconds(workingTime);
        working = false;
        taskNumber++;
    }

    public void SetWorkType(bool set)
    {
        switch (currenTask.workType)
        {
            case WorkType.FARMING:
                _animator.SetBool("Farming", set);
                break;
            case WorkType.FISHING:
                _animator.SetBool("Fishing", set);
                break;
            case WorkType.GATHERING:
                _animator.SetBool("Gathering", set);
                break;
            case WorkType.HAMMERWORKING:
                _animator.SetBool("Hammerworking", set);
                break;
            case WorkType.MINNING:
                _animator.SetBool("Minning", set);
                break;
            default:
                break;
        }
    }

    [System.Serializable]
    public class WorkTask
    {
        public GameObject WorkingTool;
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
        MINNING = 6
    }
}
