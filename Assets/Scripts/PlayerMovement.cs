using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private GameObject controller;
    public GameObject ballPrefab;
    public float velocity;
    private PlayerInputs controls;
    private Rigidbody2D rigidbody;
    private int playerSize = 3;
    private GameObject ball;
    private int ballCount;
    private bool hasLaser = false;
    private float laserTime;
    public GameObject laserPrefab;
    public SpriteRenderer[] lasers;
    public GameObject[] laserPoints;
    private bool isSecondPlayer = false;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GameObject.FindGameObjectWithTag("IA");
        rigidbody = GetComponent<Rigidbody2D>();
        ball = FindBall();
        ballCount++;

        controls = new PlayerInputs();
        if (!isSecondPlayer)
        {
            controls.Player.Enable();
            controls.Player.Movement.performed += MovePlayer;
            controls.Player.Movement.canceled += MovePlayer;
            controls.Player.ShootLaser.performed += Shoot;
        } else
        {
            controls.Player2.Enable();
            controls.Player2.Movement.performed += MovePlayer;
            controls.Player2.Movement.canceled += MovePlayer;
            controls.Player2.ShootLaser.performed += Shoot;
        }
    }

    private void OnDisable()
    {
        if (isSecondPlayer)
        {
            controls.Player2.Movement.performed -= MovePlayer;
            controls.Player2.Movement.canceled -= MovePlayer;
            controls.Player2.ShootLaser.performed -= Shoot;
            controls.Player2.Disable();
        } else
        {
            controls.Player.Movement.performed -= MovePlayer;
            controls.Player.Movement.canceled -= MovePlayer;
            controls.Player.ShootLaser.performed -= Shoot;
            controls.Player.Disable();
        }
    }

    public void SetSecondPlayer()
    {
        isSecondPlayer = true;
    }

    private void MovePlayer(InputAction.CallbackContext callbackContext)
    {
        rigidbody.velocity = callbackContext.ReadValue<Vector2>() * velocity;
    }

    void Update()
    {
        if (hasLaser)
        {
            if (laserTime > 0)
            {
                laserTime -= Time.deltaTime;
            } else
            {
                hasLaser = false;
                foreach (SpriteRenderer sprite in lasers)
                {
                    sprite.enabled = false;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("ball"))
        {
            audioSource.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
         switch (collision.tag)
        {
            case "shrink":
                if (playerSize > 1)
                {
                    transform.localScale = Vector3.Scale(transform.localScale, new Vector3(0.5F, 1F, 1F));
                    playerSize--;
                }
                break;
            case "expand":
                if (playerSize < 5)
                {
                    transform.localScale = Vector3.Scale(transform.localScale, new Vector3(2, 1, 1));
                    playerSize++;
                }
                break;
            case "ballsbrick":
                SpawnBalls();
                break;
            case "bigball":
                ball.transform.localScale = Vector3.Scale(ball.transform.localScale, new Vector3(2, 2, 1));
                break;
            case "fastbrick":
                Time.timeScale *= 1.25F;
                break;
            case "slowbrick":
                Time.timeScale *= 0.75F;
                break;
            case "laserbrick":
                SetLasers();
                break;
        }
        Destroy(collision.gameObject);
    }

    private void SetLasers()
     {
        if (hasLaser)
        {
            laserTime = 5F;
        } else
        {
            laserTime = 5F;
            hasLaser = true;
            foreach (SpriteRenderer sprite in lasers)
            {
                sprite.enabled = true;
            }
        }
    }

    private void Shoot(InputAction.CallbackContext callbackContext)
    {
        if (hasLaser)
        {
            foreach (GameObject laser in laserPoints)
            {
                GameObject newLaser = Instantiate(laserPrefab, laser.transform.position, Quaternion.identity);
                newLaser.GetComponent<Rigidbody2D>().velocity = Vector2.up * 10;
            }
        }
         
    }

    private void SpawnBalls()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject newBall = Instantiate(ballPrefab, ball.transform.position, Quaternion.identity);
            newBall.GetComponent<BallBehaviour>().SetAlive();
            newBall.transform.position = ball.transform.position;

            float angle = Random.Range(0F, 6.28319F);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            newBall.GetComponent<Rigidbody2D>().velocity = direction * 5;

            ballCount++;
        }
    }

    public void BallDeath(GameObject deadBall, Vector3 scale)
    {
        if (!isSecondPlayer)
        {
            if (ballCount > 1)
            {
                Destroy(deadBall);
                ballCount--;
                ball = FindBall();
            }
            else
            {
                ball = deadBall;
                ball.GetComponent<BallBehaviour>().alive = false;
                ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                ball.transform.localScale = scale;
                controller.GetComponent<GameController>().DeductBall(ball);
            }
        }
    }

    private GameObject FindBall()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, 25F);

        foreach (Collider2D collider in objects)
        {
            if (collider.gameObject.tag == "ball")
            {
                return collider.gameObject;
            }
        }

        return null;
    }
}
