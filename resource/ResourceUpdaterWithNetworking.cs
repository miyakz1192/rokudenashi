using NovelGame;
using NovelGame.Log;
using NovelGame.Net.Protocol;
using NovelGame.ResourceManagement;

namespace NovelGame.ResourceManagement{
public class ResourceUpdaterWithNetworking
{
    //LogをLoadしてSaveする。
    public void UpdatePlayLog(Player p){
        //PlayerLogAtServerインスタンスを生成する
        //1. ログをDownloadする
        //ネットワークエラーが発生した場合、Update失敗する。(HttpClientExceptioを送出)
        PlayLog tempPlayLog = this.LoadPlayLog(p);
        //Player pのplayLogとDownloadしてログを連結したPlayLogを新たにテンポラリで作る
        //p.PlayLog.recordsはload_player命令でPlayerPrefsからロードしたログ＋今プレイで溜まったログ
        //それをサーバのログに連結。
        tempPlayLog.records.AddRange(p.playLog.records);
        //2. そのログをSaveする
        try{
            new PlayerLogAtServer(p.id).Save(tempPlayLog);
            p.playLog.records.Clear();//PlayerPrefsからロードしたログ＋今プレイで溜まったログをクリア
            //成功した場合、PlayPrefsをクリアする。
            p.playLog.Save();//PlayerPrefsに溜まったログをクリア
        }catch(HttpClientException e){
            MyDebug.Log($"ERROR: HttpClientException@UpdatePlayLog {e.Message}");
            //失敗した場合、PlayerPrefsからロードしたログ＋今プレイで溜まったログをPlayerPrefsにセーブ
            p.playLog.Save();//PlayerPrefsに溜まったログをクリア
        }
    }

    public PlayLog LoadPlayLog(Player p){
        return new PlayerLogAtServer(p.id).Load();
    }
}
}