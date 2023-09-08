using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace NovelGame{
    public class NovelManager
    {
        public List<string> masterNovel{get;}
        public Dictionary<string, List<string>> novels = new Dictionary<string, List<string>>();

        public NovelManager(TextAsset masterNovelTextAsset)
        {
            //最初のマスターnovelの読み込みを、アセットに設定されているAssets/Text/novel.txtから読み込む。
            StringReader reader = new StringReader(masterNovelTextAsset.text);
            masterNovel = LoadNovel(reader);
            LoadAllSubNovel();
        }

        public List<string> ChoiceNovelByRandom()
        {
            string key = (string)MyCollection.RandomChoice(novels.Keys.ToList<string>());
            if(key == null){
                return null;
            }
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
            //Assets/Resources/Novels/*.txtを列挙する
            string[] names = Directory.GetFiles(@"Assets/Resources/Novels/", "*.txt");
            foreach(string name in names){
                //それぞれに対して、LoadNovelFromFileを実行
                string subNovelFileName = Path.GetFileNameWithoutExtension(name);
                List<string> novel = LoadNovelFromFile(subNovelFileName);
                string id = subNovelFileName;
                novels.Add(id, novel);
            }
        }

        private List<string> LoadNovelFromFile(string fileName)
        {
            List<string> res = new List<string>();
            TextAsset text = Resources.Load<TextAsset>("Novels/" + fileName);
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
