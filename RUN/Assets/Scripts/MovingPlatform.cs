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
    public float m_Speed = 1.0f;
    public float m_Distance = 10.0f;
    public float m_PercentageOffset = 0.0f;

    //Other vars
    bool m_IsMoving;
    int m_CurrentDir;
    Vector3 m_MoveToPos;
    Vector3 m_VDirection;
    Vector3 m_StartPos;

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
    }
	
	void Update ()
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
                    }
                    else if (transform.position.x < m_StartPos.x)
                    {
                        m_CurrentDir = 1;
                    }
                }
                else if (m_Distance < 0)
                {
                    if (transform.position.x < m_MoveToPos.x)
                    {
                        m_CurrentDir = 1;
                    }
                    else if (transform.position.x > m_StartPos.x)
                    {
                        m_CurrentDir = -1;
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
                    }
                    else if (transform.position.y < m_StartPos.y)
                    {
                        m_CurrentDir = 1;
                    }
                }
                else if (m_Distance < 0)
                {
                    if (transform.position.y < m_MoveToPos.y)
                    {
                        m_CurrentDir = 1;
                    }
                    else if (transform.position.y > m_StartPos.y)
                    {
                        m_CurrentDir = -1;
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
                    }
                    else if (transform.position.z < m_StartPos.z)
                    {
                        m_CurrentDir = 1;
                    }
                }
                else if (m_Distance < 0)
                {
                    if (transform.position.z < m_MoveToPos.z)
                    {
                        m_CurrentDir = 1;
                    }
                    else if (transform.position.z > m_StartPos.z)
                    {
                        m_CurrentDir = -1;
                    }
                }
                transform.Translate(0.0f, 0.0f, m_Speed * m_CurrentDir * Time.deltaTime, Space.World);
            }
        }
	}

    void OnCollisionEnter(Collision col)
    {
        //Set IsMoving to true if mode isn't Constant
        //Set player parent to this platform
        if (col.gameObject.tag == "Player")
        {
            if (!GetMode().Equals(Mode.Constant))
            {
                m_IsMoving = true;
            }
            col.transform.parent = transform;
        }
    }

    void OnCollisionStay(Collision col)
    {
        //Keep IsMoving true if mode is CollisionStay
        if (col.gameObject.tag == "Player")
        {
            if (GetMode().Equals(Mode.OnPlayerCollisionStay))
            {
                m_IsMoving = true;
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
}
