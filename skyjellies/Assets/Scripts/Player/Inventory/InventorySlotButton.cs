using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    /// <summary>
    /// Facilitates interface with the player's inventory vias Canvas Buttons
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class InventorySlotButton : MonoBehaviour
    {

        int[] _index;

        public void SetIndex(int new0, int new1)
        {
            _index[0] = new0;
            _index[1] = new1;
        }

        public int[] GetIndex()
        {
            return _index;
        }

        public void Pressed()
        {
            //Inventory.PlayerInventory().SlotPressed(_index[0], _index[1]);
        }

        public void Initialize()
        {
            _index = new int[2];
            _index[1] = _index[0] = -1;
        }
    }
}
