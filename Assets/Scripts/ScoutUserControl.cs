using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


[RequireComponent(typeof(ScoutController))]
public class ScoutUserControl : MonoBehaviour
{
    private ScoutController m_Character;
    private bool m_Jump;
    [HideInInspector] public bool disableMovement = false;

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
        float newZ = Helper.GetTargetAngle(GetComponent< Rigidbody2D>().velocity);
        transform.rotation.SetEulerAngles(0, 0, newZ);
    }


    private void FixedUpdate()
    {
        //Debug.Log("Disable movement: " + disableMovement.ToString());
        // Read the inputs.
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        // Pass all parameters to the character control script.

        if (disableMovement) // if movement is disabled, pass zeros/false to move (since some functionality still needs to occur in Move())
            m_Character.Move(0, 0, false);
        else
            m_Character.Move(h, v, m_Jump);
        m_Jump = false;
    }
}
