using Funq;
using ServiceStack;
using ServiceStack.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ReactChat.Resources;
using ReactChat.ServiceInterface;
using ServiceStack.Auth;
using ServiceStack.Redis;
using ServiceStack.Text;
using System.IO;
using ServiceStack.Configuration;

namespace ReactChat.AppConsole
{
    public class AppHost : AppSelfHostBase
    {
        /// <summary>
        /// Default constructor.
        /// Base constructor requires a name and assembly to locate web service classes. 
        /// </summary>
        public AppHost()
            : base("ReactChat.AppConsole", typeof(ServerEventsServices).Assembly)
        {
            var customSettings = new FileInfo("appsettings.txt");
            AppSettings = customSettings.Exists
                ? (IAppSettings)new TextFileSettings(customSettings.FullName)
                : new AppSettings();
        }

        /// <summary>
        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        /// </summary>
        /// <param name="container"></param>
        public override void Configure(Container container)
        {
            //Config examples
            //this.Plugins.Add(new PostmanFeature());
            //Plugins.Add(new CorsFeature());

            InitializeAppSettings();

            Plugins.Add(new RazorFormat
            {
                LoadFromAssemblies = { typeof(CefResources).Assembly }
            });

            SetConfig(new HostConfig
            {
                DebugMode = true,
                EmbeddedResourceBaseTypes = { typeof(AppHost), typeof(CefResources) }
            });

            JsConfig.EmitCamelCaseNames = true;

            Plugins.Add(new RazorFormat());
            Plugins.Add(new ServerEventsFeature());

            MimeTypes.ExtensionMimeTypes["jsv"] = "text/jsv";

            SetConfig(new HostConfig
            {
                DebugMode = AppSettings.Get("DebugMode", false),
                DefaultContentType = MimeTypes.Json,
                AllowFileExtensions = { "jsx" },
            });

            CustomErrorHttpHandlers.Remove(HttpStatusCode.Forbidden);

            //Register all Authentication methods you want to enable for this web app.            
            Plugins.Add(new AuthFeature(
                () => new AuthUserSession(),
                new IAuthProvider[] {
                    new TwitterAuthProvider(AppSettings)   //Sign-in with Twitter
                }));

            container.RegisterAutoWiredAs<MemoryChatHistory, IChatHistory>();

            var redisHost = AppSettings.GetString("RedisHost");
            if (redisHost != null)
            {
                container.Register<IRedisClientsManager>(new RedisManagerPool(redisHost));

                container.Register<IServerEvents>(c =>
                    new RedisServerEvents(c.Resolve<IRedisClientsManager>()));
                container.Resolve<IServerEvents>().Start();
            }
        }

        private void InitializeAppSettings()
        {
            var allKeys = AppSettings.GetAllKeys();
            if(!allKeys.Contains("oauth.RedirectUrl"))
                AppSettings.Set("oauth.RedirectUrl", Program.HostUrl);
            if(!allKeys.Contains("oauth.CallbackUrl"))
                AppSettings.Set("oauth.CallbackUrl", Program.HostUrl + "auth/{0}");
            if(!allKeys.Contains("oauth.twitter.ConsumerKey"))
                AppSettings.Set("oauth.twitter.ConsumerKey", "6APZQFxeVVLobXT2wRZArerg0");
            if (!allKeys.Contains("oauth.twitter.ConsumerSecret"))
                AppSettings.Set("oauth.twitter.ConsumerSecret", "bKwpp31AS90MUBw1s1w0pIIdYdVEdPLa1VvobUr7IXR762hdUn");
            //if (!allKeys.Contains("RedisHost"))
            //    AppSettings.Set("RedisHost", "localhost:6379");
        }
    }
}
