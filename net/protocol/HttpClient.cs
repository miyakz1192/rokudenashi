using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

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
 
    public IEnumerator _Download(string url) {
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

}



}
