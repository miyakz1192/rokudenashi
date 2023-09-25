using UnityEngine;
using System.IO;
using System;
using NovelGame.Log;

using NovelGame.ResourceManagement;
using System.Linq;
using System.Collections.Generic;

namespace NovelGame
{
    [System.Serializable] //定義したクラスをJSONデータに変換できるようにする
    public class PlayerPlan
    {
        public string disp_name;
        public string disp_target_name;
        public string id;
        public string player_id;
        public int start_val;
        public int goal_val;
        public int unit_val;
        public string class_type; //例：PlayerPlanDecType
        [NonSerialized]
        public Player player;


        protected static string LoadData(string id)
        {
            try{
                MyDebug.Log($"[INFO] PlayerPlan.LoadData, id={id}");
                TextAsset playerPlanTextAsJSON = Resources.Load<TextAsset>($"PlayerInfo/Plan/{id}");
                MyDebug.Log($"[INFO] playerPlanTextAsJSON, id={playerPlanTextAsJSON}");
                MyDebug.Log($"[INFO] playerPlanTextAsJSON.txt, id={playerPlanTextAsJSON.text}");
                return playerPlanTextAsJSON.text;
            }catch(Exception e){
                MyDebug.Log($"[ERROR] in PlayerPlan.LoadData {e.Message}");
            }
            MyDebug.Log($"[WARN] PlayerPlan.LoadData, return null route");
            return null;
            /* //Android対応前
            string filePath = Application.dataPath + "/Resources/PlayerInfo/Plan/" + id + ".txt";
            if(System.IO.File.Exists(filePath)){
                StreamReader reader = new StreamReader(filePath);
                string jsonData = reader.ReadToEnd();
                reader.Close();
                return jsonData;
            }else{
                return null;
            }
            */
        }

        public static PlayerPlan Load(string id)
        {
            string data = LoadData(id);

            if(data != null){
                return JsonUtility.FromJson<PlayerPlan>(data);
            }else{
                PlayerPlan plan = new PlayerPlan();
                plan.id = id;
                plan.player_id = id;
                return plan;
            }   
        }

        public virtual List<string> CurrentStatusAsNovelText()
        {
            return new List<string>();
        }
    }

    public class PlayerPlanDecType : PlayerPlan
    {
        public static new PlayerPlanDecType Load(string id) //public static new とすることで、ベースクラスのLoadを隠すことができる(オーバライドではない)。
        {
            return JsonUtility.FromJson<PlayerPlanDecType>(LoadData(id));
        }

        public override List<string> CurrentStatusAsNovelText()
        {
            List<string> retList = new List<string>();

            if(this.player == null){
                retList.Add("ERROR: player not found in CurrenctStatusAsNovelText");
                return retList;
            }

            //1.いわゆる、PlayerPrefs上のログ＋現在のプレイのログを読み込む
            PlayLog log = this.player.playLog;

            //2.サーバに溜まっているログを読み込む
            ResourceUpdaterWithNetworking sv = new ResourceUpdaterWithNetworking();
            PlayLog logAtServer = sv.LoadPlayLog(this.player);
            Debug.Log($"LoadLog At Server Len={logAtServer.records.Count}");

            //1.と2.をつなげたログを作る。
            PlayLog temp = new PlayLog(this.player.id);
            temp.records = temp.records.Concat(log.records).Concat(logAtServer.records).ToList();

            int now_val = start_val - (unit_val * temp.records.Count);

            string res = disp_target_name + "の最初は、" + start_val + "でした。";
            res += player.disp_name + "は、いままで" + temp.records.Count + " 回問題をやりましたから、" + disp_target_name + "は減っており、現在は、" + now_val + "です。";
            res += "目標 " + goal_val + " まで、頑張ろう！";

            retList.Add(res);

            //サーバから持ってきたログの長さが0の場合、サーバがダウン等の場合があるので、
            //プレイヤーが心配しないように追加しておく
            if(logAtServer.records.Count == 0){
                retList.Add($"サーバに接続できないから、今まで実施した問題回数が少なく表示されているかも({temp.records.Count})。不明点はお父さんに聞いてね！");
            }

            return retList;
        }
    }

    public class PlayerPlanFactory
    {
        public static PlayerPlan Load(string player_id)
        {
            //まず最初にベーシックな読み出しをする
            PlayerPlan temp = PlayerPlan.Load(player_id);
            //classに記載されているクラスを指定して再度読み込んで返す。
            //evalを使いたいが標準では存在しない様子
            PlayerPlan res;
            switch(temp.class_type){
                case "PlayerPlanDecType":
                    res = PlayerPlanDecType.Load(player_id);
                    break;
                default:
                    res =  null;
                    break;
            }
            return res;
        }
    }
}
