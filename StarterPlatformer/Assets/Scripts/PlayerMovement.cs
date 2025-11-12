using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Health and Score")]
    public int hp = 6;
    public int score = 0;

    [Header("Post Damage Invincibility")]
    public bool invincible = false;
    public float invincibilityTime = 1f;
    public float invincibilityFlashDuration = 0.2f;

    [Header("Player Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Checks")]
    public float groundRaycastDistance = 1f;
    public float raycastHorzSpacing = 0.5f;
    bool onGround;
    Vector3 lastSafePos;

    [Header("UI Elements")]
    public Image heart0;
    public Image heart1;
    public Image heart2;
    public TextMeshProUGUI scoreText, gameOverText;
    public Sprite fullHeart, halfHeart, emptyHeart;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lastSafePos = transform.position;
        UpdateUI();
    }

    void Update()
    {
        PerformGroundCheck();

        // Horizontal movement
        float horzInput = Input.GetAxis("Horizontal");
        rb.linearVelocityX = horzInput * moveSpeed;
        anim.SetBool("Walking", horzInput != 0f && onGround);
        if (horzInput > 0f) sr.flipX = false;
        if (horzInput < 0f) sr.flipX = true;

        // Jumping
        // Only jump if on ground, and jump key pressed this frame.
        if(onGround && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetTrigger("Jump");
        }
    }

    void PerformGroundCheck()
    {
        // Create a layer mask, so we only check for hits on the ground layer.
        LayerMask mask = LayerMask.GetMask("Ground");
        // Check if on ground with 3 raycasts, down from the centre, left and right of the player.
        RaycastHit2D hitCentre = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastDistance, mask);
        RaycastHit2D hitLeft   = Physics2D.Raycast(transform.position + Vector3.left*raycastHorzSpacing, Vector2.down, groundRaycastDistance, mask);
        RaycastHit2D hitRight  = Physics2D.Raycast(transform.position + Vector3.right*raycastHorzSpacing, Vector2.down, groundRaycastDistance, mask);
        // We're on ground if any of these raycasts gets a ground hit.
        onGround = hitCentre || hitLeft || hitRight;
        // If the centre raycast hits ground, save the last safe position.
        if(hitCentre) lastSafePos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If we hit spikes, remove 1hp and run the post damage coroutine.
        if(collision.CompareTag("Spikes"))
        {
            TakeDamage(1, true);
        }
        else if(collision.CompareTag("Collectible"))
        {
            CollectibleItem item = collision.GetComponent<CollectibleItem>();
            score += item.scoreValue;
            item.OnCollect();
            UpdateUI();
        }
        else if(collision.CompareTag("KillBox"))
        {
            TakeDamage(1, false);
            transform.position = lastSafePos;
        }
    }

    void TakeDamage(int damageAmount, bool postDamageInvincibility)
    {
        hp -= damageAmount;
        UpdateUI();

        if(hp <= 0)
        {
            StartCoroutine(Death());
            return;
        }

        if(postDamageInvincibility)
        {
            StartCoroutine(PostDamageInvincibility());
        }
    }

    IEnumerator PostDamageInvincibility()
    {
        // Set player to invincible
        invincible = true;
        float startTime = Time.time;
        // Turn on/off sprite renderer at intervals until the invincibility period ends.
        while ((Time.time - startTime) < invincibilityTime)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(invincibilityFlashDuration);
        }
        sr.enabled = true;
        invincible = false;
    }
    IEnumerator Death()
    {
        // Make player invisible, and show game over text.
        sr.enabled = false;
        gameOverText.enabled = true;
        // Wait for 1 second to reload scene.
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateUI()
    {
        // Update the UI to show the hearts based on current health.
        if (hp >= 6) heart2.sprite = fullHeart;
        else if (hp == 5) heart2.sprite = halfHeart;
        else heart2.sprite = emptyHeart;
        if (hp >= 4) heart1.sprite = fullHeart;
        else if (hp == 3) heart1.sprite = halfHeart;
        else heart1.sprite = emptyHeart;
        if (hp >= 2) heart0.sprite = fullHeart;
        else if (hp == 1) heart0.sprite = halfHeart;
        else heart0.sprite = emptyHeart;

        // Show the score text.
        scoreText.text = "Score: " + score.ToString();
    }
}
