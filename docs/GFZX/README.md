# Import Guides

This directory contains guides for importing the various assets of  <u>*F-Zero GX*</u> and <u>*F-Zero AX*</u> (hereafter GFZ). This document prefaces each with a few simple preparatory steps and outlines the purpose of the other guides.



## Revisions Table

| Author            | Date       | Version | Description      |
| ----------------- | ---------- | ------- | ---------------- |
| Raphaël Tétreault | 2020/06/01 | 1.0     | Initial document |



## Table Of Contents

[TOC]

# Preparing Your Files in Unity

The Manifold project is maintained using version control software git. If you are unfamiliar with git, carefully read the [git](#git) section below.

## git

git is version control software which is vital to working on and maintaining any software develop project. While other version control solutions exist, git is both widely used and free. Part of it's job is to record the state of files at various points in time, effectively creating backups of code and documentation over the lifetime of a project. It does this by tracking changes to files. Anytime a new, edited, or deleted files is visible in the project repository, git tracks that change. These changes are usually committed to project and carried forward. Other members of the project can then receive those changes.

However, we do not always wish to do this. There are a few reasons that will be common when using Manifold.

One such case is that some cases the files are of little to no utility for others. This often happens when a project can generate files. Tracking the files introduces clutter that becomes deprecated quickly or can be cleanly generated on another user's machine. When a project is large enough, certain files are discipline specific, thus having all files generated makes little sense. Manifold has tools to reinterpret many files types that range in purpose. Since your needs will differ from many others, basically every file you generate should not be tracked.

Another very important reason why you would not want to commit some files are that they contain copyright material. Manifold's existence owes to reinterpreting proprietary file formats from copyright games. These materials are explicitly unwanted in version control for these reasons. Copyright material should not be shared. Thus, anything generated whose source is copyright should not be tracked.

However, Manifold will import files into the tracked range of git. While you may not be contributing to the project, it is advised you do the same as any other developer and set up you project to ignore these files. The procedure to do so follows this section.

## Managing Untracked Files

Manifold uses a .gitignore file to keep unwanted files out of version control. Within Unity, the path `Assets/Untracked` is tellingly untracked. Since git does not track empty folders, you are required to make the folder yourself. In `Assets/`, right-click inside the Project panel to bring up the context menu. Select `Create > Folder`. Name the folder `Untracked`.

Any folder generate by the tools should also be untracked. For instance, importing files from *<u>F-Zero GX</u>* should output folders to `Assets/GFZE01`, `Assets/GFZJ01`, or `Assets/GFZP01` depending on the source. These folders are currently ignored. As the Manifold project evolves, the ignored folders list will be updated. 



# Import Guides List

Below is a list of the guides and a summary of their purpose.

## GMA Models

The `GMA` files are model archives containing multiple models. Textures are housed separately in TPL files. The purpose of importing GMAs is to allow viewing model data rather than editing it. Other tools exist which allow this. Manifold may support roundtrip editing in the future, but it is not a priority.

Guide: [Importing GMA Models.md](Importing GMA Models.md)

## COLI_COURSE Scenes

The `COLI_SCOURSE` files are scene files for GFZ games. The importer relies on imported models, so be sure to go through [GMA Models](#GMA Models) first. The importer rebuilds the scene using the imported assets and binds them to their associated data such as animation, transform, and more when possible.

Guide: [Importing COLI_COURSE Scenes.md](Importing COLI_COURSE Scenes.md)

