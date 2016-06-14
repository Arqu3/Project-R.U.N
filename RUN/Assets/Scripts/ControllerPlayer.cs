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
    float m_BlinkTimer = 0.0f;

    Vector3 m_ForwardDir;
    Vector3 m_PlayerVel;
    Vector3 m_hMovement;

    float m_AccelPercent = 0;
    public float m_MovementSpeed;
    public float m_MaxSpeed;
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
            m_BlinkTimer += Time.deltaTime;
            Blink();
        }
        else
        {
            m_BlinkTimer = 0.0f;
        }

        if (Input.GetMouseButtonDown(0))
        {
            ToggleBlink();
        }

        //CheckClimb(hMovement);
        if (!m_MoveState.Equals(MovementState.Blinking) && !m_MoveState.Equals(MovementState.Grabbing))
        {
            m_hMovement = new Vector3(Input.GetAxis("Horizontal") * 0.6f, 0, Input.GetAxis("Vertical"));
            HorizontalMovement(m_hMovement);
            CalculateFriction(m_hMovement);
            Jump();
        }
    }
    
    public MovementState GetState()
    {
        return m_MoveState;
    }

    void CheckState()
    {
        //AccelPercent is incremented/decremented here

        //Blink is a special state and is checked first before all others
        if (!m_IsBlinking) { 

            //Checks if player is in air
            if (!RaycastDir(Vector3.down))
            {
                m_AccelPercent = m_AccelPercent + Time.deltaTime * 20;
                m_MoveState = MovementState.Jumping;

                if (m_IsGrabbed)
                {
                    m_AccelPercent = m_AccelPercent - Time.deltaTime * 20;
                    m_MoveState = MovementState.Grabbing;
                }
            }
            //If player is not in air then only following states are possible
            else
            {
                if (m_Rigidbody.velocity.magnitude > 1f || m_hMovement.magnitude > 0.4f)
                {
                    m_AccelPercent = m_AccelPercent + Time.deltaTime * 20;
                    m_MoveState = MovementState.Moving;
                }
                else
                {
                    m_AccelPercent = m_AccelPercent - Time.deltaTime * 150;
                    m_MoveState = MovementState.Idle;
                }
            }
        }
        else
        {
            m_MoveState = MovementState.Blinking;
        }

        m_AccelPercent = Mathf.Clamp(m_AccelPercent, 0, 100);
        
    }

    void HorizontalMovement(Vector3 movementVector)
    {
        float currentMoveSpeed = Mathf.Lerp(m_MovementSpeed, m_MaxSpeed, m_AccelPercent * 0.01f);
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;

        Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);
        movementVector = rotation * movementVector * currentMoveSpeed;

        m_Rigidbody.velocity = new Vector3(movementVector.x, m_Rigidbody.velocity.y, movementVector.z);
 
        //m_Rigidbody.AddRelativeForce(movementVector * currentMoveSpeed, ForceMode.Acceleration); 
    }

    void CalculateFriction(Vector3 movementVector)
    {
        bool gravityBool = true;

        if (m_MoveState.Equals(MovementState.Moving) || m_MoveState.Equals(MovementState.Idle))
        {

            gravityBool = false;
            if (movementVector.magnitude < 0.4 && m_Rigidbody.velocity.magnitude > 0f)
            {
                m_Rigidbody.velocity -= m_Rigidbody.velocity.normalized * m_Friction;
            }

            if (m_MoveState.Equals(MovementState.Idle))
            {
                m_Rigidbody.velocity = Vector3.Lerp(Vector3.zero, m_Rigidbody.velocity, m_AccelPercent * 0.01f);
            }
        }
        ToggleGravity(gravityBool);
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
        if (m_BlinkTimer < m_BlinkTime)
        {
            m_Rigidbody.velocity = m_ForwardDir * m_BlinkVelocity;
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
        m_Rigidbody.AddForce(Vector3.up * m_JumpForce * 0.4f, ForceMode.Impulse);
        m_Rigidbody.AddForce(Camera.main.transform.forward * m_MovementSpeed, ForceMode.Impulse);

        //Debug.Log("FastClimb");
    }

    void SlowClimb()
    {
        //Debug.Log("SlowClimb");
    }
}


