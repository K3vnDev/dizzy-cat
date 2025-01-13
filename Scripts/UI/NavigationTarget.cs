using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NavigationTarget : MonoBehaviour
{
    public enum Type { Button, Slider, Toggle }
    public Type type = Type.Button;

    [SerializeField] Graphic[] overrideGraphics;
    public Graphic[] graphs { get; private set; }
    public Neighbours neighbours;

    private void Awake()
    {
        graphs = GetGraphs();
        neighbours = new Neighbours(this);
    }

    public void Trigger(float sliderDirection = 0)
    {
        if (type == Type.Button)
        {
            ButtonsController buttonsController = GetComponent<ButtonsController>();
            buttonsController.OnPointerClick(null);
        }
        else if (type == Type.Toggle)
        {
            Toggle toggle = GetComponent<Toggle>();
            toggle.isOn = !toggle.isOn;
        }
        else if (type == Type.Slider)
        {
            Slider slider = GetComponent<Slider>();
            float newValue = slider.value + sliderDirection;

            if (newValue >= 0 && newValue <= 100)
            {
                slider.onValueChanged.Invoke(newValue);
            }
        }
    }

    Graphic[] GetGraphs()
    {
        if (overrideGraphics.Length > 0)
        {
            return overrideGraphics;
        }

        if (type == Type.Button)
        {
            Graphic[] graphics = GetComponents<Graphic>();
            return graphics;
        }
        else if (type == Type.Slider)
        {
            Slider slider = GetComponent<Slider>();

            return new Graphic[]
            {
                slider.fillRect.GetComponent<Graphic>(),
                slider.handleRect.GetComponent<Graphic>()
            };
        }

        return GetComponentsInChildren<Graphic>();
    }
}
