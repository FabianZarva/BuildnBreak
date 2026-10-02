using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; 
using TMPro;

public class SettingsMenuManager : MonoBehaviour
{
   public TMP_Dropdown graphicsDropdown;
   public Slider musicVol, sfxVol;
   public AudioMixer mainAudioMixer;

   public void ChangeGraphicsQuality()
   {
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
   }

   public void ChangeMusicVolume()
   {
       mainAudioMixer.SetFloat("Music", musicVol.value);
   }

   public void ChangeSfxVolume()
   {
       mainAudioMixer.SetFloat("SFX", sfxVol.value);
   }
}