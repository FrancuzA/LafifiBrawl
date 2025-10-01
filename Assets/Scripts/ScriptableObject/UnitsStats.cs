using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Units Statistics", menuName = "Scriptable/Units Statistics")]
public class UnitsStats : ScriptableObject
{
    [Header("Name and Image")]
    public string CharacterName;
    public Sprite CharacterImage;
    [Header("Health")]
    public int CurrentHealthPoints;
    public int MaxHealthPoints;
    [Header("Attack")]
    public int AttackDMG;
}