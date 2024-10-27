using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterOutfitsGenerator : NetworkBehaviour
{
    [Networked]
    public int skinsNumber_Network{get; set;}
    [SerializeField] int defaultSkinsNumber = 12;
    [SerializeField] Transform skinsTrans;
    [SerializeField] List<Transform> skinsList;

    //others
    ChangeDetector changeDetector;


    public override void Spawned() {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if(Object.HasInputAuthority) {
            if(SceneManager.GetActiveScene().name == "MainLobby") {
                RPC_RandomSKinsNumsGenerator(defaultSkinsNumber);
            }
            else {
                int skinsNums = Random.Range(0, skinsList.Count);
                RPC_RandomSKinsNumsGenerator(skinsNums);
            }
        }
        

        OnSkinsChanged();
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
}
