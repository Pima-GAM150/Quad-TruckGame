using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CargoController : SerializedMonoBehaviour
{
    public static CargoController Instance;
    public Dictionary<string, GameObject> Cargo;
    public Transform SpawnLocation;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
}