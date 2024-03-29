﻿using Ninject;
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
        /// A shortcut to access the <see cref="IUpdateManager"/>
        /// </summary>
        public static IUpdateManager UpdateManager => IoC.Get<IUpdateManager>();

        /// <summary>
        /// A shortcut to access the <see cref="IUIManager"/>
        /// </summary>
        public static IUIManager UI => IoC.Get<IUIManager>();

        /// <summary>
        /// A shortcut to access the <see cref="ISettingsManager"/>
        /// </summary>
        public static ISettingsManager Settings => IoC.Get<ISettingsManager>();

        /// <summary>
        /// A shortcut access to the <see cref="ApplicationViewModel"/>
        /// </summary>
        public static ApplicationViewModel ApplicationVM => Get<ApplicationViewModel>();

        /// <summary>
        /// A shortcut access to the <see cref="MainViewModel"/>
        /// </summary>
        //public static MainViewModel MainVM => Get<MainViewModel>();

        /// <summary>
        /// A shortcut access to the <see cref="ProgressBoxDialogViewModel"/>
        /// </summary>
        public static ProgressBoxDialogViewModel SavingProgressVM => Get<ProgressBoxDialogViewModel>();

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
        }

        /// <summary>
        /// Binds all singleton view models
        /// </summary>
        private static void BindViewModels()
        {
            Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
            //Kernel.Bind<MainViewModel>().ToConstant(new MainViewModel());
            Kernel.Bind<ProgressBoxDialogViewModel>().ToConstant(new ProgressBoxDialogViewModel());
        }
    }
}
