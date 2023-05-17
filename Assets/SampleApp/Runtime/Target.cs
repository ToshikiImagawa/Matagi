using UnityEngine;

namespace SampleApp
{
    [DisallowMultipleComponent]
    public class Target : MonoBehaviour
    {
        [SerializeField] private string id;

        public string GetId()
        {
            return id;
        }
    }
}