using ConsoleApp;
using Serilog;

//Setup Logger
Setup.Serilog();

//Write some usefull logs
Log.Information("Info error");
Log.Warning("Warning error");

//Write some exception logs
try
{
    Log.Information("Info error");
    throw new StackOverflowException();
}
catch (Exception ex)
{
    //Catch exceptions and write hints
    Log.Error("An exception occured.".AddHints(
        new
        {
            A = "Value of A",
            B = "Value of B",
            C = new { Fname = "Sangee", LName = "Nandakumar" }
        }
        , ex));
}


Console.Read();