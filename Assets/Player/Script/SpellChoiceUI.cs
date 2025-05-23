using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellChoiceUI : MonoBehaviour
{
    public static SpellChoiceUI Instance;

    public GameObject panel;
    public Button[] spellButtons;

    private List<Spell> currentSpells;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowChoices()
    {
        SpellManager spellManager = FindFirstObjectByType<SpellManager>();
        if (spellManager == null) return;

        spellManager.OnSpellChoices += DisplayChoices;
        spellManager.ProposeNewSpells();
    }

    void DisplayChoices(List<Spell> spells)
    {
        panel.SetActive(true);
        currentSpells = spells;

        for (int i = 0; i < spellButtons.Length; i++)
        {
            if (i < spells.Count)
            {
                spellButtons[i].gameObject.SetActive(true);
                spellButtons[i].GetComponentInChildren<Text>().text = spells[i].spellName;
                int index = i;
                spellButtons[i].onClick.RemoveAllListeners();
                spellButtons[i].onClick.AddListener(() => OnSpellSelected(index));
            }
            else
            {
                spellButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnSpellSelected(int index)
    {
        SpellManager spellManager = FindFirstObjectByType<SpellManager>();
        if (spellManager == null) return;

        spellManager.CmdChooseSpell(currentSpells[index].spellName);
        panel.SetActive(false);
    }
}
