using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System;

[LuaCallCSharp]
public class U3DScripting2 : MonoBehaviour
{
    public TextAsset luaScript;
    public InjectionObj[] injections;

    //All lua behaviour shared one luaenv only!
    internal static LuaEnv luaEnv = new LuaEnv();
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1; //one second

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;

    private LuaTable scriptEnv;

    private void Awake()
    {
        scriptEnv = luaEnv.NewTable();
        //创建一个元表
        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        //把数据赋值给Lua表
        scriptEnv.Set("self", this);

        foreach (var injection in injections)
        {
            scriptEnv.Set(injection.Inject_Name, injection.Inject_Value);
        }

        luaEnv.DoString(luaScript.text, "111111", scriptEnv);
        Action luaAwake = scriptEnv.Get<Action>("awake");

        scriptEnv.Get("start", out luaStart);
        scriptEnv.Get("update", out luaUpdate);
        scriptEnv.Get("ondestroy", out luaOnDestroy);

        if (luaAwake != null)
        {
            Debug.Log("Language of Lua contains Awake method");
            luaAwake();
        }
        else
        {
            Debug.Log("Language of Lua doesnt contain Awake method");
        }
    }

    private void Start()
    {
        if (luaStart != null)
        {
            luaStart();
        }
    }

    private void Update()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
        }
        if (Time.time - LuaBehaviour.lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            LuaBehaviour.lastGCTime = Time.time;
        }
    }

    private void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        scriptEnv.Dispose();
        injections = null;
    }

}
[Serializable]
public class InjectionObj
{
    public string Inject_Name;
    public GameObject Inject_Value;
}
