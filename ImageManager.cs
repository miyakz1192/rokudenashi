using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;  //DOTweenを使うときはこのusingを入れる

namespace NovelGame
{
    public class ImageManager : MonoBehaviour
    {
        [SerializeField] Sprite _background1;
        
        [SerializeField] GameObject _backgroundObject;
        [SerializeField] GameObject _eventObject;
        [SerializeField] GameObject _imagePrefab;

        // テキストファイルから、文字列でSpriteやGameObjectを扱えるようにするための辞書
        Dictionary<string, Sprite> _textToSprite;
        Dictionary<string, GameObject> _textToParentObject;

        // 操作したいPrefabを指定できるようにするための辞書
        Dictionary<string, GameObject> _textToSpriteObject;

        void Awake()
        {
            _textToSprite = new Dictionary<string, Sprite>();
            _textToSprite.Add("background1", _background1);

            _textToParentObject = new Dictionary<string, GameObject>();
            _textToParentObject.Add("backgroundObject", _backgroundObject);
            _textToParentObject.Add("eventObject", _eventObject);

            _textToSpriteObject = new Dictionary<string, GameObject>();

        }

        // 画像を配置する
        public void PutImage(string imageName, string parentObjectName)
        {
            if(_textToSprite.TryGetValue(imageName, out Sprite temp_sprite) == false){
                temp_sprite = Resources.Load<Sprite>("Sprites/" + imageName);
            }

            Sprite image = temp_sprite;
            GameObject parentObject = _textToParentObject[parentObjectName];

            Vector2 position = new Vector2(0, 0);
            Quaternion rotation = Quaternion.identity;
            Transform parent = parentObject.transform;
            GameObject item = Instantiate(_imagePrefab, position, rotation, parent);
            item.GetComponent<Image>().sprite = image;

            _textToSpriteObject.Add(imageName, item);
        }

        // 画像を削除する
        public void RemoveImage(string imageName)
        {
            if(_textToSpriteObject.TryGetValue(imageName, out GameObject prefab)){
                Destroy(prefab);
                _textToSpriteObject.Remove(imageName);
                //TODO: use Resources.UnloadUnusedAssetsを使って、Resource.Loadで読み込んだものを解放する。
            }
        }

        /*
        この機能を使う場合の注意。shakeimgを通じてShakeImageを実行したあと、もう一度同じimageに対してshakeimg(ShakeImage）を実行すると、
        transformオブジェクトがないといったようなエラーになる。

        DOTWEEN ► Target or field is missing/null () ► The object of type 'RectTransform' has been destroyed but you are still trying to access it.
        Your script should either check if it is null or you should not destroy the object.

        このため、一度、shakeimgしたら、そのimgをrmimgして再度同じimgを作ってshakeimgするということを行う必要がある。
        なぜこうなるかはなぞ(dotweenの仕様か・・・？)。

        以下、うまく行くシナリオの記載例。

        &img background1 backgroundObject
        キーンコーンカーンコーン！学校のはじまりです！
        &img fumi_cg1 eventObject //最初のimgの読み込み
        こんにちは１！    
        攻撃1！
        &shakeimg fumi_cg1
        ぐは1！ //ここの文字列表示は重要。shakeimgのアニメーションは別スレッドで実施されるようなので、ここで意図的なwaitがないと、さっさと次のrmimgに進み、エラーになる
        &rmimg fumi_cg1  //shakeimgすると上記の説明のように、一旦rmimgを実施する必要がある。
        &img fumi_cg1 eventObject //あとは最初の繰り返しということ。
        こんにちは2！
        攻撃2！
        &shakeimg fumi_cg1
        ぐは2！
        &rmimg fumi_cg1
        &end

        ※しかし、上記のような対処を行っても、ユーザのクリックのタイミングによっては、（クリックを素早く押しまくっているようなケースでは）、shakeimg中にrmimgが実行されてしまうようで、
        いかのエラーがでる。
        DOTWEEN ► Target or field is missing/null () ► The object of type 'RectTransform' has been destroyed but you are still trying to access it.
        Your script should either check if it is null or you should not destroy the object.

        */
        //画像を揺らす
        public void ShakeImage(string imageName)
        {
            if(_textToSpriteObject.TryGetValue(imageName, out GameObject prefab)){
                Debug.Log("ShakeImage: " + imageName);
                /// 揺れ開始
                /// </summary>
                /// <param name="duration">時間</param>
                /// <param name="strength">揺れの強さ</param>
                /// <param name="vibrato">どのくらい振動するか</param>
                /// <param name="randomness">ランダム度合(0〜180)</param>
                /// <param name="fadeOut">フェードアウトするか</param>
        //		item.transform.DOMove(new Vector2(5f, 0f), 3f);
                var duration = 2.0f;
                var strength = 90.0f;
                var vibrato = 90.0f;
                prefab.transform.DOShakePosition(duration, strength, (int) vibrato, 90.0f, true);
            }
    	}
    }
}
