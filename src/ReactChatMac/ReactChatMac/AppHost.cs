using System;
using ServiceStack;
using Funq;
using ServiceStack.Razor;
using ReactChat.ServiceInterface;
using ReactChat.Resources;
using System.Reflection;
using ServiceStack.Text;
using System.Net;
using ServiceStack.Auth;
using ServiceStack.Redis;
using MonoMac.AppKit;
using System.Linq;

namespace ReactChatMac
{
	public class AppHost : AppSelfHostBase
	{
		/// <summary>
		/// Default constructor.
		/// Base constructor requires a name and assembly to locate web service classes. 
		/// </summary>
		public AppHost ()
			: base ("ReactChatMac", typeof(ServerEventsServices).Assembly)
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

			// This route is added using Routes.Add and ServiceController.RegisterService due to
			// using ILMerge limiting our AppHost : base() call to one assembly.
			// If two assemblies are used, the base() call searchs the same assembly twice due to the ILMerged result.
			Routes.Add<NativeHostAction>("/nativehost/{Action}");
			ServiceController.RegisterService(typeof(NativeHostService));
		}

		private void InitializeAppSettings()
		{
			var allKeys = AppSettings.GetAllKeys();
			if (!allKeys.Contains("platformsClassName"))
				AppSettings.Set("platformsClassName", "console");
			if (!allKeys.Contains("PlatformCss"))
				AppSettings.Set("PlatformCss", "mac.css");
			if (!allKeys.Contains("PlatformJs"))
				AppSettings.Set("PlatformJs", "mac.js");

			if(!allKeys.Contains("oauth.RedirectUrl"))
				AppSettings.Set("oauth.RedirectUrl", MainClass.HostUrl);
			if(!allKeys.Contains("oauth.CallbackUrl"))
				AppSettings.Set("oauth.CallbackUrl", MainClass.HostUrl + "auth/{0}");
			if(!allKeys.Contains("oauth.twitter.ConsumerKey"))
				AppSettings.Set("oauth.twitter.ConsumerKey", "6APZQFxeVVLobXT2wRZArerg0");
			if (!allKeys.Contains("oauth.twitter.ConsumerSecret"))
				AppSettings.Set("oauth.twitter.ConsumerSecret", "bKwpp31AS90MUBw1s1w0pIIdYdVEdPLa1VvobUr7IXR762hdUn");
			//if (!allKeys.Contains("RedisHost"))
			//    AppSettings.Set("RedisHost", "localhost:6379");
		}
	}

	public class NativeHostService : Service
	{
		public object Get(NativeHostAction request)
		{
			if (string.IsNullOrEmpty(request.Action)) {
				throw HttpError.NotFound ("Function Not Found");
			}
			Type nativeHostType = typeof(NativeHost);
			object nativeHost = nativeHostType.CreateInstance<NativeHost>();
			string methodName = request.Action.First ().ToString ().ToUpper () + String.Join ("", request.Action.Skip (1));
			MethodInfo methodInfo = nativeHostType.GetMethod(methodName);
			if (methodInfo == null)
			{
				throw new HttpError(HttpStatusCode.NotFound,"Function Not Found");
			}
			methodInfo.Invoke(nativeHost, null);
			return null;
		}
	}

	public class NativeHostAction : IReturnVoid
	{
		public string Action { get; set; }
	}

	public class NativeHost
	{
		public void ShowAbout()
		{
			//Invoke native about menu item programmatically.
			MainClass.MainMenu.InvokeOnMainThread (() => {
				foreach (var item in MainClass.MainMenu.ItemArray()) {
					if (item.Title == "ReactChatMac") {
						item.Submenu.PerformActionForItem(0);
					}
				}
			});
		}

		public void Quit()
		{
			Environment.Exit(0);
		}
	}
}


