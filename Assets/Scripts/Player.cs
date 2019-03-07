using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Config
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(25f, 25f);

    // State
    bool isAlive = true;

    // Cached component references
    private Rigidbody2D myRigidbody;
    private Animator playerAnimator;
    private CapsuleCollider2D playerBodyCollider2D;
    private BoxCollider2D playerFeetCollider2D;
    private float gravityScaleAtStart;

    // Message then methods
    void Start () {
        myRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerBodyCollider2D = GetComponent<CapsuleCollider2D>();
        playerFeetCollider2D = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    void Update () {
        if (!isAlive) {
            return;
        }
        Run();
        ClimbLadder();
        Jump();
        Die();
        FlipSprite();
    }

    private void Run () {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // value is between -1 to +1
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        playerAnimator.SetBool("Running", playerHasHorizontalSpeed);
    }

    private void Jump () {
        if (!playerFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }
        if (CrossPlatformInputManager.GetButtonDown("Jump")) {
            print("Jumping!");
            Vector2 jumpBelocityToAdd = new Vector2(myRigidbody.velocity.x, jumpSpeed);
            myRigidbody.velocity += jumpBelocityToAdd;
        }
    }

    private void ClimbLadder () {
        if (!playerFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ladder"))) {
            myRigidbody.gravityScale = gravityScaleAtStart;
            playerAnimator.SetBool("Climbing", false);
            return;
        }
        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical"); // value is between -1 to +1
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, controlThrow * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        playerAnimator.SetBool("Climbing", playerHasVerticalSpeed);
    }

    private void Die () {
        if (playerBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards"))) {
            isAlive = false;
            playerAnimator.SetTrigger("Dying");
            GetComponent<Rigidbody2D>().velocity = deathKick;
        }
    }

    private void FlipSprite () {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed) {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }
}
