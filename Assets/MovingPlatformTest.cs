using UnityEngine;
using System.Collections;

public class MovingPlatformTest : MonoBehaviour {
    [SerializeField] float movingSpeedx;
    [SerializeField] float movingSpeedy;
    public GameObject platform; // The actual sprite
    public GameObject start, end;
    private bool orientation; // Where the platform is facing.
    private GameObject player;
    private bool horiz_movement, verti_movement;
    
    // Use this for initialization
    void Start () 
    {
        player = GameObject.Find ("Player"); // Player's character.
        orientation = true;
        
        // Is it horizontal movement?
        if (end.transform.position.y != start.transform.position.y) 
        {
            horiz_movement = false;
        } 
        else 
        {
            horiz_movement = true;
        }
        
        // Is it vertical movement?
        if (end.transform.position.x != start.transform.position.x) 
        {
            verti_movement = false;
        } 
        else 
        {
            verti_movement = true;
        }
        
        // Move the platform at the start point.
        platform.transform.Translate(start.transform.position.x, start.transform.position.y, start.transform.position.z);
    }
  
    void FixedUpdate () 
    {
        if (horiz_movement && !verti_movement) 
        {
            if (orientation) 
            {
                platform.transform.Translate (Vector2.right * movingSpeedx * Time.deltaTime);
                
                if (end.transform.position.x < platform.transform.position.x) 
                {
                    orientation = false;        
                }
            } 
            else 
            {
                platform.transform.Translate (-Vector2.right * movingSpeedx * Time.deltaTime);
                
                if (start.transform.position.x > platform.transform.position.x) 
                {
                    orientation = true;     
                }
            }
        }
        else if (!horiz_movement && verti_movement)
        {
            if (orientation) 
            {
                platform.transform.Translate(-Vector2.up * movingSpeedy * Time.deltaTime);
                
                if (end.transform.position.y > platform.transform.position.y) 
                {
                    orientation = false;        
                }
            } 
            else 
            {
                platform.transform.Translate(Vector2.up * movingSpeedy * Time.deltaTime);
                
                if (start.transform.position.y < platform.transform.position.y) 
                {
                    orientation = true;     
                }
            }
        }
        else if (!horiz_movement && !verti_movement)
        {
            if (start.transform.position.x < end.transform.position.x && start.transform.position.y < end.transform.position.y)
            {
                if (orientation) 
                {
                    platform.transform.Translate(Vector2.up * movingSpeedy * Time.deltaTime);
                    platform.transform.Translate(Vector2.right * movingSpeedx * Time.deltaTime);
                    
                    if (end.transform.position.y < platform.transform.position.y && end.transform.position.x < platform.transform.position.x) 
                    {
                        orientation = false;        
                    }
                } 
                else 
                {
                    platform.transform.Translate(-Vector2.right * movingSpeedx * Time.deltaTime);
                    platform.transform.Translate(-Vector2.up * movingSpeedy * Time.deltaTime);
                    
                    if (start.transform.position.y > platform.transform.position.y && start.transform.position.x > platform.transform.position.x) 
                    {
                        orientation = true;     
                    }
                }
            }
            else
            {
                if (orientation) 
                {
                    platform.transform.Translate(-Vector2.up * movingSpeedy * Time.deltaTime);
                    platform.transform.Translate(Vector2.right * movingSpeedx * Time.deltaTime);
                    
                    if (end.transform.position.y > platform.transform.position.y && end.transform.position.x < platform.transform.position.x) 
                    {
                        orientation = false;        
                    }
                } 
                else 
                {
                    platform.transform.Translate(-Vector2.right * movingSpeedx * Time.deltaTime);
                    platform.transform.Translate(Vector2.up * movingSpeedy * Time.deltaTime);
                    
                    if (start.transform.position.y < platform.transform.position.y && start.transform.position.x > platform.transform.position.x) 
                    {
                        orientation = true;     
                    }
                }
            }
        }
    }
    
    //If character collides with the platform, make it its child.
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player")) 
        {
            MakeChild ();   
        }
    }
        //Once it leaves the platform, become a normal object again.
    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player")) 
        {
            ReleaseChild(); 
        }
    }
    
    void MakeChild()
    {
        player.transform.parent = platform.transform;
    }
    
    void ReleaseChild()
    {
        player.transform.parent = null;
    }   
}