using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementFadeOut : MonoBehaviour
{
    public void RunFace(float time, float fadeTime)
    {
        this.gameObject.SetActive(true);
        StartCoroutine(Fade(time, fadeTime));
    }

    public IEnumerator Fade(float time, float fadeTime)
    {
        MaskableGraphic thisElement = this.GetComponent<MaskableGraphic>();

        if (thisElement != null)
        {
            this.enabled = true;
            thisElement.color = new Color(1, 1, 1, 0);

            float elapsedTime = 0.0f;
            while (elapsedTime < 1)
            {
                elapsedTime += Time.deltaTime / fadeTime;
                thisElement.color = new Color(1, 1, 1, elapsedTime);
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(time);

            while (elapsedTime > 0)
            {
                elapsedTime -= Time.deltaTime / fadeTime;
                thisElement.color = new Color(1, 1, 1, elapsedTime);
                yield return new WaitForEndOfFrame();
            }
        }
        this.gameObject.SetActive(false);
    }
}
