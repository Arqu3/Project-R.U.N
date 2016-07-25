using UnityEngine;
using System.Collections;

public class PlayerRotationHandler : MonoBehaviour
{

    ControllerPlayer m_Player;

    void Start()
    {
        m_Player = GetComponentInChildren<ControllerPlayer>();
    }

    void Update()
    {
        if (m_Player.GetIsControls())
        {
            if (!m_Player.GetState().Equals(MovementState.Wallrunning))
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);
                Camera.main.GetComponent<SimpleSmoothMouseLook>().ClampMouseX(transform.forward, 360);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, m_Player.GetWallrunDir(), 0.05f, 0f));
            }
        }
    }
}
