using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

/**
https://tech.mof-mof.co.jp/blog/unity-http-request/
UnityWebRequestを使うのが一番良いらしい。
*/

namespace NovelGame.Net.Protocol{


    ///https://qiita.com/Teach/items/2fa2b4fa4334a0a3e34d
    /// /// <summary>
    /// This class allows us to start Coroutines from non-Monobehaviour scripts
    /// Create a GameObject it will use to launch the coroutine on
    /// </summary>
    /**
このクラスはシーン(ゲーム本体)から使うとうまく動作するが、TestRunnerからだとうまく動作しないため、
都合が悪い
*/
    public class CoroutineHandler : MonoBehaviour
{
    static protected CoroutineHandler m_Instance;
    static public CoroutineHandler instance
    {
        get
        {
            if(m_Instance == null)
            {
                GameObject o = new GameObject("CoroutineHandler");
                DontDestroyOnLoad(o);
                Debug.Log("[CALLED] Create Game Object");
                m_Instance = o.AddComponent<CoroutineHandler>();
            }
            Debug.Log("[CALLED] CoroutineHandler instance");
            return m_Instance;
        }
    }

    public void OnDisable()
    {
        if(m_Instance)
            Destroy(m_Instance.gameObject);
    }

    static public Coroutine StartStaticCoroutine(IEnumerator coroutine)
    {
        Debug.Log("[CALLED] StartStaticCoroutine");
        return instance.StartCoroutine(coroutine);
    }
}

public class HttpClientException : System.Exception
{
    public HttpClientException() { }
    public HttpClientException(string message) : base(message) { }
    public HttpClientException(string message, System.Exception inner) : base(message, inner) { }
    protected HttpClientException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

/**
注意：このクラスのインスタンスを複数スレッド/コルーチンから使わないこと。
*/
public class HttpClient{

//    private bool done_flg = false;
    public string buf_string = null;
    public byte[] buf_byte = null;

    public void Download(string url) {
        Debug.Log("[START] Download");
        IEnumerator ie = this._Download(url);
        while(ie.MoveNext()){}
        Debug.Log("[END] Download");
    }

    public void Upload(string url, string content) {
        Debug.Log("[START] Upload");
        IEnumerator ie = this._Upload(url, content);
        while(ie.MoveNext()){}
        Debug.Log("[END] Upload");
    }

    protected IEnumerator _Download(string url) {
        Debug.Log("[START] _DownloadText=" + url);
//        this.done_flg = false;
        Debug.Log("_DownloadText:UnityWebRequest.Get(url)-START");
        //UnityWebRequest www = UnityWebRequest.Get(url);
        UnityWebRequest www =  UnityWebRequest.Get(url);
        Debug.Log("_DownloadText:UnityWebRequest.Get(url)-END");
        yield return www.SendWebRequest();

        Debug.Log("wait for request");
        
        //リクエストが完了するまで待機
        while(!www.isDone){
            Debug.Log("waiting.... yield return 0");
            yield return 0;
        }
        
        Debug.Log("Request is done!");

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log("Request Error = " + www.result);
            Debug.Log(www.error);
            throw new HttpClientException();
        }
        else {
            // 結果をテキストで表示
            Debug.Log("download ok");
            Debug.Log(www.downloadHandler.text);
 
            // 結果をテキストで取得
            this.buf_string = www.downloadHandler.text;
            // または、バイナリデータとして結果を取得します。
            this.buf_byte = www.downloadHandler.data;
        }
        Debug.Log("[END] _DownloadText");
    }

    /*
    protected IEnumerator _Upload(string url, string content){
        // content文字列をバイト列として読み込む
        byte[] rawData = Encoding.UTF8.GetBytes(content);

        UnityWebRequest www = UnityWebRequest.Put(url,rawData);

        // サーバーからのレスポンスを受け取るためのダウンロードハンドラーを設定
        // PUTメソッド時には必須ではないが、一応、設定しておく。
        // こうすることで、PUTリクエスト時のサーバからの応答を受け取れる。
        www.downloadHandler = new DownloadHandlerBuffer();

        // リクエストを送信し、レスポンスを待つ
        yield return www.SendWebRequest();

        // エラーチェック
        //if (www.isNetworkError || www.isHttpError)//この形式は古い
        if( www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Upload Error: " + www.error);
        }
        else
        {
            Debug.Log("Upload OK!");
            Debug.Log("Server Response " + www.downloadHandler.text);
        }

        // リクエストを解放
        //www.Dispose();
    }*/
   
    protected IEnumerator _Upload(string url, string content){
        //byte[] myData = System.Text.Encoding.UTF8.GetBytes(content);
        byte[] myData = Encoding.UTF8.GetBytes(content);
        using (UnityWebRequest www = UnityWebRequest.Put(url, myData)){
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            Debug.Log("wait for request");

            //リクエストが完了するまで待機
            //パターン1
            /*
            while(!www.isDone){
                Debug.Log("waiting.... yield return 0");
                yield return 0;
            }*/
            //パターン2 https://qiita.com/yjiro0403/items/df4b6ba855db2a1cc41b
            while (!www.isDone);

            Debug.Log("Request is done!");

            switch(www.result){
                case UnityWebRequest.Result.InProgress:
                    Debug.Log("Upload InProgress!");
                break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Upload complete!");
                break;
                default: //その他のケースはすべてエラー
                    Debug.Log("Upload Error!");
                    Debug.Log(www.error);
                break;
            }
            /*
            if( www.result == UnityWebRequest.Result.InProgress){

            }

            if (www.result != UnityWebRequest.Result.Success){
                Debug.Log("Upload Error!");
                Debug.Log(www.error);
            }else{
                Debug.Log("Upload complete!");
            }
            */
        }
    }
    }

}


