using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    public Sprite[] tileSpriteArray;
    private bool mouseDown;
    private Rigidbody2D body;
    private int letterNumber;
    public char letter;

    private bool wasMoving;
    private Vector2 pointerPosition;
    //private float fallspeed = 1;

    public void Start(){
        body = GetComponent<Rigidbody2D>();
        letterNumber = GetRandomLetterNumber();//(int) Random.Range(0,26);
        (GetComponent<SpriteRenderer>()).sprite = tileSpriteArray[letterNumber];
        letter = (char) (letterNumber + 96); //96 is ` (grave accent), 97 is a 

        // InvokeRepeating("FallDown", 2.0f, 1.0f);
    }

    // public void FallDown()
    // {
    //     Vector2 newPosition = transform.position;
    //     newPosition.y += -1;
    //     transform.position = newPosition;
    // }

    private int GetRandomLetterNumber()
    {
        // or create an array and choose at random from array
        float randomNumber = Random.Range(0f,1f);
        if (randomNumber < 0.014f) return 10; //j
        if (randomNumber < 0.028f) return 11; //k
        if (randomNumber < 0.042f) return 17; //q
        if (randomNumber < 0.056f) return 24; //x
        if (randomNumber < 0.070f) return 26; //z
        if (randomNumber < 0.091f) return 2; //b
        if (randomNumber < 0.112f) return 3; //c
        if (randomNumber < 0.133f) return 6; //f
        if (randomNumber < 0.154f) return 8; //h
        if (randomNumber < 0.175f) return 13; //m
        if (randomNumber < 0.196f) return 16; //p
        if (randomNumber < 0.217f) return 22; //v
        if (randomNumber < 0.238f) return 23; //w
        if (randomNumber < 0.259f) return 25; //y
        if (randomNumber < 0.287f) return 7; //g
        if (randomNumber < 0.322f) return 12; //l
        if (randomNumber < 0.364f) return 4; //d
        if (randomNumber < 0.406f) return 19; //s
        if (randomNumber < 0.448f) return 21; //u
        if (randomNumber < 0.504f) return 14; //n
        if (randomNumber < 0.567f) return 20; //t
        if (randomNumber < 0.630f) return 18; //r
        if (randomNumber < 0.705f) return 15; //o
        if (randomNumber < 0.787f) return 9; //i
        if (randomNumber < 0.876f) return 1; //a
        return 5; // else (randomNumber <= 1f) //e
    }

    public void OnCollisionEnter2D(Collision2D other) {
        //Debug.Log("This layer: " + this.gameObject.layer + ", Other layer: " + other.gameObject.layer);
        if (/*other.gameObject.layer == 10 && */other.GetContact(0).normal == Vector2.up) // Tile layer
        {
            FixedJoint2D fj = this.gameObject.AddComponent<FixedJoint2D>();
            fj.connectedBody = other.rigidbody;
        }
    }

    public void OnMouseDown() {
        mouseDown = true;
        //Debug.Log("MOUSE DOWN");
    }

    public void OnMouseUp() {
        mouseDown = false;
    }

    public void FixedUpdate() 
    {
        // physics stuff in here
        Vector2 newPosition = body.position;
        Vector2 deltaPosition = Vector2.zero;
        if (mouseDown)
        {
            //body.isKinematic = true;
            body.velocity = Vector2.zero;

            if (pointerPosition.x > body.position.x + 0.5f && !(pointerPosition.x > body.position.x + 1f))
            {
                // prepare to move in x axis by 1
                deltaPosition.x = 1;
            }
            else if (pointerPosition.x < body.position.x - 0.5f && !(pointerPosition.x > body.position.x - 1f))
            {
                deltaPosition.x = -1;
            }
            deltaPosition.y = pointerPosition.y - body.position.y;
            newPosition = newPosition + deltaPosition;
            
            body.MovePosition(newPosition);
        }

        // never have x movement
        Vector2 v = body.velocity;
        v.x = 0;
        body.velocity = v;
            // snap to x grid old code
            //v.x = 0;
            //body.velocity = v;
            //deltaPosition = new Vector3((float) Mathf.Round(body.position.x), body.position.y, transform.position.z);
            //transform.position = deltaPosition;
            // should never be able to move in the x position unless moved by mouse
            //Vector2 v = body.velocity;
            //v.x = 0;
            //body.velocity = v;
        
        // snap to grid
        //deltaPosition.x = Mathf.Round(body.position.x) - body.position.x;
        //body.AddForce(deltaPosition * 1000, ForceMode2D.Force); // 2D only has Force (continuous), and Impulse (like for a jump)
        //body.AddForce(Vector2.left * 100, ForceMode2D.Force);
        //Debug.Log("Velocity = (" + body.velocity.x + ", " + body.velocity.y + ")");
      
    }

    public void Update() {
        // input and game stuff in here
        //Vector2 deltaPosition;
        if(mouseDown) 
        {
            
            //body.isKinematic = true;
            //body.useFullKinematicContacts = true;
            /* original 1
            deltaPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            //transform.Translate(deltaPosition);
            body.velocity = deltaPosition * 10;
            */
            pointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            //body.isKinematic = false;
        }

        // snap x
        Vector2 newPosition = transform.position;
        float roundedX = Mathf.Round(newPosition.x);
        if (newPosition.x > roundedX + 0.01f || newPosition.x < roundedX - 0.01f)
        //if (roundedX != newPosition.x) 
        {
            Debug.Log(newPosition.x + " != " + roundedX);
            newPosition.x = roundedX;

            //body.velocity = Vector2.zero;
            transform.position = newPosition;
            Debug.Log("New postion: " + transform.position.x + ", " + transform.position.y);
        }

        /* original 1
        //else if (body.velocity.y <= 0 && Mathf.Abs(body.velocity.y) >= Mathf.Abs(body.velocity.x))
        if (Mathf.Abs(body.velocity.x) < 2.5)// && body.velocity.y <= 0)
        {
            Vector3 v = body.velocity;
            // consider rounding in direction of throw or getting rid of throws
            //v.x = ((float) Mathf.Round(transform.position.x) - transform.position.x) * 10;
            //if (v.y > 0) { v.y = 0; }
            
            //if (Mathf.Abs(v.x) >= 1)
            //{
            //    body.velocity = v;
            //}
            //else
            //{
                // snap x
                v.x = 0;
                body.velocity = v;
                deltaPosition = new Vector3((float) Mathf.Round(transform.position.x), transform.position.y, transform.position.z);
                transform.position = deltaPosition;
            //}
        }
        */

        // clear words when tile stops moving
        if (wasMoving && body.velocity == Vector2.zero)
        {
            // snap y
            newPosition = transform.position;
            float roundedY = Mathf.Round(newPosition.y);
            if (newPosition.y > roundedY + 0.01f || newPosition.y < roundedY - 0.01f)
            //if (roundedY != newPosition.y) 
            {
                Debug.Log(newPosition.y + " != " + roundedY);
                newPosition.y = roundedY;

                //body.velocity = Vector2.zero;
                transform.position = newPosition;
                Debug.Log("New postion: " + transform.position.x + ", " + transform.position.y);
            }

            // tile was moving and has now stopped
            wasMoving = false;
            //Debug.Log("Stopped moving");
            //clear words
            Collider2D c = GetComponent<Collider2D>();
            Collider2D[] otherArray = new Collider2D[2];

            ContactFilter2D cf = new ContactFilter2D();
            cf.SetLayerMask(LayerMask.GetMask("Row", "Col"));

            int length = c.OverlapCollider(cf, otherArray);

            //Debug.Log("otherArray length: " + length);

            WordClear[] rowCol = new WordClear[2];
            rowCol[0] = null;
            rowCol[1] = null;

            for (int i = 0; i < length; i++)
            {
                if (otherArray[i] != null)
                {
                    // there should only be 2
                    if (otherArray[i].gameObject.layer == 8) 
                    {
                        rowCol[0] = otherArray[i].gameObject.GetComponent<WordClear>();
                        //Debug.Log("IS ROW!");
                    }
                    else if (otherArray[i].gameObject.layer == 9)
                    {
                        rowCol[1] = otherArray[i].gameObject.GetComponent<WordClear>();
                        //Debug.Log("IS COL!");
                    }
                }
            }

            for (int i = 0; i < rowCol.Length; i++)
            {
                if (rowCol[i] != null) 
                {
                    rowCol[i].ClearWords();
                }
            }
        }
        else if (body.velocity != Vector2.zero)
        {
            // tile is moving again
            wasMoving = true;
        }
    }
}
