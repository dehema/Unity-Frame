using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedeemGiftPanel : BaseUIForms
{
    public Button HomeButton;
    public Button AddressButton;
    public Button RandomButton;
    public Button InstructionsButton;
    public Transform Container;

    private Puzzle randomPuzzle;

    // Start is called before the first frame update
    void Start()
    {
        InstructionsButton.onClick.AddListener(() =>
        {
            UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("RewardsGiftInstructionPanel"));
        });

        HomeButton.onClick.AddListener(() =>
        {
            CloseUIForm(SOHOShopUtil.PanelName(GetType().Name));
        });
        AddressButton.onClick.AddListener(() =>
        {
            UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("AddressPanel"));
        });

        RandomButton.onClick.AddListener(() =>
        {
            if (randomPuzzle != null)
            {
                ADMgr.Ins.playRewardVideo((success) =>
                {
                    SOHOShopManager.Ins.AddRewardPuzzle(randomPuzzle);
                    refreshList();
                }, "110");

            }
        });
    }

    public override void Display()
    {
        base.Display();

        refreshList();

        // 打点
        PostEventScript.GetInstance().SendEvent("1303");
    }

    private void refreshList()
    {
        randomPuzzle = SOHOPuzzleManager.instance.getUnachievedPuzzle();
        if (randomPuzzle == null)
        {
            RandomButton.gameObject.SetActive(false);
        }
        else
        {
            RandomButton.gameObject.SetActive(true);
        }

        int childCount = Container.childCount;
        List<GameObject> removeObjList = new List<GameObject>();
        if (childCount > 0)
        {
            for (int i = 0; i < childCount; i++)
            {
                if (!Container.GetChild(i).gameObject.CompareTag("DonotRemoveItem"))
                {
                    removeObjList.Add(Container.GetChild(i).gameObject);
                }
            }
        }
        if (removeObjList.Count > 0)
        {
            foreach (GameObject obj in removeObjList)
            {
                Destroy(obj);
            }
        }


        GameObject prefab = Resources.Load<GameObject>("SOHOShop/UIPanel/" + (CommonUtil.IsPortrait() ? "Portrait" : "Landscape") + "/RedeemGiftItem");
        foreach (Puzzle item in SOHOPuzzleManager.instance.puzzleShopGroup)
        {
            GameObject obj = Instantiate(prefab, Container);
            obj.transform.Find("TitleText").GetComponent<Text>().text = item.name;
            obj.transform.Find("RewardImage").GetComponent<Image>().sprite = Resources.Load<Sprite>(item.reward_img);
            obj.transform.Find("PuzzleSliderBG/PuzzleSlider").GetComponent<Image>().fillAmount = (float)item.claim_count / item.sum_count;
            obj.transform.Find("PuzzleSliderBG/Text").GetComponent<Text>().text = NumberUtil.DoubleToStr(item.claim_count) + "/" + item.sum_count;
        }

        Container.GetComponent<RectTransform>().sizeDelta = new Vector2(Container.GetComponent<RectTransform>().sizeDelta.x, 236 * (SOHOPuzzleManager.instance.puzzleShopGroup.Length + 1) / 3);
    }
}
