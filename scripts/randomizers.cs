using Godot;
using System;
using System.Linq;

public partial class randomizers : Node
{
    public static void bagGenerator(game.pieceType[] queue)
    {
        Random rnd = new Random();
        int[] tmp = new int[7];

        Array.Fill(tmp,-1);

        for(int i=0; i<7; i++)
        {
            int x = rnd.Next(7);
            if(!tmp.Contains(x))
                tmp[i] = x;
            else i--;
        }

        for(int i=0; i<7; i++)
        {
            queue[i] = (game.pieceType)tmp[i];
            GD.Print(queue[i]);
        }
    }
}
