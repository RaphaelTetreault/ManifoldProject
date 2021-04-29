namespace GameCube.GFZ.CourseCollision
{
    //https://craftedcart.github.io/SMBLevelWorkshop/documentation/index.html?page=stagedefFormat2#spec-stagedefFormat2-section-animationKeyframe

    // Review types
    // http://www.john-player.com/maya/interface/maya-animation-tangent-types-explained/

    public enum InterpolationMode : int
    {
        Constant = 0,
        Linear,
        unknown1, // could be auto or auto clamped?
        unknown2, // could be auto or auto clamped?
    }
}