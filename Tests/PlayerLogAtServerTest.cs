using System.Threading;
using NovelGame;
using NovelGame.Log;
using NovelGame.ResourceManagement;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert; // AssertはNUnitのではなくUnityのものを使う

namespace Tests{
public class PlayerLogAtServerTest
{
    public string player_id = "testplayer";
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

    protected void PlayerLogAtServerShouldSaveAndLoadPlayerLogsFromServer_num(int playLogCount = 0){
        PlayLog playLog = this.GenerateTestLog(this.player_id, playLogCount);
        PlayerLogAtServer plas = new PlayerLogAtServer(this.player_id);

        //念の為のチェック。
        Assert.AreEqual(playLogCount >= 0, true);
        //生成したテストデータの長さを念の為チェック。
        Assert.AreEqual(playLog.records.Count, playLogCount);

        //一回Uploadする
        plas.Save(playLog);

        //それが正しくダウンロードされるかを試験
        PlayLog target = plas.Load();
        Assert.AreEqual(target.records.Count, playLog.records.Count);
        Assert.AreEqual(target.playerId, playLog.playerId);

        //データのチェック
        for(int i = 0; i < playLogCount ; i ++){
            string chkTgt = target.records[i].targetQuestion;
            MyDebug.Log($"PlayerLogAtServerShouldSaveAndLoadPlayerLogsFromServer [{i}]=={chkTgt}");
            Assert.AreEqual(chkTgt, playLog.records[i].targetQuestion);
        }
    }

    [Test]
    public void PlayerLogAtServerShouldSaveAndLoadPlayerLogsToServer(){
        this.PlayerLogAtServerShouldSaveAndLoadPlayerLogsFromServer_num(0);
        this.PlayerLogAtServerShouldSaveAndLoadPlayerLogsFromServer_num(1);
        this.PlayerLogAtServerShouldSaveAndLoadPlayerLogsFromServer_num(2);
        this.PlayerLogAtServerShouldSaveAndLoadPlayerLogsFromServer_num(10);
    }
    [Test]
    public void PlayerLogAtServerShouldSaveAndLoadPlayerLogsToServerHuge(){
        //100万件だと、UnitTest時にUnityEditerまるごとダウン。
        //this.PlayerLogAtServerShouldSaveAndLoadPlayerLogsFromServer_num(1000000);
        //個人プレイだと1万件ですごいレベルなので、コレくらいに抑えておく。
        this.PlayerLogAtServerShouldSaveAndLoadPlayerLogsFromServer_num(10000);
    }


}
}