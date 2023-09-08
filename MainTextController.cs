using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;

namespace NovelGame
{
    public class MainTextController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _mainTextObject;
        int _displayedSentenceLength;
        int _sentenceLength;
        float _time;
        float _feedTime;
        bool _esc_flg = false; //ESC(ゲーム終了)が押下されたかどうか

        // Start is called before the first frame update
        void Start()
        {
            _time = 0f;
            _feedTime = 0.05f;

            // 最初の行のテキストを表示、または命令を実行
            string statement = GameManager.Instance.userScriptManager.GetCurrentSentence();
            if (GameManager.Instance.userScriptManager.IsStatement(statement))
            {
                GameManager.Instance.userScriptManager.ExecuteStatement(statement);
                GoToTheNextLine();
            }
            DisplayText();
        }

        // Update is called once per frame
        void Update()
        {
            if(_esc_flg) {
                Thread.Sleep(3000);
                GameManager.Instance.FinishGame();
            }
            // 文章を１文字ずつ表示する
            _time += Time.deltaTime;
            if (_time >= _feedTime)
            {
                _time -= _feedTime;
                if (!CanGoToTheNextLine())
                {
                    _displayedSentenceLength++;
                    _mainTextObject.maxVisibleCharacters = _displayedSentenceLength;
                }
            }

            // クリックされたとき、次の行へ移動
            if (Input.GetMouseButtonUp(0) && GameManager.Instance.isAnsweringNow == false)
            {
                if (CanGoToTheNextLine())
                {
                    GoToTheNextLine();
                    DisplayText();
                }
                else
                {
                    _displayedSentenceLength = _sentenceLength;
                }
            }

            //ESCが押されたとき、ゲームを終了
            if(Input.GetKey(KeyCode.Escape))
            {
                string disp = "ESCが押されましたので、ゲームを終わります！";
                _mainTextObject.text = disp;
                _mainTextObject.maxVisibleCharacters = disp.Length;
                _esc_flg = true; //とりあえずフラグを立てて関数を終了することで、次回のUpdate呼び出し時にdispを表示できるようにする。
            }
        }

        // その行の、すべての文字が表示されていなければ、まだ次の行へ進むことはできない
        public bool CanGoToTheNextLine()
        {
            string sentence = GameManager.Instance.userScriptManager.GetCurrentSentence();
            _sentenceLength = sentence.Length;
            return (_displayedSentenceLength > sentence.Length);
        }

        // 次の行へ移動
        public void GoToTheNextLine()
        {
            _displayedSentenceLength = 0;
            _time = 0f;
            _mainTextObject.maxVisibleCharacters = 0;
            //GameManager.Instance.lineNumber++;
            GameManager.Instance.userScriptManager.IncLineNumber();
            string sentence = GameManager.Instance.userScriptManager.GetCurrentSentence();
            if (GameManager.Instance.userScriptManager.IsStatement(sentence))
            {
                GameManager.Instance.userScriptManager.ExecuteStatement(sentence);
                GoToTheNextLine();
            }
        }

        // テキストを表示
        public void DisplayText()
        {
            string sentence = GameManager.Instance.userScriptManager.GetCurrentSentence();
            _mainTextObject.text = sentence;
        }
    }
}
