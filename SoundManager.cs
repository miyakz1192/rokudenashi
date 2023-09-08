using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelGame
{
    public class SoundManager : MonoBehaviour
    {

        Dictionary<string, AudioClip> _textToAudioClipObject;

        void Awake()
        {
            _textToAudioClipObject = new Dictionary<string, AudioClip>();
        }    

        /// <summary>
        /// 効果音を鳴らす
        /// </summary>
        /// <param name="rsc_name">効果音名(Assets/Resources配下に存在する、拡張子を除くサウンドファイル名)</param>
        public void Effect(string rsc_name)
        {
            Debug.Log("start play one shot with " + rsc_name);
            if(_textToAudioClipObject.TryGetValue(rsc_name, out AudioClip ac) == false){
                Debug.Log("Load AudioClip with " + rsc_name);
                ac = Resources.Load<AudioClip>("AudioClip/" + rsc_name);
            }

            Debug.Log(System.Environment.GetEnvironmentVariable("PULSE_SERVER"));
            GameObject st = GameObject.Find("AudioSource");
            AudioSource audioSource = st.GetComponent<AudioSource>();
            audioSource.PlayOneShot(ac);
            Debug.Log("end play one shot with " + rsc_name);

            if(_textToAudioClipObject.TryGetValue(rsc_name, out AudioClip ac_temp) == false){
                _textToAudioClipObject.Add(rsc_name, ac);
            }
        }
    }
}
