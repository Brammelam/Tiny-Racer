
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource _honk;
    public Slider _slider;
    public bool _mouseUp = false;

    public void Start()
    {
        if (this.name.ToString() == "effectsVolume")
            _slider.onValueChanged.AddListener(delegate { Honk(); });
    }
    private void Update()
    {
        // Honk when effects-slider is adjusted and released
        if (Input.GetMouseButtonDown(0)) _mouseUp = true;
        else if (Input.GetMouseButtonUp(0)) _mouseUp = false;





    }
    public void SetLevel(float _sliderValue)
    {
        string _volumeParameter = this.name.ToString();
        mixer.SetFloat(_volumeParameter, Mathf.Log10(_sliderValue) * 20);
        PlayerPrefs.SetFloat(_volumeParameter, _sliderValue);
        
    }

    public void Honk()
    {
        if(_mouseUp && !_honk.isPlaying)
        _honk.Play(0);
    }
}
