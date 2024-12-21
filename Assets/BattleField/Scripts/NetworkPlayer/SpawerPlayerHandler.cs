using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SpawerPlayerHandler : NetworkBehaviour
{
    bool isSpawnPositionOnStartingBattle = false;
    CharacterMovementHandler characterMovementHandler;
    private void Awake() {
        isSpawnPositionOnStartingBattle = false;
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
    }
    
    public override void FixedUpdateNetwork()
    {
        return;
        if(HasStateAuthority == false) return;

        if(Object.HasStateAuthority) {
            if(isSpawnPositionOnStartingBattle == false && (Matchmaking.Instance.IsDone || MatchmakingTeam.Instance.IsDone)) {
                isSpawnPositionOnStartingBattle = true;
                StartCoroutine(Delay());
            }
        }
    }

    IEnumerator Delay() {
        Debug.Log($"_____ delay then requestRespawn");
        yield return new WaitForSeconds(0.1f);
        characterMovementHandler.RequestRespawn();
    }
}
