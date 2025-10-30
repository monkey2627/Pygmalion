using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector2 moveDir;
    public LayerMask detectLayer;
    int count = 0;

    public Sprite[] spritePlayers;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GetComponent<SpriteRenderer>().sprite = spritePlayers[3];
            moveDir = Vector2.right;
        }


        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GetComponent<SpriteRenderer>().sprite = spritePlayers[2];
            moveDir = Vector2.left;
        }


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GetComponent<SpriteRenderer>().sprite = spritePlayers[0];
            moveDir = Vector2.up;
        }


        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            GetComponent<SpriteRenderer>().sprite = spritePlayers[1];
            moveDir = Vector2.down;
        }
            

        if(moveDir != Vector2.zero)
        {
            if(CanMoveToDir(moveDir))
            {
                Move(moveDir);
            }
        }

        moveDir = Vector2.zero;
    }

    bool CanMoveToDir(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1f, detectLayer);

        if (!hit)
            return true;
        else
        {


            if (hit.collider.GetComponent<Box>() != null)
            {
                Debug.Log(hit.collider.name);
                return hit.collider.GetComponent<Box>().CanMoveToDir(dir);
            }

           

        }

        return false;
    }

    void Move(Vector2 dir)
    {
        transform.Translate(dir);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<question>() != null)
        {
            count++;
            if (count == 2)
            {
                Debug.Log("跳转剧情");
            }
        }
    }
}
