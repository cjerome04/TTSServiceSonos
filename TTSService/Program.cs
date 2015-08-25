using System;
using System.Diagnostics;
using TTSService.ServiceModel;

namespace TTSService
{
    class Program
    {
        private static AppHost _appHost;
        static void Main(string[] args)
        {
            Settings.Init();


            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            _appHost = new AppHost();

            _appHost.Init();
            _appHost.Start(Settings.UrlService);
            Console.WriteLine("");
            Console.WriteLine("Press ENTER to quit...");
            Console.ReadLine();
        }

        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EventLog.WriteEntry(Settings.ServiceName, "The TTSService has a Unhandled exception. " + e.ExceptionObject, EventLogEntryType.Error);
        }
    }
}
