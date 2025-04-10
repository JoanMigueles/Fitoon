using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System;

public class FaceTrackingToMovement : MonoBehaviour
{
    ARFaceManager faceManager;
    ARFace face;
	GameManager gameManager;

    float avgVelocityAcumulator;
    uint numberOfFrames;
	DateTime startTime;

	/// <summary>
	/// The rotation of the face in the Y axis, multiplied by the rotation intensity.
	/// </summary>
	public Quaternion faceRotation
    {
        get
        {
            if (face == null)
                return Quaternion.identity;

            float angle = face.transform.eulerAngles.y > 180 ? face.transform.eulerAngles.y - 360 : face.transform.eulerAngles.y;
            angle *= rotationIntensity;
            angle = Mathf.Clamp(angle, -90f, 90f);
            return Quaternion.Euler(0, angle, 0);
		}
    }
    public float speed { get; private set; }
	
    [Header("Rotation")]
	[SerializeField] int rotationIntensity = 2;
	[Header("Velocity")]
	[SerializeField] float sampleRate = 128; //frames para calcular la velocidad
    [SerializeField] float distanciaPaso = 0.67f;
    [SerializeField] DominantFrequencyCounter frequencyCounter;


	public bool detectado = false;
	private FadeCamera fadeCamera;

	List<float> data;

    

    //EVENTOS (Para el movimiento del personaje)
    public delegate void OnCaraDetectada();
    public static event OnCaraDetectada onCaraDetectadaEvent;

    public delegate void OnCaraNoDetectada();
    public static event OnCaraNoDetectada onCaraNoDetectadaEvent;


    private void OnEnable()
    {
        gameManager = FindFirstObjectByType<GameManager>();
		faceManager = FindFirstObjectByType<ARFaceManager>();
        fadeCamera = FindFirstObjectByType<FadeCamera>();


		faceManager.facesChanged += CaraDetectada;
        if(fadeCamera != null) fadeCamera.StartFade(true);
        else Debug.LogWarning("No se ha encontrado el FadeCamera");

		data = new List<float>();

		startTime = DateTime.Now;
	}

    private void OnDisable()
    {
        if(faceManager != null)
			faceManager.facesChanged -= CaraDetectada;
    }

    void Update()
	{
		avgVelocityAcumulator += speed;
		numberOfFrames++;
		if (!detectado)
        {
            return;
        }
		CalculateVelocity(face.transform);
		gameManager.velocityText.text = $"Velocity: {Math.Round(speed, 2, MidpointRounding.AwayFromZero)} ({Math.Round(speed * 3.6, 2, MidpointRounding.AwayFromZero)} km/h)";
    }

    private void CalculateVelocity(Transform faceData)
    {
        //Cuando pasen X frames, mandar esa lista a la FFT y sacar la frecuencia
        if (data.Count == sampleRate)
        {
			float frecuencia = frequencyCounter.DoFFT(data.ToArray());
			Debug.Log($"Frecuencia: {frecuencia} Hz.");
			//Calcular la velocidad. Pasos/segundo -> Metros/segundo
			speed = distanciaPaso * frecuencia;
            Debug.Log($"Velocidad media: {speed} m/s.");

            float cadencia = frecuencia * 60f;

			gameManager.cadenceText.text = $"Cadence: {cadencia}";

            //Reiniciar lista
            data.Clear();
        }
        else
        {
			//Guardar en una lista el input del face.transform.y
			data.Add(faceData.position.y);
			
		}

    }

    public float GetTotalDistance()
    {
        float avgSpeed = avgVelocityAcumulator / numberOfFrames;
        float secondsElapsed = (float)(DateTime.Now - startTime).TotalSeconds;
		float distance = avgSpeed * secondsElapsed;
        Debug.Log("Frames transcurridos: " + numberOfFrames);
		Debug.Log($"Velocidad media: {avgSpeed} m/s.");
		Debug.Log($"Tiempo transcurrido: {secondsElapsed} s.");
		Debug.Log($"Distancia recorrida: {distance} m.");
		return distance;
	}

    //-----------EVENTS---------
    void CaraDetectada(ARFacesChangedEventArgs aRFacesChangedEventArgs)
    {
		//Si existe una cara en "updated" (lista de caras detectadas)
		if (aRFacesChangedEventArgs.updated != null && aRFacesChangedEventArgs.updated.Count > 0 && !detectado)
        {
            //Guardar el objeto de la cara
            face = aRFacesChangedEventArgs.updated[0];
            detectado = true;
            Debug.Log("CARA DETECTADA!");

            if (onCaraDetectadaEvent != null)
            {
                onCaraDetectadaEvent();
            }
            if (fadeCamera != null) fadeCamera.StartFade(false);
        }

        if (aRFacesChangedEventArgs.removed.Count > 0)
        {
            Debug.Log("Cara quitada");
            detectado = false;

            if(onCaraNoDetectadaEvent != null)
            {
                onCaraNoDetectadaEvent();
            }
            if (fadeCamera != null) fadeCamera.StartFade(true);
        }
    }  
}
