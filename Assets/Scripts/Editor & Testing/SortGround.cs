using UnityEngine;
using System.Collections.Generic;

public class SortGround : MonoBehaviour {
    private Transform[] children;
    public string newName = "GroundTile";
    public bool debug;

    [ContextMenu("Organize Ground tiles in hierarchy")]
    public void OrganizeTiles()
    {
        updateChildrenArray();                          // get (and sort according to x value) the children of this
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetSiblingIndex(i);             // for each child, set its index in the editor by it's (already sorted) index in the children array
            children[i].name = (string.Format("{0} ({1:000})", newName, i));

        }
    }

    private void updateChildrenArray()
    {
        children = new Transform[transform.childCount];
        int z = 0;
        foreach( Transform t in transform)                  // get the children
        {
            children[z] = t;
            z++;
        }

        var xVals = new float[transform.childCount];        // an array containing the corresponding x coord of the child in the same index in the children array

        for (int i = 0; i < children.Length; i++)
            xVals[i] = children[i].position.x;              // set up the xVals array to correspond with the X of each child in children

        SelectionSort(children, xVals);                     // sort the array according to the xVals

    }
    private void SelectionSort(Object[] Array, float[] sortingArray)
    {
        // a simple Selection sort to sort the given array by the sortingArray's values (from smallest to largest)
        // this is far from optimal, but there shouldnt be many objects anyway so its not worth writing something else

        if (Array.Length != sortingArray.Length)
            throw new System.ArgumentException("the two arrays should have the same length!");

        float minVal;
        int minIndex;

        float tempVal;
        Object tempItem;
        for (int i=0; i< sortingArray.Length; i++)
        {                                               // iterate over index i and set the minimum value from all items from i onwards
            minVal = sortingArray[i];
            minIndex = i;

            for (int j = i;j<sortingArray.Length; j++)
            {                                           // iterate over sorting array to find smallest value from index i until end
                if (sortingArray[j] < minVal)
                {
                    minVal = sortingArray[j];
                    minIndex = j;
                }
            }
            // at this point, j is the index of the lowest valued item from index i to the end of the array
            // and i is the current index we're setting

            tempVal = sortingArray[i];                  // switch the items at i and j in both arrays
            tempItem = Array[i];
            sortingArray[i] = sortingArray[minIndex];
            Array[i] = Array[minIndex];
            sortingArray[minIndex] = tempVal;
            Array[minIndex] = tempItem;
        }
        if (debug == true)
        {
            Debug.Log("Debugging SelectionSort. sorted array: ");
            for (int i=0;i< sortingArray.Length; i++)
            {
                Debug.Log(string.Format("Array[{0}] = {1}", i, sortingArray[i]));
            }
        }
    }

}
