using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossBehaviour : BasicMovement
{
    // Attibutes
    private GameObject player;
    private Player pScript;
    private SpriteRenderer sprite;
    public Sprite noWeaponSprite, weaponSprite;
    public GameObject attackRange;
    private BossAxeAttack attackScript;
    private UIManager ui;

    private Collider2D coll2D;
    private Vector2 rFacingCollOffset = new Vector2(0.0f, 0.0f); //new Vector2(1.3f, -0.45f);
    private Vector2 lFacingCollOffset = new Vector2(0.0f, 0.0f); //new Vector2(-1.3f, -0.45f);

    private Vector2 viewDistance;

    private bool engagedPlayer;
    private int phase;
    // 0 = Up, 1 = Right, 2 = Down, 3 = Left
    private int lookDir;

    // 9: Usual behaviour
    // 0: Face player, then stand still     2%
    // 1: Move in fixed direction - Initially towards the player    3%
    // 2: Standard axe swing towards player     20%
    // 3: Charged axe swing towards player      10%
    // 4: Standard jump     20%
    // 5: Jump and axe swing towards player (midair)    10%
    // 6: Cyclone axe ability       15%
    // 7: Slam axe into ground - creating shockwaves    10%
    // 8: Flail axe madly - chasing the player and then falling over after 5 seconds (?)    10%
    private const int maxTypesOfAttacks = 8;
    private bool attacking = false;
    private int currentAttack = maxTypesOfAttacks + 1;
    private int previousAttack;

    // Individual Attack Variables ---------------------------------------------------------------
    private float attackTimer = 0.0f;
    // Attack 0
    private bool hasTurned = false;
    private const float attack0MaxTimer = 0.5f;
    // Attack 1
    private const float attack1MaxTimer = 2.0f;
    // Attack 2
    private bool setupAttack = false;
    // Attack 4
    private Vector3 jumpDistance = new Vector3(25.0f, 0.0f, 0.0f);
    private bool setupJump = false;
    private const float initialJumpForce = 20.0f;
    // Attack 5
    private bool swungAxe = false;
    // Attack 8
    private int prevLookDir = 1;
    // Individual Attack Variables ---------------------------------------------------------------

    private float maxCooldown;
    private float cooldownTimer = -2.0f;
    private bool attackReady = false;

    private int wallMask;

    private Vector3 attackPivot = new Vector3(0.0f, 0.0f, 0.0f);
    private Gate gate;

    private const int maxHealth = 25;

    private const float accel = 18.0f;
    private const float stuckInFloorVel = 2.0f;
    public GameObject lifeBar, bossName, bossNameBackground;
    public Slider lifeBarSlider;
    public GameObject particle;

    // Start is called before the first frame update
    public void Start()
    {
        ui = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<UIManager>();
        gate = GameObject.Find("Gate").GetComponent<Gate>();
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        pScript = player.GetComponent<Player>();
        sprite = this.gameObject.GetComponent<SpriteRenderer>();
        attackScript = attackRange.GetComponent<BossAxeAttack>();
        coll2D = this.gameObject.GetComponent<BoxCollider2D>();

        size = new Vector2(sprite.sprite.bounds.size.x / 2.0f * this.gameObject.transform.localScale.x, sprite.sprite.bounds.size.y / 2.0f * this.gameObject.transform.localScale.y);
        halfSize = size / 2.0f;

        viewDistance = new Vector2(10.0f, 3.0f);

        engagedPlayer = false;
        previousAttack = currentAttack;
        phase = 0;
        speedMultiplier = 1.0f;
        maxCooldown = 1.8f;

        wallMask = LayerMask.GetMask("Wall");
        lifeBarSlider.maxValue = health;
        lifeBarSlider.minValue = 0;
        lifeBarSlider.value = health;
        lifeBar.SetActive(false);
        bossName.SetActive(false);
        particle.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (engagedPlayer)
        {
            SetLookDirection();

            if (attacking)
                Attack();
            else if (!attackReady)
                Cooldown();
            else
                SetAttack();

            GroundStuckFix();
        }
        else
        {
            base.Update();
            
            SearchForPlayer();
        }

        if (sprite.flipX)
            coll2D.offset = lFacingCollOffset;
        else
            coll2D.offset = rFacingCollOffset;
    }

    private void GroundStuckFix()
    {
        PreventWallCollide();

        if (GetStuckInGround())
        {
            Vector3 pos = transform.position;
            pos.y += stuckInFloorVel * Time.deltaTime;
            transform.position = pos;
        }
    }

    private void Attack()
    {
        sr.sprite = noWeaponSprite;
        switch (currentAttack)
        {
            case 0:
                AttackStandStill();
                break;
            case 1:
                AttackMove();
                break;
            case 2:
                AttackStandardAxe();
                break;
            case 3:
                AttackChargedAxe();
                break;
            case 4:
                AttackJump();
                break;
            case 5:
                AttackJumpAndAxe();
                break;
            case 6:
                AttackCyclone();
                break;
            case 7:
                AttackAxeSlam();
                break;
            case 8:
                AttackAxeFlail();
                break;
            default:
                break;
        }
    }

    private void AttackStandStill()
    {
        if (!hasTurned)
        {
            LookAtPlayer();

            hasTurned = true;
        }
        else
        {
            if (attackTimer > attack0MaxTimer)
            {
                attacking = false;
                hasTurned = false;
                attackTimer = 0.0f;
            }
            else
                attackTimer += Time.deltaTime;
        }
    }

    private void AttackMove()
    {
        if (!hasTurned)
        {
            LookAtPlayer();

            hasTurned = true;
        }

        base.Update();

        if (attackTimer > attack1MaxTimer)
        {
            attacking = false;
            hasTurned = false;
            attackTimer = 0.0f;
        }
        else
            attackTimer += Time.deltaTime;
    }

    private void AttackStandardAxe()
    {
        if (!setupAttack)
        {
            LookAtPlayer();
            attackScript.SetupBasicAttack(lookDir);
            setupAttack = true;
        }
        else if (!attackScript.IsAttacking())
        {
            setupAttack = false;
            attacking = false;
        }
    }

    private void AttackChargedAxe()
    {
        if (!attackScript.GetCharged())
        {
            attackScript.Charge();
            particle.SetActive(true);
        }
        else if (!setupAttack)
        {
            LookAtPlayer();
            attackScript.SetupBasicAttack(lookDir);
            setupAttack = true;
        }
        else if (!attackScript.IsAttacking())
        {
            particle.SetActive(false);
            setupAttack = false;
            attacking = false;
        }
    }

    private void AttackJump()
    {
        if (!setupJump)
        {
            if (!hasTurned)
            {
                LookAtPlayer();

                hasTurned = true;
            }
            bool safeToJump;
            Vector3 pos = transform.position;
            if (lookDir == 1)
                safeToJump = !Physics2D.Linecast(pos, pos + jumpDistance, wallMask);
            else
            {
                Debug.DrawLine(pos, pos - jumpDistance, Color.green, 5);
                safeToJump = !Physics2D.Linecast(pos, pos - jumpDistance, wallMask);
            }

            if (safeToJump)
            {
                SetYVel(initialJumpForce);
                setupJump = true;
            }
            else
            {
                base.Update();
            }
        }
        else
        {
            if (!GetGrounded() || GetYVel() != 0.0f)
            {
                Vector3 pos = transform.position;

                if (lookDir == 1)
                    pos.x += maxXVel * speedMultiplier * 3.2f * Time.deltaTime;
                else
                    pos.x -= maxXVel * speedMultiplier * 3.2f * Time.deltaTime;

                velocity.y += gravAcc * speedMultiplier * Time.deltaTime;
                SetGrounded();
                PreventWallCollide();
                pos.y += velocity.y * speedMultiplier * Time.deltaTime;

                transform.position = pos;
            }
            else
            {
                hasTurned = false;
                setupJump = false;
                attacking = false;
            }
        }
    }

    private void AttackJumpAndAxe()
    {
        if (!setupJump)
        {
            if (!hasTurned)
            {
                LookAtPlayer();

                swungAxe = false;
                hasTurned = true;
            }
            bool safeToJump;
            Vector3 pos = transform.position;
            if (lookDir == 1)
                safeToJump = !Physics2D.Linecast(pos, pos + jumpDistance, wallMask);
            else
            {
                Debug.DrawLine(pos, pos - jumpDistance, Color.green, 5);
                safeToJump = !Physics2D.Linecast(pos, pos - jumpDistance, wallMask);
            }

            if (safeToJump)
            {
                SetYVel(initialJumpForce);
                setupJump = true;
            }
            else
            {
                base.Update();
            }
        }
        else
        {
            if (!GetGrounded() || GetYVel() != 0.0f)
            {
                Vector3 pos = transform.position;

                if (lookDir == 1)
                    pos.x += maxXVel * 3.2f * Time.deltaTime;
                else
                    pos.x -= maxXVel * 3.2f * Time.deltaTime;

                velocity.y += gravAcc * Time.deltaTime;
                SetGrounded();
                PreventWallCollide();
                pos.y += velocity.y * Time.deltaTime;

                transform.position = pos;

                if (!swungAxe && velocity.y < 5.0f)
                {
                    Vector3 playerPos = player.transform.position;
                    Vector3 distance = playerPos - pos;

                    if (!(Mathf.Abs(distance.x) > Mathf.Abs(distance.y)))
                    {
                        if (distance.y > 0)
                            lookDir = 0;
                        else
                            lookDir = 2;
                    }

                    attackScript.SetupBasicAttack(lookDir);
                    swungAxe = true;
                    setupAttack = true;
                }
            }
            else
            {
                setupAttack = false;
                hasTurned = false;
                setupJump = false;
                attacking = false;
            }
        }
    }

    private void AttackCyclone()
    {
        if (!setupAttack)
        {
            if (player.transform.position.x > this.transform.position.x)
            {
                lookDir = 1;
                sprite.flipX = false;
                coll2D.offset = rFacingCollOffset;
            }
            else
            {
                lookDir = 3;
                sprite.flipX = true;
                coll2D.offset = lFacingCollOffset;
            }
            attackScript.SetupSpecialAttack();
            setupAttack = true;
        }
        else if (!attackScript.IsAttacking())
        {
            setupAttack = false;
            attacking = false;
        }
    }

    private void AttackAxeSlam()
    {
        // TODO: Design this
        attacking = false;
    }

    private void AttackAxeFlail()
    {
        // TODO: Design this
        if (!setupAttack)
        {
            LookAtPlayer();
            ResetVelocity();
            attackScript.SetupFlailAttack(lookDir);
            prevLookDir = lookDir;
            setupAttack = true;
        }
        else if (attackScript.IsAttacking())
        {
            Vector3 pos = transform.position;
            if (player.transform.position.x > pos.x)
            {
                if (prevLookDir == 3)
                {
                    attackScript.FlipFlailAttack(1);
                }
                lookDir = prevLookDir = 1;
                sprite.flipX = false;
                coll2D.offset = rFacingCollOffset;
                velocity.x += accel * speedMultiplier * Time.deltaTime;
                if (velocity.x > maxXVel * 2.0f * speedMultiplier)
                    velocity.x = maxXVel * 2.0f * speedMultiplier;
            }
            else
            {
                if (prevLookDir == 1)
                {
                    attackScript.FlipFlailAttack(3);
                }
                lookDir = prevLookDir = 3;
                sprite.flipX = true;
                coll2D.offset = lFacingCollOffset;
                velocity.x -= accel * speedMultiplier * Time.deltaTime;
                if (velocity.x < -maxXVel * 2.0f * speedMultiplier)
                    velocity.x = -maxXVel * 2.0f * speedMultiplier;
            }
            PreventWallCollideWithWep();

            pos.x += velocity.x * Time.deltaTime;
            transform.position = pos;
        }
        else
        {
            setupAttack = false;
            attacking = false;
        }
    }
    
    private void Cooldown()
    {
        sr.sprite = weaponSprite;
        if (cooldownTimer > maxCooldown)
        {
            attackReady = true;
            cooldownTimer = 0.0f;
        }
        else
            cooldownTimer += Time.deltaTime;
    }

    private void SetAttack()
    {
        // 0: Face player, then stand still     2%
        // 1: Move in fixed direction - Initially towards the player     3%
        // 2: Standard axe swing towards player     20%
        // 3: Charged axe swing towards player      10%
        // 4: Standard jump     20%
        // 5: Jump and axe swing towards player (midair)    10%
        // 6: Cyclone axe ability       15%
        // 7: Slam axe into ground - creating shockwaves    10%
        // 8: Flail axe madly - chasing the player and then falling over after 5 seconds (?)    10%

        ResetVelocity();

        if (previousAttack != 1 && (Random.Range(0, 2) == 0))
        {
            currentAttack = 1;
        }
        else
        {
            int rand;
            do
            {
                rand = Random.Range(0, 100);
                if (rand < 2)
                    currentAttack = 0;
                else if (rand < 5)
                    currentAttack = 1;
                else if (rand < 25)
                    currentAttack = 2;
                else if (rand < 35)
                    currentAttack = 3;
                else if (rand < 55)
                    currentAttack = 4;
                else if (rand < 65)
                    currentAttack = 5;
                else if (rand < 80)
                    currentAttack = 6;
                else if (rand < 90)
                    currentAttack = 5;
                else if (rand < 100)
                    currentAttack = 8;
            }
            while (currentAttack == previousAttack);
        }

        //currentAttack = 8;

        previousAttack = currentAttack;

        attackReady = false;
        attacking = true;
    }

    private void SetLookDirection()
    {
        if (sprite.flipX)
            lookDir = 3;
        else
            lookDir = 1;
    }

    private void SearchForPlayer()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 bossPos = this.transform.position;

        bool playerWithinView = (playerPos.y - viewDistance.y) < (bossPos.y + halfSize.y) &&
                                (playerPos.y + viewDistance.y) > (bossPos.y - halfSize.y) &&
                                (playerPos.x + viewDistance.x) > (bossPos.x - halfSize.x) &&
                                (playerPos.x - viewDistance.x) < (bossPos.x + halfSize.x);

        engagedPlayer = playerWithinView && !Physics2D.Linecast(bossPos, playerPos, wallMask);
        lifeBar.SetActive(false);
        if(!am.GetPlayingNormalMusic())
        {
            am.Stop("bossRoomMusic");
            am.Play("backgroundSound");
            am.SetPlayingNormalMusic(true);
        }
        if (engagedPlayer)
        {
            am.Stop("backgroundSound");
            am.Play("bossRoomMusic");
            am.SetPlayingNormalMusic(false);
            lifeBar.SetActive(true);
            StartCoroutine(DisplayBossName());
            ResetVelocity();
            LookAtPlayer();
            gate.LockPlayerLockInGate();
        }
    }
    IEnumerator DisplayBossName()
    {
        bossName.SetActive(true);
        bossName.GetComponent<Text>().color = new Color(bossName.GetComponent<Text>().color.r, bossName.GetComponent<Text>().color.g, bossName.GetComponent<Text>().color.b, 0);
        bossNameBackground.GetComponent<SpriteRenderer>().color = bossName.GetComponent<Text>().color;
        while (bossName.GetComponent<Text>().color.a < 1) // fade to black
        {
            bossName.GetComponent<Text>().color = new Color(bossName.GetComponent<Text>().color.r, bossName.GetComponent<Text>().color.g, bossName.GetComponent<Text>().color.b, bossName.GetComponent<Text>().color.a + (Time.deltaTime / 3));
            bossNameBackground.GetComponent<SpriteRenderer>().color = bossName.GetComponent<Text>().color;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        while (bossName.GetComponent<Text>().color.a > 0) // fade to black
        {
            bossName.GetComponent<Text>().color = new Color(bossName.GetComponent<Text>().color.r, bossName.GetComponent<Text>().color.g, bossName.GetComponent<Text>().color.b, bossName.GetComponent<Text>().color.a - (Time.deltaTime));
            bossNameBackground.GetComponent<SpriteRenderer>().color = bossName.GetComponent<Text>().color;
            yield return null;
        }
        bossName.SetActive(false);
    }
    private void LookAtPlayer()
    {
        if (player.transform.position.x > this.transform.position.x)
        {
            lookDir = 1;
            sprite.flipX = false;
            coll2D.offset = rFacingCollOffset;
        }
        else
        {
            lookDir = 3;
            sprite.flipX = true;
            coll2D.offset = lFacingCollOffset;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            collider.gameObject.GetComponent<Player>().Hit(transform.position, hitForce, damageAmount);
            if (player.GetComponent<Player>().GetHealth() <= 0)
            {
                if (!am.GetPlayingNormalMusic())
                {
                    am.Stop("bossRoomMusic");
                    am.Play("backgroundSound");
                    am.SetPlayingNormalMusic(true);
                }
            }
        }
    }

    public override void Hit(int damage, int attackDir)
    {
        if (phase == 0 && health < maxHealth / 2)
        {
            phase = 1;
            speedMultiplier = 1.4f;
            maxCooldown = 1.4f;
        }

        if(health <= damage)
        {
            if (!am.GetPlayingNormalMusic())
            {
                am.Stop("bossRoomMusic");
                am.Play("backgroundSound");
                am.SetPlayingNormalMusic(true);
            }
            GameObject npc = GameObject.FindWithTag("NPC");
            if (npc != null)
                Destroy(npc);
            gate.UnlockPlayerLockInGate();
            ui.RollCredits();
        }

        base.Hit(damage, attackDir);
        lifeBarSlider.value = health;
    }
    public void SetAttackPivot(Vector3 pivotPoint)
    {
        attackPivot = pivotPoint - transform.position;
    }

    public Vector3 GetAttackPivot()
    {
        return attackPivot + transform.position;
    }

    public bool GetXFlip()
    {
        return sr.flipX;
    }

    public bool FlipXPlayer()
    {
        if (sr.flipX)
        {
            sr.flipX = false;
            return false;
        }
        else
        {
            sr.flipX = true;
            return true;
        }
    }
}
