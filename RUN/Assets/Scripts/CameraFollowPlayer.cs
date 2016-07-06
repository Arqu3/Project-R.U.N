using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour
{
    Transform m_Player;
    public float m_yOffset = 1.4f;

	// Use this for initialization
	void Start ()
    {
        m_Player = GameObject.Find("PlayerBody").transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
	}
}
