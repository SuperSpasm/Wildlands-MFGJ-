using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[System.Serializable]
public class TransformSaver : MonoBehaviour {
    /* Save the position of the GameObject attached to retrieve at will. 
     * (Saves the position in PlayerPrefs to enable saving while in Play Mode)
     * 
     * NOTE: Meant to be used in conjunction with TransformSaverEditor, which provides Buttons for Save/Load 
     *                                                                & fixes savedPos reverting after exiting PlayMode
     * 
     * Recommended use:
     *      Often when testing a level, you'll want to move your player/objects around, but afterwards return them to their initial position
     *      This is useful to remember their proper position, and return them when you're done testing
     */


    [SerializeField] public Vector3 savedPos;
    [SerializeField] private string saveID;
    [SerializeField, HideInInspector] public bool IDSet;



    public void LoadPosition()
    {
        transform.position = savedPos;
    }

    public void SaveToDisk()
    {
        if (!IDSet)
        {
            saveID = Random.Range(100000, 999999).ToString();
            IDSet = true;
        }
        savedPos = transform.position;

        PlayerPrefs.SetFloat(saveID + "x", savedPos.x);
        PlayerPrefs.SetFloat(saveID + "y", savedPos.y);
        PlayerPrefs.SetFloat(saveID + "z", savedPos.z);
        PlayerPrefs.Save();
    }

    [ExecuteInEditMode]
    public void LoadFromDisk()
    {
        if (!IDSet)
            return;
        Debug.Log("Load from disk");
        savedPos.x = PlayerPrefs.GetFloat(saveID + "x");
        savedPos.y = PlayerPrefs.GetFloat(saveID + "y");
        savedPos.z = PlayerPrefs.GetFloat(saveID + "z");
    }
}
#endif