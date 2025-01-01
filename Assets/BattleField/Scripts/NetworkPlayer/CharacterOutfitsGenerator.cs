using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class CharacterOutfitsGenerator : NetworkBehaviour
{
    [Networked]
    public int skinsNumber_Network{get; set;}

    [Networked]
    public int hatsNumber_Network { get; set; }

    [Header("Skin select settings")]
    [SerializeField] Transform skinsTrans;
    [SerializeField] List<Transform> skinsList;
    [SerializeField] int skinSelectedNum;

    [Header("Hat select settings")]
    [SerializeField] Transform headTrans;
    [SerializeField] List<Transform> hatsList;
    [SerializeField] int hatSelectedNum;

    //others
    ChangeDetector changeDetector;
    public bool isBot;


    public override void Spawned() {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if(Object.HasStateAuthority) {

            if(Matchmaking.Instance.currentMode == Matchmaking.Mode.Duo) {
                skinSelectedNum = Matchmaking.Instance.SkinSelectedNumber;
            }
            skinSelectedNum = Random.Range(0, skinsList.Count());
            Debug.Log("zzz select skin " + skinSelectedNum);
            RPC_RandomSKinsNumsGenerator(skinSelectedNum);

            if (isBot)
            {
                hatSelectedNum = Random.Range(0, hatsList.Count());
                Debug.Log("zzz select hat " + hatSelectedNum);
                RPC_RandomHatNumsGenerator(hatSelectedNum);
            }
        }
        

        OnSkinsChanged();
        if (isBot)
            OnHatsChanged();
    }

    private void Awake() {
        if (isBot) return;
        foreach (Transform item in skinsTrans)
        {
            skinsList.Add(item);
        }

    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer)) {
            switch (change)
            {
                case nameof(skinsNumber_Network):
                    OnSkinsChanged();
                    break;
                case nameof(hatsNumber_Network):
                    OnHatsChanged();
                    break;
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void RPC_RandomSKinsNumsGenerator(int skinsNums) {
        skinsNumber_Network = skinsNums;
        
    }

    private void OnSkinsChanged()
    {
        // clear all GO in skinsTrans
        foreach (Transform item in skinsTrans) {
            item.gameObject.SetActive(false);
        }

        skinsTrans.GetChild(skinsNumber_Network).gameObject.SetActive(true);
    }

    public void SetSkinSelectedNumber(int skinNum) {
        this.skinSelectedNum = skinNum;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void RPC_RandomHatNumsGenerator(int hatNums)
    {
        hatsNumber_Network = hatNums;

    }

    private void OnHatsChanged()
    {
        // clear all GO in skinsTrans
        foreach (Transform item in headTrans)
        {
            item.gameObject.SetActive(false);
        }

        headTrans.GetChild(hatsNumber_Network).gameObject.SetActive(true);
    }
}
