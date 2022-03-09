namespace Manifold.EditorTools.Attributes
{
    /// <summary>
    /// Base Attribute to be derived for creating custom string browsing fields
    /// </summary>
    public abstract class BrowsePathAttribute : UnityEngine.PropertyAttribute
    {
        protected string relativeDirectory = "Assets/";
        protected string panelTitle = "undefined";

        public string RelativeDirectory => relativeDirectory;
        public string PanelTitle => panelTitle;


        public BrowsePathAttribute()
        {

        }

        public BrowsePathAttribute(string relativeDirectory)
        {
            this.relativeDirectory = relativeDirectory;
        }

        public BrowsePathAttribute(string relativeDirectory, string panelTitle)
        {
            this.relativeDirectory = relativeDirectory;
            this.panelTitle = panelTitle;
        }
    }
}