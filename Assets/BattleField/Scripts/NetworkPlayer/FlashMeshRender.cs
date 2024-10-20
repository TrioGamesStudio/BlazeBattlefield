using System.Collections.Generic;
using UnityEngine;

public class FlashMeshRender
{
    MeshRenderer meshRenderer;
    SkinnedMeshRenderer skinnedMeshRenderer;

    List<Color> defaultColor = new List<Color>();

    public FlashMeshRender(MeshRenderer meshRenderer, SkinnedMeshRenderer skinnedMeshRenderer) {
        this.meshRenderer = meshRenderer;
        this.skinnedMeshRenderer = skinnedMeshRenderer;

        if(meshRenderer != null) {
            foreach (Material material in meshRenderer.materials) {
                defaultColor.Add(material.color);
            }
        }
        if(skinnedMeshRenderer != null) {
            foreach (Material material in skinnedMeshRenderer.materials) {
                defaultColor.Add(material.color); //! co the ko run tai day
            }
        }
        
    }

    public void ChangeColor(Color flashColor) {
        if(meshRenderer != null) {
            for (int i = 0; i < meshRenderer.materials.Length; i++) {
                meshRenderer.materials[i].color = flashColor;
            }
        }

        if(skinnedMeshRenderer != null) {
            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++) {
                skinnedMeshRenderer.materials[i].color = flashColor;
            }
        }
        
    }

    public void RestoreColor() {
        if(meshRenderer != null) {
            for (int i = 0; i < meshRenderer.materials.Length; i++) {
                meshRenderer.materials[i].color = defaultColor[i];
            }
        }
        
        if(skinnedMeshRenderer != null) {
            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++) {
                skinnedMeshRenderer.materials[i].color = defaultColor[i];
            }
        }
    }
}
