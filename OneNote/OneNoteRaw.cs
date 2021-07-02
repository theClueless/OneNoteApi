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
using OneNoteApi.Common;

namespace OneNoteApi
{
    public interface IOneNoteRaw : IDisposable
    {
        /// <summary>
        /// Navigate to a page (And an object inside of it)
        /// </summary>
        /// <param name="hierarchyObjectId">the page to navigate to</param>
        /// <param name="objectId">an optional parameter of an object inside the page to navigate to</param>
        /// <param name="openNewWindow">should open a new window</param>
        void NavigateTo(string hierarchyObjectId, string objectId = null, bool openNewWindow = false);

        /// <summary>
        /// Gets the specified section and its child page hierarchy when null return all notebooks
        /// </summary>
        /// <returns>A Section element with Page children</returns>
        XElement GetPageHierarchy(string id = null);

        XElement GetPageContent(string id, PageDetail detail);

        void UpdatePageContent(string content, bool force = false, DateTime? dateExpectedLastModified = null);
    }

    /// <summary>
    /// Wraps the OneNote interop API, for easier usage use <see cref="OneNoteApi.OneNote"/> 
    /// </summary>
    /// <see cref="https://docs.microsoft.com/en-us/office/client-developer/onenote/application-interface-onenote"/>
    public class OneNoteRaw : IDisposable, IOneNoteRaw
    {
        private IApplication _onenote;
        private bool _disposed;

        // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        // Constructors...

        /// <summary>
        /// Initialize a new wrapper
        /// </summary>
        public OneNoteRaw()
        {
            _onenote = new Application();
        }

        #region Lifecycle
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _onenote = null;
                }

                _disposed = true;
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
            _onenote.NavigateTo(hierarchyObjectId, objectId, openNewWindow);
        }

        /// <summary>
        /// Gets the specified section and its child page hierarchy when null return all notebooks
        /// </summary>
        /// <returns>A Section element with Page children</returns>
        public XElement GetPageHierarchy(string id = null)
        {
            try
            {
                _onenote.GetHierarchy(id, HierarchyScope.hsPages, out var xml, XMLSchema.xs2013);
                if (!string.IsNullOrEmpty(xml))
                {
                    return XElement.Parse(xml);
                }

            }
            catch (Exception e)
            {
                var oneNote = ExceptionHelper.TryToWrap(e);
                if (oneNote != null)
                {
                    throw oneNote;
                }

                throw;
            }


            return null;
        }

        public XElement GetPageContent(string id, PageDetail detail)
        {
            try
            {
                _onenote.GetPageContent(id, out string xml, (PageInfo)detail, XMLSchema.xs2013);
                if (!string.IsNullOrEmpty(xml))
                {
                    return XElement.Parse(xml);
                }

            }
            catch (Exception e)
            {
                var oneNote = ExceptionHelper.TryToWrap(e);
                if (oneNote != null)
                {
                    throw oneNote;
                }

                throw;
            }

            return null;
        }

        public void UpdatePageContent(string content, bool force = false, DateTime? dateExpectedLastModified = null)
        {
            try
            {
                _onenote.UpdatePageContent(content, dateExpectedLastModified.GetValueOrDefault(DateTime.MinValue),XMLSchema.xs2013, force);
            }
            catch (Exception e)
            {
                var oneNote = ExceptionHelper.TryToWrap(e);
                if (oneNote != null)
                {
                    throw oneNote;
                }

                throw;
            }
        }


        //private T RunOnOneNote<T>(Func<IApplication, T> action)
        //{
        //    try
        //    {
        //        return action(_onenote);
        //    }
        //    catch (Exception e)
        //    {
        //        var oneNote = ExceptionHelper.TryToWrap(e);
        //        if (oneNote != null)
        //        {
        //            throw oneNote;
        //        }

        //        throw;
        //    }
        //}

    }

    public enum PageDetail
    {
        All = PageInfo.piAll,
        Basic = PageInfo.piBasic,
        BinaryData = PageInfo.piBinaryData,
        BinaryDataFileType = PageInfo.piBinaryDataFileType,
        BinaryDataSelection = PageInfo.piBinaryDataSelection,
        FileType = PageInfo.piFileType,
        Selection = PageInfo.piSelection,
        SelectionFileType = PageInfo.piSelectionFileType
    }
}
