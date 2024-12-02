using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodLens : MonoBehaviour
{
    public static Action OnSlashEffect;

    private Image slashImage;
    private int randomHit = 5;
    private int index = 0;
    
    public float fadeIn = .5f;
    public float fadeUp = .25f;

    [SerializeField] private List<Sprite> bloodSprite = new();
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> hurtSoundList = new();
    private void Awake()
    {
        slashImage = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        
        slashImage.DOFade(0, 0);
        OnSlashEffect += FlashEffect;
        
        hurtSoundList = SoundManager.SoundAsset.GetSoundClip("hurt", 5);
        slashImage.sprite = GetRandomBloodSprite();
    }

    private void OnDestroy()
    {
        OnSlashEffect -= FlashEffect;
    }

    [Button]
    public void FlashEffect()
    {
        if(index > randomHit)
        {
            slashImage.sprite = GetRandomBloodSprite();
        }
        audioSource.CustomPlaySound(GetRandomHurtAudioClip());
        slashImage.DOKill();
        slashImage.DOFade(1, fadeUp).OnComplete(() =>
        {
            slashImage.DOFade(0, fadeIn);
        });
        index++;
    }


    private Sprite GetRandomBloodSprite()
    {
        return GetRandomBloodSprite(bloodSprite);
    }
    private AudioClip GetRandomHurtAudioClip()
    {
        return GetRandomBloodSprite(hurtSoundList);
    }
    private T GetRandomBloodSprite<T>(List<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
}
