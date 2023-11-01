using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;

[System.Serializable]
public class BagRequest
{
    public string user_id;
    public string item_name;
}

public class BagManager : MonoBehaviour
{
    //�׽�Ʈ�� UI ����
    public Text bagContentsText;
    public InputField addItemInputField;
    public InputField removeItemInputField;

    public string user_id = "";

    private const string serverURL = "http://127.0.0.1:3000/";

    public void GetBagContents()
    {
        StartCoroutine(GetBagContentsCoroutine(user_id));
    }

    IEnumerator GetBagContentsCoroutine(string user_id)
    {
        Debug.Log(serverURL + "bag?user_id=" + user_id);
        UnityWebRequest webRequest = UnityWebRequest.Get(serverURL + "bag?user_id=" + user_id);
        yield return webRequest.SendWebRequest();

        if(webRequest.result == UnityWebRequest.Result.ConnectionError ||
           webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("���� ��û ���� : " + webRequest.error);
        }
        else
        {
            string jsonResponse = webRequest.downloadHandler.text;
            bagContentsText.text = jsonResponse;
        }
    }
    public void AddItemToBag()
    {
        BagRequest request = new BagRequest
        {
            user_id = user_id,
            item_name = addItemInputField.text
        };       
        string jsonData = JsonConvert.SerializeObject(request);     //����ȭ
        StartCoroutine(AddItemToBagCoroutine(jsonData));
    }

    IEnumerator AddItemToBagCoroutine(string jsonData)
    {        
        UnityWebRequest webRequest = new UnityWebRequest(serverURL + "bag/add" , "POST");
        webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
           webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("���� ��û ���� : " + webRequest.error);
        }
        else
        {
            string jsonResponse = webRequest.downloadHandler.text;
            bagContentsText.text = jsonResponse;
        }
        webRequest.Dispose();
    }

}
