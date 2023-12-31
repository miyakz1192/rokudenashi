using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert; // AssertはNUnitのではなくUnityのものを使う

using System;
using NovelGame.Log;
using System.IO;
using NovelGame;

namespace Tests{
    public class MyDebugTest{
        [Test]
        public void MyDebugShuldSendingMessage(){
            MyDebug.Log("debug message from uniy rokudenashi!");
        }
    }

    //使い捨てのメソッド
    public class DataTransfer{
        /**
        　PlayerLogをResources配下から、PlayerPrefsに移行するための
        　使い捨てのプログラムを、テストとして作った。
        　すでに移行済みのため、もう以下のコードは使わない。
        　テストとしていつも成功する。
        */
        [Test]
        public void DataTransferTest(){
            /*//完了
            //souchanのPlayerLogを生stringで取得する。
            string filePath = Application.dataPath + "/Resources/PlayerInfo/Log/" + id + ".txt";
            StreamReader reader = new StreamReader(filePath);
            string jsonData = reader.ReadToEnd();
            //それをPlayerPerfsに移す。
            //PlayerPrefs.SetString("Player Name", "Foobar");
            PlayerPrefs.SetString(id + "_playlog", jsonData);
            */
            //試しにReadしてみる。
            //PlayerPrefs.GetString(id + "_playlog");
            Player p = new Player
            {
                id = "souchan"
            };
            p.LoadAllData();
            //Debug.Log(PlayerPrefs.GetString(id + "_playlog"));//これを実行すると、デバッグログの表示があふれて、後続がデバッグログ出ず。なので、コメントアウト
            Debug.Log($"loglen = {p.playLog.records.Count}");
            foreach(PlayLogRecord r in p.playLog.records){
                Debug.Log(r.timeStamp);
            }
        }

        [Test]
        public void ShowResources(){
            //たまたまResources配下を見たくなっただけの捨てテストコード
            // Resourcesディレクトリ内のすべてのアセットを読み込む
            UnityEngine.Object[] loadedAssets = Resources.LoadAll("");
            //string res = "";

            // 読み込んだアセットをログに表示
            foreach (UnityEngine.Object asset in loadedAssets)
            {
                Debug.Log("Loaded Asset: " + asset.name);
                //res += " " + asset.name;
            }
            string root_url = Resources.Load<TextAsset>("root_url_for_test").text;
            string player_group_uuid = Resources.Load<TextAsset>("player_group_uuid_for_test").text;

            //原因は、root_url_for_testに"root_url _for_test"と余計な空白が混じっていただけ
//            string root_url = Resources.Load<TextAsset>("root_url").text;
//            string player_group_uuid = Resources.Load<TextAsset>("player_group_uuid").text;

            //return res;
        }
    }
}
