using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert; // AssertはNUnitのではなくUnityのものを使う
using System;
using NovelGame.Net.Protocol;
using NovelGame;
using System.Linq;

namespace Tests{
public class ResourceUpdaterWithNetworkingTest
{
    protected int serialNum = 0;
    protected string playerId = "souchan";
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
        サーバのログ、PlayerPrefs、今プレイのログ →Update(OK)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             0             0                            0              0            0
        サーバのログ、PlayerPrefs、今プレイのログ →Update(OK)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             0             1                            1              0            0
        サーバのログ、PlayerPrefs、今プレイのログ →Update(OK)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             1             0                            1              0            0
        サーバのログ、PlayerPrefs、今プレイのログ →Update(OK)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             1             1                            2              0            0
        --------------------------------------------------------------------------------------------------
        サーバのログ、PlayerPrefs、今プレイのログ →Update(OK)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             0             2                            2              0            0
        サーバのログ、PlayerPrefs、今プレイのログ →Update(OK)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             2             0                            2              0            0
        サーバのログ、PlayerPrefs、今プレイのログ →Update(OK)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             2             2                            4              0            0
        --------------------------------------------------------------------------------------------------
        サーバのログ、PlayerPrefs、今プレイのログ →Update(ERR)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             0             0                            0              0            0
        サーバのログ、PlayerPrefs、今プレイのログ →Update(ERR)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             0             1                            0              1            0
        サーバのログ、PlayerPrefs、今プレイのログ →Update(ERR)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             1             0                            0              1            0
        サーバのログ、PlayerPrefs、今プレイのログ →Update(ERR)→　サーバのログ、PlayerPrefs、今プレイのログ
            0             1             1                            0              2            0
    */
    [Test]
    public void TestAll(){

    }

    /**
    　正常パターンはPlayerPrefsと今プレイログの合算が、サーバログに行くので、そのパターンのテスト
    */
    protected void TestNormalOnePattern(int initSvLog, int initPlayerPrefs, int initPlayLogs){
        PlayLogRecord[] svLogs = SetServerLog(initSvLog);
        PlayLogRecord[] playerPrefs = SetPlayerPrefs(initPlayerPrefs);
        PlayLogRecord[] playLogs = SetNowPlayLog(initPlayLogs);

        PlayLogRecord[] temp = new PlayLogRecord[svLogs.Length + playerPrefs.Length + playLogs.Length];

        temp.Concat(svLogs);
        temp.Concat(playerPrefs);
        temp.Concat(playLogs);

        CheckServerLog(temp.ToArray());
    }

    protected PlayLogRecord[] CreateLogRecords(int numOfLogs){
        return null;
    }

    //serialNum：テストデータの可読性向上のためにデバッグ目的でのみ使う番号。ログに付与される。
    //追加したログレコードを返却
    protected PlayLogRecord[] SetServerLog(int numOfLogs){
        PlayLogRecord[] records = CreateLogRecords(numOfLogs);
        //処理を・・・
        return null;
    }

    protected PlayLogRecord[] SetPlayerPrefs(int numOfLogs){
        return null;
    }

    protected PlayLogRecord[] SetNowPlayLog(int numOfLogs){
        return null;
    }

    //サーバからログをDownloadして引数に指定したログと完全一致するかをチェック。だめならテストNG
    protected void CheckServerLog(PlayLogRecord[] target){

    }

    //PlayerPrefsのログを取り出し、引数に指定したログと完全一致するかをチェック。だめならテストNG
    protected void CheckPlayerPrefs(PlayLogRecord[] target){

    }

    //今のPlayer.playLog.recordsを取り出し、引数に指定したログと完全一致するかをチェック。だめならテストNG
    protected void CheckNowPlayerLogs(PlayLogRecord[] target){

    }

}
}
