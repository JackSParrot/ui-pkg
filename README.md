# es.jacksparrot.commands unity package

## Dependencies
You need to import the following packages before importing this package
1. https://github.com/JackSParrot/utils-pkg
2. https://github.com/JackSParrot/json-pkg

## Installation using the Unity Package Manager (Unity 2019.3+)
1. Open the Package Manager Window. 
2. Click on the + icon on the top left.
3. Select "Add package from git URL...".
4. In the imput field enter this: "https://github.com/JackSParrot/commands-pkg.git"
5. Click on add and wait while the package is downloaded and imported.

## Installation using the Unity Package Manager (Unity 2018.1+)

The [Unity Package Manager](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html) (UPM) is a new method to manage external packages. It keeps package contents separate from your main project files.

1. Modify your project's `Packages/manifest.json` file adding this line:

    ```json
    "es.jacksparrot.commands": "https://github.com/JackSParrot/commands-pkg.git"
    ```

    Make sure it's still a valid JSON file. For example:

    ```json
    {
        "dependencies": {
            "com.unity.package-manager-ui": "2.2.0",
            "es.jacksparrot.commands": "https://github.com/JackSParrot/commands-pkg.git"
        }
    }
    ```

2. To update the package you need to delete the package lock entry in the `lock` section in `Packages/manifest.json`. The entry to delete could look like this:

    ```json
    "es.jacksparrot.commands": {
      "hash": "a7ffd9287ac3c0ce1c68204873d24e540b88940d",
      "revision": "HEAD"
    }
    ```
