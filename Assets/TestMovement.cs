using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    /**
     * Test Movement / Physics System
     * 
     * 11/16/19
     * 
     */

    // Walk Speed
    public float speed = 40f;
    // Jumping Speed
    public float jumpSpeed = 10f;
    // If the player is jumping
    public bool isJumping = false;
    // The y level before leaving the ground
    public float yBeforeJump = 0f;
    // The maximum jump height (unused in favor of acceleration system)
    public float jumpMax = 2f;
    // If the player is currently on the ground.
    public bool isGrounded = true;

    private float deltaY = 0;
    private float previousY = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, 0, -10);
        previousY = transform.position.y;
        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
        if(Input.GetKey(KeyCode.W) && !isJumping && isGrounded)
        {
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            yBeforeJump = gameObject.transform.position.y;
            isJumping = true;
            jump();
        }
        if (isJumping) jump();
        deltaY = transform.position.y - previousY;
    }
    // The current acceleration
    public float acc = 3;
    // The default acceleration
    public float defAcc = 3;
    // the acceleration speed
    public float accSpeed = 0.2f;

    void jump()
    {
        if (!isJumping) return;
       // if (gameObject.transform.position.y > (yBeforeJump + jumpMax))
       // {
           // Debug.Log(yBeforeJump + jumpMax);
           // isJumping = false;
           // gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
           // acc = 3;
           // return;
        //}
        //float velocY = (-2 * (gameObject.transform.position.y - yBeforeJump) - 4) * jumpSpeed * Time.deltaTime;
        float velocY = acc * jumpSpeed * Time.deltaTime;
        if(acc < 0.3)
        {
            isJumping = false;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            acc = defAcc;
            return;
        }
        acc -= accSpeed * Time.deltaTime;
        //gameObject.transform.position += new Vector3(0, jumpSpeed * Time.deltaTime, 0);
        gameObject.transform.position += new Vector3(0, velocY, 0);

    }

    public LayerMask msk;
    public LayerMask yu;

    /**
     * OnDrawGizmos is for the editor only. This can not be seen in game. Shows the Raycast for ground collision.
     */
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + new Vector3(0, (-transform.localScale.y / 2) - 0.2f, 0));
    }

    void OnCollisionEnter2D(Collision2D theCollision)
    {
        // If there is a piece of ground below the player. Replaced 0.5f with transform.position.y /2
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), -gameObject.transform.up, transform.localScale.y/2 + 0.2f, msk);
        // If there is not a piece of ground below the player than do this.
        Debug.Log(theCollision.gameObject.layer);
        if (hit.collider == null && msk == (msk | (1 << theCollision.gameObject.layer)))
        {
            acc = defAcc;
            isJumping = false;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            return;
        }

        RaycastHit2D semisolidHit = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), gameObject.transform.up, 0.5f, yu);
        Debug.Log(LayerMask.GetMask("semiSolid"));
        if(semisolidHit.collider != null)
        {
            semisolidHit.collider.isTrigger = true;
        }

        if (theCollision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
        }
    }

    //consider when character is jumping .. it will exit collision.
    void OnCollisionExit2D(Collision2D theCollision)
    {
        if (theCollision.gameObject.CompareTag("ground") && deltaY != 0f)
        {
            isGrounded = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (yu == (yu | (1 << other.gameObject.layer)))
        {
            other.isTrigger = false;
        }
        if (other.gameObject.CompareTag("ground") && deltaY != 0f)
        {
            isGrounded = false;
        }
    }
}
