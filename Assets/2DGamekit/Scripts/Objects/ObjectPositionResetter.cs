#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class ObjectPositionResetter : MonoBehaviour
    {
        public GameObject resettingGameObject;

        public void ResetPosition()
        {
            GameObjectTeleporter.Teleport(resettingGameObject, transform);
        }
    }
}