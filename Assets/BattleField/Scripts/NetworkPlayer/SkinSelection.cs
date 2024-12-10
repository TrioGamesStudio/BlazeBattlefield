using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelection : MonoBehaviour
{
    [SerializeField] Matchmaking matchmaking;
    [SerializeField] MatchmakingTeam matchmakingTeam;

    [SerializeField] int skinsNextNumber = 12;
    [SerializeField] Transform skinsParent;
    List<Transform> skinsList;
    int skinMaxNumber;
    // buttons
    [SerializeField] Button selectButton;

    private void Awake() {
        matchmaking = FindObjectOfType<Matchmaking>();
        matchmakingTeam = FindObjectOfType<MatchmakingTeam>();
        
        skinMaxNumber = skinsParent.childCount;

        selectButton.onClick.AddListener(SkinSelectNext);
    }

    private void Start() {
        //int randomSkinsLocal = Random.Range(0, skinsParent.childCount);

        if(Matchmaking.Instance.currentMode == Matchmaking.Mode.Solo) {
            SkinsSlectionSoloUpdate(skinsNextNumber);
        }

    }

    void SkinSelectNext() {

        if(skinsNextNumber == skinMaxNumber - 1) {
            skinsNextNumber = 0;
            
        } else {
            skinsNextNumber ++;
        }

        SkinsSlectionSoloUpdate(skinsNextNumber);

        if(Matchmaking.Instance.currentMode == Matchmaking.Mode.Duo) {
            FindObjectOfType<CharacterOutfitsGenerator>().RPC_RandomSKinsNumsGenerator(skinsNextNumber);
        }
        

        matchmaking.SkinSelectedNumber = skinsNextNumber;
        matchmakingTeam.SkinSelectedNumber = skinsNextNumber;
    }

    void SkinsSlectionSoloUpdate(int skinNumber) {
        foreach (Transform item in skinsParent) {
            item.gameObject.SetActive(false);
        }

        skinsParent.GetChild(skinNumber).gameObject.SetActive(true);
    }

}
