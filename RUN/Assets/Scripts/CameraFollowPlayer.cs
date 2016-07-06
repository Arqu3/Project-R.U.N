using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour
{
    Transform m_Player;

	// Use this for initialization
	void Start ()
    {
        m_Player = GameObject.Find("PlayerBody").transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = new Vector3(m_Player.position.x, m_Player.position.y + 1.4f, m_Player.position.z);
	}
}
