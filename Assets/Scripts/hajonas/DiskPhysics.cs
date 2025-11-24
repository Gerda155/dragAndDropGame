using UnityEngine;

public class DiskPhysics : MonoBehaviour
{
    public int size;
    [HideInInspector] public PegPhysics currentPeg;
    [HideInInspector] public Rigidbody2D rb;

    private bool isDragging = false;
    private Vector3 mouseWorldPos;

    public float dragSpeed = 10f; // сила для перетаскивания

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1;
    }

    void Update()
    {
        if (isDragging)
        {
            // Получаем позицию мыши или тач на экране
            Vector3 pos;
            if (Input.mousePresent)
                pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            else if (Input.touchCount > 0)
                pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            else
                return;

            pos.z = 0;
            mouseWorldPos = pos;

            // Двигаем Rigidbody к позиции мыши плавно через силу
            Vector2 direction = (mouseWorldPos - transform.position);
            rb.linearVelocity = direction * dragSpeed;
        }
    }

    void OnMouseDown()
    {
        Debug.Log("CLICKED ON DISK!!!!");
        if (currentPeg == null) return;
        if (currentPeg.disks.Peek() != this) return; // только верхний диск

        isDragging = true;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;
        rb.gravityScale = 1;

        // Проверяем, можно ли поставить диск на какую-то башню
        foreach (var peg in FindObjectsOfType<PegPhysics>())
        {
            if (peg.CanPlaceDisk(this))
            {
                if (currentPeg != null)
                    currentPeg.TakeDisk();
                peg.PlaceDisk(this);

                // фиксируем диск на башне
                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = 0;
                return;
            }
        }
    }
}
