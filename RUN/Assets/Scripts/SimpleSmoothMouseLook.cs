using System.Collections;
using UnityEngine;

// Very simple smooth mouselook modifier for the MainCamera in Unity
// by Francis R. Griffiths-Keam - www.runningdimensions.com

[AddComponentMenu("Camera/Simple Smooth Mouse Look ")]
public class SimpleSmoothMouseLook : MonoBehaviour
{
    public Vector2 _mouseAbsolute;
    Vector2 _smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);
    public bool lockCursor;
    public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);
    public Vector2 targetDirection;
    public Vector2 targetCharacterDirection;

    float m_ClampX;
    float m_ClampXMax;
    float m_ClampXMin;

    float m_initialAngle;
    float m_ControllerSens;

    Vector2[] inputs = new Vector2[2];

    // Assign this if there's a parent object controlling motion, such as a Character Controller.
    // Yaw rotation will affect this object instead of the camera if set.
    public GameObject characterBody;

    void Start()
    {
        m_ControllerSens = PlayerPrefs.GetFloat("Controller Sensitivity", 1.0f);

        sensitivity = new Vector2(m_ControllerSens, m_ControllerSens);

        m_initialAngle = Vector3.Angle(transform.forward, Vector3.right);

        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        if (characterBody) targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
    }

    void Update()
    {   
        // Ensure the cursor is always locked when set
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        inputs[0] = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        inputs[1] = new Vector2(Input.GetAxisRaw("Joystick X"), Input.GetAxisRaw("Joystick Y"));

        foreach (Vector2 input in inputs)
        {
            // Get raw mouse input for a cleaner reading on more sensitive mice.
            //var mouseDelta = new Vector2(Input.GetAxisRaw("Joystick X"), Input.GetAxisRaw("Joystick Y"));
            var mouseDelta = input;

            // Scale input against the sensitivity setting and multiply that against the smoothing value.
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

            // Interpolate mouse movement over time to apply smoothing delta.
            _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
            _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

            // Find the absolute mouse movement value from point zero.
            _mouseAbsolute += _smoothMouse;

            /*
            if (_mouseAbsolute.x < 0)
            {
                _mouseAbsolute.x = _mouseAbsolute.x + 360;
            }
            _mouseAbsolute.x = _mouseAbsolute.x % 360;
            */

            // Clamp and apply the local x value first, so as not to be affected by world transforms.
            if (_mouseAbsolute.x < 0)
            {
                _mouseAbsolute.x = 720 + _mouseAbsolute.x;
            }

            if (m_ClampX < 360)
            {
                _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, m_ClampXMin, m_ClampXMax);
            }

            // _mouseAbsolute.x = _mouseAbsolute.x % 360;

            var xRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
            transform.localRotation = xRotation;

            // Then clamp and apply the global y value.
            if (clampInDegrees.y < 360)
                _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

            transform.localRotation *= targetOrientation;

            // If there's a character body that acts as a parent to the camera
            if (characterBody)
            {
                var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, characterBody.transform.up);
                characterBody.transform.localRotation = yRotation;
                characterBody.transform.localRotation *= targetCharacterOrientation;
            }

            else
            {
                var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
                transform.localRotation *= yRotation;
            }
        }
    }

    public void ClampMouseX(Vector3 dir, float amount)
    {
        m_ClampX = amount;

        dir = Quaternion.AngleAxis(m_initialAngle, Vector3.up) * dir;

        float orientation = -Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

        float x = -Mathf.Atan2(dir.z, dir.x);
        
        x = (x > 0 ? x : (2 * Mathf.PI + x)) * 360 / (2 * Mathf.PI);
        x = (x) % 360;

        orientation = x;

        if (amount < 360)
        {
            float tempMouseAbs = _mouseAbsolute.x;
            int positive = 1;
            int mult = 0;

            if (tempMouseAbs < 0)
            {
                positive = -1;
                tempMouseAbs = Mathf.Abs(tempMouseAbs);
            }

            while(tempMouseAbs > 360) { 
                tempMouseAbs -= 360;
                mult++;
            }

            float round = 0;

            if (Mathf.Abs(tempMouseAbs - orientation) > Mathf.Abs(tempMouseAbs - 360 - orientation))
                round = 360;
            
            m_ClampXMax = (float)((orientation + amount * 0.5) + mult * 360 + round) * positive;
            m_ClampXMin = (float)((orientation - amount * 0.5) + mult * 360 + round) * positive;

            if (m_ClampXMax < 0 || m_ClampXMin < 0)
            {
                m_ClampXMax += 360;
                m_ClampXMin += 360;
                _mouseAbsolute.x += 360;
            }

            if (m_ClampXMax < m_ClampXMin)
            {
                float temp = m_ClampXMax;
                m_ClampXMax = m_ClampXMin;
                m_ClampXMin = temp;
            }
        }
    }
}
