using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Office.Interop.OneNote;

namespace OneNoteApi
{
    /// <summary>
    /// Wraps the OneNote interop API
    /// </summary>
    /// <see cref="https://docs.microsoft.com/en-us/office/client-developer/onenote/application-interface-onenote"/>
    public class OneNoteRaw : IDisposable
    {
        private IApplication onenote;
        private bool disposed;

        // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        // Constructors...

        /// <summary>
        /// Initialize a new wrapper
        /// </summary>
        public OneNoteRaw()
        {
            onenote = new Application();
        }

        #region Lifecycle
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    onenote = null;
                }

                disposed = true;
            }
        }


        public void Dispose()
        {
            Dispose(disposing: true);
            // DO NOT call this otherwise OneNote will not shutdown properly
            //GC.SuppressFinalize(this);
        }
        #endregion Lifecycle

        /// <summary>
        /// Navigate to a page (And an object inside of it)
        /// </summary>
        /// <param name="hierarchyObjectId">the page to navigate to</param>
        /// <param name="objectId">an optional parameter of an object inside the page to navigate to</param>
        /// <param name="openNewWindow">should open a new window</param>
        public void NavigateTo(string hierarchyObjectId, string objectId = null, bool openNewWindow = false)
        {
            onenote.NavigateTo(hierarchyObjectId, objectId,openNewWindow);
        }

        /// <summary>
        /// Gets the specified section and its child page hierarchy when null return all notebooks
        /// </summary>
        /// <returns>A Section element with Page children</returns>
        public XElement GetPageHierarchy(string id = null)
        {
            onenote.GetHierarchy(id, HierarchyScope.hsPages, out var xml, XMLSchema.xs2013);
            if (!string.IsNullOrEmpty(xml))
            {
                return XElement.Parse(xml);
            }

            return null;
        }

    }
}
