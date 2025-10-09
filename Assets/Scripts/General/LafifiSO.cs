using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Lafifi", menuName = "Lafifi")]
public class LafifiSO : ScriptableObject
{
    public Sprite lafifiSprite;
    public ItemShape lafifiShape;
    public string lafifiName;
    public int lafifiID;
    public float lafifiPrice;
    public string lafifiDescription;
}
