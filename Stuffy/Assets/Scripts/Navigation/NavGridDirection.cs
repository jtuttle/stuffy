using System.Collections.Generic;
using System;

public class NavGridDirection {
    public static readonly NavGridDirection N = new NavGridDirection();
    public static readonly NavGridDirection E = new NavGridDirection();
    public static readonly NavGridDirection S = new NavGridDirection();
    public static readonly NavGridDirection W = new NavGridDirection();
    
    private NavGridDirection() { }
    
    public override string ToString() {
        if(this == N) return "N";
        if(this == E) return "E";
        if(this == S) return "S";
        if(this == W) return "W";
        throw new Exception("Unable to stringify direction");
    }
    
    public NavGridDirection Reverse() {
        if(this == N) return S;
        if(this == E) return W;
        if(this == S) return N;
        if(this == W) return E;
        throw new Exception("Unable to reverse direction");
    }
    
    public static List<NavGridDirection> All {
        get { return new List<NavGridDirection>() { N, E, S, W }; }
    }
}