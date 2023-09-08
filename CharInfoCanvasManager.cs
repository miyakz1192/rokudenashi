using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelGame
{
    public class CharInfoCanvasManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            GetComponent<RectTransform>().SetAsLastSibling();
        }
    }
}