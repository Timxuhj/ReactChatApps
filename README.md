# React Chat Apps
A chat demo of building a cross-platform application using the `React Desktop Apps` template from the ServiceStackVS extension. In this demo we've ported the [Chat-React demo application](https://github.com/ServiceStackApps/Chat-React) to the React Desktop Apps template to take advantage of [CefSharp](https://github.com/cefsharp/CefSharp) to help build a multi-platform native desktop single page app with ServiceStack.

## Optimal Development and Deployment workflow

Fast dev iterations are one of the immediate benefits when developing JavaScript-based Apps, made possible since you're editing the same plain text files browsers execute, so they get quickly rendered after each refresh without needing to wait for the rebuilding of VS.NET projects or ASP.NET's AppDomain to restart. 

> This fast dev cycle also extends to [ServiceStack Razor](http://razor.servicestack.net/) dynamic server pages which supports live-reloading of modified `.cshtml` Razor Views so they're view-able on-the-fly without an AppDomain restart.

For minimal friction, the Gulp/Grunt build system takes a non-invasive approach that works around normal web dev practices of being able to reference external css, js files - retaining the development experience of a normal static html website where any changes to html, js or css files are instantly visible after a refresh. 

Then to package your app for optimal deployment to production, [Gulp's useref](https://www.npmjs.org/package/gulp-useref) plugin lets you annotate existing references with how you want them bundled. This is ideal as the existing external references (and their ordering) remains the master source for your Apps dependencies, reducing the maintenance and friction required in developing and packaging optimized Single Page Apps.

We can look at React Chat's dependencies to see how this looks:

```html
<!--build:css css/app.min.css-->
<link rel="stylesheet" href="css/app.css" />
<!-- endbuild -->
<!-- build:js lib/js/jquery.min.js -->
<script src="bower_components/jquery/dist/jquery.js"></script>
<!-- endbuild -->
<!-- build:js lib/js/react.min.js -->
<script src="bower_components/react/react.js"></script>
<!-- endbuild -->
<!-- build:js lib/js/reflux.min.js -->
<script src="bower_components/reflux/dist/reflux.js"></script>
<!-- endbuild -->
<!-- build:remove -->
<script src="bower_components/react/JSXTransformer.js"></script>
<!-- endbuild -->
<script src="js/ss-utils.js"></script>
...
<!-- build:js js/app.jsx.js -->
<script type="text/jsx" src="js/components/Actions.js"></script>
<script type="text/jsx" src="js/components/User.jsx"></script>
<script type="text/jsx" src="js/components/Header.jsx"></script>
<script type="text/jsx" src="js/components/Sidebar.jsx"></script>
<script type="text/jsx" src="js/components/ChatLog.jsx"></script>
<script type="text/jsx" src="js/components/Footer.jsx"></script>
<script type="text/jsx" src="js/components/ChatApp.jsx"></script>
<!-- endbuild -->
```

During development the HTML comments are ignored and React Chat runs like a normal static html website. Then when packaging the client app for deployment (i.e. by running the `03-package-client` task), the build annotations instructs Gulp on how to package and optimize the app ready for production.

As seen in the above example, each build instruction can span one or multiple references of the same type and optionally specify the target filename to write the compressed and minified output to.

### Design-time only resources

Gulp also supports design-time vs run-time dependencies with the `build:remove` task which can be used to remove any unnecessary dependencies not required in production like react's `JSXTransformer.js`:

```html
<!-- build:remove -->
<script src="bower_components/react/JSXTransformer.js"></script>
<!-- endbuild -->
```

React's `JSXTransformer.js` is what enables the optimal experience of letting you directly reference `.jsx` files in HTML as if they were normal `.js` files by transpiling and executing `.jsx` files directly in the browser at runtime - avoiding the need for any manual pre-compilation steps and retaining the fast `F5` reload cycle that we've come to expect from editing `.js` files. 

```html
<!-- build:js js/app.jsx.js -->
<script type="text/jsx" src="js/components/Actions.js"></script>
<script type="text/jsx" src="js/components/User.jsx"></script>
<script type="text/jsx" src="js/components/Header.jsx"></script>
<script type="text/jsx" src="js/components/Sidebar.jsx"></script>
<script type="text/jsx" src="js/components/ChatLog.jsx"></script>
<script type="text/jsx" src="js/components/Footer.jsx"></script>
<script type="text/jsx" src="js/components/ChatApp.jsx"></script>
<!-- endbuild -->
```

Then when the client app is packaged, all `.jsx` files are compiled and minified into a single `/js/app.jsx.js` with the reference to `JSXTransformer.js` also stripped from the optimized HTML page as there's no longer any need to transpile and execute `.jsx` files at runtime.

**For more info on working with React, see the [Chat-React project](https://github.com/ServiceStackApps/Chat-React#introducing-reactjs) documentation.** 

# Project Structure
Just like other templates in ServiceStackVS, the **React Desktop Apps** template provides the same recommended structure as well as 3 additional other projects for producing the Console and WinForms applications.

![](https://github.com/ServiceStack/Assets/raw/master/img/servicestackvs/react-desktop-apps-proj-structure.png)

- **ReactChat** - Web applicaton which contains all our resources and files used while developing.
- **ReactChat.AppConsole*** - Console application, launches default browser on users application
- **ReactChat.AppWinForms*** - WinForms application using CefSharp and Chromium Embedded Framework to output our web application in a native application.
- **ReactChat.Resources*** - Embedded resources that are used by our AppWinForms and AppConsole application and target of `01-bundle-all` Grunt task. This project has references to all minified client resources (CSS, JavaScript, images, etc) and includes each of them as an *Embedded Resource*.
- **ReactChat.ServiceInterface** - Contains ServiceStack services.
- **ReactChat.ServiceModel** - Contains request/response classes.
- **ReactChat.Tests** - Contains NUnit tests. 


#### ReactChat Project
This project contains all our development resources, JS/JSX, CSS, images, Razor views, etc. This project also has all the required Grunt/Gulp tasks used for deploying the 3 application outputs. Taking advantage of Visual Studio 2015's Task Runner Explorer, we can look at the `Alias` tasks to get an idea of how we can build and deploy our console, winforms and web application.

![](https://github.com/ServiceStack/Assets/raw/master/img/servicestackvs/react-desktop-apps-task-runner.png)

- **default** - grunt task builds and packages both the console and winforms projects by running `02-package-console` and `03-package-winforms`.
- [**01-bundle-all**](#01-bundle-all) - bundles all the application resources into the `Resources` project and into `wwwroot` to stage the web application for deployment
- [**02-package-console**](#02-package-console) - bundles and packages the console application and produces the result in `wwwroot_build\apps` directory.
- [**03-package-winforms**](#03-package-winforms) - bundles and packages the winforms application and produces the result in `wwwroot_build\apps` directory.
- [**04-deploy-webapp**](#04-deploy-webapp) - bundles, packages and deploys the web application using the `wwwroot_build\publish\config.json` file settings and webdeploy to your existing IIS server.

This project also has includes ILMerge and 7zip tools to help package the console and winforms application ready for release. The `wwwroot_build` folder contains the following structure.

The `/wwwroot_build` folder contains the necessary files required for deployments including:

```
/wwwroot_build
  /apps                       # output directory of console and winforms applications
  /deploy                     # copies all files in folder to /wwwroot
    appsettings.txt           # production appsettings to override dev defaults
  /publish                    
    config.json               # deployment config for WebDeploy deployments
  /tools                      # deployment tools for console and winforms applications
    7za.exe                   # 7zip console for packaging
    7zsd_All.sfx	          # 7zip Self Extract module used for bundling winforms app to self executing zip
    ILMerge.exe			      # ILMerge to merge console app output into single binary
  00-install-dependencies     # runs NPM install and bower install, used when getting started after cloning application
  config-winforms.txt         # 7zip SFX config for self executing zip
  package-deploy-console.bat  # runs ILMerge to package the console application
  package-deploy-winforms     # stagings winforms app and packages using 7zip SFX and config-winforms.txt
```

The minimum steps to deploy an app is to fill in `config.json` with the remote IIS WebSite settings as well as a UserName and Password of a User that has permission to remote deploy an app:

```json
{
    "iisApp": "AppName",
    "serverAddress": "deploy-server.example.com",
    "userName": "{WebDeployUserName}",
    "password" : "{WebDeployPassword}"
}
```

#### ReactChat.AppConsole
This project is for producing a SelfHost ServiceStack application that utilizes the user's default browser. Combined with the Grunt/Gulp and ILMerge, we can produce a cross-platform, single executable that has embedded resources used by our application.

This project uses the bundled resources from the web application that are bundled using the Grunt/Gulp tasks. These resources are embedded in the `ReactChat.Resources` and the AppHost needs to be configured to look for these embedded resources. For the compiled Razor views, we use the following configuration for our `RazorFormat` plugin.

``` csharp
Plugins.Add(new RazorFormat
{
    LoadFromAssemblies = { typeof(CefResources).Assembly }
});
```

`CefResources` is a class in the `ReactChat.Resources` project so we can easily refer to it's assembly with `typeof(CefResources).Assembly`. 

For our other resources, we need to set the `EmbeddedResourceBaseTypes` to both our current project and the `ReactChat.Resources` using the `CefResources` type.

```
SetConfig(new HostConfig
{
    EmbeddedResourceBaseTypes = { typeof(AppHost), typeof(CefResources) }
});
```

>We need to specify base types instead of assemblies so their namespaces are preserved once they're ILMerged into a single .exe

#### ReactChat.AppWinForms
This project utilizes the CefSharp project for embedding a high performing Chromium browser in a WinForms application. This project, also uses the bundled resources from the web application via the `ReactChat.Resources` project as well being a `AppSelfHostBase` based application, we need to set the same config as our `ReactChat.AppConsole` application in the AppHost.

``` csharp
Plugins.Add(new RazorFormat
{
    LoadFromAssemblies = { typeof(CefResources).Assembly }
});

SetConfig(new HostConfig
{
    EmbeddedResourceBaseTypes = { typeof(AppHost), typeof(CefResources) }
});
```

To embed the Chromium web browser, we reference the `CefSharp.WinForms` project and instantiate a `ChromiumWebBrowser` specifying the applications URL, in this case `http://localhost:1337/`. When using `CefSharp.WinForms` reference, `ChromiumWebBrowser` is a WinForms control that is added to our Form. We also bind `FormClosing`, `FormClosed` and `Load` WinForms events to give the application more of a native feel.

```csharp
public FormMain()
{
    InitializeComponent();
    VerticalScroll.Visible = false;
    var chromiumBrowser = new ChromiumWebBrowser(Program.HostUrl)
    {
        Dock = DockStyle.Fill
    };
    Controls.Add(chromiumBrowser);

    Load += (sender, args) =>
    {
        FormBorderStyle = FormBorderStyle.None;
        Left = Top = 0;
        Width = Screen.PrimaryScreen.WorkingArea.Width;
        Height = Screen.PrimaryScreen.WorkingArea.Height;
    };

    FormClosing += (sender, args) =>
    {
        //Make closing feel more responsive.
        Visible = false;
    };

    FormClosed += (sender, args) =>
    {
        Cef.Shutdown();
    };

    chromiumBrowser.RegisterJsObject("aboutDialog", new AboutDialogJsObject());
    chromiumBrowser.RegisterJsObject("winForm",new WinFormsApp(this));
}
```

CefSharp also enabled integration between JavaScript and native calls via exposing JavaScript objects that are registered .NET classes. In ReactChat and the ServiceStackVS template, we wire up 2 objects to show how this can be leveraged. One to simply show a message box when "About" is clicked and the other to close the application. The .NET classes are POCOs that have matching function names with the JavaScript object registered. The default setting is to camel case the JS object following the common naming conventions when using JS.

```csharp
public class AboutDialogJsObject
{
    public void Show()
    {
        MessageBox.Show(@"ServiceStack with CefSharp + ReactJS", @"ReactChat.AppWinForms", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

public class WinFormsApp
{
    public FormMain Form { get; set; }

    public WinFormsApp(FormMain form)
    {
        Form = form;
    }

    public void Close()
    {
        Form.InvokeOnUiThreadIfRequired(() =>
        {
            Form.Close();  
        });
    }
}
```

In our ReactChat web application, we create an object as a placeholder or web equivalent so that function calls will work in both a web app and when using CefSharp to enable these native hooks.

``` javascript
    //Show about function for local dev, hooks into winforms when depoyed.
    window.aboutDialog = window.aboutDialog || {
        show: function() {
            alert("ReactChat - ServiceStack + ReactJS");
        }
    };
    window.winForm = window.winForm || {
        close: function() {
            window.close();
        }
    }
```

If CefSharp is being used, these objects are registered before page is rendered and the native hooks will be used instead.

#### ReactChat.Resources
This project has references to the output files from the `01-bundle-all` Grunt task. If any additional images or minified JS/CSS files are added to your project, they must be referenced by this project to be included as an embedded resource for use in both AppConsole and AppWinForms projects. The structure of the project follows what is deployed in the `wwwroot` project.

```
/wwwroot
  /css
    app.min.css
  /img              #  all application images
  /js
    app.jsx.js
  /lib
    /css            # 3rd party css, eg bootstrap
    /fonts          # 3rd party fonts
    /js             # 3rd party minified JS
      bootstrap.min.js
      jquery.min.js
      modernizr.min.js
      react.min.js
      reflux.min.js
  default.cshtml
```

All files have a `Build Action` of `Embedded Resource` so they are ready to be used from AppConsole and AppWinForms.

![](https://github.com/ServiceStack/Assets/raw/master/img/servicestackvs/react-desktop-apps-embedded-resource.png)

# Grunt Tasks
Grunt and Gulp are used in the ReactChat project to automate our bundling, packaging and deployment of the applications. These tasks are declared as small, single responsibility Grunt tasks and then orchastrated using Alias tasks to be able to run these simply either from Visual Studio using the Task Runner Explorer or from the command line.

#### 01-bundle-all
Just like the AngularJS and React App template, we stage our application ready for release and avoid any build steps at development time to improve the simplicity and speed of the development workflow. This alias task is made up of small, simple tasks that use Gulp to process resources and perform tasks like minification, JSX transformation, copying/deleting of resources, etc.

The bundling searches for assets in any `*.cshtml` file and follows build comments to minify and replace references. This enables simple use of debug JS files whilst still having control how our resources minify.

```html
<!-- build:js lib/js/react.min.js -->
<script src="bower_components/react/react.js"></script>
<!-- endbuild -->
<!-- build:js lib/js/reflux.min.js -->
<script src="bower_components/reflux/dist/reflux.js"></script>
<!-- endbuild -->
<!-- build:remove -->
<script src="bower_components/react/JSXTransformer.js"></script>
<!-- endbuild -->

<!-- build:js js/app.jsx.js -->
<script type="text/javascript" src="js/components/Actions.js"></script>
<script type="text/jsx" src="js/components/User.jsx">
</script>
<script type="text/jsx" src="js/components/Header.jsx">
</script>
<script type="text/jsx" src="js/components/Sidebar.jsx">
</script>
<script type="text/jsx" src="js/components/ChatLog.jsx">
</script>
<script type="text/jsx" src="js/components/Footer.jsx">
</script>
<script type="text/jsx" src="js/components/ChatApp.jsx">
</script>
<!-- endbuild -->
```

When creating new JS files for your application, they should be added in the `build:js js/app.jsx.js` comments shown above. `build:remove` is used to remove the use of the runtime JSX transformer that we use for our React components, but is not longer needed ([and recommended not to be used in a production environment](https://facebook.github.io/react/docs/tooling-integration.html)) in our deployed application.

#### 02-package-console
This task also performs `01-build-all` as well restoring NuGet packages and building the **AppConsole** project. Once the project resources are ready, it calls the `package-deploy-console.bat` batch file which, using **ILMerge**, produces the stand alone exe of the console application. 

#### 03-package-winforms

#### 04-deploy-webapp



# Working with Redis
- Though these apps are hosted individually, when each of them connect to a shared Redis instance, this app can be used as a hosted web app, self hosted console app or CefSharp application chatting to different users regardless of client type.
- Separate AppHost configuration.
  - Web can load from appsettings.txt deployed using the Grunt `04-deploy-webapp` task
  - WinForms can load from app.config that is deployed with the application in the self executable zip.
  - Console can have these configurations set directly to the `AppSettings` in code if deployed as an ILMerged executable, or loaded from either accompanying app.config or appsettings.txt file if it's packaged.


