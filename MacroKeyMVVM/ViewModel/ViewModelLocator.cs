using Ninject;

namespace MacroKey.ViewModel
{
    public class ViewModelLocator
    {
        private static StandardKernel kernel = new StandardKernel(new ViewModelNinjectModule());

        static ViewModelLocator()
        {
            //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            //SimpleIoc.Default.Register<MainViewModel>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public static MainViewModel Main => kernel.Get<MainViewModel>();
        //public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static void Cleanup()
        {
            Main.Cleanup();
        }
    }
}