using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


[RequireComponent(typeof(ScoutController))]
public class ScoutUserControl : MonoBehaviour
{
    private ScoutController m_Character;
    private bool m_Jump;


    private void Awake()
    {
        m_Character = GetComponent<ScoutController>();
    }


    private void Update()
    {
        if (!m_Jump)
        {
            // Read the jump input in Update so button presses aren't missed.
            m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }
    }


    private void FixedUpdate()
    {
        // Read the inputs.
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        // Pass all parameters to the character control script.
        m_Character.Move(h, v, m_Jump);
        m_Jump = false;
    }
}
