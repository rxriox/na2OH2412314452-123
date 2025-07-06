using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CharacterSwitcher : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    public CinemachineCamera vcamPlataforma;
    public CinemachineCamera vcamBulletHell;
    public static bool esModoBulletHellGlobal = false;

    [Header("Prefabs de Personajes")]
    public List<GameObject> prefabsPersonajes;

    [Header("Configuración de Recarga")]
    public float tiempoDeRecarga = 20f;
    private float recargaActual = 0f;

    [Header("UI (Opcional)")]
    public Text textoRecarga;

    private int indicePersonajeActivo = 0;
    private GameObject personajeActualInstanciado;
    private ControladorJugador controladorActual;
    
    private ControlesJugador controles;
    private Vector2 vectorDeMovimientoInput;
    private bool teclaSaltoPresionada = false;

    public static void ActivarModoBulletHellGlobal()
    {
        Debug.Log("¡MODO BULLET HELL GLOBAL ACTIVADO!");
        esModoBulletHellGlobal = true;

        CharacterSwitcher switcher = FindFirstObjectByType<CharacterSwitcher>();
        if (switcher != null)
        {
            switcher.vcamPlataforma.Priority = 5;
            switcher.vcamBulletHell.Priority = 10;
        }
    }

    private void Awake()
    {
        controles = new ControlesJugador();

        controles.Plataformas.CambiarPersonaje.performed += _ => IntentarCambiarPersonaje();

        controles.Plataformas.Saltar.started += _ => teclaSaltoPresionada = true;
        controles.Plataformas.Saltar.canceled += _ => teclaSaltoPresionada = false;
        
        controles.Plataformas.Mover.performed += ctx => vectorDeMovimientoInput = ctx.ReadValue<Vector2>();
        controles.Plataformas.Mover.canceled += ctx => vectorDeMovimientoInput = Vector2.zero;
    }

    private void OnEnable()
    {
        controles.Plataformas.Enable();
    }

    private void OnDisable()
    {
        controles.Plataformas.Disable();
    }

    void Start()
    {
        esModoBulletHellGlobal = false;
        vcamPlataforma.Priority = 10;
        vcamBulletHell.Priority = 5;
        
        if (prefabsPersonajes.Count > 0)
        {
            CrearPersonaje(indicePersonajeActivo, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("¡No hay prefabs de personajes asignados en el CharacterSwitcher!");
        }
    }

    void Update()
    {
        if (controladorActual != null)
        {
            controladorActual.SetVectorDeMovimiento(vectorDeMovimientoInput);
        }

        if (teclaSaltoPresionada)
        {
            IntentarSalto();
        }
        
        if (recargaActual > 0)
        {
            recargaActual -= Time.deltaTime;
        }
        ActualizarUI();
    }
    
    private void IntentarSalto()
    {
        if (controladorActual != null)
        {
            controladorActual.RealizarSalto();
        }
    }

    private void IntentarCambiarPersonaje()
    {
        if (recargaActual <= 0 && personajeActualInstanciado != null)
        {
            CambiarPersonaje();
        }
    }
    
    void CambiarPersonaje()
    {
        Vector3 posicionActual = personajeActualInstanciado.transform.position;
        Quaternion rotacionActual = personajeActualInstanciado.transform.rotation;

        Destroy(personajeActualInstanciado);

        indicePersonajeActivo++;
        if (indicePersonajeActivo >= prefabsPersonajes.Count)
        {
            indicePersonajeActivo = 0;
        }

        CrearPersonaje(indicePersonajeActivo, posicionActual, rotacionActual);

        recargaActual = tiempoDeRecarga;
        
        Debug.Log("Cambiado a: " + prefabsPersonajes[indicePersonajeActivo].name);
    }

    void CrearPersonaje(int indice, Vector3 posicion, Quaternion rotacion)
    {
        personajeActualInstanciado = Instantiate(prefabsPersonajes[indice], posicion, rotacion);
        controladorActual = personajeActualInstanciado.GetComponent<ControladorJugador>();

        vcamPlataforma.Follow = personajeActualInstanciado.transform;

        if (controladorActual == null)
        {
            Debug.LogError("El prefab " + personajeActualInstanciado.name + " no tiene el script ControladorJugador.");
        }
    }

    void ActualizarUI()
    {
        if (textoRecarga != null)
        {
            if (recargaActual > 0)
            {
                textoRecarga.text = "Cambio en: " + recargaActual.ToString("F1") + "s";
            }
            else
            {
                textoRecarga.text = "Cambio de personaje: LISTO (C)";
            }
        }
    }
}