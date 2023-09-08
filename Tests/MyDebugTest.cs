using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert; // AssertはNUnitのではなくUnityのものを使う

using System;
using NovelGame.Log;

namespace Tests{
    public class MyDebugTest{
        [Test]
        public void MyDebugShuldSendingMessage(){
            MyDebug.Log("debug message from uniy rokudenashi!");
        }
    }
}
