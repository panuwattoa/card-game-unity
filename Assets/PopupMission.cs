using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PopupMission : MonoBehaviour
{
    [SerializeField] GameObject[] clear;
    [SerializeField] TextMeshProUGUI[] gold;
    [SerializeField] Slider slider;
    [SerializeField] Button buttonReward;
    [SerializeField] TextMeshProUGUI current;
    // Start is called before the first frame update
    void Awake()
    {
        Setup();
    }


    public void OnClickReward()
    {
        buttonReward.interactable = false;
        Reward();
    }

    void Setup()
    {
        buttonReward.interactable = false;

        for (int i = 0; i < clear.Length; i++)
        {
            clear[i].SetActive(false);
        }
        var data = GameManager.Instance.loginRequestData.playRewardData;
        var userData = GameManager.Instance.UserData;
        current.text = userData.current_play.ToString();
        slider.value = userData.current_play;
        if (data != null)
        {
            if (data.reward.Length > 0)
            {
                for (int i = 0; i < data.reward.Length; i++)
                {
                    if (gold.Length >= data.reward.Length)
                    {
                        gold[i].text = string.Format("{0:n0}", data.reward[i].gold);
                    }
                    if (userData.current_play >= data.reward[i].num_round)
                    {
                        if (userData.current_play_rewarded < data.reward[i].num_round)
                        {
                            buttonReward.interactable = true;
                        }
                        else
                        {
                            if (clear.Length >= data.reward.Length)
                            {
                                clear[i].SetActive(true);
                            }
                        }
                    }
                }
            }
        }
    }

    async void Reward()
    {
        try
        {
            var reward = await GameApi.ReqestClaimPlayReward();
            GameManager.Instance.SetUserData(UserData.GetDetail(reward));
            Setup();
            popupMessage.Create("ระบบ", "ได้รับรางวัลแล้ว");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            popupMessage.Create("ระบบ", "ผิดพลาด");

        }
    }
    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }
}
