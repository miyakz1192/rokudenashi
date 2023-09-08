using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace NovelGame{
    public class QuizManager
    {
        public int q_prob {get; set;} //問題の出現確率

        /// <summary>
        /// 問題を出すかどうか
        /// </summary>
        /// <returns></returns>
        public bool IsQuestion()
        {
            System.Random r1 = new System.Random();
            int temp = r1.Next(0,101);//0~100までの数を生成する。
            return temp <= this.q_prob;
        }

        public List<string> AttackPointQuizSentenceMultiply(CharacterModel defender, int attack_point)
        {
            List<string> novel = new List<string>();
            List<int> a = new List<int>();
            foreach(int x in Prime.PrimeFactors(attack_point)){
                a.Add(x);
            }
            List<int> front = null;
            List<int> rear = null;
            int start = 0;
            int end = 0;
            end = a.Count/2;
            front = a.GetRange(start, a.Count/2);
            rear = a.GetRange(end, a.Count-front.Count);

            //thanks: https://qiita.com/Inscientor/items/5ca8523bcd3ff245ef18
            int p = 1;
            int q = 1;

            if(front.Count != 0){
                p = front.Aggregate((result, current) => result * current);
            }
            
            if(rear.Count !=0 ){
                q = rear.Aggregate((result, current) => result * current);
            }


            novel.Add(defender.sub_name + " に " + p + " × " + q + " のダメージ！");
            novel.Add("クイズです！ここで、 与えたダメージを計算してみましょう！ " + p + " × " + q + " は？");

            int ans = p * q;
            novel.Add(CreateQuestionDialogInstruction(ans + 10, ans , ans -10, ans, question: p.ToString()+"*"+q.ToString()));
            novel.Add("答えを選択してね！");
            novel.Add("答えは " + ans + "　でした!");

            return novel;
        }

        public int AttackPointQuizSentenceDivision(CharacterModel attacker, CharacterModel defender, List<string> novel)
        {
            System.Random r1 = new System.Random();
            System.Random r2 = new System.Random();
            int ptn = r1.Next(0,2); //0~1までの数値を生成する
            int p = 0;//商
            int q = 0;//割る数(約数)
            int r = 0;//あまり
            int ans = 0;
            int attack_point = 0;
            int temp = 0; 
            string question = "Q-none";

            switch(ptn){
                case 0://　商を求める問題(必ず割り切れる)
                    attack_point = attacker.attack + r1.Next(0,attacker.attack);
                    q = r2.Next(2,11);//2~10の数値を生成 //TODO: チューナブルにする
                    p = attack_point * q;
                    ans = p/q;
                    question = p.ToString()+"/"+q.ToString();
                    novel.Add(defender.sub_name + " に " + p + " ÷ " + q + " のダメージ！");
                    novel.Add("クイズです！ここで、 与えたダメージを計算してみましょう！ " + p + " ÷ " + q + " の商は？");
                    break;
                case 1://　あまりを求める問題(必ず割り切れない)
                    q = r2.Next(2,11);//2~10の数値を生成 //TODO: チューナブルにする
                    r = r2.Next(1,q);
                    if((int)(attacker.attack * 0.1) < 1){//Next関数のエラー回避。Random min 'minValue' can not be greater then 'maxValue'
                        p = 1;
                    }else{
                        p = r2.Next(1,(int)(attacker.attack * 0.1));//TODO: ↑のq = r2.Next(2,11);の11(10)のところと関連性あり
                    }
                    temp  = p * q + r;
                    ans = (temp) % q;
                    attack_point = ans;
                    question = temp.ToString()+"%"+q.ToString();
                    novel.Add(defender.sub_name + " に " +temp + " ÷ " + q + " の「あまり」ダメージ！");
                    novel.Add("クイズです！ここで、 与えたダメージを計算してみましょう！ " + temp + " ÷ " + q + " のあまりは？");
                    break;
                default:
                    Debug.Log("ERROR:! invalid ptn");
                    break;
            }
            novel.Add(CreateQuestionDialogInstruction(ans + 10, ans , ans -10, ans, question: question));
            novel.Add("答えを選択してね！");
            novel.Add("答えは " + ans + "　でした!");
            return attack_point;
        }

        public List<string> AttackPointQuizSentenceAddition(CharacterModel defender, int attack_point)
        {
            List<string> novel = new List<string>();
            System.Random r1 = new System.Random();
            int ptn = r1.Next(0,2); //0~1までの数値を生成する
            int p = 0;
            int q = 0;

            switch(ptn){
                case 0://　ひと桁＋ひと桁〜複数桁
                    System.Random r2 = new System.Random();
                    p = r2.Next(1,10);//1~9の数値を生成
                    q = attack_point - p;
                    novel.Add(defender.sub_name + " に " + p + " ＋ " + q + " のダメージ！");
                    break;
                case 1://　複数桁＋複数桁
                    p = attack_point / 2;
                    q = attack_point - p;
                    novel.Add(defender.sub_name + " に " + p + " ＋ " + q + " のダメージ！");                    
                    break;
                default:
                    Debug.Log("ERROR:! invalid ptn");
                    break;
            }
            novel.Add("クイズです！ここで、 与えたダメージを計算してみましょう！ " + p + " ＋ " + q + " は？");
            int ans = p + q;
            string question = p.ToString()+"+"+q.ToString();
            novel.Add(CreateQuestionDialogInstruction(ans + 10, ans , ans -10, ans, question: question));
            novel.Add("答えを選択してね！");
            novel.Add("答えは " + ans + "　でした!");
            return novel;
        }

        public int AttackPointQuizSentence(CharacterModel attacker, CharacterModel defender, List<string> novel)
        {
            System.Random r1 = new System.Random();
            int ptn = r1.Next(0,3); //0~2までの数値を生成する
            int attack_point = 0;

            switch(ptn){
                case 0://乗算
                    attack_point = attacker.attack + r1.Next(0,attacker.attack);
                    novel.AddRange(AttackPointQuizSentenceMultiply(defender, attack_point));
                    break;
                case 1://除算
                    attack_point = AttackPointQuizSentenceDivision(attacker, defender, novel);
                    break;
                case 2://加算
                    attack_point = attacker.attack + r1.Next(0,attacker.attack);
                    novel.AddRange(AttackPointQuizSentenceAddition(defender, attack_point));
                    break;
                default:
                    Debug.Log("ERROR:! invalid ptn");
                    break;
            }

            return attack_point;
        }


        private string CreateQuestionDialogInstruction(int val_choice1, int val_choice2, int val_choice3, int ok_val, string question="Q")
        {
            //"&dialog " +  (defender.hp + -11).ToString() + " " + (defender.hp).ToString() + " " + (defender.hp + 10).ToString() + " " + ThreeChoiceDialog.ThreeChoiceDialogResult.Choice2
            List<string> temp = new List<string>();
            List<ThreeChoiceDialog.ThreeChoiceDialogResult> idx = new List<ThreeChoiceDialog.ThreeChoiceDialogResult>();

            temp.Add(val_choice1.ToString());
            temp.Add(val_choice2.ToString());
            temp.Add(val_choice3.ToString());

            idx.Add(ThreeChoiceDialog.ThreeChoiceDialogResult.Choice1);
            idx.Add(ThreeChoiceDialog.ThreeChoiceDialogResult.Choice2);
            idx.Add(ThreeChoiceDialog.ThreeChoiceDialogResult.Choice3);

            //https://webbibouroku.com/Blog/Article/array-shuffle
            temp = temp.OrderBy(i => Guid.NewGuid()).ToList();
            ThreeChoiceDialog.ThreeChoiceDialogResult correct_choice = idx[temp.FindIndex(x => x == (ok_val).ToString())];

            string res = "&dialog " + temp[0] + " " + temp[1] + " " + temp[2] + " " + correct_choice + " " + question;

            Debug.Log(res);

            return res;
        }
    }

    public class Prime{
        public static IEnumerable<int> PrimeFactors(int n)
        {
            int i = 2;
            int tmp = n;

            while (i * i <= n) //※1
            {
                if(tmp % i == 0){
                    tmp /= i;
                    yield return i;
                }else{
                    i++;
                }
            }
            if(tmp != 1) yield return tmp;//最後の素数も返す
        }
    }
}