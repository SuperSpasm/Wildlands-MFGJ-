using UnityEngine;
using System.Collections;

public class VineCollisionFix : MonoBehaviour {
    public LayerMask mask;
    private bool[] layersToDisable1;
    private bool[] layersToDisable2;
    private int thisLayer;
    public enum WhenToDisable { Awake, Start, Update, FixedUpdate }
    public WhenToDisable whenToDisable = WhenToDisable.Start;
    void Awake()
    {
        thisLayer = gameObject.layer;                                                       // get this object's layer


        if (whenToDisable == WhenToDisable.Awake)
            Disable();
    }

    void Start()
    {
        if (whenToDisable == WhenToDisable.Start)
            Disable();
    }
    void Update()
    {
        if (whenToDisable == WhenToDisable.Update)
            Disable();
    }
    void FixedUpdate()
    {
        if (whenToDisable == WhenToDisable.FixedUpdate)
            Disable();
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        int layer = coll.collider.gameObject.layer;                                         // the layer of the colliding object
        if ((mask & (1 << layer)) != 0)                                                     // if the layer is in the layermask
            Debug.Log("a weirdness has happened!");
    }

    public void GetLayerArray(LayerMask mask, out bool[] layersInMask)
    {
        layersInMask = new bool[31];
        for (int i=0;i < 31; i++)
        {
            if ((mask & (1 << i)) != 0)                               // if layer i is in mask
                layersInMask[i] = true;

            else
                layersInMask[i] = false;

        }
    }

    private void Disable()
    {
                GetLayerArray(mask, out layersToDisable1);                                           // assign all layers in mask as true in [layersToDisable]
        for (int currLayer = 0; currLayer < layersToDisable1.Length; currLayer++)                   // iterate over layers
            if (layersToDisable1[currLayer])
            {
                Physics2D.IgnoreLayerCollision(currLayer, thisLayer);
                Debug.Log(string.Format("disabled collisions between [{0}] and [{1}]!", LayerMask.LayerToName(currLayer), LayerMask.LayerToName(thisLayer)));
            }
    }

}
