using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerWallet
{
    public int gold;

    public static PlayerWallet GetWallet(string jsonString)
    {
        return JsonUtility.FromJson<PlayerWallet>(jsonString);
    }
}


[Serializable]
public class RootRoomList
{
    public RoomList[] roomlist;
    public static RootRoomList GetRoomList(string jsonString)
    {
        return JsonUtility.FromJson<RootRoomList>("{\"roomlist\":" + jsonString + "}");
    }
}


[Serializable]
public class RoomList
{
    public string match_id;
    public string authoritative;
    public Label label;
}

[Serializable]
public class Label
{
    public string value;
}


[Serializable]
public class JoinDetail
{
    public int NumSitUser;

    public int BetRate;

    public int MaxBetRate;

    public UserSit[] UserSit;

    public bool RoomPlaying;

    public static JoinDetail GetJoinDetail(string jsonString)
    {
        return JsonUtility.FromJson<JoinDetail>(jsonString);
    }
}

[Serializable]
public class UserSit
{
    public string UID;
    public string Name;
    public int Position;
    public string ProfilePic;

    public static UserSit GetUserSit(string jsonString)
    {
        return JsonUtility.FromJson<UserSit>(jsonString);
    }
}

[Serializable]
public class PlayerLeave
{
    public int Position;
    public static PlayerLeave GetUserSit(string jsonString)
    {
        return JsonUtility.FromJson<PlayerLeave>(jsonString);
    }
}

[Serializable]
public class SetDealer
{
    public int Position;
    public bool IsDealerBot;
    public int DealerTurnCount;
    public string DealerName;
    public static SetDealer GetDealer(string jsonString)
    {
        return JsonUtility.FromJson<SetDealer>(jsonString);
    }
}


[Serializable]
public class UserBet
{
    public int Position;
    public int BetRate;
    public static UserBet GetUserBet(string jsonString)
    {
        return JsonUtility.FromJson<UserBet>(jsonString);
    }
}


[Serializable]
public class OnChangeTurn
{
    public string UID;
    public int Position;
    public int TurnTime;
    public bool IsDealerTurn;
    public static OnChangeTurn GetUserTurn(string jsonString)
    {
        return JsonUtility.FromJson<OnChangeTurn>(jsonString);
    }
}


[Serializable]
public class DealCard
{
    public int Position;
    public int CardResult;
    public int PointMultiply;
    public int Point;
    public HandCard[] HandCard;
    public string[] PlayerList;
    public static DealCard GetHandCard(string jsonString)
    {
        return JsonUtility.FromJson<DealCard>(jsonString);
    }

}

[Serializable]

public class HandCard
{
    public int CardID;
}


[Serializable]
public class PlayerJua
{
    public string UID;
    public int Position;
    public int CardID;
    public int CardResult;
    public int PointMultiply;
    public int Point;
    public static PlayerJua GetPlayerJua(string jsonString)
    {
        return JsonUtility.FromJson<PlayerJua>(jsonString);
    }

}


[Serializable]
public class PlayerNotJua
{
    public string UID;
    public int Position;
    public static PlayerNotJua GetPlayerNotJua(string jsonString)
    {
        return JsonUtility.FromJson<PlayerNotJua>(jsonString);
    }

}


[Serializable]
public class PlayerResult
{
    public int Result;
    public int Chip;
    public int CurrentChip;
    public static PlayerResult GetResult(string jsonString)
    {
        return JsonUtility.FromJson<PlayerResult>(jsonString);
    }
}



[Serializable]
public class DealerResult
{
    public string[] DealerWinName;
    public string[] DealerLostName;
    public int DealerWinTotal;
    public int DealerLostTotal;
    public int DealerChipGenTotal;
    public int CurrentChip;
    public static DealerResult GetResult(string jsonString)
    {
        return JsonUtility.FromJson<DealerResult>(jsonString);
    }
}

[Serializable]
public class DealerWinName
{
    public string name;
}

[Serializable]
public class DealerLostName
{
    public string name;
}


[Serializable]
public class DealerJub
{
    public int NumJub;
    public static DealerJub GetJub(string jsonString)
    {
        return JsonUtility.FromJson<DealerJub>(jsonString);
    }
}

[Serializable]
public class PlayerJub
{
    public int Position;
    public static PlayerJub GetPlayerJub(string jsonString)
    {
        return JsonUtility.FromJson<PlayerJub>(jsonString);
    }
}

[Serializable]
public class IAPList
{
    public ProductGame[] product;
    public static IAPList GetDetail(string jsonString)
    {
        return JsonUtility.FromJson<IAPList>(jsonString);
    }
} 

[Serializable]
public class ProductGame
{
    public string ProductID;
    public string ProductNameText;
    public int Gold;
    public int Diamond;
    public int Bonus;
}