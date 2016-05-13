using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CompleteProject
{
	public class PlayerHealth : MonoBehaviour
	{
		public int startingHealth = 1000;                            // The amount of health the player starts the game with.
		public int currentHealth;                                   // The current health the player has.
		//public Slider healthSlider;                                 // Reference to the UI's health bar.

		//PlayerMovement playerMovement;                              // Reference to the player's movement.
		//PlayerShooting playerShooting;                              // Reference to the PlayerShooting script.
		bool isDead;                                                // Whether the player is dead.
		bool damaged;                                               // True when the player gets damaged.
		
		
		void Awake ()
		{
			// Setting up the references.
			//playerMovement = GetComponent <PlayerMovement> ();
			//playerShooting = GetComponentInChildren <PlayerShooting> ();
			
			// Set the initial health of the player.
			currentHealth = startingHealth;
		}
		
		
		void Update ()
		{
			// If the player has just been damaged...
			//if(damaged)
			//{
			//	damageImage.color = flashColour;
			//}
			// Otherwise...
			//else
			//{
			//	// ... transition the colour back to clear.
			//	damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
			//}
			
			// Reset the damaged flag.
			damaged = false;
		}
		
		//Take damage only if the bullet is not yours (havent implemented)
		public void TakeDamage (int amount)
		{
			// Set the damaged flag so the screen will flash.
			damaged = true;
			
			// Reduce the current health by the damage amount.
			currentHealth -= amount;
			
			// Set the health bar's value to the current health.
			//healthSlider.value = currentHealth;
			
			// If the player has lost all it's health and the death flag hasn't been set yet...
			if(currentHealth <= 0 && !isDead)
			{
				// ... it should die.
				Death ();
			}
		}
		
		
		void Death ()
		{
			// Set the death flag so this function won't be called again.
			isDead = true;
			
			// Disable gestures, because the player is dead.
			// We can despawn them, or they can be a defective bomb like in mario kart (fly around but cant do anything)
			// If we do the above, we will need to change the character somehow so the other players know
			//??????
			
			// Turn off the movement? and shooting scripts.
			//playerMovement.enabled = false;
			//playerShooting.enabled = false;
		}
	}
}