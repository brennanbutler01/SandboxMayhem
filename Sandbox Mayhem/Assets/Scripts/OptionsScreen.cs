using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class OptionsScreen : MonoBehaviour
{
    public Toggle fullscreenTog, vsyncTog;

    public List<ResItem> resolutions = new List<ResItem>();
    private int selectedResolution;

    public TMP_Text resolutionLabel;

    public AudioMixer mixer;

    public TMP_Text masterLabel, musicLabel, sfxLabel;

    public Slider masterSlider, musicSlider, sfxSlider;

    // Start is called before the first frame update
    void Start()
    {
        //Set full screen option to whatever the user has chosen
        fullscreenTog.isOn = Screen.fullScreen;

        // If the user has vSync off, toggle the option off, else turn it on.
        if(QualitySettings.vSyncCount == 0)
        {
            vsyncTog.isOn = false;
        } else
        {
            vsyncTog.isOn = true;
        }

        bool foundResolution = false;

        // Loop through all available resolutions
        for(int i = 0; i < resolutions.Count; i++)
        {
            // If the resolutions match, update the res label
            if(Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                foundResolution = true;

                selectedResolution = i;

                UpdateResLabel();
            }
        }

        // If the user's resolution isn't in the list, we're going to add it.
        if(!foundResolution)
        {
            // Get the new resolution dimensions
            ResItem newRes = new ResItem();
            newRes.horizontal = Screen.width;

            // Add the resolution to the list
            newRes.vertical = Screen.height;
            resolutions.Add(newRes);

            // Update the selected Resolution to the newly added value in the list
            selectedResolution = resolutions.Count - 1;

            // Update the resolution label.
            UpdateResLabel();
        }

        // Variable for the volume value
        float vol = 0f;

        // Getting the value and sending it out to the newly created vol variable
        mixer.GetFloat("MasterVol", out vol);
        masterSlider.value = vol;

        mixer.GetFloat("MusicVol", out vol);
        musicSlider.value = vol;

        mixer.GetFloat("SFXVol", out vol);
        sfxSlider.value = vol;
    }

    // Select the resolution option to the left
    public void ResLeft()
    {
        selectedResolution--;
        // Stop when we get to the end of the list
        if(selectedResolution < 0)
        {
            selectedResolution = resolutions.Count - 1;
        }

        UpdateResLabel();
    }

    // Select the resolution option to the right
    public void ResRight()
    {
        selectedResolution++;
        // Stop when we get to the end of the list
        if(selectedResolution > resolutions.Count - 1)
        {
            selectedResolution = 0;
        }

        UpdateResLabel();
    }

    public void UpdateResLabel()
    {
        resolutionLabel.text = resolutions[selectedResolution].horizontal.ToString() + " x " + resolutions[selectedResolution].vertical.ToString();
    }

    public void ApplyGraphics()
    {
        // Change the fullScreen option to whatever the toggle is set as
        // Screen.fullScreen = fullscreenTog.isOn;

        // Set the vsync to whatever the vSync is set as
        if(vsyncTog.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, fullscreenTog.isOn);
    }

    public void SetMasterVol()
    {
        // Adding 80 to keep the range between 0 and 100 instead of -80 to 20
        masterLabel.text = Mathf.RoundToInt(masterSlider.value + 80).ToString();

        mixer.SetFloat("MasterVol", masterSlider.value);

        // Save the value to PlayerPrefs
        PlayerPrefs.SetFloat("MasterVol", masterSlider.value);
    }

    public void SetMusicVol()
    {
        // Adding 80 to keep the range between 0 and 100 instead of -80 to 20
        musicLabel.text = Mathf.RoundToInt(musicSlider.value + 80).ToString();

        mixer.SetFloat("MusicVol", musicSlider.value);

        // Save the value to PlayerPrefs
        PlayerPrefs.SetFloat("MusicVol", musicSlider.value);
    }

    public void SetSFXVol()
    {
        // Adding 80 to keep the range between 0 and 100 instead of -80 to 20
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value + 80).ToString();

        mixer.SetFloat("SFXVol", sfxSlider.value);

        // Save the value to PlayerPrefs
        PlayerPrefs.SetFloat("SFXVol", sfxSlider.value);
    }
}

// Make the ResItem class show up in Unity as a serialized dropdown list
[System.Serializable]
public class ResItem
{
    // Variables for resolutions
    public int horizontal, vertical;
}