# F-Zero GX and F-Zero AX

**Developer**: Amusement Vision

**Release**: 2003

| Game      | Region   | Code   |
| --------- | -------- | ------ |
| F-Zero GX | Japan    | GFZJ01 |
| F-Zero GX | Americas | GFZE01 |
| F-Zero GX | Europe   | GFZP01 |
| F-Zero AX | Japan    | GFZJ8P |

F-Zero GX (GameCube X) and F-Zero AX (Arcade X) are games developed by Amusement Vision, a now-defunct development house of SEGA. Both games are in essence different branches of the same project with GX being released on GameCube and AX released on the Triforce arcade platform. Since both use GameCube hardware and are mostly the same project, their files a quite similar. F-Zero AX's build is earlier than that of F-Zero GX and so there are minor discrepancies between files.

# File Types Summary

This is a complete list of all file types in **F-Zero GX **(US) once all archives are opened. Some files need further investigation.

| Ct   | Ct*  | Extension | Data                        | Description                                                  |
| ---- | ---- | --------- | --------------------------- | ------------------------------------------------------------ |
| 1    | =    |           | Vehicle Stats Archive       | Vehicles stats binary                                        |
| 37   | =    |           | Collision Data              | All files prefixed with COLI and have no extension. COLI is better called "Scene" as it houses a lot of other things besides collision. |
| 90   | =    | ADX       | Audio                       | BGM Audio. [ADH File Format](https://en.wikipedia.org/wiki/ADX_(file_format)) |
| 947  | =    | AHX       | Audio                       | Voice Audio. [AHX File Format](https://en.wikipedia.org/wiki/ADX_(file_format)) |
| 45   | =    | ARC       | Lip Animation Archive       | Character's lip animation archive                            |
| 0    | 995  | LTT       |                             | Lip animation?                                               |
| 89   |      | ARC       | Vehicle Model Archive       | [chara]\_e.arc [chara]\_p.arc                                |
| 208  |      | ARC       | Character Interview Archive | Note: sel, 500, 1000, 3000                                   |
| 3    | 210* | ARC/BIN   | JPEG Archive                | Countdown timer animation graphics, heavily compressed       |
| 41   |      | BIN       | Dialogue                    | Dialogue for post Grand Prix banter                          |
| 43   |      | BIN       | Enemy Line                  | Really strange contents, lots of repetition. Used Ghidra, it is not PPC code |
| 2    |      | BIN       | Emblem                      | Custom emblem data for Garage vehicles                       |
| 1    |      | BIN       | Font 24x24                  | Standard font                                                |
| 26   |      | BIN       | LiveCam                     | Camera...?                                                   |
| 26   |      | BIN       | LiveCam Stage               | Camera data when viewing vehicles from stage perspective (after race, replay) |
| 39   |      | BIN       | LiveCam Ball                | Camera...?                                                   |
| 22   |      | BIN       | LiveCam StageDemo           | Camera data for stage introduction                           |
| 5    |      | BIN       | Memcard                     | UNKNOWN                                                      |
| 4    |      | BIN       | Sound Effect                | SFX Audio                                                    |
| 29   | =    | BIN/DAT   | Ghost Data                  | Binaries for Staff Ghosts (DAT) and Story Ghosts (BIN)       |
| 1    | =    | BNR       | Banner                      | GameCube banner format                                       |
| 75   | 279  | FMI       | UNKNOWN (Vehicle)           | Custom part positioning data?                                |
| 804  | 1589 | GMA       | Geometry Archive            | Archive for models meshes, sub-meshes, and material definitions |
| 1    | =    | GMO       | UNKNOWN                     | Looks like a debug list / translation?                       |
| 608  |      | LZ        | Lempel-Ziv Archive          | Lempel-Ziv archive. TODO: reference what is custom about this archive because there is something |
| 46   | 226  | MAL       | Model Animation Library (?) | Lives in /motion. Format is [pilot]_common.mal               |
| 306  | 1032 | MTA       | Motion (?)                  | Lives in /motion directory, same count as SKL so it could be weights |
| 14   | =    | REL       | Relinkable Module           | GameCube Relinkable module                                   |
| 60   | =    | SFD       | Video                       | Video data for story cutscenes                               |
| 306  | 1126 | SKL       | Skeleton                    | Skeleton animation data. TODO: does it include weights?      |
| 1    | =    | STR       | String Table (?)            | Lists absolute paths from compiler's drive to many *.PLF files with the same name as *.REL files |
| 1018 | 2338 | TPL       | Texture Pallet              | Modified Texture Pallet format                               |
| 39   | =    | TXT       | Text                        | Subtitles for SFD videos                                     |

EOF