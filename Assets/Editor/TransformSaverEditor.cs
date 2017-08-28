using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(TransformSaver))]
[System.Serializable]
public class TransformSaverEditor : Editor {
    /* Custom Editor for TransformSaver
     * Provides easy-to-use Buttons for saving/loading, as well as
     * checking for when PlayMode to ensure that Unity's serialization doesn't
     * destroy changes made in Play Mode
     */
    TransformSaver script;

    public void OnEnable()
    {
        EditorApplication.playmodeStateChanged += PlayModeChangeHandler;
        script = (TransformSaver)target;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        TransformSaver script = (TransformSaver)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("savedPos"));

        if (!script.IDSet)
            EditorGUILayout.HelpBox("Press \"Save Position\" To initialize saveID! \nUntil ID is set, any positions saved in Play Mode will be reverted.", MessageType.Warning);

        // buttons for save/load functionality
        if (GUILayout.Button("Save Position"))
        {
            Undo.RecordObjects(new Object[] { script, script.transform }, "save object position");
            script.SaveToDisk();
        }
        if (GUILayout.Button("Load Position"))
        {
            Undo.RecordObjects(new Object[] { script, script.transform }, "load object position");
            script.LoadPosition();
        }

        serializedObject.ApplyModifiedProperties();
    }


    // to avoid savedPos reverting to old value, load from disk whenever playmode is changed
    void PlayModeChangeHandler()
    {
        if (script)
            script.LoadFromDisk();
    }
}
#endif