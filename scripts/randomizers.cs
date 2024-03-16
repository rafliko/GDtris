using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class randomizers : Node
{
    public static void bagGenerator(List<game.pieceType> queue)
    {
        Random rnd = new Random();
        List<game.pieceType> tmp = new List<game.pieceType>();

        for(int i=0; i<7; i++)
        {
            game.pieceType x = (game.pieceType)rnd.Next(7);
            if(!tmp.Contains(x)) tmp.Add(x);
            else i--;
        }

        for(int i=0; i<7; i++)
        {
            queue.Add(tmp[i]);
            GD.Print(queue[i]);
        }
    }
}
