using UnityEngine;
using System.Collections;

public class Front : MonoBehaviour
{
    public float m_GraceTime = 0.5f;
    float m_GraceTimer = 0.0f;

    ControllerPlayer m_Player;

	void Start ()
    {
        m_Player = GetComponentInParent<ControllerPlayer>();
	}

    void Update()
    {
        if (m_Player.GetState().Equals(MovementState.Falling))
            m_GraceTimer += Time.deltaTime;
        else
            m_GraceTimer = 0.0f;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ParkourObject>() && m_GraceTimer < m_GraceTime)
        {
            if (col.gameObject.GetComponent<ParkourObject>().m_VerticalClimbable)
            {
                m_Player.SetVerticalClimb(true);
                //Camera.main.GetComponent<SimpleSmoothMouseLook>()._mouseAbsolute.y = 65.0f;
                Debug.Log("Climb");
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.GetComponent<ParkourObject>())
        {
            m_Player.SetVerticalClimb(false);
        }
    }
}
