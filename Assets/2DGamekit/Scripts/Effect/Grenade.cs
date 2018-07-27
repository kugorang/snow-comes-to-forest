#region

using System.Collections;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class Grenade : MonoBehaviour
    {
        public GameObject explosion;
        public float explosionTimer = 3;
        public Vector2 initialForce;

        private Rigidbody2D Rigidbody;
        public float timer = 1;

        private void OnEnable()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        private IEnumerator Start()
        {
            Rigidbody.AddForce(initialForce);
            yield return new WaitForSeconds(timer);
            var eGo = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(eGo, explosionTimer);
            Destroy(gameObject);
        }
    }
}