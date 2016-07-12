using UnityEngine;
using System.Collections;

public class HeadBob : MonoBehaviour {

    public bool m_Active;
    public float m_HeadBobAmount;
    public float m_BobSpeed;
    int direction = 1;
    Vector3 m_OriginalPosition;
    Vector3 m_PositionOffset;
    ControllerPlayer m_Player;
    const float m_CameraOffset = 1.4f;


    void Start()
    {
        m_Player = FindObjectOfType<ControllerPlayer>();
        m_OriginalPosition = m_Player.transform.position;


    }

	void Update () {
        if (m_Active)
        {
            if (m_Player.GetState().Equals(MovementState.Moving) || m_Player.GetState().Equals(MovementState.Wallrunning))
            {
                Bob();
            }
            else
            {
                MoveToCenter();
            }

            transform.position = m_Player.transform.position + new Vector3(0, m_CameraOffset) + m_PositionOffset;
        }
    }

    void Bob()
    {
            if (m_Player.transform.position.y + m_CameraOffset + m_PositionOffset.y > m_Player.transform.position.y + m_CameraOffset + m_HeadBobAmount && direction != -1)
            {
                direction = -1;
            }
            if (m_Player.transform.position.y + m_CameraOffset + m_PositionOffset.y < m_Player.transform.position.y + m_CameraOffset - m_HeadBobAmount && direction != 1)
            {
                direction = 1;
            }

            m_PositionOffset += Vector3.up * Time.deltaTime * direction * m_BobSpeed;
    }

    void MoveToCenter()
    {
        float positionDelta = m_Player.transform.position.y + m_CameraOffset - (m_Player.transform.position.y + m_CameraOffset + m_PositionOffset.y);

        if (positionDelta > 0.1f || positionDelta < -0.1)
        {
            m_PositionOffset += Vector3.up * Mathf.Sign(positionDelta) * Time.deltaTime;
        }
    }
}
