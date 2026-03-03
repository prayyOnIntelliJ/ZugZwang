using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(LoadingTextReferences))]
public class LoadingTextAnimation : TextMeshProUGUI
{
    private LoadingTextReferences loadingTextReferences;

    protected override void Awake()
    {
        base.Awake();
        loadingTextReferences = GetComponent<LoadingTextReferences>();
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(TextAnimation(loadingTextReferences.animationDuration));
    }

    private IEnumerator TextAnimation(float duration)
    {
        float fourth = duration / 4f;

        text = loadingTextReferences.prefix;
        yield return new WaitForSeconds(fourth);
        
        text = loadingTextReferences.prefix + ".";
        yield return new WaitForSeconds(fourth);
        
        text = loadingTextReferences.prefix + "..";
        yield return new WaitForSeconds(fourth);
        
        text = loadingTextReferences.prefix + "...";
        yield return new WaitForSeconds(fourth);
        
        StartCoroutine(TextAnimation(duration));
    }
}
