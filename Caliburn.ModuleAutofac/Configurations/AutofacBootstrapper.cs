using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Caliburn.Micro;

namespace ModuleSystem.ModuleAutofac.Configurations
{
    /// <summary>
    /// A strongly-typed version of Caliburn.Micro.Bootstrapper that specifies the type of root
    /// model to create for the application.
    /// </summary>
    /// <typeparam name="TRootViewModel"> The type of root view model for the application. </typeparam>
    public abstract class AutofacBootstrapper<TRootViewModel> : Caliburn.Micro.BootstrapperBase
    {
        #region Public Constructors

        public AutofacBootstrapper(string searchPattern)
           
        {
            AssemblySearchPattern = searchPattern;
            Initialize();
        }

        //public AutofacBootstrapper()
        //{
        //    Initialize();
        //}

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the assembly search pattern. If set, executing program's local directory is
        /// searched for assemblies matching the specified pattern and is automatically loaded and
        /// added to <see cref="Caliburn.Micro.AssemblySource"/>
        /// </summary>
        public string AssemblySearchPattern { get; set; }

        /// <summary>
        /// Should the IoC automatically subscribe any types found that implement the IHandle
        /// interface at activation
        /// </summary>
        public bool AutoSubscribeEventAggegatorHandlers { get; set; }

        /// <summary>
        /// Method for creating the event aggregator 
        /// </summary>
        public Func<IEventAggregator> CreateEventAggregator { get; set; }

        /// <summary>
        /// Method for creating the window manager 
        /// </summary>
        public Func<IWindowManager> CreateWindowManager { get; set; }

        #endregion Public Properties

        #region Protected Properties

        protected IContainer Container { get; private set; }

        #endregion Protected Properties

        #region Protected Methods

        /// <summary>
        /// Do not override unless you plan to full replace the logic. This is how the framework
        /// retrieves services from the Autofac container.
        /// </summary>
        /// <param name="instance"> The instance to perform injection on. </param>
        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }

        /// <summary>
        /// Do not override this method. This is where the IoC container is configured. <remarks>
        /// Will throw <see cref="System.ArgumentNullException"/> is either CreateWindowManager or
        /// CreateEventAggregator is null. </remarks>
        /// </summary>
        protected override void Configure()
        { //  allow base classes to change bootstrapper settings
            ConfigureBootstrapper();

            // validate settings 
            if (CreateWindowManager == null)
                throw new ArgumentNullException("CreateWindowManager");
            if (CreateEventAggregator == null)
                throw new ArgumentNullException("CreateEventAggregator");

            // configure container 
            var builder = new ContainerBuilder();

            // register the single window manager for this container 
            builder.Register<IWindowManager>(c => CreateWindowManager()).SingleInstance();
            // register the single event aggregator for this container 
            builder.Register<IEventAggregator>(c => CreateEventAggregator()).SingleInstance();

            // allow derived classes to add to the container 
            ConfigureContainer(builder);

            Container = builder.Build();
        }

        /// <summary>
        /// Override to provide configuration prior to the Autofac configuration. You must call the
        /// base version BEFORE any other statement or the behaviour is undefined. Current Defaults:
        /// EnforceNamespaceConvention = true ViewModelBaseType =
        /// <see cref="System.ComponentModel.INotifyPropertyChanged"/> CreateWindowManager =
        /// <see cref="Caliburn.Micro.WindowManager"/> CreateEventAggregator = <see cref="Caliburn.Micro.EventAggregator"/>
        /// </summary>
        protected virtual void ConfigureBootstrapper()
        {
            if (!string.IsNullOrWhiteSpace(AssemblySearchPattern))
            {
                var location = System.IO.Path.GetDirectoryName(GetType().Assembly.Location);
                var files = System.IO.Directory.GetFiles(location, AssemblySearchPattern);
                foreach (var file in files)
                {
                    var asm = System.Reflection.Assembly.LoadFrom(file);
                    AssemblySource.Instance.Add(asm);
                }
            }

            // default is to auto subscribe known event aggregators 
            AutoSubscribeEventAggegatorHandlers = false;
            // default window manager 
            CreateWindowManager = () => new WindowManager();
            // default event aggregator 
            CreateEventAggregator = () => new EventAggregator();
        }

        /// <summary>
        /// Override to include your own Autofac configuration after the framework has finished its
        /// configuration, but before the container is created.
        /// </summary>
        /// <param name="builder"> The Autofac configuration builder. </param>
        protected virtual void ConfigureContainer(ContainerBuilder builder)
        {
            foreach (var asm in AssemblySource.Instance)
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(ExcludedAttribute), false).Any() || type.IsInterface || type.IsAbstract)
                        continue;

                    var tree = GetTypeTree(type);

                    if (type.IsGenericType)
                    {
                        var generic = type.GetGenericTypeDefinition();
                        //if type has generic parameter, then register for base types with generic parameters,
                        //as it cannot be instatiated without generic arguments which won't be available for non generic services
                        var registration = builder.RegisterGeneric(generic).AsSelf().As(tree.Where(x => x.IsGenericType && x.GetGenericArguments().Length == type.GetGenericArguments().Length).ToArray());
                        if (type.GetCustomAttributes(typeof(SingleInstanceAttribute), false).Any())
                            registration.SingleInstance();
                        else
                            registration.InstancePerDependency();
                    }
                    else
                    {
                        var registration = builder.RegisterType(type).AsSelf().As(tree.ToArray());
                        if (type.GetCustomAttributes(typeof(SingleInstanceAttribute), false).Any())
                            registration.SingleInstance();
                        else
                            registration.InstancePerDependency();
                    }
                }
            }
        }

        /// <summary>
        /// Do not override unless you plan to full replace the logic. This is how the framework
        /// retrieves services from the Autofac container.
        /// </summary>
        /// <param name="service"> The service to locate. </param>
        /// <returns> The located services. </returns>
        protected override System.Collections.Generic.IEnumerable<object> GetAllInstances(System.Type service)
        {
            return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        /// <summary>
        /// Do not override unless you plan to full replace the logic. This is how the framework
        /// retrieves services from the Autofac container.
        /// </summary>
        /// <param name="service"> The service to locate. </param>
        /// <param name="key"> The key to locate. </param>
        /// <returns> The located service. </returns>
        protected override object GetInstance(System.Type service, string key)
        {
            object instance;
            if (string.IsNullOrWhiteSpace(key))
            {
                if (Container.TryResolve(service, out instance))
                    return instance;
            }
            else
            {
                if (Container.TryResolveNamed(key, service, out instance))
                    return instance;
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
        }

        protected virtual IEnumerable<Type> GetTypeTree(Type type)
        {
            var baseTypes = new List<Type>();

            var current = type;
            baseTypes.AddRange(current.GetInterfaces());
            while (current.BaseType != null && current.BaseType != typeof(object))
            {
                baseTypes.Add(current.BaseType);
                current = current.BaseType;
            }

            return baseTypes.Distinct();
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            DisplayRootViewFor<TRootViewModel>();
        }

        #endregion Protected Methods
    }

    /// <summary>
    /// Excludes the applied type from registering with the container
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExcludedAttribute : Attribute
    {
    }

    /// <summary>
    /// Signals to Register the type as a single instance in the container
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SingleInstanceAttribute : Attribute
    {
    }
}
