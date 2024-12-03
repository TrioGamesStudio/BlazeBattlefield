using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class CroshairManager : MonoBehaviour
{
    public static CroshairManager instance;
    [SerializeField] private Image normalCroshair;
    [SerializeField] private Image hitCroshair;
    [SerializeField] private Transform croshairContainer;
    [SerializeField] private AudioSource hitmakerSoundPrefab;
    [SerializeField] private AudioSource shootingSoundPrefab;
    private void Awake()
    {
        normalCroshair.DOFade(1, 0);
        hitCroshair.DOFade(0, 0);

        hitmakerSoundPrefab.gameObject.SetActive(false);
        shootingSoundPrefab.gameObject.SetActive(false);
    }
    public float cooldownTime = 0.5f;
    public float strength = 1;
    public Ease easeUP;
    public Ease easeDown;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ShakeCroshair(strength, cooldownTime);
            HitTarget(isHit);
        }
    }

    public void ShakeCroshair(float strength, float cooldownTime)
    {
        
    }
    public float fadeUp;
    public float fadeDown;
    public bool isHit;
    public float hitVolume = .5f;
    public float noHitVolume = .5f;
    public float rotateRandom = 5;
    public void SetGunSound(AudioClip audiclip)
    {
        shootingSoundPrefab.clip = audiclip;
    }
    [Button]
    public void HitTarget(bool isHit)
    {
        hitCroshair.DOKill();
        normalCroshair.DOKill();
        if (isHit)
        {
            CreateHitAudio();
            shootingSoundPrefab.volume = hitVolume;
        }
        else
        {
            shootingSoundPrefab.volume = noHitVolume;
        }
        CreateShootingAudio();
        if (isHit)
        {
            hitCroshair.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-rotateRandom, rotateRandom));
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

    private AudioSource CreateHitAudio()
    {
        var audio = Instantiate(hitmakerSoundPrefab);
        audio.gameObject.SetActive(true);
        audio.Play();
        Destroy(audio.gameObject, audio.clip.length);
        return audio;
    }
    private AudioSource CreateShootingAudio()
    {
        var audio = Instantiate(shootingSoundPrefab);
        audio.gameObject.SetActive(true);
        audio.Play();
        Destroy(audio.gameObject, audio.clip.length);
        return audio;
    }
}
