using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerCollision : MonoBehaviour
{
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;

    public PlayerDetails_SO playerDetails;
    public bool invincible;

    Vector2 size = new Vector2(1, 1);
    Collider2D[] colliders = new Collider2D[100];

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        EventHandler.AddEventListener<Vector3>("MovePlayerPos", OnMovePlayerPosEvent);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener<Vector3>("MovePlayerPos", OnMovePlayerPosEvent);
    }

    private void Start()
    {
        playerDetails.energy = playerDetails.maxEnergy / 2;
        EventHandler.TriggerEvent<float, float>("UpdatePlayerHealth", playerDetails.heath, playerDetails.maxHeath);
        EventHandler.TriggerEvent<float, float>("UpdatePlayerEnergy", playerDetails.energy, playerDetails.maxEnergy);
        EventHandler.TriggerEvent<int>("UpdateEnhancementPointsEvent", playerDetails.enhancementPoints);
    }

    private void Update()
    {
        if (GameManager.Instance.Conquer || playerController.Dead)
            return;

        Array.Clear(colliders, 0, colliders.Length);
        Physics2D.OverlapBoxNonAlloc(transform.position, size, 0, colliders);

        foreach (var coll in colliders)
        {
            if (coll == null)
                break;
            if (coll.gameObject.TryGetComponent(out BaseBarrage barrage))
            {
                if (!invincible)
                {
                    if (!barrage.isFrozen && barrage.damage != 0) 
                        GetHit(barrage.damage);
                    barrage.CollPlayer();
                }
            }
            if (coll.gameObject.TryGetComponent(out StarSpeed starSpeed))
            {
                starSpeed.SetStarSpeed();
            }
            if (coll.gameObject.TryGetComponent(out EnemyAttackArea enemyAttackArea))
            {
                if (!enemyAttackArea.attackOver)
                {
                    GetHit(enemyAttackArea.damage);
                    enemyAttackArea.attackOver = true;
                }
            }
            if (coll.gameObject.TryGetComponent(out Trap trap))
            {
                if (!trap.attackOver)
                {
                    GetHit(trap.damage);
                    trap.attackOver = true;
                }
            }
            if (coll.gameObject.TryGetComponent(out ShieldRanged shieldRanged))
            {
                if (shieldRanged.isRevert && playerController.convoluteStep < 3) 
                {
                    ObjectPool.Instance.PushObject(coll.gameObject);
                    playerDetails.hasConvolute = true;
                }
            }
            if (coll.gameObject.TryGetComponent(out RubbingBulletsArea rubbingBulletsArea))
            {
                if (!invincible && !rubbingBulletsArea.isRubOver)
                {
                    rubbingBulletsArea.isRubOver = true;
                    rubbingBulletsArea.Rub(transform);
                }
            }
            if (coll.gameObject.TryGetComponent(out SpellCard spellCard))
            {
                spellCard.CollPlayer();
                playerDetails.energy += spellCard.cardEnergy;
                EventHandler.TriggerEvent<float, float>("UpdatePlayerEnergy", playerDetails.energy, playerDetails.maxEnergy);
            }
            if (coll.gameObject.TryGetComponent(out IceBallRanged iceBallRanged))
            {
                if (iceBallRanged.becomeDrill)
                {
                    iceBallRanged.CollPlayer();
                    playerDetails.hasDrillingBit = true;
                    EventHandler.TriggerEvent<bool>("UpdateDrillingBitEvent", playerDetails.hasDrillingBit);
                }
            }
        }

        UpdatePlayerEnergy();
    }

    public void GetHit(float damage)
    {
        if (playerDetails.heath > 0)
        {
            invincible = true;
            playerDetails.heath -= damage;
            EventHandler.TriggerEvent<float, float>("UpdatePlayerHealth", playerDetails.heath, playerDetails.maxHeath);
            EventHandler.TriggerEvent("PlayFXSource", SoundName.Hit);
            spriteRenderer.DOColor(Color.red, 1.2f).SetEase(Ease.Flash, 8, 1).OnComplete(() => { spriteRenderer.color = Color.white; });
            transform.DOShakePosition(0.1f, 0.3f, 5, 90);
            StartCoroutine(Invincible());
        }
        else
            playerDetails.heath = 0;
    }

    IEnumerator Invincible()
    {
        yield return new WaitForSeconds(1f);
        invincible = false;
    }

    private void OnMovePlayerPosEvent(Vector3 targetPos)
    {
        transform.position = targetPos;
    }

    private void UpdatePlayerEnergy()
    {
        if (playerDetails.energy <= playerDetails.maxEnergy && playerDetails.enhancementPoints <= playerDetails.maxEnhancementPoints)
        {
            playerDetails.energy += Time.deltaTime;
            EventHandler.TriggerEvent<float, float>("UpdatePlayerEnergy", playerDetails.energy, playerDetails.maxEnergy);
        }
        if (playerDetails.energy > playerDetails.maxEnergy && playerDetails.enhancementPoints < playerDetails.maxEnhancementPoints)
        {
            playerDetails.energy = 0;
            playerDetails.enhancementPoints++;
            EventHandler.TriggerEvent<float, float>("UpdatePlayerEnergy", playerDetails.energy, playerDetails.maxEnergy);
            EventHandler.TriggerEvent<int>("UpdateEnhancementPointsEvent", playerDetails.enhancementPoints);
        }
        else if (playerDetails.energy < 0 && playerDetails.enhancementPoints > 0)
        {
            playerDetails.energy += playerDetails.maxEnergy;
            playerDetails.enhancementPoints--;
            EventHandler.TriggerEvent<float, float>("UpdatePlayerEnergy", playerDetails.energy, playerDetails.maxEnergy);
            EventHandler.TriggerEvent<int>("UpdateEnhancementPointsEvent", playerDetails.enhancementPoints);
        }
    }
}
