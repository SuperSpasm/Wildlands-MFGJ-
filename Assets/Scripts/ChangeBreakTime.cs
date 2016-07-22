using UnityEngine;
using System.Collections;

public class ChangeBreakTime : MonoBehaviour {
    public float breakTime = 1.5f;

    private BreakBranch branchScript;

    void OnValidate()
    {
        if (!branchScript)
            branchScript = GetComponentInChildren<BreakBranch>();

        branchScript.timeToFall = breakTime;
    }
}
