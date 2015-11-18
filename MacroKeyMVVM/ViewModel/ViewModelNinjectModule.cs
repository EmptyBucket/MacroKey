using MacroKey.InputData;
using MacroKey.LowLevelApi;
using MacroKey.LowLevelApi.Hook;
using MacroKey.LowLevelApi.HookReader;
using MacroKey.Machine;
using Ninject;
using Ninject.Modules;

namespace MacroKey.ViewModel
{
    class ViewModelNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IHooker>().To<HookerKey>();
            Bind<IHookReader<KeyData>>().To<HookKeyDataReader>();
            Bind<IHookReader<KeyDataDelay>>().To<HookKeyDataDelayReader>();
            Bind<ISenderInput>().To<SenderKeyDelayInput>();
            Bind<IState<KeyData>>().To<Tree<KeyData>>().WithConstructorArgument(Kernel.Get<KeyDataEqualityComparer>());
            Bind<IState<KeyData>>().To<Branch<KeyData>>().WithConstructorArgument(Kernel.Get<KeyDataEqualityComparer>());
            Bind<IState<KeyData>>().To<State<KeyData>>().WithConstructorArgument(Kernel.Get<KeyDataEqualityComparer>());
            Bind<IState<KeyDataDelay>>().To<Tree<KeyDataDelay>>().WithConstructorArgument(Kernel.Get<KeyDataEqualityComparer>());
            Bind<IState<KeyDataDelay>>().To<Branch<KeyDataDelay>>().WithConstructorArgument(Kernel.Get<KeyDataEqualityComparer>());
            Bind<IState<KeyDataDelay>>().To<State<KeyDataDelay>>().WithConstructorArgument(Kernel.Get<KeyDataEqualityComparer>());
        }
    }
}
