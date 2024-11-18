using UnityEngine;
using Fusion;
using System;

public class HPHandler : NetworkBehaviour
{
    [Networked]
    public byte Networked_HP { get; set; } = 5;
    
    [Networked]
    public bool Networked_IsDead {get; set;} = false;
    [Networked]
    public NetworkString<_16> Networked_Killer { get; set; }

    //[Networked]
    //public int deadCount {get; set;}

    bool isInitialized = false;
    const byte startingHP = 5;

    public Action OnHpChanged;
    public Action OnPlayerDeath;


    // change material when player hit damage (doi mau SkinnedMeshRenderer)

    [SerializeField] PlayerNotify PlayerNotify;
    //[SerializeField] OnTakeDamageModel OnTakeDamageModel;


    //? thong bao khi bi killed
    NetworkInGameMessages networkInGameMessages;
    NetworkPlayer networkPlayer;
    ChangeDetector changeDetector;  //duoc foi khi spawned => col 187

    bool isPublicDeathMessageSent = false;
    string killerName;

    // show thong tin player in game HP
    [SerializeField] InGamePlayerStatusUIHandler inGamePlayerStatusUIHandler;

    private void Awake() {
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        networkPlayer = GetComponent<NetworkPlayer>();
        PlayerNotify = GetComponent<PlayerNotify>();
    }
    void Start() {

        isInitialized = true;
    }

    //? ham duoc goi khi Object was spawned
    public override void Spawned() {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        // show HP player on message box
        inGamePlayerStatusUIHandler.OnGamePlayerHpRecieved(Networked_HP);
    }

    public override void Render()
    {
        if (changeDetector == null) return;
        foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(Networked_HP):
                var byteReader = GetPropertyReader<byte>(nameof(Networked_HP));
                var (previousByte, currentByte) = byteReader.Read(previousBuffer, currentBuffer);
                OnHPChanged(previousByte, currentByte);
                    break;
                
                case nameof(Networked_IsDead):
                var boolReader = GetPropertyReader<bool>(nameof(Networked_IsDead));
                var (previousBool, currentBool) = boolReader.Read(previousBuffer, currentBuffer);
                //OnStateChanged(previousBool, currentBool);
                    break;
            }
        }
    }

    public override void FixedUpdateNetwork() {
        CheckPlayerDeath(Networked_HP);
    }

    //? server call | coll 55 WeaponHandler.cs | khi hitInfo.HitBox tren player
    public void OnTakeDamage(string damageCausedByPlayerNickName, byte damageAmount, WeaponHandler weaponHandler) {
        if(Networked_IsDead) return;

        //gioi han gia tri damageAmount
        if(damageAmount > Networked_HP) damageAmount = Networked_HP;

        Networked_HP -= damageAmount;

        killerName = damageCausedByPlayerNickName;
        RPC_SetNetworkedHP(Networked_HP, damageCausedByPlayerNickName);

        Debug.Log($"{Time.time} {transform.name} took damage {Networked_HP} left");

        if(Networked_HP <= 0) {
            OnPlayerDeath?.Invoke();
            Debug.Log($"{Time.time} {transform.name} is dead by {damageCausedByPlayerNickName}");
            /* RPC_SetNetworkedKiller(damageCausedByPlayerNickName); */ // can use
            isPublicDeathMessageSent = false;
            //StartCoroutine(ServerRespawnCountine()); //Phuc comment: not allow player respawn
            /* RPC_SetNetworkedIsDead(true); */ // can use
            

            //deadCount ++;
            weaponHandler.killCount ++;
        }
    }

    void CheckPlayerDeath(byte networkHP) {
        if(networkHP <= 0 && !isPublicDeathMessageSent) {
            isPublicDeathMessageSent = true;
            if(Object.HasStateAuthority) {
                networkInGameMessages.SendInGameRPCMessage(Networked_Killer.ToString(), 
                    $" killed <b>{networkPlayer.nickName_Network.ToString()}<b>");
            }
        }
    }




    //RPC
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_SetNetworkedHP(byte hp, string name) {
        this.Networked_HP = hp;

        if(Networked_HP <= 0) {
            this.Networked_IsDead = true;
            this.Networked_Killer = name;
        } 
        else {
            this.Networked_IsDead = false;
            this.Networked_Killer = null;
        } 
    }

    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    //void RPC_SetNetworkedIsDead(bool isDead) {
    //    this.Networked_IsDead = isDead;
    //}

    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    //void RPC_SetNetworkedKiller(string killerName) => this.Networked_Killer = killerName;

    void OnHPChanged(byte previous, byte current)  {
        // if HP decreased
        if(current < previous) OnHPReduced();

        // khi hp thay doi show hp on screen
        inGamePlayerStatusUIHandler.OnGamePlayerHpRecieved(Networked_HP);
    }


    void OnHPReduced() {
        if(!isInitialized) return;
        OnHpChanged?.Invoke();
        //OnTakeDamageModel.Runasd(Object.HasStateAuthority,Networked_IsDead);
    }
    
    public void OnRespawned_ResetHPIsDead() {
        // khoi toa lai gia tri bat dau
        RPC_SetNetworkedHP(startingHP, null);

        /* RPC_SetNetworkedIsDead(false); */    // can use
    }


    
}
