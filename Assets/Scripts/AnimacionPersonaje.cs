using UnityEngine;

public class AnimacionPersonaje : MonoBehaviour
{
    public Animator animController; 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
       if (animController == null)
        {
            animController = GetComponentInChildren<Animator>();
        }
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimationStatesManager(string inputState){
        Debug.Log(inputState);
        if(inputState == "enterKey"){
            animController.SetTrigger("ACargado");
        }

        /*switch (input)
        {
            case "enterKey" :

            animController.SeTrigger("ACargado");
            
            default:
        }*/
    }
}
