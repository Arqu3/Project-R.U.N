using UnityEngine;
using System.Collections;

public enum MovementState
{
    Idle, Moving, Jumping, Wallrunning, Grabbing, Blinking
}

public class ControllerPlayer : MonoBehaviour
{
 
    MovementState m_MoveState = MovementState.Idle;
    
    public float m_Friction;
    public float m_BlinkTime = 0.5f;
    public float m_BlinkVelocity = 200.0f;

    bool m_IsBlinking = false;
    float m_Timer = 0.0f;

    Vector3 m_ForwardDir;
    Vector3 m_PlayerVel;

    public float m_MovementSpeed;
    public float m_JumpForce;

    Rigidbody m_Rigidbody;
    Collider m_Collider;
    Hands m_PlayerHands;

    bool m_IsGrabbed = false;

    void Start()
    {
        m_PlayerHands = GetComponentInChildren<Hands>();
        m_Collider = GetComponent<Collider>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckState();

        //Blink timer
        if (m_IsBlinking)
        {
            m_Timer += Time.deltaTime;
            Blink();
        }
        else
        {
            m_Timer = 0.0f;
        }

        if (Input.GetMouseButtonDown(0))
        {
            ToggleBlink();
        }

        //CheckClimb(hMovement);
        if (!m_IsGrabbed)
        {
            Vector3 hMovement = new Vector3(Input.GetAxis("Horizontal") * 0.6f, 0, Input.GetAxis("Vertical"));
            HorizontalMovement(hMovement);
            Jump();
        }
    }
    
    public MovementState GetState()
    {
        return m_MoveState;
    }

    void CheckState()
    {
        if (!m_IsBlinking) { 
        //Checks if player is in air
            if (!RaycastDir(Vector3.down))
            {
                m_MoveState = MovementState.Jumping;

                if (m_IsGrabbed)
                {
                    m_MoveState = MovementState.Grabbing;
                }
            }
            //If player is not in air then only following states are possible
            else
            {
                Debug.Log(m_Rigidbody.velocity.magnitude);

                if (m_Rigidbody.velocity.magnitude > 1f)
                {
                    m_MoveState = MovementState.Moving;
                }
                else
                {
                    m_MoveState = MovementState.Idle;
                }
            }
        }
        else
        {
            m_MoveState = MovementState.Blinking;
        }
    }

    void HorizontalMovement(Vector3 inputVector)
    {
        Vector3 movementVector = inputVector;
        bool gravityBool;

        if (m_MoveState.Equals(MovementState.Moving) || m_MoveState.Equals(MovementState.Idle)) {

            gravityBool = false;
            if (movementVector.magnitude < 0.4 && m_Rigidbody.velocity.magnitude > 0f)
            {
                m_Rigidbody.velocity -= m_Rigidbody.velocity.normalized * m_Friction;
            }
        }
        else
        {
            gravityBool = true;
        }

        ToggleGravity(gravityBool);
        m_Rigidbody.AddRelativeForce(inputVector * m_MovementSpeed, ForceMode.Acceleration);


    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && RaycastDir(Vector3.down))
        {
            m_Rigidbody.AddForce(m_JumpForce * Vector3.up, ForceMode.Impulse);
        }
    }

    bool RaycastDir(Vector3 direction)
    {
        Vector3 v = new Vector3(m_Collider.bounds.center.x, m_Collider.bounds.min.y, m_Collider.bounds.center.z);

        Ray ray = new Ray(v, direction);

        Debug.DrawRay(v, direction);

        if (Physics.Raycast(ray, 1f))
        {
            return true;
        }

        return false;
    }

    public void ToggleGravity(bool active)
    {
        m_Rigidbody.useGravity = active;
    }

    void ToggleBlink()
    {
        //Get forward direction
        m_ForwardDir = Camera.main.transform.forward;

        //Store velocity
        m_PlayerVel = m_Rigidbody.velocity;

        if (!m_IsBlinking)
        {
            ToggleGravity(false);
            m_IsBlinking = true;
        }
    }

    void Blink()
    {
        //Add velocity
        if (m_Timer < m_BlinkTime)
        {
            m_Rigidbody.AddForce(m_ForwardDir * m_BlinkVelocity);
        }
        else
        {
            ToggleGravity(true);
            m_IsBlinking = false;
            m_Rigidbody.velocity = m_PlayerVel;
        }
    }

    void CheckClimb(Vector3 inputVector)
    {
        if (m_PlayerHands.m_CanClimb && inputVector.magnitude > 0.4f)
        {
            m_Rigidbody.AddForce(Vector3.up * 3.0f, ForceMode.Impulse);
        }
    }

    void IsGrabbed()
    {
        m_IsGrabbed = true;
    }

    void IsntGrabbed()
    {
        m_IsGrabbed = false;
    }

    void FastClimb()
    {
        m_Rigidbody.AddForce(Vector3.up * m_JumpForce * 0.9f, ForceMode.Impulse);
        m_Rigidbody.AddForce(Vector3.forward * m_MovementSpeed, ForceMode.Impulse);

        Debug.Log("FastClimb");
    }

    void SlowClimb()
    {
        Debug.Log("SlowClimb");
    }
}


