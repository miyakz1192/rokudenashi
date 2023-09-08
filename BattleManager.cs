using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace NovelGame
{
    public class BattleManager : MonoBehaviour
    {
        private int _battleTurn = 0;
        private bool _isBattle = false; //バトル中か、そうでないかを保持するフラグ(true is in Battle)
        private List<CharacterModel> _chars = new List<CharacterModel>(); //CharacterModelを保持するリスト
        private int _player = 0; //_charsのうち、何番目がプレイヤーキャラクターかを指定。
        private QuizManager _quiz_manager = new QuizManager();
        private bool _isBattleFinished = false;//バトルが決着ついたかどうか。trueなら決着済み。
        private int _turn_cond = -1;//バトルを中断するターン数(-1は中断しない、デフォルト)
        
        /// <summary>
        /// バトルを開始し、各リソースを確保する
        /// </summary>
        /// <param name="charName1">キャラクタ１の識別子（通常、こちらがプレイヤー）</param>
        /// <param name="charName2">キャラクタ２の識別子（通常、こちらが敵）</param>
        /// <param name="q_prob">問題を出す確率(0~100)で指定</param>
        public void StartBattle(string charName1, string charName2, string q_prob)
        {
            CharacterModel char1 = CharacterModel.Load(charName1);//Resourcesディレクトリ配下のcharName1に一致するキャラクタ定義ファイルを読み込み            
            CharacterInfoView ciview1 = new CharacterInfoView();
            ciview1.SetWindowIndex(1);//1番目のウインドウ
            char1.AssociateModelAndView(ciview1); //モデルと、キャラクタ情報ウインドウの関連付け

            CharacterModel char2 = CharacterModel.Load(charName2);
            CharacterInfoView ciview2 = new CharacterInfoView();//2番目のウインドウ
            ciview2.SetWindowIndex(2);//1番目のウインドウ
            char2.AssociateModelAndView(ciview2);

            _chars.Add(char1);
            _chars.Add(char2);

            Debug.Log(char1.name);
            Debug.Log(char2.name);
            _quiz_manager.q_prob = int.Parse(q_prob);

            _isBattle = true;
        }

        /// <summary>
        /// バトルを終了し、各リソースを開放する
        /// </summary>
        public void EndBattle()
        {
            foreach(var c in _chars){
                CharacterModel.Unload(c);
            }
            _isBattle = false;
        }

        public bool InBattle()
        {
            return _isBattle;
        }

        public void Pause()
        {
            _isBattle = false;
        }

        public void Resume()
        {
            if(_isBattleFinished){//すでにバトルが決着ついた場合は再開できない。
                return;
            }
            _isBattle = true;
        }

        public CharacterModel Player()
        {
            return _chars[_player];
        }

        /// <summary>
        /// このターンの攻撃者のインデックスを返却
        /// </summary>
        /// <returns></returns>
        private int AttackerIndexInThisTurn()
        {
            return _battleTurn % _chars.Count;
        }
        /// <summary>
        /// このターンの防御者のインデックスを返却
        /// </summary>
        /// <returns></returns>
        private int DefenderIndexInThisTurn()
        {
            return (_chars.Count - 1) - AttackerIndexInThisTurn();
        }

        /// <summary>
        /// このターンの攻撃者を返却
        /// </summary>
        /// <returns></returns>
        private CharacterModel AttackerInThisTurn()
        {
            return _chars[AttackerIndexInThisTurn()];
        }

        /// <summary>
        /// このターンの防御者を返却
        /// </summary>
        /// <returns></returns>
        private CharacterModel DefenderInThisTurn()
        {
            return _chars[DefenderIndexInThisTurn()];
        }

        /// <summary>
        /// Defenderの情報(HPなど)をもとに、問題用のダイアログ命令テキストを生成する
        /// 正答位置(Choise)は毎回ランダム
        /// </summary>
        /// <param name="defender"></param>
        /// <returns></returns>
        private string CreateQuestionDialogInstruction(CharacterModel defender, string question="Q-none1")
        {
            //"&dialog " +  (defender.hp + -11).ToString() + " " + (defender.hp).ToString() + " " + (defender.hp + 10).ToString() + " " + ThreeChoiceDialog.ThreeChoiceDialogResult.Choice2
            List<string> temp = new List<string>();
            List<ThreeChoiceDialog.ThreeChoiceDialogResult> idx = new List<ThreeChoiceDialog.ThreeChoiceDialogResult>();

            temp.Add((defender.hp + -11).ToString());
            temp.Add((defender.hp).ToString());//正解
            temp.Add((defender.hp + 10).ToString());

            idx.Add(ThreeChoiceDialog.ThreeChoiceDialogResult.Choice1);
            idx.Add(ThreeChoiceDialog.ThreeChoiceDialogResult.Choice2);
            idx.Add(ThreeChoiceDialog.ThreeChoiceDialogResult.Choice3);

            //https://webbibouroku.com/Blog/Article/array-shuffle
            temp = temp.OrderBy(i => Guid.NewGuid()).ToList();
            ThreeChoiceDialog.ThreeChoiceDialogResult correct_choice = idx[temp.FindIndex(x => x == (defender.hp).ToString())];

            string res = "&dialog " + temp[0] + " " + temp[1] + " " + temp[2] + " " + correct_choice + " " + question;

            Debug.Log(res);

            return res;
        }

        /// <summary>
        /// バトルを1ターン実施し、次に実施すべきユーザシナリオのリストを返す
        /// </summary>
        /// <returns></returns>
        private List<string> BattleOneTurnV2()
        {
            List<string> novel = new List<string>();

            if(InBattle() == false){
                return novel;
            }

            CharacterModel attacker = AttackerInThisTurn();
            CharacterModel defender = DefenderInThisTurn();

            novel.Add("&img " + defender.main_image + " eventObject");
            novel.Add(attacker.sub_name + " のこうげき！");

            AttackInfo atk_info = attacker.AttackInfoByRandam();

            novel.Add(attacker.sub_name + "：「" + atk_info.name + "！！」");
 
            System.Random r1 = new System.Random();
            int atk = 0;

            if(_quiz_manager.IsQuestion()){
                atk = _quiz_manager.AttackPointQuizSentence(attacker, defender, novel);
            }else{
                atk = attacker.attack + r1.Next(0,attacker.attack);
                novel.Add(defender.sub_name + " に " + atk + " のダメージ！");
            }
            
            if( atk_info.v_effect != null){
                novel.Add("&v_effect " + atk_info.v_effect);
            }
            if( atk_info.s_effect != null){
                novel.Add("&efctsnd " + atk_info.s_effect);
            }
            novel.Add("&shakeimg " +  defender.main_image);
            novel.Add(defender.sub_name + "：" + "「ぐわわ！」");

            string before_hp = defender.hp.ToString();
            defender.hp -= atk;
            novel.Add("&battle dec_hp " + defender.name + " " + atk.ToString());

            if(_quiz_manager.IsQuestion() == true){
                novel.Add("クイズです！ここで、 " + defender.sub_name + " の残り体力はいくらでしょう？");
                novel.Add("体力はもともと " + before_hp + " あり、 " + atk.ToString() + " のダメージを受けたから?" );
                novel.Add(CreateQuestionDialogInstruction(defender,question: before_hp+"-"+atk.ToString()));
                //novel.Add("&dialog " +  (defender.hp + -11).ToString() + " " + (defender.hp).ToString() + " " + (defender.hp + 10).ToString() + " " + ThreeChoiceDialog.ThreeChoiceDialogResult.Choice2);
                novel.Add("答えを選んでね！");
                novel.Add("答えは " + defender.hp.ToString() + " でした!");
            }

            if( defender.hp <= 0 ){//imgefct explosion
                novel.Add(defender.sub_name + " は倒れた！");
                novel.Add("&efctsnd zombievoice1");
                novel.Add(defender.sub_name + "：" + "「ぐええ！」");

                novel.Add("&rmimg " + defender.main_image);
                novel.Add("&battle result");
                novel.Add("&end_battle");
                _isBattleFinished = true;
                //EndBattle();
            }else{
                if(IsPauseCondition()){//バトルの一時停止の条件の場合
                    novel.Add("&rmimg " + defender.main_image);//かならずイメージを削除する(そうしないと二重登録エラー)
                    novel.Add("&battle pause");           
                }else{//上記以外の場合はバトルを継続
                    novel.Add("&rmimg " + defender.main_image);//かならずイメージを削除する(そうしないと二重登録エラー)
                    novel.Add("&battle new_turn");
                }
            }

            _battleTurn ++;

            return novel;
        }

        public void SetTurnCondition(int turn_cond)
        {
            this._turn_cond = turn_cond;
        }

        public void ResetTurnCondition()
        {
            this._turn_cond = -1;
        }

        /// <summary>
        /// バトルを一時停止する条件
        /// </summary>
        /// <returns></returns>
        private bool IsPauseCondition()
        {
            if(_turn_cond == -1){//turn_condが未設定ならばいつもPauseCondではない(常に継続)
                return false;
            }

            if(_battleTurn == _turn_cond){//条件を検査
                return true;
            }
            return false;
        }

        /// <summary>
        /// バトルを1ターン実施し、次に実施すべきユーザシナリオのリストを返す
        /// </summary>
        /// <returns></returns>
        private List<string> BattleOneTurn()
        {
            /**
                バトルの基本仕様（初版：まずは単純バージョン）
                　・プレイヤーの攻撃が先
                　・敵があと
                　・プレイヤーが技などを選択する画面（ダイアログボックスか？）はまだ、作成しない（難しいので）。
                　・プレイヤーと敵がそれぞれ、攻撃を出し合う。プレイヤーと敵の攻撃の値はランダムに変化することで問題にバリエーションを作る
                　　（将来的には連続した桁の繰り下がりだったり、何だったりを意図的に出題できるようにしたいが、、、例：100 - 23とか、100-1とか）
                　・どちらかのHPが先に0または負になった時点で終了。
                　・ランダムに出題する。
            */
            List<string> novel = new List<string>();

            if(InBattle() == false){
                return novel;
            }

            CharacterModel attacker = AttackerInThisTurn();
            CharacterModel defender = DefenderInThisTurn();

            novel.Add("&img " + defender.main_image + " eventObject");
            novel.Add(attacker.sub_name + " のこうげき！");

            AttackInfo atk_info = attacker.AttackInfoByRandam();

            novel.Add(attacker.sub_name + "：「" + atk_info.name + "！！」");
 
            System.Random r1 = new System.Random();
            int atk = attacker.attack + r1.Next(0,attacker.attack);;

            novel.Add(defender.sub_name + " に " + atk + " のダメージ！");
            if( atk_info.v_effect != null){
                novel.Add("&v_effect " + atk_info.v_effect);
            }
            if( atk_info.s_effect != null){
                novel.Add("&efctsnd " + atk_info.s_effect);
            }
            novel.Add("&shakeimg " +  defender.main_image);
            novel.Add(defender.sub_name + "：" + "「ぐわわ！」");

            string before_hp = defender.hp.ToString();
            defender.hp -= atk;
            novel.Add("&battle dec_hp " + defender.name + " " + atk.ToString());

            if(_quiz_manager.IsQuestion() == true){
                novel.Add("クイズです！ここで、 " + defender.sub_name + " の残り体力はいくらでしょう？");
                novel.Add("体力はもともと " + before_hp + " あり、 " + atk.ToString() + " のダメージを受けたから?" );
                novel.Add(CreateQuestionDialogInstruction(defender, question: before_hp+"-"+atk.ToString()));
                //novel.Add("&dialog " +  (defender.hp + -11).ToString() + " " + (defender.hp).ToString() + " " + (defender.hp + 10).ToString() + " " + ThreeChoiceDialog.ThreeChoiceDialogResult.Choice2);
                novel.Add("答えを選んでね！");
                novel.Add("答えは " + defender.hp.ToString() + " でした!");
            }

            if( defender.hp <= 0 ){//imgefct explosion
                novel.Add(defender.sub_name + " は倒れた！");
                novel.Add("&efctsnd zombievoice1");
                novel.Add(defender.sub_name + "：" + "「ぐええ！」");

                novel.Add("&rmimg " + defender.main_image);
                novel.Add("&battle result");
                novel.Add("&end_battle");
                //EndBattle();
            }else{
                novel.Add("&rmimg " + defender.main_image);
                novel.Add("&battle new_turn");
            }

            _battleTurn ++;

            return novel;
        }

        /// <summary>
        /// 新しいバトルシナリオを自動生成して、UserScriptControllerに返すことで、バトルを進行する
        /// </summary>
        /// <returns></returns>
        public List<string> GetNewSentence()
        {
            /*//テストコード
            List<string> res = new List<string>();
            _battleTurn ++;
            if(_battleTurn < 10){
                res.Add("バトル中！" + _battleTurn);
            }else{
                res.Add("&end_battle");
            }
            return res;
            */

            //開発中の新規ルート
            return BattleOneTurnV2();

            //従来ルート(開発中の新規ルートがうまく行かない場合にすぐ戻れるように、しばらくは従来処理を残しておく。)
            //return BattleOneTurn();
        }

        public void ShowPlayerInfo(string name)
        {
            CharacterModel model = _chars.Find(m => m.name == name);
            if(model != null){
                model.view.ShowAllByInitial();
            }
        }

        public void DecHp(string name, string val)
        {
            CharacterModel model = _chars.Find(m => m.name == name);
            if(model != null){
                model.view.DecHpGauge(int.Parse(val));
            }
        }
    }
}