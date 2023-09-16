using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert; // AssertはNUnitのではなくUnityのものを使う

using System;
using NovelGame.Log;
using System.IO;

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
            string id = "souchan";

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
            Debug.Log(PlayerPrefs.GetString(id + "_playlog"));
        }
    }
}
