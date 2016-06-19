using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum MovementState
{
    Idle, Moving, Jumping, Wallrunning, Grabbing, Blinking, Climbing
}

public class ControllerPlayer : MonoBehaviour
{
    public MovementState m_MoveState = MovementState.Idle;
    
    public float m_Friction;
    public float m_BlinkTime = 0.5f;
    public float m_BlinkVelocity = 200.0f;

    bool m_IsBlinking = false;
    float m_BlinkTimer = 0.0f;
    public float m_BlinkCD = 3.0f;
    float m_CurBlinkCD;
    bool m_IsBlinkCD = false;
    public Text m_BlinkCDText;

    Vector3 m_ForwardDir;
    Vector3 m_PlayerVel;
    Vector3 m_hMovement;

    float m_AccelPercent = 0;
    public float m_MovementSpeed;
    public float m_MaxSpeed;
    public float m_JumpForce;

    Rigidbody m_Rigidbody;
    Collider m_Collider;
    Collider m_MeshCol;
    Hands m_PlayerHands;

    bool m_IsGrabbed = false;
    bool m_IsClimbing = false;
    float m_ClimbTimer = 0.0f;
    float m_ColliderTimer = 0.0f;
    bool m_IsColliderActive = true;

    float m_FallTimer = 0.0f;
    bool m_Dampening = false;
    bool m_RequireDampening = false;
    bool m_Dampened = false;
    float m_SlowedTimer = 0.0f;
    bool m_Slowed = false;

    void Start()
    {
        m_PlayerHands = GetComponentInChildren<Hands>();
        m_Collider = GetComponent<Collider>();
        m_MeshCol = GetComponentInChildren<MeshCollider>();
        m_Rigidbody = GetComponent<Rigidbody>();

        m_CurBlinkCD = m_BlinkCD;
    }

    void Update()
    {
        m_Dampening = Input.GetKey("left ctrl");

        CheckState();  

        BlinkUpdate();

        ClimbUpdate();

        FallUpdate();

        TextUpdate();

        //CheckClimb(hMovement);
        if (!m_MoveState.Equals(MovementState.Blinking) && !m_MoveState.Equals(MovementState.Grabbing) && !m_MoveState.Equals(MovementState.Climbing))
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

        //Checks if player is currently climbing
        if (m_IsClimbing)
        {
            m_MoveState = MovementState.Climbing;
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

        if (!m_Slowed)
        {
            m_Rigidbody.velocity = new Vector3(movementVector.x, m_Rigidbody.velocity.y, movementVector.z);
        }
        else
        {
            m_Rigidbody.velocity = new Vector3(movementVector.x / 2, m_Rigidbody.velocity.y, movementVector.z / 2);
        }

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

    void IsGrabbed(bool state)
    {
        m_IsGrabbed = state;
    }

    void FastClimb()
    {
        m_IsClimbing = true;
        m_IsColliderActive = false;

        m_Rigidbody.AddForce(Vector3.up * m_JumpForce * 0.4f, ForceMode.Impulse);
        Vector3 temp = new Vector3(transform.forward.x, 0.0f, transform.forward.z);
        m_Rigidbody.AddForce(temp * m_MovementSpeed * 4, ForceMode.Impulse);

        Debug.Log("Fastclimb");
    }

    void SlowClimb()
    {
        //Debug.Log("SlowClimb");
    }

    void FeetClimb()
    {
        //Safety check
        if (m_hMovement.magnitude > 0.4f && !m_IsClimbing)
        {
            m_IsClimbing = true;
            m_IsColliderActive = false;

            m_Rigidbody.AddForce(Vector3.up * m_JumpForce * 0.2f, ForceMode.Impulse);
            Vector3 temp = new Vector3(transform.forward.x, 0.0f, transform.forward.z);
            m_Rigidbody.AddForce(temp * m_MovementSpeed, ForceMode.Impulse);

            Debug.Log("Feetclimb");
        }
    }

    void BlinkUpdate()
    {
        //Blinking
        if (m_IsBlinking)
        {
            m_BlinkTimer += Time.deltaTime;
            Blink();
        }
        else
        {
            m_BlinkTimer = 0.0f;
        }

        if (!m_IsBlinkCD && Input.GetMouseButtonDown(0))
        {
            m_IsBlinkCD = true;
            ToggleBlink();
        }

        //Clink cooldown
        if (m_IsBlinkCD)
        {
            m_CurBlinkCD -= Time.deltaTime;
            if (m_CurBlinkCD <= 0.0f)
            {
                m_CurBlinkCD = m_BlinkCD;
                m_IsBlinkCD = false;
            }
        }
    }

    void ClimbUpdate()
    {
        //Climbing
        if (m_IsClimbing)
        {
            m_ClimbTimer += Time.deltaTime;
        }
        else
        {
            m_ClimbTimer = 0.0f;
        }

        //Climb timer
        if (m_IsClimbing && m_ClimbTimer > 0.4f)
        {
            m_IsClimbing = false;
        }

        //How long mesh collider is disabled
        if (!m_IsColliderActive)
        {
            m_ColliderTimer += Time.deltaTime;
            m_MeshCol.enabled = false;
        }
        else
        {
            m_MeshCol.enabled = true;
            m_ColliderTimer = 0.0f;
        }

        if (m_ColliderTimer > 0.2f)
        {
            m_IsColliderActive = true;
        }
    }

    void TextUpdate()
    {
        if (!m_IsBlinkCD)
        {
            //Set text to display blink is ready
            m_BlinkCDText.text = "Blink Ready";
            m_BlinkCDText.color = Color.green;
        }
        else
        {
            //Set text to current CD
            m_BlinkCDText.text = m_CurBlinkCD.ToString("F1");
            m_BlinkCDText.color = Color.red;
        }
    }

    void FallUpdate()
    {
        if (m_MoveState == MovementState.Jumping)
        {
            m_FallTimer += Time.deltaTime;
        }
        else
        {
            m_FallTimer = 0.0f;
        }

        if (RaycastDir(Vector3.down))
        {
            if (m_RequireDampening && !m_Dampened)
            {
                //Debug.Log("Slowed");
                m_Slowed = true;
            }
            else if (m_RequireDampening && m_Dampened)
            {
                //Debug.Log("Not Slowed");
                m_Slowed = false;
            }
        }

        if (m_Slowed)
        {
            m_SlowedTimer += Time.deltaTime;
        }
        else
        {
            m_SlowedTimer = 0.0f;
        }

        if (m_SlowedTimer > 0.5f)
        {
            //Debug.Log("No more slow");
            m_Slowed = false;
        }

        if (m_FallTimer > 0.5f)
        {
            m_RequireDampening = true;
        }
        else
        {
            m_RequireDampening = false;
        }

        if (m_RequireDampening && !m_Dampening)
        {
            m_Dampened = false;
        }
        else if (m_RequireDampening && m_Dampening)
        {
            m_Dampened = true;
        }
    }
}