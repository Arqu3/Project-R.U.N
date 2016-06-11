using UnityEngine;
using System.Collections;

public class ControllerPlayer : MonoBehaviour
{

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
            Vector3 hMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            HorizontalMovement(hMovement);
            Jump();
        }
    }

    void HorizontalMovement(Vector3 inputVector)
    {


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

    void ToggleBlink()
    {
        //Get forward direction
        m_ForwardDir = Camera.main.transform.forward;

        //Store velocity
        m_PlayerVel = m_Rigidbody.velocity;

        if (!m_IsBlinking)
        {
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
        m_Rigidbody.AddForce(Vector3.up * 6.5f, ForceMode.Impulse);
        m_Rigidbody.AddForce(Vector3.forward * 3.0f, ForceMode.Impulse);

        Debug.Log("FastClimb");
    }

    void SlowClimb()
    {
        Debug.Log("SlowClimb");
    }
}


