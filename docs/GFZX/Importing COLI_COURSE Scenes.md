# Importing COLI_COURSE Scenes

This guides demonstrates how to import scenes from <u>*F-Zero GX*</u> and <u>*F-Zero AX*</u> (hereafter GFZ). 



## Revisions Table

| Author            | Date       | Version | Description      |
| ----------------- | ---------- | ------- | ---------------- |
| Raphaël Tétreault | 2020/06/01 | 1.0     | Initial document |



## Table Of Contents

[TOC]

# Part 0: Previous Requirements

To generate scene files appropriately requires you to have imported models needed for any given scene. While it is possible to create scene files without models, it means that all model references will be substituted with generic placeholders.

For any given COLI_COURSE scene, you should have the following model archives imported:

* The stage's models, `st##.gma`
  * All model archives found in `GFZ/stage`
* The stage's background models, such as `bg_mut.gma` for Mute City stages.
  * All model archives found in `GFZ/bg`
* Common gameplay elements found in `GFZ/init/race.gma`

If you intent on viewing all stages, it is recommended to import all models.

We recommend you follow along with the [Importing GMA Models](Importing GMA Models.md) guide before continuing if you have not already.



# Part 1: Import COLI_COURSE

The first step to getting GFZ scenes in Manifold is to load the files as `ScriptableObject`s. Manifold has definitions for many files formats where the file is imported non-destructively as a data asset. This will become useful for Part 2 when we reconstruct the scene by iterating over the data inside the asset.



## 1.1 Create a COLI Importer object

To import the assets, we need to create an instance of the importer. Right-click in the Project panel in Unity to bring up the context menu. At the top, select `Create > Manifold > GFZX01 > Course Collision > COLI Importer`.

![](res\coli\coli-importer-1.jpg)

A new asset called `New COLI Importer` will be created. Place it inside the `Untracked` folder.

![](res\coli\coli-importer-2.jpg)



## 1.2 Set Import Parameters

### 1.2.1 Set Import From

We need to tell the importer where we will import the data from. The **Import From** parameter tells the program where to look for files. Press the `Browse` button to bring up the folder menu and select the corresponding folder to use. Here, we are selecting the root folder of the extracted GFZJ01 ISO. This will fill the field with the folder's path. In this screenshot, that path is `C:/GFZJ01`.

![](res\coli\coli-importer-3.jpg)

### 1.2.2 Set Import To

We also need to tell the importer where we will save our Unity assets to. The **Import to** parameter tells the program where to place the imported files. The path is relative to Unity's `Assets/`, so `Assets/GFZJ01` is written in **Import From** simply as `GFZJ01`. Here, we are telling the program to place our imported files into a folder called `GFZJ01`.

**NOTE**: the folders `GFZE01`, `GFZJ01`, `GFZP01`, and `GFZJ8P` are specially marked folders ignored by git in the Manifold project. If you wish to use a different name, it is recommended you place the output folder inside `Untracked`, such as `Untracked/F-Zero GX (JP)`.

![](res\coli\coli-importer-4.jpg)



### 1.2.3 Understand File Search Option

The **File Search Option** parameter tells the program how to search for files inside the **Import From** path.

* **Top Directory Only**: search for files only inside the specified directory.
* **All Directories**: search for files inside the specified directory and all sub-directories.

Here, we are using **All Directories** to find all the COLI_COURSE files. Since there are only 37 courses in the GameCube versions and import times are quick, we don't need to concern ourselves with selecting specific files.



### 1.2.4 Understand Search Pattern

The **Search Pattern** parameter does not need to be modified. However, we will take a moment to discuss it. The default value here is `COLI_COURSE*`. The asterisk `*` indicates "wildcard" which means the program will search for any file that starts with `COLI_COURSE` followed by anything else. Thus, this captures all `COLI_COURSE##` and `COLI_COURSE##.LZ` files. The importer knows how to handle unpacking any `.LZ` files on it's own and will save all decompressed files in the same directory as the compressed files. For subsequent imports, it will be quicker as it will not need to decompress the files before importing them.



## 1.3 Press Import Button

Push. The. Button. 

Pressing `Import COLI` will begin the import process. For a clean extract of `GFZJ01` <u>*F-Zero GX*</u> (Japan), there should be 37 stages to import.

![](res\coli\coli-importer-5.jpg)



# Part 2: Import COLI_COURSE as Unity Scene

Now that the COLI_COURSE files have been turned into data assets, we can use another tool to parse them and reconstruct the scenes.



## 2.1 Create a GMA Model Importer

To reconstruct the meshes, we need to create an instance of the model importer. Right-click in the Project panel in Unity to bring up the context menu. At the top, select `Create > Manifold > GFZX01 > GMA Model > GMA Model Importer`.

![](res/coli/coli-scene-importer-1.jpg)

A new asset called `New COLI Scene Importer` will be created. Place it inside the `Untracked` folder.

![](res/coli/coli-scene-importer-2.jpg)



## 2.2 Set Import Parameters

### 2.2.1 Set Import From

We need to tell the importer where we will import the data from. The **Import From** parameter tells the program where to look for files. Unlike the previous importer, this will search for files within Unity. Here, we are selecting the newly created folder `GFZJ01` that is inside the Unity project at `Assets/GFZJ01`. Of course, your folder name may be different from this.

![](res/coli/coli-scene-importer-3.jpg)



### 2.2.2 Set Import To

Again, we need to tell the importer where we will save our Unity assets to. The **Import to** parameter tells the program where to place the imported files. Here, we are telling the program to place our imported files into a folder called `GFZJ01`, adjacent to our previously imported files.

![](res/coli/coli-scene-importer-4.jpg)



### 2.2.3 Set Import Option

The **Import Option** parameter tells the program how to search for files inside the **Import From** path. Specifically, it indicates how Unity will search for these assets.

* **Selected Files**: only use the files selected by the **Import Files** parameter.
  * The **Import Files** parameter takes in objects of type `ColiSobj`, the assets we generated in Part 1. If you wish to only import select files, you can set them in the inspector explicitly.
* **All from Source Folder**: search for files inside **Import From** including all sub-directories.
* **All from Asset Database**: search for files inside the entirety of the Unity Asset database. Effectively this gets all `ColiSobj` in the project.

Here, we are using **All from Asset Database** to find all `ColiSobj`s inside the newly imported folder.

![](res/coli/coli-scene-importer-5.jpg)



## 2.3 Press Import Button

Press. The. Button.

Pressing `Import COLI Scene Models` will begin the import process. What the importer is doing is adding each referenced model to a scene in Unity and applying the transform values for each. Some scenes are generated quicker as they have less models total, while others (like Green Plant stages) take a good chunk of time.

![](res/coli/coli-scene-importer-6.jpg)

Congrats! You now have scenes in Unity! However, due to the default material, it may be hard to inspect.



# Part 3: Getting a Better View

## 3.1 Setting up the Scene view

In the Scene window, in the upper-left, there are view options. The default view is Shaded. We recommend Shaded Wireframe for use with the vertex color material. This will expose triangle edges in the scene view.

![](res/coli/coli-scene-importer-8.jpg)

You can now readily inspect the scenes in Unity.

![](res/coli/coli-scene-importer-9.jpg)



## 3.2 FAQ

### 3.2.1 Why is the scene mirrored?

It is but it isn't. The short version is that Unity and Amusement Vision's engine have the X coordinate pointing in opposite directions. In the meantime, this axis will be mirrored. Once the developers of Manifold are confident in their interpretation of the data, we will compensate for this discrepancy.





