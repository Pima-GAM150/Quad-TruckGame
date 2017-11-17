#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct PushColor
{
    public Color Info;
    public Color Warning;
    public Color Error;
    public Color Debug;
    public Color Success;
    public Color Failed;
}

public class PushNotification : SerializedMonoBehaviour
{
    public static PushNotification Instance { get; private set; }

    [OdinSerialize]
    public Dictionary<string, Color> Colors = new Dictionary<string, Color>()
    {
        { "Info", Color.white },
        { "Warning", Color.yellow },
        { "Error", Color.red },
        { "Debug", Color.grey },
        { "Success", Color.green },
        { "Failed", Color.red },
    };

    [BoxGroup("Setup")]
    public GameObject PushNotificationPrefab;

    [BoxGroup("Setup")]
    public int Duration = 2000;

    private readonly Queue<Tuple<GameObject, int>> _activeQueue = new Queue<Tuple<GameObject, int>>();
    private ScrollRect _scrollRect;
    private bool _queueActive;

    public void Push(string msg, string type, float size = 12f)
    {
        if (!Colors.ContainsKey(type))
            return;
        var obj = Instantiate(PushNotificationPrefab, _scrollRect.content);
        var text = obj.GetComponent<TMP_Text>();
        text.fontSize = size;
        text.color = GetColor(type);
        text.text = msg;
        _activeQueue.Enqueue(new Tuple<GameObject, int>(obj, Mathf.RoundToInt(Time.time) + Duration));

        if (!_queueActive)
            StartQueue();

        //Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    private void Awake()
    {
        if (transform.parent.GetComponent<Canvas>())
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(transform.parent);
            }
            else
                Destroy(this);
        }
    }

    private Color GetColor(string type)
    {
        return Colors[type];
    }

    private void Start()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    private async Task StartQueue()
    {
        if (_queueActive)
            return;

        _queueActive = true;

        while (_activeQueue.Count > 0)
        {
            var obj = _activeQueue.Dequeue();

            if (Time.time > obj.Item2)
                Destroy(obj.Item1);
            else
            {
                await Task.Delay(Mathf.RoundToInt(obj.Item2 - Time.time));

                // this is for the sake of not being spammed with errors after ending play
                if (Application.isPlaying)
                    Destroy(obj.Item1);
            }
        }
        _queueActive = false;
    }
}