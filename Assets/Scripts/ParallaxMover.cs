using UnityEngine;
using System.Collections.Generic;

public class ParallaxMover : MonoBehaviour {
    public GameObject mainCamera;           // the main camera of the scene
    public float dampening;                 // will dampen the movement of the main camera by this factor when moving the bg camera

    private Vector3 lastPos;                // the position of the camera in the last frame update
    private Vector3 deltaPos;               // the difference in position since last frame

    private int frame;                      // the current frame number
    private const int frameToStart = 10;    // the frame at which to start tracking movement. used to make sure the camera jumps to the player prior to the bg tracking

    // BGCameraController will change this according to whether or not the bgCamera has left the bg image boundaries
    public Dictionary<Vector3, bool> movementAllowed;

    private Camera cam;
    void Awake()
    {
        // set up references
        cam = GetComponent<Camera>();

        movementAllowed = new Dictionary<Vector3, bool>();
        movementAllowed[Vector3.left]  = true;
        movementAllowed[Vector3.right] = true;
        movementAllowed[Vector3.up]    = true;
        movementAllowed[Vector3.down]  = true;
    }
	// Update is called once per frame
	void Update()
    {
                frame++;
        if (frame == frameToStart)
            lastPos = mainCamera.transform.position;
        if (frame >= frameToStart)
        {
            deltaPos = mainCamera.transform.position - lastPos;

            if (deltaPos != Vector3.zero)                             // if there was movement last frame
            {
                // if trying to move in a direction that is currently disallowed, kill that axis of movement
                if ((deltaPos.x > 0 && !movementAllowed[Vector3.right]) || (deltaPos.x < 0 && !movementAllowed[Vector3.left]))
                    deltaPos.x = 0;
                if ((deltaPos.y > 0 && !movementAllowed[Vector3.up])    || (deltaPos.y < 0 && !movementAllowed[Vector3.down]))
                    deltaPos.y = 0;

                // enable movement in the opposite direction of the last movement
                if (deltaPos.x > 0)
                    movementAllowed[Vector3.left] = true;
                if (deltaPos.x < 0)
                    movementAllowed[Vector3.right] = true;
                if (deltaPos.y > 0)
                    movementAllowed[Vector3.down] = true;
                if (deltaPos.y < 0)
                    movementAllowed[Vector3.up] = true;

                transform.position += ((1/dampening) * deltaPos);     // move the bg camera in the opposite direction of the camera's movement, and take dampening into account
            }
            lastPos = mainCamera.transform.position;
        }
    }
}
