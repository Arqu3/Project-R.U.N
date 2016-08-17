using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public enum MovementState
{
    Idle, Moving, Jumping, Falling, Wallrunning, Grabbing, Blinking, Climbing, VerticalClimbing
}

public struct Keybinding
{
    string m_Bind1;
    string m_Bind2;
    string m_Axis;
    int m_ID;
    KeyCode m_Keycode;
    KeyCode m_Keycode2;

    public Keybinding(string keycode, string keycode2, string axis, int id)
    {
        this.m_Bind1 = keycode;
        this.m_Bind2 = keycode2;
        this.m_Axis = axis;
        this.m_ID = id;
        m_Keycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keycode);
        m_Keycode2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keycode2);
    }

    public void SetID(int id)
    {
        m_ID = id;
    }
    public void SetKeyCode(string keycode)
    {
        if (keycode != "Right Trigger" && keycode != "Left Trigger" && keycode != "DPadX" && keycode != "DPadY")
        {
            m_Bind1 = keycode;
            m_Keycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keycode);
        }
    }

    public void SetKeyCode2(string keycode)
    {
        if (keycode != "Right Trigger" && keycode != "Left Trigger" && keycode != "DPadX" && keycode != "DPadY")
        {
            m_Axis = "";
            m_Bind2 = keycode;
            m_Keycode2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keycode);
        }
        else
        {
            m_Bind2 = "";
            m_Axis = keycode;
        }
    }

    public bool IsButtonDown()
    {
        if (Input.GetKeyDown(m_Keycode))
            return true;
        if (m_Bind2 != "")
        {
            if (Input.GetKeyDown(m_Keycode2))
                return true;
        }
        return false;
    }
    public bool IsButton()
    {
        if (Input.GetKey(m_Keycode))
            return true;
        if (m_Bind2 != "")
        {
            if (Input.GetKey(m_Keycode2))
                return true;
        }
        return false;
    }
    public bool IsAxis()
    {
        if (m_Axis != "")
        {
            if (Input.GetAxis(m_Axis) != 0)
                return true;
        }
        return false;
    }
    public bool IsAxisRaw()
    {
        if (m_Axis != "")
        {
            if (Input.GetAxisRaw(m_Axis) != 0)
                return true;
        }
        return false;
    }

    public void PrintValues()
    {
        Debug.Log(m_ID + " : " + m_Bind1 + " : " + m_Bind2 + " : " + m_Axis);
    }
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
    public float m_AccelMultiplier = 1.0f;
    public float m_JumpForce;
    public float m_FallThreshold = 1.0f;
    public float m_SlopeValue = 0.7f;
    public LayerMask m_BlinkMask;
    public LayerMask m_JumpMask;
    public bool m_IsAirControl = false;
    public float m_SlowMultiplier = 0.5f;
    public float m_SlowTime = 0.5f;
    public float m_DampeningTime = 0.4f;
    public float m_BoostTime = 2.0f;
    public float m_BoostAmount = 10.0f;
    public float m_BoostDecayAmount = 4.0f;
    public float m_VerticalClimbTimer = 1.5f;

    //Input vars
    string[] m_Keybinds;
    bool m_HasKeybinds;
    Keybinding[] m_Keys = new Keybinding[8];

    //Basic movement vars
    Vector3 m_ForwardDir;
    Vector3 m_PlayerVel;
    Vector3 m_hMovement;
    float m_AccelPercent = 0;
    bool m_OnGround;
    bool m_ControlsActive = true;
    ParticleSystem m_SanicParticles;
    float m_XDir = 0.0f;
    float m_ZDir = 0.0f;

    //Component vars
    Rigidbody m_Rigidbody;
    Collider m_Collider;
    Collider m_MeshCol;
    Hands m_PlayerHands;
    Sides m_MySides;
    SoundEmitter m_FootStepEmitter;
    SoundEmitter m_BlinkSoundEmitter;
    SoundEmitter m_BlinkChargeSoundEmitter;
    AnimationHandler m_AnimHandler;
    PlayerCheckpoint m_PCheckpoint;

    //Blink vars
    bool m_IsBlinking = false;
    bool m_IsBlinkCD = false;
    float m_BlinkTimer = 0.0f;
    float m_CurBlinkCD;
    float m_DistanceTravelled = 0.0f;
    RaycastHit m_Hit;
    Ray m_Ray;
    Vector3 m_BPlayerVel;
    bool m_CanBlinkCD = true;
    float m_FOVTimer;
    bool m_IsFOVChange = false;
    ParticleSystem m_BlinkParticles;
    ParticleSystem m_ConstantParticles;

    //Ledgegrab vars
    Vector3 m_GrabbedDir;
    bool m_IsColliderActive = true;
    float m_DisableTime = 0.3f;
    bool m_IsGrabbed = false;
    bool m_IsClimbing = false;
    float m_ClimbTimer = 0.0f;
    float m_ColliderTimer = 0.0f;

    //Fall and dampen vars
    bool m_Dampening = false;
    bool m_Dampening1 = false;
    bool m_RequireDampening = false;
    bool m_Dampened = false;
    bool m_Slowed = false;
    float m_SlowedTimer = 0.0f;
    float m_FallTimer = 0.0f;
    float m_DampeningTimer = 0.0f;
    float m_CurrentBoostAmount = 0.0f;
    float m_BoostTimer;
    bool m_IsBoosted = false;
    bool m_FallDampenInUse = false;

    //Wallrun vars
    bool m_IsWallrunning;
    bool m_CanWallrun;
    bool m_WallrunDirSet;
    bool m_WallrunInterrupted;
    bool m_WallrunRight;
    Vector3 m_WallrunDir;
    const float m_WallrunAngle = 45;
    public float m_WallrunGraceTime;
    float m_WallrunGraceTimer = 0f;

    //Vertical climb vars
    bool m_IsVerticalClimb = false;
    float m_VClimbTimer;

    //MovingFromInput vars
    bool m_isMovingFromInput;
    Vector3 m_lastPosition;
    int m_NotMovingCount = 0;

    //AudioVars
    float m_StepTime = 0.4f;
    float m_CurrentStepTime;
    MovementState m_lastState;

    //Slope vars
    bool m_CanMoveForward = true;
    RaycastHit m_SlopeHit;
    RaycastHit m_DownHit;
    Vector3 m_SlopeDirection;

    void Start()
    {
        m_PlayerHands = GetComponentInChildren<Hands>();
        m_MySides = GetComponentInChildren<Sides>();
        m_Collider = GetComponent<Collider>();
        m_Rigidbody = GetComponentInParent<Rigidbody>();
        m_FootStepEmitter = transform.FindChild("AudioEmitter").GetComponent<SoundEmitter>();
        m_AnimHandler = GetComponentInParent<AnimationHandler>();
        m_PCheckpoint = GetComponentInParent<PlayerCheckpoint>();

        m_BlinkParticles = Camera.main.transform.FindChild("BlinkParticles").GetComponent<ParticleSystem>();
        m_ConstantParticles = m_BlinkParticles.transform.FindChild("ConstantParticles").GetComponent<ParticleSystem>();
        m_SanicParticles = Camera.main.transform.FindChild("SanicParticles").GetComponent<ParticleSystem>();

        m_BlinkSoundEmitter = m_BlinkParticles.GetComponent<SoundEmitter>();
        m_BlinkChargeSoundEmitter = Camera.main.transform.FindChild("BlinkChargeEmitter").GetComponent<SoundEmitter>();

        m_MeshCol = GameObject.Find("Collider").GetComponent<CapsuleCollider>();

        m_CurBlinkCD = m_BlinkCD;
        m_FOVTimer = m_BlinkCD;
        m_VClimbTimer = m_VerticalClimbTimer;
        m_BoostTimer = m_BoostTime;
        m_HasKeybinds = false;
    }

    void Update()
    {
        for (int i = 0; i < m_Keys.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.H))
                m_Keys[i].PrintValues();
        }

        if (m_ControlsActive)
        {
            ResetUpdate();

            CheckSlope();

            DampeningUpdate();

            CheckNotMovingFromInput();

            CheckState();

            BlinkUpdate();

            ClimbUpdate();

            FallUpdate();

            CheckWallrun();

            VerticalClimbUpdate();

            CheckSound();

            BoostUpdate();

            CheckCanBlinkCD();

            UpdateParticles();

            if (!m_MoveState.Equals(MovementState.Blinking) && !m_MoveState.Equals(MovementState.Grabbing) && !m_MoveState.Equals(MovementState.Climbing) && !m_MoveState.Equals(MovementState.VerticalClimbing))
            {
                if (!m_IsAirControl)
                {
                    JumpUpdate();
                    CheckState();
                    CalculateFriction(m_hMovement);

                    //Disable horizontal movement while in air
                    if (!m_MoveState.Equals(MovementState.Jumping) && !m_MoveState.Equals(MovementState.Falling))
                    {
                        if (m_CanMoveForward)
                        {
                            m_hMovement = new Vector3(Mathf.Clamp(InputX() + Input.GetAxis("Horizontal"), -1.0f, 1.0f) * 0.6f, 0, Mathf.Clamp(InputZ() + Input.GetAxis("Vertical"), -1.0f, 1.0f));
                            HorizontalMovement(m_hMovement);
                        }
                    }
                    else
                    {
                        //m_Rigidbody.AddForce(transform.forward * Input.GetAxis("Vertical") * 0.5f + transform.right * Input.GetAxis("Horizontal") * 1, ForceMode.Impulse);
                    }

                    if (m_WallrunInterrupted)
                    {
                        m_WallrunInterrupted = false;
                    }
                }
                else
                {
                    //Horizontal movement is enabled while in air

                    JumpUpdate();
                    CalculateFriction(m_hMovement);

                    m_hMovement = new Vector3(Mathf.Clamp(InputX() + Input.GetAxis("Horizontal"), -1.0f, 1.0f) * 0.6f, 0, Mathf.Clamp(InputZ() + Input.GetAxis("Vertical"), -1.0f, 1.0f) * 0.1f);
                    HorizontalMovement(m_hMovement);


                    if (m_WallrunInterrupted)
                    {
                        m_WallrunInterrupted = false;
                    }

                }
            }
        }
    }

    public MovementState GetState()
    {
        return m_MoveState;
    }

    void UpdateParticles()
    {
        m_SanicParticles.GetComponent<ParticleSystemRenderer>().enabled = true;
        m_SanicParticles.startColor = new Color(1, 1, 1, Mathf.Lerp(0, 1, m_AccelPercent * 0.01f));
    }

    void CheckState()
    {
        //AccelPercent is incremented/decremented here

        //Blink is a special state and is checked first before all others
        if (!m_IsBlinking)
        {
            //Checks if player is in air
            if (!RaycastDir(Vector3.down))
            {
                m_OnGround = false;

                if (m_Rigidbody.velocity.y > 0f)
                {
                    m_AccelPercent = m_AccelPercent + Time.deltaTime * 20 * m_AccelMultiplier;
                    m_MoveState = MovementState.Jumping;
                }
                else
                {
                    m_AccelPercent = m_AccelPercent + Time.deltaTime * 10 * m_AccelMultiplier;
                    m_MoveState = MovementState.Falling;
                }

                if (m_IsWallrunning)
                {
                    m_AccelPercent = m_AccelPercent + Time.deltaTime * 20 * m_AccelMultiplier;
                    m_MoveState = MovementState.Wallrunning;
                }
            }
            //If player is not in air then only following states are possible
            else
            {
                m_OnGround = true;
                if (m_Rigidbody.velocity.magnitude > 1f || m_hMovement.magnitude > 0.4f)
                {
                    m_AccelPercent = m_AccelPercent + Time.deltaTime * 20 * m_AccelMultiplier;
                    m_MoveState = MovementState.Moving;
                }
                else
                {
                    m_AccelPercent = m_AccelPercent - Time.deltaTime * 150 * m_AccelMultiplier;
                    m_MoveState = MovementState.Idle;
                }

                //Drag player down if trying to run up a steep slope
                if (CheckSlope())
                {
                    if (m_SlopeHit.collider.gameObject.GetComponent<Terrain>())
                    {
                        if (Vector3.Dot(Vector3.up, m_SlopeHit.normal) < m_SlopeValue)
                        {
                            m_CanMoveForward = false;
                            m_Rigidbody.velocity = new Vector3(0.0f, -50.0f, 0.0f);
                            Debug.Log("Slope too steep yo");
                        }
                        else
                        {
                            m_CanMoveForward = true;
                            Debug.Log("We cool");
                        }
                    }
                }
                else
                    m_CanMoveForward = true;
            }
        }
        else
        {
            m_OnGround = false;
            m_MoveState = MovementState.Blinking;
        }


        if (m_IsGrabbed)
        {
            m_AccelPercent = m_AccelPercent - Time.deltaTime * 20 * m_AccelMultiplier;
            m_MoveState = MovementState.Grabbing;
        }
        else if (m_IsClimbing)
        {
            m_OnGround = false;
            m_MoveState = MovementState.Climbing;
        }
        else if (m_IsVerticalClimb && !m_IsWallrunning)
        {
            m_MoveState = MovementState.VerticalClimbing;
        }

        m_AccelPercent = Mathf.Clamp(m_AccelPercent, 0, 100);
    }

    void HorizontalMovement(Vector3 movementVector)
    {
        float currentMoveSpeed = Mathf.Lerp(m_MovementSpeed, m_MaxSpeed + m_CurrentBoostAmount, m_AccelPercent * 0.01f);

        if (m_MoveState.Equals(MovementState.Wallrunning) && !m_WallrunInterrupted)
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

    void CheckSound()
    {
        if (m_MoveState.Equals(MovementState.Moving) || m_MoveState.Equals(MovementState.Wallrunning) || m_MoveState.Equals(MovementState.VerticalClimbing))
        {
            float stepTime = m_StepTime;

            if (m_MoveState.Equals(MovementState.Wallrunning))
            {
                stepTime = stepTime * 0.75f;
            }
            if (m_MoveState.Equals(MovementState.VerticalClimbing))
            {
                stepTime = stepTime * 0.60f;
            }


            if (m_lastState.Equals(MovementState.Falling))
            {
                m_FootStepEmitter.PlayClip(Mathf.RoundToInt(Random.Range(0, 2)));
                m_CurrentStepTime = 0;
            }

            else if (m_CurrentStepTime > stepTime)
            {
                m_FootStepEmitter.PlayRandomClip(2);
                m_CurrentStepTime -= stepTime;
            }

            else
            {
                m_CurrentStepTime += Time.deltaTime;
            }
        }



        if (m_lastState.Equals(MovementState.Falling) && m_MoveState.Equals(MovementState.Idle))
        {
            m_FootStepEmitter.PlayClip(Mathf.RoundToInt(Random.Range(0, 1)));
            m_CurrentStepTime = 0;
        }

        m_lastState = m_MoveState;
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

        if (m_MoveState.Equals(MovementState.Falling) || m_MoveState.Equals(MovementState.Jumping))
        {
            m_Rigidbody.AddForce(Vector3.down * (2.5f - Mathf.Clamp01(m_Rigidbody.velocity.y) * 1.5f), ForceMode.Impulse);
            //Debug.Log(Mathf.Clamp01(m_Rigidbody.velocity.y));
        }

        if (m_MoveState.Equals(MovementState.Wallrunning))
        {
            gravityBool = false;
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, -1 + movementVector.z * 0.8f, m_Rigidbody.velocity.z);
        }
        if (m_MoveState.Equals(MovementState.Climbing))
        {
            gravityBool = false;
        }

        ToggleGravity(gravityBool);
    }

    void JumpUpdate()
    {
        if ((m_Keys[5].IsButtonDown() || m_Keys[5].IsAxisRaw()) && (m_OnGround || m_MoveState.Equals(MovementState.Wallrunning)))
        {
            if (m_MoveState.Equals(MovementState.Wallrunning))
            {
                m_WallrunInterrupted = true;
                m_CanWallrun = false;
            }

            m_WallrunGraceTimer = 0f;
            m_Rigidbody.AddForce(m_JumpForce * Vector3.up, ForceMode.Impulse);
            m_FootStepEmitter.PlayRandomClip(2, 4);
        }

        if (m_MoveState.Equals(MovementState.Falling))
        {
            m_WallrunGraceTimer += Time.deltaTime;

            if (!m_isMovingFromInput)
            {
                m_hMovement = Vector3.zero;
            }
        }
    }

    bool RaycastDir(Vector3 direction)
    {
        float offsetCenter = 0.3f;
        float offsetHeight = 0.6f;
        float distance = 0.6f;

        Vector3 v = new Vector3(transform.position.x, m_Collider.bounds.min.y - offsetHeight, transform.position.z);
        Vector3 tempV = Vector3.zero;

        Ray ray = new Ray();

        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case (0):
                    tempV = new Vector3(transform.forward.x * offsetCenter, 0, transform.forward.z * offsetCenter);
                    break;
                case (1):
                    tempV = -tempV;
                    break;
                case (2):
                    tempV = new Vector3(transform.right.x * offsetCenter, 0, transform.right.z * offsetCenter);
                    break;
                case (3):
                    tempV = -tempV;
                    break;
                default:
                    break;
            }

            ray = new Ray(tempV + v, direction);
            Debug.DrawRay(tempV + v, direction * distance, Color.red);

            if (Physics.Raycast(ray, distance, m_JumpMask))
            {
                return true;
            }
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
        if (!m_IsBlinkCD && (m_Keys[4].IsButton() || m_Keys[4].IsAxisRaw()))
        {
            m_IsFOVChange = true;
            m_IsBlinkCD = true;
            ToggleBlink();
            m_FOVTimer = 0;

            //Audio
            m_BlinkSoundEmitter.PlayRandomClip(2);
            m_BlinkParticles.Play();
            m_BlinkChargeSoundEmitter.ToggleLoop(true);
            //m_BlinkChargeSoundEmitter.PlayClip(0);

            if (RaycastDir(Vector3.down))
            {
                m_CanBlinkCD = true;
            }
            else
            {
                m_CanBlinkCD = false;
            }
        }

        //Blinking
        if (m_IsBlinking)
        {
            m_BlinkParticles.startColor = new Color(1, 1, 1, Mathf.Lerp(0, 1, m_BlinkTimer / m_BlinkTime));
            m_ConstantParticles.startColor = m_BlinkParticles.startColor;
            m_BlinkTimer += Time.deltaTime;
            Blink();
            Camera.main.fieldOfView = Mathf.Lerp(70, 90, m_BlinkTimer / m_BlinkTime);
        }
        else
        {
            //m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            if (m_FOVTimer < 1)
            {
                m_FOVTimer += Time.deltaTime;
                Camera.main.fieldOfView = Mathf.Lerp(90, 70, m_FOVTimer / 1);
                m_BlinkParticles.startColor = new Color(1, 1, 1, Mathf.Lerp(1, 0, m_FOVTimer / 0.5f));
                m_ConstantParticles.startColor = m_BlinkParticles.startColor;
            }

            else
                Camera.main.fieldOfView = 70;

            m_BlinkTimer = 0.0f;
        }



        //Blink cooldown
        if (m_IsBlinkCD && m_CanBlinkCD)
        {
            if (m_CurBlinkCD == m_BlinkCD)
            {
                m_BlinkChargeSoundEmitter.CrossfadeToClip(1 + Mathf.RoundToInt(Random.value), 0.3f);
                m_BlinkChargeSoundEmitter.ToggleLoop(false);
            }

            m_CurBlinkCD -= Time.deltaTime;
            if (m_CurBlinkCD <= 0.0f)
            {
                m_CurBlinkCD = m_BlinkCD;
                m_IsBlinkCD = false;
            }
        }

        //FOV timer
        if (m_IsBlinkCD && m_IsFOVChange)
        {
            //m_FOVTimer += Time.deltaTime;
            if (m_FOVTimer <= 0.0f)
            {
                // m_FOVTimer = m_BlinkCD;
                m_IsFOVChange = false;
            }
        }
    }

    void CheckCanBlinkCD()
    {
        if (RaycastDir(Vector3.down) || m_IsWallrunning || m_IsVerticalClimb || m_IsGrabbed || m_IsClimbing)
        {
            m_CanBlinkCD = true;
        }
    }

    public bool GetCanBlinkCD()
    {
        return m_CanBlinkCD;
    }

    bool IsMovingForward()
    {
        //Checks if player is moving foward
        //Translate world space velocity to localspace velocity to be able to read if negative or not
        Vector3 vel = transform.InverseTransformDirection(m_Rigidbody.velocity);

        if (vel.z > 0)
        {
            Debug.Log("Forward");
            return true;
        }
        else
        {
            Debug.Log("Backward");
            return false;
        }
    }

    public void IsGrabbed(bool state)
    {
        m_IsGrabbed = state;
        if (m_IsGrabbed)
            m_GrabbedDir = m_PlayerHands.GetLedgeForward();
    }

    void FeetClimb()
    {
        //Safety check
        if (IsMovingForward() && !m_IsClimbing && !m_OnGround && !m_MoveState.Equals(MovementState.Blinking))
        {
            m_IsClimbing = true;
            m_IsColliderActive = false;

            Vector3 temp = new Vector3(transform.forward.x, 1 * 0.4f, transform.forward.z);
            m_Rigidbody.AddForce(temp, ForceMode.Impulse);

            Debug.Log("Feetclimb");
        }
    }

    void ClimbUpdate()
    {
        if (m_PlayerHands.m_CanClimb && (m_Keys[5].IsButton() || m_Keys[5].IsAxisRaw()))
        {
            Climb();
            m_AnimHandler.PlayAnimation("Climb");
        }

        if (!m_MoveState.Equals(MovementState.Jumping))
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

            if (m_ColliderTimer > m_DisableTime)
            {
                m_IsColliderActive = true;
                m_DisableTime = 0.3f;
            }
        }
    }

    public void DisableCollider(float time)
    {
        m_IsColliderActive = false;
        m_DisableTime = time;
    }

    public void Climb()
    {
        m_IsClimbing = true;
    }

    void DampeningUpdate()
    {
        if ((m_Dampening || m_Dampening1) && m_DampeningTimer < m_DampeningTime)
        {
            m_DampeningTimer += Time.deltaTime;
        }
        else
        {
            m_DampeningTimer = 0.0f;
            m_Dampening = m_Keys[6].IsButton();
            m_Dampening1 = m_Keys[6].IsAxisRaw();
            if (m_Keys[6].IsButton() || m_Keys[6].IsAxisRaw())
            {
                if (!m_FallDampenInUse)
                {
                    m_FallDampenInUse = true;
                    m_Dampening = true;
                    m_Dampening1 = true;
                }
            }
            if (!m_Keys[6].IsButton() && !m_Keys[6].IsAxisRaw())
            {
                m_FallDampenInUse = false;
            }
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

        if (m_OnGround)
        {
            if (m_RequireDampening && !m_Dampened)
            {
                m_Slowed = true;
                Debug.Log("Dampening failed");
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

        if (m_RequireDampening && (m_Dampening || m_Dampening1))
        {
            m_Dampened = true;
            m_IsBoosted = true;
            Debug.Log("Dampening successful");
        }
        else
        {
            m_Dampened = false;
        }
    }

    void BoostUpdate()
    {
        if (m_IsBoosted)
        {
            m_BoostTimer -= Time.deltaTime;
            m_CurrentBoostAmount = m_BoostAmount;

            if (m_BoostTimer <= 0.0f)
            {
                m_BoostTimer = m_BoostTime;
                m_IsBoosted = false;
            }
        }
        else
        {
            if (m_CurrentBoostAmount > 0.0f)
            {
                m_CurrentBoostAmount -= m_BoostDecayAmount * Time.deltaTime;
            }
            else
            {
                m_CurrentBoostAmount = 0.0f;
            }
        }
    }

    void CheckWallrun()
    {
        if (m_MySides.m_CanWallrun && (m_MoveState.Equals(MovementState.Jumping) || (m_WallrunGraceTimer < 0.7f && m_MoveState.Equals(MovementState.Falling))) && m_CanWallrun)
        {
            m_IsWallrunning = true;

            StartWallrun();
        }

        if (!m_MySides.m_CanWallrun || m_WallrunInterrupted || !m_CanWallrun)
        {
            m_IsWallrunning = false;
            m_WallrunDirSet = false;
        }

        if (m_IsWallrunning)
        {

            if ((m_Keys[5].IsButtonDown() || m_Keys[5].IsAxisRaw()) || !m_isMovingFromInput)
            {
                m_WallrunInterrupted = true;
                m_CanWallrun = false;
            }
        }

        if (m_MySides.WallrunObjectChanged || m_OnGround)
        {
            m_CanWallrun = true;
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

            for (int t = 0; t < 2; t++)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Vector3.Angle(t * 2 * -tempV[i] + tempV[i], forward) < m_WallrunAngle)
                    {

                        m_WallrunDir = (t * 2 * -tempV[i] + tempV[i]).normalized;
                        t = 2;
                        i = 4;
                    }
                }
            }
            m_WallrunDir = Vector3.ClampMagnitude(m_WallrunDir, 1f);

            Camera.main.GetComponent<SimpleSmoothMouseLook>().ClampMouseX(m_WallrunDir, 200);
        }
    }

    public void SetVerticalClimb(bool state)
    {
        m_IsVerticalClimb = state;
    }

    void VerticalClimbUpdate()
    {
        if (m_MoveState.Equals(MovementState.VerticalClimbing))
        {
            ToggleGravity(false);
            bool controllerVertical = false;
            if (Input.GetAxis("Vertical") > 0)
            {
                controllerVertical = true;
            }
            if ((m_VClimbTimer > 0 && m_Keys[0].IsButton() || controllerVertical) && !m_IsGrabbed)
            {
                m_VClimbTimer -= Time.deltaTime;
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_VClimbTimer * 10, m_Rigidbody.velocity.z);
            }
            else
            {
                SetVerticalClimb(false);
            }
        }
        else
        {
            m_VClimbTimer = m_VerticalClimbTimer;
        }
    }

    bool CheckSlope()
    {
        float offset = 0.6f;
        float distance = 1.0f;
        Vector3 tempV = Vector3.zero;
        Vector3 v = new Vector3(transform.position.x, transform.position.y - offset, transform.position.z);
        Ray ray = new Ray();
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case (0):
                    tempV = transform.forward;
                    break;

                case (1):
                    tempV = -tempV;
                    break;

                case (2):
                    tempV = transform.right;
                    break;

                case (3):
                    tempV = -tempV;
                    break;
            }

            tempV.y = 0.0f;
            Debug.DrawRay(v, tempV * distance, Color.red);
            ray = new Ray(v, tempV);
            if (Physics.Raycast(ray, out m_SlopeHit, distance, m_JumpMask) && IsStandingOnSlope())
            {
                return true;
            }
        }
        return false;
    }

    bool IsStandingOnSlope()
    {
        Vector3 v = new Vector3(transform.position.x, transform.position.y - m_Collider.bounds.size.y, transform.position.z);
        Ray ray = new Ray(v, Vector3.down);
        Debug.DrawRay(v, Vector3.down * 3.0f, Color.red);
        if (Physics.Raycast(ray, out m_DownHit, 3.0f, m_JumpMask))
        {
            if (m_DownHit.collider.gameObject.GetComponent<Terrain>())
                return true;
        }

        return false;
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

        if (movementDelta.magnitude > 0.075f)
        {
            m_isMovingFromInput = true;
            m_NotMovingCount = 0;
        }

        else
        {
            m_NotMovingCount++;
            m_NotMovingCount = Mathf.Clamp(m_NotMovingCount, 0, 3);
        }

        if (m_NotMovingCount == 3)
        {
            m_isMovingFromInput = false;
        }
    }

    /// <summary>
    /// Returns UI values from the player in an array in order: (Current Blink CD, Blink CD)
    /// </summary>
    /// <returns></returns>
    public float[] GetUIValues()
    {
        return new float[2] { m_CurBlinkCD, m_BlinkCD };
    }

    /// <summary>
    /// Toggles all player movement controls.
    /// </summary>
    /// <param name="active">If true then input is received.</param>
    public void ToggleControls(bool active)
    {
        m_ControlsActive = active;
    }

    public bool GetIsControls()
    {
        return m_ControlsActive;
    }

    public Vector3 GetWallrunDir()
    {
        return m_WallrunDir;
    }

    public Vector3 GetGrabbedDir()
    {
        return m_GrabbedDir;
    }

    public void ResetValues()
    {
        //Blink
        m_IsBlinkCD = false;
        m_CurBlinkCD = m_BlinkCD;
        m_BlinkChargeSoundEmitter.Pause(true);
        m_BlinkChargeSoundEmitter.ToggleLoop(false);

        //Fall timer
        m_FallTimer = 0.0f;

        //Boost
        m_IsBoosted = false;
        m_CurrentBoostAmount = 0.0f;
        m_BoostTimer = m_BoostTime;

        //Wallrunning
        m_IsWallrunning = false;

        //Vertical climbing
        SetVerticalClimb(false);

        //Grabbing/climbing
        m_IsGrabbed = false;
        m_IsClimbing = false;

        GameObject.Find("Player").transform.parent = null;
    }

    void ResetUpdate()
    {
        if (m_Keys[7].IsButtonDown() || m_Keys[7].IsAxisRaw())
        {
            m_PCheckpoint.SetToCheckpoint(m_PCheckpoint.GetLastPassed());
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            m_PCheckpoint.ReloadScene();
        }
    }

    public void SetKeybinds(string[] binds)
    {
        m_Keybinds = new string[binds.Length];

        if (m_Keybinds.Length > 0)
        {
            for (int i = 0; i < m_Keybinds.Length; i++)
            {
                m_Keybinds[i] = PlayerPrefs.GetString(binds[i]);
            }

            int inc = 1;
            int laps = 0;
            for (int i = 0; i < m_Keybinds.Length; i+=inc)
            {
                if (i <= 3)
                    m_Keys[laps].SetKeyCode(m_Keybinds[i]);
                else
                {
                    inc = 2;
                    if (i + 1 < m_Keybinds.Length)
                    {
                        m_Keys[laps].SetKeyCode(m_Keybinds[i]);
                        m_Keys[laps].SetKeyCode2(m_Keybinds[i + 1]);
                    }
                }

                if (laps < m_Keys.Length - 1)
                {
                    laps++;
                    m_Keys[laps].SetID(laps);
                }
            }            
            m_HasKeybinds = true;
        }
        else
            Debug.Log("Keybinds array is empty");
    }

    public bool GetHasKeyBinds()
    {
        return m_HasKeybinds;
    }

    float InputZ()
    {
        if (m_Keys[0].IsButton())
        {
            m_ZDir = 1.0f;
        }
        else if (m_Keys[1].IsButton())
        {
            m_ZDir = -1.0f;
        }
        else
            m_ZDir = 0.0f;

        return m_ZDir;
    }

    float InputX()
    {
        if (m_Keys[2].IsButton())
        {
            m_XDir = -1.0f;
        }
        else if (m_Keys[3].IsButton())
        {
            m_XDir = 1.0f;
        }
        else
            m_XDir = 0.0f;

        return m_XDir;
    }
}