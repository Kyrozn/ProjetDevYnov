[System.Serializable]
public class SpellInstance
{
    public Spell spell;
    public int level;

    public SpellInstance(Spell spell)
    {
        this.spell = spell;
        level = 1;
    }

    public void Upgrade()
    {
        spell.Upgrade(this);
    }
}
