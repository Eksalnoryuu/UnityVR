using NetMQ;
using NetMQ.Sockets;
using System;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;
public class ObjectReceiver
{
    private readonly Thread receiveThread;
    private bool running;
    private Action<AllNamesData> receiveCallback;
    public ObjectReceiver()
    {
        receiveThread = new Thread(() =>
        {
            using (var socket = new SubscriberSocket())
            {
                socket.Connect("tcp://127.0.0.1:9091");
                socket.SubscribeToAnyTopic();
                while (running)
                {
                    var message = new NetMQMessage();
                    if (socket.TryReceiveMultipartMessage(ref message))
                    {
                        var Topic = message.Pop().ConvertToString();
                        if(Topic == "RESPONSE_ALL_NAMES")
                        {
                            Debug.Log("[UNITY] Received RESPONSE_ALL_NAMES");
                            var content = message.Pop().ConvertToString();
                            var data = JsonConvert.DeserializeObject<AllNamesData>(content);
                            socket.Close();
                            receiveCallback.Invoke(data);
                        }
                    }
                    Debug.Log("[UNITY] Try to receive RESPONSE_ALL_NAMES!");
                    Thread.Sleep(100);
                }
            }
        });
    }

    public void Start(Action<AllNamesData> callback)
    {
        Debug.Log("[UNITY] Starting RESPONSE_ALL_NAMES Thread");
        running = true;
        receiveCallback = callback;
        receiveThread.Start();
    }

    public void Stop()
    {
        Debug.Log("[UNITY] Stop RESPONSE_ALL_NAMES Thread");
        running = false;
        receiveThread.Join();
        receiveThread.Abort();
    }

}