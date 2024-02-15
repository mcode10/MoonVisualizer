using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    int scenes;
    public Optimization optimization;

    public enum Optimization 
    { 
        Distance,
        LeastHillClimbing
    }
    public void Start()
    {
        optimization = Optimization.Distance;
    }
    public void LeastHillClimbing() 
    {
            optimization = Optimization.LeastHillClimbing;
            Debug.Log("Least");
        
    }
    public void PlayGame ()
    {
        Debug.Log(optimization);
;       if (optimization == Optimization.Distance) 
        {
            SceneManager.LoadScene(1);
        }
        if (optimization == Optimization.LeastHillClimbing) 
        {
            SceneManager.LoadScene(2);
        }
    }
}
