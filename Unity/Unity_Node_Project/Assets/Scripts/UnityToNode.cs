using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;

public class UnityToNode : MonoBehaviour
{
    public string host;                 //127.0.0.1 
    public int port;                    //3030

    public string idUrl;                //��� �ּ� ����
    public string postUrl;
    public string resDataUrl;
    public string startConstructionUrl;
    public string checkConstructionUrl;

    public Button btnGetExample;
    public Button btnPostExample;
    public Button btnResDataExample;
    public Button btnConstruction_Post;
    public Button btnConstruction_Get;

    public int id;
    public string data;

    private void Start()
    {
        this.btnPostExample.onClick.AddListener(() =>
        {
            var url = string.Format("{0}:{1}/{2}", host, port, postUrl);      //URL �ּ� ����
            Debug.Log(url);

            var req = new Protocols.Packets.req_data();                         //Req �������� ������ �Է�
            req.cmd = 1000;
            req.id = id;
            req.data = data;
            var json = JsonConvert.SerializeObject(req);
            Debug.Log(json);

            StartCoroutine(this.PostData(url, json, (raw) =>
            {
                Protocols.Packets.common res = JsonConvert.DeserializeObject< Protocols.Packets.common>(raw);
                Debug.LogFormat("{0}, {1}", res.cmd, res.message);         
            }));

        });

        this.btnGetExample.onClick.AddListener(() =>
        {
            var url = string.Format("{0}:{1}/{2}", host, port, idUrl);      //URL �ּ� ����
            Debug.Log(url);

            StartCoroutine(this.GetData(url, (raw) =>
            {
                var res = JsonConvert.DeserializeObject<Protocols.Packets.common>(raw);       //JSON ��ȯ

                Debug.LogFormat("{0}, {1}", res.cmd, res.message);          //����׷α׷� �������� �����ذ� Ȯ��
            }));

        });

        this.btnResDataExample.onClick.AddListener(() =>
        {
            var url = string.Format("{0}:{1}/{2}", host, port, resDataUrl);      //URL �ּ� ����
            Debug.Log(url);

            StartCoroutine(this.GetData(url, (raw) =>
            {
                var res = JsonConvert.DeserializeObject<Protocols.Packets.res_data>(raw);       //JSON ��ȯ

                foreach(var user in res.result)
                {
                    Debug.LogFormat("{0}, {1}", user.id, user.data);          //����׷α׷� �������� �����ذ� Ȯ��
                }
                
            }));
        });

        this.btnConstruction_Post.onClick.AddListener(() =>             //�Ǽ� ���� POST ��� 
        {
            var url = string.Format("{0}:{1}/{2}", host, port, startConstructionUrl);      //URL �ּ� ����
            Debug.Log(url);

            var req = new Protocols.Packets.req_data();                                 //���������� ������ش�. 
            req.cmd = 1000;
            req.id = id;
            req.data = data;
            var json = JsonConvert.SerializeObject(req);
            Debug.Log(json);

            StartCoroutine(this.PostData(url, json, (raw) =>
            {
                Protocols.Packets.common res = JsonConvert.DeserializeObject<Protocols.Packets.common>(raw);
                Debug.Log(res);
            }));
        });

        this.btnConstruction_Get.onClick.AddListener(() =>              //�Ǽ� Ȯ�� GET ���
        {
            var url = string.Format("{0}:{1}/{2}", host, port, checkConstructionUrl);      //URL �ּ� ����
            Debug.Log(url);

            StartCoroutine(this.GetData(url, (raw) =>
            {
                var res = JsonConvert.DeserializeObject<Protocols.Packets.common>(raw);
                Debug.LogFormat("{0}, {1}", res.cmd, res.message);
            }));
        });
    }

    private IEnumerator GetData(string url, System.Action<string> callback)
    {
        var webRequest = UnityWebRequest.Get(url);                //����Ƽ �Լ� UnityWebRequest�� Get
        yield return webRequest.SendWebRequest();                   //����� ���ƿö� ���� �ڷ�ƾ ���

        Debug.Log("-->" + webRequest.downloadHandler.text);

        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||   //Ŀ�ؼ� Error �̰ų�
            webRequest.result == UnityWebRequest.Result.ProtocolError)      //�������� Error �ϰ��
        {
            Debug.Log("���� ��� ����");
        }
        else  //������ ���� ���
        {
            callback(webRequest.downloadHandler.text);
        }
    }

    private IEnumerator PostData(string url, string json, System.Action<string> callback)
    {
        var webRequest = new UnityWebRequest(url , "POST");
        var bodyRaw = Encoding.UTF8.GetBytes(json);     //json ���ڵ� 

        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||   //Ŀ�ؼ� Error �̰ų�
           webRequest.result == UnityWebRequest.Result.ProtocolError)      //�������� Error �ϰ��
        {
            Debug.Log("���� ��� ����");
        }
        else  //������ ���� ���
        {
            Debug.LogFormat("{0}\n{1}\n{2}", webRequest.responseCode, webRequest.downloadHandler.data, webRequest.downloadHandler.text);
            callback(webRequest.downloadHandler.text);
        }

        webRequest.Dispose();       //���� ���� (������ �޸� ����)
    }


}
