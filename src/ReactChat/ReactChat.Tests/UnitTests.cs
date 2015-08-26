using System;
using System.Linq;
using NUnit.Framework;
using ReactChat.ServiceInterface;
using ReactChat.ServiceModel;
using ServiceStack;
using ServiceStack.Testing;
using ServiceStack.Auth;

namespace ReactChat.Tests
{
    [TestFixture]
    public class UnitTests
    {
        private readonly ServiceStackHost appHost;

        public UnitTests()
        {
            appHost = new BasicAppHost(typeof (ServerEventsServices).Assembly)
            {
                ConfigureContainer = container =>
                {
                    //Add your IoC dependencies here
                    container.RegisterAutoWiredAs<MemoryChatHistory, IChatHistory>();
                    appHost.Plugins.Add(new ServerEventsFeature());
                    
                }
            };
            appHost.Init();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
        }

        [Test]
        public void TestMethod1()
        {
            var service = appHost.Container.Resolve<ServerEventsServices>();

            var serverEvents = appHost.Container.Resolve<IServerEvents>() as MemoryServerEvents;
            serverEvents.Register(new EventSubscription(new MockHttpResponse())
            {
                CreatedAt = DateTime.Now,
                LastPulseAt = DateTime.Now,
                Channels = new []{"home"},
                SubscriptionId = "FooSub",
                UserId = "Foo",
                UserName = null,
                SessionId = Guid.NewGuid().ToString(),
                IsAuthenticated = false,
                UserAddress = "",
                OnPublish = null,
                Meta = {
                    { "userId", "Foo" },
                    { "channels", string.Join(",", "home") },
                    { AuthMetadataProvider.ProfileUrlKey, AuthMetadataProvider.DefaultNoProfileImgUrl },
                }
            });

            service.Any(new PostChatToChannel
            {
                Channel = "home",
                From = "FooSub",
                Message = "Hello, World!",
                Selector = "message",
                ToUserId = null
            });

            var response = (GetChatHistoryResponse)service.Any(new GetChatHistory { Channels = new[] { "home" } });

            Assert.That(response.Results.First().Message, Is.EqualTo("Hello, World!"));
        }
    }
}
