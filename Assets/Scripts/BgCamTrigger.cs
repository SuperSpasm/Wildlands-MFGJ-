using UnityEngine;
using System.Collections;

public class BgCamTrigger : MonoBehaviour {
    public enum Action { ZoomIn, ZoomOut, SmoothMove, ScaleToMax }
    public Action action;
    public GameObject bgCamera;

    [Header("Zoom")]
    public float targetSize;
    public float zoomTime;

    [Header("Smooth Move")]
    public Transform targetLocation;
    public float moveTime;

    private BGCameraController camControl;

    private bool triggered = false;
    void Awake()
    {
        //set up references
        camControl = bgCamera.GetComponent<BGCameraController>();
    }


    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.tag != "player_tag" || triggered)
            return;

        switch (action)
        {
            case Action.ZoomIn:
                camControl.ZoomIn(targetSize, zoomTime);
                break;
            case Action.ZoomOut:
                camControl.ZoomOut(targetSize, zoomTime);
                break;
            case Action.SmoothMove:
                camControl.SmoothMoveTo(targetLocation.position, moveTime);
                break;
            case Action.ScaleToMax:
                camControl.ScaleCamToMax(zoomTime);
                break;
        }
        triggered = true;
    }





}
