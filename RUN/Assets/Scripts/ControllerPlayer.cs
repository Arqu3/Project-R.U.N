using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum MovementState
{
    Idle, Moving, Jumping, Falling, Wallrunning, Grabbing, Blinking, Climbing
}

public class ControllerPlayer : MonoBehaviour
{
    //Public vars
    public MovementState m_MoveState = MovementState.Idle;
    public float m_Friction;
    public float m_BlinkTime = 0.5f;
    public float m_BlinkVelocity = 200.0f;
    public float m_BlinkCD = 3.0f;
    public float m_MovementSpeed;
    public float m_MaxSpeed;
    public float m_JumpForce;
    public float m_FallThreshold = 1.0f;
    public LayerMask m_BlinkMask;
    public LayerMask m_JumpMask;
    public bool m_IsAirControl = false;
    public float m_SlowMultiplier = 0.5f;
    public float m_SlowTime = 0.5f;
    public float m_DampeningTime = 0.4f;

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
    float m_DistanceTravelled = 0.0f;
    RaycastHit m_Hit;
    Ray m_Ray;
    Vector3 m_BPlayerVel;
    Text m_BlinkCDText;

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
    bool m_WallrunRight;
    Vector3 m_WallrunDir;
    const float m_WallrunAngle = 45;

    bool m_isMovingFromInput;
    Vector3 m_lastPosition;
    int m_NotMovingCount = 0;

    void Start()
    {
        m_PlayerHands = GetComponentInChildren<Hands>();
        m_MySides = GetComponentInChildren<Sides>();
        m_Collider = GetComponent<Collider>();
        m_Rigidbody = GetComponent<Rigidbody>();

        m_MeshCol = transform.FindChild("Collider").GetComponent<CapsuleCollider>();

        m_BlinkCDText = GameObject.Find("BlinkText").GetComponent<Text>();
        m_CurBlinkCD = m_BlinkCD;
    }

    void Update()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);

        DampeningUpdate();

        CheckNotMovingFromInput();

        CheckState();

        BlinkUpdate();

        ClimbUpdate();

        FallUpdate();

        CheckWallrun();

        TextUpdate();

        if (!m_MoveState.Equals(MovementState.Blinking) && !m_MoveState.Equals(MovementState.Grabbing) && !m_MoveState.Equals(MovementState.Climbing))
        {
            if (!m_IsAirControl)
            {
                //Disable horizontal movement while in air
                if (!m_MoveState.Equals(MovementState.Jumping) && !m_MoveState.Equals(MovementState.Falling))
                {
                    m_hMovement = new Vector3(Input.GetAxis("Horizontal") * 0.6f, 0, Input.GetAxis("Vertical"));
                    HorizontalMovement(m_hMovement);
                }
                CalculateFriction(m_hMovement);
                JumpUpdate();
            }
            else
            {
                //Horizontal movement is enabled while in air
                m_hMovement = new Vector3(Input.GetAxis("Horizontal") * 0.6f, 0, Input.GetAxis("Vertical"));
                HorizontalMovement(m_hMovement);
                CalculateFriction(m_hMovement);
                JumpUpdate();
            }
        }

        Debug.DrawRay(transform.position, m_ForwardDir, Color.red);
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

                    if (m_Rigidbody.velocity.y > 0f) { 
                        m_AccelPercent = m_AccelPercent + Time.deltaTime * 20;
                        m_MoveState = MovementState.Jumping;
                    }
                    else
                    {
                        m_AccelPercent = m_AccelPercent + Time.deltaTime * 10;
                        m_MoveState = MovementState.Falling;
                    }
                    
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
            m_Rigidbody.velocity = new Vector3(movementVector.x * m_SlowMultiplier, m_Rigidbody.velocity.y, movementVector.z * m_SlowMultiplier);
        }
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
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, -10f + movementVector.z * 9.5f + m_Rigidbody.velocity.y, m_Rigidbody.velocity.z);
        }

        ToggleGravity(gravityBool);
    }

    void JumpUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (RaycastDir(Vector3.down) || m_MoveState.Equals(MovementState.Wallrunning)))
        {
            if (m_MoveState.Equals(MovementState.Wallrunning)){
                m_WallrunInterrupted = true;
            }

            m_Rigidbody.AddForce(m_JumpForce * Vector3.up, ForceMode.Impulse);
        }

        if (m_MoveState.Equals(MovementState.Falling) && !m_isMovingFromInput)
        {
            m_hMovement = Vector3.zero;
        }
    }
    
    bool RaycastDir(Vector3 direction)
    {
        Vector3 v = new Vector3(m_Collider.bounds.center.x, m_Collider.bounds.min.y, m_Collider.bounds.center.z);

        Ray ray = new Ray(v, direction);

        if (Physics.Raycast(ray, 1f, m_JumpMask))
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
        m_BPlayerVel = m_Rigidbody.velocity;

        m_Rigidbody.AddForce(new Vector3(0.0f, 10.0f, 0.0f), ForceMode.Impulse);

        if (!m_IsBlinking)
        {
            //m_Rigidbody.AddForce(m_ForwardDir * m_BlinkVelocity * 10, ForceMode.Impulse);

            ToggleGravity(false);
            m_IsBlinking = true;
        }
    }

    void Blink()
    {
        //Distance ray
        m_Ray = new Ray(transform.position, m_ForwardDir);

        //Add velocity
        if (m_BlinkTimer <= m_BlinkTime && (!Physics.Raycast(m_Ray, out m_Hit, 20.0f, m_BlinkMask) || m_Hit.distance > 2.0f))
        {
            m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            m_Rigidbody.velocity = m_ForwardDir * m_BlinkVelocity;
        }
        else
        {
            m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;

            m_Rigidbody.velocity = m_ForwardDir * m_BPlayerVel.magnitude;
            ToggleGravity(true);
            m_IsBlinking = false;
        }
    }

    void BlinkUpdate()
    {
        if (!m_IsBlinkCD && Input.GetMouseButtonDown(0))
        {
            m_IsBlinkCD = true;
            ToggleBlink();
        }

        //Blinking
        if (m_IsBlinking)
        {
            m_BlinkTimer += Time.deltaTime;
            Blink();

            /*m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            m_Ray = new Ray(transform.position, m_ForwardDir);
            if (!Physics.Raycast(m_Ray, out m_Hit, 20.0f, m_LayerMask) || m_Hit.distance > 1.5f)
            {
                m_BlinkTimer = m_BlinkTime + 1;
            }*/
        }
        else
        {
            //m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;

            m_BlinkTimer = 0.0f;
        }

        /*if (m_BlinkTimer > m_BlinkTime)
        {
            m_Rigidbody.velocity = m_ForwardDir * m_PlayerVel.magnitude;
            ToggleGravity(true);
            m_IsBlinking = false;
        }*/

        //Blink cooldown
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

    void BlinkReset()
    {
        m_IsBlinkCD = false;
        m_CurBlinkCD = m_BlinkCD;
    }

    bool IsMovingForward()
    {
        //Checks if player is moving foward
        //Translate world space velocity to localspace velocity to be able to read if negative or not
        Vector3 vel = transform.InverseTransformDirection(m_Rigidbody.velocity);

        if (vel.z > 0)
        {
            return true;
        }
        else
        {
            return false;
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

        m_Rigidbody.AddForce(Vector3.up * m_JumpForce * 0.6f, ForceMode.Impulse);
        //Only get forward in X and Z
        Vector3 temp = new Vector3(transform.forward.x, 0.0f, transform.forward.z);
        m_Rigidbody.AddForce(temp * m_MovementSpeed * 4, ForceMode.Impulse);

        Debug.Log("Fastclimb");
    }

    void FeetClimb()
    {
        //Safety check
        if (IsMovingForward() && m_hMovement.magnitude > 0.4f && !m_IsClimbing && m_Rigidbody.velocity.y > 1.0f && !m_MoveState.Equals(MovementState.Blinking))
        {
            m_IsClimbing = true;
            m_IsColliderActive = false;

            m_Rigidbody.AddForce(Vector3.up * m_JumpForce * 0.2f, ForceMode.Impulse);
            Vector3 temp = new Vector3(transform.forward.x, 0.0f, transform.forward.z);
            m_Rigidbody.AddForce(temp * m_MovementSpeed, ForceMode.Impulse);

            Debug.Log("Feetclimb");
        }
    }

    void ClimbUpdate()
    {
        if (!m_MoveState.Equals(MovementState.Jumping )) { 

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

        //Set mesh collider to trigger when climbing
        if (!m_IsColliderActive)
        {
            m_ColliderTimer += Time.deltaTime;
            m_MeshCol.isTrigger = true;
        }
        else
        {
            m_MeshCol.isTrigger = false;
            m_ColliderTimer = 0.0f;
        }

        if (m_ColliderTimer > 0.2f)
        {
            m_IsColliderActive = true;
        }
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

    void DampeningUpdate()
    {
        if (m_Dampening && m_DampeningTimer < m_DampeningTime)
        {
            m_DampeningTimer += Time.deltaTime;
        }
        else
        {
            m_DampeningTimer = 0.0f;
            m_Dampening = Input.GetKeyDown("left ctrl");
        }
    }

    void FallUpdate()
    {
        if (m_MoveState == MovementState.Falling)
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
                m_Slowed = true;
            }
            else if (m_RequireDampening && m_Dampened)
            {
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

        if (m_SlowedTimer > m_SlowTime)
        {
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
        
        if (!m_MySides.m_CanWallrun || m_WallrunInterrupted)
        {
            Camera.main.GetComponent<SimpleSmoothMouseLook>().clampInDegrees = new Vector2(360, 180);
            m_IsWallrunning = false;
            m_WallrunDirSet = false;
        }

        if (m_IsWallrunning) {


            if (Input.GetKeyDown(KeyCode.Space) || !m_isMovingFromInput)
            {
                m_WallrunInterrupted = true;
            }
        }

        if (m_MySides.WallrunObjectChanged || RaycastDir(Vector3.down))
        {
            m_WallrunInterrupted = false;
        }
    }

    void StartWallrun()
    {
        if (m_IsWallrunning && !m_WallrunDirSet)
        {
            m_WallrunRight = m_MySides.m_WallrunSideRight;
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
            Debug.Log(transform.localRotation.eulerAngles);

            //transform.localRotation.SetFromToRotation(transform.forward, m_WallrunDir);



            //Camera.main.GetComponent<SimpleSmoothMouseLook>().targetDirection = new Vector2(m_WallrunDir.x, m_WallrunDir.z);
           // Camera.main.GetComponent<SimpleSmoothMouseLook>().clampInDegrees = new Vector2(180, 180);
        }
    }

    void CheckNotMovingFromInput()
    {
        Vector3 movementDelta = transform.position - m_lastPosition;

        movementDelta.y = 0;

        m_lastPosition = transform.position;

        if (m_MoveState.Equals(MovementState.Climbing) || m_MoveState.Equals(MovementState.Blinking))
        {
            m_isMovingFromInput = true;
        }

        if (movementDelta.magnitude > 0.2f)
        {
            m_isMovingFromInput = true;
            m_NotMovingCount = 0;
        }
        else
        {
            m_NotMovingCount++;            
        }

        Mathf.Clamp(m_NotMovingCount, 0, 3);

        if (m_NotMovingCount == 3)
        {
            m_isMovingFromInput = false;
        }
    }
}