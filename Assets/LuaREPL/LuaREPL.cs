using UnityEngine;

public class LuaREPL : MonoBehaviour
{
    public int InitialCLIPortLine = 7500;
    public int InitialCLIPortFile = 7501;
    public string FileServerDomain = "127.0.0.1:8000/";

    [HideInInspector] public CLI.Bridge CLILine;
    [HideInInspector] public CLI.Bridge CLIFile;

    private XLua.LuaEnv L;

    private void Awake()
    {
        CLILine = gameObject.AddComponent<CLI.Bridge>();
        CLILine.InitialPort = InitialCLIPortLine;
        CLILine.WelcomeMessage = "Lua REPL by Line";

        CLIFile = gameObject.AddComponent<CLI.Bridge>();
        CLIFile.InitialPort = InitialCLIPortFile;
        CLIFile.WelcomeMessage = "Lua REPL with file";
    }

    private void OnEnable()
    {
        if (CLILine.Executer == null)
            CLILine.Executer = new CLI.CustomExecuter(ExecuteByLine);
        if (CLIFile.Executer == null)
            CLIFile.Executer = new CLI.CustomExecuter(ExecuteWithFile);
    }

    private CLI.Result ExecuteByLine(CLI.Command cmd, int argFrom)
    {
        if (L == null) L = new XLua.LuaEnv();
        return LuaReturnToCLIResult(L.DoString(cmd.Raw));
    }

    private CLI.Result ExecuteWithFile(CLI.Command cmd, int argFrom)
    {
        var www = new WWW(FileServerDomain + cmd[0]);
        while (!www.isDone) ;
        if (L == null) L = new XLua.LuaEnv();
        return LuaReturnToCLIResult(L.DoString(www.text));
    }

    private static string Concat(object[] objs)
    {
        var ret = "[ ";
        foreach (var obj in objs) ret += obj + ", ";
        ret += "]";
        return ret;
    }

    private static CLI.Result LuaReturnToCLIResult(object[] rets)
    {
        if (rets == null || rets.Length == 0) return CLI.Result.Success();
        else if (rets.Length == 1) return CLI.Result.Success(rets[0].ToString());
        else return CLI.Result.Success(Concat(rets));
    }
}
