using UnityEngine;

public class PaddleController : MonoBehaviour
{
    const float MAX_X = 3.1f;
    const float MIN_X = -3.1f;

    [SerializeField] float speed;

    void Update()
    {
        float x = transform.position.x;

        if (x > MIN_X && Input.GetKey(KeyCode.LeftArrow))
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        else if (x < MAX_X && Input.GetKey(KeyCode.RightArrow))
            transform.Translate(speed * Time.deltaTime, 0, 0);
        
        // control más suave aunque no da una sensación retro
        /*float horizontal = Input.GetAxis("Horizontal");
        float newX = Mathf.Clamp(transform.position.x + horizontal * speed * Time.deltaTime, MIN_X, MAX_X);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);*/
    }
}
