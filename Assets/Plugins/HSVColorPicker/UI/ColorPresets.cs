﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HSVPicker
{
    public class ColorPresets : MonoBehaviour
    {
	    public ColorPicker picker;
	    public GameObject[] presets;
	    public Image createPresetImage;

        private ColorPresetList _colors;

	    void Awake()
	    {
    //		picker.onHSVChanged.AddListener(HSVChanged);
		    picker.onValueChanged.AddListener(ColorChanged);
	    }

        void Start()
        {
            GenerateDefaultPresetColours();
        }

        void OnEnable()
        {
            // if the picker is set to regenerate its settings on open, then
            // regenerate the default picker options.
            if (picker.Setup.RegenerateOnOpen)
            {
                GenerateDefaultPresetColours();
            }
        }

        private void GenerateDefaultPresetColours()
        {
            List <Color> empty = new List<Color>();
            _colors = ColorPresetManager.Get(picker.Setup.PresetColorsId);
            _colors.UpdateList(empty);

            if (_colors.Colors.Count < picker.Setup.DefaultPresetColors.Length)
            {
                _colors.UpdateList(picker.Setup.DefaultPresetColors);
            }

            _colors.OnColorsUpdated += OnColorsUpdate;
            OnColorsUpdate(_colors.Colors);
        }

        private void OnColorsUpdate(List<Color> colors)
        {

            colors = colors.Distinct().ToList();

            for (int cnt = 0; cnt < presets.Length; cnt++)
            {
                if (colors.Count <= cnt)
                {
                    presets[cnt].SetActive(false);
                    continue;
                }


                presets[cnt].SetActive(true);
                presets[cnt].GetComponent<Image>().color = colors[cnt];
            
            }

            createPresetImage.gameObject.SetActive((colors.Count < presets.Length) && picker.Setup.UserCanAddPresets);

        }

        public void CreatePresetButton()
	    {
            _colors.AddColor(picker.CurrentColor);

      //      for (var i = 0; i < presets.Length; i++)
		    //{
		    //	if (!presets[i].activeSelf)
		    //	{
		    //		presets[i].SetActive(true);
		    //		presets[i].GetComponent<Image>().color = picker.CurrentColor;
		    //		break;
		    //	}
		    //}
	    }

	    public void PresetSelect(Image sender)
	    {
		    picker.CurrentColor = sender.color;
	    }

	    // Not working, it seems ConvertHsvToRgb() is broken. It doesn't work when fed
	    // input h, s, v as shown below.
    //	private void HSVChanged(float h, float s, float v)
    //	{
    //		createPresetImage.color = HSVUtil.ConvertHsvToRgb(h, s, v, 1);
    //	}
	    private void ColorChanged(Color color)
	    {
		    createPresetImage.color = color;
	    }

        private void OnDestroy()
        {
            picker.onValueChanged.RemoveListener(ColorChanged);
            _colors.OnColorsUpdated -= OnColorsUpdate;
        }
    }
}