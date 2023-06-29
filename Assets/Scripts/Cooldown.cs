using UnityEngine;
using UnityEngine.UI;


public class Cooldown : MonoBehaviour
{
    public float totalCD;
    public float currCD;

    public Slider slider;

    public Text text;
    void Start()
    {
        slider.value = CalculateCD();   
    }
    void Update()
    {
        slider.value = CalculateCD();

        if(text != null)
        {
            if (currCD > 0)
                text.text = (int)currCD + "";
            else text.text = "";
        }

    }

    private float CalculateCD()
    {
         if(currCD / totalCD < 0) return 0;
         else if (currCD / totalCD > 1) return 1;
         return (currCD / totalCD);
    }

}
