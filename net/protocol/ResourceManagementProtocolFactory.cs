using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelGame.Net.Protocol{
public class ResourceManagementProtocolFactory
{
    public static ResourceManagementProtocol Create(string player_id){
        string root_url = Resources.Load<TextAsset>("root_url").text;
        string player_group_uuid = Resources.Load<TextAsset>("player_group_uuid").text;
        return new ResourceManagementProtocol(root_url, player_group_uuid, player_id);
    }
}
}
