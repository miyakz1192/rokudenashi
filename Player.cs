using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace NovelGame{
    [System.Serializable] //定義したクラスをJSONデータに変換できるようにする
    public class Player
    {
        //キャラクタプロファイル名＝id。通常、disp_nameはゲームに表示する名前
        public int total_answered_questions = 0; //そのバトル回での合計回答問題数
        public int total_correct_questions = 0; //そのバトル回での合計正答問題数
        public string disp_name = "no_disp_name";
        public string id = "no_id";
        public string plan_id = "no_plan_id";
        public PlayLog playLog = null;
        public PlayerPlan plan = null;
    

        /// <summary>
        /// idで指定したplayerプロファイル
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Player Load(string id)
        {
            string filePath = Application.dataPath + "/Resources/PlayerInfo/Player/" + id + ".txt";
            if(System.IO.File.Exists(filePath)){
                StreamReader reader = new StreamReader(filePath);
                string jsonData = reader.ReadToEnd();
                return JsonUtility.FromJson<Player>(jsonData);
            }else{
                Player res = new Player();
                res.id = id;
                return res;
            }
        }

        public void AddLog(string targetQuestion = "invalid question", string playerAnswer = "invalid ans", Boolean playerResult=false)
        {
            if(this.playLog == null){
                return;
            }
            PlayLogRecord logrec = new PlayLogRecord(targetQuestion, playerAnswer, playerResult);
            this.playLog.records.Add(logrec);
        }

        public void LoadAllData()
        {
            LoadLog();
            LoadPlan();
        }

        private void LoadLog()
        {
            PlayLog temp = PlayLog.Load(this.id);
            this.playLog = temp;
        }

        private void LoadPlan()
        { 
            PlayerPlan plan = PlayerPlanFactory.Load(this.id);
            this.plan = plan;
            plan.player = this;
        }

        public void SaveAllData()
        {
            SaveLog();
            SavePlan();
        }

        private void SaveLog()
        {
            if(this.playLog == null){
                return;
            }
            this.playLog.Save();
        }

        private void SavePlan()
        {
            //Planは今の所read onlyなので、saveは空。
        }
    }
}