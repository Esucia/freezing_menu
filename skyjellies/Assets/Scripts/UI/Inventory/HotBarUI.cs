using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class HotBarUI : MonoBehaviour
    {
        [SerializeField, Tooltip("add a prefab for an inventory slot")]
        private GameObject _inventorySlotPrefab;

        [SerializeField, Tooltip("Add the GameObject representing thr hotbar that has a grid layout group component")]
        private GameObject _hotBarGrid;

        private int[] _heldIndex; //stores what slot items are being moved from
        private InventorySlot _heldItem; //holds data while two inventory slots are being swapped 

        private int _hotbarSelected; //what item in the hotbar is currently selected

        private Player.HotBar _hotbar; // The list of the player's hotbar data

        private void Awake()
        {
            _hotbar = GameObject.FindGameObjectWithTag("Player").GetComponent<Player.HotBar>();
            if(_hotbar == null)
            {
                Debug.LogError("Hotbar on player could not be found.");
            }
        }

        private void OnEnable()
        {
            _hotbar.OnAddItem += AddItemsToGrid;
            _hotbar.OnUpdateItem += UpdateGrid;
        }

        private void OnDestroy()
        {
            _hotbar.OnAddItem -= AddItemsToGrid;
            _hotbar.OnUpdateItem -= UpdateGrid;
        }

        /// <summary>
        /// This is subscribed to when a value on the hotbar is being changed. It updates all the values of each
        /// inventory ui gameobject. Could be more efficient.
        /// </summary>
        /// <param name="hotbar">The list of hotbar data</param>
        private void UpdateGrid(List<Item> hotbar)
        {
            int index = 0;
            
            foreach (var item in hotbar)
            {
                GameObject slot = _hotBarGrid.gameObject.transform.GetChild(index).gameObject;
                var image = slot.transform.Find("Image").GetComponent<Image>();
                var count = slot.transform.Find("Item Count").GetComponent<TextMeshProUGUI>();

                image.sprite = item.ItemInfo.Icon;
                count.text = item.Amount.ToString();
                index++;
            }
        }

        /// <summary>
        /// This is subsribed to when a new item is created in the hotbar. This Destroys all the gameobjects in 
        /// the hotbar UI grid and replaces them each time. Inefficient but it works. Most likely needs to be 
        /// written better.
        /// </summary>
        /// <param name="hotbar">The list of hotbar data</param>
        private void AddItemsToGrid(List<Item> hotbar)
        {
            ClearHotbarUI();

            foreach (var item in hotbar)
            {
                GameObject slot = Instantiate(_inventorySlotPrefab, _hotBarGrid.gameObject.transform);
                var image = slot.transform.Find("Image").GetComponent<Image>();
                var count = slot.transform.Find("Item Count").GetComponent<TextMeshProUGUI>();

                image.sprite = item.ItemInfo.Icon;
                count.text = item.Amount.ToString();
            }
        }

        private void ClearHotbarUI()
        {
            for(int i = 0; i < _hotBarGrid.gameObject.transform.childCount; i++)
            {
                Destroy(_hotBarGrid.gameObject.transform.GetChild(i).gameObject);
            }
        }
    }
}