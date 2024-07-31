using System;
using System.Collections.Generic;
using System.Reflection;
using Emby.Drawing;
using Emby.Server.Implementations;
using Emby.Server.Implementations.Session;
using Jellyfin.Api.WebSocketListeners;
using Jellyfin.Drawing.Skia;
using Jellyfin.Server.Implementations;
using Jellyfin.Server.Implementations.Activity;
using Jellyfin.Server.Implementations.Devices;
using Jellyfin.Server.Implementations.Events;
using Jellyfin.Server.Implementations.Security;
using Jellyfin.Server.Implementations.Users;
using MediaBrowser.Controller;
using MediaBrowser.Controller.BaseItemManager;
using MediaBrowser.Controller.Devices;
using MediaBrowser.Controller.Drawing;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Lyrics;
using MediaBrowser.Controller.Net;
using MediaBrowser.Controller.Security;
using MediaBrowser.Model.Activity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Server
{
    /// <summary>
    /// Implementation of the abstract <see cref="ApplicationHost" /> class.
    /// </summary>
    public class CoreAppHost : ApplicationHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreAppHost" /> class.
        /// </summary>
        /// <param name="applicationPaths">The <see cref="ServerApplicationPaths" /> to be used by the <see cref="CoreAppHost" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" /> to be used by the <see cref="CoreAppHost" />.</param>
        /// <param name="options">The <see cref="StartupOptions" /> to be used by the <see cref="CoreAppHost" />.</param>
        /// <param name="startupConfig">The <see cref="IConfiguration" /> to be used by the <see cref="CoreAppHost" />.</param>
        public CoreAppHost(
            IServerApplicationPaths applicationPaths,
            ILoggerFactory loggerFactory,
            IStartupOptions options,
            IConfiguration startupConfig)
            : base(
                applicationPaths,
                loggerFactory,
                options,
                startupConfig)
        {
        }

        /// <inheritdoc/>
        protected override void RegisterServices(IServiceCollection serviceCollection)
        {
            // Register an image encoder
            bool useSkiaEncoder = SkiaEncoder.IsNativeLibAvailable();
            Type imageEncoderType = useSkiaEncoder
                ? typeof(SkiaEncoder)
                : typeof(NullImageEncoder);
            serviceCollection.AddSingleton(typeof(IImageEncoder), imageEncoderType);

            // Log a warning if the Skia encoder could not be used
            if (!useSkiaEncoder)
            {
                Logger.LogWarning("Skia not available. Will fallback to {ImageEncoder}.", nameof(NullImageEncoder));
            }

            serviceCollection.AddEventServices();
            serviceCollection.AddSingleton<IBaseItemManager, BaseItemManager>();
            serviceCollection.AddSingleton<IEventManager, EventManager>();

            serviceCollection.AddSingleton<IActivityManager, ActivityManager>();
            serviceCollection.AddSingleton<IUserManager, UserManager>();
            serviceCollection.AddScoped<IDisplayPreferencesManager, DisplayPreferencesManager>();
            serviceCollection.AddSingleton<IDeviceManager, DeviceManager>();

            // TODO search the assemblies instead of adding them manually?
            serviceCollection.AddSingleton<IWebSocketListener, SessionWebSocketListener>();
            serviceCollection.AddSingleton<IWebSocketListener, ActivityLogWebSocketListener>();
            serviceCollection.AddSingleton<IWebSocketListener, ScheduledTasksWebSocketListener>();
            serviceCollection.AddSingleton<IWebSocketListener, SessionInfoWebSocketListener>();

            serviceCollection.AddSingleton<IAuthorizationContext, AuthorizationContext>();

            serviceCollection.AddScoped<IAuthenticationManager, AuthenticationManager>();

            foreach (var type in GetExportTypes<ILyricProvider>())
            {
                serviceCollection.AddSingleton(typeof(ILyricProvider), type);
            }

            base.RegisterServices(serviceCollection);
        }

        /// <inheritdoc />
        protected override void RestartInternal() => Program.Restart();

        /// <inheritdoc />
        protected override IEnumerable<Assembly> GetAssembliesWithPartsInternal()
        {
            // Jellyfin.Server
            yield return typeof(CoreAppHost).Assembly;

            // Jellyfin.Server.Implementations
            yield return typeof(JellyfinDb).Assembly;
        }

        /// <inheritdoc />
        protected override void ShutdownInternal() => Program.Shutdown();
    }
}
