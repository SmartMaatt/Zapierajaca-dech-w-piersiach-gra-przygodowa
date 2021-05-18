using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MakeDamage))]
[RequireComponent(typeof(CharacterController))]
public class RelativeMovement : MonoBehaviour
{
    //Variables
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float runStamina;
    [SerializeField] private float runRestTime;
    [SerializeField] private float rotSpeed;

    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool isJumping;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float gravity;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private LayerMask PortalMask;

    [Space]
    [SerializeField] UIBar _staminaBar;

    private Vector3 _moveDirection;
    private Vector3 _velocity;
    private float _runStaminaCounter;
    private bool _canSprint;

    //References
    private CharacterController _controller;
    private AudioManager _audioManager;
    private MakeDamage _damageScript;
    private Animator _animator;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _audioManager = GetComponent<AudioManager>();
        _damageScript = GetComponent<MakeDamage>();
        _animator = GetComponent<Animator>();

        _runStaminaCounter = runStamina;
        _canSprint = true;
        _staminaBar.setUpBar((int)(runStamina*100));
    }

    private void Update()
    {
        Move();
        CheckForPortal();
    }

    private void Move()
    { 
        if(Physics.CheckSphere(transform.position, groundCheckDistance, groundMask) || Physics.CheckSphere(transform.position, groundCheckDistance, wallMask))
            isGrounded = true;
        else
            isGrounded = false;

        if (isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
            isJumping = false;
        }
       
        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        _moveDirection = new Vector3(horInput, 0, vertInput);

        if (!_damageScript.isAttacting() && !_damageScript.isBlocking())
        {
            if (_moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
            {
                Walk();
                Rotate();
            }
            else if (_canSprint && _moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
            {
                Run();
                Rotate();
            }
            else if (_moveDirection == Vector3.zero || !_canSprint)
            {
                Idle();
            }
        }
        else
        {
            AttackStop();
        }

        _moveDirection *= moveSpeed;
        _moveDirection = Vector3.ClampMagnitude(_moveDirection, moveSpeed);

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        _animator.SetBool("Jumping", isJumping);
        _velocity.y += gravity * Time.deltaTime;

        _controller.Move(_moveDirection * Time.deltaTime);
        _controller.Move(_velocity * Time.deltaTime);
        _staminaBar.setBarValue((int)(_runStaminaCounter * 100));
    }

    private void Idle()
    {
        moveSpeed = 0.0f;
        _animator.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);

        if(_runStaminaCounter < runStamina && _canSprint)
            _runStaminaCounter += Time.deltaTime;
    }

    private void Walk()
    {
        moveSpeed = walkSpeed;
        _animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);

        _audioManager.Play("GrassSteps", UnityEngine.Random.Range(0.2f, 0.4f), 1f, false);

        if (_runStaminaCounter < runStamina && _canSprint)
        {
            if (_damageScript.isEquiped())
                _runStaminaCounter += Time.deltaTime / 4;
            else
                _runStaminaCounter += Time.deltaTime / 2;
        }
    }

    private void Run()
    {
        moveSpeed = runSpeed;
        _animator.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);

        _audioManager.Play("GrassSteps", UnityEngine.Random.Range(0.2f, 0.4f), 1.3f, false);

        if (_damageScript.isEquiped())
            _runStaminaCounter -= Time.deltaTime * 2;
        else
            _runStaminaCounter -= Time.deltaTime;

        if (_runStaminaCounter <= 0.0f)
            StartCoroutine(StopRunning());
    }

    private void AttackStop()
    {
        _animator.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
        _moveDirection = Vector3.zero;
    }

    private void Rotate()
    {
        Quaternion tmp = cameraTransform.rotation; // zachowanie początkowej rotacji
        cameraTransform.eulerAngles = new Vector3(0, cameraTransform.eulerAngles.y, 0);
        _moveDirection = cameraTransform.TransformDirection(_moveDirection); // zmiana kierunku ruchu z lokalnych na globalne
        cameraTransform.rotation = tmp;

        Quaternion direction = Quaternion.LookRotation(_moveDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        _velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        isJumping = true;

        if (!_audioManager.isPlaying("Jump"))
            _audioManager.Play("Jump");
    }

    public void addRunStamina(float stamina)
    {
        runStamina += stamina;
    }

    public void CheckForPortal()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.5f, PortalMask);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.GetComponent<SceneChanger>().ChangeScene();
        }
    }

    public IEnumerator Explosion(float time, float waitTime, float power, float _min, float _max)
    {
        float elapsedTime = 0.0f;
        float max = _max;
        float min = _min;

        yield return new WaitForSeconds(waitTime);

        Vector3 moveDirection = -cameraTransform.forward - new Vector3(0, 1f, 0);

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime / time;
            _controller.Move(moveDirection * power + new Vector3(0, Mathf.Lerp(max,min, elapsedTime), 0));
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator StopRunning()
    {
        _canSprint = false;
        yield return new WaitForSeconds(runRestTime);
        _canSprint = true;
    }
}
