using Robust.Client;

namespace Content.Oathlord.Client
{
    internal sealed class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ContentStart.Start(args);
        }
    }
};
