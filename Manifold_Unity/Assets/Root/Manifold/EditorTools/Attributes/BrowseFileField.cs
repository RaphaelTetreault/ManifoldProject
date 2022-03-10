namespace Manifold.EditorTools.Attributes
{
    /// <summary>
    /// Attribute that turns string field into a browasable field that stores the path of the selected file.
    /// </summary>
    public class BrowseFileField : BrowsePathAttribute
    {
        public const string defaultPanelTitle = "Open File Path";

        protected string[] fileTypes;
        protected string fileTypeArgs;
        protected bool removeExtension;

        public string FileTypeArgs => fileTypeArgs;

        public bool RemoveExtension
        {
            get
            {
                return removeExtension;
            }
        }

        /// <summary>
        /// Converts string array into single string argument for EditorUtlility.OpenFilePanel
        /// </summary>
        private string CreateFileTypeArgs
        {
            get
            {
                string fileTypeArg = string.Empty;

                foreach (string fileType in fileTypes)
                    fileTypeArg += string.Format("{0}, ", fileType);

                return fileTypeArg;
            }
        }

        public BrowseFileField() : base()
        {
            this.panelTitle = defaultPanelTitle;
            fileTypeArgs = string.Empty;
        }

        /// <param name="relativeDirectory">The relative directory to Regex.Match against</param>
        public BrowseFileField(string relativeDirectory) : base()
        {
            this.relativeDirectory = relativeDirectory;
            this.panelTitle = defaultPanelTitle;
            fileTypeArgs = string.Empty;
        }

        /// <param name="fileTypes">The filetypes to be searched for by default</param>
        public BrowseFileField(string[] fileTypes) : base()
        {
            this.panelTitle = defaultPanelTitle;
            this.fileTypes = fileTypes;
            fileTypeArgs = CreateFileTypeArgs;
        }

        /// <param name="relativeDirectory">The relative directory to Regex.Match against</param>
        /// <param name="fileTypes">The filetypes to be searched for by default</param>
        public BrowseFileField(string relativeDirectory, string[] fileTypes) : base(relativeDirectory)
        {
            this.relativeDirectory = relativeDirectory;
            this.panelTitle = defaultPanelTitle;
            this.fileTypes = fileTypes;
            fileTypeArgs = CreateFileTypeArgs;
        }

        /// <param name="relativeDirectory">The relative directory to Regex.Match against</param>
        /// <param name="panelTitle">The title of the browsing window</param>
        /// <param name="fileTypes">The filetypes to be searched for by default</param>
        public BrowseFileField(string relativeDirectory, string panelTitle, string[] fileTypes) : base(panelTitle)
        {
            this.relativeDirectory = relativeDirectory;
            this.panelTitle = panelTitle;
            this.fileTypes = fileTypes;
            fileTypeArgs = CreateFileTypeArgs;
        }

        // NEW

        /// <param name="removeExtension">Trims the extension out of the result string</param>
        public BrowseFileField(bool removeExtension) : base()
        {
            this.panelTitle = defaultPanelTitle;
            fileTypeArgs = string.Empty;
            this.removeExtension = removeExtension;
        }
        /// <param name="relativeDirectory">The relative directory to Regex.Match against</param>
        /// <param name="removeExtension">Trims the extension out of the result string</param>
        public BrowseFileField(string relativeDirectory, bool removeExtension) : base()
        {
            this.relativeDirectory = relativeDirectory;
            this.panelTitle = defaultPanelTitle;
            fileTypeArgs = string.Empty;
            this.removeExtension = removeExtension;
        }
        /// <param name="fileTypes">The filetypes to be searched for by default</param>
        /// <param name="removeExtension">Trims the extension out of the result string</param>
        public BrowseFileField(string[] fileTypes, bool removeExtension) : base()
        {
            this.panelTitle = defaultPanelTitle;
            this.fileTypes = fileTypes;
            fileTypeArgs = CreateFileTypeArgs;
            this.removeExtension = removeExtension;
        }
        /// <param name="relativeDirectory">The relative directory to Regex.Match against</param>
        /// <param name="fileTypes">The filetypes to be searched for by default</param>
        /// <param name="removeExtension">Trims the extension out of the result string</param>
        public BrowseFileField(string relativeDirectory, string[] fileTypes, bool removeExtension) : base(relativeDirectory)
        {
            //this.relativeDirectory = relativeDirectory;
            this.panelTitle = defaultPanelTitle;
            this.fileTypes = fileTypes;
            fileTypeArgs = CreateFileTypeArgs;
            this.removeExtension = removeExtension;
        }
        /// <param name="relativeDirectory">The relative directory to Regex.Match against</param>
        /// <param name="panelTitle">The title of the browsing window</param>
        /// <param name="fileTypes">The filetypes to be searched for by default</param>
        /// <param name="removeExtension">Trims the extension out of the result string</param>
        public BrowseFileField(string relativeDirectory, string panelTitle, string[] fileTypes, bool removeExtension) : base(relativeDirectory, panelTitle)
        {
            //this.relativeDirectory = relativeDirectory;
            //this.panelTitle = panelTitle;
            this.fileTypes = fileTypes;
            fileTypeArgs = CreateFileTypeArgs;
            this.removeExtension = removeExtension;
        }
    }
}
