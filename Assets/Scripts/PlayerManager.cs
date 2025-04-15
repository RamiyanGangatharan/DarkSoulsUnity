using UnityEngine;

namespace DarkSouls
{
    public class PlayerManager : MonoBehaviour
    {
        PlayerInputHandler inputHandler;
        Animator animator;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            inputHandler = GetComponent<PlayerInputHandler>();
            animator = GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            inputHandler.isInteracting = animator.GetBool("isInteracting");
            inputHandler.rollFlag = false;
        }
    }

}

