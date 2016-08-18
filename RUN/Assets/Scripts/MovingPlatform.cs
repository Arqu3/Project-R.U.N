using UnityEngine;
using System.Collections;

public enum Direction
{
    X,
    Y,
    Z
};
public enum Mode
{
    Constant,
    OnPlayerCollision,
    OnPlayerCollisionStay
};

public class MovingPlatform : MonoBehaviour
{
    //Public vars
    public Direction m_Direction = Direction.X;
    public Mode m_Mode = Mode.Constant;
    public bool m_StayOnReachedEnd = false;
    public bool m_ReturnOnExit = false;
    public float m_Speed = 1.0f;
    public float m_Distance = 10.0f;
    public float m_PercentageOffset = 0.0f;

    //Other vars
    bool m_IsMoving;
    bool m_HasReachedEnd;
    bool m_IsAtStart;
    bool m_IsReturning;
    bool m_IsAtEnd;
    int m_CurrentDir;
    Vector3 m_MoveToPos;
    Vector3 m_VDirection;
    Vector3 m_StartPos;

    //Startup vars
    int m_StartDir;
    Vector3 m_StartOffsetPos;
    bool m_StartStayOnEnd;
    bool m_StartHasReachedEnd;
    bool m_StartIsAtStart;
    bool m_StartReturnOnExit;
    bool m_StartIsReturning;
    bool m_StartIsAtEnd;

	void Start ()
    {
        //Set direction and set MoveToPos depending on direction
        if (m_Distance > 0)
        {
            m_CurrentDir = 1;
        }
        else
        {
            m_CurrentDir = -1;
        }
        if (GetDirection().Equals(Direction.X))
        {
            m_MoveToPos = new Vector3(transform.position.x + m_Distance, 0.0f, 0.0f);
            m_VDirection = new Vector3(1.0f, 0.0f, 0.0f);
        }
        else if (GetDirection().Equals(Direction.Y))
        {
            m_MoveToPos = new Vector3(0.0f, transform.position.y + m_Distance, 0.0f);
            m_VDirection = new Vector3(0.0f, 1.0f, 0.0f);
        }
        else if (GetDirection().Equals(Direction.Z))
        {
            m_MoveToPos = new Vector3(0.0f, 0.0f, transform.position.z + m_Distance);
            m_VDirection = new Vector3(0.0f, 0.0f, 1.0f);
        }
        m_StartPos = transform.position;

        //Do not use negative speed pls
        m_Speed = Mathf.Clamp(m_Speed, 0.0f, Mathf.Infinity);
        m_HasReachedEnd = false;
        m_IsAtStart = false;
        m_IsReturning = false;
        m_IsAtEnd = false;

        //Set IsMoving depening on Mode
        if (GetMode().Equals(Mode.Constant))
        {
            m_IsMoving = true;
        }
        else if (GetMode().Equals(Mode.OnPlayerCollision))
        {
            m_IsMoving = false;
        }
        else if (GetMode().Equals(Mode.OnPlayerCollisionStay))
        {
            m_IsMoving = false;
        }

        //Set percentage offset position
        m_PercentageOffset = Mathf.Clamp01(m_PercentageOffset);
        transform.position += m_VDirection * m_Distance * m_PercentageOffset;

        //Set start variables
        m_StartOffsetPos = m_VDirection * m_Distance * m_PercentageOffset;
        m_StartDir = m_CurrentDir;
        m_StartStayOnEnd = m_StayOnReachedEnd;
        m_StartHasReachedEnd = m_HasReachedEnd;
        m_StartIsAtStart = m_IsAtStart;
        m_StartReturnOnExit = m_ReturnOnExit;
        m_StartIsReturning = m_IsReturning;
        m_StartIsAtEnd = m_IsAtEnd;
    }
	
	void Update()
    {
        //Only update if IsMoving
        if (m_IsMoving && m_Distance != 0.0f)
        {
            //Update platform position depending on direction
            //Direction X
            if (GetDirection().Equals(Direction.X))
            {
                if (m_Distance > 0)
                {
                    if (transform.position.x > m_MoveToPos.x)
                    {
                        m_CurrentDir = -1;
                        if (m_StayOnReachedEnd)
                            m_HasReachedEnd = true;
                        m_IsAtEnd = true;
                    }
                    else if (transform.position.x < m_StartPos.x)
                    {
                        m_CurrentDir = 1;
                        if (m_IsReturning)
                        {
                            m_IsReturning = false;
                        }
                        m_IsAtStart = true;
                    }
                    else
                    {
                        m_IsAtEnd = false;
                        m_IsAtStart = false;
                    }
                }
                else if (m_Distance < 0)
                {
                    if (transform.position.x < m_MoveToPos.x)
                    {
                        m_CurrentDir = 1;
                        if (m_StayOnReachedEnd)
                            m_HasReachedEnd = true;
                        m_IsAtEnd = true;
                    }
                    else if (transform.position.x > m_StartPos.x)
                    {
                        m_CurrentDir = -1;
                        if (m_IsReturning)
                        {
                            m_IsReturning = false;
                        }
                        m_IsAtStart = true;
                    }
                    else
                    {
                        m_IsAtEnd = false;
                        m_IsAtStart = false;
                    }
                }
                transform.Translate(m_Speed * m_CurrentDir * Time.deltaTime, 0.0f, 0.0f, Space.World);
            }
            //Direction Y
            else if (GetDirection().Equals(Direction.Y))
            {
                if (m_Distance > 0)
                {
                    if (transform.position.y > m_MoveToPos.y)
                    {
                        m_CurrentDir = -1;
                        if (m_StayOnReachedEnd)
                            m_HasReachedEnd = true;
                        m_IsAtEnd = true;
                    }
                    else if (transform.position.y < m_StartPos.y)
                    {
                        m_CurrentDir = 1;
                        if (m_IsReturning)
                        {
                            m_IsReturning = false;
                        }
                        m_IsAtStart = true;
                    }
                    else
                    {
                        m_IsAtEnd = false;
                        m_IsAtStart = false;
                    }
                }
                else if (m_Distance < 0)
                {
                    if (transform.position.y < m_MoveToPos.y)
                    {
                        m_CurrentDir = 1;
                        if (m_StayOnReachedEnd)
                            m_HasReachedEnd = true;
                        m_IsAtEnd = true;
                    }
                    else if (transform.position.y > m_StartPos.y)
                    {
                        m_CurrentDir = -1;
                        if (m_IsReturning)
                        {
                            m_IsReturning = false;
                        }
                        m_IsAtStart = true;
                    }
                    else
                    {
                        m_IsAtEnd = false;
                        m_IsAtStart = false;
                    }
                }
                transform.Translate(0.0f, m_Speed * m_CurrentDir * Time.deltaTime, 0.0f, Space.World);
            }
            //Direction Z
            if (GetDirection().Equals(Direction.Z))
            {
                if (m_Distance > 0)
                {
                    if (transform.position.z > m_MoveToPos.z)
                    {
                        m_CurrentDir = -1;
                        if (m_StayOnReachedEnd)
                            m_HasReachedEnd = true;
                        m_IsAtEnd = true;
                    }
                    else if (transform.position.z < m_StartPos.z)
                    {
                        m_CurrentDir = 1;
                        if (m_IsReturning)
                        {
                            m_IsReturning = false;
                        }
                        m_IsAtStart = true;
                    }
                    else
                    {
                        m_IsAtEnd = false;
                        m_IsAtStart = false;
                    }
                }
                else if (m_Distance < 0)
                {
                    if (transform.position.z < m_MoveToPos.z)
                    {
                        m_CurrentDir = 1;
                        if (m_StayOnReachedEnd)
                            m_HasReachedEnd = true;
                        m_IsAtEnd = true;
                    }
                    else if (transform.position.z > m_StartPos.z)
                    {
                        m_CurrentDir = -1;
                        if (m_IsReturning)
                        {
                            m_IsReturning = false;
                        }
                        m_IsAtStart = true;
                    }
                    else
                    {
                        m_IsAtEnd = false;
                        m_IsAtStart = false;
                    }
                }
                transform.Translate(0.0f, 0.0f, m_Speed * m_CurrentDir * Time.deltaTime, Space.World);
            }

            //Stop moving if mode is set and reached end
            if (!m_IsReturning)
            {
                if (m_StayOnReachedEnd)
                {
                    if (m_IsAtStart || m_IsAtEnd)
                    m_IsMoving = false;
                }
            }
        }
	}

    void OnCollisionEnter(Collision col)
    {
        //Set IsMoving to true if mode isn't Constant
        //Set player parent to this platform
        if (col.gameObject.tag == "Player")
        {
            if (!m_HasReachedEnd || m_IsAtStart)
            {
                if (!GetMode().Equals(Mode.Constant))
                {
                    m_IsMoving = true;
                    Debug.Log("Entered");
                }
                col.transform.parent = transform;
            }
        }
    }

    void OnCollisionStay(Collision col)
    {
        //Keep IsMoving true if mode is CollisionStay
        if (col.gameObject.tag == "Player")
        {
            if (!m_HasReachedEnd)
            {
                if (GetMode().Equals(Mode.OnPlayerCollisionStay))
                {
                    m_IsMoving = true;
                }
            }
            col.transform.parent = transform;
        }
    }

    void OnCollisionExit(Collision col)
    {
        //Set IsMoving to false if mode is CollisionStay
        //Set player parent to null when exiting
        if (col.gameObject.tag == "Player")
        {
            if (GetMode().Equals(Mode.OnPlayerCollisionStay))
            {
                m_IsMoving = false;

                if (m_ReturnOnExit)
                {
                    m_IsMoving = true;
                    m_IsReturning = true;
                    m_HasReachedEnd = false;
                    if (m_CurrentDir == 1)
                        m_CurrentDir = -1;
                    else
                        m_CurrentDir = 1;
                }
            }
            col.transform.parent = null;
        }
    }

    void SetDirection(Direction newDir)
    {
        m_Direction = newDir;
    }

    Direction GetDirection()
    {
        return m_Direction;
    }

    void SetMode(Mode newMode)
    {
        m_Mode = newMode;
    }
    Mode GetMode()
    {
        return m_Mode;
    }

    public void Reset()
    {
        transform.position = m_StartPos + m_StartOffsetPos;
        m_CurrentDir = m_StartDir;
        m_StayOnReachedEnd = m_StartStayOnEnd;
        m_HasReachedEnd = m_StartHasReachedEnd;
        m_IsAtStart = m_StartIsAtStart;
        m_ReturnOnExit = m_StartReturnOnExit;
        m_IsReturning = m_StartIsReturning;
        m_IsAtEnd = m_StartIsAtEnd;
        if (!GetMode().Equals(Mode.Constant))
        {
            m_IsMoving = false;
        }
        else
        {
            m_IsMoving = true;
        }
    }
}
