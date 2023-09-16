using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a scriptable object for a list of scriptable object items of type ItemBase. This will most likely be used
/// for a master list of all the items in the game to be referenced. This can also be used to create
/// catagorized lists which may be useful for the backpack category sections.
/// </summary>
[CreateAssetMenu(fileName ="ItemList",menuName ="Item/New Item List")]
public class ItemList : ScriptableObject
{
    public List<ItemBase> items = new List<ItemBase>();
}
