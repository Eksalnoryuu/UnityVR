using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using UnityEngine;
public class PublisherRequestFile
{
    private readonly Thread _receiveThread;
    private bool _running;
    private string _fileName;
    public PublisherRequestFile()
    {
        _receiveThread = new Thread(() =>
        {
            using (var socket = new PublisherSocket())
            {
                socket.Connect("tcp://127.0.0.1:9090");
                while (_running)
                {
                    Debug.Log(_running);
                    var message = new NetMQMessage();
                    message.Append("REQUEST_FILE");
                    message.Append(_fileName);
                    socket.SendMultipartMessage(message);
                    Debug.Log("[UNITY] SENDING REQUEST_FILE");
                    Thread.Sleep(2000);
                }
                _receiveThread.Join();
            }
        });
    }

    public void Start(string fileName)
    {
        Debug.Log("[UNITY] STARTING REQUEST_FILE THREAD");
        _fileName = fileName;
        _running = true;
        _receiveThread.Start();
    }

    public void Stop()
    {
        Debug.Log("[UNITY] STOP REQUEST_FILE THREAD");
        _running = false;
        _receiveThread.Abort();
    }
    public void OnApplicationQuit()
    {
        _receiveThread.Abort();
    }

}