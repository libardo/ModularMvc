Modular.Mvc
==========

A helper library which allows segmenting ASP.NET MVC controllers and views by feature rather than type


Folder Structure
----------------

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


Initialization
--------------

Call this code from Global.asax's Application_Start:

<pre><code>
    // The default means the calling assembly (this one) is scanned for controllers below the path /Modules
    Modular.Mvc.Initializer.Default
        // A custom route for EditUserController accepts userId as the last route segment
        .Route<EditUserController>("{userId}",
            defaults: new { userId = 0 },
            constraints: new { userId = "\\d" })
        // Performs the actual registration
        .Register(routes, viewEngines);
</code></pre>

Configuration
-------------

Before Register is called on the initialization fluent interface some aspects can be tweaked:

*.Route<TController>(url,defaults,constraints)*: Adds more route segments to the route for this particular controller
*.AddAssemblies(assemblies)*: Adds more assemblies to scanned assemblies
*.AddCallingAssembly()*: Adds the calling assemblies to scanned assemblies (default when using Initializer.Default)
*.BelowPath(url)*: Changes the path below which modules are registered (default = "Modules")
*.Configure(expression)*: Allows for advanced configuration options
*.PromoteControllerInSubpath(suburl)*: The path to route as the parent url (default = "Home")
