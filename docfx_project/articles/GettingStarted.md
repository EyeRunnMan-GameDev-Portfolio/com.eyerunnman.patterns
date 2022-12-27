# Installation

Follow the steps below to import this unity package (upm - unity package manager) in your own unity projects.

1.  Go to **Edit > Project Settings > Package manager**
2.  give a suitable package name
3.  enter this url in the registry url field `https://registry.npmjs.org/`
4.  add scope `com.eyerunnman.<pacakge name>`

![](../images/GettingStarted.png)

# Get Documentation for **vX.X.X**

**NOTE:** only the docs for latest package version versions is supported.

But there is a workaround follow the steps below to get documentation for **vX.X.X** .

1.  clone the whole repo in your local machine using `git clone https://github.com/EyeRunnMan-GameDev-Portfolio/<package you want the docs for>.git`.
2.  fetch all tags and checkout your specific tag for the **v.X.X.X**.
3.  install `docfx` [follow the steps here](https://dotnet.github.io/docfx/tutorial/docfx_getting_started.html#2-use-docfx-as-a-command-line-tool).
4.  run `docfx ./docfx_project/docfx.json --serve` in your terminal.
5.  🎉 enjoy your documentation for **vX.X.X**
