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
        if(Object.HasInputAuthority) {
            if(isSpawnPositionOnStartingBattle == false && (Matchmaking.Instance.IsDone || MatchmakingTeam.Instance.IsDone)) {
                isSpawnPositionOnStartingBattle = true;
                characterMovementHandler.RequestRespawn();
                return;
            }
        }
    }
}
