using UnityEngine;
using System.Collections;

public class Feet : MonoBehaviour
{
    ControllerPlayer m_Player;
    AnimationHandler m_AnimHandler;
    bool m_HasSentMsg = false;

	// Use this for initialization
	void Start ()
    {
        m_Player = GetComponentInParent<ControllerPlayer>();
        m_AnimHandler = GetComponentInParent<AnimationHandler>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<ParkourObject>())
        {
            if (col.GetComponent<ParkourObject>().m_Climbable)
            {
                if (!m_HasSentMsg)
                {
                    //m_AnimHandler.PlayAnimation("FeetClimb");
                    m_Player.SendMessage("FeetClimb");
                    m_HasSentMsg = true;

                    Debug.Log("Feetclimb");
                }
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.GetComponent<ParkourObject>())
        {
            if (col.GetComponent<ParkourObject>().m_Climbable)
            {
                m_HasSentMsg = false;
            }
        }
    }
}
