// See https://aka.ms/new-console-template for more information
using ProjWizInc.Core.Managers;

Console.WriteLine("Hello, World!");
Console.WriteLine("Enter any key to exit...");
DefinitionManager defs = new DefinitionManager();
defs.GenerateTemplate();
Console.ReadLine();
