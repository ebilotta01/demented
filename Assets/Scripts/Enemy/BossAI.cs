using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System;

public class BossAI : MonoBehaviour
{
    public NavMeshAgent boss;
    public Transform player;
	public PlayerController pc;
    private BossActions currentState;
    public float health;
    [SerializeField]
	public BossHealthBar healthBar;
    public float attackRadius = 8.0f;
    public float playerBossDist;
    private bool isDodging;
    private Queue<Vector3> bezierCurve;
    private Vector3 [] bezierPts;
    private int numBezierPts = 20;
    public bool bossFightStarted = false;
    public DebuffDisplay debuffDisplay;
    

    // Boss states
    public enum BossActions{
        Idle = 0,
        Following = 1,
        Dodging = 2
    }
    /*
    Speed function was modeled in Desmos:
    https://www.desmos.com/calculator/sverbcdwcs
    */
	static float SpeedFunction(float distance){
		// speed function which depends on player-boss distance and player speed
		return (distance > 2 && distance < 32) ? Boss.b - Boss.a * Mathf.Log(distance, 2) : (distance <= 2) ? Boss.minSpeed : Boss.maxSpeed;
	}
	void Awake() {
		pc = GameObject.Find("PLAYER").GetComponent<PlayerController>();
        debuffDisplay = GameObject.Find("Debuff").GetComponent<DebuffDisplay>();
	}

    void Start(){
        healthBar = GameObject.Find("Boss Health").GetComponent<BossHealthBar>();
        boss = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        currentState = BossActions.Idle;
        healthBar.gameObject.SetActive(false);
        Boss.health = Boss.MaxHealth;
        healthBar.SetHealth();
        bezierCurve = new Queue<Vector3>();
        bezierPts = new Vector3[4];
    }

    void Update(){
        // if health is below zero, then boss is defeated        
        if (Boss.health <= 0.0f){
            currentState = BossActions.Idle;
            boss.transform.SetPositionAndRotation(Level.BossRoom.Center, Quaternion.identity);
            Destroy(this.gameObject);
            Player.inBossRoom = false;
            Level.switchLevel();
        }

        // allow boss to move only when the player is in the boss room
        if (Player.inBossRoom && !bossFightStarted){
            currentState = currentState | BossActions.Following;
            healthBar.gameObject.SetActive(true);
            StartBossFight();
            bossFightStarted = true;
        }
        // change the speed of the boss
        playerBossDist = xzDistance(player.position, boss.transform.position);
        //boss.speed = SpeedFunction(playerBossDist);

        /* Boss will dodge player attacks to try to avoid getting hit while getting closer to the player */
        /* If boss gets close to player, boss will only follow the player and try to tag him             */
        if ((currentState & BossActions.Dodging) != 0){
            boss.speed = SpeedFunction(playerBossDist/8.0f);
            Dodge();
        }
        else if ((currentState & BossActions.Following) != 0){
            boss.speed = SpeedFunction(playerBossDist);
            Follow();
        }
    }

    public void StartBossFight() {
        List<int> subList = getSubItemList();
        if (subList.Count > 0){
            StartCoroutine(StartDebuffCycle(subList));
        }
    }
    private List<int> getSubItemList(){
        // get itemList and convert it to a Dictionary
        Dictionary<int, int> itemCounter = new Dictionary<int, int>();
        for (int i = 0; i < Player.itemList.Length; i++){
            itemCounter.Add(i, Player.itemList[i]);
        }
        
        // sort Dictionary by value
        var myList = itemCounter.ToList().OrderByDescending(x => x.Value);
        List<int> subList = new List<int>();

        /* Fill in sublist with the top 3 most frequent items           */
        /* If there are less than 3 items, return as many as there is   */
        foreach (KeyValuePair<int, int> pair in myList){
            if (subList.Count == 3 || pair.Value == 0) 
                break;
            subList.Add(pair.Key);
        }

        return subList;
    }
    public IEnumerator wait(float waitTime) {
        float counter = 0;
        while (counter < waitTime)  {
            //Increment Timer until counter >= waitTime
            counter += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator StartDebuffCycle(List<int> items) {
        // debuff only if boss is not defeated
        while(Boss.health > 0) {
            // choose an item ID from the sublist
            int index = UnityEngine.Random.Range(0, items.Count);
            Item.Modifier debuff = Item.getModifierByID(items[index]);
            //Item.Modifier debuff = (Item.Modifier)items[index];

            // debuff the player's attribute relative to the item ID
            float original = Player.GetPlayerAttribute(debuff);
            Player.DebuffPlayer(debuff);
            debuffDisplay.Show((int)debuff);
            Debug.Log("debuffing " + debuff);
            yield return wait(5.0f);
            Player.SetPlayerAttribute(debuff, original);
            debuffDisplay.Hide((int)debuff);
        }
    }
    private void OnTriggerEnter(Collider collider){
        // update boss health and start dodging player attacks
        if (collider.tag == "Bullet"){
            Boss.health -= Player.damage;
            healthBar.SetHealth();
            // if true, then dodge
            if (playerBossDist >= 8.0f && bezierCurve.Count == 0){
                currentState = currentState | BossActions.Dodging;
                CreateBezierCurve();
                boss.isStopped = true;
                boss.ResetPath();
                foreach (int id in getSubItemList()){
                    Debug.Log("ID: " + id + "\t" + "Count: " + Player.itemList[id]);
                }
            }
        }
        // update the player's health and health bar when boss colliders with player
        if (collider.gameObject == player.gameObject){
            Player.health -= Boss.damage;
            pc.healthBar.SetHealth();
        }
    }
    void Follow(){
        boss.SetDestination(player.position);
        currentState = currentState | BossActions.Following;
    }
    void Dodge(){
        if (bezierCurve.Count > 0){
            if (boss.remainingDistance <= 1.0f){
                boss.SetDestination(bezierCurve.Dequeue());
            }
        }
        else{
            currentState = currentState ^ BossActions.Dodging;
        }
    }


    //void DodgeAttacks(){
        //// stop dodging attacks if the boss has met the dodging point
        //if (xzDistance(dodgePt, boss.transform.position) <= 0.1){
            //currentState = currentState ^ BossActions.Dodging;
        //}

        //// compute destination point
        //if (!isDodging){
            //// determine if the boss or player is closer to the origin
            //Vector3 closestToOrigin = (player.position.magnitude > boss.transform.position.magnitude) ? boss.transform.position : player.position;

            //// translate the midpoint-closestToOrigin vector to the origin
            //Vector3 positionVector = (player.position + boss.transform.position) / 2.0f - closestToOrigin;

            //// compute the boss's destination point
            //float dodgingAngle = Mathf.PI / 4.0f;
            //int side = (int)Mathf.Pow(-1, UnityEngine.Random.Range(0, 10));
            //dodgePt.x = positionVector.x * Mathf.Cos(side * dodgingAngle) - positionVector.z * Mathf.Sin(side * dodgingAngle) + closestToOrigin.x;
            //dodgePt.y = boss.transform.position.y;
            //dodgePt.z = positionVector.x * Mathf.Sin(side * dodgingAngle) + positionVector.z * Mathf.Cos(side * dodgingAngle) + closestToOrigin.z;

            //// change boss's destination and speed
            //boss.SetDestination(dodgePt);
            //isDodging = true;
        //}
    //}

    // https://www.desmos.com/calculator/jj9mnhvqhs
    // https://www.desmos.com/calculator/jjlysduwuj
    // https://www.desmos.com/calculator/bxeia3iqx4
    // https://www.desmos.com/calculator/vkbcm6zy67
    private void CreateBezierCurve(){
        getPoints();
        for (int i = 1; i <= numBezierPts/5; i++){
            bezierCurve.Enqueue(bezierEquation((float)i/numBezierPts));
        }
    }
    private void getPoints(){
        // set initial and final points
        bezierPts[0] = boss.transform.position;
        bezierPts[3] = player.position;

        // move left or right (left: 0, right: 1)
        int side = UnityEngine.Random.Range(0, 2);
        
        // get a point on the orthogonal vector to the boss-player vector
        Vector3 Q = player.position - boss.transform.position;
        Vector3 R = new Vector3(Q.z, Q.y, -Q.x) / 4.0f;
        float alpha1 = Mathf.PI / 4.0f;
        float alpha2 = 5.0f * alpha1;

        // adjust parameters to make path go left or right
        if (side == 0){
            R.x *= -1;
            R.z *= -1;
            alpha1 -= Mathf.PI / 2.0f;
            alpha2 -= Mathf.PI / 2.0f;
        }
        
        // compute p1 and p2
        bezierPts[1] = getPoint(Q, R, alpha1, boss.transform.position);
        bezierPts[2] = getPoint(Q, R, alpha2, player.position);
    }
    private Vector3 getPoint(Vector3 Q, Vector3 R, float alpha, Vector3 bias){
        Vector3 pt = Vector3.zero;
        float Cx = R.x * Mathf.Cos(alpha) - R.z * Mathf.Sin(alpha);
        float Cz = R.x * Mathf.Sin(alpha) + R.z * Mathf.Cos(alpha);

        pt.x = 0.5f * xzDistance(boss.transform.position, player.position) * Cx / 4.0f + bias.x;
        pt.y = Q.y;
        pt.z = 0.5f * xzDistance(boss.transform.position, player.position) * Cz / 4.0f + bias.z;

        return pt;
    }
    private Vector3 bezierEquation(float t){
        return Mathf.Pow(1-t, 3)*bezierPts[0] + 3.0f*Mathf.Pow(1-t, 2)*t*bezierPts[1] + 3*(1-t)*Mathf.Pow(t, 2)*bezierPts[2] + Mathf.Pow(t, 3)*bezierPts[3];
    }

    float xzDistance(Vector3 vec1, Vector3 vec2){
        return Mathf.Sqrt(Mathf.Pow(vec1.x - vec2.x, 2) + Mathf.Pow(vec1.z - vec2.z, 2));
    }


}
