using Mirror;
using System;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnXPChanged))] public int currentXP = 0;

    public int Level { get; private set; } = 1;
    public int xpToNextLevel => Level * 100;
    public static float MaxHealth = 100;
    public float healthPoint = MaxHealth; 
    public event Action<int> OnLevelUp; // event pour UI et autres
    

    [Server]
    public void GainXP(int amount)
    {
        currentXP += amount;
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            Level++;
            OnLevelUp?.Invoke(Level);
            TargetShowSpellChoices(connectionToClient);
        }
    }

    [TargetRpc]
    void TargetShowSpellChoices(NetworkConnection target)
    {
        // Appeler la UI locale pour proposer les choix de sorts
        SpellChoiceUI.Instance.ShowChoices();
    }

    void OnXPChanged(int oldXP, int newXP)
    {
        Debug.Log($"XP: {newXP}/{xpToNextLevel} | Level: {Level}");
    }

    internal void Heal(float baseHealAmount)
    {
        healthPoint = Mathf.Min(healthPoint + baseHealAmount, MaxHealth);
    }
}
