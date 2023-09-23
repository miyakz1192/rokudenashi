using NovelGame.Net.Protocol;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert; // AssertはNUnitのではなくUnityのものを使う

namespace Tests{
public class ResourceManagementProtocolFactoryTest
{
    [Test]
    public void ResourceManagementProtocolFactoryShouldReturnNewInstance(){
        ResourceManagementProtocol rmp = ResourceManagementProtocolFactory.Create("testplayer");
        Assert.AreEqual(rmp, rmp);
    }
}
}