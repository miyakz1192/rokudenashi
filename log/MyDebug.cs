using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.IO;

namespace NovelGame.Log{
public class MyDebug 
{
    public static string serverIP = "192.168.122.38"; // サーバーのIPアドレスを指定
    public static int serverPort = 12345; // サーバーのポート番号を指定
    public static void Log(string message){
        UdpClient udpClient = new();
        try{
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpClient.Send(data, data.Length, serverIP, serverPort);
            Debug.Log("メッセージを送信しました: " + message);
        }catch (Exception e){
            Debug.LogError("メッセージの送信中にエラーが発生しました: " + e.Message);
        }
    }

    public static string TempDebug(){
        // Resourcesディレクトリ内のすべてのアセットを読み込む
        UnityEngine.Object[] loadedAssets = Resources.LoadAll("");
        string res = "";

        // 読み込んだアセットをログに表示
        foreach (UnityEngine.Object asset in loadedAssets)
        {
            //Debug.Log("Loaded Asset: " + asset.name);
            res += " " + asset.name;
        }
        return res;
    }
    public static string TempDebugAssets(){
        try{
            string res = "";
            // アセットフォルダのパスを取得
            string assetsFolderPath = Application.dataPath + "/Texts/";

            // アセットフォルダ内のファイルを一覧表示
            // ディレクトリ内のすべてのファイルを取得
            string[] files = Directory.GetFiles(assetsFolderPath);

            // ファイルを表示
            foreach (string file in files){
                    res += " " + file;
            }
            return res;
        }catch(Exception e){
            return "ERROR in TempDebugAssets="  + e.Message + " stack=" + e.ToString();
        }
    }

}
}
