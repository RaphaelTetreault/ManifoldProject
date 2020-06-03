# Importing GMA Models

This guides demonstrates how to import model assets from <u>*F-Zero GX*</u> and <u>*F-Zero AX*</u> (hereafter GFZ). 



## Revisions Table

| Author            | Date       | Version | Description      |
| ----------------- | ---------- | ------- | ---------------- |
| Raphaël Tétreault | 2020/06/01 | 1.0     | Initial document |



## Table Of Contents

[TOC]

# Part 0: General Setup

Make sure you have consulted the **Managing Untracked Files** heading under **Preparing Your Files in Unity** in [README.md](README.md).



# Part 1: Import GMAs

The first step to getting GFZ models in Manifold is to load the files as `ScriptableObject`s. Manifold has definitions for many files formats where the file is imported non-destructively as a data asset. This will become useful for Part 2 when we reconstruct the model by iterating over the data inside the asset.



## 1.1 Create a GMA Importer object

To import the assets, we need to create an instance of the importer. Right-click in the Project panel in Unity to bring up the context menu. At the top, select `Create > Manifold > GFZX01 > GMA Model > GMA Importer`.

![](res\gma\gma-importer-1.jpg)

A new asset called `New GMA Importer` will be created. Place it inside the `Untracked` folder.

![](res\gma\gma-importer-2.jpg)



## 1.2 Set Import Parameters

### 1.2.1 Set Import From

We need to tell the importer where we will import the data from. The **Import From** parameter tells the program where to look for files. Press the `Browse` button to bring up the folder menu and select the corresponding folder to use. Here, we are selecting the root folder of the extracted GFZJ01 ISO. This will fill the field with the folder's path. In this screenshot, that path is `C:/GFZJ01`.

![](res\gma\gma-importer-3.jpg)



### 1.2.2 Set Import To

We also need to tell the importer where we will save our Unity assets to. The **Import to** parameter tells the program where to place the imported files. The path is relative to Unity's `Assets/`, so `Assets/GFZJ01` is written in **Import From** simply as `GFZJ01`. Here, we are telling the program to place our imported files into a folder called `GFZJ01`.

**NOTE**: the folders `GFZE01`, `GFZJ01`, `GFZP01`, and `GFZJ8P` are specially marked folders ignored by git in the Manifold project. If you wish to use a different name, it is recommended you place the output folder inside `Untracked`, such as `Untracked/F-Zero GX (JP)`.

![](res\gma\gma-importer-4.jpg)



### 1.2.3 Set File Search Option

The **File Search Option** parameter tells the program how to search for files inside the **Import From** path.

* **Top Directory Only**: search for files only inside the specified directory.
* **All Directories**: search for files inside the specified directory and all sub-directories.

Here, we are using **All Directories** to find all GMA model archives inside the root folder.

![](res\gma\gma-importer-5.jpg)



### 1.2.4 Understand Search Pattern

The **Search Pattern** parameter does not need to be modified. However, we will take a moment to discuss it. The default value here is `*.GMA*`. The asterisk `*` indicates "wildcard" which means the program will search for any file that starts with anything proceeded by `.GMA`. In addition, it will also check for any files that then have anything else past the `.GMA` due to the ending asterisk. This effectively captures all `.GMA` and `.GMA.LZ` files. The importer knows how to handle unpacking any `.LZ` files on it's own and will save all decompressed files in the same directory as the compressed files. For subsequent imports, it will be quicker as it will not need to decompress the files before importing them.



## 1.3 Press Import Button

Push. The. Button. 

Pressing `Import GMA` will begin the import process. For a clean extract of `GFZJ01` <u>*F-Zero GX*</u> (Japan), there should be 897 GMA archives to import. Note that there are about 1600 GMA archives total. However, Manifold does not currently automate unpacking .ARC archive files.

![](res\gma\gma-importer-6.jpg)

Once complete, you should see a variety of messages in Unity's console window. These simply state any created folders for debugging purposes.

![](res\gma\gma-importer-7.jpg)



# Part 2: Import GMA Models

Now that the GMA model archives have been turned into data assets, we can use another tool to parse them and reconstruct the model geometry.

This process will create both Unity `Mesh` assets and prefabs with the model data attached. Each has their own utility.



## 2.1 Create a GMA Model Importer

To reconstruct the meshes, we need to create an instance of the model importer. Right-click in the Project panel in Unity to bring up the context menu. At the top, select `Create > Manifold > GFZX01 > GMA Model > GMA Model Importer`.

![](res\gma\gma-model-importer-1.jpg)

A new asset called `New GMA Model Importer` will be created. Place it inside the `Untracked` folder.

![](res\gma\gma-model-importer-2.jpg)



## 2.2 Set Import Parameters

### 2.2.1 Set Import From

We need to tell the importer where we will import the data from. The **Import From** parameter tells the program where to look for files. Unlike the previous importer, this will search for files within Unity. Here, we are selecting the newly created folder `GFZJ01` that is inside the Unity project at `Assets/GFZJ01`. Of course, your folder name may be different from this.

![](res\gma\gma-model-importer-3.jpg)



### 2.2.2 Set Import To

Again, we need to tell the importer where we will save our Unity assets to. The **Import to** parameter tells the program where to place the imported files. Here, we are telling the program to place our imported files into a folder called `GFZJ01`, adjacent to our previously imported files.

![](res\gma\gma-model-importer-4.jpg)



### 2.2.3 Set Import Option

The **Import Option** parameter tells the program how to search for files inside the **Import From** path. Specifically, it indicates how Unity will search for these assets.

* **Selected Files**: only use the files selected by the **Import Files** parameter.
  * The **Import Files** parameter takes in objects of type `GmaSobj`, the assets we generated in Part 1. If you wish to only import select files, you can set them in the inspector explicitly.
* **All from Source Folder**: search for files inside **Import From** including all sub-directories.
* **All from Asset Database**: search for files inside the entirety of the Unity Asset database. Effectively this gets all `GmaSobj` in the project.

Here, we are using **All from Source Folder** to find all `GmaSobj`s inside the newly imported folder.

![](res\gma\gma-model-importer-5.jpg)



## 2.3 Set Default Material

The current import method does not include materials and texture mapping. As a placeholder, the importer uses a parameter to replace all materials with a default vertex color shader. In the **Default Mat** field, search for the provided `mat_gma_model` material. This allows you to view the vertex color data of the model data.

![](res\gma\gma-model-importer-6.jpg)



## 2.4 Press Import Button

Press. The. Button.

Pressing `Import GMA Models` will begin the import process. For a clean extract of `GFZJ01` <u>*F-Zero GX*</u> (Japan), there should be 3666 models to import. This process takes about 30 minutes on my machine, your time will likely vary.

If you know what you need models for, you can select which models specifically to import to reduce import time. Consult [2.2.3 Set Import Option](#2.2.3 Set Import Option) for details.

![](res\gma\gma-model-importer-7.jpg)

Once complete, there will likely be many errors in the console. Do not worry, these are due to current issues with reconstructing mesh data from GFZ games' mesh data. This will be fixed as we better understand how to reinterpret the data in Unity terms.

![](res\gma\gma-model-importer-8.jpg)

You can now view the model data in Unity.

![](res\gma\gma-model-importer-9.jpg)



# Part 3: Getting a Better View

While this is nice, models are best viewed in context. If you wish to see the models in their respective courses, go through the [Importing COLI_COURSE Scenes](Importing COLI_COURSE Scenes.md).

Otherwise, you can play around with dragging the prefabs into a scene. Toggling the view options. In the Scene window, in the upper-left, there is an option. The default is Shaded. We recommend Shaded Wireframe for use with the vertex color material.

