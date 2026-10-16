// MINI MENU PAUSA - ROBERTO ADRIAN HERNANDO TAMAYO
// DAM-VI - DESARROLLO DE INTERFACES (2026-2027)
// SCRIPT CONTROLADOR DE LA PAUSA PARA MINI MENU PAUSA

using System.Collections;
using UnityEngine;

public class ControladorPausa : MonoBehaviour
{
    // Referencia al CanvasGroup del menu de pausa que controla la transparencia y la interaccion de la interfaz

    public CanvasGroup panelMenuPausa;

    // Ventana que pregunta si queremos salir sin aplicar cambios

    public GameObject panelConfirmacion;

    // Gestor que controla los volumenes de musica y efectos

    public GestorAudio gestorAudio;

    // Gestor que controla las resoluciones de pantalla

    public ConfiguradorResolucion configuradorResolucion;

    // Duracion de la animacion de aparicion y desaparicion

    public float duracionFade = 0.3f;

    // Indica si el juego esta pausado

    private bool juegoPausado = false;

    // Evita abrir o cerrar durante una transicion

    private bool enTransicion = false;

    // Indica si esta abierta la ventana de confirmacion

    private bool confirmacionAbierta = false;

    // Start se ejecuta una vez al iniciar el juego

    void Start()
    {
        // Restauramos el tiempo normal del juego

        Time.timeScale = 1f;

        // Ocultamos el menu al comenzar

        panelMenuPausa.alpha = 0f;
        panelMenuPausa.interactable = false;
        panelMenuPausa.blocksRaycasts = false;

        // La ventana de confirmacion empieza oculta

        panelConfirmacion.SetActive(false);
    }


    // Update se ejecuta una vez por cada fotograma

    void Update()
    {
        // Detectamos si el usuario pulsa ESC

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // No permitimos cambios durante las animaciones

            if (enTransicion)
            {
                return;
            }

            // Si estamos mostrando la confirmacion - ESC equivale a pulsar NO

            if (confirmacionAbierta)
            {
                CancelarSalida();
            }
            else if (juegoPausado)
            {
                // Si el juego esta pausado, intentamos salir

                ReanudarJuego();
            }
            else
            {
                // Si el juego esta activo, abrimos el menu

                PausarJuego();
            }
        }
    }


    // Pausa el juego y abre el menu

    void PausarJuego()
    {
        // Actualizamos el estado del juego

        juegoPausado = true;

        // Detenemos el tiempo y las fisicas 2D

        Time.timeScale = 0f;

        // Mostramos el menu progresivamente

        StartCoroutine(AnimarMenu(true));
    }


    // Intenta cerrar el menu. Se ejecuta mediante ESC o BotonReanudar

    public void ReanudarJuego()
    {
        // Comprobamos que podamos iniciar la salida

        if (!juegoPausado || enTransicion || confirmacionAbierta)
        {
            return;
        }

        // Comprobamos si hay cambios pendientes en el audio o en la resolucion de pantalla

        if (gestorAudio.HayCambiosSinAplicar() || configuradorResolucion.HayCambiosSinAplicar())
        {
            // Mostramos la ventana de confirmacion

            confirmacionAbierta = true;
            panelConfirmacion.SetActive(true);
        }
        else
        {
            // Si no hay cambios, cerramos el menu normalmente

            StartCoroutine(AnimarMenu(false));
        }
    }


    // Confirma que queremos salir sin aplicar los cambios. Se ejecuta desde BotonSI

    public void ConfirmarSalida()
    {
        // Comprobamos que la ventana este abierta

        if (!confirmacionAbierta)
        {
            return;
        }

        // Restauramos los ultimos volumenes guardados

        gestorAudio.RestaurarVolumenes();

        // Restauramos la resolucion anterior del Dropdown

        configuradorResolucion.RestaurarResolucion();

        // Ocultamos la ventana de confirmacion

        confirmacionAbierta = false;
        panelConfirmacion.SetActive(false);

        // Cerramos el menu y reanudamos las fisicas

        StartCoroutine(AnimarMenu(false));
    }


    // Cancela la salida del menu - se ejecuta desde BotonNO

    public void CancelarSalida()
    {
        // Cerramos unicamente la ventana de confirmacion

        confirmacionAbierta = false;
        panelConfirmacion.SetActive(false);

        // Los cambios pendientes permanecen en los controles para que el usuario pueda continuar editandolos
    }

    // Corrutina que controla la animacion del menu
    // mostrar = true: fade in
    // mostrar = false: fade out

    IEnumerator AnimarMenu(bool mostrar)
    {
        // Indicamos que comienza la transicion

        enTransicion = true;

        // Guardamos la transparencia actual

        float alphaInicial = panelMenuPausa.alpha;

        // Seleccionamos la transparencia final

        float alphaFinal = mostrar ? 1f : 0f;

        // Contador para controlar la duracion del fade

        float tiempo = 0f;

        // Desactivamos temporalmente los controles

        panelMenuPausa.interactable = false;

        // Permitimos capturar clics unicamente al abrir

        panelMenuPausa.blocksRaycasts = mostrar;


        // Animamos hasta completar el tiempo establecido

        while (tiempo < duracionFade)
        {
            // Usamos tiempo real para poder animar incluso con Time.timeScale = 0

            tiempo += Time.unscaledDeltaTime;

            // Calculamos el progreso entre 0 y 1

            float progreso = Mathf.Clamp01(
                tiempo / duracionFade
            );

            // Modificamos progresivamente la transparencia

            panelMenuPausa.alpha = Mathf.Lerp(alphaInicial,alphaFinal,progreso);

            // Esperamos al siguiente fotograma

            yield return null;
        }


        // Establecemos la transparencia final exacta

        panelMenuPausa.alpha = alphaFinal;

        // Permitimos interactuar cuando el menu esta visible

        panelMenuPausa.interactable = mostrar;
        panelMenuPausa.blocksRaycasts = mostrar;


        // Si estamos cerrando el menu

        if (!mostrar)
        {
            // Actualizamos el estado

            juegoPausado = false;

            // Reanudamos el tiempo normal del juego

            Time.timeScale = 1f;
        }

        // Indicamos que ha terminado la animacion

        enTransicion = false;
    }

  // Cierra la aplicacion cuando el usuario pulsa el boton SALIR de la pantalla principal

    public void SalirJuego()
    {
        // Cerramos la aplicacion

        Application.Quit();
    }

    // Se ejecuta al desactivar el componente o GameObject

    void OnDisable()
    {
        // Evitamos dejar el tiempo detenido accidentalmente

        Time.timeScale = 1f;
    }
}
