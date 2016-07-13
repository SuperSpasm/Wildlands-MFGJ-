using UnityEngine;
using System.Collections;

public static class Helper
{
    // a static helper class for common methods and shortcuts
    public static GameObject GetParent(GameObject obj)
    {
        Transform parentTransform = obj.transform.parent;
        if (parentTransform == null)
            return null;

        return parentTransform.gameObject;
    }

    public static float GetEdge(Collider2D coll, string side)
    {
        // if passed a Collider2D - check whether it's a circle or a box and call appropriate overload
        if (coll.GetType() == typeof(BoxCollider2D))
            return GetEdge((BoxCollider2D)coll,side);

        else if (coll.GetType() == typeof(CircleCollider2D))
            return GetEdge((CircleCollider2D)coll,side);

        else // if it's not a circle or box collider, throw an exception
            throw new System.ArgumentException("GetRealPos(Collider2D) only accepts box or circle colliders!");
    }

    public static float GetEdge(BoxCollider2D coll,string side)
    {
        side = side.ToUpper(); // ignore caps in input

        if (coll.transform.rotation != Quaternion.identity) // only accept unrotated objects
            throw new System.ArgumentException("GetEdge() can only take unrotated objects!");


        Vector2 trueCenter = GetRealPos(coll);
        switch (side)
        {
            case "LEFT":
                return trueCenter.x - (coll.size.x / 2);
            case "RIGHT":
                return trueCenter.x + (coll.size.x / 2);
            case "TOP":
                return trueCenter.y + (coll.size.y / 2);
            case "BOTTOM":
                return trueCenter.y - (coll.size.y / 2);

            default: // handle invalid side argument
                throw new System.ArgumentException(string.Format("called GetEdge() with invalid 'side' argument. ({0}) allowed: left, right,top, bottom",
                                          side == null ? "null" : side) );
        }
    }


    /// <summary>
    ///get the position (center or pivot) of a collider in world space (considering offset).
    /// ONLY WORKS WITH BOX OR CIRCLE COLLIDERS
    /// </summary>

    public static Vector2 GetRealPos(Collider2D coll)
    {
        // if passed a Collider2D - check whether it's a circle or a box and call appropriate overload
        if (coll.GetType() == typeof(BoxCollider2D))
            return GetRealPos((BoxCollider2D)coll);

        else if (coll.GetType() == typeof(CircleCollider2D))
            return GetRealPos((CircleCollider2D)coll);

        else // if it's not a circle or box collider, throw an exception
            throw new System.ArgumentException("GetRealPos(Collider2D) only accepts box or circle colliders!");
    }
    public static Vector2 GetRealPos(BoxCollider2D boxCollider)
    { 
        Vector2 initialPos = boxCollider.transform.position;
        return initialPos + boxCollider.offset;
    }
    public static Vector2 GetRealPos(CircleCollider2D circleCollider)
    {
        Vector2 initialCenter = circleCollider.transform.position;
        return initialCenter + circleCollider.offset;
    }

    public static void Main (string[] args)
    {
        CircleCollider2D c = new CircleCollider2D();
        System.Console.WriteLine(c.GetType().IsInstanceOfType(new CircleCollider2D()));
    }
}