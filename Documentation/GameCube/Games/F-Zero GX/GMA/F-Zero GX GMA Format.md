# GMA - Geometry Archive

Game: F-Zero GX

## Structures

### GMA Header

| Offset (h) | Type          | Name         | Comment |
| ---------- | ------------- | ------------ | ------- |
| 0          | UINT 32       | Model Count  |         |
| 4          | UINT 32       | Header Size  |         |
| -          | GMA PTR ARRAY | GMA Pointers |         |

### GMA Pointer

| Offset (h) | Type    | Name             | Comment                                                      |
| ---------- | ------- | ---------------- | ------------------------------------------------------------ |
| 8          | UINT 32 | GCMF Offset      | Add to Header_Size to get correct offset                     |
| C          | UINT 32 | GCMF Name Offset | Add to (0x08 + Header_Size * Model_Count) to get correct offset. Names are terminated by 0x00, no alignment. Header aligns to 32 bytes due to GameCube FIFO |

