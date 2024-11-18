using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OnTakeDamageModel : MonoBehaviour
{
    List<FlashMeshRender> flashMeshRenders = new List<FlashMeshRender>();

    [SerializeField] GameObject playerModel;
    private HPHandler HPHandler;
    public Color uiOnHitColor;
    public Image uiOnHitImage;
    private void Awake()
    {
        ResetMeshRenders();
        HPHandler = GetComponent<HPHandler>();
        HPHandler.OnHpChanged += OnHpChanged;
    }

    private void OnDestroy()
    {
        HPHandler.OnHpChanged -= OnHpChanged;

    }

    private void OnHpChanged()
    {
        Runasd(HPHandler.Object.HasStateAuthority, HPHandler.Networked_IsDead);
    }

    public void Runasd(bool HasStateAuthority, bool Networked_IsDead)
    {
        StartCoroutine(OnHitCountine(HasStateAuthority, Networked_IsDead));
    }

    public IEnumerator OnHitCountine(bool HasStateAuthority,bool Networked_IsDead)
    {
        // this.Object Run this.cs (do dang bi ban trung) 
        // render for Screen of this.Object - localPlayer + remotePlayer
        foreach (FlashMeshRender flashMeshRender in flashMeshRenders)
        {
            flashMeshRender.ChangeColor(Color.red);
        }
        // this.Object Run this.cs (do dang bi ban trung) 
        // (Object.HasInputAuthority) => chi render tai man hinh MA THIS.OBJECT NAY DANG HasInputAuthority
        if (HasStateAuthority) uiOnHitImage.color = uiOnHitColor;

        yield return new WaitForSeconds(0.2f);
        foreach (FlashMeshRender flashMeshRender in flashMeshRenders)
        {
            flashMeshRender.RestoreColor();
        }

        // render cho man hinh cua this.Object run this.cs - KO HIEN THI O REMOTE
        if (HasStateAuthority && !Networked_IsDead)
        {
            uiOnHitImage.color = new Color(0, 0, 0, 0);
        }
    }

    private void ResetMeshRenders()
    {
        //clear old
        flashMeshRenders.Clear();

        //? change color when getting damage
        MeshRenderer[] meshRenderers = playerModel.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            flashMeshRenders.Add(new FlashMeshRender(meshRenderer, null)); // chi dang tao mang cho meshRender
        }

        SkinnedMeshRenderer[] skinnedMeshRenderers = playerModel.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
        {
            flashMeshRenders.Add(new FlashMeshRender(null, skinnedMeshRenderer)); // chi dang tao mang cho meshRender
        }
    }
}