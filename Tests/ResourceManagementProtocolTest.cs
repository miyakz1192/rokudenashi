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
        public void ResourceManagementProtocolShuldUploadDownloadLogAsString(){
            ResourceManagementProtocol rmp = ResourceManagementProtocolFactory.Create(player_id);
            string expected = "uploaded";
            string result;

            rmp.UploadLog(expected);
            result = rmp.DownloadLog();
            Debug.Log($"ResourceManagementProtocolShuldUploadDownloadLogAsString[1] upload({expected}) -> download({result})");
            Assert.AreEqual(result, expected);

            rmp.UploadLog(this.sample_log);//もとに戻しておく
            result = rmp.DownloadLog();
            expected = this.sample_log;
            Debug.Log($"ResourceManagementProtocolShuldUploadDownloadLogAsString[2] upload({this.sample_log}) -> download({result})");
            Assert.AreEqual(result, expected);
            Debug.Log("ResourceManagementProtocolShuldUploadDownloadLogAsStrin DONE");
        }
    }
}
