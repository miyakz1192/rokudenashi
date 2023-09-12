using UnityEngine;

using NovelGame.Log;
using System.Threading;

namespace NovelGame
{
    public class GameManager : MonoBehaviour
    {
        // 別のクラスからGameManagerの変数などを使えるようにするためのもの。（変更はできない）
        public static GameManager Instance { get; private set; }

        public UserScriptManager userScriptManager;
        public MainTextController mainTextController;
        public ImageManager imageManager;
        public SoundManager soundManager;
        public BattleManager battleManager;
        public VisualEffectManager visualEffectManager;
        public Player player = new Player();

        public bool isAnsweringNow = false;//ダイアログで回答中かどうか。

        // ユーザスクリプトの、今の行の数値。クリック（タップ）のたびに1ずつ増える。
        //[System.NonSerialized] public int lineNumber;

        void Awake()
        {
            // これで、別のクラスからGameManagerの変数などを使えるようになる。
            Instance = this;
            //lineNumber = 0;
            MyDebug.Log("TempDebug=" + MyDebug.TempDebug());
        }

        public void FinishGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;   // UnityEditorの実行を停止する処理
#else
            Application.Quit();                                // ゲームを終了する処理
#endif
        }

        public void LoadPlayer(string player_id)
        {
            Debug.Log("loading player = " + player_id);
            //まず、player_idで読み込んで見る
            Player p = Player.Load(player_id);
            if( p != null){
                Debug.Log("loading player ok");
                this.player = p;
                this.player.LoadAllData();
            }
        }

        public void SavePlayer()
        {
            this.player.SaveAllData();
        }
    }
}
