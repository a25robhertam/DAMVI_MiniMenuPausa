// MINI MENU PAUSA - ROBERTO ADRIAN HERNANDO TAMAYO
// DAM-VI - DESARROLLO DE INTERFACES (2026-2027)
// SCRIPT CONFIGURADOR DE RESOLUCIoN PARA MINI MENU PAUSA

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfiguradorResolucion : MonoBehaviour
{
    // Selector de las resoluciones de pantalla disponibles

    public TMP_Dropdown selectorResolucion;

    // Toggle para activar o desactivar pantalla completa

    public Toggle togglePantallaCompleta;

    // Lista de resoluciones disponibles sin duplicados

    private List<Resolution> resoluciones = new List<Resolution>();


    // Se ejecuta una vez al iniciar el juego

    void Start()
    {
        // Cargamos las resoluciones compatibles con el equipo

        CargarResoluciones();

        // Mostramos los ultimos ajustes guardados

        CargarResolucionGuardada();

        // Si existe alguna configuracion de pantalla anterior, solicitamos su aplicacion al comenzar

        if (PlayerPrefs.HasKey("ResolucionAncho") || PlayerPrefs.HasKey("PantallaCompleta"))
        {
            Resolution resolucion = resoluciones[selectorResolucion.value];

            Screen.SetResolution(resolucion.width,resolucion.height,ObtenerModoPantalla(togglePantallaCompleta.isOn));
        }
    }

    // Obtiene las resoluciones admitidas por el equipo

    void CargarResoluciones()
    {
        // Limpiamos las listas anteriores

        resoluciones.Clear();
        selectorResolucion.ClearOptions();

        // Textos que mostraremos en el desplegable

        List<string> opciones = new List<string>();

        // Recorremos las resoluciones del dispositivo

        foreach (Resolution resolucion in Screen.resolutions)
        {
            bool repetida = false;

            // Buscamos si ya existe el mismo ancho y alto (para descartar las opciones de refresco y resoluciones duplicadas)

        for (int i = 0; i < resoluciones.Count && !repetida; i++)
        {
            if (resoluciones[i].width == resolucion.width && resoluciones[i].height == resolucion.height)
            {
                repetida = true;
            }
        }

            // Solo añadimos resoluciones diferentes

            if (!repetida)
            {
                resoluciones.Add(resolucion);

                opciones.Add(resolucion.width + " x " + resolucion.height);
            }
        }

        // Si no encontramos opciones, añadimos una alternativa

        if (resoluciones.Count == 0)
        {
            Resolution actual = Screen.currentResolution;

            resoluciones.Add(actual);

            opciones.Add(actual.width + " x " + actual.height);
        }

        // Mostramos las opciones en el Dropdown

        selectorResolucion.AddOptions(opciones);
    }


    // Recupera los ajustes guardados de resolucion y pantalla y los muestra en el Dropdown y en el Toggle
   
    void CargarResolucionGuardada()
    {
        // Leemos la resolucion guardada. Si no existe, utilizamos el tamaño actual

        int ancho = PlayerPrefs.GetInt("ResolucionAncho", Screen.width);

        int alto = PlayerPrefs.GetInt("ResolucionAlto", Screen.height);

        // indice de la resolucion que vamos a seleccionar

        int indice = 0;

        // Buscamos la resolucion guardada

        bool encontrado = false;

        for (int i = 0; i < resoluciones.Count && !encontrado; i++)
        {
            if (resoluciones[i].width == ancho &&
                resoluciones[i].height == alto)
            {
                indice = i;
                encontrado = true;
            }
        }

        // Actualizamos la seleccion sin generar eventos

        selectorResolucion.SetValueWithoutNotify(indice);
        selectorResolucion.RefreshShownValue();

        // Recuperamos el modo de pantalla anterior - 1 = pantalla completa y 0 = modo ventana

        bool pantallaCompleta = PlayerPrefs.GetInt("PantallaCompleta",Screen.fullScreen ? 1 : 0) == 1;

        // Actualizamos el Toggle sin generar eventos.

        togglePantallaCompleta.SetIsOnWithoutNotify(pantallaCompleta);
    }


    // Convierte el estado del Toggle en un modo de pantalla

    FullScreenMode ObtenerModoPantalla(bool pantallaCompleta)
    {
        // Usamos pantalla completa sin bordes o modo ventana

        if (pantallaCompleta)
        {
            return FullScreenMode.FullScreenWindow;
        }
        else
        {
            return FullScreenMode.Windowed;
        }
    }

    // Aplica y guarda la configuracion seleccionada. Se ejecuta al pulsar BotonAplicar

    public void AplicarResolucion()
    {
        // Recuperamos la opcion seleccionada

        int indice = selectorResolucion.value;

        // Comprobamos que corresponda a una resolucion valida

        if (indice < 0 || indice >= resoluciones.Count)
        {
            return;
        }

        // Recuperamos la resolucion correspondiente

        Resolution seleccionada = resoluciones[indice];

        // Comprobamos si el Toggle esta activado

        bool pantallaCompleta = togglePantallaCompleta.isOn;

        // Determinamos el modo de pantalla

        FullScreenMode modo = ObtenerModoPantalla(pantallaCompleta);

        // Solicitamos el cambio de resolucion y modo

        Screen.SetResolution(seleccionada.width,seleccionada.height,modo);

        // Guardamos el ancho y el alto seleccionados

        PlayerPrefs.SetInt("ResolucionAncho", seleccionada.width);

        PlayerPrefs.SetInt("ResolucionAlto", seleccionada.height);

        // Guardamos el estado de pantalla completa

        PlayerPrefs.SetInt("PantallaCompleta",pantallaCompleta ? 1 : 0);

        // Guardamos definitivamente la configuracion

        PlayerPrefs.Save();

        // Mensaje de diagnostico en la Console de Unity, para ver si cambiamos de resolucion y toggle de pantalla

        Debug.Log ("APLICANDO: " + seleccionada.width + " x " + seleccionada.height + " | Pantalla completa: " + pantallaCompleta );
    }

    // Comprueba si existen cambios pendientes de aplicar

    public bool HayCambiosSinAplicar()
    {
        // Evitamos acceder a una lista todavia vacia

        if (resoluciones.Count == 0)
        {
            return false;
        }

        // Recuperamos la resolucion guardada

        int anchoGuardado = PlayerPrefs.GetInt("ResolucionAncho", Screen.width);

        int altoGuardado = PlayerPrefs.GetInt("ResolucionAlto", Screen.height);

        // Recuperamos el estado guardado del Toggle

        bool pantallaGuardada = PlayerPrefs.GetInt("PantallaCompleta",Screen.fullScreen ? 1 : 0) == 1;

        // Recuperamos la seleccion actual del Dropdown

        Resolution seleccionada = resoluciones[selectorResolucion.value];

        // Devolvemos true si hay alguna diferencia

        return seleccionada.width != anchoGuardado || seleccionada.height != altoGuardado || togglePantallaCompleta.isOn != pantallaGuardada;
    }

    // Descarta los cambios pendientes de pantalla para confirmar la salida sin aplicar

    public void RestaurarResolucion()
    {
        // Recuperamos los valores guardados en los controles

        CargarResolucionGuardada();
    }

    // elecciona los valores predeterminados y se ejecuta desde BotonValoresDefecto

    public void ValoresPorDefecto()
    {
        // Obtenemos la resolucion actual del monitor principal

        int ancho = Screen.currentResolution.width;
        int alto = Screen.currentResolution.height;

        // indice predeterminado por si no encontramos una resolucion coincidente

        int indice = 0;

        // Buscamos una resolucion con el ancho y alto correspondientes al monitor principal

        bool encontrado = false;

        for (int i = 0; i < resoluciones.Count && !encontrado; i++)
        {
            if (resoluciones[i].width == ancho && resoluciones[i].height == alto)
            {
                indice = i;
                encontrado = true;
            }
        }

        // Seleccionamos la resolucion predeterminada

        selectorResolucion.SetValueWithoutNotify(indice);
        selectorResolucion.RefreshShownValue();

        // Activamos pantalla completa como modo predeterminado

        togglePantallaCompleta.SetIsOnWithoutNotify(true);

        // No aplicamos ni guardamos todavia los cambios, se debera pulsar Aplicar para confirmarlos
    }
}
