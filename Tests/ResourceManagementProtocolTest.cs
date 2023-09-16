using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert; // AssertはNUnitのではなくUnityのものを使う

using System;
using NovelGame.Net.Protocol;

namespace Tests{
    public class ResourceManagementProtocolTest{
        public string player_id = "souchan";
        public string sample_log = "sample_log";
        [Test]
        public void ResourceManagementProtocolShuldDownloadLogAsString(){
            ResourceManagementProtocol rmp = ResourceManagementProtocolFactory.Create(player_id);
            string expected = this.sample_log;
            string result = rmp.DownloadLog();
            Debug.Log($"ResourceManagementProtocolShuldDownloadLogAsString = {result}");
            Assert.AreEqual(result, expected);
        }
        [Test]
        public void ResourceManagementProtocolShuldUploadLogAsString(){
            ResourceManagementProtocol rmp = ResourceManagementProtocolFactory.Create(player_id);
            string expected = "uploaded";
            string result;

            rmp.UploadLog(expected);
            result = rmp.DownloadLog();
            Debug.Log($"ResourceManagementProtocolShuldUploadLogAsString[1] upload({expected}) -> download({result})");
            Assert.AreEqual(result, expected);

            rmp.UploadLog(this.sample_log);//もとに戻しておく
            result = rmp.DownloadLog();
            expected = this.sample_log;
            Debug.Log($"ResourceManagementProtocolShuldUploadLogAsString[2] upload({this.sample_log}) -> download({result})");
            Assert.AreEqual(result, expected);
        }
    }
}
