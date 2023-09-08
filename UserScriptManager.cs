using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//各UI用
using UnityEngine.UI;//UIを扱う際に必要
using TMPro; //TextMeshProを扱う際に必要
using System;

namespace NovelGame
{
    public class UserScriptManager : MonoBehaviour
    {
        [SerializeField] TextAsset _textFile;
        [SerializeField] ThreeChoiceDialog _3cdialog;
        [SerializeField] GameObject _explosion;

        [System.NonSerialized] public int lineNumber;

        // 文章中の文（ここでは１行ごと）を入れておくためのリスト(マスターnovelへのポインタ)
        List<string> _sentences = null;

        NovelManager _novelManager = null;

        void Awake()
        {
            lineNumber = 0;
            _novelManager = new NovelManager(_textFile);
            _sentences = _novelManager.masterNovel;
        }

        public void IncLineNumber()
        {
            lineNumber ++;
        }

        // 現在の行の文を取得する
        public string GetCurrentSentence()
        {
            return _sentences[lineNumber];
        }

        // 文が命令かどうか
        public bool IsStatement(string sentence)
        {
            if (sentence[0] == '&')
            {
                return true;
            }
            return false;
        }

        private void SetMiniCharImage(string mini_char_image_game_obj_name, string image_name){
            GameObject temp = GameObject.Find(mini_char_image_game_obj_name); //words[1] like "CharNameText1"
            Image obj = temp.GetComponent<Image>();
            Sprite temp_sprite = Resources.Load<Sprite>("Sprites/" + image_name);
            obj.sprite = temp_sprite;
            //FIXME: もしすでにミニキャラに画像が登録されていたらそれを解放する処理必要(やっぱりクラス化（プレハブ化？）したほうが良い。)
        }

        private void SetCharNameText(string char_name_text_game_obj_name, string value){
            GameObject temp = GameObject.Find(char_name_text_game_obj_name); //words[1] like "CharNameText1"
            TextMeshProUGUI obj = temp.GetComponent<TextMeshProUGUI>();
            obj.text = "<color=#4169e1>" + value + "</color>";
        }

        private void SetMaxHpSlider(string slider_name, string value){
            GameObject temp1 = GameObject.Find(slider_name); //words[1] like "CharHPSlider1"
            Slider slider1 = temp1.GetComponent<Slider>();
            slider1.maxValue = int.Parse(value);//words[1] like 1000
        }

        private void DecHpSlider(string slider_name, string value){
            //slider のテスト
            GameObject temp2 = GameObject.Find(slider_name); //words[1] like "CharHPSlider1"
            Slider slider2 = temp2.GetComponent<Slider>();
            slider2.value -= int.Parse(value);//words[1] like 1000
        }

        private void SetHpSlider(string slider_name, string value){
            //slider のテスト
            GameObject temp3 = GameObject.Find(slider_name); //words[1] like "CharHPSlider1"
            Slider slider3 = temp3.GetComponent<Slider>();
            slider3.value = int.Parse(value);//words[1] like 1000
        }

        private void Dialog(string choice1, string choice2, string choice3, string correct_choice, string question = "Q-none"){
            // 生成してCanvasの子要素に設定
            var _dialog = Instantiate(_3cdialog);
            var parent = GameObject.Find("Game");
            GameManager.Instance.isAnsweringNow = true;

            GameObject textobj1 = GameObject.Find("3CD_ChoiceText1");
            var text1 = textobj1.GetComponent<TextMeshProUGUI>();
            text1.text = "<color=#4169e1>" + choice1 + "</color>";

            GameObject textobj2 = GameObject.Find("3CD_ChoiceText2");
            var text2 = textobj2.GetComponent<TextMeshProUGUI>();
            text2.text = "<color=#4169e1>" + choice2 + "</color>";

            GameObject textobj3 = GameObject.Find("3CD_ChoiceText3");
            var text3 = textobj3.GetComponent<TextMeshProUGUI>();
            text3.text = "<color=#4169e1>" + choice3 + "</color>";

            _dialog.transform.SetParent(parent.transform, false);
            _dialog.correct_choice = correct_choice;
            // ボタンが押されたときのイベント処理
            _dialog.FixDialog = (result) => {
                Debug.Log("in dialog," +_dialog.correct_choice + "==" + result.ToString());
                Boolean bResult = (_dialog.correct_choice == result.ToString());
                if(bResult){
                    Debug.Log("correct!");
                    GameManager.Instance.soundManager.Effect("correct1");
                    GameManager.Instance.player.total_correct_questions ++;
                }
                GameManager.Instance.player.total_answered_questions ++;
                
                GameManager.Instance.player.AddLog(targetQuestion: question, playerResult: bResult);
                GameManager.Instance.isAnsweringNow = false;
            };
        }

        private void ImageEffect()
        {
            var expl = Instantiate(_explosion);
            var target = GameObject.Find("VisualEffectTarget");
            //var target = GameObject.Find("Event");
            //var target = GameObject.Find("Game");
            //var target = GameObject.Find("Canvas");
            Instantiate(expl, target.transform.position, Quaternion.identity); //パーティクル用ゲームオブジェクト生成
            Destroy(expl); //衝突したゲームオブジェクトを削除            
        }

        private void StartBattle(string charName1 , string charName2, string q_prob)
        {
            GameManager.Instance.battleManager.StartBattle(charName1, charName2, q_prob);
        }

        private void EndBattle()
        {
            GameManager.Instance.battleManager.EndBattle();
        }

        private bool InBattle()
        {
            return GameManager.Instance.battleManager.InBattle();
        }

        private void SubExecuteStatement_Novel(string[] words)
        {
            List<string> temp_novel = null;
            switch(words[1]){
                case "fork":
                    switch(words[2]){
                        case "random":
                            temp_novel = _novelManager.ChoiceNovelByRandom();
                            _sentences.InsertRange(lineNumber+1, temp_novel);
                        break;
                        default:
                            temp_novel = _novelManager.ChoiceNovelById(words[2]);
                            _sentences.InsertRange(lineNumber+1, temp_novel);
                        break;
                    }
                    break;
                default:
                    Debug.Log("no such as SUB instruction" + words[1]);       
                    break;         
            }
        }
        private void SubExecuteStatement_Battle(string[] words)
        {
            if(InBattle()){
                switch(words[1]){
                    case "show_player_info":
                        ShowPlayerInfo(words[2]);
                        break;
                    case "new_turn":
                        Debug.Log("in battle!");
                        List<string> battleSentences = GameManager.Instance.battleManager.GetNewSentence();
                        _sentences.InsertRange(lineNumber+1, battleSentences);
                        break;
                    case "dec_hp":
                        GameManager.Instance.battleManager.DecHp(words[2], words[3]);
                        break;
                    case "result":
                        Player player = GameManager.Instance.player;

                        List<string> temp = new List<string>();
                        temp.Add("本日の結果は、正解数＝" + player.total_correct_questions + "、全問数＝" + player.total_answered_questions + "でした!");
                        temp.Add("お疲れ様でした。よくがんばったね！");
                        temp.Add(player.plan.CurrenctStatusAsNovelText());
                        _sentences.InsertRange(lineNumber+1, temp);
                        break;
                    case "pause":
                        GameManager.Instance.battleManager.Pause();
                        break;
                    case "set_turn_cond":
                        GameManager.Instance.battleManager.SetTurnCondition(int.Parse(words[2]));
                        break;
                    case "reset_turn_cond":
                        GameManager.Instance.battleManager.ResetTurnCondition();
                        break;
                    default:
                        Debug.Log("no such as SUB instruction" + words[1]);
                        break;
                }
            }else{
                switch(words[1]){
                    case "resume":
                        GameManager.Instance.battleManager.Resume();
                        break;
                    default:
                        Debug.Log("no such as SUB instruction" + words[1]);
                        break;
                }
            }
        }

        private void ShowPlayerInfo(string name)
        {
            if(InBattle()){
                 GameManager.Instance.battleManager.ShowPlayerInfo(name);
            }
        }

        private void LoadPlayer(string playerId)
        {
            GameManager.Instance.LoadPlayer(playerId);
        }

        private void SavePlayer()
        {
            GameManager.Instance.SavePlayer();
        }

        // 命令を実行する
        public void ExecuteStatement(string sentence)
        {
            string[] words = sentence.Split(' ');
            switch(words[0])
            {
                case "//":
                    break; //コメント行
                case "&img":
                    GameManager.Instance.imageManager.PutImage(words[1], words[2]);
                    break;
                case "&rmimg":
                    GameManager.Instance.imageManager.RemoveImage(words[1]);
                    break;
                case "&shakeimg":
                    GameManager.Instance.imageManager.ShakeImage(words[1]);
                    break;
                case "&end":
                    GameManager.Instance.FinishGame();
                    break; 
                case "&efctsnd":
                    GameManager.Instance.soundManager.Effect(words[1]);
                    break;
                case "&setmaxhpslider":
                    SetMaxHpSlider(words[1], words[2]);
                    break;
                case "&dechpslider":
                    DecHpSlider(words[1], words[2]);
                    break;
                case "&sethpslider":
                    SetHpSlider(words[1], words[2]);
                    break;
                case "&setcharnametext":
                    SetCharNameText(words[1], words[2]);
                    break;
                case "&setminicharimage":
                    SetMiniCharImage(words[1], words[2]);
                    break;
                case "&dialog"://テスト
                    Dialog(words[1], words[2], words[3], words[4], words[5]); //Choice1,2,3のテキスト
                    break;
                case "&imgefct"://テスト
                    ImageEffect();
                    break;
                case "&battle":
                    SubExecuteStatement_Battle(words);
                    break;
                case "&start_battle":
                    Debug.Log("START battle!");
                    StartBattle(words[1], words[2], words[3]);
                    break;
                case "&end_battle":
                    Debug.Log("END battle!");
                    EndBattle();
                    break;
                case "&finish_game":
                    GameManager.Instance.FinishGame();
                    break;
                case "&v_effect":
                    GameManager.Instance.visualEffectManager.Effect(words[1]);
                    break;
                case "&instant_debug":
                    (new MiscDebug()).Test();
                    break;
                case "&load_player":
                    LoadPlayer(words[1]);//input player_id
                    break;
                case "&save_player":
                    SavePlayer();
                    break;
                case "&novelctl":
                    SubExecuteStatement_Novel(words);
                    break;                    
                default:
                    Debug.Log("no such as instruction" + words[0]);
                    break;
            }
        }
    }
}
