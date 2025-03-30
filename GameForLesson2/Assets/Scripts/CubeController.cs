using UnityEngine;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private float Move;
    public float jump = 300f;
    private Rigidbody2D rb;
    private bool isJumping;
    public Animator animator;
    public float raycastDistance = 0.2f;
    public LayerMask groundLayer; 
    public int collectibleCount = 0;
    public Text collectibleText;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        UpdateCollectibleUI();
    }
    void Update()
    {
        Move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(speed * Move, rb.linearVelocity.y);
        
        bool grounded = IsGrounded();
        
        if (Input.GetButtonDown("Jump") && grounded)
        {
            rb.AddForce(new Vector2(rb.linearVelocity.x, jump));
            isJumping = true;
        }
        
        bool isInAir = !grounded || rb.linearVelocity.y > 0.1f || rb.linearVelocity.y < -0.1f;
        
        if (isInAir || isJumping)
        {
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsRunning", false); 
        }
        else
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsRunning", Move != 0);
        }
        
        if (grounded && isJumping && rb.linearVelocity.y <= 0)
        {
            isJumping = false;
        }
        
        if (Move > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (Move < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    
    bool IsGrounded()
    {
        Vector2 rayStart = new Vector2(transform.position.x, transform.position.y - 0.85f);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, raycastDistance, groundLayer);
        if (hit.collider != null)
        {
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            return angle < 45f; 
        }
        return false;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            collectibleCount++; 
            Destroy(other.gameObject); 
            UpdateCollectibleUI();
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 rayStart = new Vector2(transform.position.x, transform.position.y - 0.85f);
        Gizmos.DrawRay(rayStart, Vector2.down * raycastDistance);
    }
    
    void UpdateCollectibleUI()
    {
        if (collectibleText != null)
        {
            collectibleText.text = "Count: " + collectibleCount;
        }
    }
}
