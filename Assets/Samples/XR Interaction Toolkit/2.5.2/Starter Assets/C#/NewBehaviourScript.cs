using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Dummiesman;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class NewBehaviourScript : MonoBehaviour
{

    public GameObject buttonPrefab;
    public List<string> buttonTexts;
    private FileReceiver _fileReceiver;
    private PublisherRequestFile _publisher;
    private string _fileContent;
    private bool _isRendered;
    private byte[] _fileBytes;
    public GameObject _gameObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClick()
    {

        _gameObject = new OBJLoader().Load(@"C:\Users\OEM\Downloads\all-bones3.obj");
    }
}
