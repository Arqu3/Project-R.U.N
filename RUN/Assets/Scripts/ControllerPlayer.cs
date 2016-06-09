using UnityEngine;
using System.Collections;

public class ControllerPlayer : MonoBehaviour
{

    public float blinkTime = 0.5f;
    public float blinkVelocity = 200.0f;

    bool isBlinking = false;
    float timer = 0.0f;

    Vector3 forwardDir;
    Vector3 playerVel;

    public float m_MovementSpeed;
    public float m_JumpForce;

    Rigidbody m_Rigidbody;
    Collider m_Collider;
    Hands m_PlayerHands;

    void Start()
    {
        m_PlayerHands = GetComponentInChildren<Hands>();
        m_Collider = GetComponent<Collider>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Blink timer
        if (isBlinking)
        {
            timer += Time.deltaTime;
            Blink();
        }
        else
        {
            timer = 0.0f;
        }

        if (Input.GetMouseButtonDown(0))
        {
            ToggleBlink();
        }

        Vector3 hMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        CheckClimb(hMovement);
        HorizontalMovement(hMovement);
        Jump();
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
        forwardDir = Camera.main.transform.forward;

        //Store velocity
        playerVel = m_Rigidbody.velocity;

        if (!isBlinking)
        {
            isBlinking = true;
        }
    }

    void Blink()
    {
        //Add velocity
        if (timer < blinkTime)
        {
            m_Rigidbody.AddForce(forwardDir * blinkVelocity);
        }
        else
        {
            isBlinking = false;
            m_Rigidbody.velocity = playerVel;
        }
    }

    void CheckClimb(Vector3 inputVector)
    {
        if (m_PlayerHands.m_CanClimb && inputVector.magnitude > 0.4f)
        {
            m_Rigidbody.AddForce(Vector3.up * 6.5f, ForceMode.Impulse);
        }
    }
}


