using System.Threading.Tasks;
using Scripts.Session;
using UnityEngine;
using Nakama.TinyJson;

public class GameApi 
{

    public static async Task<string> GetMatchWithValue(string value, string uid)
    {
        var payload = "{\"gold\": \"" + value + "\",\"uid\": \"" + uid + "\" }";
        var rpcid = "get_matches";
        //var payload = "{\"gold\": \"dragonite\",\"uid\": \"dragonite\"}";
        Debug.Log("playload "+ payload);
        var response = await RpcAsync(payload, rpcid);
        return response.Payload;
    }


    public static async Task<string> CheckVersion(int value)
    {
        var payload = "{\"version\": "+ value +" }";
        var rpcid = "check_version";
        //var payload = "{\"gold\": \"dragonite\",\"uid\": \"dragonite\"}";
        Debug.Log("playload " + payload);
        var response = await RpcAsync(payload, rpcid);
        return response.Payload;
    }

    public static async Task<string> GetRoomOnline(string value)
    {
        var payload = "{\"gold\": \"" + value + "\"}";
        var rpcid = "get_room_online";
        var response = await RpcAsync(payload, rpcid);
        Debug.Log("playload " + response.Payload);
        return response.Payload;
    }

    private static async Task<Nakama.IApiRpc> RpcAsync(string payload,string rpcid)
    {

        //var rpcid = "get_pokemon";
        var nakama = NakamaSessionManager.Instance;
        var pokemonInfo = await nakama.Client.RpcAsync(nakama.Session, rpcid, payload);
        Debug.LogFormat("Retrieved pokemon info: {0}", pokemonInfo);
        return pokemonInfo;
    }
}


