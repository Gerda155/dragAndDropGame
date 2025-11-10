using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public CharacterController controller;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("Stamina Settings")]
    public float maxStamina = 5f; // максимум выносливости
    public float staminaDrain = 1f; // за секунду при беге
    public float staminaRegen = 0.5f; // за секунду при ходьбе
    public float lowStaminaThreshold = 1.5f; // когда включается heavy breath

    [HideInInspector] public float currentStamina;

    [Header("Audio Sources")]
    public AudioSource walkSound;
    public AudioSource runSound;
    public AudioSource normalBreath;
    public AudioSource heavyBreath;

    private bool isBreathing = false;
    private float speed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina;
        speed = walkSpeed;
    }

    void Update()
    {
        if (!controller.enabled) return; // защита, если контроллер отключён (GameOver)

        // --- Ввод движения ---
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.right * h + transform.forward * v;

        bool isMoving = move.magnitude > 0;

        // --- Бег ---
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && isMoving;

        if (isRunning)
        {
            speed = runSpeed;
            currentStamina -= staminaDrain * Time.deltaTime;
            if (currentStamina < 0) currentStamina = 0;
        }
        else
        {
            speed = walkSpeed;
            currentStamina += staminaRegen * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
        }

        controller.SimpleMove(move * speed);

        // --- Звуки шагов ---
        HandleStepSounds(isMoving, isRunning);

        // --- Звуки дыхания ---
        HandleBreath();
    }

    void HandleStepSounds(bool isMoving, bool isRunning)
    {
        if (isMoving)
        {
            if (isRunning)
            {
                if (!runSound.isPlaying)
                {
                    runSound.Play();
                    walkSound.Stop();
                }
            }
            else
            {
                if (!walkSound.isPlaying)
                {
                    walkSound.Play();
                    runSound.Stop();
                }
            }
        }
        else
        {
            // если стоим, стоп все шаги
            walkSound.Stop();
            runSound.Stop();
        }
    }

    void HandleBreath()
    {
        if (currentStamina <= lowStaminaThreshold)
        {
            if (!isBreathing)
            {
                heavyBreath.Play();
                normalBreath.Stop();
                isBreathing = true;
            }
        }
        else
        {
            if (isBreathing)
            {
                heavyBreath.Stop();
                normalBreath.Play();
                isBreathing = false;
            }
        }

        // Если движемся и стамина не на минимуме, включаем нормальное дыхание
        if (!isBreathing && normalBreath != null && !normalBreath.isPlaying)
            normalBreath.Play();
    }
}
