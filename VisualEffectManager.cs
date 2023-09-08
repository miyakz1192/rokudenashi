using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelGame
{
    public class VisualEffectManager : MonoBehaviour
    {
        [SerializeField] GameObject _big_explosion;
        [SerializeField] GameObject _flame_stream;
        [SerializeField] GameObject _plasma_explosion;
        [SerializeField] GameObject _earth_shatter;
        [SerializeField] GameObject _energy_explosion;
        [SerializeField] GameObject _fire_flies;
        [SerializeField] GameObject _goop_spray;
        [SerializeField] GameObject _rocket_trail;
        [SerializeField] GameObject _star3;
        [SerializeField] GameObject _star5;
        [SerializeField] GameObject _charge_01_3_fairy_dust;

        //v_effect命令で指定するVisual Effect名と実際のPrefabのマッピング        
        Dictionary<string, GameObject> _textToVisualEffecter;

        //以下のスケルトンで作られるメソッドは実装していない
        // Start is called before the first frame update
        // Update is called once per frame
        void Awake()
        {
            _textToVisualEffecter = new Dictionary<string, GameObject>();
            _textToVisualEffecter.Add("big_explosion", _big_explosion);
            _textToVisualEffecter.Add("flame_stream", _flame_stream);
            _textToVisualEffecter.Add("plasma_explosion", _plasma_explosion);
            _textToVisualEffecter.Add("earth_shatter", _earth_shatter);
            _textToVisualEffecter.Add("energy_explosion", _energy_explosion);
            _textToVisualEffecter.Add("fire_flies", _fire_flies);
            _textToVisualEffecter.Add("goop_spray", _goop_spray);
            _textToVisualEffecter.Add("rocket_trail", _rocket_trail);
            _textToVisualEffecter.Add("star3", _star3);
            _textToVisualEffecter.Add("star5", _star5);
            _textToVisualEffecter.Add("charge_01_3_fairy_dust", _charge_01_3_fairy_dust);
        }

        public void Effect(string effect_name)
        {
            if(effect_name == "none"){
                return;
            }
            if(_textToVisualEffecter.TryGetValue(effect_name, out GameObject obj) == false){
                Debug.Log("ERROR! no such of visual effect name " + effect_name);
                return;
            }
            var effect = Instantiate(obj);
            var target = GameObject.Find("VisualEffectTarget");
            Instantiate(effect, target.transform.position, Quaternion.identity); //パーティクル用ゲームオブジェクト生成
            Destroy(effect); //衝突したゲームオブジェクトを削除            
        }
    }
}