using System.Collections.Generic;
using UnityEngine;

public class BallController0 : MonoBehaviour
{
    Rigidbody2D rb;
    AudioSource sfx;

    [SerializeField] float force;
    [SerializeField] float delay;

    [SerializeField] AudioClip sfxPaddle;
    [SerializeField] AudioClip sfxBrick;
    [SerializeField] AudioClip sfxWall;

    List<string> bricks = new List<string>
    {
        {"brick-y"},
        {"brick-g"},
        {"brick-a"},
        {"brick-r"}
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sfx = GetComponent<AudioSource>();
        sfx.volume = 0.2f;

        rb = GetComponent<Rigidbody2D>();

        Invoke("LaunchBall", delay);
    }

    private void LaunchBall()
    {
        // reset position and velocity
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;

        // get random direction
        float dirX, dirY = -1;
        dirX = Random.Range(0, 2) == 0 ? -1 : 1;
        Vector2 dir = new Vector2(dirX, dirY);
        dir.Normalize();

        // apply force
        rb.AddForce(dir * force, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        string tag = other.gameObject.tag;

        if (bricks.Contains(tag))
        {
            sfx.clip = sfxBrick;
            sfx.Play();
        }
        else if (tag == "paddle")
        {
            sfx.clip = sfxPaddle;
            sfx.Play();
        }
        else if (tag == "wall-top" || tag == "wall-lateral")
        {
            sfx.clip = sfxWall;
            sfx.Play();
        }
    }
}
