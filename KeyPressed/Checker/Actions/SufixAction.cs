namespace Actions
{
    public class SufixAction: ICheckerAction
    {
        protected readonly string[] Args;

        public SufixAction(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentException("SufixAction needs at last 1 arguments.");
            }

            Args = args;
        }

        public void Run(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine($"SufixAction: {args[0]} - {Args[0]}");
            }
            else
            {
                Console.WriteLine($"SufixAction: needs at last 1 argument on Run sufix action.");
            }
        }
    }

}