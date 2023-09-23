using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert; // AssertはNUnitのではなくUnityのものを使う
using System;
using NovelGame.Net.Protocol;
using NovelGame;
using System.Linq;
using System.Collections.Generic;
using NovelGame.ResourceManagement;

namespace Tests{
public class ResourceUpdaterWithNetworkingTest
{
    protected int serialNum = 0;
    protected string playerId = "testplayer";
    //タイムアウトだと時間がかかるのでRSTを食らう形式で。1192なんてポートは普通使われないかな。
    protected string errrurl = "http://127.0.0.1:1192/";
    /**
        テストパターンの整理
        状態要素は以下で定義できる。
        　サーバのログ：サーバに溜まっているログの個数
        　PlayerPrefs：プレイ開始前にPlayerPrefsに保存されているPlayLogの個数
        　今プレイのログ：今回のプレイで追加される予定のPlayLog
        　　※PlayerPrefs+今プレイのログが、正常の場合は、全てサーバにたまり、ネットワークエラーの場合は、
        　　　PlayerPrefsに保存される。
        　　※プレイ開始時にPlayerPrefsがplayer.playLog.recordsに追加される。そのあと、そのリストに今プレイ
        　　　のログが追加されていくことに注意して、以下の表を見る必要がある。

        それぞれ組み合わせパターン
        [正常系]
        サーバのログ、PlayerPrefs、今プレイのログ →Update(OK)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             0             0                            0              0            0
        サーバのログ、PlayerPrefs、今プレイのログ →Update(OK)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             0             1                            1              0            1
        サーバのログ、PlayerPrefs、今プレイのログ →Update(OK)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             1             0                            1              0            1
//　　　以下、当初テストを予定していたが、LoadAllしたあとに、追加で足すような運用はないため、テストせず。
//      サーバのログ、PlayerPrefs、今プレイのログ →Update(OK)→　サーバのログ、PlayerPrefs、今プレイのログ
//            0             1             1                            2              0            2
        --------------------------------------------------------------------------------------------------
        サーバのログ、PlayerPrefs、今プレイのログ →Update(OK)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             0             2                            2              0            2
        サーバのログ、PlayerPrefs、今プレイのログ →Update(OK)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             2             0                            2              0            2

        [異常系]
        --------------------------------------------------------------------------------------------------
        サーバのログ、PlayerPrefs、今プレイのログ →Update(ERR)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             0             0                            0              0            0
        サーバのログ、PlayerPrefs、今プレイのログ →Update(ERR)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             0             1                            0              1            1
            　　　　　　　　　　　　　　　　　　　　　　　　　　　　↑今のプレイログがPlayerPrefsにSaveされるだけのため、
            　　　　　　　　　　　　　　　　　　　　　　　　　　　　　今のプレイログに残る。
        サーバのログ、PlayerPrefs、今プレイのログ →Update(ERR)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             1             0                            0              1            1
            　　　　　　　　　　　　　　　　　　　　　　　　　　　　↑PlayerPrefsから今のプレイログにロードされた今のプレイログがPlayerPrefsにSaveされるだけのため、
            　　　　　　　　　　　　　　　　　　　　　　　　　　　　　今のプレイログに残る。
    */
    [Test]
    public void TestNormal(){
        //正常系
        TestNormalOnePattern(initSvLog:0, initPlayerPrefs:0, initPlayLogs:0);
        TestNormalOnePattern(initSvLog:0, initPlayerPrefs:0, initPlayLogs:1);
        TestNormalOnePattern(initSvLog:0, initPlayerPrefs:1, initPlayLogs:0);
        
        int n = 2;
        TestNormalOnePattern(initSvLog:0, initPlayerPrefs:0, initPlayLogs:n);
        TestNormalOnePattern(initSvLog:0, initPlayerPrefs:n, initPlayLogs:0);
    }

    [Test]
    public void TestError(){
        //異常系
        TestErrorOnePattern(initSvLog:0, initPlayerPrefs:0, initPlayLogs:1);
        TestErrorOnePattern(initSvLog:0, initPlayerPrefs:1, initPlayLogs:0);
    }

    /**
    　正常パターンはPlayerPrefsと今プレイログの合算が、サーバログに行くので、そのパターンのテスト
    */
    protected void TestNormalOnePattern(int initSvLog, int initPlayerPrefs, int initPlayLogs){
        List<PlayLogRecord> svLogs = SetServerLog(initSvLog);
        List<PlayLogRecord> playerPrefs = SetPlayerPrefs(initPlayerPrefs);
        List<PlayLogRecord> playLogs = CreateNowPlayLog(initPlayLogs);
        List<PlayLogRecord> voidLogs = new List<PlayLogRecord>();
        List<PlayLogRecord> toBeServerStored = new List<PlayLogRecord>();

        //次の行はinitPlayerPrefsが0かそうでないかで分岐した方がよいのだが、
        //playerPrefsが0であれば、PlayLogsが1以上(逆は対称的)なので、分岐せず、この連結でよい。
        toBeServerStored = toBeServerStored.Concat(svLogs).Concat(playerPrefs).Concat(playLogs).ToList();
        
        Assert.AreEqual(svLogs.Count, initSvLog);
        Assert.AreEqual(playerPrefs.Count, initPlayerPrefs);
        Assert.AreEqual(playLogs.Count, initPlayLogs);

        //svLogs, playerPrefs, playLogsの準備ができたら、テスト用のPlayerを生成
        Player p = new Player{id=this.playerId};
        p.LoadAllData();//SetPlayerPrefsで作ったPlayLogデータを読み込むために、実行
        if(initPlayerPrefs == 0){//もし、playerPrefsが0でかつ、今のプレイで溜まったLogがある場合
            p.playLog.records.AddRange(playLogs);
        }

        new ResourceUpdaterWithNetworking().UpdatePlayLog(p);

        CheckServerLog(toBeServerStored);//サーバに保存されたログをロードして、内容を全チェック。長さがtempと合っているかも。
        CheckPlayerPrefs(voidLogs);
        if(initPlayerPrefs == 0){//もし、playerPrefsが0でかつ、今のプレイで溜まったLogがある場合
            CheckNowPlayerLogs(p, playLogs);
        }else{
            CheckNowPlayerLogs(p, playerPrefs);
        }
    }

    protected void TestErrorOnePattern(int initSvLog, int initPlayerPrefs, int initPlayLogs){
        List<PlayLogRecord> svLogs = SetServerLog(initSvLog);
        List<PlayLogRecord> playerPrefs = SetPlayerPrefs(initPlayerPrefs);
        List<PlayLogRecord> playLogs = CreateNowPlayLog(initPlayLogs);
        List<PlayLogRecord> voidLogs = new List<PlayLogRecord>();
        List<PlayLogRecord> toBeServerStored = new List<PlayLogRecord>();

        //次の行はinitPlayerPrefsが0かそうでないかで分岐した方がよいのだが、
        //playerPrefsが0であれば、PlayLogsが1以上(逆は対称的)なので、分岐せず、この連結でよい。
        toBeServerStored = toBeServerStored.Concat(svLogs).Concat(playerPrefs).Concat(playLogs).ToList();
        
        Assert.AreEqual(svLogs.Count, initSvLog);
        Assert.AreEqual(playerPrefs.Count, initPlayerPrefs);
        Assert.AreEqual(playLogs.Count, initPlayLogs);

        //svLogs, playerPrefs, playLogsの準備ができたら、テスト用のPlayerを生成
        Player p = new Player{id=this.playerId};
        p.LoadAllData();//SetPlayerPrefsで作ったPlayLogデータを読み込むために、実行
        if(initPlayerPrefs == 0){//もし、playerPrefsが0でかつ、今のプレイで溜まったLogがある場合
            p.playLog.records.AddRange(playLogs);
        }

        ResourceManagementProtocol.overrideRootUrl = this.errrurl;
        new ResourceUpdaterWithNetworking().UpdatePlayLog(p);
        ResourceManagementProtocol.overrideRootUrl = null;//エラー発生箇所を、ResourceUpdaterWithNetworkingのみに絞り込み

        CheckServerLog(voidLogs);//サーバに保存されたログをロードして、内容を全チェック。長さがtempと合っているかも。
        if(initPlayerPrefs == 0){//もし、playerPrefsが0でかつ、今のプレイで溜まったLogがある場合
            CheckPlayerPrefs(playLogs);
            CheckNowPlayerLogs(p, playLogs);
        }else{
            CheckPlayerPrefs(playerPrefs);
            CheckNowPlayerLogs(p, playerPrefs);
        }

    }

    protected List<PlayLogRecord> CreateLogRecords(int numOfLogs){
        List<PlayLogRecord> res = new List<PlayLogRecord>();
        for(int i = 0; i < numOfLogs; i ++){
            PlayLogRecord rec = new PlayLogRecord(targetQuestion: "UnitTest_TestNo=" + i.ToString());
            res.Add(rec);
        }
        return res;
    }

    //serialNum：テストデータの可読性向上のためにデバッグ目的でのみ使う番号。ログに付与される。
    //追加したログレコードを返却
    protected List<PlayLogRecord> SetServerLog(int numOfLogs){
       List<PlayLogRecord> records = CreateLogRecords(numOfLogs);
       Player p = new Player{id=this.playerId};
       p.LoadAllData();
       p.playLog.records.Clear();//ここでクリアしてやらないと、テストの度に次々とたされてしまう。
       p.playLog.records.AddRange(records);
       new PlayerLogAtServer(p.id).Save(p.playLog);
       return records;
    }

    protected List<PlayLogRecord> SetPlayerPrefs(int numOfLogs){
        List<PlayLogRecord> records = CreateLogRecords(numOfLogs);
        Player p = new Player{id=this.playerId};
        p.LoadAllData();
        p.playLog.records.Clear();
        p.playLog.records.AddRange(records);
        p.SaveAllData();
        return records;
    }

    protected List<PlayLogRecord> CreateNowPlayLog(int numOfLogs){
        List<PlayLogRecord> records = CreateLogRecords(numOfLogs);
        return records;
    }

    protected void CompareList(List<PlayLogRecord> l1, List<PlayLogRecord> l2){
        Assert.AreEqual(l1.Count, l2.Count);
        foreach(PlayLogRecord r in l1){
            Assert.AreEqual(l2.Contains(r), true);
        }
    }

    //サーバからログをDownloadして引数に指定したログと完全一致するかをチェック。だめならテストNG
    protected void CheckServerLog(List<PlayLogRecord> target){
        PlayLog plog = (new PlayerLogAtServer(this.playerId).Load());
        Debug.Log($"##### plog.count({plog.records.Count}) =? target.count({target.Count})");
        CompareList(plog.records, target);
    }

    //PlayerPrefsのログを取り出し、引数に指定したログと完全一致するかをチェック。だめならテストNG
    protected void CheckPlayerPrefs(List<PlayLogRecord> target){
        Player temp = new Player{id=this.playerId};
        temp.LoadAllData();
        CompareList(temp.playLog.records, target);
    }

    //今のPlayer.playLog.recordsを取り出し、引数に指定したログと完全一致するかをチェック。だめならテストNG
    protected void CheckNowPlayerLogs(Player p, List<PlayLogRecord> target){
        CompareList(p.playLog.records, target);
    }

}
}
