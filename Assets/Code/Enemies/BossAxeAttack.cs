using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAxeAttack : MonoBehaviour
{
    private GameObject boss;
    private BossBehaviour bScript;
    private SpriteRenderer sr;
    private GameObject col2D;

    private bool isAttacking = false;
    private const float maxBasicAttackTimer = 0.25f;
    private float basicAttackTimer = 0.0f;
    private const float maxSpecialAttackTimer = 0.7f;
    private float specialAttackTimer = 0.0f;
    private const float maxFlailAttackTimer = 3.5f;
    private float flailAttackTimer = 0.0f;
    private const float maxFlailDirSwitchTimer = 0.175f;
    private float flailDirSwitchTimer = 0.0f;

    private const float chargeThreshold = 1.0f;
    private float chargeTimer;
    private int chargeModifier = 1;
    private bool charged = false;

    private float cycloneFlipThreshold = 0.08f;
    private float cycloneFlipTimer = 0.0f;
    private const float cycloneSpeedModifier = 0.3f;
    private const float cycloneRangeModifier = 7.5f;

    private bool basicAttack = true;
    private bool specialAttack = false;
    private bool flailAttack = false;

    private Vector3 pivot;
    private float basicAttackSpeed = 280.0f;
    private float flailAttackSpeed = 420.0f;
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
        bScript = boss.GetComponent<BossBehaviour>();
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
            if (specialAttack)
            {
                SpecialAttack();
                SetSpecialAttackTimer();
            }
            if (flailAttack)
            {
                FlailAttack();
                SetFlailAttackTimer();
            }
        }
    }

    public void SetupSpecialAttack()
    {
        if (bScript.GetXFlip())
        {
            transform.localPosition = new Vector3(5.8f, -0.4f, 0.0f);
            transform.localEulerAngles = new Vector3(180.0f, 0.0f, -125.0f);
        }
        else
        {
            transform.localPosition = new Vector3(-5.8f, -0.4f, 0.0f);
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, 55.0f);
        }

        col2D.transform.localScale = new Vector3(1.0f, 2.5f, 1.0f);

        charged = false;
        chargeTimer = 0.0f;

        basicAttack = false;
        specialAttack = true;
        flailAttack = false;
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
            if (bScript.FlipXPlayer())
            {
                transform.localPosition = new Vector3(5.8f, -0.4f, 0.0f);
                transform.localEulerAngles = new Vector3(180.0f, 0.0f, -125.0f);
            }
            else
            {
                transform.localPosition = new Vector3(-5.8f, -0.4f, 0.0f);
                transform.localEulerAngles = new Vector3(0.0f, 0.0f, 55.0f);
            }
        }
    }

    public void Charge()
    {
        chargeTimer += Time.deltaTime;
        if (chargeTimer > chargeThreshold)
            charged = true;
    }

    public void SetupBasicAttack(int attackDir)
    {
        // 0 = Up, 1 = Right, 2 = Down, 3 = Left
        switch (attackDir)
        {
            case 0:
                transform.localPosition = new Vector3(0.0f, 6.0f, 0.0f);
                transform.localEulerAngles = new Vector3(0.0f, 0.0f, -35.0f);
                pivot = transform.position + new Vector3(0.0f, -3.3f, 0.0f);
                transform.RotateAround(pivot, Vector3.forward, -20.0f);
                if (basicAttackSpeed < 0)
                    basicAttackSpeed = Mathf.Abs(basicAttackSpeed);
                break;
            case 1:
                transform.localPosition = new Vector3(5.8f, -0.4f, 0.0f);
                transform.localEulerAngles = new Vector3(180.0f, 0.0f, -125.0f);
                pivot = transform.position + new Vector3(-3.3f, 0.0f, 0.0f);
                transform.RotateAround(pivot, Vector3.forward, 30.0f);
                if (basicAttackSpeed > 0)
                    basicAttackSpeed = -Mathf.Abs(basicAttackSpeed);
                break;
            case 2:
                transform.localPosition = new Vector3(0.0f, -6.0f, 0.0f);
                transform.localEulerAngles = new Vector3(0.0f, 180.0f, -215.0f);
                pivot = transform.position + new Vector3(0.0f, 3.3f, 0.0f);
                transform.RotateAround(pivot, Vector3.forward, 35.0f);
                if (basicAttackSpeed > 0)
                    basicAttackSpeed = -Mathf.Abs(basicAttackSpeed);
                break;
            case 3:
                transform.localPosition = new Vector3(-5.8f, -0.4f, 0.0f);
                transform.localEulerAngles = new Vector3(0.0f, 0.0f, 55.0f);
                pivot = transform.position + new Vector3(3.3f, 0.0f, 0.0f);
                transform.RotateAround(pivot, Vector3.forward, -30.0f);
                if (basicAttackSpeed < 0)
                    basicAttackSpeed = Mathf.Abs(basicAttackSpeed);
                break;
            default:
                break;
        }

        col2D.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        bScript.SetAttackPivot(pivot);

        if (charged)
        {
            chargeModifier = 2;
            charged = false;
        }
        else
            chargeModifier = 1;

        chargeTimer = 0.0f;

        basicAttack = true;
        specialAttack = false;
        flailAttack = false;
        isAttacking = true;
        sr.enabled = true;
        col2D.SetActive(true);
    }

    public void SetupFlailAttack(int attackDir)
    {
        // 0 = Up, 1 = Right, 2 = Down, 3 = Left
        if (attackDir == 1)
        {
            transform.localPosition = new Vector3(5.8f, -0.4f, 0.0f);
            transform.localEulerAngles = new Vector3(180.0f, 0.0f, -125.0f);
            pivot = transform.position + new Vector3(-3.3f, 0.0f, 0.0f);
            transform.RotateAround(pivot, Vector3.forward, 30.0f);
            if (flailAttackSpeed > 0)
                flailAttackSpeed = -Mathf.Abs(flailAttackSpeed);
        }
        else
        {
            transform.localPosition = new Vector3(-5.8f, -0.4f, 0.0f);
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, 55.0f);
            pivot = transform.position + new Vector3(3.3f, 0.0f, 0.0f);
            transform.RotateAround(pivot, Vector3.forward, -30.0f);
            if (flailAttackSpeed < 0)
                flailAttackSpeed = Mathf.Abs(flailAttackSpeed);
        }

        col2D.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        bScript.SetAttackPivot(pivot);

        basicAttack = false;
        specialAttack = false;
        flailAttack = true;
        isAttacking = true;
        sr.enabled = true;
        col2D.SetActive(true);
    }

    public void FlipFlailAttack(int attackDir)
    {
        flailDirSwitchTimer = 0.0f;

        // 0 = Up, 1 = Right, 2 = Down, 3 = Left
        if (attackDir == 1)
        {
            transform.localPosition = new Vector3(5.8f, -0.4f, 0.0f);
            transform.localEulerAngles = new Vector3(180.0f, 0.0f, -125.0f);
            pivot = transform.position + new Vector3(-3.3f, 0.0f, 0.0f);
            transform.RotateAround(pivot, Vector3.forward, 30.0f);
            flailAttackSpeed = -Mathf.Abs(flailAttackSpeed);
        }
        else
        {
            transform.localPosition = new Vector3(-5.8f, -0.4f, 0.0f);
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, 55.0f);
            pivot = transform.position + new Vector3(3.3f, 0.0f, 0.0f);
            transform.RotateAround(pivot, Vector3.forward, -30.0f);
            flailAttackSpeed = Mathf.Abs(flailAttackSpeed);
        }

        bScript.SetAttackPivot(pivot);

        sr.enabled = true;
        col2D.SetActive(true);
    }

    private void FlailAttack()
    {
        pivot = bScript.GetAttackPivot();

        transform.RotateAround(pivot, Vector3.forward, flailAttackSpeed * Time.deltaTime);

        flailDirSwitchTimer += Time.deltaTime;
        if (flailDirSwitchTimer > maxFlailDirSwitchTimer)
        {
            flailAttackSpeed *= -1.0f;
            flailDirSwitchTimer = 0.0f;
        }
    }

    private void BasicAttack()
    {
        pivot = bScript.GetAttackPivot();

        transform.RotateAround(pivot, Vector3.forward, basicAttackSpeed * Time.deltaTime);
    }

    private void SetBasicAttackTimer()
    {
        basicAttackTimer += Time.deltaTime;
        if (basicAttackTimer > maxBasicAttackTimer)
        {
            isAttacking = false;
            basicAttackTimer = 0.0f;
            sr.enabled = false;
            col2D.SetActive(false);
        }
    }

    private void SetFlailAttackTimer()
    {
        flailAttackTimer += Time.deltaTime;
        if (flailAttackTimer > maxFlailAttackTimer)
        {
            isAttacking = false;
            flailAttackTimer = 0.0f;
            flailDirSwitchTimer = 0.0f;
            sr.enabled = false;
            col2D.SetActive(false);
        }
    }

    private void SetSpecialAttackTimer()
    {
        specialAttackTimer += Time.deltaTime;
        if (specialAttackTimer > maxSpecialAttackTimer)
        {
            isAttacking = false;
            specialAttack = false;
            specialAttackTimer = 0.0f;
            sr.enabled = false;
            col2D.SetActive(false);
        }
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public void CollideWithPlayer(Player pScript)
    {
        pScript.Hit(this.transform.position, 20, 1 * chargeModifier);

        if (flailAttack)
        {
            isAttacking = false;
            flailAttackTimer = 0.0f;
            flailDirSwitchTimer = 0.0f;
            sr.enabled = false;
            col2D.SetActive(false);
            flailAttack = false;
        }
    }

    public bool GetCharged()
    {
        return charged;
    }
}
