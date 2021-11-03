using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyScript : MonoBehaviour
{
    public GameObject[] players;
    public Animator anim;
    public float targetDelay=0.3f;
    public float damage;
    public float jumpSpeed;
    public float health;
    public float triggerRange;
    public float attackRange;
    public float targetUpdateTime = 0.5f;
    [Header("Inumerator times")]
    public float deathAnimDuration;
    public float attackCoolDown;
    NavMeshAgent agent;
    private GameScaler gs;
    private Rigidbody rb;
    private GameObject target;
    private bool doAttack;
    private bool gettingTarget;
    private bool isSpawning;
    private bool isIdleWalking;
    private Vector3 idleDes;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //gs = FindObjectOfType<GameScaler>();
        agent = GetComponent<NavMeshAgent>();
        players = GameObject.FindGameObjectsWithTag("Player");
        target = players[0];
    }
    void Update()
    {
        if (isSpawning)
        {
            return;
        }
        if (!agent.enabled)
        {
            return;
        }
        
        else
        {
            float dis = Vector3.Distance(transform.position, agent.destination);
            if (dis < 5)
            {
                isIdleWalking = false;
            }
            else
            {
                isIdleWalking = true;
            }

            if (isIdleWalking)
            {
                anim.SetBool("IsWalking", true);
                agent.destination = idleDes;
            }
            else
            {
                anim.SetBool("IsWalking", false);
                agent.destination = transform.position;
                StartCoroutine("RandomRotation");
                StartCoroutine("RandomIdlePos");
            }
        }
        if (health <= 0)
        {
            StartCoroutine("Death");
        }
    }

   
    public void CallAgain()
    {
        StartCoroutine("RandomIdlePos");
    }
    public IEnumerator GetTarget()
    {
        gettingTarget = true;

        for (int i = 0; i < players.Length; i++)
        {
            float dis = Vector3.Distance(transform.position, players[i].transform.position);
            float targetDis = Vector3.Distance(transform.position, target.transform.position);
            if (dis < targetDis)
            {
                if (target != players[i])
                {
                    target = players[i];
                }
            }
        }
        yield return new WaitForSeconds(targetUpdateTime);
        gettingTarget = false;
    }
    public IEnumerator DoAttack()
    {
        doAttack = true;
        float dis = Vector3.Distance(transform.position, target.transform.position);
		if (dis > attackRange)
		{
            Ray ray = new Ray(transform.position, -transform.up);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, attackRange))
            {
				if (hitInfo.transform.tag == "Player")
				{
                    ResetAnim();
                    hitInfo.transform.GetComponent<PlayerHealth>().TakeDamage(damage,false,0,0,0, hitInfo.transform.position);
                    anim.SetBool("IsAttacking", true);
                }
            }
        }
        yield return new WaitForSeconds(attackCoolDown);
        doAttack = false;
    }
    public IEnumerator Death()
    {
        anim.SetBool("isDying", true);
        GiveReward();
        yield return new WaitForSeconds(deathAnimDuration);
        Destroy(gameObject);
    }
    public void ResetAnim()
    {
        anim.SetBool("IsAttacking", false);
        anim.SetBool("IsWalking", false);
    }
    public void GiveReward()
    {

    }
}