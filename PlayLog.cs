using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using NovelGame.Log;

namespace NovelGame{
    [System.Serializable] //定義したクラスをJSONデータに変換できるようにする
    public class PlayLogRecord
    {
        public string timeStamp;//DateTimeだと何故か、Serializeしてくれない。タイムスタンプなのでDateTimeでなくてstringで十分！
        public string targetQuestion;
        public Boolean playerResult; //プレイヤーの回答結果。正答していればtrue
        public string playerAnswer; //プレイヤーの出した回答

        public PlayLogRecord(string targetQuestion = "Q-none", string playerAnswer = "A-none", Boolean playerResult=false)
        {
            this.timeStamp = DateTime.Now.ToString();
            this.targetQuestion = targetQuestion;
            this.playerAnswer = playerAnswer;
            this.playerResult = playerResult;
        }

        public override bool Equals(object obj){
            if (obj == null || GetType() != obj.GetType()){
                return false;
            }

            PlayLogRecord r = (PlayLogRecord)obj;
            return timeStamp.Equals(r.timeStamp) && targetQuestion.Equals(r.targetQuestion) &&
                   playerAnswer.Equals(r.playerAnswer) && playerResult.Equals(r.playerResult);
        }

        public override int GetHashCode()
        {
            //Equalsをオーバライドした場合、GetHashCodeもオーバライドが必要(しないと、warningがでる)
            // 各プロパティのハッシュコードを組み合わせて一意なハッシュコードを生成
            return HashCode.Combine(timeStamp, targetQuestion, playerAnswer, playerResult);
        }
    }

    [System.Serializable] //定義したクラスをJSONデータに変換できるようにする
    public class PlayLog
    {
        public string playerId;
        public List<PlayLogRecord> records = new List<PlayLogRecord>();

        public PlayLog(string playerId)
        {
            this.playerId = playerId;
        }

        protected static string PlayerPrefsKey(string id){
            //PlayerPrefsに与えるkey_nameを生成
            //id(ユーザ名) + _logとなる。
            return id + "_playlog";
        }

        /// <summary>
        /// idで指定したplayerLogをすべて読み込む
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static PlayLog Load(string id)
        {
            PlayLog res;
            string rawDataAsJSON = PlayerPrefs.GetString(PlayerPrefsKey(id));
            res = JsonUtility.FromJson<PlayLog>(rawDataAsJSON);
            MyDebug.Log($"Loading Player Log as {id}");
            if(res == null){//Jsonパースがうまく行かなかった場合
                return new PlayLog(id);
            }
            MyDebug.Log($"Loaded Player Log as {id} , len={res.records.Count}");
            return res;
            /* //android対応前
            PlayLog res;
            string filePath = Application.dataPath + "/Resources/PlayerInfo/Log/" + id + ".txt";
            if(System.IO.File.Exists(filePath)){
                StreamReader reader = new StreamReader(filePath);
                string jsonData = reader.ReadToEnd();
                reader.Close();
                res = JsonUtility.FromJson<PlayLog>(jsonData);
                if(res == null){//Jsonパースがうまく行かなかった場合
                    return new PlayLog(id);
                }
                return res;
            }else{
                return new PlayLog(id);
            }
            */
        }

        public void Save()
        {
            PlayLog.Save(this);
        }

        public static void Save(PlayLog playLog)
        {
            string jsonstr = JsonUtility.ToJson(playLog);//受け取ったPlayerDataをJSONに変換
            PlayerPrefs.SetString(PlayerPrefsKey(playLog.playerId), jsonstr);
            MyDebug.Log($"Saving Player Log as {playLog.playerId} , len={playLog.records.Count}");
            /* //Android対応前
            string filePath = Application.dataPath + "/Resources/PlayerInfo/Log/" + playLog.playerId + ".txt";
            string jsonstr = JsonUtility.ToJson(playLog);//受け取ったPlayerDataをJSONに変換
            StreamWriter writer = new StreamWriter(filePath, false);//初めに指定したデータの保存先を開く(falseを指定してファイルを上書き)
            writer.WriteLine(jsonstr);//JSONデータを書き込み
            writer.Flush();//バッファをクリアする
            writer.Close();//ファイルをクローズする
            */
        }
    }
}