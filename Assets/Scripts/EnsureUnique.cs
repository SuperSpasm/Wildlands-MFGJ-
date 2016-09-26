using UnityEngine;
using System.Collections.Generic;

public class EnsureUnique : MonoBehaviour {
    public enum CheckMode { tag, uniqueID }
    public CheckMode checkMode;
    public string uniqueID;
    public static Dictionary<string, GameObject> uniqueGameObjects;
    private string identifier; // will be the objects tag or the uniqueID string, depending on checkMode
    void Awake()
    {
        // if the dictionary hasn't been assigned yet (has default value)
        if (uniqueGameObjects == default(Dictionary<string, GameObject>))
            uniqueGameObjects = new Dictionary<string, GameObject>();

        // set the identifier according to checkMode
        switch (checkMode)
        {
            case CheckMode.tag:
                identifier = gameObject.tag;
                break;
            case CheckMode.uniqueID:
                identifier = uniqueID;
                break;
        }


        if (!uniqueGameObjects.ContainsKey(identifier))           // if the dictionary doesn't contain the uniqueID key, add this gameobject under it
            uniqueGameObjects[identifier] = gameObject;

        else if ( uniqueGameObjects[identifier] != gameObject)    // else (if it does contain the key), and the value is NOT this game object
            Destroy(gameObject);                                  // destroy this object (it is a duplicate)
    }
}
