﻿using UnityEngine;
using System.Collections;

public class ParkourObject : MonoBehaviour
{
    bool m_IsMovingLedge = false;

    public bool m_Wallrunnable;
    public bool m_Climbable;
    public bool m_VerticalClimbable;

    void Start()
    {
        if (this.gameObject.transform.parent != null)
        {
            if (this.gameObject.transform.parent.tag == "MovingPlatform")
            {
                if (this.gameObject.name == "LedgeX" || this.gameObject.name == "LedgeZ")
                {
                    m_IsMovingLedge = true;
                }
                else
                    m_IsMovingLedge = false;
            }
        }
    }

    public bool GetClimbable()
    {
        return m_Climbable;
    }

    void OnTriggerEnter(Collider col)
    {
        if (m_IsMovingLedge)
        {
            if (col.gameObject.tag == "Hand")
            {
                //Like, really unity?
                col.transform.parent.transform.parent.transform.parent.transform.parent = transform.parent.transform;
                Camera.main.transform.parent = transform.parent.transform;
                Debug.Log("Hands hit moving ledge");
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (m_IsMovingLedge)
        {
            if (col.gameObject.tag == "Hand")
            {
                col.transform.parent.transform.parent.transform.parent.transform.parent = null;
                Camera.main.transform.parent = null;
                Debug.Log("Hands left moving ledge");
            }
        }
    }
}