using UnityEngine;
using System.Collections;

public class ApplyForce : MonoBehaviour {
    public float forceOrVelocity;
    public enum ForceType {Force, Torque, Velocity}
    public ForceType forceType;

    private Rigidbody2D m_Rigidbody;
	// Use this for initialization
	void Awake() {
        m_Rigidbody = GetComponent<Rigidbody2D>();
	}
	void FixedUpdate()
    {
        switch (forceType)
        {
            case ForceType.Force:
                m_Rigidbody.AddForce(forceOrVelocity * transform.right, ForceMode2D.Force);
                break;
            case ForceType.Torque:
                m_Rigidbody.AddTorque(forceOrVelocity, ForceMode2D.Force);
                break;
            case ForceType.Velocity:
                m_Rigidbody.velocity = transform.right.normalized * forceOrVelocity;
                break;
        }
    }
}
