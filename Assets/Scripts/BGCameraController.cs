using UnityEngine;
using System.Collections.Generic;

public class BGCameraController : MonoBehaviour {
    public Transform startBottomLeft;                           // the bottom left corner of the bg viewport to set at start

    [Tooltip("in prologueMode, camera will be set to max size and leftmost part of the bg image")]
    public bool prologueMode = false;
    [Header("Edges of background Image")]                       // place markers for the borders of the bg image, to make 
    public Transform bgBottomLeft;
    public Transform bgTopRight;

    private Camera cam;                                         // the camera component attached to this object
    private ParallaxMover parallax;                             // the component on this object used for parallax movement
    private const float adjustDist = 0.001f;                       // will move back by this value when the bg camera leaves the edges of the bg image, until camera shows bg image completely
    private const float adjustScale = 0.01f;                     // will scale down by this value when the bg camera gets too big for the edges of the screen, until camera shows bg image completely

    public Dictionary<Vector3, bool> camInBounds;                // will contain four keys- one for each 2D directional vector (up, down, left, right)
                                                                // each of these will be true if the camera is in the bounds of the bg image in that direction, false otherwise
    public enum Edge { LEFT, RIGHT, TOP, BOTTOM }               // an enum to hold edges


    public float camHalfHeight
    {
        get
        {
            return cam.orthographicSize;
        }
        // NOTE: this does NOT change aspect, and as such it changes width as well
        set
        {
            cam.orthographicSize = value;
        }
    }
    public float camHalfWidth
    {
        get
        {
            return cam.orthographicSize * cam.aspect;
        }
        // NOTE: this does NOT change aspect, and as such it changes height as well
        set
        {
            cam.orthographicSize = (value / cam.aspect);
        }
    }

    public Vector2 camBottomLeft
    {
        get
        {
            Vector2 pos = cam.transform.position;                           // the position of the camera's gameObject in world space
            return new Vector2(pos.x - camHalfWidth, pos.y - camHalfHeight);   // return the coordinates of the bottom left corner of the cameras viewport
        }
        private set
        {
            Vector2 newBottomLeft = value;
            transform.position = new Vector3(newBottomLeft.x + camHalfWidth, newBottomLeft.y + camHalfHeight, -10.0f);
        }
    }
    public Vector2 camTopRight
    {
        get
        {
            Vector2 pos = cam.transform.position;                           // the position of the camera's gameObject in world space
            return new Vector2(pos.x + camHalfWidth, pos.y + camHalfHeight);   // return the coordinates of the top right corner of the cameras viewport}
        }
        private set
        {
            Vector2 newTopRight = value;
            Vector2 pos = cam.transform.position;                           // the position of the camera's gameObject in world space
            transform.position = new Vector3(newTopRight.x - camHalfWidth, newTopRight.y - camHalfHeight, -10.0f);
        }
    }

    // smooth move vars
    public enum MoveStatus { Idle, Moving }
    private MoveStatus   moveStatus = MoveStatus.Idle;
    private Vector2     originalBottomLeft;
    public  Vector2     targetBottomLeft;
    public  float       moveTime;
    private float       moveCounter;


    // zoom vars

    public  enum        ZoomStatus { Idle, ZoomingIn, ZoomingOut }
    private ZoomStatus  zoomStatus = ZoomStatus.Idle;
    private float       originalHalfSize;                             // the height of the camera before the zoom started
    public  float       targetHalfSize;                               // the target height of the camera
    public  float       zoomTime;                                     // the time it will take to complete a zoom
    private float       zoomCounter;                                  // counts the time inbetween updates

    void Awake()
    {
        // set up references
        cam = GetComponent<Camera>();
        if (!cam.orthographic)
            throw new System.ArgumentException("camera must be orthographic!");

        parallax = GetComponent<ParallaxMover>();

        camInBounds = new Dictionary<Vector3, bool>();
        camInBounds[Vector3.left]     = false;
        camInBounds[Vector3.right]    = false;
        camInBounds[Vector3.up]       = false;
        camInBounds[Vector3.down]     = false;

    }

	void Start () {
        if (prologueMode)
        {
            ScaleCamToMax(0);
            SetToLeftMost();
        }
        else
            camBottomLeft = startBottomLeft.position;                                  // set up the camera correctly
	}

	void Update () {
        if (camBottomLeft.x <= bgBottomLeft.position.x)                            // the bg camera has gone out the boundaries (to the left)
        {
            MoveCamIntoBorders();
            parallax.movementAllowed[Vector3.left] = false;
        }

        else if (camTopRight.x >= bgTopRight.position.x)                           // the bg camera has gone out the boundaries (to the right)
        {
            MoveCamIntoBorders();
            parallax.movementAllowed[Vector3.right] = false;
        }
        if (camBottomLeft.y <= bgBottomLeft.position.y)                            // the bg camera has gone out the boundaries (underneath)
        {
            MoveCamIntoBorders();
            parallax.movementAllowed[Vector3.down] = false;
        }
        else if (camTopRight.y >= bgTopRight.position.y)                           // the bg camera has gone out the boundaries (over the top)
        {
            MoveCamIntoBorders();
            parallax.movementAllowed[Vector3.up] = false;
        }

        switch (zoomStatus)
        {
            case ZoomStatus.Idle:
                break;
            case ZoomStatus.ZoomingIn:
                if (cam.orthographicSize <= targetHalfSize)
                {
                    Debug.Log("zoom in over!");
                    cam.orthographicSize = targetHalfSize;
                    zoomStatus = ZoomStatus.Idle;
                }
                else
                    cam.orthographicSize = Mathf.Lerp(originalHalfSize, targetHalfSize, zoomCounter / zoomTime);
                break;
            case ZoomStatus.ZoomingOut:
                if (cam.orthographicSize >= targetHalfSize)
                {
                    Debug.Log("zoom out over!");
                    cam.orthographicSize = targetHalfSize;
                    zoomStatus = ZoomStatus.Idle;
                }
                else
                {
                    cam.orthographicSize = Mathf.Lerp(originalHalfSize, targetHalfSize, (zoomCounter / zoomTime));
                    //Debug.Log("lerping. lerp val = " + Mathf.Lerp(originalSize, targetSize, zoomCounter / zoomTime));
                }
                break;
        }
        switch (moveStatus)
        {
            case MoveStatus.Idle:
                break;
            case MoveStatus.Moving:
                if (moveCounter >= moveTime)                                // if the time to move has passed
                {
                    camBottomLeft = targetBottomLeft;
                    moveStatus = MoveStatus.Idle;
                }
                else                                                        // else if the time set aside for moving has not yet passed
                    camBottomLeft = Vector2.Lerp(originalBottomLeft, targetBottomLeft, (moveCounter / moveTime));
                break;
        }
        zoomCounter += Time.deltaTime;                                      // increase zoom/move counters. these will be reset to 0 when zoom/move status
        moveCounter += Time.deltaTime;                                      // is changed, so it doesn't matter that they're increased even when each status is idle

    }

    private void MoveCamIntoBorders(int directionsTried=0)
    {
        if (CamIsTooBig())
            ScaleCamToMax();

        if (IsInBorders())                                                  // if the cam is now within all borders, our work is done. return
            return;

        if (!camInBounds[Vector3.left])
        {
            //Debug.Log(string.Format("cam not in bounds from left! trying to fix. cam x: {0}, bg x: {1}", camBottomLeft.x, bgBottomLeft.position.x));
            float newX = bgBottomLeft.position.x + adjustDist;
            camBottomLeft = new Vector2(newX, camBottomLeft.y);
            //Debug.Log(string.Format("done moving RIGHT. cam x: {0}, bg x: {1}. in borders? {2}", camBottomLeft.x, bgBottomLeft.position.x, IsInBorders()));
        }
        else if (!camInBounds[Vector3.right])
        {
            float newX = bgTopRight.position.x - adjustDist;
            camTopRight = new Vector2(newX, camTopRight.y);
        }
        if (!camInBounds[Vector3.down])
        {
            //Debug.Log(string.Format("cam not in bounds from bottom! trying to fix. cam y: {0}, bg y: {1}", camBottomLeft.y, bgBottomLeft.position.y));
            float newY = bgBottomLeft.position.y + adjustDist;
            camBottomLeft = new Vector2(camBottomLeft.x, newY);
            //Debug.Log(string.Format("done moving UP. cam y: {0}, bg y: {1}. in borders? {2}", camBottomLeft.y, bgBottomLeft.position.y, IsInBorders()));
        }
        else if (!camInBounds[Vector3.up])
        {
            float newY = bgTopRight.position.y - adjustDist;
            camTopRight = new Vector2(camTopRight.x, newY);
        }

        // if script was unable to return bgCamera to within the specified border within [maxIterations], throw an error
        if (!IsInBorders())
            throw new System.Exception(string.Format("Unable to move camera back within borders! adjustment distance: {0}", adjustDist));
    }

    public void SmoothMoveTo(Vector2 targetBottomLeft, float moveTime)
    {
        if (CamIsTooBig())
            throw new UnityException("Camera is too big for bg image! can't move it until you resize.");

        Vector2 maxBottomLeft = GetMaxBottomLeft(camHalfHeight, cam.aspect);    // the top-most,   right-most bottomLeft corner for the camera that wont leave any bounds
        Vector2 minBottomLeft = GetMinBottomLeft();                             // the bottom-most, left-most bottomLeft corner for the camera that wont leave any bounds

        List<Edge> bordersCrossed = GetBordersCrossed(targetBottomLeft, camHalfHeight,cam.aspect);
        foreach (Edge border in bordersCrossed)                                 // for each border crossed (hypothetically), change the target position so that it is within that border
            switch (border)                                                     
            {
                case Edge.BOTTOM:
                    targetBottomLeft.y = minBottomLeft.y;
                    break;
                case Edge.TOP:
                    targetBottomLeft.y = maxBottomLeft.y;
                    break;
                case Edge.LEFT:
                    targetBottomLeft.x = minBottomLeft.x;
                    break;
                case Edge.RIGHT:
                    targetBottomLeft.x = maxBottomLeft.x;
                    break;
            }

        moveStatus = MoveStatus.Moving;
        this.originalBottomLeft = camBottomLeft;
        this.targetBottomLeft = targetBottomLeft;
        this.moveTime = moveTime;
        moveCounter = 0;
    }

    public void ZoomOut(float targetHalfSize, float zoomTime)
    {
        Debug.Log(string.Format("ZoomOut(halfSize={0},zoomTime={1})", targetHalfSize, zoomTime));
        if (targetHalfSize < cam.orthographicSize)
            throw new System.ArgumentException(string.Format("camera height already larger than target size provided! now: {0}, target: {1}", cam.orthographicSize, targetHalfSize));

          Debug.Log("2");
        if (CamIsTooBig(targetHalfSize, cam.aspect))                        // if the target size is bigger than the bg image, scale it to max size
        {
                Debug.Log(string.Format("target size too big: {0}, calling ScaleCamToMax()", targetHalfSize));
                ScaleCamToMax(zoomTime);                                    // this will call ZoomOut() again with the appropriate value
                return;
        }
        if (zoomTime == 0)                                                  // if the zoom time is set to 0, set the size and end coroutine
        {
            Debug.Log("3");
            cam.orthographicSize = targetHalfSize;
            return;
        }
            

        this.originalHalfSize = cam.orthographicSize;
        this.targetHalfSize = targetHalfSize;
        this.zoomTime = zoomTime;
        zoomCounter = 0;
        zoomStatus = ZoomStatus.ZoomingOut;

    }
    public void ZoomIn(float targetHalfSize, float zoomTime)
    {
        if (cam.orthographicSize <= targetHalfSize)
            throw new System.ArgumentException(string.Format("camera height already smaller than target size provided! now: {0}, target: {1}", cam.orthographicSize, targetHalfSize));

        if (zoomTime == 0)                                                  // if the zoom time is set to 0, set the size and end coroutine
        {
            cam.orthographicSize = targetHalfSize;
            return;
        }

        this.originalHalfSize = cam.orthographicSize;
        this.targetHalfSize = targetHalfSize;
        this.zoomTime = zoomTime;
        zoomCounter = 0;
        zoomStatus = ZoomStatus.ZoomingIn;
    }


    #region helper methods

    // NOTE: these are pretty ugly, and many are redundant. I went through a few iterations in designing this and the parallax controller
    // each time adding functionality and trying to modularize where I could, which led to an abundance of methods, some of which are unnecessary
    // the efficiency boost of replacing them would probably be unremarkable though, so I'm not going to for now.
    //
    // If it ain't broke, don't fix it!

    public void ScaleCamToMax()
    {
        ScaleCamToMax(this.zoomTime);
    }
    public void ScaleCamToMax(float zoomTime)
    {
        Debug.Log("ScaleCamToMax() called");
        float maxHalfHeight = GetMaxHalfHeight(cam.aspect);
        if (cam.orthographicSize < maxHalfHeight)
            ZoomOut(maxHalfHeight, zoomTime);
        else if (cam.orthographicSize > maxHalfHeight)
            ZoomIn(maxHalfHeight, zoomTime);
        else
            Debug.Log("ScaleCamToMax called, but cam size is already max! doing nothing. curr size: " + cam.orthographicSize);

        //float imageHeight = bgTopRight.position.y - bgBottomLeft.position.y;
        //float imageWidth  = bgTopRight.position.x - bgBottomLeft.position.x;
        //camHalfHeight = (imageHeight - adjustScale) / 2;                     // first, scale down the camera height to slightly below maximum
        //if (camHalfWidth * 2 >= imageWidth)                                  // if cam width is still larger or equal to image width
        //{
        //    camHalfWidth = (imageWidth - adjustScale) / 2;
        //}

    }

    ///<summary>
    ///used to check whether camera is currently bigger than bg image in x or y axis
    ///</summary> 
    private bool CamIsTooBig()
    {
        float bgImageHeight = bgTopRight.position.y - bgBottomLeft.position.y;
        float bgImageWidth = bgTopRight.position.x - bgBottomLeft.position.x;
        if (camHalfHeight * 2 >= bgImageHeight || camHalfWidth * 2 >= bgImageWidth)
            return true;
        return false;
    }
    ///<summary>
    ///used to check whether a hypothetical size of the camera will be bigger than bg image in x or y axis
    ///</summary> 
    private bool CamIsTooBig(float targetHalfHeight, float targetAspect)
    {

        float bgImageHeight = bgTopRight.position.y - bgBottomLeft.position.y;
        //float bgImageWidth = bgTopRight.position.x - bgBottomLeft.position.x;
        //float targetHalfWidth = targetAspect * targetHalfHeight;

        if (targetHalfHeight * 2 >= bgImageHeight)// || targetHalfWidth * 2 >= bgImageWidth)
            return true;
        return false;
    }



    // Set cam to the furthest in x or y (in the direction chosen) without altering the other (x/y) coordinate
    private void SetToRightMost()
    {
        float targetX = bgTopRight.position.x - camHalfWidth - (adjustDist / 2);
        camBottomLeft = new Vector2(targetX, camBottomLeft.y);
    }
    private void SetToLeftMost()
    {
        float targetX = GetMinBottomLeft().x;
        camBottomLeft = new Vector2(targetX, camBottomLeft.y);
    }
    private void SetToTopMost()
    {
        float targetY = bgTopRight.position.y - camHalfHeight - (adjustDist / 2);
        camBottomLeft = new Vector2(camBottomLeft.x, targetY);
    }
    private void SetToBottomMost()
    {
        float targetY = GetMinBottomLeft().y;
        camBottomLeft = new Vector2(camBottomLeft.x, targetY);
    }


    private bool IsInBorders()
    {
        bool debug = false;
        UpdateBounds();                                     // updates the camInBounds dictionary by checking the  relative position of the camera to the bg image borders
        // if camera is within all borders
        if (camInBounds[Vector3.left] && camInBounds[Vector3.right] && camInBounds[Vector3.up] && camInBounds[Vector3.down])          // AND camera isn't off to the bottom or top 
        {
            if (debug)
                Debug.Log("Is in borders!");
            return true;
        }

        ///// debugging
        if (debug)
        {
            if (!camInBounds[Vector3.left])
                Debug.Log("Out of LEFT border");
            else if (!camInBounds[Vector3.right])
                Debug.Log("Out of RIGHT border");
            else if (!camInBounds[Vector3.up])
                Debug.Log("Out of TOP border");
            else if (!camInBounds[Vector3.down])
                Debug.Log("Out of BOTTOM border");
        }
        /////
        return false;
    }

    private bool IsInBorders(Vector2 targetBottomLeft, float targetHalfHeight, float targetAspect)
    {
        Vector2 maxBottomLeft = GetMaxBottomLeft(targetHalfHeight, targetAspect);           // the maximum bottom left (in terms of x/y values) for which the top-right corner of the camera will nearly leave bounds
        Vector2 minBottomLeft = GetMinBottomLeft();                                         // the minimum bottom left (in terms of x/y values) for which the bottom-left corner of the camera will nearly leave bounds
        bool xOK = (targetBottomLeft.x >= minBottomLeft.x && targetBottomLeft.x <= maxBottomLeft.x);
        bool yOK = (targetBottomLeft.y >= minBottomLeft.y && targetBottomLeft.y <= maxBottomLeft.y);
        return (xOK && yOK);
    }
    /// <summary>
    /// For a hypothetical size and position, get the borders that would be crossed
    /// </summary>
    /// <returns>A list of Edges crossed or an empty list if is in borders</returns>
    private List<Edge> GetBordersCrossed(Vector2 targetBottomLeft, float targetHalfHeight, float targetAspect)
    {
        Vector2 maxBottomLeft = GetMaxBottomLeft(targetHalfHeight, targetAspect);
        Vector2 minBottomLeft = GetMinBottomLeft();

        var bordersCrossed = new List<Edge>();
        if (targetBottomLeft.x < minBottomLeft.x) bordersCrossed.Add(Edge.LEFT);      // left LEFT border
        if (targetBottomLeft.x > maxBottomLeft.x) bordersCrossed.Add(Edge.RIGHT);     // left RIGHT border
        if (targetBottomLeft.y < minBottomLeft.y) bordersCrossed.Add(Edge.BOTTOM);    // left BOTTOM border
        if (targetBottomLeft.y > maxBottomLeft.y) bordersCrossed.Add(Edge.TOP);       // left TOP border

        return bordersCrossed;
    }



    /// <summary>
    /// Get the rightmost/topmost Bottom left corner for the camera for the hypothetical given height and aspect
    /// such that the camera wil have [adjustDist] units between it and the bg image border on each axis
    /// </summary>
    private Vector2 GetMaxBottomLeft(float targetHalfHeight, float targetAspect)
    {
        float targetHalfWidth = targetHalfHeight * targetAspect;

        Vector2 maxBottomLeft;
        maxBottomLeft.x = bgTopRight.position.x - (targetHalfWidth * 2);
        maxBottomLeft.y = bgTopRight.position.y - (targetHalfHeight * 2);

        maxBottomLeft.x -= adjustDist;
        maxBottomLeft.y -= adjustDist;

        return maxBottomLeft;

    }
    private Vector2 GetMinBottomLeft()
    {
        return new Vector2(bgBottomLeft.position.x - adjustDist, bgBottomLeft.position.y - adjustDist);
    }

    private float GetMaxHalfHeight(float aspect)
    {
        float bgX = bgTopRight.position.x - bgBottomLeft.position.x;
        float bgY = bgTopRight.position.y - bgBottomLeft.position.y;
        Vector2 maxSize = new Vector2(bgX, bgY);

        float maxHalfHeight = (maxSize.y - adjustScale) / 2;             // first, scale down the camera height to slightly below maximum (in height)
        float maxHalfWidth = maxHalfHeight * aspect;

        if (maxHalfWidth * 2 >= maxSize.x)                              // if cam width is still larger or equal to BG image width
        {
            maxHalfWidth = (maxSize.x - adjustScale) / 2;               // get max width
            maxHalfHeight = maxHalfWidth / aspect;                      // set max height according to max width
        }
        return maxHalfHeight;
    }

    private void UpdateBounds()
    {
        camInBounds[Vector3.left] = (camBottomLeft.x >= bgBottomLeft.position.x) ? true : false;
        camInBounds[Vector3.right] = (camTopRight.x <= bgTopRight.position.x) ? true : false;
        camInBounds[Vector3.down] = (camBottomLeft.y >= bgBottomLeft.position.y) ? true : false;
        camInBounds[Vector3.up] = (camTopRight.y <= bgTopRight.position.y) ? true : false;
    }

    // updates the parallax movementAllowed dictionary according to the camInBounds dictionary
    private void UpdateParallaxByBounds()
    {
        parallax.movementAllowed[Vector3.left] = camInBounds[Vector3.left];
        parallax.movementAllowed[Vector3.right] = camInBounds[Vector3.right];
        parallax.movementAllowed[Vector3.up] = camInBounds[Vector3.up];
        parallax.movementAllowed[Vector3.down] = camInBounds[Vector3.down];
    }
    #endregion
}
