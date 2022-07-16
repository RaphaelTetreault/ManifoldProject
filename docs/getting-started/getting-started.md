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



TEST OPEN



### Opening Project

![github](.\res\github.png)

Download the project repository from Github. https://github.com/RaphaelTetreault/ManifoldProject



![extract](.\res\extract.png)

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



First, open the project.

![editor1](.\res\editor1.png)

The default view should be like so. However, I use a different layout. We can change that so that you can better follow along.



![editor2](.\res\editor2.png)

Go to `Window / Layouts` and select `2 by 3`.



![editor3](.\res\editor3.png)

In the Project window on the right, select the options icon (three vertical dots, right of the lock symbol). Select `One Column Layout`.



![editor4](.\res\editor4.png)

We still need to decompress the game files from earlier. In the `Manifold /  LZ Compression Tools` and select `Decompress all LZ files in directory tree`.



![editor5](.\res\editor5.png)

Locate the bootable files folder from earlier. Press confirm your choice.

The program will now decompress all LZ files in and under that folder.



![editor6](.\res\editor6.png)

Now that the files are decompressed, we need to tell the tools where to import and manage files from.

In the menu options, select `Manifold` and then `Open Settings Window`.



![editor7](.\res\editor7.png)

This window houses all the options I have specified for the various tools. Above is what my settings look like. Importantly, this is just point to folders on my computer to load in and export files to various locations.

**General**

* **Serialize Format**: select if files are for *<u>F-Zero AX</u>* or <u>*F-Zero GX*</u>.
* **Source Directory**: point this to our clean source files that we decompressed. This folder is used when importing game files.
* **Working Files Directory**: point this to a folder where you want exported files to go. Ideally this is a working directory that Dolphin can run so you can test out the files you made.
* **Unity Working Directory**: where imported files are moved to. NOTE: currently this is the idea, but I haven't smoothed over some hard-coded paths. Not all files may be placed inside this folder.

**Scene Generation**

* **Scene Export Path**: point this to where you wish generated scene files to be exported.
* **Single Scene Index**: the scene index used when importing scenes with the "single scene" option. By default, index 1 is for the file COLI_COURSE01.
* **Convert Coord Space**: deprecated, does nothing at the moment.

**Collider Generation**:

* **Create Collider Backfaces**: when importing colliders, if the mesh should have both front and back faces.
* **Collider 256 Scene Index**: the scene index used when importing all 256 chunked static scene colliders. By default, index 1 is for the file COLI_COURSE01. This is *slow* and is meant for debugging.
* **Collider 256 Mesh Type**: the kind of mesh to construct all 256 chunks of. 

**File Output Directories**

* **Log Output Directory**: where log files are placed.
* **Analysis Output Directory**: where file analysis spreadsheets are placed.
* **File/Binary Output Directory**: where binary files are output (for debugging functions).



![editor8](.\res\editor8.png)

We need to generate some assets to see scenes. Technically, you don't need any of this, but it is very helpful.

Select `Manifold / GMA` and choose `Import all models from source folder`.

![editor-model-import](.\res\editor-model-import.png)

This process takes about 15 minutes on a high end machine with a NVMe 3.0 drive. Expect this to take a while. Grab yourself a tea :tea:.



![editor9](.\res\editor9.png)

**Optional**: import colliders meshes.

Select `Manifold / Colliders` and choose `Create collider meshes`. This should be fairly brief, only a couple of minutes.



![editor10](.\res\editor10.png)

**Optional**: import textures. They aren't used yet, but if you want to peruse them, you can.

Selecting `Manifold / TPL` and choosing `Import all textures and mipmaps (with hash reference objects)` will import each unique texture only once and name it its PNG hash. This is not really human-friendly, so not advised.

Selecting `Manifold / TPL` and choosing `Convert TPL Textures - Main Textures Only` will import all main textures from TPLs to subfolders named after the TPL archive. Mipmaps are not generated. The user selects which folder to place them in. It is recommended to place the folder outside of Unity as Unity with optimize the PNGs, which takes more time.

In either case, this will take a while to process.



![editor11](.\res\editor11.png)

Finally, select `Manifold / Scenes` and choose `Import All Stages`. This will construct a Unity scene which replicates GFZ's scenes, creating objects for all of the data.



![editor12](.\res\editor12.png)

Once the import is done, you should see something like this.



![editor13](.\res\editor13.png)

Let's change the render view to be more helpful. In the render options, select `Shaded Wireframe`.



![editor14](.\res\editor14.png)

Things look clearer now. We can also disable all of the other visual layers. Uncheck items in the menu.



![editor15](.\res\editor15.png)

If we open up COLI_COURSE01, we can peruse it a bit.



At this point, the assets are in the editor and you can begin manipulating them. A guide on creating stage data to come at a later date.
