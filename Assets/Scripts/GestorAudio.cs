// MINI MENU PAUSA - ROBERTO ADRIAN HERNANDO TAMAYO
// DAM-VI - DESARROLLO DE INTERFACES (2026-2027)
// SCRIPT GESTOR DE AUDIO PARA MINI MENU PAUSA

using UnityEngine;
using UnityEngine.UI;

public class GestorAudio : MonoBehaviour
{
    // Reproductor de la musica de fondo

    public AudioSource reproductorMusica;

    // Reproductor de los efectos de sonido

    public AudioSource reproductorEfectos;

    // Sliders que controlan el volumen

    public Slider sliderVolumenMusica;
    public Slider sliderVolumenEfectos;

    // Array de efectos de sonido: Fa, Rizz y What

    public AudioClip[] efectosSonido;

    // Se ejecuta una vez al iniciar el juego

    void Start()
    {
        // Recuperamos los volumenes guardados. Si todavia no existen, usamos 70 % y 80 %

        float volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 0.7f);
        float volumenEfectos = PlayerPrefs.GetFloat("VolumenEfectos", 0.8f);

        // Aplicamos los volumenes a los reproductores

        reproductorMusica.volume = volumenMusica;
        reproductorEfectos.volume = volumenEfectos;

        // Colocamos los sliders en los valores guardados

        sliderVolumenMusica.SetValueWithoutNotify(volumenMusica);
        sliderVolumenEfectos.SetValueWithoutNotify(volumenEfectos);

        // Vinculamos los eventos de los sliders.Se ejecutaran automaticamente al moverlos

        sliderVolumenMusica.onValueChanged.AddListener(CambiarVolumenMusica);
        sliderVolumenEfectos.onValueChanged.AddListener(CambiarVolumenEfectos);
    }


    // Cambia el volumen de musica en tiempo real.(No se guarda el valor)

    public void CambiarVolumenMusica(float volumen)
    {
        reproductorMusica.volume = volumen;
    }


    // Cambia el volumen de efectos en tiempo real.(No se guarda el valor)

    public void CambiarVolumenEfectos(float volumen)
    {
        reproductorEfectos.volume = volumen;
    }

    // Reproduce uno de los efectos de sonido aleatoriamente. Se ejecuta al pulsar BotonTestEfectos

    public void ReproducirEfectoAleatorio()
    {
        // Comprobamos que haya efectos disponibles

        if (efectosSonido == null || efectosSonido.Length == 0)
        {
            return;
        }

        // Seleccionamos una posicion aleatoria del array

        int indiceAleatorio = Random.Range(0, efectosSonido.Length);

        // Recuperamos el efecto seleccionado

        AudioClip efectoSeleccionado = efectosSonido[indiceAleatorio];

        // Reproducimos el sonido con el volumen actual

        reproductorEfectos.PlayOneShot(efectoSeleccionado);
    }

    // Aplica y guarda los volumenes seleccionados. Se ejecuta cuando pulsamos BotonAplicar

    public void AplicarVolumenes()
    {
        // Obtenemos los valores actuales de los sliders

        float volumenMusica = sliderVolumenMusica.value;
        float volumenEfectos = sliderVolumenEfectos.value;

        // Nos aseguramos de aplicar los valores al audio

        reproductorMusica.volume = volumenMusica;
        reproductorEfectos.volume = volumenEfectos;

        // Guardamos los valores definitivamente

        PlayerPrefs.SetFloat("VolumenMusica", volumenMusica);
        PlayerPrefs.SetFloat("VolumenEfectos", volumenEfectos);
        PlayerPrefs.Save();
    }

    // Restaura los ultimos volumenes guardados. Se utiliza al salir sin aplicar los cambios

    public void RestaurarVolumenes()
    {
        // Recuperamos los valores guardados

        float volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 0.7f);
        float volumenEfectos = PlayerPrefs.GetFloat("VolumenEfectos", 0.8f);

        // Recuperamos los volumenes anteriores

        reproductorMusica.volume = volumenMusica;
        reproductorEfectos.volume = volumenEfectos;

        // Restauramos tambien los sliders sin lanzar eventos

        sliderVolumenMusica.SetValueWithoutNotify(volumenMusica);
        sliderVolumenEfectos.SetValueWithoutNotify(volumenEfectos);
    }

    // Comprueba si hay cambios pendientes de aplicar. Devuelve true si algun volumen difiere del guardado
 
    public bool HayCambiosSinAplicar()
    {
        // Consultamos los ultimos valores guardados

        float volumenMusicaGuardado = PlayerPrefs.GetFloat("VolumenMusica", 0.7f);

        float volumenEfectosGuardado = PlayerPrefs.GetFloat("VolumenEfectos", 0.8f);

        // Comparamos los sliders con los valores guardados

        return !Mathf.Approximately(sliderVolumenMusica.value,volumenMusicaGuardado) || !Mathf.Approximately(sliderVolumenEfectos.value,volumenEfectosGuardado);
    }

    // Recupera los volumenes predeterminados

    public void ValoresPorDefecto()
    {
        // Establecemos los valores predeterminados y permitimos actualizar los valores de los sliders en tiempo real

        sliderVolumenMusica.value = 0.7f;
        sliderVolumenEfectos.value = 0.8f;

        // Los cambios solo se guardaran al pulsar Aplicar
    }
}
