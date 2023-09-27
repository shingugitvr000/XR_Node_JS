using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public InputField usernameInput;        //���� �ʵ�
    public InputField passwordInput;        //�н����� �ʵ�
    public Text loginStatuText;

    public string token;

    private const string apiUrl = "http://localhost:3000"; //Node.js �ּ�

    public void Login()
    {
        StartCoroutine(AttemptLogin(usernameInput.text, passwordInput.text));
    }
    IEnumerator AttemptLogin(string username , string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(apiUrl + "/login", form))
        {
            yield return webRequest.SendWebRequest();

            if(webRequest.result != UnityWebRequest.Result.Success)
            {
                loginStatuText.text = "Login failed : " + webRequest.error;
            }
            else
            {
                loginStatuText.text = "Login successful!";
                string responseText = webRequest.downloadHandler.text;
                //JSON ���信�� ��ū ���� ����
                var responseData = JsonConvert.DeserializeObject<ResponseData>(responseText);
                token = responseData.token;

                Debug.Log("Login successful! Token : " + token);
            }
        }
    }

    [System.Serializable]
    private class ResponseData
    {
        public string token;
    }
}
