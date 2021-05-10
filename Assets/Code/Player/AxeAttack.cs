using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeAttack : MonoBehaviour
{
    public GameObject player;
    private Player pScript;
    private SpriteRenderer sr;
    private GameObject col2D;
    private UIManager ui;
    public GameObject pEffect;

    private bool isAttacking = false;
    private const float maxBasicAttackTimer = 0.25f;
    private float basicAttackTimer = 0.0f;
    private const float maxSpecialAttackTimer = 0.7f;
    private float specialAttackTimer = 0.0f;
    private const float maxCooldownTimer = 0.2f;
    private float cooldownTimer = 0.0f;
    private bool cooldown = false;

    private int damage;
    private Vector3 pivot;
    private float attackSpeed = 280.0f;

    private const float chargeThreshold = 1.0f;
    private float chargeTimer;
    private int chargeModifier = 1;
    private bool charged = false;

    private float cycloneFlipThreshold = 0.08f;
    private float cycloneFlipTimer = 0.0f;
    private const float cycloneSpeedModifier = 0.3f;
    private const float chargeSpeedModifier = 0.5f;

    private bool basicAttack = true;
    private bool specialAttack = false;
    private AudioManager am;

    private int attackDir = -1;
    private bool knockbackApplied = false;

    void Start()
    {
        am = FindObjectOfType<AudioManager>();
        pScript = player.GetComponent<Player>();
        ui = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<UIManager>();
        damage = pScript.GetDamage();
        sr = this.gameObject.GetComponent<SpriteRenderer>();
        col2D = this.transform.GetChild(0).gameObject;
        sr.enabled = false;
        col2D.SetActive(false);
    }

    void Update()
    {
        if (isAttacking)
        {
            if (basicAttack)
            {
                BasicAttack();
                SetBasicAttackTimer();
            }
            else if (specialAttack)
            {
                SpecialAttack();
                SetSpecialAttackTimer();
            }
            else
                Cooldown();
            
        } else if (CanAttack())
        {
            if (Input.GetButton("BasicAttack"))
                Charge();

            if (Input.GetButtonUp("BasicAttack"))
            {
                SetupBasicAttack();
                if(pEffect.activeSelf)
                {
                    pEffect.SetActive(false);
                    pEffect.GetComponent<ParticleSystem>().startColor = Color.gray;
                }
            }
            else if (Input.GetButtonDown("SpecialAttack") && pScript.HasMagic())
            {
                 SetupSpecialAttack();
                 ui.UpdateMagic(pScript.GetMagic());
            }
        }
    }
    private void Cooldown()
    {
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer > maxCooldownTimer)
        {
            isAttacking = false;
            cooldownTimer = 0.0f;
        }
    }

    private void SetupSpecialAttack()
    {
        pScript.ReduceMagic(1);
        pScript.SetSpecialAttack(true);
        pScript.SetSpeedModifier(cycloneSpeedModifier);

        if (pScript.FlipXPlayer())
        {
            attackDir = 1;
            transform.localPosition = new Vector3(2.0f, -0.4f, 0.0f);
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, -125.0f);
        }
        else
        {
            attackDir = 3;
            transform.localPosition = new Vector3(-2.0f, -0.4f, 0.0f);
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, 55.0f);
        }

        col2D.transform.localScale = new Vector3(1.0f, 2.5f, 1.0f);

        charged = false;
        chargeTimer = 0.0f;

        knockbackApplied = false;
        basicAttack = false;
        specialAttack = true;
        isAttacking = true;
        sr.enabled = true;
        col2D.SetActive(true);
    }

    private void SpecialAttack()
    {
        cycloneFlipTimer += Time.deltaTime;

        if (cycloneFlipTimer > cycloneFlipThreshold)
        {
            cycloneFlipTimer = 0.0f;
            if (pScript.FlipXPlayer())
            {
                attackDir = 1;
                transform.localPosition = new Vector3(2.0f, -0.4f, 0.0f);
                transform.localEulerAngles = new Vector3(180.0f, 0.0f, -125.0f);
                am.Play("swordSound");
            }
            else
            {
                attackDir = 3;
                transform.localPosition = new Vector3(-2.0f, -0.4f, 0.0f);
                transform.localEulerAngles = new Vector3(0.0f, 0.0f, 55.0f);
                am.Play("swordSound");
            }
        }

    }

    private void Charge()
    {
        pScript.SetSpeedModifier(chargeSpeedModifier);

        chargeTimer += Time.deltaTime;
        if(chargeTimer > chargeThreshold / 4)
        {
            pEffect.SetActive(true);
            pEffect.GetComponent<ParticleSystem>().startColor = Color.black;

        }
        if (chargeTimer > chargeThreshold)
        {
            charged = true;
            pEffect.GetComponent<ParticleSystem>().startColor = Color.grey;
        }
    }

    private void SetupBasicAttack()
    {
        // 0 = Up, 1 = Right, 2 = Down, 3 = Left
        attackDir = pScript.GetAttackDir();

        switch (attackDir)
        {
            case 0:
                if (pScript.GetXFlip())
                {
                    transform.localPosition = new Vector3(0.3f, 2.5f, 0.0f);
                    transform.localEulerAngles = new Vector3(0.0f, 0.0f, -35.0f);
                    pivot = transform.position + new Vector3(0.0f, -1.0f, 0.0f);
                    transform.RotateAround(pivot, Vector3.forward, -20.0f);
                }
                else
                {
                    transform.localPosition = new Vector3(-0.8f, 2.5f, 0.0f);
                    transform.localEulerAngles = new Vector3(0.0f, 0.0f, -35.0f);
                    pivot = transform.position + new Vector3(0.0f, -1.0f, 0.0f);
                    transform.RotateAround(pivot, Vector3.forward, -20.0f);
                }
                
                if (attackSpeed < 0)
                    attackSpeed = Mathf.Abs(attackSpeed);
                break;
            case 1:
                transform.localPosition = new Vector3(2.0f, -0.4f, 0.0f);
                transform.localEulerAngles = new Vector3(180.0f, 0.0f, -125.0f);
                pivot = transform.position + new Vector3(-0.7f, 0.0f, 0.0f);
                transform.RotateAround(pivot, Vector3.forward, 30.0f);
                if (attackSpeed > 0)
                    attackSpeed = -Mathf.Abs(attackSpeed);
                break;
            case 2:
                if (pScript.GetXFlip())
                {
                    transform.localPosition = new Vector3(0.0f, -2.5f, 0.0f);
                    transform.localEulerAngles = new Vector3(0.0f, 180.0f, -215.0f);
                    pivot = transform.position + new Vector3(0.0f, 1.0f, 0.0f);
                    transform.RotateAround(pivot, Vector3.forward, 35.0f);
                }
                else
                {
                    transform.localPosition = new Vector3(-0.5f, -2.5f, 0.0f);
                    transform.localEulerAngles = new Vector3(0.0f, 180.0f, -215.0f);
                    pivot = transform.position + new Vector3(0.0f, 1.0f, 0.0f);
                    transform.RotateAround(pivot, Vector3.forward, 35.0f);
                }
                
                if (attackSpeed > 0)
                    attackSpeed = -Mathf.Abs(attackSpeed);
                break;
            case 3:
                transform.localPosition = new Vector3(-2.0f, -0.4f, 0.0f);
                transform.localEulerAngles = new Vector3(0.0f, 0.0f, 55.0f);
                pivot = transform.position + new Vector3(0.7f, 0.0f, 0.0f);
                transform.RotateAround(pivot, Vector3.forward, -30.0f);
                if (attackSpeed < 0)
                    attackSpeed = Mathf.Abs(attackSpeed);
                break;
            default:
                break;
        }

        col2D.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        pScript.SetAttackPivot(pivot);
        pScript.SetSpeedModifier(1.0f);

        if (charged)
        {
            am.Play("chargedHit");
            chargeModifier = 2;
            charged = false;
        }
        else
        { 
            chargeModifier = 1;
            am.Play("swordSound");
        }

        chargeTimer = 0.0f;

        knockbackApplied = false;
        basicAttack = true;
        isAttacking = true;
        sr.enabled = true;
        col2D.SetActive(true);
    }

    private void BasicAttack()
    {
        pivot = pScript.GetAttackPivot();

        transform.RotateAround(pivot, Vector3.forward, attackSpeed * Time.deltaTime);
    }

    private void SetBasicAttackTimer()
    {
        basicAttackTimer += Time.deltaTime;
        if (basicAttackTimer > maxBasicAttackTimer)
        {
            attackDir = -1;
            isAttacking = false;
            basicAttackTimer = 0.0f;
            sr.enabled = false;
            col2D.SetActive(false);
        }
    }

    private void SetSpecialAttackTimer()
    {
        specialAttackTimer += Time.deltaTime;
        if (specialAttackTimer > maxSpecialAttackTimer)
        {
            attackDir = -1;
            pScript.SetSpecialAttack(false);
            pScript.SetSpeedModifier(1.0f);
            specialAttack = false;
            specialAttackTimer = 0.0f;
            sr.enabled = false;
            col2D.SetActive(false);
        }
    }

    private bool CanAttack()
    {
        return !isAttacking && pScript.CanMove();
    }

    public void CollideWithObject(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Enemy":
                other.gameObject.GetComponent<BasicMovement>().Hit(damage * chargeModifier, attackDir);
                if (!knockbackApplied)
                {
                    am.Play("playerPogo");
                    pScript.ApplyWeaponKnockback(attackDir);
                    knockbackApplied = true;
                }
                break;
            case "Boss":
                other.gameObject.GetComponent<BossBehaviour>().Hit(damage * chargeModifier, attackDir);
                if (!knockbackApplied)
                {
                    am.Play("playerPogo");
                    pScript.ApplyWeaponKnockback(attackDir);
                    knockbackApplied = true;
                }
                break;
            case "Spikes":
                if (!knockbackApplied)
                {
                    am.Play("playerPogo");
                    pScript.ApplyWeaponKnockback(attackDir);
                    knockbackApplied = true;
                }
                break;
            default:
                break;
        }
    }
    public void UpdateDamage()
    {
        damage = pScript.GetDamage();
    }
}
