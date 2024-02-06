using NetMQ.Sockets;
using UnityEngine;
using AsyncIO;
using NetMQ;
using System.Collections.Generic;
using System.Linq;

public class ZmqClient : MonoBehaviour
{
    private ObjectReceiver objectReceiver;
    private Publisher publisher;
    public ButtonCreator buttonCreator;
    private List<string> ListOfObjectNames;
    void Start()
    {
        ForceDotNet.Force();
        publisher = new Publisher();
        publisher.Start();
        objectReceiver = new ObjectReceiver();
        objectReceiver.Start((AllNamesData d) => OnReceiveFiles(d));
    }
    public void Update()
    {
        if (ListOfObjectNames != null)
        {
            if (ListOfObjectNames.Count > 0)
            {
                var newList = ListOfObjectNames.ToList();
                buttonCreator.CreateButtons(newList);
                ListOfObjectNames.Clear();
            }
        }
    }

    private void OnDestroy()
    {
        objectReceiver.Stop();
        publisher.Stop();
        NetMQConfig.Cleanup();
    }
    private void OnApplicationQuit()
    {
        publisher.OnApplicationQuit();
    }
    private void OnReceiveFiles(AllNamesData data)
    {
        ListOfObjectNames = data.content;
        publisher.Stop();

    }
}
