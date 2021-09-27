using System.Collections;
using System.Collections.Generic;

namespace GameCube.GFZ
{
    public static class HashLibrary
    {
        /// <summary>
        /// Readonly table of MD5 hashes for each stage (decompressed) in GFZJ01
        /// </summary>
        public static readonly Dictionary<int, string> ColiCourseMD5_GFZJ01 = new Dictionary<int, string>()
        {
            { 01, "4178a91a2deff2069099719940eced75" },
            { 03, "8669ce6cbe7a06f8dbd9b61b53b7b087" },
            { 05, "4010769a44ce3c4f41b93179a43e580c" },
            { 07, "8509478f7cefbdf9f1107cdfd5c820d7" },
            { 08, "23957fba9aa19e5f0e44e81bc97008bb" },
            { 09, "16474256fd7f6e9b8a5c7cb02b5f2fee" },
            { 10, "666a9209ccaff76310017a76c3081a38" },
            { 11, "0d569c3f8d51fdde48b692108f55d83d" },
            { 13, "73ba4da79c2aafa586cebb6ce0bdbfbe" },
            { 14, "d7189a8518573a7b7ef466391fff54a2" },
            { 15, "c681cbb4016f36f509d344092f916869" },
            { 16, "48f18b06586a45141b00049a41acf34a" },
            { 17, "74703e3f71ad4d8874a3bae82c67c66e" },
            { 21, "68767b1daf72bc9d4149ace6d8680c31" },
            { 24, "14c7add4c079c735d73a0c3843e6e30f" },
            { 25, "818975e1af488acbe357176cb1e46d0f" },
            { 26, "a06f5a1b52b3a0ffff8806c5cfe58fc0" },
            { 27, "36e302eaa978390c8e27f4eed97e9695" },
            { 28, "f924b48c7762e229cf3060f9b17160a4" },
            { 29, "130f115ba15cada6a334fccf6d218315" },
            { 31, "f2d33bf48d46faad1abfaf751dd33c01" },
            { 32, "ee9c91015643c1bc49dc1758565694f4" },
            { 33, "0281f8c68fe36ebcbad7864990535aad" },
            { 34, "490788d630df053d6f00e7563ac1bd5e" },
            { 35, "04f34c049160f00dd28ea1e1b2b45234" },
            { 36, "7fe9e79720b1fc93f16e19e43c0c66cf" },
            { 37, "f95c1537c187940c7aa2397524075c04" },
            { 38, "d43b4b81798c5cfb1763255c1bb2f1d1" },
            { 39, "cc64505cd317e63dfd345ff9061fcd41" },
            { 40, "8241d00159004cca1402797c02129af0" },
            { 41, "1504ad2334e94dce29f7c3b0c211b35e" },
            { 42, "9d3ab3148f3579775e91ad3e41998e2b" },
            { 43, "2351423354002b92da66d7223b1841cc" },
            { 44, "65579969d65d5c8cfa75a82733f91d61" },
            { 45, "9a60a6af73bd99e5ed81ea48d1b9e2b6" },
            { 49, "c9ea24fdd9b64aca7c1601f7862ff91a" },
            { 50, "a9a289f330b079bc5f06253e215199d9" },
        };

        /// <summary>
        /// Readonly table of MD5 hashes for each stage (decompressed) in GFZE01
        /// </summary>
        public static readonly Dictionary<int, string> ColiCourseMD5_GFZE01 = new Dictionary<int, string>()
        {
            { 01, "4178a91a2deff2069099719940eced75" },
            { 03, "8669ce6cbe7a06f8dbd9b61b53b7b087" },
            { 05, "4010769a44ce3c4f41b93179a43e580c" },
            { 07, "a8da0bd6f68cd15af2157240f5321293" }, // changed vs jp
            { 08, "23957fba9aa19e5f0e44e81bc97008bb" },
            { 09, "16474256fd7f6e9b8a5c7cb02b5f2fee" },
            { 10, "666a9209ccaff76310017a76c3081a38" },
            { 11, "0d569c3f8d51fdde48b692108f55d83d" },
            { 13, "73ba4da79c2aafa586cebb6ce0bdbfbe" },
            { 14, "d7189a8518573a7b7ef466391fff54a2" },
            { 15, "c681cbb4016f36f509d344092f916869" },
            { 16, "48f18b06586a45141b00049a41acf34a" },
            { 17, "74703e3f71ad4d8874a3bae82c67c66e" },
            { 21, "68767b1daf72bc9d4149ace6d8680c31" },
            { 24, "14c7add4c079c735d73a0c3843e6e30f" },
            { 25, "8d41b351da4d32e5578cf8b0c1a441fd" }, // changed vs jp
            { 26, "a06f5a1b52b3a0ffff8806c5cfe58fc0" },
            { 27, "36e302eaa978390c8e27f4eed97e9695" },
            { 28, "f924b48c7762e229cf3060f9b17160a4" },
            { 29, "130f115ba15cada6a334fccf6d218315" },
            { 31, "f2d33bf48d46faad1abfaf751dd33c01" },
            { 32, "ee9c91015643c1bc49dc1758565694f4" },
            { 33, "0281f8c68fe36ebcbad7864990535aad" },
            { 34, "490788d630df053d6f00e7563ac1bd5e" },
            { 35, "04f34c049160f00dd28ea1e1b2b45234" },
            { 36, "7fe9e79720b1fc93f16e19e43c0c66cf" },
            { 37, "f95c1537c187940c7aa2397524075c04" },
            { 38, "d43b4b81798c5cfb1763255c1bb2f1d1" },
            { 39, "cc64505cd317e63dfd345ff9061fcd41" },
            { 40, "8241d00159004cca1402797c02129af0" },
            { 41, "1504ad2334e94dce29f7c3b0c211b35e" },
            { 42, "9d3ab3148f3579775e91ad3e41998e2b" },
            { 43, "2351423354002b92da66d7223b1841cc" },
            { 44, "65579969d65d5c8cfa75a82733f91d61" },
            { 45, "9a60a6af73bd99e5ed81ea48d1b9e2b6" },
            { 49, "c9ea24fdd9b64aca7c1601f7862ff91a" },
            { 50, "a9a289f330b079bc5f06253e215199d9" },
        };
    }
}
