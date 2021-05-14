// Legacy notes
// 0, 1, 2, 4, 8

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    public enum TrackTopologyMetadata : byte
    {
        /// <summary>
        /// When set to nothing, TrackProperty has a value
        /// </summary>
        IsUsingTrackProperty = 0,

        /// <summary>
        /// The flag for pipes or cylinders when not using TrackProperty?
        /// </summary>
        IsPipeOrCylinder = 1 << 0, // 0x01

        /// <summary>
        /// Flag when this is the final node EXCLUDING children for TrackProperties
        /// If children is ice, dirt, etc, then will have this flag ON
        /// If children is pipe type, then flag will be [1 << 3] and have no [1 << 1]
        /// </summary>
        IsFinalTopologyNode = 1 << 1, // 0x02

        /// <summary>
        /// Node is one of multiple children (parent of this has multiple children).
        /// Often used for double dirt on sides, branching paths, etc? (are there more cases?)
        /// </summary>
        IsBranchedNode = 1 << 2, // 0x04
        
        /// <summary>
        /// 
        /// </summary>
        IsNotFinalTopologyNode = 1 << 3, // 0x08
    }
}

