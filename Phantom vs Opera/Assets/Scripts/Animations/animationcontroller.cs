using UnityEngine;

public class animationcontroller : MonoBehaviour
{
    public GameObject earth, room, moon;
    private Vector3 test;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activateanimation(earth, "earth");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activateanimation(room, "room");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            activateanimation(moon, "moon");
        }

    }

    void activateanimation (GameObject obj, string animname)
    {
        obj.GetComponent<Animator>().Play(animname);
    }
}
