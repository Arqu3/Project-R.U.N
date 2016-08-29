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

        if (!m_Player.GetState().Equals(MovementState.VerticalClimbing))
            Camera.main.GetComponent<SimpleSmoothMouseLook>().m_LookUp = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ParkourObject>())
        {
            if (col.gameObject.GetComponent<ParkourObject>().m_VerticalClimbable && m_GraceTimer < m_GraceTime && m_Player.GetCanVertical())
            {
                m_Player.SetVerticalClimb(true);
                Debug.Log("Climb");
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.GetComponent<ParkourObject>())
        {
            Camera.main.GetComponent<SimpleSmoothMouseLook>().m_LookUp = false;
            m_Player.SetVerticalClimb(false);
            m_Player.SetCanVertical(false);
        }
    }
}
