using UnityEngine;
using System.IO;
using System;
using NovelGame.Log;


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

        public virtual string CurrentStatusAsNovelText()
        {
            return "none";
        }
    }

    public class PlayerPlanDecType : PlayerPlan
    {
        public static new PlayerPlanDecType Load(string id) //public static new とすることで、ベースクラスのLoadを隠すことができる(オーバライドではない)。
        {
            return JsonUtility.FromJson<PlayerPlanDecType>(LoadData(id));
        }

        public override string CurrentStatusAsNovelText()
        {
            if(this.player == null){
                return "ERROR: player not found in CurrenctStatusAsNovelText";
            }

            PlayLog log = this.player.playLog;//TODO: Serverに溜まっているログをロードして加算が必要。

            int now_val = start_val - (unit_val * log.records.Count);

            string res = disp_target_name + "の最初は、" + start_val + "でした。";
            res += player.disp_name + "は、いままで" + log.records.Count + " 回問題をやりましたから、" + disp_target_name + "は減っており、現在は、" + now_val + "です。";
            res += "目標 " + goal_val + " まで、頑張ろう！";

            return res;
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
