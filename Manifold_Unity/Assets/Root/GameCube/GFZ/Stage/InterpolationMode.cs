namespace GameCube.GFZ.Stage
{
    // Super Monkey Ball (SEGA AV developed) had info on keys 0, 1.
    // https://craftedcart.github.io/SMBLevelWorkshop/documentation/index.html?page=stagedefFormat2#spec-stagedefFormat2-section-animationKeyframe

    // Review types
    // http://www.john-player.com/maya/interface/maya-animation-tangent-types-explained/

    /// <summary>
    /// Animation key interpolation methods.
    /// </summary>
    public enum InterpolationMode : int
    {
        /// <summary>
        /// No interpolation between keys. Values are kept until next key is hit. Maya "step" tangent.
        /// </summary>
        Constant = 0,

        /// <summary>
        /// Linear interpolation between keys.
        /// </summary>
        Linear,

        /// <summary>
        /// 
        /// </summary>
        unknown1, // could be auto or auto clamped? SMB2 community calls it "ease"

        /// <summary>
        /// 
        /// </summary>
        unknown2, // could be auto or auto clamped?
    }
}