using NovelGame.Net.Protocol;
using UnityEngine;

namespace NovelGame.ResourceManagement{
public class PlayerLogAtServer
{
    public string playerId;
    public PlayerLogAtServer(string player_id){
        this.playerId = player_id;
    }
    public PlayLog Load(){
        ResourceManagementProtocol rmp = ResourceManagementProtocolFactory.Create(this.playerId);
        return JsonUtility.FromJson<PlayLog>(rmp.DownloadLog());
    }
    public void Save(PlayLog playLog){
        if(playLog.playerId != this.playerId){
            throw new System.Exception($"ERROR: FAITAL: playLog.playerId({playLog.playerId}) != this.playerId({this.playerId})");
        }
        ResourceManagementProtocol rmp = ResourceManagementProtocolFactory.Create(playLog.playerId);
        rmp.UploadLog(JsonUtility.ToJson(playLog));
    }
}
}
