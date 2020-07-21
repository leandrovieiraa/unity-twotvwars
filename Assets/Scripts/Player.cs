using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Controllers")]
    public NavMeshAgent agent;
    public GameController gameController;
    public Entity entity;

    [Header("Patrol")]
    public Vector3 startPosition;
    public float patrolWaitTime;
    float currentPatrolWaitTime;
    bool inRoute;

    [Header("Combat")]
    public Player target;
    public float battleRange = 1.5f;
    public bool attackAnimIsPlaying;
    bool isDead = false;

    [Header("Bloods")]
    public Transform bloodArea;
    public GameObject blood;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        this.name = entity.entityName;
        transform.Find("Canvas").transform.Find("Name").GetComponent<Text>().text = entity.entityName;

        entity.healthSlider = transform.Find("Canvas").transform.Find("HealthSlider").GetComponent<Slider>();
        entity.healthSlider.maxValue = entity.health;
        entity.healthSlider.value = entity.health;

        entity.animator = GetComponent<Animator>();

        startPosition = transform.position;
    }

    private void Update()
    {
        if(entity.health <= 0)
        {
            if(!isDead)
                Die();
            return;
        }

        entity.healthSlider.value = entity.health;
    }

    private void FixedUpdate()
    {
        if(!isDead)
            Behavior();

        if (target != null)
            if (target.isDead)
                target = null;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (isDead || target)
            return;

        if (collider.transform.tag == "Entity")
        {
            Player tmpTarget = collider.GetComponent<Player>();

            if (target == null && tmpTarget.target == null && !tmpTarget.isDead)
            {
                target = tmpTarget;
                tmpTarget.target = this;
            }
             
            //if (target)
                //Debug.LogFormat("{0} vs {1}", entity.entityName, target.entity.entityName);
        }  
    }

    private void OnTriggerExit(Collider collider)
    {
        if (isDead)
            return;

        if (collider.transform.tag == "Entity")
        {
            Player tmpTarget = collider.GetComponent<Player>();
            if (target && tmpTarget.entity.entityName == target.entity.entityName)
            {
                target = null;
                //Debug.LogFormat("{0} away battle from {1}", entity.entityName, tmpTarget.entity.entityName);
            }
        }
    }

    void Behavior()
    {
        // if combat animatuion is playing return
        if (attackAnimIsPlaying)
            return;

        // NPC has target
        if (target)
        {
            // Battle mode
            if (Vector3.Distance(transform.position, target.transform.position) < battleRange)
            {
                //agent.isStopped = true;     
                transform.LookAt(target.transform);
                StartCoroutine(AttackAnimationCoroutine(entity.speed));
            }
            // Chase player
            else
            {
                //agent.isStopped = false;
                transform.LookAt(target.transform);
                agent.SetDestination(target.transform.position);         
                ControlAnimations("MOVING");
            }
        }
        else
        {
            // agent reach path
            if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
            {
                //agent.isStopped = true;
                ControlAnimations("IDLE");

                //  not reach patorl time
                if (currentPatrolWaitTime > 0)
                {
                    currentPatrolWaitTime -= Time.deltaTime;
                    return;
                }

                // patrol time reach, go patrol
                Vector3 newPos = gameController.RandomPointInBounds(gameController.spawnArea.boxCollider.GetComponent<Collider>().bounds);
                Patrol(newPos);

                // reset timer
                currentPatrolWaitTime = patrolWaitTime;
            }
            else
            {
                // no reach path, continue walking
                //agent.isStopped = false;
                ControlAnimations("MOVING");
            }
        }
    }

    void Patrol(Vector3 randPosition)
    {
        if (isDead)
            return;

        Vector3 randomDirection = randPosition;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 5, 1);
        Vector3 finalPosition = hit.position;
      
        agent.destination = finalPosition;
        transform.LookAt(agent.destination);
    }

    void ControlAnimations(string state)
    {
        if(state == "IDLE")
        {
            entity.animator.SetBool("IDLE", true);
            entity.animator.SetBool("MOVING", false);
            entity.animator.SetBool("DEAD", false);
            entity.animator.SetBool("ATTACK", false);
            entity.animator.SetBool("DAMAGED", false);
        }
        else if (state == "MOVING")
        {
            entity.animator.SetBool("IDLE", false);
            entity.animator.SetBool("MOVING", true);
            entity.animator.SetBool("DEAD", false);
            entity.animator.SetBool("ATTACK", false);
            entity.animator.SetBool("DAMAGED", false);
        }
        else if (state == "ATTACK")
        {
            entity.animator.SetBool("IDLE", false);
            entity.animator.SetBool("MOVING", false);
            entity.animator.SetBool("DEAD", false);
            entity.animator.SetBool("ATTACK", true);
            entity.animator.SetBool("DAMAGED", false);
        }
        else if (state == "DAMAGED")
        {
            entity.animator.SetBool("IDLE", false);
            entity.animator.SetBool("MOVING", false);
            entity.animator.SetBool("DEAD", false);
            entity.animator.SetBool("ATTACK", true);         
            entity.animator.SetBool("DAMAGED", true);
        }
        else if (state == "DEAD")
        {
            entity.animator.SetBool("IDLE", false);
            entity.animator.SetBool("MOVING", false);
            entity.animator.SetBool("DEAD", true);
            entity.animator.SetBool("ATTACK", false);
            entity.animator.SetBool("DAMAGED", false);
        }
    }

    private IEnumerator AttackAnimationCoroutine(float attackAnimWaitTime)
    {
        if (isDead)
            yield return null;

        attackAnimIsPlaying = true;
        ControlAnimations("ATTACK");

        // has a target
        if (target != null && !target.isDead)
        {
            // damage
            int randomDamage = UnityEngine.Random.Range(entity.damage / 2, entity.damage);
            int damage = CalculateDamage(randomDamage);

            if (target.tag == "Entity")
            {
                int player_defense = target.CalculateDefense(target.entity.defense);
                int totalDamage = damage - player_defense;
                //Debug.LogFormat("{0} - {1} - {2}", damage, player_defense, totalDamage);
                DoDamage(totalDamage, target);
            }
        }

        yield return new WaitForSeconds(attackAnimWaitTime);
        attackAnimIsPlaying = false;
    }

    public Int32 CalculateDamage(int weaponDamage)
    {
        int damage = (weaponDamage * 2) + UnityEngine.Random.Range(1, 20);
        return damage;
    }

    public Int32 CalculateDefense(int armorDefense)
    {
        int defense = UnityEngine.Random.Range(1, 5) + armorDefense;
        return defense;
    }

    void DoDamage(int totalDamage, Player target)
    {
        if (target.tag == "Entity")
        {
            target.entity.animator.SetBool("DAMAGED", true);
            target.entity.health -= totalDamage;
        }
    }

    void Die()
    {
        ControlAnimations("DEAD");
     
        if (target)
        {
            gameController.UpdateKills(target.entity);
            if(target.agent.isActiveAndEnabled)
                target.agent.ResetPath();
        }
        
        gameController.RemovePlayerFromList(entity);

        isDead = true;
        target = null;

        GameObject bloodSpawn = Instantiate(blood, bloodArea);
        bloodArea.DetachChildren();

        this.gameObject.SetActive(false);
    }
}
