using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] AudioClip sfxPaddle;
    [SerializeField] AudioClip sfxBrick;
    [SerializeField] AudioClip sfxWall;
    [SerializeField] AudioClip sfxFail;
    [SerializeField] AudioClip sfxNextLevel;

    [Header("Settings")]
    [SerializeField] float force;
    [SerializeField] float forceInc;
    [SerializeField] float delay;
    [SerializeField] float hitOffset;

    Rigidbody2D rb;
    AudioSource sfx;

    int hitCount;
    int brickCount;
    int sceneId;

    GameObject paddle;
    bool halved;

    Dictionary<string, int> bricks = new Dictionary<string, int>
    {
        {"brick-y", 10},
        {"brick-g", 15},
        {"brick-a", 20},
        {"brick-r", 25},
        {"brick-pass", 25}
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneId = SceneManager.GetActiveScene().buildIndex;

        sfx = GetComponent<AudioSource>();
        sfx.volume = 0.2f;

        rb = GetComponent<Rigidbody2D>();

        paddle = GameObject.FindWithTag("paddle");

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

        if (bricks.ContainsKey(tag))
        {
            DestroyBrick(other.gameObject);
        }

        else if (tag == "paddle")
        {
            sfx.clip = sfxPaddle;
            sfx.Play();

            // paddle position
            /*Vector3 paddle = other.gameObject.transform.position;

            // get the contact point
            Vector2 contact = other.GetContact(0).point;

            if ((rb.linearVelocity.x < 0 && contact.x > (paddle.x + hitOffset)) || 
                (rb.linearVelocity.x > 0 && contact.x < (paddle.x - hitOffset)))
            {
                rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
            }*/

            // Permite al jugador tener un mejor control de la bola
            Vector2 paddlePosition = other.transform.position;
            float offset = transform.position.x - paddlePosition.x;
            float width = other.collider.bounds.size.x;
            float normalizedOffset = offset / (width / 2);

            // Calculamos la dirección y evitamos que sea completamente vertical
            float directionX = normalizedOffset;
            float directionY = Mathf.Sqrt(1 - directionX * directionX); // Asegura que la magnitud del vector sea 1

            // Forzamos un mínimo ángulo en X si está demasiado cerca del centro
            if (Mathf.Abs(directionX) < 0.2f)
            {
                directionX = directionX >= 0 ? 0.2f : -0.2f; // Le damos una pequeña inclinación hacia un lado
                directionY = Mathf.Sqrt(1 - directionX * directionX);
            }

            Vector2 direction = new Vector2(directionX, directionY).normalized;

            rb.linearVelocity = direction * rb.linearVelocity.magnitude;

            // increment the hit count
            hitCount++;
            if (hitCount % 4 == 0)
            {
                rb.AddForce(direction * forceInc, ForceMode2D.Impulse);
            }
        }
        else if (tag == "wall-top" || tag == "wall-lateral" || tag == "brick-rock")
        {
            sfx.clip = sfxWall;
            sfx.Play();

            if (!halved && tag == "wall-top")
            {
                // Reduce the size of the paddle
                HalvePaddle(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "wall-bottom")
        {
            sfx.clip = sfxFail;
            sfx.Play();

            GameController.UpdateLifes(-1);

            // Restore the paddle to its original size
            if (halved)
            {
                HalvePaddle(false);
            }

            Invoke("LaunchBall", delay);
        }
        else if (other.tag == "brick-pass")
        {
            DestroyBrick(other.gameObject);
        }
    }

    void DestroyBrick(GameObject obj)
    {
        sfx.clip = sfxBrick;
        sfx.Play();

        // update player's score
        GameController.UpdateScore(bricks[obj.tag]);

        Destroy(obj);

        ++brickCount;
        if (brickCount == GameController.totalBricks[sceneId])
        {
            sfx.clip = sfxNextLevel;
            sfx.Play();

            rb.linearVelocity = Vector2.zero;
            GetComponent<SpriteRenderer>().enabled = false;

            Invoke("NextScene", 3);
        }
    }

    void HalvePaddle(bool halve)
    {
        halved = halve;

        Vector3 scale = paddle.transform.localScale;

        paddle.transform.localScale = halved ?
            new Vector3(scale.x * 0.5f, scale.y, scale.z):
            new Vector3(scale.x * 2f, scale.y, scale.z);
    }

    void NextScene()
    {
        int nextId = sceneId + 1;
        if (nextId == GameController.totalBricks.Count)
            nextId = 0;
        SceneManager.LoadScene(nextId);
    }
}
