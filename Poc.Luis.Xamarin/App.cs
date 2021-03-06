﻿using System;
using Poc.Luis.Xamarin.Views;
using Prism.Unity;
using Microsoft.Practices.Unity;

namespace Poc.Luis.Xamarin
{
    public class App : PrismApplication
    {
        public static SQLite.SQLiteConnection AppSQLiteConnection;
        public static bool OnEmulator { get; set; } = false;

        protected override void OnInitialized()
        {
            try
            {
                NavigationService.NavigateAsync("BaseNavigationPage/HomePage");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void RegisterTypes()
        {
            Container.RegisterType(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            Container.RegisterType(typeof(IApplicationServices<>), typeof(ApplicationServices<>));

            Container.RegisterInstance(Plugin.Media.CrossMedia.Current);
            Container.RegisterInstance(Acr.UserDialogs.UserDialogs.Instance);

            Container.RegisterTypeForNavigation<HomePage>();
            Container.RegisterTypeForNavigation<BlankPage>();
            Container.RegisterTypeForNavigation<BaseNavigationPage>();
        }
    }
}