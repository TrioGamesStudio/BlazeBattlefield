using System.Collections;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HPHandler : NetworkBehaviour
{
    [Networked]
    public byte Networked_HP { get; set; } = 100;

    [Networked]
    public bool Networked_IsDead { get; set; } = false;
    [Networked]
    public NetworkString<_16> Networked_Killer { get; set; }

    //[Networked]
    //public int deadCount {get; set;}

    bool isInitialized = false;
    const byte startingHP = 100;

    public Color uiOnHitColor;
    public Image uiOnHitImage;

    // change material when player hit damage (doi mau SkinnedMeshRenderer)
    List<FlashMeshRender> flashMeshRenders = new List<FlashMeshRender>();

    [SerializeField] GameObject playerModel;
    [SerializeField] GameObject hitBox;
    [SerializeField] GameObject localGunHolder;
    [SerializeField] GameObject deathParticlePf;
    HitboxRoot hitboxRoot;
    CharacterMovementHandler characterMovementHandler;

    //? thong bao khi bi killed
    NetworkInGameMessages networkInGameMessages;
    NetworkPlayer networkPlayer;
    public bool isSkipSettingStartValues = false; // ko cho chay lai ham start() khi thay doi host migration
    ChangeDetector changeDetector;  //duoc foi khi spawned => col 187

    bool isPublicDeathMessageSent = false;
    string killerName;

    // show thong tin player in game HP
    [SerializeField] InGamePlayerStatusUIHandler inGamePlayerStatusUIHandler;
    bool isShowResultTable = false;
    public UnityEvent<float> OnTakeDamageEvent = new UnityEvent<float>();

    LocalCameraHandler localCameraHandler;
    [SerializeField] bool isBot;
    private void Awake()
    {
        if (isBot) return;
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        hitboxRoot = GetComponent<HitboxRoot>();
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        networkPlayer = GetComponent<NetworkPlayer>();
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
    }
    void Start()
    {
        if (!isSkipSettingStartValues)
        {
            //deadCount = 0;
        }

        ResetMeshRenders();

        isInitialized = true;
    }

    //? ham duoc goi khi Object was spawned
    public override void Spawned()
    {     
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        if (isBot) return;
        // kiem tra co dang spawn tai ready scene hay khong
        bool isReadyScene = SceneManager.GetActiveScene().name == "MainLobby";
        if (isReadyScene) return;
        // show HP player on message box
        inGamePlayerStatusUIHandler.OnGamePlayerHpRecieved(Networked_HP);
        if (Object.HasStateAuthority)
            HealthBarUI.OnSetMaxHPAction(startingHP);
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
                    OnStateChanged(previousBool, currentBool);
                    break;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!isBot)
            CheckPlayerDeath(Networked_HP);
        if (!isShowResultTable && Networked_HP <= 0)
        {
            isShowResultTable = true;
            //if (Matchmaking.Instance.currentMode == Matchmaking.Mode.Solo)
            {
                PlayerRoomController playerRoomController = GetComponent<PlayerRoomController>();
                RPC_EliminatePlayer(playerRoomController.TeamID.ToString(), playerRoomController);
                if (isBot)
                {
                    FindObjectOfType<GameHandler>().CheckWin(); //check win for remaining real player
                    return;
                }
                RPC_HideLocalPlayerUI();
                RPC_ShowResultDuo();
            }
            //else
            //{
            //    PlayerRoomController playerRoomController = GetComponent<PlayerRoomController>();
            //    RPC_EliminatePlayer(playerRoomController.TeamID.ToString(), playerRoomController);
            //    RPC_HideLocalPlayerUI();
            //    RPC_ShowResultDuo();
            //}
        }

    }

    //? server call | coll 55 WeaponHandler.cs | khi hitInfo.HitBox tren player
    public void OnTakeDamage(string damageCausedByPlayerNickName, byte damageAmount, WeaponHandler weaponHandler)
    {
        Debug.LogWarning("Before damge:" + Networked_HP);
        if (Networked_IsDead || Networked_HP <= 0) return;

        //gioi han gia tri damageAmount
        if (damageAmount > Networked_HP) damageAmount = Networked_HP;
        Networked_HP -= damageAmount;
        RPC_SetNetworkedHP(Networked_HP, damageCausedByPlayerNickName);

        if (!isBot) RPC_UpdateStats(damageAmount);

        if (weaponHandler == null) return;

        CroshairManager.OnHitTarget();

        //Debug.LogWarning("After damge:" + Networked_HP);
        RPC_UpdateTeammateHP(damageAmount);
        killerName = damageCausedByPlayerNickName;       
        Debug.Log($"{Time.time} {transform.name} took damage {Networked_HP} left");

        if (Networked_HP <= 0)
        {
            Debug.Log($"{Time.time} {transform.name} is dead by {damageCausedByPlayerNickName}");
            /* RPC_SetNetworkedKiller(damageCausedByPlayerNickName); */ // can use
            isPublicDeathMessageSent = false;
            weaponHandler?.RequestUpdateKillCount();
            PlayerStats.Instance.MarkEndGame();
            //AlivePlayerControl.OnUpdateAliveCountAction?.Invoke();
        }
    }


    [EditorButton]
    private void Test()
    {

    }
    void CheckPlayerDeath(byte networkHP)
    {
        if (networkHP <= 0 && !isPublicDeathMessageSent)
        {
            isPublicDeathMessageSent = true;
            if (Object.HasStateAuthority)
            {
                networkInGameMessages.SendInGameRPCMessage(Networked_Killer.ToString(),
                    $" killed <b>{networkPlayer.nickName_Network.ToString()}<b>");
                GetComponent<PlayerMessageManager>().SendKillLogRPC(Networked_Killer.ToString(), networkPlayer.nickName_Network.ToString());
            }
        }
    }

    IEnumerator ServerRespawnCountine()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("xet respawn sau 2s");
        characterMovementHandler.RequestRespawn();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void OnHealRPC(byte amount)
    {
        var temp = Networked_HP;
        if (temp + amount >= startingHP)
        {
            Networked_HP = startingHP;
        }
        else
        {
            Networked_HP += amount;
        }

        //OnTakeDamageEvent.Invoke(-amount);
        RPC_UpdateTeammateHP(-amount);

        HealthBarUI.OnHealthChangeAction?.Invoke(Networked_HP);

        PlayerStats.Instance.AddHealthHealed(amount);
    }

    //RPC
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_SetNetworkedHP(byte hp, string name)
    {
        this.Networked_HP = hp;

        if (Networked_HP <= 0)
        {
            this.Networked_IsDead = true;
            this.Networked_Killer = name;
        }
        else
        {
            this.Networked_IsDead = false;
            this.Networked_Killer = null;
        }
        if (isBot) return;
        HealthBarUI.OnHealthChangeAction?.Invoke(hp);
    }

    //RPC
    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_UpdateTeammateHP(float damageAmount)
    {
        Debug.Log("+++ " + gameObject.name + " was hit");
        OnTakeDamageEvent.Invoke(damageAmount);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_UpdateStats(int damageAmount)
    {
        PlayerStats.Instance.AddDamageReceived(damageAmount);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_SetNetworkedIsDead(bool isDead)
    {
        this.Networked_IsDead = isDead;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_SetNetworkedKiller(string killerName) => this.Networked_Killer = killerName;

    void OnHPChanged(byte previous, byte current)
    {
        // if HP decreased
        if (current < previous) OnHPReduced();

        if (isBot) return;
        // khi hp thay doi show hp on screen
        inGamePlayerStatusUIHandler.OnGamePlayerHpRecieved(Networked_HP);
    }
    void OnHPReduced()
    {
        //if (isBot) return;
        if (!isInitialized) return;
        if (Object.HasStateAuthority)
        {
            if (!isBot)
            {
                //uiOnHitImage.color = uiOnHitColor;
                BloodLens.OnSlashEffect?.Invoke();

                // shaking camera
                if (localCameraHandler != null)
                {
                    localCameraHandler.SetRecoil_GetDamage(-7, 1, 0.35f, 2, 6);
                }
            }
        }
        StartCoroutine(OnHitCountine());
    }

    IEnumerator OnHitCountine()
    {
        // this.Object Run this.cs (do dang bi ban trung)
        // (Object.HasInputAuthority) => chi render tai man hinh MA THIS.OBJECT NAY DANG HasInputAuthority

        // this.Object Run this.cs (do dang bi ban trung)
        // render for Screen of this.Object - localPlayer + remotePlayer
        foreach (FlashMeshRender flashMeshRender in flashMeshRenders)
        {
            flashMeshRender.ChangeColor(Color.red);
        }



        yield return new WaitForSeconds(0.2f);
        foreach (FlashMeshRender flashMeshRender in flashMeshRenders)
        {
            flashMeshRender.RestoreColor();
        }

        // render cho man hinh cua this.Object run this.cs - KO HIEN THI O REMOTE
        if (Object != null && Object.HasStateAuthority && !Networked_IsDead && uiOnHitImage != null)
        {
            uiOnHitImage.color = new Color(0, 0, 0, 0);
        }
    }

    void ResetMeshRenders()
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

    //? OnChange Render networked variable
    void OnStateChanged(bool previous, bool current)
    {
        if (current)
        {
            OnDeath(); // dang song turn die(current)
        }

        else if (!current && previous)
        {
            OnRelive(); // dang die turn alive(current)
        }
    }

    void OnDeath()
    {
        Debug.Log($"{Time.time} onDeath");
        playerModel.SetActive(false);
        hitBox.SetActive(false);
        Instantiate(deathParticlePf, transform.position + Vector3.up * 1, Quaternion.identity);
        if (isBot) return;
        if (localGunHolder)
            localGunHolder.SetActive(false);   // khi death tat luon local gun
        hitboxRoot.HitboxRootActive = false; // ko de nhan them damage
        characterMovementHandler.CharacterControllerEnable(false);     
    }

    void OnRelive()
    {
        Debug.Log($"{Time.time} onRelive");

        if (Object.HasStateAuthority)
        {
            uiOnHitImage.color = new Color(0, 0, 0, 0);
        }
        playerModel.SetActive(true);
        if (localGunHolder)
            localGunHolder.SetActive(true);
        hitboxRoot.HitboxRootActive = true;
        characterMovementHandler.CharacterControllerEnable(true);
    }


    public void OnRespawned_ResetHPIsDead()
    {
        // khoi toa lai gia tri bat dau
        RPC_SetNetworkedHP(startingHP, null);

        /* RPC_SetNetworkedIsDead(false); */    // can use
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_ShowResult(int alivePlayer)
    {
        UIController.Instance.ShowResultPanel(alivePlayer);
        networkPlayer.localUI.SetActive(false);
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_EliminatePlayer(string teamID, PlayerRoomController playerRoomController)
    {
        FindObjectOfType<GameHandler>().Eliminate(teamID, playerRoomController);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_ShowResultDuo()
    {
        GetComponent<PlayerRoomController>().IsAlive = false;
        Debug.Log(":::Player shut down");
        StartCoroutine(FindObjectOfType<GameHandler>().CheckLose(GetComponent<PlayerRoomController>().TeamID.ToString()));
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_HideLocalPlayerUI()
    {
        if (!isBot)
            networkPlayer.localUI.SetActive(false);
    }
}