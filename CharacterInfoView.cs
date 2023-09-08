using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//各UI用
using UnityEngine.UI;//UIを扱う際に必要
using TMPro; //TextMeshProを扱う際に必要

namespace NovelGame
{
    public class CharacterInfoView
    {
        //リソース管理情報
        private string _miniCharImageObj = "MiniCharImage"; //MiniCharImage* , キャラクタのサブネイル画像を収めるImageオブジェクトでMiniCharImage1~2
        private string _charNameTextObj = "CharNameText"; //CharNameText*, キャラクタ名のテクストオブジェクトで、CharNameText1~2
        private string _charHPSliderObj = "CharHPSlider"; //CharHPSlider*, キャラクタのHPを表現するスライダーで、CharHPSlider1~2
        private string _windowIndexStr = "1"; //デフォルトで1番目
        public CharacterModel model = null; //表示する対象のモデル

        private void SetMiniCharImageAux(string mini_char_image_game_obj_name, string image_name){
            GameObject temp = GameObject.Find(mini_char_image_game_obj_name); //words[1] like "CharNameText1"
            Image obj = temp.GetComponent<Image>();
            Sprite temp_sprite = Resources.Load<Sprite>("Sprites/" + image_name);
            obj.sprite = temp_sprite;
            //FIXME: もしすでにミニキャラに画像が登録されていたらそれを解放する処理必要(やっぱりクラス化（プレハブ化？）したほうが良い。)
        }

        private void SetCharNameTextAux(string char_name_text_game_obj_name, string value){
            GameObject temp = GameObject.Find(char_name_text_game_obj_name); //words[1] like "CharNameText1"
            TextMeshProUGUI obj = temp.GetComponent<TextMeshProUGUI>();
            obj.text = "<color=#4169e1>" + value + "</color>";
        }

        private void SetMaxHpSliderAux(string slider_name, int value){
            GameObject temp1 = GameObject.Find(slider_name); //words[1] like "CharHPSlider1"
            Slider slider1 = temp1.GetComponent<Slider>();
            slider1.maxValue = value;
        }

        private void DecHpSliderAux(string slider_name, int value){
            //slider のテスト
            GameObject temp2 = GameObject.Find(slider_name); //words[1] like "CharHPSlider1"
            Slider slider2 = temp2.GetComponent<Slider>();
            slider2.value -= value;
        }

        private void SetHpSliderAux(string slider_name, int value){
            //slider のテスト
            GameObject temp3 = GameObject.Find(slider_name); //words[1] like "CharHPSlider1"
            Slider slider3 = temp3.GetComponent<Slider>();
            slider3.value = value;
        }

        public void SetMiniCharImage()
        {
            SetMiniCharImageAux(_miniCharImageObj, model.main_image);
        }

        public void SetCharNameText()
        {
            SetCharNameTextAux(_charNameTextObj, model.main_name + "(" + model.sub_name + ")");
        }

        public void SetMaxHpGauge()
        {
            SetMaxHpSliderAux(_charHPSliderObj, model.hp);
        }

        public void DecHpGauge(int value)
        {
            DecHpSliderAux(_charHPSliderObj, value);
        }

        public void SetHpGauge(int value)
        {
            SetHpSliderAux(_charHPSliderObj, value);
        }

        public void ShowAllByInitial()
        {
            SetMiniCharImage();
            SetCharNameText();
            SetMaxHpGauge();
            SetHpGauge(model.hp);
        }

        public void SetWindowIndex(int index)
        {
            _windowIndexStr = index.ToString();

            _miniCharImageObj = _miniCharImageObj + _windowIndexStr;
            _charNameTextObj = _charNameTextObj + _windowIndexStr;
            _charHPSliderObj = _charHPSliderObj + _windowIndexStr;
        }
    }
}