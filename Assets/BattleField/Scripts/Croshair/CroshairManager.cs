using DG.Tweening;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CroshairManager : MonoBehaviour
{
    public static CroshairManager instance;

    public static Action<Vector3,bool> OnHitTarget;

    [SerializeField] private Image normalCroshair;
    [SerializeField] private Image hitCroshair;
    [SerializeField] private Transform croshairContainer;
    [SerializeField] private AudioSource hitmakerSoundPrefab;
    [SerializeField] private AudioSource shootingSoundPrefab;
    [SerializeField] private float cooldownTime = 0.5f;
    [SerializeField] private float strength = 1;
    [SerializeField] private Ease easeUP;
    [SerializeField] private Ease easeDown;
    [SerializeField] private int count;
    [SerializeField] private float fadeUp;
    [SerializeField] private float fadeDown;
    [SerializeField] private bool isAlwayHit;
    [SerializeField] private float hitVolume = .5f;
    [SerializeField] private float noHitVolume = .5f;
    [SerializeField] private float rotateRandom = 5;
    private void Awake()
    {
        instance = this;
        normalCroshair.DOFade(1, 0);
        hitCroshair.DOFade(0, 0);
        OnHitTarget += HitTarget;
        //hitmakerSoundPrefab.gameObject.SetActive(false);
        //shootingSoundPrefab.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        OnHitTarget -= HitTarget;
    }

    public void SetGunSound(AudioClip audiclip)
    {
        shootingSoundPrefab.clip = audiclip;
    }
    [Button]
    public void HitTarget(Vector3 spawnPosition,bool isHit)
    {
        if (croshairContainer.gameObject.activeSelf == false) return;

        hitCroshair.DOKill();
        normalCroshair.DOKill();
        if (isHit || isAlwayHit)
        {
            CreateHitAudio(spawnPosition);
            shootingSoundPrefab.volume = hitVolume;
        }
        else
        {
            shootingSoundPrefab.volume = noHitVolume;
        }
        CreateShootingAudio(spawnPosition);
        if (isHit)
        {
            hitCroshair.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-rotateRandom, rotateRandom));
            normalCroshair.DOFade(0, fadeUp).OnComplete(() =>
            {
                normalCroshair.DOFade(1, fadeDown);
            });
            hitCroshair.transform.DOScale(Vector3.one, 0);
            hitCroshair.transform.DOScale(Vector3.one * strength, .3f).SetEase(easeUP);
            hitCroshair.DOFade(1, fadeUp).OnComplete(() =>
            {
                hitCroshair.DOFade(0, fadeDown);
                hitCroshair.transform.rotation = Quaternion.Euler(0, 0, 0);
            });
        }
        
        //hitCroshair.DOFade(0, 0.2f);
    }

    private void CreateHitAudio(Vector3 spawnPosition)
    {
        
        var go = Instantiate(hitmakerSoundPrefab, spawnPosition, Quaternion.identity);
        go.Play();
        Destroy(go.gameObject, shootingSoundPrefab.clip.length);
    }
    private void CreateShootingAudio(Vector3 spawnPosition)
    {
        var go = Instantiate(shootingSoundPrefab, spawnPosition,Quaternion.identity);
        go.Play();
        Destroy(go.gameObject, shootingSoundPrefab.clip.length);
    }

    public void ShowCroshair()
    {
        croshairContainer.transform.gameObject.SetActive(true);
    }

    public void HideCroshair()
    {
        croshairContainer.transform.gameObject.SetActive(false);
    }
}
