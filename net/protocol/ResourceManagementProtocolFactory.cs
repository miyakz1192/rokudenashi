using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelGame.Net.Protocol{
public class ResourceManagementProtocolFactory
{
    public static ResourceManagementProtocol Create(string player_id){
#if UNITY_EDITOR//テスト環境
        string root_url = Resources.Load<TextAsset>("root_url_for_test").text;
        string player_group_uuid = Resources.Load<TextAsset>("player_group_uuid_for_test").text;
#else//本番環境
        string root_url = Resources.Load<TextAsset>("root_url").text;
        string player_group_uuid = Resources.Load<TextAsset>("player_group_uuid").text;
#endif
        return new ResourceManagementProtocol(root_url, player_group_uuid, player_id);
    }
}
}
