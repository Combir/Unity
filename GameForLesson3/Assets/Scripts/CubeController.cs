using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 300f;
    private Rigidbody2D rb;
    private bool isJumping;
    public Animator animator;
    public float raycastDistance = 0.2f;
    public LayerMask groundLayer; 
    public int collectibleCount = 0;
    public Text collectibleText;

    public Transform groundCheck; 

    public Vector2[] groundCheckOffsets = new Vector2[] { new Vector2(0f, 0f), new Vector2(-0.3f, 0f), new Vector2(0.3f, 0f) };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        UpdateCollectibleUI();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        bool grounded = IsGrounded();

        // Если на земле и пробел, то можно прыгнуть
        if (grounded && Input.GetButtonDown("Jump") && !isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(new Vector2(0, jumpForce));
            isJumping = true;
            animator.SetBool("IsJumping", true); 
        }

        // Если персонаж на земле то сбрасываем состояние прыжка и разрешаем движение
        if (grounded)
        {
            isJumping = false;
            animator.SetBool("IsJumping", false);  // выключаем анимацию прыжка
            animator.SetBool("IsRunning", Mathf.Abs(moveInput) > 0.01f);  // если движется, включаем анимацию бега
        }
        else if (rb.linearVelocity.y < 0) // Если персонаж падает
        {
            animator.SetBool("IsFalling", true);
        }
        else
        {
            animator.SetBool("IsFalling", false);
        }

        // Движение в воздухе возможно но это не прыжок
        if (!isJumping) 
        {
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y); 
        }

        // Переворачиваем персонажа в зависимости от направления движения
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Проверка находимся ли мы на земле
    bool IsGrounded()
    {
        Vector2 basePosition = groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position;
        basePosition.y -= 0.85f;

        foreach (Vector2 offset in groundCheckOffsets)
        {
            Vector2 rayOrigin = basePosition + offset;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, raycastDistance, groundLayer);
            if (hit.collider != null)
            {
                float angle = Vector2.Angle(hit.normal, Vector2.up);
                if (angle < 45f)  
                {
                    return true;
                }
            }
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 basePosition = (groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position);
        basePosition.y -= 0.85f;
        foreach (Vector2 offset in groundCheckOffsets)
        {
            Vector2 rayOrigin = basePosition + offset;
            Gizmos.DrawRay(rayOrigin, Vector2.down * raycastDistance);
        }
    }

    // Обработка подбора предметов
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            collectibleCount++;
            Destroy(other.gameObject);
            UpdateCollectibleUI();
        }
    }

    // Обновление UI для отображения количества собранных предметов
    void UpdateCollectibleUI()
    {
        if (collectibleText != null)
        {
            collectibleText.text = "Count: " + collectibleCount;
        }
    }
}
