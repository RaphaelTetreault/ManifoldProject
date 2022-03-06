namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// 
    /// </summary>
    public enum TrackSegmentType : byte
    {
        /// <summary>
        /// When set to nothing, TrackProperty has a value.
        /// </summary>
        IsEmbeddedProperty = 0, // 0x00

        /// <summary>
        /// The flag for pipes or cylinders when NOT using TrackProperty?
        /// </summary>
        IsPipeOrCylinder = 1 << 0, // 0x01

        /// <summary>
        /// Flag when this is the final node EXCLUDING children for TrackProperties
        /// If children is ice, dirt, etc, then will have this flag ON
        /// If children is pipe type, then flag will be [1 << 3] and have no [1 << 1]
        /// </summary>
        IsTransformLeaf = 1 << 1, // 0x02

        /// <summary>
        /// Node is one of multiple children (parent of this has multiple children).
        /// Often used for double dirt on sides, branching paths, etc? (are there more cases?)
        /// </summary>
        IsBranchedNode = 1 << 2, // 0x04

        /// <summary>
        /// Node will always have a child (assert assumption) with the same code or "IsTransformLeaf".
        /// </summary>
        IsTransformParent = 1 << 3, // 0x08
    }
}

