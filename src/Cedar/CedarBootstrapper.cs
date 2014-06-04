﻿namespace Cedar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Cedar.CommandHandling;
    using Cedar.CommandHandling.Serialization;
    using Cedar.Hosting;
    using Nancy;

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

        /// <summary>
        ///     Gets the command module path.
        /// </summary>
        /// <value>
        ///     The command module path. Default value is '/commands'
        /// </value>
        public virtual string CommandModulePath
        {
            get
            {
                return "/commands";
            }
        }

        /// <summary>
        ///     Gets the nancy modules types.
        /// </summary>
        /// <value>
        ///     The nancy modules types.
        /// </value>
        public virtual IEnumerable<Type> NancyModulesTypes
        {
            get { return AssembliesToScan.SelectMany(assembly => assembly.GetImplementorsOfInterface<INancyModule>()); }
        }

        /// <summary>
        ///     Gets the command deserializers.
        /// </summary>
        /// <value>
        ///     The command deserializers.
        /// </value>
        public virtual IEnumerable<Type> CommandDeserializers
        {
            get { return AssembliesToScan.SelectMany(assembly => assembly.GetImplementorsOfInterface<ICommandDeserializer>()); }
        }

        public virtual void Initialize()
        {
            
        }
    }
}