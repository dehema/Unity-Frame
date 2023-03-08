using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour
{
    void Start()
    {

    }


    void Update()
    {

    }

    /// <summary>
    /// 弹窗出现效果
    /// </summary>
    /// <param name="PopBarUp"></param>
    public static void PopShow(GameObject PopBarUp, System.Action finish)
    {
        /*-------------------------------------初始化------------------------------------*/
        PopBarUp.GetComponent<CanvasGroup>().alpha = 0;
        PopBarUp.transform.localScale = new Vector3(0, 0, 0);
        /*-------------------------------------动画效果------------------------------------*/
        PopBarUp.GetComponent<CanvasGroup>().DOFade(1, 0.3f);
        PopBarUp.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            if (finish != null)
            {
                finish();
            }
        });
    }

    /// <summary>
    /// 弹窗消失效果
    /// </summary>
    /// <param name="PopBarDisapper"></param>
    public static void PopHide(GameObject PopBarDisapper, System.Action finish)
    {
        /*-------------------------------------初始化------------------------------------*/
        PopBarDisapper.GetComponent<CanvasGroup>().alpha = 1;
        PopBarDisapper.transform.localScale = new Vector3(1, 1, 1);
        /*-------------------------------------动画效果------------------------------------*/
        PopBarDisapper.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        PopBarDisapper.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            if (finish != null)
            {
                finish();
            }
        });
    }

    /// <summary>
    /// 数字变化动画
    /// </summary>
    /// <param name="startNum">初始值</param>
    /// <param name="endNum">结束值</param>
    /// <param name="text">索要更改的文本</param>
    /// <param name="finish">结束回调</param>
    public static void ChangeNumber(int startNum, int endNum, float delay, Text text, System.Action finish)
    {
        DOTween.To(() => startNum, x => text.text = x.ToString(), endNum, 0.5f).SetDelay(delay).OnComplete(() =>
        {
            if (finish != null)
            {
                finish();
            }
        });
    }

    /// <summary>
    /// 进度条动画
    /// </summary>
    /// <param name="startValue">初始值</param>
    /// <param name="endValue">结束值</param>
    /// <param name="sliderImage">进度条图片</param>
    /// <param name="finish">结束回调</param>
    public static void ChangeSliderValue(float startValue, float endValue, Image sliderImage, System.Action finish, float delay = 0)
    {
        DOTween.To(() => startValue, x => sliderImage.fillAmount = x, endValue, 0.5f).SetDelay(delay).OnComplete(() =>
        {
            if (finish != null)
            {
                finish();
            }
        });
    }

    /// <summary>
    /// 宝箱开启
    /// </summary>
    /// <param name="StarBox"></param>
    /// <param name="Content"></param>
    /// <param name="StartPoint"></param>
    /// <param name="MidPoint"></param>
    /// <param name="finish"></param>
    public static void StarBoxMove(GameObject StarBox, float BoxMoveOffset, GameObject Fx_StarBoxOpen, float delayTime, System.Action RewardShow, System.Action finish)
    {
        /*-------------------------------------动画效果------------------------------------*/
        StarBox.transform.DOScaleY(0.8f, 0.2f).SetDelay(delayTime).OnComplete(() =>
        {
            StarBox.transform.DOScaleY(1.2f, 0.2f).SetEase(Ease.OutBack);
            StarBox.transform.DOScaleX(0.7f, 0.2f);
            StarBox.transform.DOMoveY((StarBox.transform.position.y + BoxMoveOffset), 0.2f).OnComplete(() =>
            {
                if (RewardShow != null)
                {
                    RewardShow();
                }
                Fx_StarBoxOpen.SetActive(true);
                StarBox.transform.DOScaleX(1, 0.2f).OnComplete(() =>
                {
                    StarBox.transform.DOMoveY((StarBox.transform.position.y - BoxMoveOffset), 0.3f);
                    StarBox.transform.DOScaleY(1, 0.45f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        if (finish != null)
                        {
                            finish();
                        }
                    });
                });
            });
        });
    }

    /// <summary>
    /// 呼吸缩放效果
    /// </summary>
    /// <param name="BankBtn"></param>
    /// <param name="i"></param>
    public static void Breathe(GameObject BankBtn, int i)
    {
        float offset = -0.6f;
        DOTween.Kill("FlashMove");
        BankBtn.GetComponent<Image>().material = null;
        BankBtn.transform.localScale = new Vector3(1, 1, 1);
        if (i == 1)
        {
            BankBtn.GetComponent<Image>().material = Resources.Load<Material>("Effects/Mat_Flash");
            var BankAni = DOTween.Sequence();
            BankAni.Append(BankBtn.transform.DOScale(new Vector3(0.95f, 0.95f, 0.95f), 1.2f));
            BankAni.Append(BankBtn.transform.DOScale(new Vector3(1f, 1f, 1f), 1.2f));
            BankAni.Insert(0, DOTween.To(() => offset, x => BankBtn.GetComponent<Image>().material.SetFloat("_LightOffset", offset = x), 0.6f, 1f).SetDelay(1).OnComplete
                (() =>
                {
                    BankBtn.GetComponent<Image>().material.SetFloat("_LightOffset", -0.6f);
                }));
            BankAni.SetLoops(-1);
            BankAni.SetId<Tween>("FlashMove");
            BankAni.Play();
        }
    }


    /// <summary>
    /// 收金币
    /// </summary>
    /// <param name="GoldImage">金币图标</param>
    /// <param name="a">金币数量</param>
    /// <param name="StartTF">起始位置</param>
    /// <param name="EndTF">最终位置</param>
    /// <param name="finish">结束回调</param>
    public static void GoldMoveBest(GameObject GoldImage, int a, Transform StartTF, Transform EndTF, System.Action finish)
    {
        //如果没有就算了
        if (a == 0)
        {
            finish();
        }
        //数量不超过15个
        else if (a > 15)
        {
            a = 15;
        }
        //每个金币的间隔
        float Delaytime = 0;
        for (int i = 0; i < a; i++)
        {
            int t = i;
            //每次延迟+1
            Delaytime += 0.06f;
            //复制一个图标
            GameObject GoldIcon = Instantiate(GoldImage, GoldImage.transform.parent);
            if (a < 6)
            {
                GoldIcon.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            else
            {
                GoldIcon.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            }
            //初始化
            GoldIcon.transform.position = StartTF.position;
            //金币弹出随机位置
            float OffsetX = UnityEngine.Random.Range(-0.5f, 0.5f);
            float OffsetY = UnityEngine.Random.Range(-0.5f, 0.5f);
            //创建动画队列
            var s = DOTween.Sequence();
            //金币出现
            s.Append(GoldIcon.transform.DOMove(new Vector3(GoldIcon.transform.position.x + OffsetX, GoldIcon.transform.position.y + OffsetY, GoldIcon.transform.position.z), 0.15f).SetDelay(Delaytime).OnComplete(() =>
            {
                //限制音效播放数量
                //if (Mathf.Sin(t) > 0)
                if (t % 5 == 0)
                {
                    AudioMgr.Ins.PlaySound(AudioSound.Sound_GoldCoin);
                }
            }));
            //金币移动到最终位置
            s.Append(GoldIcon.transform.DOMove(EndTF.position, 0.6f).SetDelay(0.2f));
            s.Join(GoldIcon.transform.DOScale(1f, 0.3f).SetEase(Ease.InBack));
            s.AppendCallback(() =>
            {
                //收尾
                s.Kill();
                Destroy(GoldIcon);
                if (t == a - 1)
                {
                    if (finish != null)
                    {
                        finish();
                    }
                }
            });
        }
    }

    /// <summary>
    /// 发牌动画
    /// </summary>
    /// <param name="list">牌组</param>
    /// <param name="StartTF">牌堆位置</param>
    /// <param name="finish">结束回调</param>
    public static void CardShow(GameObject card, System.Action finish)
    {
        Vector3 B;
        B = card.transform.position;
        card.SetActive(true); ;
        card.transform.DOMove(B, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            if (finish != null)
            {
                finish();
            }
        });
    }

    public static void CollectCard(List<GameObject> CardList, Transform EndTF, System.Action finish)
    {
        for (int i = 0; i < CardList.Count; i++)
        {
            GameObject Card = CardList[i];
            Card.transform.DOMove(EndTF.position, 0.3f).OnComplete(() =>
            {
                if (finish != null)
                {
                    finish();
                }
            });
        }
    }

    /// <summary>
    /// 翻牌动画
    /// </summary>
    /// <param name="card"></param>
    /// <param name="finish"></param>
    public static void FropCard_1(GameObject card, System.Action finish = null, System.Action _allFinish = null)
    {
        card.transform.DOScaleX(0, 0.15f).OnComplete(() =>
        {
            if (finish != null)
            {
                finish();
            }

            card.transform.DOScaleX(1, 0.15f).onComplete = () =>
            {
                if (_allFinish != null)
                {
                    _allFinish();
                }
            };
        });
    }
    /// <summary>
    /// 翻牌动画
    /// </summary>
    /// <param name="card"></param>
    /// <param name="finish"></param>
    public static void FropCard_Revocation(GameObject card, System.Action finish, System.Action _allFinish = null)
    {
        card.transform.DOScaleX(0, 0.15f).OnComplete(() =>
        {
            if (finish != null)
            {
                finish();
            }

            card.transform.DOScaleX(1, 0.15f).onComplete = () =>
            {
                if (_allFinish != null)
                {
                    _allFinish();
                }
            };
        });
    }

    /// <summary>
    /// 摇摇
    /// </summary>
    /// <param name="card"></param>
    /// <param name="finish"></param>
    public static void CardShake(GameObject card, System.Action finish)
    {
        Sequence A = DOTween.Sequence();
        A.Append(card.transform.DORotate(new Vector3(0, 0, 15), 0.04f));
        A.Append(card.transform.DORotate(new Vector3(0, 0, -15), 0.08f));
        A.Append(card.transform.DORotate(new Vector3(0, 0, 15), 0.08f));
        A.Append(card.transform.DORotate(new Vector3(0, 0, -15), 0.08f));
        A.Append(card.transform.DORotate(new Vector3(0, 0, 0), 0.04f).OnComplete(() =>
        {
            if (finish != null)
            {
                finish();
            }
        }));
        A.Play();
    }

}
