using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private Transform cameraTransform;
    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private Transform bulletParent;
    [SerializeField]
    private GameObject bulletPrefab; 
    [SerializeField]
    private Transform barrelTransform;
    [SerializeField]
    public float rotationSpeed = 5f;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;
    public InputAction interactAction;
    public InputAction pauseAction;
    [SerializeField]
    public PlayerHealthBar healthBar;
    [SerializeField]
    public GameObject enemy;
    float timeSinceShot = 0.0f;
    bool canShoot;

    public bool isHit;

    Vector3 enemyDirection;
    int numBullets = 8;
    float maxBulletOffset = 2.0f;
    [SerializeField]
    public ItemList itemDisplay;
    [SerializeField]
    public Animator animator;
    public ImpactReceiver fakeRb;

    private void Awake() {
        Player.health = 100.0f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraTransform = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        fakeRb = GetComponent<ImpactReceiver>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
        interactAction = playerInput.actions["Interact"];
        animator.SetBool("IsWalking", false);
        
    }
    private void Start() {
        isHit = false;
        canShoot = true;
        Cursor.lockState = CursorLockMode.Confined;
        healthBar.SetMaxHealth();
        enemyDirection = -transform.forward;
        for(int i = 0; i < Player.itemList.Length; i ++){
            itemDisplay.AddItemToList(i, Player.itemList[i]);
        }
        StartCoroutine(ProcessHealth());
    }

    private void ShootGun() {
		// use trig functions and quaternions to fire bullets in a cone shape
        float angle, xOffset, yOffset, zOffset;
        for (int i = 0; i < numBullets; i++){
            GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, barrelTransform.rotation, bulletParent); 
            bullet.tag = "Bullet";
            BulletController bc = bullet.GetComponent<BulletController>();
            RaycastHit hit = new RaycastHit();
			
			// angle of the bullet in the xy-plane relative to the camera's perspective and xyz offsets
            angle = i * 360.0f / numBullets;
            xOffset = maxBulletOffset * Mathf.Cos(angle);
            yOffset = maxBulletOffset * Mathf.Sin(angle);
            zOffset = maxBulletOffset * Mathf.Cos(angle);

            if(Physics.Raycast(cameraTransform.position, Quaternion.Euler(xOffset, yOffset, zOffset) * cameraTransform.forward, out hit, Mathf.Infinity)) {
                bc.target = hit.point;
                bc.hit = true;
            }
        }
        canShoot = false;
    }
    

    void Update()
    {
        if(!canShoot) {
            timeSinceShot += Time.deltaTime;
            if(timeSinceShot >= Player.shotCooldown) {
                canShoot = true;
                timeSinceShot = 0.0f;
            }
        }
        if(canShoot && shootAction.triggered == true) {
            ShootGun();
        }

        
        if (Player.health <= 0.0f) {
            // PLAY THE DEATH ANIMATION and WAIT FOR SECONDS

            // ^^^^^^^^^^^^^^^^^^^^^
            Level.Clear();
            Player.Clear();
            Boss.Clear();
            Enemy.Clear();
            LoadGame.saveGame();
            SceneManager.LoadScene("MainMenu");
        }
        ProcessInput();
        //Rotate towards camera direction
        RotateTowardsCamera();
        
    }

    public void ProcessInput() {
        
        groundedPlayer = controller.isGrounded; // checks to see if the player is grounded
        if (groundedPlayer && playerVelocity.y < 0)  // checks if player is jumping
        {
            playerVelocity.y = 0f;
        }
        Vector2 input = moveAction.ReadValue<Vector2>();   // reads keydown input as 2d vector (w/s = y, a/d = x)
        Debug.Log(input);
        animator.SetFloat("moveX", input.x);
        animator.SetFloat("moveY", input.y);
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized; //normalize movement to go in camera direction
        move.y = 0f;
        controller.Move(move * Time.deltaTime * Player.playerSpeed);
        // Changes the height position of the player..
        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void RotateTowardsCamera() {
        float targetAngle = cameraTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void teleport(Vector3 point){
        Debug.Log("Teleporting Player to: " + point);
        CharacterController cc = transform.GetComponent<CharacterController>();
        cc.enabled = false;
        gameObject.transform.SetPositionAndRotation(point, Quaternion.identity);
        cc.enabled = true;
    }

    IEnumerator ProcessHealth() {
        while(Player.health > 0){
            if(Player.health < Player.maxHealth){
                Player.health += Player.regen;
            }
            healthBar.SetHealth();
            yield return wait(1.0f);
        }
        yield return null;
    }

    public IEnumerator wait(float waitTime) {
        float counter = 0;
        while (counter < waitTime)  {
            //Increment Timer until counter >= waitTime
            counter += Time.deltaTime;
            yield return null;
        }
    }

    
    public void CollectItem(int item) {
        Player.UpdatePlayer(item, 1);
        Player.toString();
        itemDisplay.AddItemToList(item, 1);
        healthBar.SetMaxHealth();
        // Item.getModifierByID(item);
        // for(int i = 0; i < 10; i++) {
        //     Debug.Log(i + " " + Player.itemList[i]);
        // }
    }
    public void ProcessKnockback(Vector3 direction) {
        direction *= 50.0f;
        fakeRb.AddImpact(direction);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Item") {
            GameObject chestItem = other.gameObject;
            int id = chestItem.GetComponent<Identifier>().id;
            CollectItem(id);
            Destroy(chestItem.gameObject);
        }

    }
    
}