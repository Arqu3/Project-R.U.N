using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum MovementState
{
    Idle, Moving, Jumping, Wallrunning, Grabbing, Blinking, Climbing
}

public class ControllerPlayer : MonoBehaviour
{
    //Public vars
    public MovementState m_MoveState = MovementState.Idle;
    public Text m_BlinkCDText;
    public float m_Friction;
    public float m_BlinkTime = 0.5f;
    public float m_BlinkVelocity = 200.0f;
    public float m_BlinkCD = 3.0f;
    public float m_MovementSpeed;
    public float m_MaxSpeed;
    public float m_JumpForce;
    public float m_FallThreshold = 1.0f;

    //Basic movement vars
    Vector3 m_ForwardDir;
    Vector3 m_PlayerVel;
    Vector3 m_hMovement;
    float m_AccelPercent = 0;

    //Component vars
    Rigidbody m_Rigidbody;
    Collider m_Collider;
    Collider m_MeshCol;
    Hands m_PlayerHands;
    Sides m_MySides;

    //Blink vars
    bool m_IsBlinking = false;
    bool m_IsBlinkCD = false;
    float m_BlinkTimer = 0.0f;
    float m_CurBlinkCD;

    //Ledgegrab vars
    bool m_IsColliderActive = true;
    bool m_IsGrabbed = false;
    bool m_IsClimbing = false;
    float m_ClimbTimer = 0.0f;
    float m_ColliderTimer = 0.0f;

    //Fall and dampen vars
    bool m_Dampening = false;
    bool m_RequireDampening = false;
    bool m_Dampened = false;
    bool m_Slowed = false;
    float m_SlowedTimer = 0.0f;
    float m_FallTimer = 0.0f;
    float m_DampeningTimer = 0.0f;

    //Wallrun vars
    bool m_IsWallrunning;
    bool m_WallrunDirSet;
    bool m_WallrunInterrupted;
    Vector3 m_WallrunDir;
    const float m_WallrunAngle = 30;

    void Start()
    {
        m_PlayerHands = GetComponentInChildren<Hands>();
        m_MySides = GetComponentInChildren<Sides>();
        m_Collider = GetComponent<Collider>();
        m_MeshCol = GetComponentInChildren<MeshCollider>();
        m_Rigidbody = GetComponent<Rigidbody>();

        m_CurBlinkCD = m_BlinkCD;
    }

    void Update()
    {
        if (m_Dampening && m_DampeningTimer < 0.4f)
        {
            m_DampeningTimer += Time.deltaTime;
        }
        else
        {
            m_DampeningTimer = 0.0f;
            m_Dampening = Input.GetKeyDown("left ctrl");
        }

        CheckState();

        BlinkUpdate();

        ClimbUpdate();

        FallUpdate();

        CheckWallrun();

        TextUpdate();

        //CheckClimb(hMovement);
        if (!m_MoveState.Equals(MovementState.Blinking) && !m_MoveState.Equals(MovementState.Grabbing) && !m_MoveState.Equals(MovementState.Climbing))
        {
            m_hMovement = new Vector3(Input.GetAxis("Horizontal") * 0.6f, 0, Input.GetAxis("Vertical"));
            CalculateFriction(m_hMovement);
            JumpUpdate();
            HorizontalMovement(m_hMovement);
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
            if (!m_IsWallrunning) {  

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
                m_AccelPercent = m_AccelPercent + Time.deltaTime * 20;
                m_MoveState = MovementState.Wallrunning;
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

        if (m_MoveState.Equals(MovementState.Wallrunning))
        {
            
            movementVector = m_WallrunDir * Mathf.Clamp01(movementVector.z) * currentMoveSpeed;   
        }
        else
        { 
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;

            Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);
            movementVector = rotation * movementVector * currentMoveSpeed;
        }

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

        if (m_MoveState.Equals(MovementState.Wallrunning)) {
            gravityBool = false;
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, -10f + movementVector.z * 9.5f, m_Rigidbody.velocity.z);
        }

        ToggleGravity(gravityBool);

    }

    void JumpUpdate()
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

    void FeetClimb()
    {
        //Safety check
        if (m_hMovement.magnitude > 0.4f && !m_IsClimbing && m_Rigidbody.velocity.y > -2.0f)
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

        if (m_FallTimer > m_FallThreshold)
        {
            m_RequireDampening = true;
        }
        else
        {
            m_RequireDampening = false;
        }

        if (m_RequireDampening && m_Dampening)
        {
            m_Dampened = true;
        }
        else
        {
            m_Dampened = false;
        }
    }

    void CheckWallrun()
    {
        if (m_MySides.m_CanWallrun && m_MoveState.Equals(MovementState.Jumping))
        {
            m_IsWallrunning = true;

            StartWallrun();
        }
        
        if (!m_MySides.m_CanWallrun || Input.GetKeyDown(KeyCode.Space))
        {
            m_IsWallrunning = false;
            m_WallrunDirSet = false;
        }


        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_WallrunInterrupted = true;
        }
        */
    }

    void StartWallrun()
    {
        if (m_IsWallrunning && !m_WallrunDirSet)
        {
            m_WallrunDirSet = true;

            Vector3[] tempV = m_MySides.GetColliderInfo();

            Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z);

            for (int t = 0; t < 2; t++) {  
                for (int i = 0; i < 4; i++){
                    if (Vector3.Angle(t * 2 * -tempV[i] +tempV[i], forward) < m_WallrunAngle)
                    {
                        m_WallrunDir = (t * 2 * -tempV[i] + tempV[i]).normalized;
                        t = 2;
                        i = 4;
                    }
                }
            }
        }
    }
}