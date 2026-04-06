using UnityEngine;

public class AudienceSupport : MonoBehaviour
{
    // Private Variables 
        private float _audienceSupport = 10; //don't know if this should be the value - Delete
        
        [Header("Rate Audience Support Decreases (seconds)")]
        [SerializeField] private float _rateBySecond = 1f;

    // Player Object 
    public Player player; // or should this be connect thru GameObserver ? - Delete

    public void PlayerOnPlatform() // Does it need to be void? Delete
    {
        if (player.IsOnPlatform == true)
        {
            _audienceSupport++;
            _audienceSupport = Mathf.Clamp(_audienceSupport, 0, 10); // Clamp - audienceSupport bar cannot go below 0 or above 10
            Debug.Log("Audience support (+) : " + _audienceSupport); // Added debug - delete later ? - Delete
        }

        else if (player.IsOnPlatform == false) // is this right ?
        {
            _audienceSupport -= Time.deltaTime * _rateBySecond;
            _audienceSupport = Mathf.Clamp(_audienceSupport, 0, 10); // Clamp - audienceSupport bar cannot go below 0 or above 10
            Debug.Log("Audience support (-) : " + _audienceSupport); // Added debug - delete later ? - Delete
        }

        // Need to decrease audience support overtime, when player is not on a platform :
        // - if IsOnPlatform = false
        // - Audience support-- over time

    }

    public void PlayerFall()  // Does it need to be void? Delete
    {
        if (player.FellOnFloor == true)
        {
            _audienceSupport = 0;
            Debug.Log("PlayerFall() active"); // Delete Debug log 
        }
    }

    void Start() //Idk if i need this - Delete
    {
        
    }

    void Update() //Idk if i need this - Delete
    {
        PlayerOnPlatform(); // idk if should keep this here ... ?
        PlayerFall();
    }
}
