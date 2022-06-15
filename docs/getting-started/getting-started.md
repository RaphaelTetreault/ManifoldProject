# Getting Started

## Recommended Software

* [Unity Hub](https://unity3d.com/get-unity/download). This program helps you manage many versions of Unity and acts as a project launcher. Use whichever version is most up-to-date.

## Required Software

* [Unity](https://unity3d.com/get-unity/download). Check the git repository for the specific version of Unity required. You *should* be able to use later versions assuming there are no breaking changes between editors. It is *highly* recommended you stick to using the version of Unity specified at the time of the commit.
* [Dolphin](https://dolphin-emu.org/download/).

## First Time Configuration

To edit files using the project, you will need to configure a few settings. These settings are not part of Unity's settings, but custom settings built explicitly for editing F-Zero AX and F-Zero GX data.

The following steps assume you have downloaded Unity Hub and the specific version of Unity used by the project.



### Extracting Source Files

We require assets from F-Zero AX or F-Zero GX to create new assets for each respective title. To get the assets, we need to extract them from the game's disc image.

This requires a copy of the aforementioned game(s). If you do not own a copy of the game you wish to mod, go buy a used copy. We don't distribute the game and will not help in acquiring an illegal copy. We will direct you to https://wii.guide/ to learn how to softmod a Wii system. After softmodding a Wii, you can follow https://wii.guide/dump-games to learn how to legally back up your game GameCube and Wii disc images.



![dolphin-1](.\res\dolphin-1.png)

Open Dolphin.



![dolphin-2](.\res\dolphin-2.png)

In the Toolbar, select *Options* then *Configuration*.



![dolphin-3](.\res\dolphin-3.png)

In the settings window, select the Toolbar tab named *Paths*.



![dolphin-4](.\res\dolphin-4.png)

Press the *Add* button and browse to the folder on your computer which contains the game ISO. The disc images will then populate the main window in Dolphin.



![dolphin-5](.\res\dolphin-5.png)

In Dolphin's main window, right-click the title you wish to extract the contents from. Select *Properties* from the context menu.



![dolphin-6](.\res\dolphin-6.png)

In the properties window, select the Toolbar tab named *Filesystem*. Right-click the root item in the list and select *Extract Entire Disc*. 



![dolphin-7](.\res\dolphin-7.png)

A folder browser window will appear. Selected the folder you wish to extract the files to. We recommend the output folder be named the game ID of the extracted disc. Click *Select Folder*.



![dolphin-8](.\res\dolphin-8.png)

Wait for the files to be extracted. On systems using HDD or SSD hard drives, this may take a bit of time and appear to be frozen. Wait for the operation to complete on it's own.



We have successfully extracted the game files at this point. However, we have two independent uses for the files. We will want to use the default, unedited files when rebuilding the assets in Unity *and* we will want a folder on the host computer to run and tests created files locally in the Dolphin emulator.



![dolphin-10](.\res\dolphin-10.png)

Note that after the extraction process, we have two folders.

1. *files*: contains all of the files on the disc.
2. *sys*: contains essential files and the initial program loader.



Let's begin by creating our working folder for development. Duplicate the root folder (in our example, *gfze01*) and name the copy something like *gfze01-working* to designate it as our working files for development.

![dolphin-11](.\res\dolphin-11.png)

The contents of our copied folder will look like this. The structure here is a hard requirement and cannot be altered.



For our source import folder, we do not need *sys*. Furthermore, we do not need to have our files inside the *files* folder. We can delete *sys* and move the files from inside *files* up one directory, then delete the *files* folder. 

![dolphin-12](.\res\dolphin-12.png)

The final folder structure will look like the above.



### Opening Project

[]

Download the project repository from Github. https://github.com/RaphaelTetreault/ManifoldProject



[]

Extract the files from the archive and place them somewhere on your system. We recommend placing you files somewhere "low" on your system, where the root path is as short as possible to prevent path errors.



![unity-hub-1](.\res\unity-hub-1.png)

Open the *<u>Unity Hub</u>* program. This window appear.



![unity-hub-2](.\res\unity-hub-2.png)

In the upper right-hand corner, click on the "Open" dropdown icon and click on "Add project from disk"



![unity-hub-3](.\res\unity-hub-3.png)

Locate the folder "Manifold_Unity" from within the extracted files. This will be located inside the root path where the files were extracted.



![unity-hub-4](.\res\unity-hub-4.png)

You will now see the project listed in <u>*Unity Hub*</u> and can now select it to open it with the selected version of the Unity editor denoted in the *EDITOR VERSION* column.



![unity-hub-5](.\res\unity-hub-5.png)

Optional: select or install the version of Unity you wish to use with the project files. We recommend using the exact same version of the editor as was used to created the project files. 



### Setting Up GFZ-Specific Settings

Now that we have our files and editor ready, we can configure the project to import files from and export files to specific folders.































