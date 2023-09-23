using System;
using System.Collections;
using System.Collections.Generic;
using NovelGame;
using NovelGame.Log;
using NovelGame.Net.Protocol;
using NovelGame.ResourceManagement;

namespace NovelGame.ResourceManagement{
public class ResourceUpdaterWithNetworking
{
    //LogをLoadしてSaveする。
    public void UpdatePlayLog(Player p){
        List<PlayLogRecord> backup = new List<PlayLogRecord>(p.playLog.records);
        try{
            //PlayerLogAtServerインスタンスを生成する
            //1. ログをDownloadする
            //ネットワークエラーが発生した場合、PlayerPrefsからロードしたログ＋今プレイで溜まったログをPlayerPrefsにセーブ
            PlayLog tempPlayLog = this._LoadPlayLog(p);
            //Player pのplayLogとDownloadしてログを連結したPlayLogを新たにテンポラリで作る
            //p.PlayLog.recordsはload_player命令でPlayerPrefsからロードしたログ＋今プレイで溜まったログ
            //それをサーバのログに連結。
            tempPlayLog.records.AddRange(p.playLog.records);
            //2. そのログをSaveする
            new PlayerLogAtServer(p.id).Save(tempPlayLog);
            p.playLog.records.Clear();//PlayerPrefsからロードしたログをクリア
            //成功した場合、PlayPrefsをクリアする。
            p.playLog.Save();//①PlayerPrefsに溜まったログをクリア(recordsをクリアしてそれで上書きするので、クリア動作になる)
            //万一、Saveでエラーした場合、サーバにはデータが保存完了なので、ゲームがエラー終了しても問題なし。
            p.playLog.records.AddRange(backup);
        }catch(HttpClientException e){
            MyDebug.Log($"ERROR: HttpClientException@UpdatePlayLog {e.Message}");
            //失敗した場合、PlayerPrefsからロードしたログ＋今プレイで溜まったログをPlayerPrefsにセーブ
            p.playLog.Save();//②
        }finally{
            //①か②で失敗した場合のリカバリルート
            //もし、p.playLog.records.Count == 0の場合の判定が重要
            //これにより、Save前のクリアで失敗した場合のリカバリになる。
            if(p.playLog.records.Count == 0){
                p.playLog.records.AddRange(backup);
            }
        }
    }

    protected PlayLog _LoadPlayLog(Player p){
        return new PlayerLogAtServer(p.id).Load();
    }

    public PlayLog LoadPlayLog(Player p){
        try{
            return _LoadPlayLog(p);
        }catch(Exception e){
            MyDebug.Log($"ERROR: HttpClientException@LoadPlayLog {e.Message}");
            PlayLog plog = new PlayLog(p.id);
            return plog;
        }
    }
}
}