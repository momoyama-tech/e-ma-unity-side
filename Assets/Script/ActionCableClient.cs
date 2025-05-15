using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class ActionCableClient : MonoBehaviour
{
    [Header("ActionCable �ݒ�")]
    public string roomId = "room_channel";
    public string channelName = "RoomChannel";

    private WebSocket _ws;
    private Queue<string> _flowerUrlQueue = new Queue<string>();

    void Start()
    {
        // WebSocket URL�iEditor/�{�� �؂蕪���j
#if UNITY_EDITOR
        string wsUrl = "wss://e-ma-rails-staging-986464278422.asia-northeast1.run.app/cable";
#else
        string wsUrl = "wss://your-production-url.com/cable";
#endif
        _ws = new WebSocket(wsUrl);

        _ws.OnOpen += (s, e) => SubscribeToChannel();
        _ws.OnMessage += (sender, e) =>
        {
            // ��M�f�[�^���܂����O�o�́i�f�o�b�O�p�j
            Debug.Log($"[WebSocket] Received: {e.Data}");

            try
            {
                // JSON �p�[�X
                var msg = JObject.Parse(e.Data);
                var type = (string)msg["type"] ?? "";

                // ping �� confirm_subscription �͖���
                if (type == "ping" || type == "confirm_subscription")
                    return;

                // illustration URL �̑��݃`�F�b�N
                var illustrationToken = msg["message"]?["data"]?["urls"]?["illustration"];
                if (illustrationToken == null)
                {
                    Debug.LogWarning("[WebSocket] illustration URL ��������܂���ł����B");
                    return;
                }

                // < �� > ����菜��
                string rawUrl = illustrationToken.ToString().Trim();
                string cleanUrl = rawUrl.TrimStart('<').TrimEnd('>');

                // �L���[�� enqueue
                lock (_flowerUrlQueue)
                {
                    _flowerUrlQueue.Enqueue(cleanUrl);
                }
                Debug.Log($"[WebSocket] Enqueued cleaned URL: {cleanUrl}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[WebSocket] OnMessage �������̗�O: {ex}\nRaw data: {e.Data}");
            }
        };
        _ws.OnError += (s, e) => Debug.LogError("WebSocket Error: " + e.Message);
        _ws.OnClose += (s, e) => Debug.Log("WebSocket Closed");

        _ws.Connect();
    }

    private void SubscribeToChannel()
    {
        // { command: "subscribe", identifier: "{\"channel\":\"RoomChannel\",\"room_id\":\"room_channel\"}" }
        string identifier = $"{{\"channel\":\"{channelName}\",\"room_id\":\"{roomId}\"}}";
        string msg = $"{{\"command\":\"subscribe\",\"identifier\":\"{identifier.Replace("\"", "\\\"")}\"}}";
        _ws.Send(msg);
        Debug.Log("Subscribed to " + channelName);
    }

    private void OnDestroy()
    {
        if (_ws != null)
        {
            // unsubscribe
            string identifier = $"{{\"channel\":\"{channelName}\",\"room_id\":\"{roomId}\"}}";
            string msg = $"{{\"command\":\"unsubscribe\",\"identifier\":\"{identifier.Replace("\"", "\\\"")}\"}}";
            _ws.Send(msg);

            _ws.Close();
            _ws = null;
        }
    }

    /// <summary>
    /// �L���[�Ɏc���Ă��� URL ������� true
    /// </summary>
    public bool HasFlowerUrl()
    {
        lock (_flowerUrlQueue)
        {
            return _flowerUrlQueue.Count > 0;
        }
    }

    /// <summary>
    /// �L���[���玟�� URL �����o���i�Ȃ���� null�j
    /// </summary>
    public string DequeueFlowerUrl()
    {
        lock (_flowerUrlQueue)
        {
            return _flowerUrlQueue.Count > 0 ? _flowerUrlQueue.Dequeue() : null;
        }
    }
}