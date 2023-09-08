using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert; // AssertはNUnitのではなくUnityのものを使う

using System;
using NovelGame.Net.Protocol;

namespace Tests{
    public class HttpClientTest{
        public string url = "http://192.168.122.38:8000/";
        public byte[] binaryHex = new byte[] { 0xde, 0xad, 0xbe, 0xaf };

        /*[UnityTest]
        public IEnumerator SecondTest(){
            HttpClient c = new HttpClient();
            //c.Download("https://www.google.com");
            return c._DownloadText("http://192.168.122.38");
        }
        */

        protected void DownloadShuldDownloadText_Common(string target, string expected = null){
            if(expected == null){
                expected = target;
            }
            Debug.Log($"Downloadメソッドが、サーバの{target}をダウンロードしてstringとして得られる");
            HttpClient c = new HttpClient();
            c.Download(this.url + target);
            Debug.Log($"Download Result={c.buf_string}");
            Assert.AreEqual(c.buf_string, expected);
        }

        protected string ByteArrayToString(byte[] array){
            string res = "";
            foreach (byte b in array){
                res += b.ToString("X2") + " "; // 16進数表記で表示（2桁表示）
            }
            return res;
        } 
        [Test]
        public void DownloadShuldDownloadTextASCIIAlphabet(){
            string target = "DownloadShuldDownloadTextASCIIAlphabet.txt";
            this.DownloadShuldDownloadText_Common(target);
        }

        [Test]
        public void DownloadShuldDownloadTextUTF8Japanese(){
            string target = "DownloadShuldDownloadTextUTF8Japanese.txt";
            this.DownloadShuldDownloadText_Common(target, "日本語");
        }

        [Test]
        public void DownloadShuldDownloadBinaryFile(){
            string target = "DownloadShuldDownloadBinaryFile.txt";
            byte []expected = this.binaryHex;
            Debug.Log($"Downloadメソッドが、サーバの{target}をダウンロードしてbyte[]として得られる");
            HttpClient c = new HttpClient();
            c.Download(this.url + target);
            Debug.Log($"Download Result={BitConverter.ToString(c.buf_byte)}");

            Debug.Log($"c.buf_byte array contents = {this.ByteArrayToString(c.buf_byte)}");
            Debug.Log($"expected array contents = {this.ByteArrayToString(expected)}");

            for(int i = c.buf_byte.Length; i < c.buf_byte.Length ; i++){
                Assert.AreEqual(c.buf_byte[i], expected[i]);
            }
        }

    }
}
