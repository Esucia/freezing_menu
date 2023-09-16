using UnityEngine;

/// <summary>
/// This is a component of every item gameobject in the game. It holds all the info necessary for an item.
/// </summary>
public class Item : MonoBehaviour
{
    [SerializeField] 
    private ItemBase _itemInfo;

    [SerializeField] 
    private int _amount;
    
    private int _index; // An index may be necessary for the future if not remove

    public ItemBase ItemInfo { get => _itemInfo;}
    public int Amount { get => _amount; }

    public Item(ItemBase itemInfo, int amount, int index)
    {
        _itemInfo = itemInfo;
        _amount = amount;
        _index = index;
    }

    public Item(ItemBase itemInfo, int amount)
    {
        _itemInfo = itemInfo;
        _amount = amount;
    }

    void Start()
    {
        // If the item exists it can never be zero or less.
        if(_amount <= 0)
        {
            _amount = 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player.HotBar hotbar))
        {
            hotbar.TryAddItem(this);
            Destroy(gameObject);
        }
        else if (other.gameObject.TryGetComponent(out Player.Inventory inv))
        {
            inv.TryAddItem(this);
            Destroy(gameObject);
        }
    }
    
    public void ItemAmountAdd(int amount)
    {
        _amount += amount;
    }

    public void ItemAmountSubtract(int amount)
    {
        _amount -= amount;
    }
}
