using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object for an item tool type. Tool Items should be created from this through the
/// asset menu.
/// </summary>
[CreateAssetMenu(fileName = "ItemTool", menuName = "Item/ItemTool")]
public class ItemTool : ItemBase
{
    [SerializeField] private ToolType _toolType;
    public enum ToolType
    {
        None,
        Pickaxe
    }

}
