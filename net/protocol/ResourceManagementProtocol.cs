using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelGame.Net.Protocol{
public class ResourceManagementProtocol
{
    //DownloadLog(host, player_group_uuid, player_id) returns string サーバに配置したログをstringで返す。
    //UploadLog(player_group_uuid, player_id, string, mode): Logをappendする(modeで指定。現時点はappendのみ)

    public string root_url;
    public string player_group_uuid;
    public string player_id;
    //overrideRootUrlが設定されている場合、コンストラクタの引数root_urlは無視され、こちらが優先される(主にUnitTest用)
    public static string overrideRootUrl = null;
    public ResourceManagementProtocol(string root_url, string player_group_uuid, string player_id){
        if(ResourceManagementProtocol.overrideRootUrl != null){
            this.root_url = ResourceManagementProtocol.overrideRootUrl;
        }else{
            this.root_url = root_url;
        }
        this.player_group_uuid = player_group_uuid;
        this.player_id = player_id;
    }
    protected string GetFullUrl(){
        return this.root_url + "/" + this.player_group_uuid + "/" + this.player_id + "/log";
    }
    /**
      root_url: http://192.168.1.1:8080/までのURL
    */
    public string DownloadLog(){
        HttpClient http = new HttpClient();
        http.Download(GetFullUrl());
        return http.buf_string;
    }

    public void UploadLog(string content){
        HttpClient http = new HttpClient();
        http.Upload(GetFullUrl(), content);
    }
}
}
