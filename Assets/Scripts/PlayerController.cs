using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.PostProcessing;
using EZCameraShake;

public class PlayerController : MonoBehaviour, ITakeFallDamage
{
	[Header ("Movement")]
	public float mouseSensitivity;
	public float walkSpeed;
	public float runSpeed;
	public float jumpForce;
	[SerializeField] float jumpInertia;
	[SerializeField] float airMult; //how much movement is multiplied while in the air
	bool jumpReady;
	public float smoothTime;
	public LayerMask groundedMask;

	[Header ("Health and Food")]
	public float maxHealth = 100f;
	public float health;
	public float maxHunger = 100f;
	public float hunger;

	public float hungerLoss = 0.1f;
	public float hungerDamage = 2f;

	public float fullLevel = 90f;
	public float fullHealthRegainAmount = 1f;

	[Header ("Falling and Interactions")]
	public float fallVelocityToDamage = 9.5f;
	public float fallDamage = 1.5f;

	public float handDamage = 15f;
	public float interactRange = 2f;
	public Item fuelItem;
	int lastEatenItem = -1; //to support eating varieties of items

	[Header ("Misc")]
	public WorldManager.WorldType currentWorld;

	public GameObject playerCameraHolder;
	public GameObject playerCamera;
	public GameObject handContainer;
	public Animator handAnimator;
	public Animation damagePanelAnim;
	public GameObject infoText;

	[HideInInspector] public Rigidbody rb;

	public ProgressManager progressUI;

	public Text noticeText;

	public Transform purgatorySpawn;
	public TextMesh purgatoryRespawnTimeText;

	public GameObject lightDeactivateObjects;
	public GameObject darkDeactivateObjects;

	[HideInInspector] public PostProcessingBehaviour playerCameraPostProcessingBehaviour;

	public PostProcessingProfile lightPostProcessingProfile;
	public PostProcessingProfile darkPostProcessingProfile;

	[SerializeField] float voidHeight;
	public Transform lightWorldEnterPoint;
	[SerializeField] Vector3 lightWorldEnterVelocity;
	public Transform darkWorldEnterPoint;
	[SerializeField] Vector3 darkWorldEnterVelocity;

	public Text realmNoticeText;
	public Color lightRealmNoticeTextColor;
	public Color darkRealmNoticeTextColor;

	public ParticleSystem useParticles;

	private float verticalLookRotation;

	float smoothedYVelocity;
	float currentMoveSpeed;
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;

	[HideInInspector] public Inventory inventory;
	AchievementManager achievementManager;
	AudioManager audioManager;
	PauseManager pauseManager;
	SettingsManager settingsManager;

	public GameObject canvas;
	Animator canvasAnim;

	[HideInInspector] public bool grounded;

	[HideInInspector] public float distanceToTarget; // Accesible from other GameObjects to see if they are in range of interaction and such
	[HideInInspector] public GameObject target;
	[HideInInspector] public RaycastHit targetHit;

	GameObject currentHandObj;

	Vector3 spawnpoint;

	Quaternion startRot;

	bool lockLook;
	bool gathering = false;
	float gatheringTime;

	bool pickingUp = false;
	float pickingUpTime;

	bool consuming = false;
	float consumeTime;

	bool firing = false;
	float drawTime;

	int difficulty;
	int mode;

	Color defaultFogColor;
	float defaultFogDensity;
	Color defaultPlayerCameraColor;

	float nextTimeToRespawn;
	float respawnTime = 7.5f;

	[HideInInspector] public bool dead = false;

	public Image healthAmountImage;
	public Image hungerAmountImage;

	bool ignoreFallDamage;
	bool climbing;

	public GameObject rain;

	float flyDoubleTapCooldown;
	bool flying;

	float interactKeyHeldDelay = 0.6f; // time until "F" / interact key can be held to put in lots of items / interact alot.
	float interactKeyHeldTime; //when time is over this value, if f key is held, constantly try to add an item.
	bool interactKeyPressed;

	WeaponHandler currentWeaponHandler;
	Animator currentWeaponAnimator;

	ResourceHandler currentResource;

	//PostProcessingProfile defaultLightProfile;
	//PostProcessingProfile defaultDarkProfile;

	float originalDrag;
	float flyingDrag = 10f;

	PersistentData persistentData;

	void Awake()
	{
		GameObject scriptHolder = GameObject.FindGameObjectWithTag("ScriptHolder");
		audioManager = scriptHolder.GetComponent<AudioManager>();
		pauseManager = scriptHolder.GetComponent<PauseManager>();
		achievementManager = scriptHolder.GetComponent<AchievementManager>();
		settingsManager = scriptHolder.GetComponent<SettingsManager>();
		canvasAnim = canvas.GetComponent<Animator>();

		rb = GetComponent<Rigidbody>();
		inventory = GetComponent<Inventory>();
		originalDrag = rb.drag;
		playerCameraPostProcessingBehaviour = playerCamera.GetComponent<PostProcessingBehaviour>();

		defaultFogColor = RenderSettings.fogColor;
		defaultFogDensity = RenderSettings.fogDensity;
		defaultPlayerCameraColor = playerCamera.GetComponent<Camera>().backgroundColor;
	}

	void Start()
	{
		health = maxHealth;
		hunger = maxHunger;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		LockLook(false);

		//defaultLightProfile = lightPostProcessingProfile;
		//defaultDarkProfile = darkPostProcessingProfile;

		difficulty = FindObjectOfType<SaveManager>().difficulty;

		persistentData = FindObjectOfType<PersistentData>();
		if (!dead && !persistentData.loadSave)
		{
			currentWorld = WorldManager.WorldType.Light;
			EnterLightWorld();
		} else
        {
			IgnoreNextFall();
		}
	}

	public void InventoryUpdate()
	{
		if (inventory.currentSelectedItem)
		{
			if (currentHandObj)
			{
				Destroy(currentHandObj);
			}
			GameObject obj = Instantiate(inventory.GetHandObject(), handContainer.transform) as GameObject;
			obj.transform.Rotate(inventory.currentSelectedItem.handRotation);
			obj.transform.localScale = inventory.currentSelectedItem.handScale;
			Rigidbody objRB = obj.GetComponent<Rigidbody>();
			if (objRB)
			{
				Destroy(objRB);
			}
			Collider[] cols = obj.GetComponentsInChildren<Collider>();
			if (cols.Length > 0)
			{
				foreach (Collider col in cols)
				{
					col.enabled = false;
				}
			}
			AudioSource[] audioSources = obj.GetComponentsInChildren<AudioSource>();
			if (audioSources.Length > 0)
			{
				foreach (AudioSource audio in audioSources)
				{
					audio.enabled = false;
				}
			}
			if (inventory.currentSelectedItem.id == 23)
			{ // AutoMiner
				obj.GetComponent<AutoMiner>().enabled = false;
				obj.GetComponent<NavMeshAgent>().enabled = false;
			}
			else if (inventory.currentSelectedItem.id == 32 || inventory.currentSelectedItem.id == 33)
			{ // Bucket or WaterBucket
				Bucket bucket = obj.GetComponent<Bucket>();
				if (bucket)
				{
					bucket.enabled = false;
				}
			}
			else if (inventory.currentSelectedItem.id == 20)
			{ // Lightning Stone
				LightningStone ls = obj.GetComponent<LightningStone>();
				if (ls)
				{
					ls.enabled = false;
				}
			}
			else if (inventory.currentSelectedItem.id == 35)
			{
				LightItem li = obj.GetComponent<LightItem>();
				if (li)
				{
					li.enabled = false;
				}
			}
			obj.tag = "Untagged";
			foreach (Transform trans in obj.transform)
			{
				trans.tag = "Untagged";
			}

			currentHandObj = obj;

			if (inventory.currentSelectedItem.type == Item.ItemType.Weapon)
			{
				currentWeaponHandler = currentHandObj.GetComponent<WeaponHandler>();
				currentWeaponAnimator = currentHandObj.GetComponent<Animator>();
			}
			else
			{
				currentWeaponHandler = null;
				currentWeaponAnimator = null;
			}

		}
		else if (currentHandObj)
		{
			Destroy(currentHandObj);
			currentHandObj = null;
			currentWeaponHandler = null;
		}
	}

	void Update()
	{
		ManageBuildingInteractKey();

		if (!dead && mode != 1)
		{
			LoseCalories(Time.deltaTime * hungerLoss);
			if (hunger <= 10f)
			{
				TakeEffectDamage(Time.deltaTime * hungerDamage);
				if (!infoText.activeSelf)
				{
					infoText.SetActive(true);
				}
			}
			else if (infoText.activeSelf)
			{
				infoText.SetActive(false);
			}
		}

		if (hunger >= fullLevel)
		{
			GainHealth(Time.deltaTime * fullHealthRegainAmount);
		}

		if (!lockLook)
		{
			transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
			verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
			verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90);
			playerCameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
		}

		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

		if (Input.GetButton("Sprint"))
		{
			currentMoveSpeed = runSpeed;
		}
		else
		{
			currentMoveSpeed = walkSpeed;
		}

		if (Input.GetButtonDown("Jump"))
		{
			jumpReady = true;
			if (mode == 1)
			{
				if (flyDoubleTapCooldown > 0f)
				{
					if (flying)
					{
						StopFlying();
					}
					else
					{
						StartFlying();
					}
				}
				else
				{
					flyDoubleTapCooldown = 0.5f;
				}
			}
		}

		if (flying)
		{ // In future make axis!
			if (Input.GetButton("Jump"))
			{
				moveDir.y = 1;
			}
			if (Input.GetButton("Crouch"))
			{
				moveDir.y = -1;
			}
		}

		Vector3 targetMoveAmount = moveDir * currentMoveSpeed;

		moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, smoothTime);

		flyDoubleTapCooldown -= Time.deltaTime;

		if (Input.GetKey(KeyCode.C))
		{
			if (canvas.activeSelf)
			{
				canvas.SetActive(false);
			}
		}
		else
		{
			if (!canvas.activeSelf)
			{
				canvas.SetActive(true);
			}
		}

		RaycastHit hit;
		Ray ray = playerCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

		Physics.Raycast(ray, out hit);

		distanceToTarget = hit.distance;

		bool hideNoticeText = false;
		string noticeText = string.Empty;

		if (!ActiveMenu())
		{
			if (currentWeaponHandler && currentWeaponHandler.weapon.type == Weapon.WeaponType.Bow)
			{
				if (!firing)
				{
					firing = true;
					currentWeaponAnimator.SetBool("PullingBack", true);
				}
				if (Input.GetMouseButton(2) || Input.GetAxisRaw("ControllerTriggers") >= 0.1f)
				{
					if (drawTime < currentWeaponHandler.weapon.chargeTime)
					{
						drawTime++;
					}
				}
			}
			else if (firing)
			{
				firing = false;
				currentWeaponAnimator.SetTrigger("Fire");
				currentWeaponAnimator.SetBool("PullingBack", false);
			}

			if (hit.collider)
			{
				target = hit.collider.gameObject;
				targetHit = hit;

				if (Input.GetMouseButtonDown(0) || Input.GetAxisRaw("ControllerTriggers") <= -0.1f)
				{
					if (hit.collider.CompareTag("Resource"))
					{
						if (distanceToTarget >= interactRange)
						{
							Attack();
						}
					}
					else
					{
						Attack();
					}
				}

				if (target.CompareTag("Item") && distanceToTarget <= interactRange && !inventory.placingStructure)
				{
					AutoMiner autoMiner = null;
					ItemHandler itemHandler = target.GetComponentInParent<ItemHandler>();
					if (itemHandler.item.id == 12)
					{ // Is it a crate?
						noticeText = "Hold [E] to pick up, [F] to open";
						if (Input.GetButton("Interact"))
						{
							itemHandler.GetComponent<LootContainer>().Open();
							achievementManager.GetAchievement(7); // Looter Achievement
						}
					}
					else if (itemHandler.item.id == 23)
					{ // Auto Miner

						autoMiner = itemHandler.GetComponent<AutoMiner>();

						if (inventory.currentSelectedItem && inventory.currentSelectedItem.type == Item.ItemType.Tool)
						{
							noticeText = "Hold [E] to pick up, [F] to replace tool";

							if (Input.GetButton("Interact"))
							{
								autoMiner.SetTool(inventory.currentSelectedItem);
								inventory.hotbar.GetChild(inventory.selectedHotbarSlot).GetComponent<InventorySlot>().DecreaseItem(1);
								inventory.InventoryUpdate();
							}

						}
						else
						{
							noticeText = "Hold [E] to pick up, [F] to gather items";
							if (Input.GetButton("Interact"))
							{
								int i = 0;
								foreach (Item item in autoMiner.items)
								{
									inventory.AddItem(item, autoMiner.itemAmounts[i]);
									i++;
								}

								audioManager.Play("Grab");

								autoMiner.ClearItems();
							}
						}
					}
					else if (itemHandler.item.id == 24)
					{ // Door
						Door door = itemHandler.GetComponent<Door>();
						if (door.open)
						{
							noticeText = "Hold [E] to pick up, [F] to close door";
						}
						else
						{
							noticeText = "Hold [E] to pick up, [F] to open door";
						}
						if (Input.GetButtonDown("Interact"))
						{
							door.ToggleOpen();
						}
					}
					else if (itemHandler.item.subType == Item.ItemSubType.Conveyor)
					{ // Conveyor belt
						ConveyorBelt conveyor = itemHandler.GetComponent<ConveyorBelt>();
						noticeText = "Hold [E] to pick up, [F] to change speed. Current speed is " + conveyor.speeds[conveyor.speedNum];
						if (Input.GetButtonDown("Interact"))
						{
							conveyor.IncreaseSpeed();
						}
					}
					else if (itemHandler.item.id == 27)
					{ // Auto Sorter
						AutoSorter sorter = itemHandler.GetComponent<AutoSorter>();
						if (inventory.currentSelectedItem)
						{
							noticeText = "Hold [E] to pick up, [F] set sorting item";
							if (Input.GetButtonDown("Interact"))
							{
								sorter.SetItem(inventory.currentSelectedItem);
							}
						}
						else
						{
							noticeText = "Hold [E] to pick up";
						}
					}
					else if (itemHandler.item.id == 30)
					{ // Radio
						Radio radio = itemHandler.GetComponent<Radio>();
						if (radio.songNum == -1)
						{
							noticeText = "Hold [E] to pick up, [F] to turn on";
						}
						else if (radio.songNum == radio.songs.Length - 1)
						{
							noticeText = "Hold [E] to pick up, [F] to turn off. Currently playing " + radio.songs[radio.songNum].name;
						}
						else
						{
							noticeText = "Hold [E] to pick up, [F] to change song. Currently playing " + radio.songs[radio.songNum].name;
						}
						if (Input.GetButtonDown("Interact"))
						{
							radio.ChangeSong();
							achievementManager.GetAchievement(11); // Groovy achievement
						}
					}
					else if (itemHandler.item.id == 35)
					{ // Light
						LightItem li = itemHandler.GetComponent<LightItem>();
						if (li.intensities[li.intensityNum] == 0)
						{
							noticeText = "Hold [E] to pick up, [F] to turn on";
						}
						else if (li.intensityNum == li.intensities.Length - 1)
						{
							noticeText = "Hold [E] to pick up, [F] to turn off. Current brightness is " + li.intensities[li.intensityNum];
						}
						else
						{
							noticeText = "Hold [E] to pick up, [F] to change brightness. Current brightness is " + li.intensities[li.intensityNum];
						}
						if (Input.GetButtonDown("Interact"))
						{
							li.IncreaseIntensity();
						}
					} else if (itemHandler.item.type == Item.ItemType.Structure && itemHandler.item.subType == Item.ItemSubType.Furnace) //furnace
                    {
						noticeText = "Hold [E] to pick up, [F] to add fuel / ore";
						if (interactKeyPressed)
						{
							Furnace furnace = itemHandler.GetComponent<Furnace>();
							if (inventory.slots[inventory.selectedHotbarSlot].currentItem)
							{
								if (inventory.slots[inventory.selectedHotbarSlot].currentItem.fuel > 0 || inventory.slots[inventory.selectedHotbarSlot].currentItem.smeltItem)
								{
									inventory.TakeHeldItem(out Item thisItem, out int stackTaken, 1);
									inventory.InventoryUpdate();
									furnace.AddItem(thisItem, stackTaken);
								}
							}
						}
					}
					else if (itemHandler.item.type == Item.ItemType.Structure && itemHandler.item.subType == Item.ItemSubType.Storage)
					{
						bool currentItemExists = false;
						if (inventory.slots[inventory.selectedHotbarSlot].currentItem)
						{
							currentItemExists = true;
						}
						if (currentItemExists)
                        {
							noticeText = "Hold [E] to pick up, [F] to (try) add item";
						} else
                        {
							noticeText = "Hold [E] to pick up, [F] to take item";
						}
						if (interactKeyPressed)
						{
							ItemHolder holder = itemHandler.GetComponent<ItemHolder>();
							if (currentItemExists)
							{
								if (inventory.GetHeldItemStack() == 0)
                                {
									StopInteractKeyRepeat();
                                }
								inventory.TakeHeldItem(out Item thisItem, out int stackTaken, 1);
								if (stackTaken > 0)
								{
									bool failed = false;
									holder.AddItem(thisItem, 1, out failed);
									if (failed)
									{
										inventory.AddItem(thisItem, 1);
									}
									else
									{
										
									}
								}
								inventory.InventoryUpdate();
							}
							else
							{
								if (holder.GetItem())
								{
									inventory.AddItem(holder.GetItem(), 1);
									holder.RemoveItem(1);
									inventory.InventoryUpdate();
								}
							}
						}
					} else if (itemHandler.item.type == Item.ItemType.Structure && itemHandler.item.subType == Item.ItemSubType.Incinerator)
                    {
						noticeText = "Hold [E] to pick up, [F] to incinerate item";
						if (interactKeyPressed)
						{
							Incinerator incinerator = itemHandler.GetComponent<Incinerator>();
							if (incinerator)
							{
								inventory.TakeHeldItem(out Item thisItem, out int stackTaken, 1);
								if (stackTaken > 0)
								{
									incinerator.IncinerateEffects();
								}
							}
						}
					}
					else
					{
						noticeText = "Hold [E] to pick up";
					}
					if (Input.GetButton("PickUp"))
					{
						if (!pickingUp || (pickingUp && !progressUI.UIEnabled()))
						{
							progressUI.EnableUI();
							pickingUp = true;
						}
						else
						{
							pickingUpTime += Time.deltaTime;
							progressUI.UpdateProgress(pickingUpTime / itemHandler.item.timeToGather, (itemHandler.item.timeToGather - pickingUpTime));

							if (pickingUpTime >= itemHandler.item.timeToGather)
							{
								if (autoMiner)
								{
									if (autoMiner.currentToolItem)
									{
										inventory.AddItem(autoMiner.GatherTool(), 1);
									}
									int q = 0;
									foreach (Item item in autoMiner.items)
									{
										inventory.AddItem(item, autoMiner.itemAmounts[q]);
										q++;
									}
								}
								if (itemHandler.item.type == Item.ItemType.Structure && itemHandler.item.subType == Item.ItemSubType.Furnace)
								{ // Is it a furnace?
									Furnace furnace = itemHandler.GetComponent<Furnace>();
									if (furnace)
									{
										inventory.AddItem(fuelItem, (int)Mathf.Floor(furnace.fuel)); // ONLY WORKS IF WOOD IS ONLY FUEL SOURCE
										if (furnace.currentSmeltingItem.Count > 0)
										{
											for (int i = 0; i < furnace.currentSmeltingItem.Count; i++) {
												inventory.AddItem(furnace.currentSmeltingItem[i], 1);
											}
										}
									}
								}
								if (itemHandler.item.type == Item.ItemType.Structure && itemHandler.item.subType == Item.ItemSubType.Storage)
                                {
									ItemHolder holder = itemHandler.GetComponent<ItemHolder>();
									if (holder.GetItem())
									{
										inventory.AddItem(holder.GetItem(), holder.ItemStack());
									}
                                }
								pickingUpTime = 0f;
								progressUI.UpdateProgress(0, 0);
								inventory.Pickup(itemHandler);
								audioManager.Play("Grab");
							}
						}
					}
					else if (pickingUp)
					{
						hideNoticeText = true;
						pickingUpTime = 0f;
						progressUI.UpdateProgress(0, 0);
						progressUI.DisableUI();
						pickingUp = false;
					}
				}
				else if (target.CompareTag("Resource") && distanceToTarget <= interactRange && !inventory.placingStructure)
				{ // Gather resources
					ResourceHandler resHand = target.GetComponent<ResourceHandler>();
					bool canMine = true;
                    if (inventory.slots[inventory.selectedHotbarSlot].currentItem && inventory.slots[inventory.selectedHotbarSlot].currentItem.toolToughness >= resHand.resource.toughness || resHand.resource.toughness == 0)
                    {
                        if (resHand.enabled)
                        {
                            noticeText = "Hold [LMB] to gather";
                            canMine = true;
                        }
                        else
                        {
                            noticeText = "Unable to gather";
                            canMine = false;
                        }
                    }
                    else
                    {
						noticeText = "Higher Tool Tier Needed";
						canMine = false;
					}
					if (Input.GetMouseButton(0) || Input.GetAxisRaw("ControllerTriggers") <= -0.1f)
					{
						if (canMine)
						{
							hideNoticeText = true;
							if (!gathering || (gathering && !progressUI.UIEnabled()))
							{
								gathering = true;
								progressUI.EnableUI();
							}
							else
							{
								float multiplier = 1f;
								if (inventory.currentSelectedItem && inventory.currentSelectedItem.type == Item.ItemType.Tool)
								{
									multiplier = inventory.currentSelectedItem.speed;
								}
								gatheringTime += Time.deltaTime;
								if (currentResource == null || currentResource.gameObject != target)
								{
									currentResource = target.GetComponent<ResourceHandler>();
								}
								progressUI.UpdateProgress(gatheringTime / (currentResource.resource.gatherTime / multiplier), currentResource.resource.gatherTime / multiplier - gatheringTime);
								if (gatheringTime >= currentResource.resource.gatherTime / multiplier)
								{
									int gathered = currentResource.ToolGather(inventory.currentSelectedItem, out Item item);
									inventory.AddItem(item, gathered);

									gatheringTime = 0f;
									progressUI.UpdateProgress(0, 0);
									CameraShaker.Instance.ShakeOnce(2f, 3f, 0.1f, 0.3f);
								}
								else
								{
									noticeText = "Still Growing";
								}
							}
						}
					}
					else if (gathering || pickingUp)
					{
						CancelGatherAndPickup();
						hideNoticeText = true;
					}
				} else if (target.CompareTag("WorldButton") && distanceToTarget <= interactRange && !inventory.placingStructure)
                {
					WorldButton wb = target.GetComponent<WorldButton>();
					noticeText = wb.DisplayText();
					if (Input.GetButtonDown("Interact"))
                    {
						wb.Interact();
                    }
                }
				else if (gathering || pickingUp)
				{
					CancelGatherAndPickup();
					hideNoticeText = true;
				}
				else
				{
					hideNoticeText = true;
				}
			}
			else
			{
				if (gathering || pickingUp)
				{
					CancelGatherAndPickup();
					hideNoticeText = true;
				}
				if (Input.GetMouseButtonDown(0) || Input.GetAxisRaw("ControllerTriggers") <= -0.1f)
				{
					Attack();
				}
				target = null;
				hideNoticeText = true;
				if (progressUI.UIEnabled())
				{
					progressUI.DisableUI();
				}
			}
		}

		handAnimator.SetBool("Gathering", gathering);

		if (hideNoticeText)
		{
			if (!inventory.placingStructure)
			{
				HideNoticeText();
			}
		}
		else
		{
			ShowNoticeText(noticeText);
		}

		if (transform.position.y < -voidHeight)
		{
			if (currentWorld == WorldManager.WorldType.Light)
			{
				EnterDarkWorld();
				achievementManager.GetAchievement(10); // Achievement: Explorer
			}
			else
			{
				EnterLightWorld();
			}
		}

		if (dead)
		{
			purgatoryRespawnTimeText.text = (-(Time.time - nextTimeToRespawn)).ToString("0");
			if (Time.time >= nextTimeToRespawn)
			{
				Respawn();
			}
		}
	}

	void ManageBuildingInteractKey () //NOTE: only use interactKeyPressed if you are dealing with lots of something.
    {
		interactKeyPressed = false;
		if (Input.GetButtonDown("Interact"))
        {
			interactKeyHeldTime = Time.time + interactKeyHeldDelay;
			interactKeyPressed = true;
        }

		if (Time.time > interactKeyHeldTime)
		{
			if (Input.GetButton("Interact")) {
				if (interactKeyPressed)//should half the "true" returns that interactKeyPressed should give
				{
					interactKeyPressed = false;
				}
				else
				{
					interactKeyPressed = true;
				}
			} else
            {
				interactKeyHeldTime = Time.time + 10000f;
            }
		}
    }

	void StopInteractKeyRepeat () //mainly for preventing storage boxes from having some... weird behaviour
    {
		interactKeyHeldTime = Time.time + interactKeyHeldDelay;
	}

	public void LoadCreativeMode()
	{
		mode = 1;
		hungerAmountImage.gameObject.SetActive(false);
		healthAmountImage.gameObject.SetActive(false);
	}

	public void Consume(Item item) //or eat, comment is here for reference
	{
		//handAnimator.SetTrigger("Consume");
		useParticles.Play();
		if (lastEatenItem == item.id)
		{
			GainCalories(item.calories - item.boringFoodPenalty);
		} else
        {
			GainCalories(item.calories);
		}
		lastEatenItem = item.id;
	}

	public bool ActiveMenu()
	{
		return inventory.inventoryContainer.activeSelf || ActiveSystemMenu();
	}

	public bool ActiveSystemMenu()
	{
		return pauseManager.paused;
	}

	void Attack()
	{
		if (handAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "PlayerAttack")
		{ // IF PLAYERATTACK IS RENAMED, THIS WILL NOT WORK
			handAnimator.SetTrigger("Attack");
			if (target)
			{
				Health targetHealth = target.GetComponent<Health>();
				if (targetHealth)
				{
					if (inventory.currentSelectedItem)
					{
						WeaponHandler handler = inventory.currentSelectedItem.prefab.GetComponent<WeaponHandler>();
						if (handler && handler.weapon.type == Weapon.WeaponType.Melee)
						{
							targetHealth.TakeDamage(handler.weapon.damage);
						}
					}
					else
					{
						targetHealth.TakeDamage(handDamage);
					}
				}
			}
		}
	}

	void OnCollisionEnter(Collision col)
	{
		//WAS used for taking fall damage
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Item"))
		{
			ItemHandler itemHandler = other.GetComponentInParent<ItemHandler>();
			if (itemHandler)
			{
				if (itemHandler.item.id == 36)
				{ // Ladder
					StartClimbing();
				}
			}
		}
		if (other.CompareTag("Damage Player"))
        {
			PlayerDamage damage = other.GetComponent<PlayerDamage>();
			if (damage)
            {
				TakeDamage(damage.GetDamage());
            }
        }
	}

	void OnTriggerStay(Collider other)
	{
		if (!climbing)
		{
			if (other.CompareTag("Item"))
			{
				ItemHandler itemHandler = other.GetComponentInParent<ItemHandler>();
				if (itemHandler)
				{
					if (itemHandler.item.id == 36)
					{ // Ladder
						StartClimbing();
					}
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		StopClimbing();
	}

	void StartClimbing()
	{
		climbing = true;
		rb.useGravity = false;
	}

	void StopClimbing()
	{
		climbing = false;
		if (!flying)
		{
			rb.useGravity = true;
		}
	}

	void StartFlying()
	{
		flying = true;
		rb.velocity = Vector3.zero;
		rb.useGravity = false;
		rb.drag = flyingDrag;
	}

	void StopFlying()
	{
		flying = false;
		rb.drag = originalDrag;
		if (!climbing)
		{
			rb.useGravity = true;
		}
	}

	void ResetIgnoreFallDamage()
	{
		ignoreFallDamage = false;
	}

	void GainCalories(float amount)
	{
		hunger += amount;
		if (hunger > maxHunger)
		{
			hunger = maxHunger;
		}
		HungerChange();
	}

	void LoseCalories(float amount)
	{
		hunger -= amount;
		HungerChange();
		if (hunger < 0f)
		{
			hunger = 0f;
		}
	}

	void GainHealth(float amount)
	{
		health += amount;
		if (health > maxHealth)
		{
			health = maxHealth;
		}
		HealthChange();
	}

	public void TakeDamage(float amount)
	{
		if (mode != 1)
		{
			health -= amount;
			CameraShaker.Instance.ShakeOnce(4f, 5f, 0.1f, 0.5f);
			damagePanelAnim.Play();
			if (health <= 0 && !dead)
			{
				Die();
			}
			else if (health < 0f)
			{
				health = 0f;
			}
			HealthChange();
		}
	}

	public void TakeFallDamage (float amount)
    {
		TakeDamage(amount);
    }

	public void TakeFallDamage(GameObject source)
    {
		NegateFallDamage nfd = source.GetComponent<NegateFallDamage>();
		if (nfd && !nfd.NegateDamage() || !nfd)
		{
			if (-smoothedYVelocity >= fallVelocityToDamage && !dead && mode != 1)
			{
				if (ignoreFallDamage)
				{
					Invoke("ResetIgnoreFallDamage", 1f);
				}
				else
				{
					if (nfd)
					{
						TakeDamage(Mathf.Pow(-smoothedYVelocity * fallDamage * nfd.GetFallDamageMult(), 1.39f));
					} else
                    {
						TakeDamage(Mathf.Pow(-smoothedYVelocity * fallDamage, 1.39f));
					}
				}
			}
		}
	}

	public void TakeEffectDamage(float amount)
	{
		health -= amount;
		if (health <= 0 && !dead)
		{
			Die();
		}
		else if (health < 0f)
		{
			health = 0f;
		}
		HealthChange();
	}

	void HungerChange()
	{
		hungerAmountImage.fillAmount = hunger / maxHunger;
	}

	void HealthChange()
	{
		healthAmountImage.fillAmount = health / maxHealth;
	}

	/*
	void ApplyHealthEffects() {
		MotionBlurModel.Settings motionBlurSettings = playerCameraPostProcessingBehaviour.profile.motionBlur.settings;
		motionBlurSettings.frameBlending = health / maxHealth;
		ColorGradingModel.Settings colorGradingSettings = playerCameraPostProcessingBehaviour.profile.colorGrading.settings;
		colorGradingSettings.basic.saturation = health / maxHealth;
		VignetteModel.Settings vignetteSettings = playerCameraPostProcessingBehaviour.profile.vignette.settings;
		vignetteSettings.smoothness = health / maxHealth;
		vignetteSettings.intensity = 1 - (health / maxHealth);
		playerCameraPostProcessingBehaviour.profile.motionBlur.settings = motionBlurSettings;
		playerCameraPostProcessingBehaviour.profile.vignette.settings = vignetteSettings;
		playerCameraPostProcessingBehaviour.profile.colorGrading.settings = colorGradingSettings;
	}
	*/

	void CancelGatherAndPickup()
	{
		gatheringTime = 0f;
		pickingUpTime = 0f;
		progressUI.UpdateProgress(0, 0);
		progressUI.DisableUI();
		gathering = false;
		pickingUp = false;
	}

	public void StopGathering ()
    {
		gatheringTime = 0f;
		progressUI.UpdateProgress(0, 0);
		progressUI.DisableUI();
		gathering = false;
	}

	void EnterLightWorld()
	{

		LoadLightWorld();

		transform.position = lightWorldEnterPoint.position;
		rb.velocity = lightWorldEnterVelocity;

		IgnoreNextFall();
		realmNoticeText.color = lightRealmNoticeTextColor;
		realmNoticeText.text = "ENTERING LIGHT REALM";
		canvasAnim.SetTrigger("RealmNoticeTextEnter");
		Invoke("HideRealmNoticeText", 3);
	}

	void EnterDarkWorld()
	{

		LoadDarkWorld();

		transform.position = darkWorldEnterPoint.position;
		rb.velocity = darkWorldEnterVelocity;

		IgnoreNextFall();
		realmNoticeText.color = darkRealmNoticeTextColor;
		realmNoticeText.text = "ENTERING DARK REALM";
		canvasAnim.SetTrigger("RealmNoticeTextEnter");
		Invoke("HideRealmNoticeText", 3);
	}

	public void IgnoreNextFall ()
    {
		ignoreFallDamage = true;
	}

	public void LoadLightWorld()
	{
		currentWorld = WorldManager.WorldType.Light;
		RenderSettings.fogColor = defaultFogColor;
		RenderSettings.fogDensity = defaultFogDensity;
		playerCamera.GetComponent<Camera>().backgroundColor = defaultPlayerCameraColor;
		lightDeactivateObjects.SetActive(true);
		darkDeactivateObjects.SetActive(false);
		playerCamera.GetComponent<PostProcessingBehaviour>().profile = lightPostProcessingProfile;
		rain.SetActive(false);
	}

	public void LoadDarkWorld()
	{
		currentWorld = WorldManager.WorldType.Dark;
		RenderSettings.fogColor = Color.black;
		RenderSettings.fogDensity = 0.04f;
		playerCamera.GetComponent<Camera>().backgroundColor = Color.black;
		darkDeactivateObjects.SetActive(true);
		lightDeactivateObjects.SetActive(false);
		playerCamera.GetComponent<PostProcessingBehaviour>().profile = darkPostProcessingProfile;
		rain.SetActive(true);
	}

	void HideRealmNoticeText()
	{
		canvasAnim.SetTrigger("RealmNoticeTextExit");
	}

	public void Die()
	{
		if (difficulty > 1)
		{
			inventory.ClearInventory();
		}
		if (inventory.placingStructure)
		{
			inventory.CancelStructurePlacement();
		}
		playerCameraPostProcessingBehaviour.profile = darkPostProcessingProfile;
		transform.position = purgatorySpawn.position;
		RenderSettings.fogColor = Color.black;
		RenderSettings.fogDensity = 0.1f;
		playerCamera.GetComponent<Camera>().backgroundColor = Color.black;
		nextTimeToRespawn = respawnTime + Time.time;
		dead = true;
	}

	void Respawn()
	{
		health = maxHealth;
		hunger = maxHunger;
		dead = false;
		if (currentWorld == WorldManager.WorldType.Light)
		{
			EnterLightWorld();
		}
		else
		{
			EnterDarkWorld();
		}
	}

	public void ShowNoticeText(string text)
	{
		if (!noticeText.gameObject.activeSelf)
		{
			noticeText.gameObject.SetActive(true);
		}
		noticeText.text = text;
	}

	public void HideNoticeText()
	{
		if (noticeText.gameObject.activeSelf)
		{
			noticeText.text = "";
			noticeText.gameObject.SetActive(false);
		}
	}

	public void LockLook(bool _lockLook)
	{
		lockLook = _lockLook;
	}

	void FixedUpdate()
	{
		if (grounded && !flying && jumpReady) //jump logic
		{
			rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
			rb.AddForce(transform.TransformDirection(moveAmount) * jumpInertia * 10f, ForceMode.Impulse); //to transfer movement speed into the jump; what gives the player a sort of "momentum"
		}

		if (grounded)
		{
			Collider[] cols = Physics.OverlapBox(transform.position - Vector3.up * 0.5f, 0.5f * Vector3.one);
			if (cols.Length - 2 < 1)
			{ // Doesn't seem to be ground near (Prevents flying). We do the -2 because the player and groundCheck collide with it.
				grounded = false;
			}
		}

		if (climbing)
		{
			Collider[] cols = Physics.OverlapCapsule(transform.position - Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f, 0.5f);
			if (cols.Length - 2 < 1)
			{ // Doesn't seem to be a ladder near (Prevents flying). We do the -2 because the player and groundCheck collide with it.
				StopClimbing();
			}

			rb.MovePosition(rb.position + (transform.TransformDirection(Vector3.right * moveAmount.x + Vector3.up * moveAmount.y + (Vector3.forward * (moveAmount.z > 1 ? moveAmount.z : 0))) + Vector3.up * moveAmount.z) * Time.fixedDeltaTime);
			rb.velocity = Vector3.zero;
		}
		else if (grounded || flying) //Normal movement on the ground or flying
		{
			rb.MovePosition(rb.position + (transform.TransformDirection(moveAmount) * Time.fixedDeltaTime));
		} else //movement in the air
        {
			rb.MovePosition(rb.position + (transform.TransformDirection(moveAmount) * Time.fixedDeltaTime * airMult));
		}
		smoothedYVelocity = (smoothedYVelocity + rb.velocity.y) / 2;
		jumpReady = false; //this is put here so jumps can't be "queued"
	}

	public void SetLastEatenItem (int id)
    {
		lastEatenItem = id;
    }

	public int GetLastEatenItem ()
    {
		return lastEatenItem;
    }
}