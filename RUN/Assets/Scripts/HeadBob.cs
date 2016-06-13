using UnityEngine;
using System.Collections;

public class HeadBob : MonoBehaviour {

    public bool m_Active;
    public float m_HeadBobAmount;
    public float m_BobSpeed;
    int direction = 1;
    Vector3 m_OriginalPosition;
    ControllerPlayer m_Player;

    void Start()
    {
        m_OriginalPosition = transform.localPosition;
        m_Player = GetComponentInParent<ControllerPlayer>();
    }

	void Update () {
        if (m_Active)
        {
            if (m_Player.GetState().Equals(MovementState.Moving))
            {
                Bob();
            }
            else
            {
                MoveToCenter();
            }
        }
    }

    void Bob()
    {
            if (transform.localPosition.y > m_OriginalPosition.y + m_HeadBobAmount && direction != -1)
            {
                direction = -1;
            }
            if (transform.localPosition.y < m_OriginalPosition.y - m_HeadBobAmount && direction != 1)
            {
                direction = 1;
            }

            transform.localPosition += Vector3.up * Time.deltaTime * direction * m_BobSpeed;
    }

    void MoveToCenter()
    {
        float positionDelta = m_OriginalPosition.y - transform.localPosition.y;

        if (positionDelta > 0.1f || positionDelta < -0.1)
        {
            transform.localPosition += Vector3.up * Mathf.Sign(positionDelta) * Time.deltaTime;
        }
    }
}
