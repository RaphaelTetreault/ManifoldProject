### Extracting Source Files

![dolphin-1](D:/github/manifold-project/docs/getting-started/res/dolphin-1.png)

Open Dolphin.



![dolphin-2](D:/github/manifold-project/docs/getting-started/res/dolphin-2.png)

In the Toolbar, select *Options* then *Configuration*.



![dolphin-3](D:/github/manifold-project/docs/getting-started/res/dolphin-3.png)

In the settings window, select the Toolbar tab named *Paths*.



![dolphin-4](D:/github/manifold-project/docs/getting-started/res/dolphin-4.png)

Press the *Add* button and browse to the folder on your computer which contains the game ISO. The disc images will then populate the main window in Dolphin.



![dolphin-5](D:/github/manifold-project/docs/getting-started/res/dolphin-5.png)

In Dolphin's main window, right-click the title you wish to extract the contents from. Select *Properties* from the context menu.



![dolphin-6](D:/github/manifold-project/docs/getting-started/res/dolphin-6.png)

In the properties window, select the Toolbar tab named *Filesystem*. Right-click the root item in the list and select *Extract Entire Disc*. 



![dolphin-7](D:/github/manifold-project/docs/getting-started/res/dolphin-7.png)

A folder browser window will appear. Selected the folder you wish to extract the files to. We recommend the output folder be named the game ID of the extracted disc. Click *Select Folder*.



![dolphin-8](D:/github/manifold-project/docs/getting-started/res/dolphin-8.png)

Wait for the files to be extracted. On systems using HDD or SSD hard drives, this may take a bit of time and appear to be frozen. Wait for the operation to complete on it's own.



We have successfully extracted the game files at this point. We can now play using these files on the host computer to run custom files locally in the Dolphin emulator.



![dolphin-10](D:/github/manifold-project/docs/getting-started/res/dolphin-10.png)

Note that after the extraction process, we have two folders.

1. *files*: contains all of the files on the disc.
2. *sys*: contains essential files and the initial program loader.



To patch in custom stages, place the COLI_COURSE and related `.gma` and `.tpl` files in`./stage`.

To play, open Dolphin and select the "Open" option from the GUI in the upper-left. Browse to the source's `sys` folder (in the example, this is `D:/gfze/sys`) and open the `main.dol` file. 

