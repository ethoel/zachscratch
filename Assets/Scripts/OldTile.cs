using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldTile : MonoBehaviour
{
    public Sprite[] tileSpriteArray;
    public char letter;
    public bool mouseDown;
    public bool heldUp;

    private static float DELTA_MAX = 0.5f;
    private static int TILE = 10;
    private static char[] LETTER_ARRAY = { 'j', 'j', 'k', 'k', 'q', 'q', 'x', 'x', 'z', 'z', 'b', 'b', 'b', 'c', 'c', 'c', 'f', 'f', 'f', 'h', 'h', 'h', 'm', 'm', 'm', 'p', 'p', 'p', 'v', 'v', 'v', 'w', 'w', 'w', 'y', 'y', 'y','g', 'g', 'g', 'g','l', 'l', 'l', 'l', 'l', 'd', 'd', 'd', 'd', 'd', 'd', 's', 's', 's', 's', 's', 's', 'u', 'u', 'u', 'u', 'u', 'u', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 't', 't', 't', 't', 't', 't', 't', 't', 't', 'r', 'r', 'r', 'r', 'r', 'r', 'r', 'r', 'r', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'i', 'i', 'i', 'i', 'i', 'i', 'i', 'i', 'i', 'i', 'i', 'i', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e' };

    private bool mouseUp;
    private bool heldUpJustNow;
    private bool blockedTop;
    private bool blockedBottom;
    private bool blockedLeft;
    private bool blockedRight;
    private float speed;
    private Vector2 pointerPosition;

    // Start is called before the first frame update
    void Start()
    {
        // set tile letter and sprite
        letter = LETTER_ARRAY[Random.Range(0, LETTER_ARRAY.Length)];
        (GetComponent<SpriteRenderer>()).sprite = tileSpriteArray[(int) letter - 97]; // 97 is a

        // set initials
        speed = 3f;
        mouseDown = false;
        mouseUp = false;
        heldUp = false;
        heldUpJustNow = false;
        blockedTop = false;
        blockedBottom = false;
        blockedLeft = false;
        blockedRight = false;
    }

    void OnMouseDown() {
        mouseDown = true;
    }

    void OnMouseUp() {
        mouseDown = false;
        mouseUp = true;
    }

    // FixedUpdate is called once per physics update
    void FixedUpdate()
    {
        Move();
        Fall();
        CorrectPosition();
    }

    // Update is called once per frame
    void Update()
    {
        GetPointerPosition();
        ClearWords();
    }

    private void GetPointerPosition()
    {
        if (mouseDown)
        {
            pointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pointerPosition.x -= 0.5f; // adjust to center of tile
            pointerPosition.y -= 0.5f; // adjust to center of tile  
        }
    }

    // move with mouse
    private void Move()
    {
        if (mouseDown)
        {
            Vector2 newPosition = transform.position;
            //newPosition.x = Mathf.Round(newPosition.x);
            //newPosition.y = Mathf.Round(newPosition.y);

            float deltaX = Mathf.Abs(pointerPosition.x - newPosition.x);
            if (deltaX > DELTA_MAX) deltaX = DELTA_MAX; // max value to move along y axis

            if (pointerPosition.x > newPosition.x && !blockedRight)
            {
                newPosition.x += deltaX;
            }
            else if (pointerPosition.x < newPosition.x && !blockedLeft)
            {
                newPosition.x -= deltaX;
            }

            float deltaY = Mathf.Abs(pointerPosition.y - newPosition.y);
            if (deltaY > DELTA_MAX) deltaY = DELTA_MAX; // max value to move along y axis

            if (pointerPosition.y > newPosition.y && !blockedTop) 
            {
                newPosition.y += deltaY;
            }
            else if (pointerPosition.y < newPosition.y && !blockedBottom)
            {
                newPosition.y -= deltaY;
            }
            transform.position = newPosition;
        }
        else if (mouseUp)
        {
            // snap x once when mouse goes up
            Vector2 newPosition = transform.position;
            float roundedX = Mathf.Round(newPosition.x);
            
            newPosition.x = roundedX;
            /*if (roundedX > newPosition.x && !blockedRight)
            {
                newPosition.x = roundedX;
            }
            else if (roundedX < newPosition.x && !blockedLeft)
            {
                newPosition.x = roundedX;
            }
            else if (roundedX == newPosition.x)
            {
                newPosition.x = roundedX;
            }
            else if (blockedLeft)
            {
                newPosition.x = roundedX + 1f;
            }
            else if (blockedRight)
            {
                newPosition.x = roundedX - 1f;
            }
            else if (blockedLeft && blockedRight)
            {
                Debug.Log("Tile Error 1: Cannot snap x");
            }*/
            transform.position = newPosition;
            mouseUp = false;
        }
    }

    // tile falls down constantly if not blocked or held
    private void Fall()
    {
        if (!mouseDown && !blockedBottom)
        {
            Vector2 newPosition = transform.position;
            newPosition.y -= speed * Time.deltaTime;
            transform.position = newPosition;
        }
    }

    // because of float precision, tile positions have to be corrected a bit after they settle
    private void CorrectPosition()
    {
        if (!mouseDown && blockedBottom)
        {
            Vector2 currentPosition = transform.position;
            float roundedX = Mathf.Round(currentPosition.x);
            if (currentPosition.x != roundedX)
            {
                currentPosition.x = roundedX;
            }
            
            if (!heldUp && !heldUpJustNow)
            {
                float roundedY = Mathf.Round(currentPosition.y);
                if (currentPosition.y != roundedY)
                {
                    currentPosition.y = roundedY;
                }

            }
            heldUpJustNow = heldUp; // using this variable to extend heldUp by one physics cycle
            transform.position = currentPosition;
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        CheckBlocked(other);
    }

    public void OnCollisionStay2D(Collision2D other)
    {
        //CheckBlocked(other);
    }

    public void OnCollisionExit2D(Collision2D other) 
    {
        CheckBlocked(other);
    }

    // check if there are colliders blocking movement on each side
    private void CheckBlocked(Collision2D other)
    {
        heldUp = false;
        blockedTop = false;
        blockedBottom = false;
        blockedLeft = false;
        blockedRight = false;

        ContactPoint2D[] contactArray = new ContactPoint2D[10]; //6 should be max
        int contactCount = other.otherCollider.GetContacts(contactArray); //other.other is this
        for (int i = 0; i < contactCount; i++)
        {
            if (mouseDown)
            {
                CheckBlockedMouseDown(contactArray[i]);
            }
            else
            {
                CheckBlockedMouseUp(contactArray[i]);
            }
        }
    }

    private void CheckBlockedMouseDown(ContactPoint2D contact)
    {
        Vector2 thisPosition = transform.position;
        Vector2 otherPosition = contact.collider.transform.position;

        bool isWall = contact.collider.gameObject.layer != TILE;
        if (isWall)
        {
            otherPosition.x -= 0.5f;
            otherPosition.y -= 0.5f;
        }

        Vector2 newPosition = thisPosition;

        if (contact.normal == Vector2.down)
        {
            blockedTop = true;
            newPosition.y = otherPosition.y - 1f;
        }
        if (contact.normal == Vector2.up)
        {
            blockedBottom = true;
            newPosition.y = otherPosition.y + 1f;
        }
        if (contact.normal == Vector2.right)
        {
            blockedLeft = true;
            newPosition.x = otherPosition.x + 1f;
        }
        if (contact.normal == Vector2.left)
        {
            blockedRight = true;
            newPosition.x = otherPosition.x - 1f;
        }
        transform.position = newPosition;
    }

    private void CheckBlockedMouseUp(ContactPoint2D contact)
    {
        Vector2 thisPosition = transform.position;
        Vector2 otherPosition = contact.collider.transform.position;

        bool isWall = contact.collider.gameObject.layer != TILE;
        bool isMouseDownTile = false;
        bool isHeldUpTile = false;
        if (!isWall)
        {
            OldTile contactTile = contact.collider.gameObject.GetComponent<OldTile>();
            isMouseDownTile = contactTile.mouseDown;
            isHeldUpTile = contactTile.heldUp;
        }
        
        if (contact.normal == Vector2.down && (thisPosition.x == otherPosition.x || isWall || isMouseDownTile))
        {
            blockedTop = true;
        }
        if (contact.normal == Vector2.up && (thisPosition.x == otherPosition.x || isWall || isMouseDownTile))
        {
            blockedBottom = true;
            if (isMouseDownTile || isHeldUpTile) heldUp = true;
        }
        if (contact.normal == Vector2.right)
        {
            blockedLeft = true;
        }
        if (contact.normal == Vector2.left)
        {
            blockedRight = true;
        }
    }

    private void ClearWords()
    {
        if (!mouseDown && blockedBottom && !heldUp && !heldUpJustNow)
        {
            Collider2D c = GetComponent<Collider2D>();
            Collider2D[] otherArray = new Collider2D[2];

            ContactFilter2D cf = new ContactFilter2D();
            cf.SetLayerMask(LayerMask.GetMask("Row", "Col"));

            int length = c.OverlapCollider(cf, otherArray);

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
                    }
                    else if (otherArray[i].gameObject.layer == 9)
                    {
                        rowCol[1] = otherArray[i].gameObject.GetComponent<WordClear>();
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
    }
}
