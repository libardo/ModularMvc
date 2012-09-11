Modular.Mvc
==========

A helper library which allows segmenting ASP.NET MVC controllers and views by feature rather than type.

FAQ
---

__What does this mean, really?__

*The short story is that rathern than:*
<pre><code>
/Controllers
	/HomeController.cs
	/UsersController.cs
/Models
	/HomeViewModel.cs
	/UsersViewModel.cs
/Views
	/Home
		/Index.cshtml
	/Users
		/Index.cshtml
</code></pre>

*You can do this:*
<pre><code>
/Modules
	/Home
		/HomeController.cs
		/HomeViewModel.cs
		/Index.cshtml
	/Users
		/UsersController.cs
		/UsersViewModel.cs
		/Index.cshtml
</code></pre>

__Can I do my controller as usual?__

Pretty much, yes.

__How does it work?__

It loops all over your controllers and registers custom routes and view engines to support this convention.

__Does it register routes?__

Yes, it registers routes for all the controllers below the specified modules path. The route urls follows the folder structure when the structure becomes deeper.

__What happens if I move my controller?__

Things will stop working like you expect unless you update the controller's namespace.

Folder Structure
----------------

Use the following folder structure to work within the conventions, or see configure below.

<pre><code>
# By default the /modules folder is the root containing both controllers and views organized by feature
/Modules
	# By default the Home subpath is routed from the parent url
	/Home
		/HomeController.cs				-> /
		# Views are located int he same folder as the controller
		/Index.cshtml
	/Shared
		# Shared views are placed in the same folder structure
		_layout.cshtml
		_footer.cshtml
	/Shop
		# Home also works in subfolders
		/Home
			# The Shop controller prefix is optional but recommended since unique controller are much easier to deal with
			/[Shop]HomeController.cs	-> /Shop
			/Index.cshtml
		/Checkout
			# The routed path naturally follows the folder structure
			/ShopCheckoutController.cs	-> /Shop/Checkout
		/Shared
			# Shared views can be re-defined at a deeper level
			_footer.cshtml
	/Users
		/Manage
			/[Users]ManageController.cs	-> /Users/Manage
			/Index.cshtml
			# When multiple controllers share a folder the "other" ones are mapped to a subpath
			/EditController.cs			-> /Users/Manage/Edit
			/Other.cshtml
		/Password
			# A controller mustn't follow the folder naming conventions, 
			# when 1 controller it gets the folder path, when >1 they both get a subsegment
			/ResetController.cs			-> /Users/Password/Reset
			/ChangeController.cs		-> /Users/Password/Change
		# etc.
</code></pre>

Initialization
--------------

Call this code from Global.asax's Application_Start:

<pre><code>
    // The default means the calling assembly (this one) is scanned for controllers below the path /Modules
    Modular.Mvc.Initializer.Default
        // A custom route for EditUserController accepts userId as the last route segment
        .Route&lt;EditUserController&gt;("{userId}",
            defaults: new { userId = 0 },
            constraints: new { userId = "\\d" })
        // Performs the actual registration
        .Register(routes, viewEngines);
</code></pre>

Configuration
-------------

Before calling Register on the initialization fluent interface some aspects can be tweaked:

<pre><code>
Modular.Mvc.Initializer.Default..

	// Adds more route segments to the route for this particular controller
	.Route&lt;TController&gt;(url,defaults,constraints)
	
	// Adds more assemblies to scanned assemblies
	.AddAssemblies(assemblies)
	
	// Adds the calling assemblies to scanned assemblies (default when using Initializer.Default)
	.AddCallingAssembly()
	
	// Changes the path below which modules are registered (default = "Modules")
	.BelowPath(url)	
	
	// Allows for advanced configuration options
	.Configure((s) => expression)	
	
	// The path to route as the parent url (default = "Home")
	.PromoteControllerInSubpath(suburl)	
</code></pre>
