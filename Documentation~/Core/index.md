# Systems framework (Core module)

This is the core package for building applications with the "Systems framework".

## How to install

This module ships inside `Virtuademy-SystemCore`; install that package (see [the package index](../index.md)).

Alternatively, if you need to modify the content of the package, import it as a submodule under the `Packages` folder.

External dependencies:

- [Odin Inspector](https://odininspector.com/)

## How to use

First, add the script `SystemsControllerManager` to a `GameObject` in the scene.
To use sa system, create the `ScriptableObject` asset containing its configuration using the toolbar menu Assets > Create > Virtuademy > name of the package > name of the system.
Reference the generated asset in the `SystemsControllerManager`: it will be used to instantiate the system and setup it with the provided configuration.

## Documentation importer

To visualize better the structure of Virtuademy packages, class diagrams have been added to the Visual Studio projects (.csproj).
There class diagrams are made with the Class Designer component of Visual Studio.
Unfortunately, Unity regenerates these projects automatically, removing the files/folders it does not recognize.
To restore these files and make them visible from Visual Studio, press the menu Virtuademy > Import Reflectis documentation (the command keeps its name: it filters the real `Reflectis.*` assembly files)
