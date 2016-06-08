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
    Vector3 m_LastMousePos;

    void Start()
    {
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

        if (Input.GetMouseButton(0))
        {
            ToggleBlink();
        }

        HorizontalMovement();
        Jump();
    }

    void HorizontalMovement()
    {
        Vector3 hMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        m_Rigidbody.AddRelativeForce(hMovement * m_MovementSpeed, ForceMode.Acceleration);
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
        Vector3 v = new Vector3(GetComponent<Collider>().bounds.center.x, GetComponent<Collider>().bounds.min.y, GetComponent<Collider>().bounds.center.z);

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
        forwardDir = transform.forward;
        forwardDir.y = 0;

        //Store velocity
        playerVel = GetComponent<Rigidbody>().velocity;

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
            GetComponent<Rigidbody>().AddForce(forwardDir * blinkVelocity);
        }
        else
        {
            isBlinking = false;
            GetComponent<Rigidbody>().velocity = playerVel;
        }


    }
}


