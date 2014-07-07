﻿namespace Cedar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Cedar.CommandHandling;
    using Cedar.Hosting;
    using TinyIoC;

    public abstract class CedarBootstrapper
    {
        /// <summary>
        ///     Gets the assemblies to scan.
        /// </summary>
        /// <value>
        ///     The assemblies to scan. The default value is the assembly the bootstrapper is defined in.
        /// </value>
        protected virtual IEnumerable<Assembly> AssembliesToScan
        {
            get { return new[] { typeof(CedarBootstrapper).Assembly, GetType().Assembly }; }
        }

        /// <summary>
        ///     Gets the command handler types.
        /// </summary>
        /// <value>
        ///     The command handler types.
        /// </value>
        /// <remarks>
        ///     All types returned must implement <see cref="ICommandHandler{T}"/>
        /// </remarks>
        public virtual IEnumerable<Type> CommandHandlerTypes
        {
            get { return AssembliesToScan.SelectMany(assembly => assembly.GetImplementorsOfOpenGenericInterface(typeof(ICommandHandler<>))); }
        }

        public virtual void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register(GetSystemClock());
            container.Register(GetExceptionToModelConverter());

            var commandsAndHandlers = CommandHandlerTypes.Select(commandHandlerType => new
            {
                CommandHandlerType = commandHandlerType,
                CommandType = commandHandlerType.GetInterfaceGenericTypeArguments(typeof(ICommandHandler<>))[0]
            }).ToArray();
            MethodInfo registerCommandHandlerMethod = typeof(TinyIoCExtensions)
                .GetMethod("RegisterCommandHandler", BindingFlags.Public | BindingFlags.Static);
            foreach (var c in commandsAndHandlers)
            {
                registerCommandHandlerMethod
                    .MakeGenericMethod(c.CommandType, c.CommandHandlerType)
                    .Invoke(this, new object[] { container });
            }
        }

        public abstract string VendorName { get; } 

        protected virtual ISystemClock GetSystemClock()
        {
            return new SystemClock(DateTimeOffset.UtcNow);
        }

        protected virtual IExceptionToModelConverter GetExceptionToModelConverter()
        {
            return new ExceptionToModelConverter();
        }

        internal IEnumerable<Type> GetCommandTypes()
        {
            return CommandHandlerTypes
                .Select(commandHandlerType => commandHandlerType.GetInterfaceGenericTypeArguments(typeof(ICommandHandler<>))[0]);
        }
    }
}