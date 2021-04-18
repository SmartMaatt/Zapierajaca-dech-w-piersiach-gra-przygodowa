using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class RelativeMovement : MonoBehaviour
{
    [SerializeField] private Transform target;

    public float rotSpeed = 15.0f;
    public float moveSpeed = 6.0f;

    public float jumpSpeed = 15.0f;
    public float gravity = -9.81f;
    public float terminalVelocity = -10.0f;
    public float minFall = -1.5f;
    public float pusForce = 3.0f;

    private float _vertSpeed;
    private ControllerColliderHit _contact;
    private Animator _animator;

    private CharacterController _charController;

    private void Start()
    {
        _vertSpeed = minFall; // inicjalizacja do minimalnej prędkości spadania
        _charController = GetComponent<CharacterController>(); // uzyskanie dostępu do CharacterController
        _animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (_animator.GetBool("Attacking") == true)
            return;
        Vector3 movement = Vector3.zero; // Vector3.zero to (0, 0, 0)
        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");

        if (horInput != 0 || vertInput != 0)
        {
            movement.x = horInput * moveSpeed;
            movement.z = vertInput * moveSpeed;
            movement = Vector3.ClampMagnitude(movement, moveSpeed); // ograniczenie prędkości ruchu po przekątnej

            Quaternion tmp = target.rotation; // zachowanie początkowej rotacji
            target.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
            movement = target.TransformDirection(movement); // zmiana kierunku ruchu z lokalnych na globalne
            target.rotation = tmp;

            //transform.rotation = Quaternion.LookRotation(movement); // obliczane kwternionu aby skierować obiekt w stronę movement
            Quaternion direction = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
        }

        bool hitGround = false;
        RaycastHit hit;
        if (_vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float check = (_charController.height + _charController.radius) / 1.9f - _charController.center.y;
            hitGround = hit.distance <= check;
        }

        _animator.SetFloat("Speed", movement.sqrMagnitude);

        if (hitGround)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _vertSpeed = jumpSpeed;
            }
            else
            {
                _vertSpeed = minFall;
            }
            if (_contact != null)
            {
                if (_animator.GetBool("Jumping") == true)
                    _animator.SetBool("Jumping", false);
            }
        }
        else
        {
            _vertSpeed += gravity * 5 * Time.deltaTime;
            if (_vertSpeed < terminalVelocity)
            {
                _vertSpeed = terminalVelocity;
            }
            if (_contact != null)
            {
                _animator.SetBool("Jumping", true);
            }

            if (_charController.isGrounded)
            {
                if (Vector3.Dot(movement, _contact.normal) < 0) // sprawdzenie w jaką stronę zwrócona jest postać
                {
                    movement = _contact.normal * moveSpeed;
                }
                else
                {
                    movement += _contact.normal * moveSpeed;
                }
            }

        }
        movement.y = _vertSpeed;

        movement *= Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && _animator.GetBool("Jumping") == false && _animator.GetBool("Attacking") == false)
        {
            _animator.SetBool("Attacking", true);
            StartCoroutine(StopAttack());
        }
        _charController.Move(movement);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _contact = hit;
        Rigidbody body = hit.collider.attachedRigidbody; // czy obiekt zderzony posiada fizykę
        if (body != null && !body.isKinematic)
            body.velocity = hit.moveDirection * pusForce; // nadanie prędkości obiektowi
    }

    private IEnumerator StopAttack()
    {
        yield return new WaitForSeconds(0.75f);
        _animator.SetBool("Attacking", false);
    }
}
