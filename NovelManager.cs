using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

using NovelGame.Log;
using System;

namespace NovelGame{
    public class NovelManager
    {
        public List<string> masterNovel{get;}
        public Dictionary<string, List<string>> novels = new Dictionary<string, List<string>>();

        public NovelManager()
        {
            //最初のマスターnovelの読み込みを、アセットに設定されているAssets/Text/novel.txtから読み込む。
            //しかし、このコードは配布ビルドだとバイナリにAssets/Text/novel.txtが含まれないため意味がない。
            //StringReader reader = new StringReader(masterNovelTextAsset.text);
            //masterNovel = LoadNovel(reader);

            //したがって、新たにResources/novel.txtを読み込んでくる必要がある。
            MyDebug.Log("NovelManager1");
            masterNovel = LoadNovelFromFile("novel");//拡張子は不要のunity仕様らしい。
            MyDebug.Log("NovelManager2 = " + masterNovel[0]);
            LoadAllSubNovel();
            MyDebug.Log("NovelManager3");
        }

        public List<string> ChoiceNovelByRandom()
        {
            MyDebug.Log("[START] ChoiceNovelByRandom");
            string key = (string)MyCollection.RandomChoice(novels.Keys.ToList<string>());
            if(key == null){
                MyDebug.Log("[WARN] ChoiceNovelByRandom return null");
                return null;
            }
            MyDebug.Log($"[END] ChoiceNovelByRandom {key},{novels[key].First()}");
            return novels[key];
        }

        public List<string> ChoiceNovelById(string id)
        {
            if(novels.TryGetValue(id, out List<string> novel) == false){
                return new List<string>();
            }
            return novels[id];
        }

        private void LoadAllSubNovel()
        {
            //変更後の処理。
            try{
                MyDebug.Log("[START] LoadAllSubNovel");
                //Android環境にも対応できるよう、Resource APIを用いる。
                UnityEngine.Object []texts = null;
                texts = Resources.LoadAll("Novels/", typeof(TextAsset));
                MyDebug.Log($"[INFO] texts.len={texts.Length}");    
                foreach(UnityEngine.Object text in texts){
                    TextAsset t = (TextAsset)text;
                    List<string> novel = LoadNovel(new StringReader(t.text));
                    string id = text.name;
                    novels.Add(id, novel);
                    MyDebug.Log($"[INFO] id={id}");    
                    MyDebug.Log($"[INFO] name={text.name} added in to DB(novels hash) first={novel.First()}");
                }
                MyDebug.Log("[END] LoadAllSubNovel");
            }catch(Exception e){
                MyDebug.Log("[ERROR] LoadAllSubNovel=" + e.Message + " stack= " + e.ToString());
            }
            //変更後の処理ここまで


            //従来処理(一通り疎通し終わったら削除する)
            //Assets/Resources/Novels/*.txtを列挙する
            /*
            try{
                MyDebug.Log("[START] LoadAllSubNovel");
                MyDebug.Log("[INFO] LoadAllSubNovel PWD files = " + MyDebug.GetPWDfiles());
                //Android環境では以下で例外が発生してしまう。
                string[] names = Directory.GetFiles(@"Assets/Resources/Novels/", "*.txt");
                MyDebug.Log("LoadAllSubNovel:1");
                foreach(string name in names){
                    MyDebug.Log("LoadAllSubNovel:2-1");
                    //それぞれに対して、LoadNovelFromFileを実行
                    string subNovelFileName = Path.GetFileNameWithoutExtension(name);
                    MyDebug.Log("LoadAllSubNovel:2-1-1");
                    List<string> novel = LoadNovelFromFile("Novels/" + subNovelFileName);
                    MyDebug.Log("LoadAllSubNovel:2-1-2");
                    string id = subNovelFileName;
                    novels.Add(id, novel);
                    MyDebug.Log("LoadAllSubNovel:2-2");
                }
                MyDebug.Log("[END] LoadAllSubNovel");
            }catch(Exception e){
                MyDebug.Log("[ERROR] LoadAllSubNovel=" + e.Message + " stack= " + e.ToString());
            }
            //従来処理ここまで
            */
        }

        private List<string> LoadNovelFromFile(string filePath)
        {
            List<string> res = new List<string>();
            TextAsset text = Resources.Load<TextAsset>(filePath);
            StringReader reader = new StringReader(text.text);
            return LoadNovel(reader);
        }

        private List<string> LoadNovel(StringReader reader)
        {
            List<string> novel = new List<string>();

            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                if(IsCommentedSentence(line)){//コメント行は入れない
                    continue;
                }
                novel.Add(line);
            }
            return novel;
        }

        private bool IsCommentedSentence(string sentence)
        {
            string[] words = sentence.Split(' ');
            if(words[0] == "//"){
                return true;
            }
            return false;
        }

    }
}
