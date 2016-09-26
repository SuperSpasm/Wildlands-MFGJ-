using UnityEngine;
using System.Collections.Generic;
using System;

public static class Helper
{
    // a static helper class for common methods and shortcuts
    public static Transform GetParent(Transform trans)
    {
        // returns the parent transform for the requested object, or null if it is top-level
        Transform parentTransform = trans.transform.parent;
        if (!parentTransform)
            return null;

        return parentTransform;
    }

    public static string GetHierarchy(GameObject obj)
    {
        // returns a string of the hierarchy for the given object (with each parent's name) seperated by '/' (including the object itself)
        var parents = new Stack<string>();
        string returnString = "/";
        Transform currParent = GetParent(obj.transform);

        int iterations = 0;
        while (currParent != null)
        {
            parents.Push(currParent.name);              //push each parent's name onto the stack until reached the top level game object
            currParent = GetParent(currParent);
            if (++iterations >= 50)
                throw new System.Exception("probably stuck in an infinite loop.");
        }

        foreach (string parentName in parents)          // for each parent's name in the stack (starting with the top level first)
            returnString += parentName + '/';           // append the parent name to the return string

        returnString += obj.name;                       // finally, append the name of the object this was called on
            
        return returnString;
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

    public static GameObject GetPlayer()
    {
        return ScoutController.player;
        //foreach (GameObject player in GameObject.FindGameObjectsWithTag("player_tag"))
        //{
        //    if (!player.name.EndsWith("(Clone)"))
        //        return player;
        //}
        //return null;
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