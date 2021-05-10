using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.LowLevel;

public class Player : MonoBehaviour
{
    // Attributes
    private int maxHealth = 3;
    private int health = 3;
    private int maxMagic = 3;
    private int magic = 3;
    private RoomManager roomManager;
    private Vector2 size;
    private Vector2 halfSize;
    private SpriteRenderer sr;
    private LootDropsManager ldm;
    private UIManager ui;
    private Animator animator;

    private Vector2 velocity;
    private const float minYVel = -40.0f;
    private const float minlandingYVel = -10.0f;

    private const float minYWallClingVel = -5.0f;
    private const float initialJumpForce = 20.0f;
    private const float minJumpHeight = 4.0f;
    private const float maxJumpHeight = 7.34f;
    private const float wallClingVelMultiplier = 0.5f;
    private const float wallJumpYVelMultiplier = 0.8f;
    private float maxGravAcc;
    private float minGravAcc;

    private float maxXVel = 6.0f;
    private bool canMove;

    private bool slowMotion = false;
    private int wallLayerMask;

    private Vector2 kbVel;
    private bool applyKnockback = false;
    private bool applyIFrames = false;
    private const float kbDuration = 0.5f;
    private const float iFrameDuration = 0.5f;
    private float kbTimer;
    private float iFrameTimer;
    private bool kbFirstFrame = false;
    float kbXVelComponent, kbYVelComponent;
    private const float corWall = 0.4f;
    private const float floorStuckVel = 2.0f;

    private const float accel = 18.0f;
    private const float deccel = 72.0f;

    private bool grounded = true;
    private bool stuckInGround = false;
    private bool landed = true;
    private float jumpAboveGroundOffset = 0.5f;

    // 0 = Up, 1 = Right, 2 = Down, 3 = Left
    private int lookDir = 3;

    private Inventory inventory;

    private const float deadzone = 0.99f;
    //private Gamepad gamepad;

    private GameObject interactableObj = null;
    private bool canInteract = false;
    private bool waitOneFrame = false;

    private int damage = 1;
    AxeAttack axe;
    private Vector3 attackPivot = new Vector3(0.0f, 0.0f, 0.0f);
    private float speedModifier;
    private bool specialAttack = false;

    private Collider2D coll2D;
    private Vector2 rFacingCollOffset = new Vector2(0.45f, 0.0f);
    private Vector2 lFacingCollOffset = new Vector2(-0.45f, 0.0f);
    private AudioManager am;
    bool landedSoundPlayed = false;
    bool walkingSoundPlaying = false;

    private const float xWepKB = 10.0f;
    private const float yWepKB = initialJumpForce;

    // Upgrades
    private bool dashing;
    private const float dashVelocity = 18.0f;
    private const float dashMaxTimer = 0.3f;
    private float dashTimer;
    private const float dashMaxCooldown = 0.15f;
    private float dashCooldown;
    private bool canDash;

    private bool canDoubleJump;

    private bool leftWallCling;
    private bool rightWallCling;
    private const float xWallJumpForce = 10.0f;
    private float yWallJumpForce;

    // Basic Functions
    private void Start()
    {
        am = am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        animator = GetComponent<Animator>();
        axe = GameObject.FindGameObjectWithTag("Weapon").GetComponent<AxeAttack>();
        ui = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<UIManager>();
        inventory = new Inventory(ui);
        ldm = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<LootDropsManager>();
        sr = this.gameObject.GetComponent<SpriteRenderer>();
        size = sr.sprite.bounds.size / 2.0f;
        halfSize = size / 2.0f;
        roomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
        velocity = new Vector2(0.0f, 0.0f);
        wallLayerMask = LayerMask.GetMask("Wall");
        kbTimer = 0.0f;
        kbVel = new Vector2(0.0f, 0.0f);
        ui.UpdateHealth(health);
        speedModifier = 1.0f;
        coll2D = this.gameObject.GetComponent<BoxCollider2D>();
        //Debug.Log(string.Join("\n", Gamepad.all));
        dashing = false;
        dashTimer = 0.0f;
        dashCooldown = 0.0f;
        canDash = true;
        canDoubleJump = true;
        leftWallCling = false;
        rightWallCling = false;
        canMove = false;

        maxGravAcc = (0.5f * Mathf.Pow(initialJumpForce, 2.0f)) / minJumpHeight;
        minGravAcc = (0.5f * Mathf.Pow(initialJumpForce, 2.0f)) / maxJumpHeight;
        yWallJumpForce = initialJumpForce * wallJumpYVelMultiplier;
    }

    private void Update()
    {
        if (ui.GetGameStarted())
        {
            //gamepad = Gamepad.current;
            //slowMotion = Input.GetButton("Slow_Motion");
            if (waitOneFrame)
                waitOneFrame = false;
            else if (canMove)
                Move();

            if (Input.GetKeyDown("m") && !ui.GetPaused())
            {
                ui.DisplayMenu();
            }
            if (ui.DisplayingMenus())
            {
                if (Input.GetKeyDown("n"))
                {
                    ui.CycleNextMenu();
                }
                if (Input.GetKeyDown("p"))
                {
                    ui.CyclePreviousMenu();
                }
            }
            if (health <= 0)
            {
                ui.UpdateHealth(health);
                health = maxHealth;
                roomManager.PlayerDied();
                ui.PlayerLost(GetGold() / 4);
                inventory.LoseMoneyFromDeath(GetGold() / 4);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (ui.DisplayingMenus())
                    ui.DisplayMenu();
                else
                {
                    ui.Pause();
                }
            }
            if (canInteract && Input.GetButtonDown("Interact") && canMove)
            {
                if (interactableObj.tag == "NPC")
                {
                    interactableObj.GetComponent<TextPopUp>().DisplayStoryText();
                }
                else if (interactableObj.tag == "ResPoint")
                {
                    roomManager.SetResPoint();
                    interactableObj.GetComponent<ResPoint>().SetAsResPoint();
                }
                else if (interactableObj.tag == "Chest")
                {
                    interactableObj.GetComponent<Chest>().Open();
                }
            }
        }
    }

    // Physics Functions
    private void Move()
    {
        Vector3 pos = transform.position;
        Vector3 halfSizeX = new Vector3(halfSize.x - 0.2f, 0.0f, 0.0f);
        Vector3 halfSizeY = new Vector3(0.0f, halfSize.y - 0.1f, 0.0f);

        velocity = new Vector2(velocity.x, velocity.y);
        grounded = (Physics2D.Raycast(transform.position + halfSizeX, Vector2.down, halfSize.y, wallLayerMask) ||
                    Physics2D.Raycast(transform.position - halfSizeX, Vector2.down, halfSize.y, wallLayerMask) ||
                    Physics2D.Raycast(transform.position, Vector2.down, halfSize.y, wallLayerMask)) &&
                    velocity.y <= floorStuckVel;

        landed = (Physics2D.Raycast(transform.position + halfSizeX, Vector2.down, halfSize.y + jumpAboveGroundOffset, wallLayerMask) ||
                  Physics2D.Raycast(transform.position - halfSizeX, Vector2.down, halfSize.y + jumpAboveGroundOffset, wallLayerMask) ||
                  Physics2D.Raycast(transform.position, Vector2.down, halfSize.y + jumpAboveGroundOffset, wallLayerMask)) &&
                  velocity.y <= floorStuckVel;

        CheckForWallCling(halfSizeY);

        if (leftWallCling || rightWallCling || grounded)
            canDoubleJump = true;

        if (!applyKnockback || applyIFrames)
        {
            if (dashing)
            {
                if (!DashCancel())
                    Dash();
            }
            else 
            {
                if (!canDash)
                    CooldownDash();
                else if (Input.GetButtonDown("Dash"))
                {
                    dashTimer = 0.0f;
                    dashing = true;
                    canDash = false;
                    am.Play("playerDash");
                    if (sr.flipX)
                        velocity.x = dashVelocity;
                    else
                        velocity.x = -dashVelocity;
                    velocity.y = 0.0f;
                }
            }

            if (applyIFrames)
            {
                iFrameTimer += Time.deltaTime;
                if (iFrameTimer > iFrameDuration)
                {
                    iFrameTimer = 0.0f;
                    applyIFrames = false;
                }
            }
            if (!dashing)
                InputMovement();
        }
        else
            Knockback();

        PreventWallCollide(halfSizeX);

        SetLookDirection();

        if (landed && velocity.y < minlandingYVel)
            velocity.y = minlandingYVel;

        if (!slowMotion)
        {
            pos.x += velocity.x * speedModifier * Time.deltaTime;
            pos.y += velocity.y * Time.deltaTime;
        }
        else
        {
            pos.x += velocity.x * 0.25f * speedModifier * Time.deltaTime;
            pos.y += velocity.y * 0.25f * Time.deltaTime;
        }

        transform.position = pos;
        if(velocity.x != 0 || velocity.y != 0)
        {
            animator.SetBool("Moving", true);
            if(Physics2D.Raycast(transform.position, Vector2.down, halfSize.y, wallLayerMask) && !walkingSoundPlaying)
            {
                walkingSoundPlaying = true;
                //am.Play("walking");
            }
        }
        else
        {
            if(walkingSoundPlaying)
            {
                walkingSoundPlaying = false;
                //am.Stop("walking");
            }
            animator.SetBool("Moving", false);
        }
    }

    private void CheckForWallCling(Vector3 halfSizeY)
    {
        RaycastHit2D wallToLeft1 = Physics2D.Raycast(transform.position, Vector2.left, halfSize.x, wallLayerMask);
        RaycastHit2D wallToLeft2 = Physics2D.Raycast(transform.position + halfSizeY, Vector2.left, halfSize.x, wallLayerMask);
        RaycastHit2D wallToLeft3 = Physics2D.Raycast(transform.position - halfSizeY, Vector2.left, halfSize.x, wallLayerMask);

        RaycastHit2D wallToRight1 = Physics2D.Raycast(transform.position, Vector2.right, halfSize.x, wallLayerMask);
        RaycastHit2D wallToRight2 = Physics2D.Raycast(transform.position + halfSizeY, Vector2.right, halfSize.x, wallLayerMask);
        RaycastHit2D wallToRight3 = Physics2D.Raycast(transform.position - halfSizeY, Vector2.right, halfSize.x, wallLayerMask);

        leftWallCling = wallToLeft1 || wallToLeft2 || wallToLeft3;
        rightWallCling = wallToRight1 || wallToRight2 || wallToRight3;
        bool hitPlayer = false;

        if (leftWallCling)
        {
            Transform t1 = wallToLeft1.transform;
            Transform t2 = wallToLeft2.transform;
            Transform t3 = wallToLeft3.transform;

            if (t1 != null)
            {
                if (t1.name == "Tilemap_Traps")
                {
                    Hit(t1.position, 5.0f, 1);
                    hitPlayer = true;
                }
            }
            if (!hitPlayer && t2 != null)
            {
                if (t2.name == "Tilemap_Traps")
                {
                    Hit(t2.position, 5.0f, 1);
                    hitPlayer = true;
                }
            }
            if (!hitPlayer && t3 != null)
            {
                if (t3.name == "Tilemap_Traps")
                {
                    Hit(t3.position, 5.0f, 1);
                    hitPlayer = true;
                }
            }
        }
        if (!hitPlayer && rightWallCling)
        {
            Transform t1 = wallToRight1.transform;
            Transform t2 = wallToRight2.transform;
            Transform t3 = wallToRight3.transform;

            if (t1 != null)
            {
                if (t1.name == "Tilemap_Traps")
                {
                    Hit(t1.position, 5.0f, 1);
                    hitPlayer = true;
                }
            }
            if (!hitPlayer && t2 != null)
            {
                if (t2.name == "Tilemap_Traps")
                {
                    Hit(t2.position, 5.0f, 1);
                    hitPlayer = true;
                }
            }
            if (!hitPlayer && t3 != null)
            {
                if (t3.name == "Tilemap_Traps")
                {
                    Hit(t3.position, 5.0f, 1);
                }
            }
        }
    }

    private void Dash()
    {
        dashTimer += Time.deltaTime;
        if (dashTimer > dashMaxTimer)
            dashing = false;
    }

    private bool DashCancel()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Dash") || Input.GetButtonDown("Jump") || leftWallCling || rightWallCling)
            dashing = false;
        else if (velocity.x > 0 && horizontalInput < -0.7f || velocity.x < 0 && horizontalInput > 0.7f)
            dashing = false;

        return !dashing;
    }

    private void CooldownDash()
    {
        dashCooldown += Time.deltaTime;

        if ((grounded || leftWallCling || rightWallCling) && dashCooldown > dashMaxCooldown)
        {
            canDash = true;
            dashCooldown = 0.0f;
        }
    }

    private void SetLookDirection()
    {
        // 0 = Up, 1 = Right, 2 = Down, 3 = Left
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput < 0.7f && horizontalInput > -0.7f)
            horizontalInput = 0.0f;
        if (verticalInput < 0.7f && verticalInput > -0.7f)
            verticalInput = 0.0f;

        if (verticalInput == 1.0f)
            lookDir = 0;
        else if (verticalInput == -1.0f)
            lookDir = 2;
        else if (horizontalInput != 0.0f || verticalInput != 0.0f)
            if (Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput))
                if (verticalInput > 0.0f)
                    lookDir = 0;
                else
                    lookDir = 2;
            else if (horizontalInput > 0.0f)
                lookDir = 1;
            else
                lookDir = 3;
        else if (sr.flipX)
            lookDir = 1;
        else
            lookDir = 3;
    }

    private void InputMovement()
    {
        float horizontalInput = 0;
        float verticalInput = 0;
        if (!ui.DisplayingMenus())
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }
        if (horizontalInput > deadzone)
        {
            if (!specialAttack)
            {
                sr.flipX = true;
                coll2D.offset = rFacingCollOffset;
            }

            if (velocity.x < 0.0f)
                velocity.x += 8.0f * accel * Time.deltaTime;
            else
                velocity.x += accel * Time.deltaTime;

            if (velocity.x > maxXVel)
                velocity.x = maxXVel;

        }
        else if (horizontalInput < -deadzone)
        {
            if (!specialAttack)
            {
                sr.flipX = false;
                coll2D.offset = lFacingCollOffset;
            }

            if (velocity.x > 0.0f)
                velocity.x -= 8.0f * accel * Time.deltaTime;
            else
                velocity.x -= accel * Time.deltaTime;

            if (velocity.x < -maxXVel)
                velocity.x = -maxXVel;
        }
        else if (velocity.x != 0.0f)
        {
            if (velocity.x > 0.5f)
                velocity.x -= deccel * Time.deltaTime;
            else if (velocity.x < -0.5f)
                velocity.x += deccel * Time.deltaTime;
            else
                velocity.x = 0.0f;

        }

        if (!grounded && !stuckInGround && !landed)
        {
            landedSoundPlayed = false;
            //if (verticalInput < -deadzone)
            //    velocity.y -= 20.0f * Time.deltaTime;

            if (leftWallCling && Input.GetButtonDown("Jump"))
            {
                am.Play("jumpingSound");
                velocity.y = yWallJumpForce;
                velocity.x = xWallJumpForce;
            }
            else if (rightWallCling && Input.GetButtonDown("Jump"))
            {
                am.Play("jumpingSound");
                velocity.y = yWallJumpForce;
                velocity.x = -xWallJumpForce;
            }
            else if (canDoubleJump && Input.GetButtonDown("Jump"))
            {
                am.Play("jumpingSound");
                velocity.y = initialJumpForce;
                canDoubleJump = false;
            }
        }
        else
        {
            if(!landedSoundPlayed)
            {
                am.Play("landingSound");
                landedSoundPlayed = true;
            }
            //gamepad = Gamepad.current;
            //if (gamepad != null)
            if (Input.GetButtonDown("Jump"))
            {
                am.Play("jumpingSound");
                velocity.y = initialJumpForce;
                animator.SetBool("Jumping", true);
            }
            else
            {
                //velocity.y = 0.0f;
                animator.SetBool("Jumping", false);
            }
        }
        ApplyGravity();
    }

    private void Knockback()
    {
        if (kbFirstFrame)
        {
            velocity.x = kbXVelComponent;
            velocity.y = kbYVelComponent - (maxGravAcc * kbTimer);
            kbFirstFrame = false;
        }
        else if (!grounded)
            ApplyGravity();

        kbTimer += Time.deltaTime;
        if (kbTimer > kbDuration)
        {
            applyKnockback = false;
            applyIFrames = true;
        }
    }

    private void ApplyGravity()
    {
        if (!leftWallCling && !rightWallCling)
        {
            if (velocity.y > minYVel)
                if (Input.GetButton("Jump"))
                    velocity.y -= minGravAcc * Time.deltaTime;
                else
                    velocity.y -= maxGravAcc * Time.deltaTime;
            else
                velocity.y = minYVel;
        }
        else
        {
            if (velocity.y > 0)
                velocity.y -= maxGravAcc * Time.deltaTime;
            else if (velocity.y > minYWallClingVel)
                velocity.y -= maxGravAcc * wallClingVelMultiplier * Time.deltaTime;
            else
                velocity.y = minYWallClingVel;
        }
    }

    private void PreventWallCollide(Vector3 halfSizeX)
    {
        // TODO: Make smoother using collision and gradually being pushed out of walls

        // Hits left wall
        if (!stuckInGround && leftWallCling && velocity.x < 0.0f)
        {
            if (!applyKnockback)
                velocity.x = 0.0f;
            else
                velocity.x *= -corWall;
        }
        // Hits right wall
        else if (!stuckInGround && rightWallCling && velocity.x > 0.0f)
        {
            if (!applyKnockback)
                velocity.x = 0.0f;
            else
                velocity.x *= -corWall;
        }
        // Hits ceiling
        if ((Physics2D.Raycast(transform.position, Vector2.up, halfSize.y, wallLayerMask) ||
             Physics2D.Raycast(transform.position + halfSizeX, Vector2.up, halfSize.y, wallLayerMask) ||
             Physics2D.Raycast(transform.position - halfSizeX, Vector2.up, halfSize.y, wallLayerMask)) && velocity.y > 0.0f)
        {
            if (!applyKnockback)
                velocity.y = 0.0f;
            else
                velocity.y *= -corWall;
        }
        // Hits floor
        else if (grounded && velocity.y < 0)
        {
            if (!applyKnockback)
            {
                if (Physics2D.Raycast(transform.position, Vector3.down, halfSize.y - 0.1f, wallLayerMask))
                {
                    stuckInGround = true;
                }
                else
                {
                    velocity.y = 0.0f;
                }
            }
            else
                velocity.y *= -corWall;
        }

        if (stuckInGround)
        {
            if (!Physics2D.Raycast(transform.position, Vector3.down, halfSize.y - 0.1f, wallLayerMask))
            {
                stuckInGround = false;
                velocity.y = 0.0f;
            }
            else
                velocity.y = floorStuckVel;
        }
    }



    private void TakeDamage(int _damageAmount)
    {
        health -= _damageAmount;
        ui.UpdateHealth(health);
        if (health > 0)
        {
            am.Play("playerHit");
        }
        else if (health <= 0)
        {
            am.Play("playerDied");
            ResetVelocity();
        }
    }

    private void SetupKnockBack(Vector3 enemyPos, float force)
    {
        float angle = CalculateAngleOfElevation(enemyPos);
        //Debug.Log("Cos: " + Mathf.Cos(angle * Mathf.Deg2Rad));
        //Debug.Log("Sin: " + Mathf.Sin(angle * Mathf.Deg2Rad));
        kbXVelComponent = force * Mathf.Cos(angle * Mathf.Deg2Rad);
        kbYVelComponent = force * Mathf.Sin(angle * Mathf.Deg2Rad);
        //Debug.Log("Angle: " + angle);
        //Debug.Log("X Velocity: " + kbXVelComponent);
        //Debug.Log("Y Velocity: " + kbYVelComponent);
        kbTimer = 0.0f;
        kbFirstFrame = true;
        applyKnockback = true;
    }
    private float CalculateAngleOfElevation(Vector3 enemyPos)
    {
        Vector3 playerPos = this.transform.position;
        Vector2 vecEnemyToPlayer = new Vector2(playerPos.x - enemyPos.x, playerPos.y - enemyPos.y);
        //Debug.Log("Player Pos: " + playerPos);
        //Debug.Log("Enemy Pos: " + enemyPos);
        float modVec = Mathf.Sqrt(Mathf.Pow(vecEnemyToPlayer.x, 2.0f) + Mathf.Pow(vecEnemyToPlayer.y, 2.0f));

        // V1 = vecEnemyToPlayer
        // V2 = ( 1  0 )
        // dotProduct = (V1.x * 1) + (V1.y * 0) = V1.x
        // normalised dotProduct = V1.x / mod
        float dotProduct = vecEnemyToPlayer.x / modVec;

        //Debug.Log("Modulus of Vector: " + modVec);
        //Debug.Log("Dot Product: " + dotProduct);

        if (playerPos.y < enemyPos.y)
            return 360.0f - (Mathf.Acos(dotProduct) * Mathf.Rad2Deg);
        else
            return Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
    }

    // Public funcitons
    public void Hit(Vector3 enemyPos, float force, int _damageAmount)
    {
        if (!applyKnockback && !applyIFrames && !dashing)
        {
            StartCoroutine(FlashRed());
            TakeDamage(_damageAmount);
            SetupKnockBack(enemyPos, force);
        }
    }

    public int GetAttackDir()
    {
        if (grounded && lookDir == 2)
        {
            if (sr.flipX)
                return 1;
            else
                return 3;
        }

        return lookDir;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Item")
        {
            if (other.gameObject.GetComponent<Loot>().GetItem().GetStrName() != "note1" && other.gameObject.GetComponent<Loot>().GetItem().GetStrName() != "note2" && other.gameObject.GetComponent<Loot>().GetItem().GetStrName() != "note3")
            {
                inventory.AddToInventory(other.gameObject.GetComponent<Loot>().GetItem());
                ldm.PickUpLoot(other.gameObject);
                if (other.gameObject.GetComponent<Loot>().GetItem().GetStrName() == "Blood")
                {
                    if (health < maxHealth)
                    {
                        ++health;
                        ui.UpdateHealth(health);
                    }
                }
            }
            else
            {
                inventory.AddToJournal(other.gameObject.GetComponent<Loot>().GetItem());
                ldm.PickUpLoot(other.gameObject);
            }
        }
        else if (other.gameObject.tag == "NPC" || other.gameObject.tag == "ResPoint" || other.gameObject.tag == "Chest")
        {
            interactableObj = other.gameObject;
            canInteract = true;
            if(other.gameObject.tag == "ResPoint")
            {
                health = maxHealth;
                magic = maxMagic;
                ui.UpdateMagic(magic);
                ui.UpdateHealth(health);
            }
        }
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Spikes")
        {
            Hit(other.transform.position, 5.0f, 1);
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "NPC" || other.gameObject.tag == "ResPoint" || other.gameObject.tag == "Chest")
        {
            canInteract = false;
        }
    }
    public void EnterDoor(float seconds)
    {
        StartCoroutine(PausePlayer(seconds));
    }
    private IEnumerator PausePlayer(float seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }
    public List<Item> GetInventory()
    {
        return inventory.GetInventory();
    }
    public int GetGold()
    {
        return inventory.GetGold();
    }
    public int GetBlood()
    {
        return inventory.GetBlood();
    }
    public void AddToInventory(Item item)
    {
        inventory.AddToInventory(item);
    }
    public void AddToInventory(StoreItem item)
    {
        ui.DisplayAddedToText(0, 1);
        inventory.AddToInventory(item);
        if (item.name == "Damage Buff")
        {
                damage++;
                axe.UpdateDamage();
        }
        if (item.name == "Health Buff")
        {
                maxHealth++;
                health++;
                ui.UpdatePlayerHealthAndMagicMax();
        }
        if (item.name == "Magic Buff")
        {
                maxMagic++;
                magic++;
                ui.UpdatePlayerHealthAndMagicMax();
        }
    }
    public bool CanMove()
    {
        return canMove;
    }
    public void SetCanMove(bool toggle)
    {
        if (toggle == true)
            waitOneFrame = true;

        canMove = toggle;
    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public int GetDamage()
    {
        return damage;
    }
    public bool HasItemInInventory(string requiredItemName)
    {
        return inventory.HasItem(requiredItemName);
    }
    public bool HasNoteInInventory(string requiredNoteName)
    {
        return inventory.HasNote(requiredNoteName);
    }
    public void SetAttackPivot(Vector3 pivotPoint)
    {
        attackPivot = pivotPoint - transform.position;
    }

    public Vector3 GetAttackPivot()
    {
        return attackPivot + transform.position;
    }
    
    public bool HasMagic()
    {
        return magic > 0;
    }

    public bool GetXFlip()
    {
        return sr.flipX;
    }

    public void SetSpeedModifier(float modifier)
    {
        speedModifier = modifier;
    }

    public void SetSpecialAttack(bool specialAttack)
    {
        this.specialAttack = specialAttack;
    }

    public bool FlipXPlayer()
    {
        if (sr.flipX)
        {
            sr.flipX = false;
            coll2D.offset = lFacingCollOffset;
            return false;
        }
        else
        {
            sr.flipX = true;
            coll2D.offset = rFacingCollOffset;
            return true;
        }
    }

    public void ReduceMagic(int amount)
    {
        magic -= amount;
        ui.UpdateMagic(magic);
    }
    public int GetHealth()
    {
        return health;
    }
    public int GetMagic()
    {
        return magic;
    }
    public int GetMaxMagic()
    {
        return maxMagic;
    }
    private IEnumerator FlashRed()
    {
        sr.color = new Color(0.5f, 0, 0);
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }
    public bool HasMaxOfItem(string name, int maxAmount)
    {
        return inventory.maxedItem(name, maxAmount);
    }
    public void ChangeLayerOrder(int num)
    {
        sr.sortingOrder = num;
    }
    public void ApplyWeaponKnockback(int attackDir)
    {
        // 0 = Up, 1 = Right, 2 = Down, 3 = Left
        if (!applyKnockback)
            switch (attackDir)
            {
                case 1:
                    velocity.x -= xWepKB;
                    break;
                case 2:
                    velocity.y = yWepKB;
                    canDoubleJump = true;
                    break;
                case 3:
                    velocity.x += xWepKB;
                    break;
                default:
                    break;
            }
    }
    public List<Item> GetJournal()
    {
        return inventory.GetJournal();
    }

    public void ResetVelocity()
    {
        applyKnockback = false;
        kbTimer = 0.0f;
        velocity = new Vector2(0.0f, 0.0f);
    }
    public void ReduceGold(int amount)
    {
        inventory.IncreaseGold(-amount);
    }
}
