using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class ClickCounterDimensionChecker : MonoBehaviour
{
    public GameObject electricity_particle_prefab;
    public AudioSource click_sound;
    public AudioSource[] dimensionSongs;
    public TextMeshProUGUI[] dimensionUIText;
    public GameObject[] dimensionScenes;
    private int total_clicks = 0;
    public TextMeshProUGUI counterText;
    public Dictionary<string, Vector3> dimensions = new Dictionary<string, Vector3>();
    System.Random rnd = new System.Random(DateTime.Now.Millisecond);
    private int clicks_til_next_dimension = 0;
    private int clicks_til_next_dimension_counter = 0;
    public int clicks_til_powerup = 0;
    public string current_dimension;
    public string next_dimension;
    public TextMeshProUGUI instantShiftText;
    public TextMeshProUGUI winningText;
    public TextMeshProUGUI storyText;
    
    
    // Start is called before the first frame update
    void Start()
    {       
        InitializeDimensions();
        clicks_til_next_dimension_counter = rnd.Next(1, 50);
        clicks_til_next_dimension = clicks_til_next_dimension_counter;
        clicks_til_powerup = ApplyPowerUp();
        current_dimension = "Arden";
        PlayMusic();
        ShowText();
        ShowScene();
        PickNextDimension();
        ShowClickCount();
        ShowStoryText();

    }

    // Update is called once per frame
    void Update()
    {
        //If clicked
        if (Input.GetMouseButtonDown(0))
        {
            //Click was on cube
            if (CubeWasHit())
            {
                PlayClickSound();
                ElectricityEffect();
                TrackClicks();
                DestroyOldElectricityParticles();
            }
        }
    }

    void PlayClickSound()
    {
        click_sound.Play();
    }

    bool CubeWasHit()
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out raycastHit, 100f))
        {
            return (raycastHit.transform != null && raycastHit.transform.gameObject.tag == "GlassCube") ? true : false;
        }
        return false;
    }

    void ElectricityEffect()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        Instantiate(electricity_particle_prefab, worldPosition, Quaternion.identity);
    }

    void TrackClicks()
    {
        total_clicks++;
        ShowClickCount();
        clicks_til_next_dimension_counter--;
        //Debug.Log("Clicks til next dimension is " + clicks_til_next_dimension);
        //Debug.Log("total clicks is " + total_clicks + " powerup number is " + clicks_til_powerup);
        if (clicks_til_next_dimension_counter == 0 || clicks_til_next_dimension_counter == clicks_til_powerup)
        {
            if(clicks_til_next_dimension_counter == clicks_til_powerup)
            {
                ShowPowerupText();
            }
            //Stop current music
            StopMusic();
            //Remove last text
            RemoveText();
            //Remove last scene
            RemoveScene();
            ////Remove powerup text
            //RemovePowerupText();
            //Pick next dimension
            current_dimension = next_dimension;
            //Change music for current dimension
            PlayMusic();
            //Display dimension name text
            ShowText();
            //Display next scene
            ShowScene();
            //Check if won
            CheckIfWon();
            //Get next dimension picked
            PickNextDimension();
            gameObject.GetComponent<MoveInArc>().enable_script = true;
            gameObject.GetComponent<MoveInArc>().Start();
            gameObject.GetComponent<MoveInArc>().Update();
            clicks_til_next_dimension_counter = rnd.Next(1, 50);
            clicks_til_next_dimension = clicks_til_next_dimension_counter;
            total_clicks = 0;
            clicks_til_powerup = ApplyPowerUp();
            ShowClickCount();
        }
       
    }

    void StopMusic()
    {
        for(int i = 0; i < dimensionSongs.Length; i++)
        {
            dimensionSongs[i].Stop();
        }
    }

    void RemoveText()
    {
        for(int i = 0; i < dimensionUIText.Length; i++)
        {
            dimensionUIText[i].gameObject.SetActive(false);
        }
    }

    void RemoveScene()
    {
        for(int i = 0; i < dimensionScenes.Length; i++)
        {
            dimensionScenes[i].SetActive(false);
        }
    }

    void RemovePowerupText()
    {
        instantShiftText.gameObject.SetActive(false);
    }

    void RemoveCheckIfWon()
    {
        winningText.gameObject.SetActive(false);
    }

    void RemoveStoryText()
    {
        storyText.gameObject.SetActive(false);
    }

    void PlayMusic()
    {
        for(int i = 0; i < dimensionSongs.Length; i++)
        {
            if(dimensionSongs[i].name == current_dimension)
            {
                dimensionSongs[i].Play();
            }
        }
    }

    void ShowText()
    {
        for(int i = 0; i < dimensionUIText.Length; i++)
        {
            if(dimensionUIText[i].name == current_dimension)
            {
                
                dimensionUIText[i].gameObject.SetActive(true);
            }
        }
    }

    void ShowStoryText()
    {
        storyText.gameObject.SetActive(true);
        Invoke("RemoveStoryText", 18.0f);
    }

    void ShowScene()
    {
        for(int i = 0; i < dimensionScenes.Length; i++)
        {
            if(dimensionScenes[i].name == current_dimension)
            {
                dimensionScenes[i].SetActive(true);
            }
        }
    }

    void ShowClickCount()
    {
        String powerup_available = "No";
        if(clicks_til_powerup != -1)
        {
            powerup_available = "Yes";
        }
        counterText.text = "Dimensional shift progress " + total_clicks.ToString() + "/" + clicks_til_next_dimension +
         "\nPowerup available? " + powerup_available;
    }

    void ShowPowerupText()
    {
        instantShiftText.gameObject.SetActive(true);
        Invoke("RemovePowerupText", 6.0f);
    }

    void CheckIfWon()
    {
        if(current_dimension == "Avalon")
        {
            winningText.gameObject.SetActive(true);
            Invoke("RemoveCheckIfWon", 12.0f);
        }
    }

    public void InitializeDimensions()
    {
        
        //Initialize number of clicks each dimension takes before shifting
        for (int i = 0; i < 6; i++)
        {
            dimensions["Arden"] = new Vector3(0, 2.98000002f, -10.2299995f); //fictional forest FACE 1. Start of game position
            dimensions["Eldrida"] = new Vector3(-3.68000007f, -0.0799999982f, -5.21999979f); //Beauty and wonder FACE 2
            dimensions["Drakonia"] = new Vector3(-0.0700000003f, 1.36000001f, -2.03999996f); //Dragons and fire FACE 3
            dimensions["Gemcrest"] = new Vector3(4.26000023f, 1.21000004f, -6.88000011f); //Water FACE 4
            dimensions["The Abyss"] = new Vector3(0.560000002f, 5.90999985f, -6.65999985f); //bottomless pit
            dimensions["Avalon"] = new Vector3(0.610000014f, -2.66000009f, -8.64999962f); //Paradise -- this is home
        }

    }

    void PickNextDimension()
    {
        int randomIndex = 0;
        do
        {
           randomIndex = rnd.Next(0, dimensions.Count);
        } while (current_dimension == dimensions.Keys.ElementAt(randomIndex));
        next_dimension = dimensions.Keys.ElementAt(randomIndex);
    }

    void DestroyOldElectricityParticles()
    {
        GameObject[] oldElectricityParticles = GameObject.FindGameObjectsWithTag("Electricity");
        //if (oldElectricityParticles.Length > 5)
        //{
        //    foreach(GameObject go in )
        //}
        foreach (GameObject go in oldElectricityParticles)
        {
            GameObject.Destroy(go, 2f);
        }
    }

    int ApplyPowerUp()
    {
        int powerup_applied = rnd.Next(0, 2);
        int powerup_number = -1;
        if(powerup_applied == 1 && clicks_til_next_dimension_counter != 1)
        {
            powerup_number = rnd.Next(1, clicks_til_next_dimension_counter-1);
        }
        return powerup_number;
    }
}

