using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

	public Text amountText;
	public Sprite slotSprite;
	public Sprite emptySlotSprite;

	public Image slotImage;
	public Image icon;

	public GameObject selector;

	public Animation anim;

	Inventory inventory;
	PlayerController player;

	int mode;

	[SerializeField] int slotID;
	[HideInInspector] public Item currentItem;
	[HideInInspector] public int stackCount;

	bool wasActiveInHierarchy;

	void Awake() {
		anim = GetComponent<Animation>();

		SetHotbarSlotSelector(false);
	}

    void Start()
    {
		InventoryEvents.RequestInventorySlot(slotID);
	}

	public void SetHotbarSlotSelector (bool show)
    {
		if (selector)
        {
			if (show)
            {
				selector.SetActive(true);
            } else
            {
				selector.SetActive(false);
            }
        }
    }

    public void SetItem(WorldItem item) {
		if(!item.item) {
			ClearSlot();
			return;
		}
		icon.gameObject.SetActive(true);
		icon.sprite = item.item.icon;
		currentItem = item.item;
		slotImage.sprite = slotSprite;
		stackCount = item.amount;
		if(mode != 1) {
			amountText.gameObject.SetActive(true);
		}
		amountText.text = stackCount.ToString();
		if(anim == null) {
			anim = GetComponent<Animation>();
		}
		anim.Play();
	}

	/*public void IncreaseItem(int count) {
		if(mode != 1) {
			stackCount += count;
			amountText.text = stackCount.ToString();
		}
		anim.Play();
	}

	public void DecreaseItem(int count) {
		if(mode != 1) {
			stackCount -= count;
			amountText.text = stackCount.ToString();
			if(stackCount <= 0) {
				ClearItem();
			}
		}
		anim.Play();
	}*/

	public void ClearSlot() {
		icon.sprite = null;
		icon.gameObject.SetActive(false);
		currentItem = null;
		slotImage.sprite = emptySlotSprite;
		stackCount = 0;
		amountText.gameObject.SetActive(false);
	}

	/*public void OnItemClick() {
		if(currentItem && !player.ActiveSystemMenu() && !player.dead) {
			if(currentItem.type == Item.ItemType.Structure) {
				PlaceItem(currentItem);
			} else {
				DropItem();
			}
		} COME BACK
	}*/
	/*
	public void OnInventoryClose() {
		if(transform.parent.name != "Hotbar") {
			anim.Play();
		}
	}
	*/
	public void OnItemPointerEnter() {
		InventoryEvents.SetHoveredItem(currentItem, this);
	}

	public void OnItemPointerExit() {
		InventoryEvents.LeaveHoveredItem();
	}


	public void LoadCreativeMode() {
		mode = 1;
		amountText.gameObject.SetActive(false);
	}

	public void PlayAnim ()
    {
		anim.Play();
    }

	public void SetSlotID (int id)
    {
		slotID = id;
    }

	public int GetSlotID ()
    {
		return slotID;
    }

	public void StartDrag ()
    {
		InventoryEvents.StartDrag(this);
    }

	public void EndDrag ()
    {
		InventoryEvents.EndDrag(this);
    }
}
