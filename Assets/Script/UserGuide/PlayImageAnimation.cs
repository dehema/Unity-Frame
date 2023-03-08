using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayImageAnimation : MonoBehaviour
{
    public List<Sprite> imageList;
    private Image image;
    public float speen;
    public CanvasGroup CanvasGroup;
    IEnumerator playAction ()
    {
        foreach(Sprite sprite in imageList)
        {
            image.sprite = sprite;
            yield return new WaitForSeconds(speen);
        }
        StartCoroutine("playAction");
    }
    private void OnEnable()
    {
        image = GetComponent<Image>();
        
        StartCoroutine("playAction");
        StartCoroutine("ShowImage");
    }
    private void OnDisable()
    {
        StopCoroutine("playAction");
    }

    private float value = 0;
    float yVelocity = 0.0f;
    IEnumerator ShowImage()
    {
        value = Mathf.Lerp(0.0f,3.0f,0.1f);
        CanvasGroup.alpha = value;
        yield return new WaitForSeconds(0.05f);
        value = Mathf.Lerp(value,3.0f,0.1f);
        CanvasGroup.alpha = value;
        yield return new WaitForSeconds(0.05f);
        value = Mathf.Lerp(value,3.0f,0.1f);
        CanvasGroup.alpha = value;
        yield return new WaitForSeconds(0.05f);
        value = Mathf.Lerp(value,3.0f,0.1f);
        CanvasGroup.alpha = value;
        yield return new WaitForSeconds(0.05f);
    }

    public void HidingImage()
    {
        if(gameObject.GetComponent<Image>().gameObject.activeSelf)
            StartCoroutine("HideImage");
    }
    IEnumerator HideImage()
    {
        value = Mathf.Lerp(3.0f,0.0f,0.1f);
        CanvasGroup.alpha = value;
        yield return new WaitForSeconds(0.05f);
        value = Mathf.Lerp(3.0f,value,0.1f);
        CanvasGroup.alpha = value;
        yield return new WaitForSeconds(0.05f);
        value = Mathf.Lerp(3.0f,value,0.1f);
        CanvasGroup.alpha = value;
        yield return new WaitForSeconds(0.05f);
        value = Mathf.Lerp(3.0f,value,0.1f);
        CanvasGroup.alpha = value;
        yield return new WaitForSeconds(0.05f);
        gameObject.SetActive(false);
    }
}
