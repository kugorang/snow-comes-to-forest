#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class StartScreenSpriteOffsetter : MonoBehaviour
    {
        private Vector3 initialPosition;
        private Vector3 newPosition;

        public float spriteOffset;

        private void Start()
        {
            initialPosition = transform.position;
        }

        private void Update()
        {
            transform.position = new Vector3(initialPosition.x + spriteOffset * Input.mousePosition.x,
                initialPosition.y + spriteOffset * Input.mousePosition.y, initialPosition.z);
        }
    }
}