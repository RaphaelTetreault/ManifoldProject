Questions

* Where do I begin when looking for Unity GUI code?
  * Naming convention used for anything that makes a menu item is *MenuItems (static classes). ei: GmaMenuItems
* How do I init models?
  * x
* TODO: in the future, combine all these steps into a "First time setup" option.









* Refactor Const classes
* For non-ref data, use [field: SerializeField]





**Hierarchy for generating own track data**

Invoked by SceneExportUtility

* GfzTrack
  * GfzSegmentShape (GfzTrackRoad)
    * GfzTrackSegment
      * GfzTrackSegment (GfzBezierSplineSegment)