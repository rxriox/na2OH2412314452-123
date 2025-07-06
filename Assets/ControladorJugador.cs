using UnityEngine;

public class ControladorJugador : MonoBehaviour
{
    [Header("Configuración General")]
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 10f;

    [Header("Configuración de Salto Repetido")]
    public float cadenciaDeSalto = 0.4f;
    private float proximoSaltoPermitido = 0f;

    [Header("Configuración de GroundCheck")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private bool estaEnSuelo = false;
    private Rigidbody2D rb;
    private Vector2 vectorDeMovimientoActual;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Se usa EXCLUSIVAMENTE el GroundCheck para saber si estamos en el suelo.
        estaEnSuelo = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (CharacterSwitcher.esModoBulletHellGlobal)
        {
            if (rb.gravityScale != 0) {
                rb.gravityScale = 0;
            }
            ControlModoBulletHell();
        }
        else
        {
            if (rb.gravityScale != 1) {
                rb.gravityScale = 1;
            }
            ControlModoPlataforma();
        }
    }

    public void SetVectorDeMovimiento(Vector2 nuevoVector)
    {
        vectorDeMovimientoActual = nuevoVector;
    }

    public void RealizarSalto()
    {
        if (!CharacterSwitcher.esModoBulletHellGlobal && estaEnSuelo && Time.time > proximoSaltoPermitido)
        {
            proximoSaltoPermitido = Time.time + cadenciaDeSalto;
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
        }
    }

    private void ControlModoPlataforma()
    {
        rb.linearVelocity = new Vector2(vectorDeMovimientoActual.x * velocidadMovimiento, rb.linearVelocity.y);
    }

    private void ControlModoBulletHell()
    {
        rb.linearVelocity = vectorDeMovimientoActual.normalized * velocidadMovimiento;
    }
}