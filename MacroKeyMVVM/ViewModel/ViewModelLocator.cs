using Ninject;

namespace MacroKey.ViewModel
{
    public class ViewModelLocator
    {
        private static StandardKernel kernel = new StandardKernel(new ViewModelNinjectModule());

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public static MainViewModel Main => kernel.Get<MainViewModel>();

        public static void Cleanup()
        {
            Main.Cleanup();
        }
    }
}