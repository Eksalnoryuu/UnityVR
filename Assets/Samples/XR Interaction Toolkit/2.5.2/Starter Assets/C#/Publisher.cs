using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using UnityEngine;
public class Publisher
{
    private readonly Thread receiveThread;
    private bool running;
    public Publisher()
    {
        receiveThread = new Thread(() =>
        {
            using (var socket = new PublisherSocket())
            {
                socket.Connect("tcp://127.0.0.1:9090");
                while (running)
                {
                    var message = new NetMQMessage();
                    message.Append("REQUEST_ALL_NAMES");
                    socket.SendMultipartMessage(message);
                    Debug.Log("[UNITY] SENDING REQUEST_ALL_NAMES");
                    Thread.Sleep(1000);
                }
                receiveThread.Join();
            }
        });
    }

    public void Start()
    {
        Debug.Log("[UNITY] STARTING REQUEST_ALL_NAMES THREAD");
        running = true;
        receiveThread.Start();
    }

    public void Stop()
    {
        Debug.Log("[UNITY] STOP REQUEST_ALL_NAMES THREAD");
        running = false;
        receiveThread.Join();
    }
    public void OnApplicationQuit()
    {
        receiveThread.Abort();
    }

}