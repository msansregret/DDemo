using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    // Store some references
    private Rigidbody2D rb2d;
    private Animator anim;

    // UI and scoring Variables
    public Text countText;
    public Text winText;
    private int count;

    // Movement variables
    public float groundSpeed;
    public float airSpeed;
    public float maxSpeed = 10f;
    public float jumpForce = 1000f;
    public float doubleJumpForce = 500f;
    private bool canDoubleJump = true;

    // Variable for the current handling of facing left and right, see Flip()
    private bool facingRight = true;
    private Quaternion currentRotation;

    // Variables concerning jumping animation state
    public Transform groundCheck;
    public LayerMask whatIsGround;
    private bool grounded = false;
    private float groundRadius = 0.5f;

    // Rootin Tootin Shootin Variables
    public GameObject projectile;
    public Transform shotSpawn;
    public float WeaponDamage;

    public float rateOfFire = 0.5f;
    private float nextFire = 0.0f;

    public float accuracy = 1f;

    void Start()
    {
        // Setup References
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Setup UI text
        count = 0;
        winText.text = "";
        SetCountText();
    }

	void FixedUpdate()
    {
        // Figure out grounded state
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        anim.SetBool("Ground", grounded);

        // Make sure if we are on the ground that Double Jump has not been used
        if (grounded)
        {
            canDoubleJump = true;
        }

        // Update in air animations that are driven by vertical speed
        anim.SetFloat("vSpeed", rb2d.velocity.y);

        //Add Force movement method
        float moveHorizontal = Input.GetAxis("Horizontal");
        anim.SetFloat("Speed", Mathf.Abs(moveHorizontal));
        float moveVertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        if (grounded && Mathf.Abs(rb2d.velocity.x) <= maxSpeed)
        {
            rb2d.AddForce(movement * groundSpeed);
        }
        else if (!grounded && Mathf.Abs(rb2d.velocity.x) <= maxSpeed)
        {
            rb2d.AddForce(movement * airSpeed);
        }

        // Based on the user input it all sprites are flipped. See Flip().
        if (moveHorizontal > 0 && !facingRight)
        {
            Flip();
        }
        else if(moveHorizontal < 0 && facingRight)
        {
            Flip();
        }
    }

    void Update()
    {
        // Jumping Input handling
        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
            {
                anim.SetBool("Ground", false);
                rb2d.AddForce(new Vector2(0, jumpForce));
            }
            else if (!grounded && canDoubleJump)
            {
                canDoubleJump = false;
                rb2d.AddForce(new Vector2(0, doubleJumpForce));
            }
        }

        // Shooting input handling
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + rateOfFire;
            if (facingRight)
            {
                currentRotation = shotSpawn.rotation;
            }
            else if (!facingRight)
            {
                currentRotation = Quaternion.Inverse(shotSpawn.rotation);
            }
            // The following line applies a random bit of play in the shots as determined by the accuracy variable
            currentRotation = currentRotation * Quaternion.Euler(0f, 0f, currentRotation.z + Random.Range(+accuracy,-accuracy));
            GameObject projectileInstance = (GameObject)Instantiate(projectile, shotSpawn.position, currentRotation);
            projectileInstance.GetComponent<ProjectileDamage>().damage = WeaponDamage;
            projectileInstance.GetComponent<ProjectileDamage>().ignoreTag = this.tag;
        }
    }

    // Flips the local scale of the character in reponse to input whether the character should be facing right or left. This is used until unique sprite assets exist for facing left.
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    // Lets objects with the PickUp tag be picked up.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PickUp") == true)
        {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
        }
    }

    // Sets UI text for the count text. Mostly a debugging thing for now.
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 12)
        {
            winText.text = "You win!";
        }
    }
}

