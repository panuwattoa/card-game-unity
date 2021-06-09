using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardPanel : MonoBehaviour
{
    [SerializeField] private DailyRewardIcon[] icon;
    // Start is called before the first frame update
    void Awake()
    {
        Setup();
    }

    void Setup()
    {
        var data = GameManager.Instance.loginRequestData.dailyRewardData;
        var userData = GameManager.Instance.UserData;
        if (data != null)
        {
            if (data.reward.Length > 0)
            {
                for (int i = 0; i < data.reward.Length; i++)
                {
                    if (icon.Length >= i)
                    {
                        icon[i].Gold.text = string.Format("{0:n0}", data.reward[i]);
                        icon[i].Focus.SetActive(true);
                        if (userData.num_daily_login == i && !userData.is_recived)
                        {
                            icon[i].Focus.SetActive(true);
                            icon[i].button.interactable = true;
                        }
                        else if (userData.num_daily_login > i)
                        {
                            icon[i].Clear.SetActive(true);
                            icon[i].Focus.SetActive(false);
                            icon[i].button.interactable = false;
                        }
                        else
                        {
                            icon[i].Focus.SetActive(false);
                            icon[i].button.interactable = false;
                        }
                    }

                }
            }
        }
    }

    public void OnClickReward()
    {
        Reward();
    }

    async void Reward()
    {
        try
        {
            var p = await GameApi.ReqestDailyReward();
            GameManager.Instance.SetUserData(UserData.GetDetail(p));
            Setup();
            popupMessage.Create("ระบบ", "ได้รับรางวัลแล้ว");
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            popupMessage.Create("ระบบ", "ผิดพลาด");
        }

    }
    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }
}
