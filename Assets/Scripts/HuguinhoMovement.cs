using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HuguinhoMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] InputActionReference inputMove;
    [SerializeField] InputActionReference inputRun;
    [SerializeField] float baseSpeed = 6;
    [SerializeField] float runSpeed = 10;
    [SerializeField] float rayLength = 1.1f;
    [SerializeField] LayerMask floorLayer;
    bool isOnIce;
    bool isTouchingWall;
    float wallTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
# if UNITY_EDITOR
        if (inputMove != null)
            Debug.Log("inputMove : OK");
        else
            Debug.Log("inputMove : Missing");

        if (inputMove != null)
            Debug.Log("rigidbody : OK");
        else
            Debug.Log("rigidbody : Missing");
#endif
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        FloorCheck();
        HandleWallTouch();
        HandleMovement();
    }

    void FloorCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayLength, floorLayer))
        {

            isOnIce = hit.collider.CompareTag("Ice");
            rigidbody.useGravity = false;
        }
        else
        {
            isOnIce = false;
            rigidbody.useGravity = true;
        }
    }

    void HandleMovement()
    {
        if (!isOnIce || (isTouchingWall && wallTimer >= 0.5f))
        {
            Vector2 moveInputValue = inputMove.action.ReadValue<Vector2>();

            if (moveInputValue.magnitude >= 0.01f)
            {
                float currentSpeed = baseSpeed;

                if (inputRun.action.phase == InputActionPhase.Performed)
                    currentSpeed = runSpeed;

                SetLinearVelocity(moveInputValue, currentSpeed);
            }
            else
            {
                SetLinearVelocity(Vector2.zero, 0);
            }
        }
    }

    void HandleWallTouch()
    {
        if (isOnIce && isTouchingWall)
            wallTimer += Time.deltaTime;
        else if (!isTouchingWall)
            wallTimer = 0;
    }

    private void SetLinearVelocity(Vector2 direction, float speed)
    {
        rigidbody.linearVelocity = new Vector3(
            direction.x * speed,
            rigidbody.linearVelocity.y,
            direction.y * speed
        );
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;
            //wallTimer += Time.deltaTime;
            //Debug.Log("Collision Stay : Hit Wall. wallTimer: " + wallTimer);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;
            wallTimer = 0;
            //Debug.Log("Collision Exit : Exit wall. wallTimer: " + wallTimer);
        }
    }
}
