using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NovelGame{
    public class MiscDebug 
    {
        public string test_player_id = "the_test_player";
        public void Test()
        {
            Debug.Log("[START] MiscDebug");
            Player souchan = Player.Load(test_player_id);
            //TestPlayerLogNew();
            //TestPlayerLogAdd();
            TestPlayerPlan();
            Debug.Log("[END] MiscDebug");
        }

        public void TestPlayerPlan()
        {
            //normal pattern
            PlayerPlan plan = PlayerPlanDecType.Load(test_player_id);

            //factory pattern
            PlayerPlan plan2 = PlayerPlanFactory.Load(test_player_id);
        }

        public void TestPlayerLogAdd()
        {
            TestPlayerLogNew();

            PlayLog playLog = PlayLog.Load(test_player_id);

            PlayLogRecord r1 = new PlayLogRecord();
            r1.timeStamp = DateTime.Now.ToString();
            r1.targetQuestion = "99 + 1192";
            r1.playerAnswer = "1291";
            r1.playerResult = true;
            playLog.records.Add(r1);
            PlayLog.Save(playLog);
        }

        public void TestPlayerLogNew()
        {
            PlayLogRecord r1 = new PlayLogRecord();
            r1.timeStamp = DateTime.Now.ToString();
            r1.targetQuestion = "10 * 30";
            r1.playerAnswer= "20";
            r1.playerResult = false;

            PlayLog playLog = new PlayLog(test_player_id);
            playLog.records.Add(r1);
            PlayLog.Save(playLog);
            Debug.Log("[END] MiscDebug");
        }

        public void TestPlayerLogNotFound()
        {
            //もし、PlayLogファイルが存在しない場合は、playLogがnullになる。
            PlayLog playLog = PlayLog.Load("deadbeef");
            //playLog.records.Add(r1); //このまま操作するとnull pointer exceptionになる
        }
    }
}