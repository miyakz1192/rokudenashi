using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using NovelGame.Log;

namespace NovelGame
{
    [System.Serializable] //定義したクラスをJSONデータに変換できるようにする
    public class CharacterModel
    {
        public string name; //キャラクタの名前(novel.txt内でのスクリプト指定用途) 常にidと同じ値になる。
        public string main_name; //キャラクタの名前(画面表示用)
        public string sub_name; //キャラクタの名前(画面表示用)
        public int hp; //ヒットポイント
        public int attack; //攻撃力
        public int defence; //防御力
        public string main_image;
        public List<AttackInfo>  attack_pattern;
        public CharacterInfoView view;
                
        /// <summary>
        /// nameで指定するキャラクタ識別子に対応するキャラクタプロファイルを読み込む。
        /// </summary>
        /// <param name="name"></param>
        public static CharacterModel Load(string name)
        {
            try{
                MyDebug.Log($"[INFO] CharacterModel.Load, name={name}");
                TextAsset textDataAsJSON = Resources.Load<TextAsset>($"Character/{name}");
                return JsonUtility.FromJson<CharacterModel>(textDataAsJSON.text);
            }catch(Exception e){
                MyDebug.Log($"[ERROR] in CharacterModel.Load {e.Message}");
            }
            MyDebug.Log($"[WARN] CharacterModel.Load, return null route");
            return null;

            /*//従来処理 androd対応前
            string filePath = Application.dataPath + "/Resources/Character/" + name + ".txt";
            StreamReader reader = new StreamReader(filePath);
            string jsonData = reader.ReadToEnd();
            return JsonUtility.FromJson<CharacterModel>(jsonData);
            */
        }

        public string GetId()
        {
            return name;
        }

        public static void Unload(CharacterModel model)
        {
            //Do nothing now.
        }

        /// <summary>
        /// view とmodelを関連付ける
        /// </summary>
        /// <param name="view"></param>
        public void AssociateModelAndView(CharacterInfoView view)
        {
            view.model = this;
            this.view = view;
        }

        public AttackInfo AttackInfoByRandam()
        {
            List<AttackInfo> temp = new List<AttackInfo>();
            int c = 0;

            foreach(AttackInfo a in attack_pattern){
                for(int i = 0; i < a.prob; i ++){
                    temp.Add(a);
                    c ++;
                }
            }
            System.Random r1 = new System.Random();
            int idx = r1.Next(0,c);//0~100までの数を生成する。
            return temp[idx];
        }
    }

    [System.Serializable] 
    public class AttackInfo
    {
        public string name;
        public string v_effect;
        public string s_effect;
        public int prob;
    }
}