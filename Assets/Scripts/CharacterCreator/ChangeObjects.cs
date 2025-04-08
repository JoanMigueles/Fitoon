using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeObjects : MonoBehaviour
{
    GameObject[] selectedObjects;

    //cuando se pulsa el zapato en cuestion, cambiar la malla actual por esa
    public void ChangeObject(ObjectItem objectItem)
    {
        selectedObjects = GameObject.FindGameObjectsWithTag("Shoes");
        foreach (GameObject obj in selectedObjects)
        {
            SkinnedMeshRenderer renderer = obj.GetComponent<SkinnedMeshRenderer>();
            renderer.sharedMesh = ShoeLoader.GetMesh(objectItem.mesh);
            renderer.materials = ShoeLoader.getMaterials(objectItem.materials);
            obj.GetComponent<WhatShoeIHave>().myShoe = objectItem;
        }
    }

}
