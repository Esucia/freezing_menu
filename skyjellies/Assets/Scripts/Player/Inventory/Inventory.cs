using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Handles adding of new inventory slots to the inventory and is written to work for any gameobject 
    /// that needs inventory, not just the player. 
    /// 
    /// I plan on the inventory to be sorted by the three different categories and displayed in their 
    /// corresponding sections every time the backback is opened.
    /// 
    /// </summary>
    /// <note>Currently is in the Player namespace but probably should be its own with how it was written.
    /// - Cherve 7/24/23</note>

    public class Inventory : MonoBehaviour
    {
        [SerializeField, Tooltip("The max amount of slots the inventory can have")]
        private int _inventorySize;

        [SerializeField]
        private List<Item> _inventoryItems;

        private bool _canAddItem = false;

        private event Action<List<Item>> OnAddItem;

        public List<Item> InventoryItems { get => _inventoryItems; }

        private void Awake()
        {
            _inventoryItems = new List<Item>();
        }

        private int LocateItem(Item newItem)
        {
            int index = -1;
            foreach (var item in _inventoryItems)
            {
                index++;
                if (item.ItemInfo.ID == newItem.ItemInfo.ID && !IsStackFull(item.Amount, item.ItemInfo.MaxStack))
                {
                    return index;
                }
            }
            index = -1;
            return index;
        }

        private bool IsInventoryFull()
        {
            return _inventoryItems.Count >= _inventorySize;
        }

        private bool IsStackFull(int oldItemAmount, int maxStack)
        {
            return oldItemAmount == maxStack;
        }

        /// <summary>
        /// Creates a new item stack if the items picked up puts the amount over the max for the next open stack.
        /// </summary>
        /// <param name="item">The stack that has not reached the max items yet</param>
        /// <param name="amount">The number of items to add to the stack</param>
        /**Note: This works but there is a warning because of using new to create an Item. Technically it works
         *      as intended but I am assuming it wouldn't work in a build of the game maybe. Should probably be
         *      rewritten to look for the next open stack of the same item instead of making a new stack. Also,
         *      needs check for if the inventory is full and drop the item. - Cherve 7/24/23
         */
        private void AddToStack(Item item, int amount)
        {
            item.ItemAmountAdd(amount);
            if(item.Amount > item.ItemInfo.MaxStack)
            {
                int overflowAmount = item.Amount - item.ItemInfo.MaxStack;
                item.ItemAmountSubtract(overflowAmount);
                Item newItem = new Item(item.ItemInfo, overflowAmount);
                AddItem(newItem);
            }
        }

        /// <summary>
        /// Try to add the picked up item to the inventory of any gameobject with an inventory (Usually the player)
        /// </summary>
        /// <param name="item">The item component that was on the collided item pickup</param>
        public bool TryAddItem(Item item)
        {
            if (item.ItemInfo.IsStackable)
            {
                int index = LocateItem(item);
                if (index == -1)
                {
                    AddItem(item);
                }
                else
                {
                    AddToStack(_inventoryItems[index], item.Amount);
                }
            }
            else
            {
                AddItem(item);
            }
            PrintInventory();
            return _canAddItem;
        }

        private void AddItem(Item item)
        {
            if (!IsInventoryFull())
            {
                _inventoryItems.Add(item);
                _canAddItem = true;
                OnAddItem?.Invoke(_inventoryItems);
            }
            else
            {
                //TODO: Drop the excess items
                Debug.Log("Can't add item because inventory is full");
                _canAddItem = false;
            }
        }

        /// <summary>
        /// For printing out the full inventory list. Used for debugging.
        /// </summary>
        public void PrintInventory()
        {
            foreach(Item item in _inventoryItems) 
            {
                Debug.Log("Inventory - ID: "+ item.ItemInfo.ID+ " Name: "+item.ItemInfo.Name + " Amount: " + item.Amount);
            }
        }
    }
}
