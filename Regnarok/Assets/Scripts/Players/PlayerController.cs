using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Experimental.VFX;
using UnityEngine.VFX;
using System.IO;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    PhotonView pv;

    //attackStats
    [SerializeField] float totalDamage, totalAttackSpeed, totalCritChance, totalLifeSteal, totalChanceToInflictBleed, totalHealthOnKill, totalExtraSpeed, totalJumps = 1, executeBelow, nimbusStacks, burnTicks, poisonChance;
    public Item heldItem;
    float remainingJumps = 1;

    //movement
    [SerializeField] float speed, sprintSpeed, weight, jumpForce, combinedSpeed;
    //stamina base
    [SerializeField] float baseStaminaLossPerSec, baseStaminaGainedPerSec, maxStamina;
    //stats to use
    float gravity = -1, staminaValue, staminaLossPerSec, staminaGainedPerSec;

    bool groundCheck = false;
    float groundCheckTime;
    Vector3 movementSpeed, movementDirection;
    [HideInInspector] public ChestInventory lastChest;
    [HideInInspector] public CraftingStation lastCratingStation;
    [HideInInspector] public OvenStation lastOvenStation;
    //CraftStation craftStation;

    //camera
    [SerializeField] GameObject head;
    [SerializeField] LayerMask playerAimMask;
    [SerializeField] Camera cam;
    [SerializeField] float mouseSensitivity, extraMouseSense, pitchDown, pitchUp;
    float cameraPitch;
    bool InventoryIsOpen;

    //third person
    bool isThirdPerson;
    [SerializeField] float pitchDownThird, pitchUpThird, turnSmoothTime = 0.1f, turnSmoothVelocity;
    [SerializeField] float distance, turnSpeed, minDistance, maxDistance;
    [SerializeField] Transform camOriginpos;

    //attacks
    [SerializeField] Transform attackPos;
    [SerializeField] float attackRadius = 1;

    //UI
    [SerializeField] Slider staminaSlider;

    //visual graph test
    public GameObject testGraph;

    public bool mayAttack;

    public Animator animController;

    public GameObject nameOfPlayer;
    GameObject localplayerObject;

    public bool placementCheck;
    float placementRotation;
    public Vector3 rotationddd;
    GameObject ghostplacement;
    [SerializeField] List<GameObject> ghostList;
    [SerializeField] List<GameObject> actualItemList;
    int selectedplacement;

    float eatCooldown;
    public float eatTime;
    bool eatingOnCooldown;
    [SerializeField] Slider foodCooldownSlider;
    float foodCooldownTimeSteps;

    public bool isDead;

    public GameObject jumpPartical;
    public GameObject runningPartical;

    private int itemSpawnedIn;

    public GameObject pressE;

    public List<StatDisplay> statTexts;
    [Header("sounds")]
    public AudioSource jump_Audio;
    public AudioSource walking_Audio;
    public AudioSource running_Audio;
    public AudioSource itemPickup_audio;
    public AudioSource itemDrop_audio;
    public AudioSource eatingSound;
    public AudioSource attackSound;
    public AudioSource coinpickUp;
    public AudioSource pressedSound;
    public AudioSource hoverSound;

    //summon
    public Queue<GameObject> listOfSummons;
    public int maxSummons = 1;

    //nimbus
    public GameObject nimbusObject;
    public AudioSource hitObjectSound;
    public List<AudioClip> hitObjectSounds;

    public AudioMixer audioMaster;

    //debug
    public GameObject doNotHitThis;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        cam.gameObject.GetComponent<Camera>().enabled = false;
        if (pv.IsMine)
        {
            controller = GetComponent<CharacterController>();
            //cursor off
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            //stamina
            staminaValue = maxStamina;
            staminaLossPerSec = baseStaminaLossPerSec;
            staminaGainedPerSec = baseStaminaGainedPerSec;
            nameOfPlayer.SetActive(false);
            head.SetActive(false);

            mayAttack = true;
            camOriginpos.position = cam.transform.position;
            cam.gameObject.GetComponent<Camera>().enabled = true;
        }
        else
        {
            cam.gameObject.GetComponent<Camera>().enabled = false;
            Destroy(cam.GetComponent<AudioListener>());
            nameOfPlayer.SetActive(true);
            nameOfPlayer.GetComponentInChildren<TextMeshProUGUI>().text = pv.Owner.NickName;
        }
        FindObjectOfType<GameManager>().playerObjectList.Add(gameObject);
        listOfSummons = new Queue<GameObject>();
    }
    private void Start()
    {
        extraMouseSense = 5;
    }
    public void RecieveStats(float _damage, float _attackSpeed, float _critChance, float _lifesteal, float _bleedChance, float _healthOnKill, float _movementSpeed, int _jumps, float _executeBelow, int nimbus, int _burnTicks, float _poisonStacks)
    {
        totalDamage = _damage;
        totalAttackSpeed = _attackSpeed;
        totalCritChance = _critChance;
        totalLifeSteal = _lifesteal;
        totalChanceToInflictBleed = _bleedChance;
        totalHealthOnKill = _healthOnKill;
        totalExtraSpeed = _movementSpeed;
        totalJumps = _jumps;
        executeBelow = _executeBelow;
        nimbusStacks = nimbus;
        burnTicks = _burnTicks;
        poisonChance = _poisonStacks;

        //ui stats
        statTexts[0].GiveStats(GetComponent<CharacterStats>().level.ToString());
        statTexts[1].GiveStats(totalDamage.ToString());
        statTexts[2].GiveStats(totalAttackSpeed.ToString());
        statTexts[3].GiveStats(totalCritChance.ToString());
        statTexts[4].GiveStats(totalChanceToInflictBleed.ToString());
        statTexts[5].GiveStats(GetComponent<Health>().maxHealth.ToString());
        statTexts[6].GiveStats(GetComponent<Health>().armor.ToString());
        statTexts[7].GiveStats(totalHealthOnKill.ToString());
        statTexts[8].GiveStats((totalExtraSpeed + speed).ToString());
    }
    private void Update()
    {
        if (pv.IsMine)
        {
            if(isDead)
            {
                return;
            }
            Movement();
            Gravity();
            if (!InventoryIsOpen)
            {
                EatFood();
                SummonAlly();
                Rotation();
                CheckForInfo();
            }
            //andere onzin
            if (Input.GetButtonDown("Fire1"))
            {
                if (!InventoryIsOpen)
                {
                    if (!placementCheck)
                    {
                        if (mayAttack)
                        {
                            StartCoroutine(AttackStuckFix());
                            Anim_attack();
                        }
                    }
                    else
                    {
                        PlaceItem();
                    }
                }
            }
            if (Input.GetButtonDown("Jump"))
            {
                if (remainingJumps > 0)
                {
                    jump_Audio.Play();

                    Jump();
                    Anim_Jump();
                    StartCoroutine(JumpPartic());
                }
            }
            //check for chestDistance
            if (lastChest != null)
            {
                float distance = Vector3.Distance(transform.position, lastChest.transform.position);
                if (distance > 5)
                {
                    lastChest.CloseChestInventory();
                    lastChest = null;
                }
            }
            if (lastCratingStation != null)
            {
                float distance = Vector3.Distance(transform.position, lastCratingStation.transform.position);
                if (distance > 5)
                {
                    lastCratingStation.CloseChestInventory();
                    lastCratingStation = null;
                }
            }
            if (lastOvenStation != null)
            {
                float distance = Vector3.Distance(transform.position, lastOvenStation.transform.position);
                if (distance > 5)
                {
                    lastOvenStation.CloseChestInventory();
                    lastOvenStation = null;
                }
            }
        }
        else
        {
            if(localplayerObject != null)
            {
                nameOfPlayer.transform.LookAt(localplayerObject.transform, transform.up);
            }
            else
            {
                for (int i = 0; i < FindObjectOfType<GameManager>().playerObjectList.Count; i++)
                {
                    if (FindObjectOfType<GameManager>().playerObjectList[i].GetComponent<PhotonView>().IsMine)
                    {
                        localplayerObject = FindObjectOfType<GameManager>().playerObjectList[i];
                    }
                }
            }
        }
    }
    private void LateUpdate()
    {
        CheckPlacement();
    }
    //apply movement to character
    private void FixedUpdate()
    {
        if (pv.IsMine)
        {
            if (isDead)
            {
                return;
            }
            ApplyMovement();
        }
    }
    void Movement()
    {
        movementSpeed = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        movementSpeed.Normalize();
        if (movementSpeed.magnitude != 0)
        {
            combinedSpeed = speed + totalExtraSpeed;
            if (Input.GetButton("Sprint"))
            {
                if (staminaValue > 0)
                {
                    staminaValue = Mathf.Clamp(staminaValue -= staminaLossPerSec * Time.deltaTime, 0, maxStamina);
                    combinedSpeed = sprintSpeed + totalExtraSpeed * 1.5f;
                    Anim_sprint();
                    pv.RPC("TogleRunningPartical", RpcTarget.All, true);
                    if (!running_Audio.isPlaying)
                    {
                        running_Audio.Play();
                    }
                }
                else
                {
                    Anim_movement();
                    pv.RPC("TogleRunningPartical", RpcTarget.All, false);
                    if (!walking_Audio.isPlaying)
                    {
                        walking_Audio.Play();
                    }
                }
            }
            else
            {
				if (!walking_Audio.isPlaying)
				{
                    walking_Audio.Play();
                }
                Anim_movement();
                staminaValue = Mathf.Clamp(staminaValue += staminaGainedPerSec * Time.deltaTime, 0, maxStamina);
            }
        }
        else
        {
            walking_Audio.Stop();
            running_Audio.Stop();
            Anim_idle();
            staminaValue = Mathf.Clamp(staminaValue += staminaGainedPerSec * Time.deltaTime, 0, maxStamina);
        }
        staminaSlider.value = staminaValue;
        if (!isThirdPerson)
        {
            if (!mayAttack)
            {
                combinedSpeed *= 0.25f;
            }
            movementDirection = (transform.forward * movementSpeed.z + transform.right * movementSpeed.x) * combinedSpeed;
            //else is done in rotation
        }
    }
    [PunRPC]
    public void TogleJumpPartical(bool b)
	{
		if (b)
		{
            jumpPartical.GetComponent<ParticleSystem>().Play();
        }
		else
		{
            jumpPartical.GetComponent<ParticleSystem>().Stop();
        }
    }
    [PunRPC]
    public void TogleRunningPartical(bool b)
    {
        if (b)
        {
            runningPartical.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            runningPartical.GetComponent<ParticleSystem>().Stop();
        }
    }
    public IEnumerator JumpPartic()
	{
        pv.RPC("TogleJumpPartical", RpcTarget.All, true);
        yield return new WaitForSeconds(1f);
        pv.RPC("TogleJumpPartical", RpcTarget.All, false);
    }
    void Gravity()
    {
        if(controller.isGrounded)
        {
            if(!groundCheck)
            {
                groundCheck = true;
                remainingJumps = totalJumps;
                gravity = -0.3f;
            }
            groundCheckTime = Time.time;
        }
        else
        {
            if (Time.time >= groundCheckTime + 0.3f)
            {
                groundCheck = false;
            }
            gravity -= weight * Time.deltaTime;
            gravity = Mathf.Clamp(gravity, -100, 100);
        }
    }
    void ApplyMovement()
    {
        movementDirection.y = gravity;
        controller.Move(movementDirection * Time.deltaTime);
    }
    void Jump()
    {
        walking_Audio.Stop();
        running_Audio.Stop();
        remainingJumps--;
        gravity = jumpForce;
        groundCheck = false;

    }
    void Rotation()
    {
        //distance = Mathf.Clamp(distance -= Input.mouseScrollDelta.y, minDistance, maxDistance);
        cam.transform.position = camOriginpos.position - new Vector3(0, 0, distance);
        isThirdPerson = distance < 0.5f ? false : true;
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (!isThirdPerson)
        {
            //player
            transform.Rotate(Vector3.up, mouseDelta.x * (mouseSensitivity + extraMouseSense));

            //camera
            cameraPitch -= mouseDelta.y * (mouseSensitivity + extraMouseSense);
            cameraPitch = Mathf.Clamp(cameraPitch, -pitchDown, pitchUp);
            cam.transform.localEulerAngles = Vector3.right * cameraPitch;
        }
        else
        {
            //player direction
            float targetDirection = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.rotation.y, targetDirection, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            if(movementDirection.magnitude > 0)
            {
                movementDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward * combinedSpeed + movementSpeed;
                movementDirection.Normalize();
            }

            //camera moet 3rd worden
            cameraPitch -= mouseDelta.y * (mouseSensitivity + extraMouseSense);
            cameraPitch = Mathf.Clamp(cameraPitch, -pitchDown, pitchUp);
            cam.transform.localEulerAngles = Vector3.right * cameraPitch;
        }
    }
    void EatFood()
    {
        if (heldItem)
        {
            if (heldItem.equipment == EquipmentType.food)
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    if(!eatingOnCooldown)
                    {
                        StartEating();
						if (!eatingSound.isPlaying)
						{
                            eatingSound.Play();

                        }
                    }
                }
            }
        }
        if(eatingOnCooldown)
        {
            if (Time.time > foodCooldownTimeSteps)
            {
                foodCooldownTimeSteps = Time.time + 0.5f;
                foodCooldownSlider.value -= 0.5f;
            }
            if (Time.time > eatCooldown)
            {
                StopEating();
            }
        }
    }
    void StartEating()
    {
        Inventory inv = GetComponent<Inventory>();
        eatingOnCooldown = true;
        foodCooldownSlider.value = eatTime;
        //particle here
        eatCooldown = Time.time + eatTime;
        inv.hotBarSlots[inv.hotbarLocation].item.itemAmount--;
        GetComponent<Health>().TakeHeal(inv.hotBarSlots[inv.hotbarLocation].item.foodLifeRestore);
        if (inv.hotBarSlots[inv.hotbarLocation].item.itemAmount == 0)
        {
            inv.hotBarSlots[inv.hotbarLocation].item = null;
        }
        GetComponent<Inventory>().RefreshUI();
    }
    void StopEating()
    {
        eatingOnCooldown = false;
    }
    void SummonAlly()
    {
        if (heldItem)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                if (heldItem.equipment == EquipmentType.staff)
                {
                    pv.RPC("SpawnAllyOnMaster", RpcTarget.MasterClient, heldItem.itemName);
                }
            }
        }
    }
    [PunRPC]
    public void SpawnAllyOnMaster(string nameOfItem)
    {
        //hier summon activaten
        GameObject spawnThis;
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit))
        {
            spawnThis = ItemList.SelectItem(nameOfItem).summonObject;
            if (spawnThis != default)
            {
                GameObject spawnInObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Summons", spawnThis.name), _hit.point + Vector3.up * 2, Quaternion.identity);
                listOfSummons.Enqueue(spawnInObject);
                spawnInObject.GetComponent<SnakeBehavour>().player = transform.gameObject;
                if (listOfSummons.Count > maxSummons)
                {
                    GameObject firstSummon = listOfSummons.Peek();
                    listOfSummons.Dequeue();
                    PhotonNetwork.Destroy(firstSummon);
                }
            }
            else
            {
                print("summon not found");
            }
        }
    }
    void CheckPlacement()
    {
        if (heldItem)
        {
            if (heldItem.equipment == EquipmentType.none)
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    if(!placementCheck)
                    {
                        placementRotation = 0;
                        if (ghostplacement != null)
                        {
                            Destroy(ghostplacement);
                        }
                        placementCheck = true;
                        if (placementCheck)
                        {
                            GameObject spawnThis = default;
                            for (int i = 0; i < ghostList.Count; i++)
                            {
                                if(spawnThis != default)
                                {
                                    continue;
                                }
                                if (heldItem.itemName == ghostList[i].name)
                                {
                                    spawnThis = ghostList[i];
                                    selectedplacement = i;
                                }
                            }
                            if (spawnThis != default)
                            {
                                ghostplacement = Instantiate(spawnThis);
                            }
                            else
                            {
                                placementCheck = false;
                                print("item not found in ghostlist");
                                return;
                            }
                        }
                    }
                    else
                    {
                        placementCheck = false;
                        if (ghostplacement != null)
                        {
                            Destroy(ghostplacement);
                        }
                    }
                }
                if (placementCheck)
                {
                    if (Input.mouseScrollDelta.y > 0 || Input.mouseScrollDelta.y < 0)
                    {
                        placementRotation -= Input.mouseScrollDelta.y * 10;
                    }
                    RaycastHit _hit;
                    rotationddd.y = placementRotation;
                    if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit))
                    {
                        ghostplacement.transform.position = _hit.point;
                        ghostplacement.transform.rotation = Quaternion.Euler(rotationddd);
                    }
                }
            }
        }
        else
        {
            placementCheck = false;
        }
    }
    void PlaceItem()
    {
        placementCheck = false;
        Destroy(ghostplacement);

        GameObject spawnThis = default;

        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit))
        {
            Vector3 ghostPosition = _hit.point;
            Quaternion ghostRotation = Quaternion.Euler(rotationddd);

            spawnThis = actualItemList[selectedplacement];
            GetComponent<Inventory>().hotBarSlots[GetComponent<Inventory>().hotbarLocation].item = null;
            heldItem = null;
            GetComponent<Inventory>().RefreshUI();
            if (spawnThis != default)
            {
                GameObject spawnInObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Stations", "placeAbleItems", "ActualItems", spawnThis.name), ghostPosition, ghostRotation);
                PlaceAbleItemId[] objectsFound = FindObjectsOfType<PlaceAbleItemId>();
                pv.RPC("SincplaceableID", RpcTarget.All, spawnInObject.GetComponent<PhotonView>().ViewID);
            }
            else
            {
                print("item not found in actualItemList");
                return;
            }
        }
    }
    [PunRPC]
    public void SincplaceableID(int id)
    {
        PlaceAbleItemId[] objectsFound = FindObjectsOfType<PlaceAbleItemId>();
        for (int i = 0; i < objectsFound.Length; i++)
        {
            if (objectsFound[i].GetComponent<PhotonView>().ViewID == id)
            {
                objectsFound[i].GetComponent<PlaceAbleItemId>().placeabelItemID = objectsFound.Length + 1;
                return;
            }
        }
    }
    public void Attack()
    {
        if (pv.IsMine)
        {
            AttackSound();
            Collider[] thingsHit = Physics.OverlapSphere(attackPos.position, attackRadius, playerAimMask);

            //check hit things
            foreach (Collider hitObject in thingsHit)
            {
                if (hitObject.gameObject != gameObject || hitObject.gameObject != doNotHitThis)
                {
                    RaycastHit _hit;
                    if (Physics.Linecast(attackPos.position, hitObject.ClosestPoint(attackPos.position), out _hit))
                    {
                        Renderer rend = _hit.transform.GetComponent<Renderer>();
                        MeshCollider meshCollider = _hit.collider as MeshCollider;
                        //if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                        //{
                        //    tempObject.GetComponent<VisualEffect>().SetVector4("GivenColor", Color.black);
                        //}
                        //else
                        //{
                        //    Texture2D tex = rend.material.mainTexture as Texture2D;
                        //    Vector2 pixelUV = _hit.textureCoord;
                        //    pixelUV.x *= tex.width;
                        //    pixelUV.y *= tex.height;

                        //    Color kleurtje = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);

                        //    particle color
                        //    tempObject.GetComponent<VisualEffect>().SetVector4("GivenColor", kleurtje);
                        //}
                    }
                    #region crits/bleed/burn/poison
                    //crit
                    float critDamage = 0;
                    bool inflictBleed = false;
                    float poisonDamage = 0;
                    float roll = Random.Range(0, 100);
                    if (roll < totalCritChance)
                    {
                        critDamage = totalDamage;
                    }
                    roll = Random.Range(0, 100);
                    if (roll < totalChanceToInflictBleed)
                    {
                        inflictBleed = true;
                    }
                    roll = Random.Range(0, 100);
                    if(roll < poisonChance * 10)
                    {
                        poisonDamage = totalDamage * 0.05f * totalCritChance;
                    }
                    #endregion
                    //actual hit
                    if (hitObject.GetComponent<HitableObject>())
                    {
                        GameObject tempObject = Instantiate(testGraph, hitObject.ClosestPoint(attackPos.position), Quaternion.identity);
                        Destroy(tempObject, 1);
                        //damage
                        if (heldItem != null)
                        {
                            hitObject.GetComponent<HitableObject>().TakeDamage(totalDamage + critDamage, heldItem.equipment, hitObject.ClosestPoint(attackPos.position));
                        }
                        else
                        {
                            hitObject.GetComponent<HitableObject>().TakeDamage(totalDamage + critDamage, EquipmentType.none, hitObject.ClosestPoint(attackPos.position));
                        }
                    }
                    else if (hitObject.GetComponent<EnemieHealth>())
                    {
                        GameObject tempObject = Instantiate(testGraph, hitObject.ClosestPoint(attackPos.position), Quaternion.identity);
                        Destroy(tempObject, 1);
                        //damage
                        if (heldItem != null)
                        {
                            if (hitObject.GetComponent<EnemieHealth>().health == 0)
                            {
                                return;
                            }
                            if (totalLifeSteal > 0)
                            {
                                float healAmount = (totalDamage + critDamage - hitObject.GetComponent<EnemieHealth>().armor) * (totalLifeSteal / 100);
                                GetComponent<Health>().TakeHeal(healAmount);
                            }
                            if (totalHealthOnKill > 0)
                            {
                                if (hitObject.GetComponent<EnemieHealth>().health - (totalDamage + critDamage - hitObject.GetComponent<EnemieHealth>().armor) <= 0)
                                {
                                    GetComponent<Health>().TakeHeal(totalHealthOnKill);
                                }
                            }
                            if(nimbusStacks > 0)
                            {
                                if (hitObject.GetComponent<EnemieHealth>().health - (totalDamage + critDamage - hitObject.GetComponent<EnemieHealth>().armor) <= 0)
                                {
                                    GameObject nimbusje = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Summons", nimbusObject.name), hitObject.transform.position + Vector3.up * 2, Quaternion.identity);
                                    StartCoroutine(nimbusje.GetComponent<NimbusCloud>().StartNimbus(nimbusStacks * 4, totalDamage / 4));
                                    nimbusje.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f) * nimbusStacks;
                                }
                            }
                            hitObject.GetComponent<EnemieHealth>().TakeDamage(totalDamage + critDamage, inflictBleed, (int)burnTicks, poisonDamage, executeBelow, hitObject.ClosestPoint(attackPos.position));
                        }
                        else
                        {
                            hitObject.GetComponent<EnemieHealth>().TakeDamage(totalDamage + critDamage, inflictBleed, (int)burnTicks, poisonDamage, executeBelow, hitObject.ClosestPoint(attackPos.position));
                        }
                    }
                }
            }
        }
    }
    public void DoneAttacking()
    {
        animController.SetInteger("Attack", 0);
        animController.speed = 1;
    }    
    IEnumerator AttackStuckFix()
    {
        mayAttack = false;
        float tempAttackSpeed = totalAttackSpeed / (totalAttackSpeed * totalAttackSpeed);
        yield return new WaitForSeconds(tempAttackSpeed * 1.1f);
        mayAttack = true;
        DoneAttacking();
    }
    public void LockCamera()
    {
        InventoryIsOpen = !InventoryIsOpen;
    }
    public void SetSensetifity(Slider value)
    {
        extraMouseSense = value.value / 10;
    }
    public void GiveItemStats(Item _itemType)
    {
        heldItem = _itemType;
    }
    void CheckForInfo()
    {
        if (pv.IsMine)
        {
            if (!InventoryIsOpen)
            {
                RaycastHit infoHit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out infoHit, 5))
                {
                    if (infoHit.transform.GetComponent<ChestScript>())
                    {
                        pressE.SetActive(true);
                    }
                    else if (infoHit.transform.GetComponent<Totem>())
                    {
                        pressE.SetActive(true);
                    }
                    else if (infoHit.transform.GetComponent<ChestInventory>())
                    {
                        pressE.SetActive(true);
                    }
                    else if (infoHit.transform.GetComponent<CraftingStation>())
                    {
                        pressE.SetActive(true);
                    }
                    else if (infoHit.transform.GetComponent<OvenStation>())
                    {
                        pressE.SetActive(true);
                    }
                    else if (infoHit.transform.GetComponent<ItemPickUp>())
                    {
                        pressE.SetActive(true);
                    }
                    else
                    {
                        pressE.SetActive(false);
                    }
                }
                else
                {
                    pressE.SetActive(false);
                }
            }
            else
            {
                pressE.SetActive(false);
            }

            //on e press
            if (Input.GetKeyDown(KeyCode.E))
            {
                pressE.SetActive(false);
                RaycastHit _hit;
                Inventory inv = GetComponent<Inventory>();
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, 5))
                {
					if (_hit.transform.GetComponent<GraveStoneScript>())
					{
                        _hit.transform.GetComponent<GraveStoneScript>().Interaction();
                    }
                    else if (_hit.transform.GetComponent<ChestScript>())
                    {
                        if (_hit.transform.GetComponent<ChestScript>().canInteract)
                        {
                            if (inv.goldCoinsInPocket >= _hit.transform.GetComponent<ChestScript>().cost)
                            {
                                inv.RemoveCoin(_hit.transform.GetComponent<ChestScript>().cost);
                                _hit.transform.GetComponent<ChestScript>().Interaction();
                            }
                        }
                    }
                    else if (_hit.transform.GetComponent<Totem>())
                    {
                        _hit.transform.GetComponent<Totem>().Interact();
                    }
                    else if (_hit.transform.GetComponent<ChestInventory>())
                    {
                        GetComponent<Inventory>().OpenActualInventory(true);
                        lastChest = _hit.transform.GetComponent<ChestInventory>();
                        lastChest.OpenChestInventory(GetComponent<CharacterStats>());
                    }
                    else if (_hit.transform.GetComponent<CraftingStation>())
                    {
                        GetComponent<Inventory>().OpenActualInventory(true);
                        lastCratingStation = _hit.transform.GetComponent<CraftingStation>();
                        lastCratingStation.OpenCratingInventory(GetComponent<CharacterStats>(), GetComponent<Inventory>());
                    }
                    else if (_hit.transform.GetComponent<OvenStation>())
                    {
                        GetComponent<Inventory>().OpenActualInventory(true);
                        lastOvenStation = _hit.transform.GetComponent<OvenStation>();
                        lastOvenStation.OpenCratingInventory(GetComponent<CharacterStats>(), GetComponent<Inventory>());
                    }
                    else if (_hit.transform.GetComponent<ItemPickUp>())
                    {
                        _hit.transform.GetComponent<ItemPickUp>().DropItems();
                        //itemDrop_audio.Play();
                    }
                }
            }
        }
    }
    #region anim
    void Anim_idle()
    {
        animController.SetInteger("State", 0);
    }
    void Anim_movement()
    {
        animController.SetInteger("State", 1);
    }
    void Anim_sprint()
    {
        animController.SetInteger("State", 2);
    }
    void Anim_attack()
    {
        animController.SetInteger("Attack", 1);
        animController.speed = totalAttackSpeed;
    }
    void Anim_Jump()
    {
        animController.SetInteger("State", 3);
    }
    #endregion

    public void ChangeMasterVolume(Slider slider)
    {
        audioMaster.SetFloat("Master", slider.value - 80);
    }
    public void ChangeSFXVolume(Slider slider)
    {
        audioMaster.SetFloat("SFX", slider.value - 80);
    }
    public void ChangeMusicVolume(Slider slider)
    {
        audioMaster.SetFloat("Music", slider.value - 80);
    }

    //sound
    public void CoinSound()
    {
        if(!coinpickUp.isPlaying)
        {
            coinpickUp.Play();
        }
    }
    public void ItemSound()
    {
        if (!itemPickup_audio.isPlaying)
        {
            itemPickup_audio.Play();
        }
    }
    public void PlayHover()
	{
        hoverSound.Play();
	}
    public void PlayPressed()
	{
        pressedSound.Play();
    }
    public void AttackSound()
    {
        attackSound.Play();
    }
}
