using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Handles the adding of items into the hotbar and overflows into the inventory if necessary.
    /// </summary>
    public class HotBar : MonoBehaviour
    {
        [SerializeField, Tooltip("The max amount of slots the hotbar can have")]
        private int _maxHotbarSize;
        private int _hotbarSize;

        private bool _canAddItem = false;

        [SerializeField]
        private List<Item> _hotbarItems;
        private Inventory _inventory;

        public event Action<List<Item>> OnAddItem;
        public event Action<List<Item>> OnUpdateItem;

        private void Awake()
        {
            _hotbarItems = new List<Item>();
        }

        private void Start()
        {
            _inventory = GetComponent<Inventory>();
        }

        public List<Item> HotBarItems { get => _hotbarItems; }
        public int MaxHotbarSize { get => _maxHotbarSize; }

        /// <summary>
        /// Searches through the hot bar list for the item that may be added.
        /// </summary>
        /// <param name="itemInfo">The item thats existance is being checked</param>
        /// <returns>Returns the index that the item is found, if not found returns -1</returns>
        private int LocateItem(ItemBase itemInfo)
        {
            int index = -1;
            foreach (var item in _hotbarItems)
            {
                index++;
                if (item.ItemInfo.ID == itemInfo.ID && !IsStackFull(item.Amount, item.ItemInfo.MaxStack))
                {
                    return index;
                }
            }
            index = -1;
            return index;
        }

        private bool IsHotbarFull()
        {
            return _hotbarItems.Count >= _maxHotbarSize;
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
            if (item.Amount > item.ItemInfo.MaxStack)
            {
                int overflowAmount = item.Amount - item.ItemInfo.MaxStack;
                item.ItemAmountSubtract(overflowAmount);
                Item newItem = new Item(item.ItemInfo, overflowAmount);
                AddItem(newItem);
            }
            _canAddItem = true;
            OnUpdateItem?.Invoke(_hotbarItems);
        }

        /// <summary>
        /// Adds the new item to the hot bar unless the hot bar is full it will try to add the item
        /// to the inventory.
        /// </summary>
        /// <param name="item">The item that will be added to the hotbar</param>
        private void AddItem(Item item)
        {
            if (!IsHotbarFull())
            {
                _hotbarItems.Add(item);
                _canAddItem = true;
                OnAddItem?.Invoke(_hotbarItems);
            }
            else
            {
                _inventory.TryAddItem(item);
                _canAddItem = false;
            }
        }

        /// <summary>
        /// Try to add the picked up item to the inventory of any gameobject with an inventory (Usually the player)
        /// </summary>
        /// <param name="item">The item component that was on the collided item pickup</param>
        /// <returns> Returns true if the item can be added, otherwise false.</returns>
        public bool TryAddItem(Item item)
        {
            if (item.ItemInfo.IsStackable)
            {
                int index = LocateItem(item.ItemInfo);
                if (index == -1)
                {
                    AddItem(item);
                }
                else
                {
                    AddToStack(_hotbarItems[index], item.Amount);
                }
            }
            else
            {
                AddItem(item);
            }

            PrintHotbar();
            return _canAddItem;
        }

        /// <summary>
        /// For printing out the hotbar list. Used for debugging before UI was setup.
        /// </summary>
        public void PrintHotbar()
        {
            foreach (Item item in _hotbarItems)
            {
                Debug.Log("Hotbar - ID: " + item.ItemInfo.ID + " Name: " + item.ItemInfo.Name + " Amount: " + item.Amount);
            }
        }

        /// <summary>
        /// Check if the item exists in the HotBar & the item's amount >= subtractedAmount
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <param name="subtractedAmount"></param>
        /// <returns>return the possibility to substract</returns>
        public bool TrySubtractItemAmount(ItemBase itemInfo, int subtractedAmount)
        {
            var itemIndex= LocateItem(itemInfo);
            if (itemIndex == -1 || _hotbarItems[itemIndex].Amount < subtractedAmount)
            {
                return false;
            }

            AddToStack(_hotbarItems[itemIndex], -subtractedAmount);
            PrintHotbar();

            return true;
        }
    }
}