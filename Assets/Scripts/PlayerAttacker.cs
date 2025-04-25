using DarkSouls;
using UnityEngine;

/// <summary>
/// Handles player attack animations for light and heavy attacks.
/// This class interacts with the <see cref="AnimatorHandler"/> to trigger appropriate animations for the player's weapon.
/// </summary>
public class PlayerAttacker : MonoBehaviour
{
    /// <summary>
    /// Reference to the <see cref="AnimatorHandler"/> component used for playing animations.
    /// </summary>
    [SerializeField] private AnimatorHandler animatorHandler;

    /// <summary>
    /// Initializes references for the <see cref="AnimatorHandler"/> component.
    /// If <see cref="animatorHandler"/> is not manually set, it attempts to find it in the child objects.
    /// </summary>
    private void Awake()
    {
        if (animatorHandler == null)
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>(); // fallback
        }
    }

    /// <summary>
    /// Handles the light attack animation by calling the corresponding animation from the weapon's light attack 1 animation string.
    /// </summary>
    /// <param name="weapon">The <see cref="WeaponItem"/> representing the weapon being used for the light attack.</param>
    /// <remarks>
    /// If either the <see cref="animatorHandler"/> or <paramref name="weapon"/> is <c>null</c>, an error message is logged.
    /// </remarks>
    public void HandleLightAttack(WeaponItem weapon)
    {
        if (animatorHandler == null || weapon == null)
        {
            Debug.LogError("Missing reference in HandleLightAttack.");
            return;
        }
        animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
    }

    /// <summary>
    /// Handles the heavy attack animation by calling the corresponding animation from the weapon's heavy attack 1 animation string.
    /// </summary>
    /// <param name="weapon">The <see cref="WeaponItem"/> representing the weapon being used for the heavy attack.</param>
    /// <remarks>
    /// If either the <see cref="animatorHandler"/> or <paramref name="weapon"/> is <c>null</c>, an error message is logged.
    /// </remarks>
    public void HandleHeavyAttack(WeaponItem weapon)
    {
        if (animatorHandler == null || weapon == null)
        {
            Debug.LogError("Missing reference in HandleHeavyAttack.");
            return;
        }
        animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
    }
}
