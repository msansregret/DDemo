using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {

    private Rigidbody2D rb2d;

    // Tracking and engaging the player variables
    public GameObject player;
    public float engagementRange = 50;
    private bool targetEngaged = false;

    // Movement Variables
    public float speed;
    public float maxSpeed = 10f;

    // Rootin Tootin Shootin Variables
    public GameObject projectile;
    public Transform shotSpawn;

    public float rateOfFire = 0.5f;
    private float nextFire = 0.0f;

    public float accuracy = 1f;

    // Variable for the current handling of facing left and right, see Flip()
    private bool facingRight = true;
    private Quaternion currentRotation;

    void Start ()
    {
        rb2d = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
    void FixedUpdate()
    {
        // If target is too far away move closer, unless already moving too fast
        if (targetEngaged == true && Vector2.Distance(player.transform.position, this.transform.position) >= 20 && rb2d.velocity.magnitude <= maxSpeed)
        {
            rb2d.AddForce((player.transform.position - this.transform.position) * speed);
        }
    }

	void Update ()
    {
        // Track and engage Player when they are close enough
	    if (Vector2.Distance(player.transform.position, this.transform.position) <= engagementRange)
        {
            targetEngaged = true;
            faceTarget();
            if (Time.time > nextFire)
            {
                nextFire = Time.time + rateOfFire;

                // Find the player and figure out what rotation to have to point from shot spawn to the target.
                Vector2 targetVector = player.transform.position - shotSpawn.position;
                currentRotation = Quaternion.LookRotation(targetVector);

                // The following line applies a random bit of play in the shots as determined by the accuracy variable
                currentRotation = currentRotation * Quaternion.Euler(90, 0f, currentRotation.z + Random.Range(+accuracy, -accuracy));
                GameObject projectileInstance = (GameObject)Instantiate(projectile, shotSpawn.position, currentRotation);
                projectileInstance.GetComponent<ProjectileDamage>().damage = 10;
                projectileInstance.GetComponent<ProjectileDamage>().ignoreTag = this.tag;

            }
        }
	}

    // Checks if the current target is left or right in the world space and then flips around to face them
    void faceTarget()
    {
        if (player.transform.position.x >= this.transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (player.transform.position.x <= this.transform.position.x && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
