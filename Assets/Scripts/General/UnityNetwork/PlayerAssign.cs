using UnityEngine;

namespace General.UnityNetwork
{
    public class PlayerAssign : MonoBehaviour
    {
        [SerializeField] private GameObject playerEq;

        private void Start() {
            Instantiate(playerEq);
            DontDestroyOnLoad(playerEq);
        }
    }
}
