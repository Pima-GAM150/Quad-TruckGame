using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UISwitchGroup : MonoBehaviour
{
    public string InitialSwitchableName = "";

    private Dictionary<string, GameObject> _switchables = new Dictionary<string, GameObject>();

    public void AddSwitchable(string name, GameObject switchable)
    {
        if (_switchables.ContainsKey(name))
            return;

        _switchables.Add(name, switchable);
    }

    public void RemoveSwitchable(string name)
    {
        if (!_switchables.ContainsKey(name))
            return;

        _switchables.Remove(name);
    }

    public void SwitchTo(string name)
    {
        foreach (var obj in _switchables)
        {
            obj.Value.SetActive(obj.Key == name);
        }
    }

    public void SwitchTo(int index)
    {
        for (int i = 0; i < _switchables.Count; ++i)
        {
            _switchables.ElementAt(i).Value.SetActive(i == index);
        }
    }

    private void Awake()
    {
        foreach (Transform obj in transform)
        {
            _switchables.Add(obj.name, obj.gameObject);
        }
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(InitialSwitchableName))
        {
            if (_switchables.Count > 0)
            {
                var firstKey = _switchables.Keys.First();
                SwitchTo(firstKey);
            }
        }
        else
            SwitchTo(InitialSwitchableName);
    }
}