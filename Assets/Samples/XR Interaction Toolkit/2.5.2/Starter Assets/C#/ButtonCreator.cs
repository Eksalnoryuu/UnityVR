using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Dummiesman;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
public class ButtonCreator : MonoBehaviour
{
    public Canvas canvas;
    public GameObject buttonPrefab;
    public List<string> buttonTexts;
    private FileReceiver _fileReceiver;
    private PublisherRequestFile _publisher;
    private string _fileContent;
    private bool _isRendered;
    private byte[] _fileBytes;
    public GameObject _gameObject;
    private Stopwatch _stopWatchFileReceiver;
    private Stopwatch _stopwatchFileRender;
    void Start()
    {
        _isRendered = false;
        buttonTexts.Clear();
        _stopWatchFileReceiver = new Stopwatch();
        _stopwatchFileRender = new Stopwatch();
        _fileReceiver = new FileReceiver(_stopWatchFileReceiver);
    }

    public void CreateButtons(List<string> stringList)
    {
        buttonTexts = stringList;
        Debug.Log(buttonTexts.Count);
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        float totalHeight = buttonTexts.Count * 1f;

        for (int i = 0; i < buttonTexts.Count; i++)
        {
            GameObject button = Instantiate(buttonPrefab, canvas.transform);

            Text buttonText = button.GetComponentInChildren<Text>(true);

            buttonText.text = buttonTexts[i];

            float xPosition = canvasRect.rect.width / 2f;
            float yPosition = canvasRect.rect.height / 2f + totalHeight / 2f - i * 1f;

            button.transform.position = new Vector3(xPosition, yPosition, 12.61f);

            int index = i;
            button.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(index));
        }
    }

    void OnButtonClick(int index)
    {
        _stopWatchFileReceiver.Reset();
        _stopwatchFileRender.Reset();
        _stopWatchFileReceiver.Start();
        _fileContent = null;
        _isRendered = false;
        AsyncIO.ForceDotNet.Force();
        _publisher = new PublisherRequestFile();
        _publisher.Start(buttonTexts[index]);
        _fileReceiver.Start((string data) => OnReceivedFile(data), buttonTexts[index]);
    }

    void OnReceivedFile(string file)
    {
        _fileContent = file;
        _publisher.Stop();
        _fileReceiver.Stop();
        _fileReceiver = null;
    }

    void Update()
    {
        if (_fileContent != null)
        {
            if (_fileContent.Length > 0)
            {
                if (!_isRendered)
                {
                    Debug.Log($"[UNITY] RENDERING OBJ FILE");
                    _fileBytes = Encoding.UTF8.GetBytes(_fileContent);
                    using (MemoryStream memoryStream = new MemoryStream(_fileBytes))
                    {
                        _stopwatchFileRender.Start();
                        _gameObject = new OBJLoader().Load(memoryStream);
                        _stopwatchFileRender.Stop();
                        Debug.Log($"[UNITY] Time to render file time:{_stopwatchFileRender.Elapsed} + milliseconds:{_stopwatchFileRender.ElapsedMilliseconds}");
                    }
                    _isRendered = true;
                }
            }
        }
    }
}
