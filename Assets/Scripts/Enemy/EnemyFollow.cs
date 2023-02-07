using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyFollow : MonoBehaviour
{
	public NavMeshAgent enemy;
	public Transform player;
	public Vector3 playerEnemyVec;
	public float playerEnemyDist;
	[SerializeField]
	public PlayerController pc;
	   
	// patrol variables
    public float patrolThreshold = 64.0f;
    public float patrolRadius = 4.0f;
    public int patrolDirection = 1; // North: 1, South: -1
    public Vector3 patrolCenter;
    public Vector3 patrolDest;
	public float patrolRemainingDist;
    public bool isPatrolling;
	float timeSinceHit = 0.0f;
    float hitCooldown = 5f;
	//Animations
	[SerializeField]
	public Animator animator;

	//UI variables
	public HealthBar healthBar;
	public bool isHit;

	public float currentHealth;
	public GameObject canvas;
	static float SpeedFunction(float distance){
		// speed function which depends on player-enemy distance and player speed
		float p = Player.playerSpeed * 0.8f;
		float b = (5.0f*p - Enemy.minSpeed) / 4.0f;
		float a = (p - Enemy.minSpeed) / 4.0f;
		return (distance > 2) ? b - a * Mathf.Log(distance, 2) : p;
	}
	void Awake() {
		pc = GameObject.Find("PLAYER").GetComponent<PlayerController>();
	}

	void Start(){
		canvas = gameObject.transform.Find("Canvas").gameObject;
		currentHealth = Enemy.health;
		healthBar.SetMaxHealth(currentHealth);
		player = GameObject.FindWithTag("Player").transform;
		enemy = GetComponent<NavMeshAgent>();
		isPatrolling = false;
	}
	void Update(){
		// update the destination of the enemy (player's destination)
		playerEnemyDist = Vector3.Distance(player.position, enemy.transform.position);
		if(isHit) {
            //ProcessKnockback(enemyDirection);
            timeSinceHit += Time.deltaTime;
            // Debug.Log(timeSinceHit);
            if(timeSinceHit >= hitCooldown) {
                isHit = false;
                timeSinceHit = 0.0f;
            }
        }
		if (playerEnemyDist <= patrolThreshold){
			// follow the enemy
			animator.SetBool("IsAttacking", true);
			enemy.SetDestination(player.position);
			enemy.speed = SpeedFunction(playerEnemyDist);
			isPatrolling = false;
			canvas.SetActive(true);
		}
		else{
			// patrol
			animator.SetBool("IsAttacking", false);
			Patrol();
			canvas.SetActive(false);
		}
		if (currentHealth <= 0.0f){
            Destroy(this.gameObject);
            Level.TotalEnemies -= 1;
			Player.enemiesKilled += 1;
        }
	}
    void Patrol(){
        // move around the area
        if (!isPatrolling){
            isPatrolling = true;
            patrolCenter = enemy.transform.position;
            patrolDest = patrolCenter;
            patrolDest.x += patrolRadius * patrolDirection;
            enemy.SetDestination(patrolDest);
        }

        // change the direction of the enemy when enemy goes out of the patrol radius
        patrolRemainingDist = Mathf.Sqrt(Mathf.Pow(Vector3.Distance(patrolCenter, enemy.transform.position), 2) - Mathf.Pow(patrolCenter.y - enemy.transform.position.y, 2)) + 0.5f;
        if (patrolRemainingDist >= patrolRadius && enemy.remainingDistance < patrolRadius){
            patrolDirection *= -1;
            //patrolDest = patrolCenter;
            patrolDest.x += 2 * patrolRadius * patrolDirection;
            enemy.SetDestination(patrolDest);
        }
    }
	private void OnTriggerEnter(Collider collider){
        if(collider.gameObject.tag == "Bullet") {
            currentHealth -= Player.damage;
            healthBar.SetHealth(currentHealth);
        }
		if(collider.gameObject == player.gameObject) {
			if(!isHit) {
				isHit = true;
                Player.health -= Enemy.damage;
                pc.healthBar.SetHealth();
				pc.ProcessKnockback(transform.forward);
			}
		}
    }
}

