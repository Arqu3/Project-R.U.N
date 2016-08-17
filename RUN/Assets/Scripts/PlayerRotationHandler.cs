using UnityEngine;
using System.Collections;

public class PlayerRotationHandler : MonoBehaviour
{
    SimpleSmoothMouseLook m_Camera;
    ControllerPlayer m_Player;

    void Start()
    {
        m_Player = GetComponentInChildren<ControllerPlayer>();
        m_Camera = Camera.main.GetComponent<SimpleSmoothMouseLook>();
    }

    void Update()
    {
        if (m_Player.GetIsControls())
        {
            if (!m_Player.GetState().Equals(MovementState.Wallrunning) && !m_Player.GetState().Equals(MovementState.Grabbing))
            {
                transform.localEulerAngles = new Vector3(0.0f, Camera.main.transform.localEulerAngles.y, 0.0f);
                m_Camera.ClampMouseX(transform.forward, 360);
            }
            else if (m_Player.GetState().Equals(MovementState.Wallrunning))
            {
                transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, m_Player.GetWallrunDir(), 0.05f, 0f));
            }
            else if (m_Player.GetState().Equals(MovementState.Grabbing))
            {
                transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, m_Player.GetGrabbedDir(), 0.05f, 0f));
                m_Camera.ClampMouseX(m_Player.GetGrabbedDir(), 200);
            }
        }
    }
}
