using UnityEngine;
using System.Collections;


public class MeaninglessUnityError : System.Exception
{

    public MeaninglessUnityError(string s) : base(s)
    {
    }
}

public class MeaninglessSystemError : UnityException
{

    public MeaninglessSystemError(string s) : base(s)
    {
    }
}

public class ErrorBuildTester : MonoBehaviour {

    public enum ErrorType { unity, system }
    public ErrorType errorType;

    void OnTriggerEnter2D(Collider2D otherColl)
    {
        if (otherColl.tag == "player_tag")
        {
            switch (errorType)
            {
                case ErrorType.system:
                    throw new MeaninglessSystemError("System-derived error");
                case ErrorType.unity:
                    throw new MeaninglessUnityError("Unity-derived error");
            }
        }
    }
}
