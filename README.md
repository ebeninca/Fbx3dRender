# Fbx3dRenderSmall

## To build

- Visual Studio 2019
- Unity 2022.3.10f1

### Install Packages (Window >> Package Manager)

- FbxExporter
- ObjLoader (source code already in this project, probably not necessary to install - https://assetstore.unity.com/packages/tools/modeling/runtime-obj-importer-49547)

### Project Settings (Edit >> Project Settings)

- Graphics >> Always Include Shaders >> Element 6 >> "Standard"
- Graphics >> Always Include Shaders >> Element 7 >> "Standard (Specular setup)"
- Player Settings > Optimization > Api Compability Level > .NET Framework

### Including the package in a build

By default, Unity does not include this package in builds, you can only use it in the Editor. However, it is possible to use this package at runtime on some specific platforms.

Note: You can currently use the package in Windows/OSX/Linux standalone builds only.

To include the Autodesk® FBX® SDK for Unity package in your build:

    In the Unity Editor main menu, select Edit > Project Settings.
    In Player properties, expand the Other Settings section.
    Under Configuration, in the Scripting Define Symbols field, add FBXSDK_RUNTIME.

### Remove development build watermark

- File >> Build Settings >> Uncheck Development build

### To generate for Visual Studio

- File >> Build settings >> Create Visual Studio Solution

### After Build to use in CM17 

- Visual studio option:
Copy and replace the generated code inside CM17, inside VS2019 select the "Master" build option.

- Binary option:
You'll need to move the files in the output folder you defined in the first build (usually "bin" folder) to the CM17 Fbx3dRenderBuild Folder.

## TODO list

- Support for other types of models 

## Changelog

v003
 - Light direction adjusted
 - TCPListener to allow communication with winform after start
 
v002
 - MouseScroll to change FOV
 - Print FOV on screen
 - MouseDrag right button to zoom in and out
 - Double click reset position, zoom and fov
 - FOV adjusted to default 20
 - Solid light gray background for MakeAutoTransparent work on photo mode
 - Added CapsuleCollider and Event Trigger


v001
 - Ball rendering succesfully
 - Script converting FBX to OBJ and rendering with textures at runtime
