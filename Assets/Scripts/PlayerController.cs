﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector2 moveDir;
    public LayerMask detectLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            moveDir = Vector2.right;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            moveDir = Vector2.left;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            moveDir = Vector2.up;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            moveDir = Vector2.down;

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
                return hit.collider.GetComponent<Box>().CanMoveToDir(dir);
        }

        return false;
    }

    void Move(Vector2 dir)
    {
        transform.Translate(dir);
    }
}
