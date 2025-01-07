using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class CharacterOutfitsGenerator : NetworkBehaviour
{
    [Networked]
    public int skinsNumber_Network{get; set;}

    [Header("Skin select settings")]
    [SerializeField] Transform skinsTrans;
    [SerializeField] List<Transform> skinsList;
    [SerializeField] int skinSelectedNum;

    //others
    ChangeDetector changeDetector;


    public override void Spawned() {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if(Object.HasInputAuthority) {

            //if(Matchmaking.Instance.currentMode == Matchmaking.Mode.Duo) {
            //    skinSelectedNum = Matchmaking.Instance.SkinSelectedNumber;
            //}
            ////skinSelectedNum = Random.Range(0, skinsList.Count());
            ////Debug.Log("zzz select skin " + skinSelectedNum);
            //RPC_RandomSKinsNumsGenerator(skinSelectedNum);
            skinsNumber_Network = Matchmaking.Instance.SkinSelectedNumber;
        }
        OnSkinsChanged();
        //OnSkinsChanged();
    }

    private void Awake() {
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
}
