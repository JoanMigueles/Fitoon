using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public int countdownTimer = 3;
    public float countdownSpeed = 1;

    private Animator anim;
    private TextMeshProUGUI textMesh;
    private int currentTime;
    private bool hasFinished = false;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        textMesh = GetComponent<TextMeshProUGUI>();

        Restart();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasFinished) {
            Debug.Log(currentTime);
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("JumpNext"))
            {
                //Update countdown number
                currentTime--;

                //Show number as text
                if (currentTime == 0) textMesh.text = "Go!";
                else textMesh.text = currentTime.ToString();

                //Choose next anim
                if (currentTime >= 0)
                {
                    anim.SetTrigger("PlayNext");
                    anim.SetFloat("CountDownSpeed", countdownSpeed);
                }
                else
                {
                    hasFinished = true;
                    anim.SetTrigger("Reset");
                }

            }

        }

    }

	public void Restart()
    {
        currentTime = countdownTimer;
        textMesh.text = currentTime.ToString();
        hasFinished = false;

        anim.SetFloat("CountDownSpeed", countdownSpeed);
    }

    public void StartCountdown()
    {
		Restart();
		//anim.enabled = true;
		//textMesh.enabled = true;

		anim.SetTrigger("Start");
    }

    public bool HasFinished()
    {
        return hasFinished;
    }

}
