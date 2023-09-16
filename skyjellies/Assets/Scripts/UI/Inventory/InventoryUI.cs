using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField, Tooltip("adjust this to change where the inventory slots are centered")]
        private Vector2 _inventoryCenter;

        [SerializeField, Tooltip("Prefab of the inventory slot element")]
        private GameObject _inventorySlotObject;

        [SerializeField, Tooltip("how many rows,columns of inventory space should there be")]
        private Vector2Int _inventoryDimensions;

        [SerializeField, Tooltip("How far apart the inventory slots are from each other")]
        private Vector2 _inventorySpacing;

        [SerializeField, Tooltip("The UI game object for the resource menu")]
        private GameObject _resourceMenu;

        [SerializeField, Tooltip("The UI game object for the jelly item menu")]
        private GameObject _jellyItemMenu;

        [SerializeField, Tooltip("The UI game object for the tool menu")]
        private GameObject _toolMenu;

        [SerializeField, Tooltip("The UI game object for the backpack button")]
        private GameObject _backpackButton;

        private InputAction _scrollAction;
        private InputAction _backpackAction;

        //private Canvas _canvas;
        private PlayerInput _playerInput;
        private Body _playerBody;

        private bool _isInventoryOpen;

        private void Start()
        {
            GameObject _player = GameObject.FindGameObjectWithTag("Player");
            if (_player != null)
            {
                _playerInput = _player.GetComponent<PlayerInput>();
                _playerBody = _player.GetComponent<Body>();
            }

            /*        _canvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<Canvas>();
                if (_canvas == null)
                {
                    Debug.LogWarning("Canvas is null");
                }*/

            _scrollAction = _playerInput.actions.FindAction("HotBarScroll", true);
            _backpackAction = _playerInput.actions.FindAction("Backpack", true);
            _scrollAction.performed += ctx => OnHotBarScroll(ctx);
            _backpackAction.performed += ctx => OnBackpack(ctx);
        }

        // Update is called once per frame
        private void Update()
        {

        }

        private void OnEnable()
        {
            _scrollAction?.Enable();
            _backpackAction?.Enable();
        }

        private void OnDisable()
        {
            _scrollAction?.Disable();
            _backpackAction?.Disable();
        }

        private void OnBackpack(InputAction.CallbackContext context)
        {
            Debug.Log("Pressing Inventory");
            if (!_isInventoryOpen)
            {
                //the player being able to open the inventory while moving causes a sliding bug
                if (_playerBody.IsMoving)
                {
                    _isInventoryOpen = !_isInventoryOpen;


                    //TODO:  Get this to work in player
                    /* UpdateInventoryDisplay();

                    GetComponent<Motion.RectilinearMovement>().enabled = false;
                    GetComponent<View.FirstPersonView>().enabled = false;
                    GetComponent<CharacterController>().enabled = false;
                    GetComponent<Body>().enabled = false;*/
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
            else
            {
                _isInventoryOpen = !_isInventoryOpen;

                /*            for (int i = 0; i < _inventorySlots.Length; i++)
                        {
                            for (int j = 0; j < _inventorySlots[i].Length; j++)
                            {
                                _inventorySlots[i][j].enabled = false;
                                _inventorySlots[i][j].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
                            }
                        }*/

                /*            GetComponent<Motion.RectilinearMovement>().enabled = true;
                        GetComponent<View.FirstPersonView>().enabled = true;
                        GetComponent<CharacterController>().enabled = true;
                        GetComponent<Body>().enabled = true;*/
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                //_heldIndex[0] = _heldIndex[1] = -1;
            }
        }

        private void OnHotBarScroll(InputAction.CallbackContext context)
        {

            if (!_isInventoryOpen)
            {
                float scrolling = context.ReadValue<float>();
                if (scrolling > 0)
                {
                    //Debug.Log("Scrolling up");
                    /*                _hotbarSelected++;
                                if (_hotbarSelected >= _theInventory[0].Length)
                                {
                                    _hotbarSelected = 0;
                                }*/
                }
                else if (scrolling < 0)
                {
                    //Debug.Log("Scrolling down");
                    /*                _hotbarSelected--;
                                if (_hotbarSelected < 0)
                                {
                                    _hotbarSelected = _theInventory[0].Length - 1;
                                }*/
                }
                //UpdateHotbar();

            }
        }

        //these ensure the display is reflective of what the data values actually are
        private void UpdateInventoryDisplay()
        {
            /*       for (int i = 0; i < _inventorySlots.Length; i++)
                {
                    for (int j = 0; j < _inventorySlots[i].Length; j++)
                    {
                        _inventorySlots[i][j].enabled = true;
                        _inventorySlots[i][j].sprite = _theInventory[i][j].ItemSprite;
                        _inventorySlots[i][j].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = _theInventory[i][j].StackAmount.ToString();
                        if (_inventorySlots[i][j].sprite == null)
                        {
                            _inventorySlots[i][j].sprite = _slotObject.GetComponent<Image>().sprite;
                            _inventorySlots[i][j].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
                        }
                    }
                }
                UpdateHotbar();*/
        }

        //first inventory slot pressed is rembemered, and the item therein is moved to the next slot pressed
        public void SlotPressed(int index0, int index1)
        {
            /*       if (_heldIndex[0] == -1)
                {
                    if (_theInventory[index0][index1].ItemID != -1)
                    {
                        _heldIndex[0] = index0;
                        _heldIndex[1] = index1;
                    }
                }
                else
                {
                    if (_theInventory[index0][index1].ItemID == -1)
                    {
                        int amount = _theInventory[_heldIndex[0]][_heldIndex[1]].StackAmount;
                        _theInventory[index0][index1].FillSlot(
                            _theInventory[_heldIndex[0]][_heldIndex[1]].ItemID,
                            _inventoryImage,
                            _theInventory[_heldIndex[0]][_heldIndex[1]].ItemSprite,
                            amount);

                        _theInventory[_heldIndex[0]][_heldIndex[1]].FillSlot(_inventoryImage);
                        UpdateInventoryDisplay();
                        _heldIndex[0] = _heldIndex[1] = -1;
                    }
                    else //when the second selected slot also has an item, swap the two
                    {
                        _heldSlot = _theInventory[index0][index1];
                        _theInventory[index0][index1] = _theInventory[_heldIndex[0]][_heldIndex[1]];
                        _theInventory[_heldIndex[0]][_heldIndex[1]] = _heldSlot;

                        _heldSlot.FillSlot(_inventoryImage);
                        UpdateInventoryDisplay();
                    }
                }*/
        }
    }
}