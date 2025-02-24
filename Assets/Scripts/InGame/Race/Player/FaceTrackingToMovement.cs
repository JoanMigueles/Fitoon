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
	[SerializeField] TextMeshProUGUI velocityText;
	[SerializeField] TextMeshProUGUI cadenceText;
	[SerializeField] TextMeshProUGUI animCadenceText;
	[SerializeField] float sampleRate = 128; //frames para calcular la velocidad
    [SerializeField] float distanciaPaso = 0.67f;


	public bool detectado = false;
	private FadeCamera fadeCamera;

	List<float> data;

	DominantFrequencyCounter frequencyCounter;
    

    //EVENTOS (Para el movimiento del personaje)
    public delegate void OnCaraDetectada();
    public static event OnCaraDetectada onCaraDetectadaEvent;

    public delegate void OnCaraNoDetectada();
    public static event OnCaraNoDetectada onCaraNoDetectadaEvent;


    private void OnEnable()
    {
        faceManager = FindFirstObjectByType<ARFaceManager>();
        fadeCamera = FindFirstObjectByType<FadeCamera>();
        frequencyCounter = FindFirstObjectByType<DominantFrequencyCounter>();

        faceManager.facesChanged += CaraDetectada;
        if(fadeCamera != null) fadeCamera.StartFade(true);

        data = new List<float>();

    }

    private void OnDisable()
    {
        faceManager.facesChanged -= CaraDetectada;
    }

    void Update()
    {
        if (!detectado)
        {
            return;
        }
		CalculateVelocity(face.transform);
		velocityText.text = $"Velocity: {Math.Round(speed, 2, MidpointRounding.AwayFromZero)} ({Math.Round(speed * 3.6, 2, MidpointRounding.AwayFromZero)} km/h)";
    }

    private void CalculateVelocity(Transform faceData)
    {
        //Cuando pasen X frames, mandar esa lista a la FFT y sacar la frecuencia
        if (data.Count == sampleRate)
        {
            float frecuencia = frequencyCounter.DoFFT(data.ToArray());
            //Calcular la velocidad. Pasos/segundo -> Metros/segundo
            speed = distanciaPaso * frecuencia;
            Debug.Log($"Velocidad media: {speed} m/s.");

            float cadencia = frecuencia * 60f;

            cadenceText.text = $"Cadence: {cadencia}";

            //Reiniciar lista
            data.Clear();
        }
        else
        {
            //Guardar en una lista el input del face.transform.y
            data.Add(faceData.position.y);
        }

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
