using UnityEngine;

public class ActivadorModo : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo nos interesa si el que entra es el jugador
        if (other.CompareTag("Player"))
        {
            // Llamamos al método estático para cambiar el estado global del juego
            CharacterSwitcher.ActivarModoBulletHellGlobal();

            // Destruimos el punto de cambio para que no se active más de una vez
            Destroy(gameObject);
        }
    }
}