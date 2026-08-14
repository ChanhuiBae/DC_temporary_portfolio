using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TreeNode
{

    public int x;
    public int y;
    public bool visited;
    public TreeNode parent;
    public TreeNode left;
    public TreeNode right;
    public TreeNode up;
    public TreeNode down;

    public TreeNode(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}


public class Tree
{
    private TreeNode root = null;
    public TreeNode Root => root;

    private TreeNode current = null;
    public TreeNode Current { get { return current; } }

    public TreeNode GetNextCurrent()
    {
        if (current == null)
        {
            current = root;
        }
        else
        {
            if (current.parent == null)
            {
                if (current.left != null)
                    current = current.left;
                else if(current.right != null)
                    current = current.right;
                else if(current.up != null)
                    current = current.up;
                else if(current.down != null)
                    current = current.down;
            }
            else
            {
                TreeNode node = current;

                while (node.visited)
                {
                    if(node.parent != null)
                    {
                        node = node.parent;
                    }

                    if(node.left != null && !node.left.visited)
                    {
                        node = node.left;
                    }
                    else if(node.right != null && !node.right.visited)
                    {
                        node = node.right;
                    }
                    else if(node.up != null && !node.up.visited)
                    {
                        node = node.up;
                    }
                    else if(node.down != null && !node.down.visited)
                    {
                        node = node.down;
                    }
                }
                current = node;
            }
        }

        current.visited = true;
        return current;
    }




    public void Instert(Direction derection, TreeNode node)
    {
        if (root == null)
        {
            root = node;
            GetNextCurrent();
            return;
        }

        switch (derection)
        {
            case Direction.Left:
                if (current.left == null)
                    current.left = node;
                break;
            case Direction.Right:
                if (current.right == null)
                    current.right = node;
                break;
            case Direction.Up:
                if (current.up == null)
                    current.up = node;
                break;
            case Direction.Down:
                if (current.down == null)
                    current.down = node;
                break;
        }
    }

}
