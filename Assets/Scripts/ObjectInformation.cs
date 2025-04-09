using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInformation
{
    public string ObjectType;
    public Vector3 Position;
    public float Distance;
    public UnityEngine.GameObject ObjectGameObject;

    public ObjectInformation(string objectType, Vector3 position, UnityEngine.GameObject objectGameObject)
    {
        ObjectGameObject = objectGameObject;
        ObjectType = objectType;
        Position = position;
    }
}
