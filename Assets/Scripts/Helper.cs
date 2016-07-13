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
    public static float GetEdge(CircleCollider2D coll, string side)
    {
        side = side.ToUpper(); // ignore caps in input

        if (coll.transform.rotation != Quaternion.identity) // only accept unrotated objects
            throw new System.ArgumentException("GetEdge() can only take unrotated objects!");


        Vector2 trueCenter = GetRealPos(coll);
        switch (side)
        {
            case "LEFT":
                return trueCenter.x - (coll.radius);
            case "RIGHT":
                return trueCenter.x + (coll.radius);
            case "TOP":
                return trueCenter.y + (coll.radius);
            case "BOTTOM":
                return trueCenter.y - (coll.radius);

            default: // handle invalid side argument
                throw new System.ArgumentException(string.Format("called GetEdge() with invalid 'side' argument. ({0}) allowed: left, right,top, bottom",
                                          side == null ? "null" : side));
        }
    }


    /// <summary>
    ///get the position (center or pivot) of a collider in world space (considering offset).
    ///
    /// </summary>

    public static Vector2 GetRealPos(Collider2D coll)
    {
        Vector2 initialPos = coll.transform.position;
        return initialPos + coll.offset;
    }

    public static void Main (string[] args)
    {
        CircleCollider2D c = new CircleCollider2D();
        System.Console.WriteLine(c.GetType().IsInstanceOfType(new CircleCollider2D()));
    }
}