using NovelGame;
using NovelGame.Log;
using NovelGame.ResourceManagement;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert; // AssertはNUnitのではなくUnityのものを使う

namespace Tests{
public class PlayerLogAtServerTest
{
    public string player_id = "souchan";
    protected void ShowPlayerLog(string player_id){
        //プレイヤーログを表示する（サンプル)
        PlayLog playLog = PlayLog.Load(player_id);
        foreach(PlayLogRecord prec in playLog.records){
            MyDebug.Log(JsonUtility.ToJson(prec));
        }
    }
    /*本番環境に移行するときに、ローカルのPlayerLog(PlayerPref)をサーバにアップするスクリプト。
    　アップ先はUnitTestのサーバになる。
    　この関数だけを呼び出すUnitTestを、このUnitTestだけ選択し(他のUnitTestは選択しない)て
    　実行することで、ローカルのPlayerLogをサーバにアップできる。
    　あとは、アップされたファイルを本番環境のサーバにコピーすれば移行は完了。

    　その後、二度とこの関数を実行しないようにするべき。（なんか合ったときのため、
    　コードを残すのは良いが、コメントアウトして呼び出さないようにするべきだ）
    */
    protected void UploadNowPlayerLogInPlayerPrefToServer_OnlyOnce(string player_id){
        PlayLog playLog = PlayLog.Load(player_id);
    }

    /**
    UnitTest用のテスト用PlayLogを生成する
    */
    protected PlayLog GenerateTestLog(string player_id, int numOfTestItems){
        PlayLog playLog = new PlayLog(player_id);
        for(int i = 0; i < numOfTestItems; i ++){
            PlayLogRecord rec = new PlayLogRecord(targetQuestion: "UnitTest_TestNo=" + i.ToString());
            playLog.records.Add(rec);
        }
        return playLog;
    }

    [Test]
    public void PlayerLogAtServerShouldLoadPlayerLogsFromServer_0recs(){
        PlayLog playLog = this.GenerateTestLog(this.player_id, 0);
        PlayerLogAtServer plas = new PlayerLogAtServer(this.player_id);

        //一回Uploadする
        plas.Save(playLog);

        //それが正しくダウンロードされるかを試験
        PlayLog target = plas.Load();
        Assert.AreEqual(target.records.Count, playLog.records.Count);
        Assert.AreEqual(target.playerId, playLog.playerId);
    }

    [Test]
    public void PlayerLogAtServerShouldSavePlayerLogsToServer(){
        
    }

}
}