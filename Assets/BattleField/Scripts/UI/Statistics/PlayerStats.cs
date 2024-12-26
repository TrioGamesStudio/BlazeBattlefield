using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    // Singleton Instance (Optional for single-player stats)
    public static PlayerStats Instance { get; private set; }

    // Player stats fields
    [SerializeField] public int TotalKill; //{ get; private set; }
    [SerializeField] public int DamageDealt; //{ get; private set; }
    [SerializeField] public int DamageReceived;//{ get; private set; }
    [SerializeField] public int HealthHealed;//{ get; private set; }
    [SerializeField] public float SurvivalTime;//{ get; private set; }
    [SerializeField] private bool isEnd = false;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
    }

    private void Update()
    {
        if (!isEnd && SceneManager.GetActiveScene().name != "MainLobby")
        {
            SurvivalTime += Time.deltaTime;
        }
    }

    // Methods to update stats
    public void AddTotalKill(int amount)
    {
        TotalKill += amount;
    }

    public void AddDamageDealt(int amount)
    {
        DamageDealt += amount;
    }

    public void AddDamageReceived(int amount)
    {
        DamageReceived += amount;
    }

    public void AddHealthHealed(int amount)
    {
        HealthHealed += amount;
    }

    public void MarkEndGame()
    {
        isEnd = true;
    }

    public void ResetStats()
    {
        TotalKill = 0;
        DamageDealt = 0;
        DamageReceived = 0;
        HealthHealed = 0;
        SurvivalTime = 0f;
        isEnd = false;
    }
}
