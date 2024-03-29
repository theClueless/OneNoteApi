﻿using System;
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
    /// <summary>
	/// Wraps the OneNote interop API
	/// </summary>
	/// <see cref="https://docs.microsoft.com/en-us/office/client-developer/onenote/application-interface-onenote"/>
	internal class OneNoteRef : IDisposable
	{
		public enum ExportFormat
		{
			EMF = PublishFormat.pfEMF,
			HTML = PublishFormat.pfHTML,
			MHTML = PublishFormat.pfMHTML,
			OneNote = PublishFormat.pfOneNote,
			OneNote2007 = PublishFormat.pfOneNote2007,
			OneNotePackage = PublishFormat.pfOneNotePackage,
			PDF = PublishFormat.pfPDF,
			Word = PublishFormat.pfWord,
			XPS = PublishFormat.pfXPS,
			XML = 1000
		}

		


		public const string Prefix = "one";

		private IApplication onenote;
		private bool disposed;

		// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
		// Constructors...

		/// <summary>
		/// Initialize a new wrapper
		/// </summary>
		public OneNoteRef()
		{
			onenote = new Application();
		}


		///// <summary>
		///// Initialize a new wrapper and return the current page
		///// </summary>
		///// <param name="page">The current Page</param>
		///// <param name="ns">The namespace of the current page</param>
		///// <param name="detail">The desired verbosity of the XML</param>
		//public OneNote(out Page page, out XNamespace ns, PageDetail detail = PageDetail.Selection)
		//	: this()
		//{
		//	page = GetPage(detail);
		//	ns = page.Namespace;
		//}


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


		// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
		// Properties

		/// <summary>
		/// Gets the currently viewed page ID
		/// </summary>
		public string CurrentPageId => onenote.Windows.CurrentWindow?.CurrentPageId;


		/// <summary>
		/// Gets the currently viewed section ID
		/// </summary>
		public string CurrentSectionId => onenote.Windows.CurrentWindow?.CurrentSectionId;


		/// <summary>
		/// Gets the currently viewed notebook ID
		/// </summary>
		public string CurrentNotebookId => onenote.Windows.CurrentWindow?.CurrentNotebookId;


		/// <summary>
		/// Gets the handle of the current window
		/// </summary>
		public IntPtr WindowHandle => (IntPtr)onenote.Windows.CurrentWindow.WindowHandle;


		// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

		/// <summary>
		/// Invoke an action with retry
		/// </summary>
		/// <param name="work">The action to invoke</param>
		public async Task InvokeWithRetry(Action work)
		{
			try
			{
				int retries = 0;
				while (retries < 3)
				{
					try
					{
						work();

						if (retries > 0)
						{
							// logger.WriteLine($"completed successfully after {retries} retries");
						}

						retries = int.MaxValue;
					}
					catch (COMException exc) when ((uint)exc.ErrorCode == ErrorCodes.hrCOMBusy)
					{
						retries++;
						var ms = 250 * retries;

						// logger.WriteLine($"OneNote is busy, retyring in {ms}ms", exc);
						await Task.Delay(ms);
					}
				}
			}
			catch (Exception exc)
			{
				// logger.WriteLine("error invoking action", exc);
			}
		}


		// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
		// Create...

		/// <summary>
		/// Creates a new page in the specified section
		/// </summary>
		/// <param name="sectionId">The section to contain the new page</param>
		/// <param name="pageId">The ID of the new page</param>
		public void CreatePage(string sectionId, out string pageId)
		{
			onenote.CreateNewPage(sectionId, out pageId);
		}


		/// <summary>
		/// Create a new section in the current notebook immediately after the open section
		/// </summary>
		/// <param name="name">The name of the new section</param>
		/// <returns>The Section element</returns>
		public XElement CreateSection(string name)
		{
			// find the current section in this notebook (may be in section group)
			var notebook = GetNotebook();
			var ns = GetNamespace(notebook);
			var current = notebook.Descendants(ns + "Section")
				.FirstOrDefault(e => e.Attribute("isCurrentlyViewed")?.Value == "true");

			XElement parent;
			var sectionIds = new List<string>();

			if (current == null)
			{
				// add first section to notebook
				notebook.Add(new XElement(ns + "Section", new XAttribute("name", name)));
				parent = notebook;
			}
			else
			{
				// get the parent of the current section (may be the notebook or a section group)
				// so we will then only update that parent rather than the entire hierarchy
				parent = current.Parent;

				// collect all section IDs for comparison
				sectionIds = parent.Elements(ns + "Section")
					.Where(e => e.Attribute("isRecycleBin") == null &&
								e.Attribute("isInRecycleBin") == null)
					.Attributes("ID").Select(a => a.Value).ToList();

				// add the new section (won't know the ID yet)
				current.AddAfterSelf(new XElement(ns + "Section", new XAttribute("name", name)));
			}

			// udpate the notebook or section group
			UpdateHierarchy(parent);

			// get the parent again so we can find the new section ID
			onenote.GetHierarchy(
				parent.Attribute("ID").Value,
				HierarchyScope.hsSections, out var xml, XMLSchema.xs2013);

			parent = XElement.Parse(xml);

			// compare the new parent with old section IDs to find the new ID
			var section = parent.Elements(ns + "Section")
				.Where(e => e.Attribute("isRecycleBin") == null &&
							e.Attribute("isInRecycleBin") == null)
				.FirstOrDefault(e => !sectionIds.Contains(e.Attribute("ID").Value));

			return section;
		}


		// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
		// Get...

		/// <summary>
		/// Get the known paths used by OneNote; this is for diagnostic logging
		/// </summary>
		/// <returns></returns>
		public (string backupFolder, string defaultFolder, string unfiledFolder) GetFolders()
		{
			onenote.GetSpecialLocation(SpecialLocation.slBackUpFolder, out var backupFolder);
			onenote.GetSpecialLocation(SpecialLocation.slDefaultNotebookFolder, out var defaultFolder);
			onenote.GetSpecialLocation(SpecialLocation.slUnfiledNotesSection, out var unfiledFolder);
			return (backupFolder, defaultFolder, unfiledFolder);
		}


		/// <summary>
		/// Gets a onenote:hyperline to an object on the specified page
		/// </summary>
		/// <param name="pageId">The ID of a page</param>
		/// <param name="objectId">
		/// The ID of an object on the page or string.Empty to link to the page itself
		/// </param>
		/// <returns></returns>
		public string GetHyperlink(string pageId, string objectId)
		{
			onenote.GetHyperlinkToObject(pageId, objectId, out var hyperlink);
			return hyperlink;
		}


		/// <summary>
		/// Creates a map of pages where the key is built from the page-id of an
		/// internal onenote hyperlink and the value is the actual pageId
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="countCallback"></param>
		/// <param name="stepCallback"></param>
		/// <returns>
		/// A Dictionary with keys build from URI params and values specifying the page IDs
		/// </returns>
		/// <remarks>
		/// There's no direct way to map onenote:http URIs to page IDs so this creates a cache
		/// of all pages in the specified scope with their URIs as keys and pageIDs as values
		/// </remarks>
		public Dictionary<string, string> BuildHyperlinkCache(
			Scope scope,
			CancellationToken token,
			Action<int> countCallback = null,
			Action stepCallback = null)
		{
			var hyperlinks = new Dictionary<string, string>();

			XElement container;
			switch (scope)
			{
				case Scope.Notebooks: container = GetNotebooks(Scope.Pages); break;
				case Scope.Sections: container = GetNotebook(Scope.Pages); break;
				default: container = GetSection(); break;
			}

			// ignore the recycle bin
			container.Elements()
				.Where(e => e.Attributes().Any(a => a.Name == "isRecycleBin"))
				.Remove();

			var pageIDs = container.Descendants(GetNamespace(container) + "Page")
				.Select(e => e.Attribute("ID").Value)
				.ToList();

			if (pageIDs.Count > 0)
			{
				countCallback?.Invoke(pageIDs.Count);

				var regex = new Regex(@"page-id=({[^}]+?})");

				foreach (var pageID in pageIDs)
				{
					if (token.IsCancellationRequested)
					{
						break;
					}

					var link = GetHyperlink(pageID, string.Empty);
					var matches = regex.Match(link);
					if (matches.Success)
					{
						hyperlinks.Add(matches.Groups[1].Value, pageID);
					}

					stepCallback?.Invoke();
				}
			}

			return hyperlinks;
		}


		/// <summary>
		/// Gets the OneNote namespace for the given element
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public XNamespace GetNamespace(XElement element)
		{
			return element.GetNamespaceOfPrefix(Prefix);
		}


		/// <summary>
		/// Gets the current notebook with a hierarchy of sections
		/// </summary>
		/// <returns>A Notebook element with Section children</returns>
		public XElement GetNotebook(Scope scope = Scope.Sections)
		{
			return GetNotebook(CurrentNotebookId, scope);
		}


		/// <summary>
		/// Get the spcified notebook with a hierarchy of sections
		/// </summary>
		/// <param name="id">The ID of the notebook</param>
		/// <returns>A Notebook element with Section children</returns>
		public XElement GetNotebook(string id, Scope scope = Scope.Sections)
		{
			if (!string.IsNullOrEmpty(id))
			{
				onenote.GetHierarchy(id, (HierarchyScope)scope, out var xml, XMLSchema.xs2013);
				if (!string.IsNullOrEmpty(xml))
				{
					return XElement.Parse(xml);
				}
			}

			return null;
		}


		/// <summary>
		/// Gets a root note containing Notebook elements
		/// </summary>
		/// <returns>A Notebooks element with Notebook children</returns>
		public XElement GetNotebooks(Scope scope = Scope.Notebooks)
		{
			// find the ID of the current notebook
			onenote.GetHierarchy(
				string.Empty, (HierarchyScope)scope, out var xml, XMLSchema.xs2013);

			if (!string.IsNullOrEmpty(xml))
			{
				return XElement.Parse(xml);
			}

			return null;
		}


		/// <summary>
		/// Gets the current page.
		/// </summary>
		/// <param name="detail">The desired verbosity of the XML</param>
		/// <returns>A Page containing the root XML of the page</returns>
		public XElement GetPage(PageDetail detail = PageDetail.Selection)
		{
			return GetPage(CurrentPageId, detail);
		}


		/// <summary>
		/// Gets the specified page.
		/// </summary>
		/// <param name="pageId">The unique ID of the page</param>
		/// <param name="detail">The desired verbosity of the XML</param>
		/// <returns>A Page containing the root XML of the page</returns>
		public XElement GetPage(string pageId, PageDetail detail = PageDetail.All)
		{
			if (string.IsNullOrEmpty(pageId))
			{
				return null;
			}

			onenote.GetPageContent(pageId, out var xml, (PageInfo)detail, XMLSchema.xs2013);
			if (!string.IsNullOrEmpty(xml))
			{
				return new XElement(xml);
			}

			return null;
		}


		/// <summary>
		/// Gets the raw XML of the specified page.
		/// </summary>
		/// <param name="pageId">The unique ID of the page</param>
		/// <param name="detail">The desired verbosity of the XML</param>
		/// <returns>A string specifying the root XML of the page</returns>
		public string GetPageXml(string pageId, PageDetail detail = PageDetail.Basic)
		{
			if (string.IsNullOrEmpty(pageId))
			{
				return null;
			}

			onenote.GetPageContent(pageId, out var xml, (PageInfo)detail, XMLSchema.xs2013);
			return xml;
		}


		///// <summary>
		///// Gets the name, file path, and OneNote hyperlink to the current page;
		///// used to build up Favorites
		///// </summary>
		///// <returns></returns>
		//public (string Name, string Path, string Link) GetPageInfo()
		//{
		//	var page = GetPage(PageDetail.Basic);
		//	if (page == null)
		//	{
		//		return (null, null, null);
		//	}

		//	// name
		//	var name = page.Attribute("name")?.Value;

		//	// path
		//	string path = null;
		//	var section = GetSection();
		//	if (section != null)
		//	{
		//		var uri = section.Attribute("path")?.Value;
		//		if (!string.IsNullOrEmpty(uri))
		//		{
		//			path = "/" + Path.Combine(
		//				Path.GetFileName(Path.GetDirectoryName(uri)),
		//				Path.GetFileNameWithoutExtension(uri),
		//				name
		//				).Replace("\\", "/");
		//		}
		//	}

		//	// link
		//	string link = GetHyperlink(page.PageId, string.Empty);

		//	return (name, path, link);
		//}


		/// <summary>
		/// Gets the ID of the parent hierachy object that owns the specified object; used when
		/// copying/moving pages from section to section
		/// </summary>
		/// <param name="objectId">The ID of the object whose parent you want to find</param>
		/// <returns>The ID of the parent or null if not found (e.g. parent of a notebook)</returns>
		public string GetParent(string objectId)
		{
			try
			{
				onenote.GetHierarchyParent(objectId, out var parentId);
				return parentId;
			}
			catch
			{
				return null;
			}
		}


		/// <summary>
		/// Gest the current section and its child page hierarchy
		/// </summary>
		/// <returns>A Section element with Page children</returns>
		public XElement GetSection()
		{
			return GetSection(CurrentSectionId);
		}


		/// <summary>
		/// Gest the specified section and its child page hierarchy
		/// </summary>
		/// <returns>A Section element with Page children</returns>
		public XElement GetSection(string id)
		{
			if (!string.IsNullOrEmpty(id))
			{
				onenote.GetHierarchy(id, HierarchyScope.hsPages, out var xml, XMLSchema.xs2013);
				if (!string.IsNullOrEmpty(xml))
				{
					return XElement.Parse(xml);
				}
			}

			return null;
		}



		// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
		// Update...

		/// <summary>
		/// Deletes the given object from the specified page
		/// </summary>
		/// <param name="pageId">The ID of the page to modify</param>
		/// <param name="objectId">The ID of the object to remove from the page</param>
		public void DeleteContent(string pageId, string objectId)
		{
			try
			{
				onenote.DeletePageContent(pageId, objectId);
			}
			catch (Exception exc)
			{
				//logger.WriteLine($"error deleting page object {objectId}", exc);
			}
		}


		/// <summary>
		/// Deletes the given object(s) from the hierarchy; used for merging
		/// </summary>
		/// <param name="element"></param>
		public void DeleteHierarchy(string objectId)
		{
			try
			{
				onenote.DeleteHierarchy(objectId);
			}
			catch (Exception exc)
			{
				//logger.WriteLine($"error deleting hierarchy object {objectId}", exc);
			}
		}


		/// <summary>
		/// Sync updates to storage
		/// </summary>
		/// <param name="id">
		/// The ID of the notebook to sync or the current notebook by default
		/// </param>
		public async Task Sync(string id = null)
		{
			await InvokeWithRetry(() =>
			{
				onenote.SyncHierarchy(id ?? CurrentNotebookId);
			});
		}


		///// <summary>
		///// Update the current page.
		///// </summary>
		///// <param name="page">A Page</param>
		//public async Task Update(Page page)
		//{
		//	await Update(page.Root);
		//}


		/// <summary>
		/// Updates the given content, with a unique ID, on the current page.
		/// </summary>
		/// <param name="element">A page or element within a page with a unique objectID</param>
		public async Task Update(XElement element)
		{
			var xml = element.ToString(SaveOptions.DisableFormatting);

			await InvokeWithRetry(() =>
			{
				onenote.UpdatePageContent(xml, DateTime.MinValue, XMLSchema.xs2013, true);
			});
		}


		/// <summary>
		/// Update the hierarchy info with the given XML; used for sorting
		/// </summary>
		/// <param name="element"></param>
		public void UpdateHierarchy(XElement element)
		{
			string xml = element.ToString(SaveOptions.DisableFormatting);

			try
			{
				onenote.UpdateHierarchy(xml);
			}
			catch (Exception exc)
			{
				//logger.WriteLine("error updating hierarchy", exc);
				//logger.WriteLine(element);
				//logger.WriteLine();
			}
		}


		// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
		// QuickFiling...

		/// <summary>
		/// A callback delegate for consumers who want to know the selected item in the
		/// QuickFiling dialog.
		/// </summary>
		/// <param name="nodeId"></param>
		public delegate Task SelectLocationCallback(string nodeId);


		/// <summary>
		/// Presents the QuickFiling dialog to the user and invokes the given callback
		/// when completed.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="description"></param>
		/// <param name="scope"></param>
		/// <param name="callback"></param>
		public void SelectLocation(
			string title, string description, Scope scope, SelectLocationCallback callback)
		{
			var dialog = onenote.QuickFiling();
			dialog.Title = title;
			dialog.Description = description;
			dialog.ParentWindowHandle = onenote.Windows.CurrentWindow.WindowHandle;

			switch (scope)
			{
				case Scope.Notebooks:
					dialog.TreeDepth = HierarchyElement.heNotebooks;
					break;
				case Scope.Sections:
					dialog.TreeDepth = HierarchyElement.heSections;
					break;
				case Scope.Pages:
					dialog.TreeDepth = HierarchyElement.hePages;
					break;
			}

			//dialog.AddButton(
			//	Resx.OK, HierarchyElement.heSections, HierarchyElement.heSections, false);

			dialog.Run(new FilingCallback(callback));
		}


		private class FilingCallback : IQuickFilingDialogCallback
		{
			private readonly SelectLocationCallback userCallback;

			public FilingCallback(SelectLocationCallback usercb)
			{
				userCallback = usercb;
			}

			public void OnDialogClosed(IQuickFilingDialog dialog)
			{
				userCallback(dialog.SelectedItem);
			}
		}


		// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
		// Utilities...

		/// <summary>
		/// Exports the specified page to a file using the given format
		/// </summary>
		/// <param name="pageId">The page ID</param>
		/// <param name="path">The output file path</param>
		/// <param name="format">The format</param>
		public void Export(string pageId, string path, ExportFormat format)
		{
			onenote.Publish(pageId, path, (PublishFormat)format, string.Empty);
		}


		/// <summary>
		/// Imports the specified file under the current section.
		/// </summary>
		/// <param name="path">The path to a .one file</param>
		/// <returns>The ID of the new hierarchy object (pageId)</returns>
		public async Task<string> Import(string path)
		{
			var start = GetSection();

			// Opening a .one file places its content in the transient OpenSections area
			// with its own notebook structure; need to dive down to find the page...
			//
			// OpenHierarchy, followed by SyncHierarchy should load the .one file and then
			// GetSection would load the hierarchy down to the page level. But the one:Section
			// has an attribute areAllPagesAvailable=false which means the page isn't loaded...
			// Instead use MergeSections to force loading the pages and merge into current section

			string openSectionId = null;

			await InvokeWithRetry(() =>
			{
				onenote.OpenHierarchy(path, null, out openSectionId, CreateFileType.cftSection);
			});

			if (string.IsNullOrEmpty(openSectionId))
			{
				return null;
			}

			await InvokeWithRetry(() =>
			{
				onenote.MergeSections(openSectionId, CurrentSectionId);
			});

			var section = GetSection();
			var ns = GetNamespace(section);

			// determine newly added pageId by comparing new section against what we started with
			var pageId = section.Descendants(ns + "Page")
				.Select(e => e.Attribute("ID").Value)
				.Except(start.Descendants(ns + "Page").Select(e => e.Attribute("ID").Value))
				.FirstOrDefault();

			return pageId;
		}


		/// <summary>
		/// Forces OneNote to jump to the specified object, onenote Uri, or Web Uri
		/// </summary>
		/// <param name="uri">A pageId, sectionId, notebookId, onenote:URL, or Web URL</param>
		public async Task NavigateTo(string uri)
		{
			if (uri.StartsWith("onenote:") || uri.StartsWith("http"))
			{
				await InvokeWithRetry(() =>
				{
					onenote.NavigateToUrl(uri);
				});
			}
			else
			{
				await InvokeWithRetry(() =>
				{
					// must be an ID
					onenote.NavigateTo(uri);
				});
			}
		}


		/// <summary>
		/// Search pages under the specified hierarchy node using the given query.
		/// </summary>
		/// <param name="nodeId"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public string Search(string nodeId, string query)
		{
			onenote.FindPages(nodeId, query, out var xml, false, false, XMLSchema.xs2013);
			return xml;
		}


		/// <summary>
		/// Search the hierarchy node for the specified meta tag
		/// </summary>
		/// <param name="nodeId">The root node: notebook, section, or page</param>
		/// <param name="name">The search string, meta key name</param>
		/// <returns>A hierarchy XML starting at the given node.</returns>
		public async Task<XElement> SearchMeta(string nodeId, string name)
		{
			string xml = null;

			await InvokeWithRetry(() =>
			{
				onenote.FindMeta(nodeId, name, out xml, false, XMLSchema.xs2013);
			});

			return XElement.Parse(xml);
		}


		/// <summary>
		/// Special helper for DiagnosticsCommand
		/// </summary>
		/// <param name="builder"></param>
		public void ReportWindowDiagnostics(StringBuilder builder)
		{
			var win = onenote.Windows.CurrentWindow;

			builder.AppendLine($"CurrentNotebookId: {win.CurrentNotebookId}");
			builder.AppendLine($"CurrentPageId....: {win.CurrentPageId}");
			builder.AppendLine($"CurrentSectionId.: {win.CurrentSectionId}");
			builder.AppendLine($"CurrentSecGrpId..: {win.CurrentSectionGroupId}");
			builder.AppendLine($"DockedLocation...: {win.DockedLocation}");
			builder.AppendLine($"IsFullPageView...: {win.FullPageView}");
			builder.AppendLine($"IsSideNote.......: {win.SideNote}");
			builder.AppendLine();

			builder.AppendLine($"Windows ({onenote.Windows.Count})");

			var e = onenote.Windows.GetEnumerator();
			while (e.MoveNext())
			{
				var window = e.Current as Window;

				//var threadId = Native.GetWindowThreadProcessId(
				//	(IntPtr)window.WindowHandle,
				//	out var processId);

				//builder.Append($"- window [processId:{processId}, threadId:{threadId}]");
				builder.Append($" handle:{window.WindowHandle:x} active:{window.Active}");

				if (window.WindowHandle == onenote.Windows.CurrentWindow.WindowHandle)
				{
					builder.AppendLine(" (current)");
				}
			}
		}
	}
}
