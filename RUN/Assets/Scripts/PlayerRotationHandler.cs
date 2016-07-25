using UnityEngine;
using System.Collections;

public class PlayerRotationHandler : MonoBehaviour {

    ControllerPlayer m_Player;

	void Start ()
    {
        m_Player = GetComponentInChildren<ControllerPlayer>();
	}
	
	void Update ()
    {
        if (m_Player.GetIsControls())
        {
            if (!m_Player.GetState().Equals(MovementState.Wallrunning))
            {
                transform.eulerAngles = new Vector3(transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);
            }
        }
	}
}
