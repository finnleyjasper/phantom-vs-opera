using UnityEngine;
using UnityEngine.UI;

public class PlayerBarUI : MonoBehaviour
{
    //DELETE !! 
    //Private Variables 
        private int _maxHealth = 10;
        private int _maxSuccess = 10; //dk if this is 10 - DELETE
        
        [Header("Health Bar Filling")]
        [SerializeField] private Image _healthBarFilling;

        [Header("Success Bar Filling")]
        [SerializeField] private Image _successBarFilling;

    public Player player;

    //Method for Updating Player Health Bar UI 
    public void UpdatePlayerHealthUI()
    {
        float healthFillingAmount = (float)player.HealthBar / _maxHealth; //does this work? why r we dividing, cant I just set current health from Player script - DELETE
        _healthBarFilling.fillAmount = healthFillingAmount; //setting fill amount value to current health 
    }

    //Method for Updating Player Success Bar UI 
    public void UpdatePlayerSuccessUI()
    {
        float successFillingAmount = (float)player.SuccessBar / _maxSuccess; //does this work? why r we dividing, cant I just set current health from Player script - DELETE
        _successBarFilling.fillAmount = successFillingAmount; //setting fill amount value to current success 
    }


    /*

    void Start() //dk if needs this - DELETE
    {
        
    }

    void Update() //dk if needs this - DELETE
    {
        
    }
    */
}
