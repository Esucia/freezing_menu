using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is the info of a single inventory slot.
/// 
/// I originally made this and intended to use it but some how did not. If it is never used it can
/// be removed.
/// </summary>
[Serializable]
public class InventorySlot
{
    private int _itemID;
    private Image _backgroundSprite;
    private Sprite _itemSprite;
    private int _stackAmount;

    // Assuming we are using a background image along with the item image this should be used.
    public void FillSlot(int id, Image backgroundSprite, Sprite itemSprite, int stackAmount)
    {
        _itemID = id;
        _backgroundSprite = backgroundSprite;
        _itemSprite = itemSprite;
        _stackAmount = stackAmount;
    }

    // If a background image isn't required this one is suitable.
    public void FillSlot(int id, Sprite itemSprite, int stackAmount)
    {
        _itemID = id;
        _backgroundSprite = null;
        _itemSprite = itemSprite;
        _stackAmount = stackAmount;
    }

    // Assuming the background is going to show for empty slots this will be used.
    public void FillSlot(Image backgroundSprite)
    {
        _itemID = -1;
        _backgroundSprite = backgroundSprite;
        _itemSprite = null;
        _stackAmount = 0;
    }

    // This sets an empty inventory slot
    public void ClearSlot()
    {
        _itemID = -1;
        _backgroundSprite = null;
        _itemSprite = null;
        _stackAmount = 0;
    }
    public void UpdateStackAmount(int amount)
    {
        _stackAmount += amount;
    }

    public int ItemID { get => _itemID; }
    public Image BackgroundSprite { get => _backgroundSprite; }
    public Sprite ItemSprite { get => _itemSprite; }
    public int StackAmount { get => _stackAmount; }
}
