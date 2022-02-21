using UnityEngine;
using System.Collections;

public class bricktrigger : MonoBehaviour
{
    private GameObject IA;
    public GameObject[] powerUps;

    private void Start()
    {
        IA = GameObject.FindGameObjectWithTag("IA");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "laser":
                IA.GetComponent<GameController>().AddScore(5);
                Destroy(collision.gameObject);
                BrickCollision();
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "ball":
                IA.GetComponent<GameController>().AddScore(10);
                BrickCollision();
                break;
        }
    }

    private void BrickCollision()
    {

        int chance = Random.Range(1, 10);
        if (chance > 5)
        {
            switch (Random.Range(1, 10))
            {
                case 1:
                case 2:
                    InstantiatePowerUp(0);
                    break;

                case 3:
                case 4:
                    InstantiatePowerUp(1);
                    break;

                case 5:
                    InstantiatePowerUp(2);
                    break;

                case 6:
                    InstantiatePowerUp(3);
                    break;

                case 7:
                    InstantiatePowerUp(4);
                    break;

                case 8:
                    InstantiatePowerUp(5);
                    break;

                case 9:
                    InstantiatePowerUp(6);
                    break;

                case 10:
                    InstantiatePowerUp(1);
                    break;
            }
        }
        IA.BroadcastMessage("BrickDeath", gameObject);
        IA.BroadcastMessage("PlayBrickSound");
        Destroy(gameObject);
    }

    private void InstantiatePowerUp(int index)
    {
        GameObject powerUp = Instantiate(powerUps[index], transform.position, Quaternion.identity);
        powerUp.GetComponent<Rigidbody2D>().velocity = Vector2.down;
    }
}