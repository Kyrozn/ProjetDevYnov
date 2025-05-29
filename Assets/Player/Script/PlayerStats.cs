using System.Collections;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    [SyncVar] public int currentXP;
    public int currentLevel;
    public CanvasGroup deathScreenSolo;      // écran gris + bouton mort solo
    public CanvasGroup deathScreenMultiplayer; // écran gris + texte "attendre la résurection"
    public int healthPoint;
    public int maxHealthPoint;
    private bool isDead = false;



    public override void OnStartLocalPlayer()
    {
        if (!isLocalPlayer) return;
    }

    [TargetRpc]
    void RpcReceiveXP(int amount)
    {
        currentXP += amount;
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        while (currentXP >= XPNeededForLevel(currentLevel + 1))
        {
            currentLevel++;
            // Lancer UI de choix de spell ici
            // GetComponent<SpellManager>().TriggerSpellChoice();
        }
    }

    int XPNeededForLevel(int level)
    {
        return level * 10; // Exemple simple
    }
    public void Heal(float baseHealAmount)
    {
        healthPoint = (int)math.min(maxHealthPoint, healthPoint + baseHealAmount);
    }
    public void TakeDamage(float baseHealAmount)
    {
        if (isDead) return; // ignore si déjà mort
        healthPoint -= (int)baseHealAmount;
        healthPoint = Mathf.Max(0, healthPoint); // ne pas descendre sous 0

        if (healthPoint <= 0)
        {
            isDead = true;
            // Replace this condition with your actual multiplayer check if available
            bool isMultiplayer = NetworkServer.connections.Count > 1;

            if (!isMultiplayer)
            {
                // SOLO
                ShowDeathScreenSolo();
            }
            else
            {
                // MULTI
                StartCoroutine(HandleMultiplayerDeath());
            }
        }
    }
    private void ShowDeathScreenSolo()
    {
        deathScreenSolo.alpha = 1;
        deathScreenSolo.blocksRaycasts = true;
        deathScreenSolo.interactable = true;
        // Ici, afficher bouton "Retour au menu"
        // Au clic du bouton, appeler méthode pour retourner au menu et détruire docker serveur (à implémenter)
    }
    private IEnumerator HandleMultiplayerDeath()
    {
        deathScreenMultiplayer.alpha = 1;
        deathScreenMultiplayer.blocksRaycasts = true;
        deathScreenMultiplayer.interactable = true;
        // Afficher "Attendre la résurection..."

        float waitTime = 15f;
        float timer = 0f;
        bool revived = false;

        while (timer < waitTime)
        {
            timer += Time.deltaTime;

            // Ici tu dois vérifier si un teammate t'a ressuscité
            // Par exemple via une variable publique set par le serveur / autre script
            // if (revivedCondition)
            // {
            //     revived = true;
            //     break;
            // }
            yield return null;
        }

        if (revived)
        {
            RevivePlayer();
        }
        else
        {
            // Même comportement que solo mais sans détruire le docker (si d'autres joueurs vivants)
            ShowDeathScreenSoloWithoutDestroyDocker();
        }
    }

    private void RevivePlayer()
    {
        isDead = false;
        healthPoint = (int)(maxHealthPoint * 0.3f); // récupère 30% max HP
        deathScreenMultiplayer.alpha = 0;
        deathScreenMultiplayer.blocksRaycasts = false;
        deathScreenMultiplayer.interactable = false;
        // Ici réactiver le joueur, la caméra, etc.
    }

    private void ShowDeathScreenSoloWithoutDestroyDocker()
    {
        // Pareil que ShowDeathScreenSolo mais sans détruire le docker serveur
        deathScreenSolo.alpha = 1;
        deathScreenSolo.blocksRaycasts = true;
        deathScreenSolo.interactable = true;
        // Afficher bouton "Retour au menu" sans détruire docker
    }
}
