using System.Threading.Tasks;
using Scripts.Session;
using UnityEngine;
using Nakama.TinyJson;
using System.Text;
using System;

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

    public static async Task<string> ClaimVideoAdsReward()
    {
        var payload = "";
        var rpcid = "request_claim_video_reward";
        var response = await RpcAsync(payload, rpcid);
        return response.Payload;
    }

    public static async Task<Nakama.IApiRpc> CheckAdAvaliable()
    {
        var payload = "";
        var rpcid = "request_check_video_reward";
        var response = await RpcAsync(payload, rpcid);
        return response;
    }

    public static async Task<string> CheckIAPPayload(string receipt)
    {
        //Debug.Log("receipt " + payload);
        var rpcid = "request_payment_apple";
#if UNITY_ANDROID

        rpcid = "request_payment_goolge";
        var resp = IAPGoolge.GetDetail(receipt);
        var response = await RpcAsync(resp.Payload, rpcid);
        Debug.Log("playload sss" + resp);
        return response.Payload;
#endif
    }

    public static async Task<string> CheckSepcialIAP()
    {
        var payload = "";
        var rpcid = "request_check_special_iap";
        var response = await RpcAsync(payload, rpcid);
        return response.Payload;
    }

    public static async Task<string> ReqestIAPlist()
    {
        var payload = "";
        var rpcid = "request_iap_list";
        var response = await RpcAsync(payload, rpcid);
        return response.Payload;
    }

    public static async Task<string> BuySpecail()
    {
        var payload = "";
        var rpcid = "buy_special";
        var response = await RpcAsync(payload, rpcid);
        return response.Payload;
    }

}


