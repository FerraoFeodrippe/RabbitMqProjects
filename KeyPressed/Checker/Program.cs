using Actions;

ICheckerAction? checkerAction = null;
var typeChecker = typeof(ICheckerAction);
var actions =  typeChecker.Assembly.GetTypes().Where(t=> t.IsAssignableTo(typeChecker) && !t.IsInterface);

foreach(var action in actions)
{
    Console.WriteLine(action.Name);
}

if (args.Length > 0)
{
    try
    {
        var action = actions.FirstOrDefault(t => args[0].Equals(t.Name, StringComparison.InvariantCultureIgnoreCase));

        if (action != null)
        {

            var arguments = args.Skip(1);
            typeChecker.Assembly.CreateInstance(typeChecker.Name);
            checkerAction = (ICheckerAction?) Activator.CreateInstance(action, new object?[] { arguments.ToArray() });
            Console.WriteLine($"Running action extension: {action.Name}");
        }

    }
    catch(Exception ex)
    {
        Console.WriteLine($"Error message on action executing:  {ex.Message}");
        Console.WriteLine($"Error message inner Exception on action executing:  {ex.InnerException?.Message}");
    }

}

using var checker = new Checker();
checker.Run(checkerAction);