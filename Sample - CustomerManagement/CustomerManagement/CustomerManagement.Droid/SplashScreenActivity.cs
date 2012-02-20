using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Android.Interfaces;
using Cirrious.MvvmCross.Android.Views;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using Cirrious.MvvmCross.Interfaces.ViewModels;
using Cirrious.MvvmCross.ViewModels;
using CustomerManagement.Droid.Views;

namespace CustomerManagement.Droid
{
    [Activity(Label = "SplashScreenActivity", Theme = "@android:style/Theme.Black.NoTitleBar", MainLauncher = true, Icon = "@drawable/icon", NoHistory = true)]
    public class SplashScreenActivity
        : MvxActivityView<MvxNullViewModel>
        , IMvxServiceConsumer<IMvxStartNavigation>
    {
        private static bool _primaryInitialized = false;
        private static bool _secondaryInitialized = false;
        private static Setup _setup;

        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);

            if (!_primaryInitialized)
            {
                _primaryInitialized = true;

                // initialize app
                _setup = new Setup(ApplicationContext);
                _setup.InitializePrimary();
            }

            base.OnCreate(bundle);

            // Set our view from the "splash" layout resource
            SetContentView(Resource.Layout.Splash);
        }

        protected override void OnViewModelSet()
        {
            // ignored
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (!_secondaryInitialized)
            {
                _secondaryInitialized = true;
                System.Threading.ThreadPool.QueueUserWorkItem((ignored) =>
                {
                    _setup.InitializeSecondary();
                    TriggerFirstNavigate();
                });

            }
            else
            {
                TriggerFirstNavigate();
            }
        }

        private void TriggerFirstNavigate()
        {
            // trigger the first navigate...
            var starter = this.GetService<IMvxStartNavigation>();
            starter.Start();
        }
    }

/*
    public class SplashScreenActivity
        : Activity
        , IMvxServiceConsumer<IMvxStartNavigation>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // assign a layout with an image
            SetContentView(Resource.Layout.Splash);

            CheckFiles(ApplicationContext);

            // initialize app
            var setup = new Setup(ApplicationContext);
            setup.Initialize();

            // trigger the first navigate...
            var starter = this.GetService<IMvxStartNavigation>();
            starter.Start();
        }

        /// <summary>
        /// Copies the contents of input to output. Doesn't close either stream.
        /// </summary>
        public void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public void CheckFiles(Context context)
        {
            string documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            string dataDirectory = Path.Combine(documents, "Xml");
            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);

            string dataFile = Path.Combine(documents, @"Xml/Customers.xml");
            if (File.Exists(dataFile))
                return;

            Stream input = context.Assets.Open(@"Xml/Customers.xml");
            FileStream output = File.Create(dataFile);
            CopyStream(input, output);
            input.Close();
            output.Close();
        }
    }
 */
}