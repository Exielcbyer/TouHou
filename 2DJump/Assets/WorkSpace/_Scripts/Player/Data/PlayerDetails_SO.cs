using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_SO", menuName = "Player/PlayerDetails")]
public class PlayerDetails_SO : ScriptableObject
{
    public float heath = 100;
    public float maxHeath = 100;
    public float energy = 25;
    public float maxEnergy = 50;
    public int enhancementPoints = 2;
    public int maxEnhancementPoints = 5;
    public bool hasIceHammer;
    public bool hasDrillingBit;
    public bool hasConvolute;
}
