// MINI MENU PAUSA - ROBERTO ADRIAN HERNANDO TAMAYO
// DAM-VI - DESARROLLO DE INTERFACES (2026-2027)
// SCRIPT GENERADOR DE CAJAS QUE CAEN

using System.Collections;
using UnityEngine;

public class GeneradorCajas : MonoBehaviour
{
    // Prefab de la caja que queremos generar automaticamente
   
    public GameObject cajaPrefab;

    // Tiempo en segundos entre la generacion de cada caja

    public float intervalo = 1.5f;


    // Start se ejecuta una vez al iniciar el objeto

    void Start()
    {
        // Iniciamos la corrutina que generara cajas continuamente

        StartCoroutine(GenerarCajas());
    }


    // Corrutina encargada de generar cajas a intervalos regulares

    IEnumerator GenerarCajas()
    {
        // Bucle infinito: seguira generando cajas mientras el objeto este activo y la corrutina continue ejecutandose

        while (true)
        {
            // Generamos una posicion horizontal aleatoria comprendida entre -6 y 6

            float posicionX = Random.Range(-6f, 6f);

            // Definimos la posicion donde aparecera la nueva caja
            // X: posicion aleatoria
            // Y: altura fija de 4 unidades
            // Z: 0, porque estamos trabajando en 2D

            Vector3 posicion = new Vector3(posicionX, 4f, 0f);

            // Creamos una instancia del prefab Caja
            // Quaternion.identity indica que no tendra rotacion inicial

            GameObject nuevaCaja = Instantiate(cajaPrefab,posicion,Quaternion.identity);

            // Programamos la destruccion de la caja despues de 12 segundos de tiempo de juego -asi no salen demasiadas cajas

            Destroy(nuevaCaja, 12f);

            // Esperamos el intervalo indicado antes de generar la siguiente caja
            // WaitForSeconds utiliza el tiempo del juego
            // Cuando Time.timeScale = 0 se pausa la espera tambien

            yield return new WaitForSeconds(intervalo);
        }
    }
}
