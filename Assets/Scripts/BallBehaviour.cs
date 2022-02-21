using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallBehaviour : MonoBehaviour
{
    private PlayerInputs controls;
    private GameObject player;
    private Rigidbody2D rigidbody;
    public bool alive;
    private Vector3 scale;

    private void Awake()
    {
        player = FindPlayer();
        scale = transform.localScale;
        controls = new PlayerInputs();
        controls.Player.Enable();
        controls.Player.StartRound.performed += LaunchBall;
        rigidbody = GetComponent<Rigidbody2D>();
        alive = false;
    }

    private void OnDestroy()
    {
        controls.Player.StartRound.performed -= LaunchBall;
        controls.Player.Disable();
    }

    void Update()
    {
        if (!alive)
        {
            transform.position = player.transform.position + new Vector3(0F, 0.5F, 0F);
        } else {
            if (rigidbody.velocity.magnitude > new Vector2(10, 10).magnitude)
                rigidbody.velocity = Vector2.Scale(rigidbody.velocity, new Vector2(0.7F, 0.7F));
            if (rigidbody.velocity.magnitude < new Vector2(3, 3).magnitude)
                rigidbody.velocity = Vector2.Scale(rigidbody.velocity, new Vector2(1.3F, 1.3F));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.tag)
        {
            case "death":
                player.GetComponent<PlayerMovement>().BallDeath(gameObject, scale);
                break;
        }
    }

    private void LaunchBall(InputAction.CallbackContext callbackContext)
    {
        if (!alive)
        {
            alive = true;
            GetComponent<Rigidbody2D>().velocity = Vector2.up * 10; 
        }
    }

    public void SetAlive()
    {
        alive = true;
    }

    private GameObject FindPlayer()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, 25F);

        foreach (Collider2D collider in objects)
        {
            if (collider.gameObject.tag == "Player")
            {
                return collider.gameObject;
            }
        }

        return null;
    }
}
