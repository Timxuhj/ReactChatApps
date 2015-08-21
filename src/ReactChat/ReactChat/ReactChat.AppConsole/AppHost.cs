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

namespace ReactChat.AppConsole
{
    public class AppHost : AppSelfHostBase
    {
        /// <summary>
        /// Default constructor.
        /// Base constructor requires a name and assembly to locate web service classes. 
        /// </summary>
        public AppHost()
            : base("React_Chat_Gap.AppConsole", typeof(ServerEventsServices).Assembly)
        {

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
                LoadFromAssemblies = { typeof(CefResources).Assembly },
            });

            SetConfig(new HostConfig
            {
                DebugMode = true,
                EmbeddedResourceBaseTypes = { typeof(AppHost), typeof(CefResources) },
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
                    new TwitterAuthProvider(AppSettings),   //Sign-in with Twitter
                    new FacebookAuthProvider(AppSettings),  //Sign-in with Facebook
                    new GithubAuthProvider(AppSettings),    //Sign-in with GitHub OAuth Provider
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
            AppSettings.Set("oauth.RedirectUrl", Program.HostUrl);
            AppSettings.Set("oauth.CallbackUrl", Program.HostUrl + "auth/{0}");
            AppSettings.Set("oauth.twitter.ConsumerKey", "6APZQFxeVVLobXT2wRZArerg0");
            AppSettings.Set("oauth.twitter.ConsumerSecret", "bKwpp31AS90MUBw1s1w0pIIdYdVEdPLa1VvobUr7IXR762hdUn");
            //AppSettings.Set("RedisHost", "localhost:6379");
        }
    }
}
