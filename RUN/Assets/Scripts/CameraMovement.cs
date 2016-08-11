using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    public bool m_Active;
    public float m_HeadBobAmount;
    public float m_BobSpeed;
    int direction = 1;
    Vector3 m_OriginalPosition;
    Vector3 m_PositionOffset;
    ControllerPlayer m_Player;
    const float m_CameraOffset = 1.4f;
    float m_MoveToCenterTimer = 0f;

    bool m_BlinkStarted = false;
    float m_BlinkTimer = 0;
    const float m_BlinkUpTime = 0.2f;
    const float m_BlinkDownTime = 0.4f;

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

            if (m_Player.GetState().Equals(MovementState.Blinking))
            {
                if (!m_BlinkStarted) { 
                    StartCoroutine(LerpOverlay());
                }
            }
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
            m_MoveToCenterTimer = 0;
        }
        else
        {
            m_MoveToCenterTimer += Time.deltaTime;
            Vector3.Lerp(m_PositionOffset, Vector3.zero, m_MoveToCenterTimer);
        }
    }

    IEnumerator LerpOverlay()
    {
        m_BlinkStarted = true;
        UnityStandardAssets.ImageEffects.ScreenOverlay overlay = GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay>();

        int max = 1, min = 0;

        float time = m_BlinkUpTime;

        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                m_BlinkTimer = 0;
                int temp = max;
                max = min;
                min = temp;

                time = m_BlinkDownTime;
            }

            while (m_BlinkTimer < time)
            {
                overlay.intensity = Mathf.Lerp(min, max, m_BlinkTimer / time);

                m_BlinkTimer += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);

            }

        }

        m_BlinkStarted = false;
    }
}
