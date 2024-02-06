using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
public class FileReceiver
{
    private Thread _receiveThread;
    private bool _running;
    private Action<string> _receiveCallback;
    private string _fileName;
    private Stopwatch _stopWatch;
    public FileReceiver(Stopwatch stopWatch)
    {
        _stopWatch = stopWatch;
    }

    public void Start(Action<string> callback, string fileName)
    {
        Debug.Log("[UNITY] Starting RESPONSE_FILE Thread");
        SetThread();
        _running = true;
        _receiveCallback = callback;
        _receiveThread.Start();
        _fileName = fileName;
    }

    public void Stop()
    {
        Debug.Log("[UNITY] Stop RESPONSE_FILE Thread");
        _running = false;
        _receiveThread.Abort();
        _receiveThread = null;
    }

    private void SetThread()
    {
        _receiveThread = new Thread(() =>
        {
            using (var socket = new SubscriberSocket())
            {
                socket.Connect("tcp://127.0.0.1:9091");
                socket.SubscribeToAnyTopic();
                while (_running)
                {
                    var message = new NetMQMessage();
                    if (socket.TryReceiveMultipartMessage(ref message))
                    {
                        _stopWatch.Stop();
                        var Topic = message.Pop().ConvertToString();
                        if (Topic == "RESPONSE_FILE")
                        {
                            var fileName = message.Pop().ConvertToString();
                            if (fileName == _fileName)
                            {
                                Debug.Log($"[UNITY] Time to receive file time:{_stopWatch.Elapsed} + milliseconds:{_stopWatch.ElapsedMilliseconds}");
                                Debug.Log($"[UNITY] Received RESPONSE_FILE file:{_fileName}");
                                var content = message.Pop().ConvertToString();
                                socket.Close();
                                _receiveCallback.Invoke(content);
                            }
                        }
                    }
                    Debug.Log("Try To Recieve!");
                    Thread.Sleep(10);
                }
            }
        });
    }
}