using UnityEngine;
using System.Collections;

public class Feet : MonoBehaviour
{
    ControllerPlayer m_Player;

	// Use this for initialization
	void Start ()
    {
        m_Player = GetComponentInParent<ControllerPlayer>();
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
                m_Player.SendMessage("FeetClimb");
            }
        }
    }
}
