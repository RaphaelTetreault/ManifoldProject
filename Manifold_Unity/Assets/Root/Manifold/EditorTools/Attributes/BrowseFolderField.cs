namespace Manifold.EditorTools.Attributes
{
    /// <summary>
    /// Attribute that turns string field into a browasable field that stores the path of the selected folder.
    /// </summary>
    public class BrowseFolderField : BrowsePathAttribute
    {
        public const string defaultPanelTitle = "Open Folder Path";

        public BrowseFolderField() : base()
        {
            this.panelTitle = defaultPanelTitle;
        }

        /// <param name="relativeDirectory">The relative directory to Regex.Match against</param>
        public BrowseFolderField(string relativeDirectory) : base(relativeDirectory)
        {
            this.relativeDirectory = relativeDirectory;
            this.panelTitle = defaultPanelTitle;
        }

        /// <param name="relativeDirectory">The relative directory to Regex.Match against</param>
        /// <param name="panelTitle">The title of the browsing window</param>
        public BrowseFolderField(string relativeDirectory, string panelTitle) : base(panelTitle)
        {
            this.relativeDirectory = relativeDirectory;
            this.panelTitle = panelTitle;
        }
    }
}