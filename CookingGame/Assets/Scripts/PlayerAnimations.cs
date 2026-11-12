using UnityEngine;
using Unity.Netcode;
public class PlayerAnimations : NetworkBehaviour
{
    [SerializeField] private Player player;

    private const string IS_WALKING = "IsWalking";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        if(!IsOwner) return;

        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}