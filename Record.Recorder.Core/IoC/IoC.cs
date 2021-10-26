using Ninject;
using System;

namespace Record.Recorder.Core
{

    /// <summary>
    /// The IoC (Inversion of Control) container for the application
    /// </summary>
    public static class IoC
    {
        /// <summary>
        /// The kernel for the IoC container
        /// </summary>
        public static IKernel Kernel { get; private set; } = new StandardKernel();

        /// <summary>
        /// Gets a service from the IoC, of the specified type
        /// </summary>
        /// <typeparam name="T">The type to get</typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }

        /// <summary>
        /// Sets up the IoC container, binds all information required and is ready for use
        /// NOTE: Must be called as soon as the application starts up to ensure all services
        ///       can be found
        /// </summary>
        public static void Setup()
        {
            // Bind all required view models
            BindViewModels();

            // Set necessary default settings data
            if (string.IsNullOrEmpty(Properties.Settings.Default["saveFolderLocation"].ToString()))
            {
                Properties.Settings.Default["saveFolderLocation"] = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }
        }

        /// <summary>
        /// Binds all singleton view models
        /// </summary>
        private static void BindViewModels()
        {
            Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
        }
    }
}
