using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerCheckpoint : MonoBehaviour {

    //Public vars
    public float ResetDepth = 0.0f;
    public static int m_LastPassed = 0;
    public Transform[] m_CheckPoints;
    public Text m_ElapsedText;

    //Other vars
    Transform m_Temp;
    bool m_IsColliding = false;
    float m_ElapsedTime = 0.0f;
    ControllerPlayer m_CPlayer;
    SimpleSmoothMouseLook m_Camera;
    bool m_HasReachedLast = false;

	void Start ()
    {
        m_CPlayer = GetComponent<ControllerPlayer>();
        m_Camera = GetComponentInChildren<SimpleSmoothMouseLook>();

        //Find checkpoints
        var checkPoints = GameObject.FindGameObjectsWithTag("Checkpoint");

        if (checkPoints.Length > 0)
        {
            m_CheckPoints = new Transform[checkPoints.Length];
            for (int i = 0; i < checkPoints.Length; i++)
            {
                m_CheckPoints[i] = checkPoints[i].transform;
            }

            //Sort checkpoint depending on ID
            for (int write = 0; write < checkPoints.Length; write++)
            {
                for (int sort = 0; sort < checkPoints.Length - 1; sort++)
                {
                    if (m_CheckPoints[sort].GetComponent<Checkpoint>().m_ID > m_CheckPoints[sort + 1].GetComponent<Checkpoint>().m_ID)
                    {
                        m_Temp = m_CheckPoints[sort + 1];
                        m_CheckPoints[sort + 1] = m_CheckPoints[sort];
                        m_CheckPoints[sort] = m_Temp;
                    }
                }
            }
        }
	}
	
	void Update ()
    {
        m_IsColliding = false;

        if (Input.GetKeyDown(KeyCode.R))
        {
            SetToLastCheckpoint();
        }

        if (transform.localPosition.y < ResetDepth)
        {
            Debug.Log("Depth reset");
            SetToLastCheckpoint();
        }

        ElapsedTimeUpdate();
	}

    void SetToLastCheckpoint()
    {
        //Reset player velocity
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        //Set position
        transform.position = m_CheckPoints[m_LastPassed].position;

        //Reset blink CD
        m_CPlayer.SendMessage("BlinkReset");

        //Set rotation to corresponding checkpoint
        m_Camera._mouseAbsolute.x = m_CheckPoints[m_LastPassed].localEulerAngles.y;
        m_Camera._mouseAbsolute.y = m_CheckPoints[m_LastPassed].localEulerAngles.x;
    }

    void OnTriggerEnter(Collider col)
    {
        if (m_IsColliding)
        {
            return;
        }
        m_IsColliding = true;

        //If collision with checkpoint
        if (col.gameObject.tag == "Checkpoint")
        {
            for (int i = 0; i < m_CheckPoints.Length; i++)
            {
                if (col.gameObject == m_CheckPoints[i].gameObject)
                {
                    m_LastPassed = i;
                }
            }
            if (m_LastPassed + 1 == m_CheckPoints.Length)
            {
                m_HasReachedLast = true;
                Debug.Log("Reached last checkpoint");
            }
        }

        //If collision with killbox
        if (col.gameObject.tag == "KillBox")
        {
            Debug.Log("Killbox reset");
            SetToLastCheckpoint();
        }
    }

    void ElapsedTimeUpdate()
    {
        if (!m_HasReachedLast)
        {
            m_ElapsedTime += Time.deltaTime;
        }
        m_ElapsedText.text = "Time: " + m_ElapsedTime.ToString("F1");
    }
}
