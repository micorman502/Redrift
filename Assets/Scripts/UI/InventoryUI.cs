using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject hotbarSlot;
    [SerializeField] GameObject inventorySlot;
    [SerializeField] Transform hotbarSlotHolder;
    [SerializeField] Transform inventorySlotHolder;
    [SerializeField] Transform craftingUIHolder;
    [SerializeField] GameObject tooltip;
    [SerializeField] Text tooltipTitle;
    [SerializeField] Text tooltipDesc;
    InventorySlot[] slots;
    RectTransform tooltipTransform;
    InventorySlot currentHoveredSlot;
    int previousSelectedItem;

    private void Awake()
    {
        SetUIState(false);
    }

    void OnEnable()
    {
        tooltipTransform = tooltip.GetComponent<RectTransform>();
        InventoryEvents.InitialiseInventoryUI += InitialiseUI;
        InventoryEvents.SetHotbarIndex += UpdateSelectedSlot;
        InventoryEvents.SetHoveredItem += SetHoveredItem;
        InventoryEvents.LeaveHoveredItem += LeaveHoveredItem;
        InventoryEvents.UpdateInventorySlot += UpdateSlot;
        PlayerEvents.OnLockStateSet += ChangeLockState;
    }

    void OnDisable()
    {
        InventoryEvents.InitialiseInventoryUI -= InitialiseUI;
        InventoryEvents.SetHotbarIndex -= UpdateSelectedSlot;
        InventoryEvents.SetHoveredItem -= SetHoveredItem;
        InventoryEvents.LeaveHoveredItem -= LeaveHoveredItem;
        InventoryEvents.UpdateInventorySlot -= UpdateSlot;
        PlayerEvents.OnLockStateSet -= ChangeLockState;
    }

    private void Update()
    {
        if (tooltip.activeSelf)
        {
            tooltipTransform.position = Input.mousePosition;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetUIState(!inventorySlotHolder.gameObject.activeSelf);
            LookLocker.Instance.SetLockedState(!inventorySlotHolder.gameObject.activeSelf);
        }
    }

    void SetUIState (bool state)
    {
        inventorySlotHolder.gameObject.SetActive(state);
        craftingUIHolder.gameObject.SetActive(state);
    }

    void ChangeLockState (bool locked)
    {
        if (locked)
        {
            InventoryEvents.LeaveHoveredItem();
        }
    }

    void UpdateSlot (WorldItem item, int index)
    {
        slots[index].SetItem(item);
    }

    void UpdateSelectedSlot (int index)
    {
        slots[previousSelectedItem].SetHotbarSlotSelector(false);
        slots[index].SetHotbarSlotSelector(true);
        previousSelectedItem = index;
    }

    void InitialiseUI (int hotbarSize, int inventorySize)
    {
        for (int i = 0; i < hotbarSlotHolder.childCount; i++)
        {
            Destroy(hotbarSlotHolder.GetChild(i).gameObject);
        }

        for (int i = 0; i < inventorySlotHolder.childCount; i++)
        {
            Destroy(inventorySlotHolder.GetChild(i).gameObject);
        }

        slots = new InventorySlot[inventorySize];
        int effectiveInventorySize = inventorySize - hotbarSize;

        for (int i = 0; i < hotbarSize; i++)
        {
            InventorySlot newSlot = Instantiate(hotbarSlot, hotbarSlotHolder).GetComponent<InventorySlot>();
            newSlot.SetSlotID(i);
            slots[i] = newSlot;
        }

        for (int i = 0; i < effectiveInventorySize; i++)
        {
            InventorySlot newSlot = Instantiate(inventorySlot, inventorySlotHolder).GetComponent<InventorySlot>();
            newSlot.SetSlotID(i + hotbarSize);
            slots[i + hotbarSize] = newSlot;
        }
    }

    public void SetHoveredItem(Item item, InventorySlot slot = null)
    {
        //audioManager.Play("UIClick");
        if (item)
        {
            if (!tooltip.activeSelf)
            {
                tooltip.SetActive(true);
            }
            tooltipTitle.text = item.itemName;
            if (string.IsNullOrEmpty(item.itemDescription))
            {
                if (item.type == Item.ItemType.Tool)
                {
                    tooltipDesc.text = "Speed: " + (item.speed).ToString();
                }
                else if (item.smeltItem)
                {
                    tooltipDesc.text = "Smeltable " + item.type.ToString();
                }
                else if (item.fuel > 0)
                {
                    tooltipDesc.text = "Burnable " + item.type.ToString();
                }
                else
                {
                    tooltipDesc.text = item.type.ToString();
                }
            }
            else
            {
                tooltipDesc.text = item.itemDescription;
            }
        }

        currentHoveredSlot = slot;
    }

    public void LeaveHoveredItem()
    {
        if(tooltip.activeSelf) {
			tooltip.SetActive(false);
		}
        if (currentHoveredSlot)
        {
            currentHoveredSlot = null;
        }
    }
}
