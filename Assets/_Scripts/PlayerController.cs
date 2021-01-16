using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;

    [SerializeField]
    private float jumpForce = 10f;

    [SerializeField]
    private float gravityMultiplier;

    public bool isOnGround = true;

    private bool _gameOver = false;

    public bool GameOver
    {
        get => _gameOver;
    }

    private const string SPEED_F = "Speed_f";
    private const string JUMP_TRIG = "Jump_trig";
    private const string DEATH_B = "Death_b";
    private const string DEATH_TYPE_INT = "DeathType_int";

    private float speedF = 1.0f;

    private Animator _animator;

    public ParticleSystem explosion;
    public ParticleSystem runningDirt;

    private AudioSource _audioSource;

    public AudioClip crashSound;
    public AudioClip jumpSound;

    [SerializeField, Range(0,1)]
    private float soundVolume = 1;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        Physics.gravity = gravityMultiplier * new Vector3(0, -9.81f, 0);

        _animator = GetComponent<Animator>();
        _animator.SetFloat(SPEED_F, speedF);

        runningDirt.Play();

        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && isOnGround && !_gameOver)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // F=m*a
            isOnGround = false;

            _animator.SetTrigger(JUMP_TRIG);
            runningDirt.Stop();
            _audioSource.PlayOneShot(jumpSound, soundVolume);
        }
        else if (!runningDirt.isPlaying && isOnGround && !_gameOver)
        {
            runningDirt.Play();
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Ground")) {
            isOnGround = true;
        } else if (other.gameObject.CompareTag("Obstacle"))
        {
            _gameOver = true;
            runningDirt.Stop();

            _animator.SetInteger(DEATH_TYPE_INT, Random.Range(1,3));
            _animator.SetBool(DEATH_B, _gameOver);
            explosion.Play();
            _audioSource.PlayOneShot(crashSound, soundVolume);
            Invoke("RestartGame", 1.0f);
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadSceneAsync("Prototype 3", LoadSceneMode.Single);
    }
}
