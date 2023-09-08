using System;
using UnityEngine;

namespace NovelGame
{
    public class ThreeChoiceDialog : MonoBehaviour
    {
        public enum ThreeChoiceDialogResult
        {
            Choice1,
            Choice2,
            Choice3,
            Cancel
        }
        //メモ:
        //C#の場合は、ThreeChoiceDialogResult型の変数をToString()すると、"Choise1","Choise2"といった文字列に変換されるので便利。
        
        // ダイアログが操作されたときに発生するイベント
        public Action<ThreeChoiceDialogResult> FixDialog { get; set; }
        public string correct_choice;
        
        private void OnChoiseCommon(ThreeChoiceDialogResult res){
            this.FixDialog?.Invoke(res);
            Destroy(this.gameObject);
        }

        // Choice1ボタンが押されたとき
        public void OnChoice1()
        {
            this.OnChoiseCommon(ThreeChoiceDialogResult.Choice1);
        }
        // Choice2ボタンが押されたとき
        public void OnChoice2()
        {
            this.OnChoiseCommon(ThreeChoiceDialogResult.Choice2);
        }
        // Choice3ボタンが押されたとき
        public void OnChoice3()
        {
            this.OnChoiseCommon(ThreeChoiceDialogResult.Choice3);
        }
        // ダイアログボックス外の背景を押下されたとき
        public void OnCancel()
        {
            //キャンセルは不可とする。下をコメントアウト。
            //this.OnChoiseCommon(ThreeChoiceDialogResult.Cancel);
        }
    }
}