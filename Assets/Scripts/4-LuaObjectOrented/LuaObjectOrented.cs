using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaObjectOrented : MonoBehaviour
{
    [CSharpCallLua]
    public delegate ICalc CalcNew(int mult, params string[] args);

    private string script = @"
        local calc_mt ={
            __index = { Add = function(self,a,b)
                    return (a+b)*self.Mult;
                    end    
        }        
}

Calc = {
    New = function(mult,...)
            print(...);
            return setmetatable({Mult = mult},calc_mt);
        end
}
    ";
    // Use this for initialization
    void Start()
    {
        LuaEnv luaEnv = new LuaEnv();
        Test(luaEnv);
        luaEnv.Dispose();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Test(LuaEnv luaEnv)
    {
        luaEnv.DoString(script);
        CalcNew calc_new = luaEnv.Global.GetInPath<CalcNew>("Calc.New");
        ICalc calc = calc_new(10, "hi", "John");
        Debug.Log("Sum(*10) = " + calc.Add(1,2));
        calc.Mult = 100;
        Debug.Log("Sum(*100) = " + calc.Add(1, 2));
    }
}

[CSharpCallLua]
public interface ICalc
{
    int Add(int a,int b);
    int Mult { get; set; }
}
