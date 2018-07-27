#region

using UnityEngine;

#endregion

public class Monster : MonoBehaviour
{
    public bool isright;

    public float leftmax;
    public float rightmax;
    public float speed;

    private SpriteRenderer spriteRenderer;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (transform.position.x < leftmax)
            isright = true;
        else if (transform.position.x > rightmax) isright = false;

        if (isright)
            transform.Translate(Vector2.right * speed * Time.deltaTime, Space.World);

        else if (isright == false) transform.Translate(Vector2.left * speed * Time.deltaTime, Space.World);
    }
}