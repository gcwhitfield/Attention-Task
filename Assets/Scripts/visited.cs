using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Pair
{
    public int i;
    public int j;

    public Pair(int i, int j)
    {
        this.i = i;
        this.j = j;
    }
}

public struct unit { }

public class visited {

    static Dictionary<Pair, unit> Sigma = new Dictionary<Pair, unit>();

    public static bool visit(int i, int j)
    {
        Pair p = new Pair(i, j);
        if (Sigma == null) Sigma = new Dictionary<Pair, unit>();
        if (Sigma.ContainsKey(p)) return false;

        Sigma[p] = new unit();
        return true;
    }
    public static int size()
    {
        if (Sigma == null) Sigma = new Dictionary<Pair, unit>();
        return Sigma.Keys.Count;
    }
    public static void clear(){
        Sigma.Clear();
    }
}


