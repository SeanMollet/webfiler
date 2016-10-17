using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebFiler.Resources;

namespace WebFiler
{
	#region Comments
	/// <summary>
	/// Create and return a table of files and folders.
	/// </summary>
	/// <remarks>
	/// <h3>Changes</h3>
	/// <list type="table">
	/// 	<listheader>
	/// 		<th>Author</th>
	/// 		<th>Date</th>
	/// 		<th>Details</th>
	/// 	</listheader>
	/// 	<item>
	/// 		<term>Mark Merrens</term>
	/// 		<description>17/03/2010</description>
	/// 		<description>Created.</description>
	/// 	</item>
	/// </list>
	/// </remarks>
	#endregion

	public class TableEx
	{
		#region Properties
		/// <summary>
		/// Gets or sets the _data.
		/// </summary>
		/// <value>The _data.</value>
		string[] _data { get; set; }

		/// <summary>
		/// Gets or sets the _root path.
		/// </summary>
		/// <value>The _root.</value>
		string _root { get; set; }

		/// <summary>
		/// Gets or sets the _page.
		/// </summary>
		/// <value>The _page.</value>
		Page _page { get; set; }

		/// <summary>
		/// Gets or sets the name of the display page.
		/// </summary>
		/// <value>The _display page.</value>
		string _displayPage { get; set; }
		#endregion

		#region Private Members
		/// <summary>
		/// The table object.
		/// </summary>
		Table _table;
		#endregion

		#region Construction
		/// <summary>
		/// Initializes a new instance of the <see cref="TableEx"/> class.
		/// </summary>
		/// <param name="Page">The page.</param>
		/// <param name="Root">The root path.</param>
		public TableEx(Page Page, string Root)
		{
			// Populate the local members.
			_page = Page;
			_displayPage = _page.Request.FilePath;
			_root = Root;

			// Get a list of files and folders from the root.
			string[] files = Directory.GetFiles(_root);
			string[] folders = Directory.GetDirectories(_root);
			_data = new string[files.Length + folders.Length];
			folders.CopyTo(_data, 0);
			files.CopyTo(_data, folders.Length);			
		}
		#endregion

		#region Table Methods
		/// <summary>
		/// Create and return the table.
		/// </summary>
		/// <returns>Table</returns>
		public Table Create()
		{
			// Create the table.
			_table = new Table();

			// Create the header row(s).
			Header();
			RootRow();

			// Create the data rows.
			DataRows();

			// Decorate the table.
			_table.BorderColor = Color.DimGray;
			_table.BorderStyle = BorderStyle.Solid;
			_table.BorderWidth = Unit.Pixel(1);
			_table.CellPadding = 3;
			_table.GridLines = GridLines.Both;
			_table.Style.Add(Strings.BorderCollapse, Strings.Collapse);

			// Table complete: return.
			return _table;
		}

		/// <summary>
		/// Create the header row.
		/// </summary>
		void Header()
		{
			TableRow row = new TableRow();
			TableCell cell;

			cell = new TableCell();
			cell.Text = Strings.Name;
			row.Cells.Add(cell);

			cell = new TableCell();
			cell.HorizontalAlign = HorizontalAlign.Right;
			cell.Text = Strings.Size;
			row.Cells.Add(cell);

			cell = new TableCell();
			cell.Text = Strings.Type;
			row.Cells.Add(cell);

			cell = new TableCell();
			cell.HorizontalAlign = HorizontalAlign.Center;
			cell.Text = Strings.RO;
			cell.ToolTip = Strings.FileIsReadOnly;
			row.Cells.Add(cell);

			cell = new TableCell();
			cell.Text = Strings.Accessed;
			row.Cells.Add(cell);

			cell = new TableCell();
			cell.Text = Strings.Modified;
			row.Cells.Add(cell);

			cell = new TableCell();
			cell.Text = Strings.Actions;
			cell.HorizontalAlign = HorizontalAlign.Center;
			row.Cells.Add(cell);

			row.BackColor = Color.LightGray;
			_table.Rows.AddAt(0, row);
		}

		/// <summary>
		/// If below the root give a way to return.
		/// </summary>
		void RootRow()
		{
			// The root from the config.
            string primary = Configuration._root;

			// If it doesn't match where we are now then create a new row.
			if (primary != _root)
			{
				// Create the row.
				TableRow row = new TableRow();
				TableCell cell;

				// Cell to hold the image and hyperlink.
				cell = new TableCell();

				// Get the folder image.
				System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
				img.ImageUrl = _page.ClientScript.GetWebResourceUrl(typeof(WebFiler.Filer), Strings.PNG_FOLDER_UP);
				img.ImageAlign = ImageAlign.AbsBottom;
				img.Style.Add(Strings.PaddingRight, Strings.FivePX);
				cell.Controls.AddAt(0, img);

				// Get the parent directory.
				DirectoryInfo di = new DirectoryInfo(_root);
				string parent = di.Parent.FullName;

				// Add the url.
				// Create the hyperlink.
				HyperLink hl = new HyperLink();
				hl.Text = Strings.FolderUpText;
				hl.NavigateUrl =
					string.Format(
						CultureInfo.InvariantCulture,
						Strings.RootNavigate,
						_displayPage,
						UrlEncoding.Encode(parent));

				cell.Controls.AddAt(1, hl);

				// Add the composite cell to the row.
				row.Cells.Add(cell);

				// Add dummy cells.
				cell = new TableCell();
				row.Cells.Add(cell);
				cell = new TableCell();
				row.Cells.Add(cell);
				cell = new TableCell();
				row.Cells.Add(cell);
				cell = new TableCell();
				row.Cells.Add(cell);
				cell = new TableCell();
				row.Cells.Add(cell);
				cell = new TableCell();
				row.Cells.Add(cell);

				// And add the row to the table.
				_table.Rows.AddAt(1, row);

				// Show the home button.
				((Button)_page.FindControl("btnHome")).Visible = true;
			}
		}

		/// <summary>
		/// Creates data rows from each file and folder item.
		/// </summary>
		void DataRows()
		{
			TableRow row;
			TableCell cell;
			FileInfo fi;
			HyperLink hl;

			foreach (string item in _data)
			{
				// Will need some file info.
				fi = new FileInfo(item);

				// If the item attributes are Hidden or System, ignore.
				if ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden || (fi.Attributes & FileAttributes.System) == FileAttributes.System)
				{
					continue;
				} 

				// New row for each row found.
				row = new TableRow();

                if ((fi.Attributes & FileAttributes.Directory )== FileAttributes.Directory)
				{
					// New cells for each item found.
					cell = new TableCell();

					// Get the folder image.
					System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
					img.ImageUrl = _page.ClientScript.GetWebResourceUrl(typeof(WebFiler.Filer), Strings.PNG_FOLDER);
					img.ImageAlign = ImageAlign.AbsBottom;
					img.Style.Add(Strings.PaddingRight, Strings.FivePX);
					cell.Controls.AddAt(0, img);

					// Create the hyperlink.
					hl = new HyperLink();
					hl.Text = fi.Name;
					hl.NavigateUrl =
					    string.Format(
					        CultureInfo.InvariantCulture,
					        Strings.RootNavigate,
					        _displayPage,
					        UrlEncoding.Encode(fi.FullName));

					// Add the url.
					cell.Controls.AddAt(1, hl);

					// Add the composite cell to the row.
					row.Cells.Add(cell);
				}
				else
				{
					// Open the file to view as appropriate.
					cell = new TableCell();

					// Get the file type image.
					System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
					img.ImageUrl = _page.ClientScript.GetWebResourceUrl(typeof(WebFiler.Filer), Strings.PNG_FILE);
					img.ImageAlign = ImageAlign.AbsBottom;
					img.Style.Add(Strings.PaddingRight, Strings.FivePX);
					cell.Controls.AddAt(0, img);

					// Create the hyperlink.
					hl = new HyperLink();
					hl.Text = fi.Name;
					hl.NavigateUrl = 
					    string.Format(
					        CultureInfo.InvariantCulture,
					        Strings.FileOpen,
					        UrlEncoding.Encode(fi.FullName));

					cell.Controls.AddAt(1, hl);

					// Add the composite cell to the row.
					row.Cells.Add(cell);
				}

				// The size of the file.
				cell = new TableCell();
				cell.HorizontalAlign = HorizontalAlign.Right;
				// 20100601: Fix from Tony Hecht via CodeProject: original code failed when looking at compressed folders.
				cell.Text = ((fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory) ? string.Empty : FormatFileSize(fi.Length);
				row.Cells.Add(cell);

				// The type of file: if a file has no extension display as 'unknown'.
				cell = new TableCell();
				cell.Text =
					(string.IsNullOrEmpty(fi.Extension))
						? ((fi.Attributes== FileAttributes.Directory) ? Strings.Folder : Strings.Unknown)
						: fi.Extension.Replace(Strings.Period, string.Empty).ToLowerInvariant();
				row.Cells.Add(cell);

				// Is the file readonly?
				cell = new TableCell();
				cell.HorizontalAlign = HorizontalAlign.Center;
				cell.VerticalAlign = VerticalAlign.Middle;
				cell.Text = (fi.IsReadOnly) ? Strings.IsReadOnly : string.Empty;
				cell.ForeColor = Color.MidnightBlue;
				cell.Font.Bold = true;
				row.Cells.Add(cell);

				// Last access time.
				cell = new TableCell();
				cell.Text = fi.LastAccessTime.ToString();
				row.Cells.Add(cell);

				// Last modified time.
				cell = new TableCell();
				cell.Text = fi.LastWriteTime.ToString();
				row.Cells.Add(cell);

				// Action buttons.
				ImageButton btn;
				cell = new TableCell();				

				// Rename.
				btn = new ImageButton();
				btn.ImageUrl = _page.ClientScript.GetWebResourceUrl(typeof(WebFiler.Filer), Strings.PNG_RENAME);
				btn.ToolTip = Strings.Rename;
				btn.Command += new CommandEventHandler(Rename);
				btn.CommandArgument = fi.FullName;
				btn.Style.Add(Strings.PaddingRight, Strings.ThreePX);
				cell.Controls.AddAt(0, btn);

				// Delete.
				btn = new ImageButton();
				btn.ImageUrl = _page.ClientScript.GetWebResourceUrl(typeof(WebFiler.Filer), Strings.PNG_DELETE);
				btn.ToolTip = Strings.Delete;
				btn.Command += new CommandEventHandler(Delete);
				btn.CommandArgument = fi.FullName;
				string deleteMsg =
					string.Format(
						CultureInfo.InvariantCulture,
						Strings.DeleteMessage,
						fi.Name);
				ConfirmButton(btn, deleteMsg);				
				cell.Controls.AddAt(1, btn);
				row.Cells.Add(cell);
				
				// Add the row to the table.
				_table.Rows.Add(row);
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Renames the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		void Rename(object sender, CommandEventArgs e)
		{
			string file = e.CommandArgument.ToString();
			string renameUrl =
			    string.Format(
			        CultureInfo.InvariantCulture,
			        Strings.RenameUrl,
			        UrlEncoding.Encode(_root),
			        UrlEncoding.Encode(file));
			HttpContext.Current.Response.Redirect(renameUrl, true);		
		}

		/// <summary>
		/// Deletes the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		void Delete(object sender, CommandEventArgs e)
		{
			string file = e.CommandArgument.ToString();

			FileInfo fi = new FileInfo(file);

			// Determine type: folder or file.
			if (fi.Attributes == FileAttributes.Directory)
			{
				ReadOnlyFolderDelete(file);
			}
			else
			{
				// Remove all attributes first otherwise
				// this will fail against readonly files.
				fi.Attributes = FileAttributes.Normal;
				File.Delete(file);
			}

			// Send back to self to refresh.
			string deleteUrl =
			    string.Format(
			        CultureInfo.InvariantCulture,
			        Strings.DeleteUrl,
			        UrlEncoding.Encode(_root));
			HttpContext.Current.Response.Redirect(deleteUrl, true);
		} 
		#endregion

		#region Helper Methods
		/// <summary>
		/// Formats the size of the file.
		/// </summary>
		/// <param name="Bytes">The bytes.</param>
		/// <returns>string</returns>
		static string FormatFileSize(long Bytes)
		{
			Decimal size = 0;
			string result;

			if (Bytes >= 1073741824)
			{
				size = Decimal.Divide(Bytes, 1073741824);
				result = 
					String.Format(
						CultureInfo.InvariantCulture,
						Strings.FFSGB,
						size);
			}
			else if (Bytes >= 1048576)
			{
				size = Decimal.Divide(Bytes, 1048576);
				result = 
					String.Format(
						CultureInfo.InvariantCulture,
						Strings.FFSMB,
						size);
			}
			else if (Bytes >= 1024)
			{
				size = Decimal.Divide(Bytes, 1024);
				result = 
					String.Format(
						CultureInfo.InvariantCulture,
						Strings.FFSKB,
						size);
			}
			else if (Bytes > 0 & Bytes < 1024)
			{
				size = Bytes;
				result = 
					String.Format(
						CultureInfo.InvariantCulture,
						Strings.FFSB,
						size);
			}
			else
			{
				result = Strings.FFS0;
			}

			return result;
		}

		/// <summary>
		/// Add a javascript confirm to a <see cref="WebControl"/>.
		/// </summary>
		/// <param name="control">Would normally expect this to be a <see cref="Button"/>.</param>
		/// <param name="Message">The message to display.</param>
		static void ConfirmButton(WebControl Control, string Message)
		{
			if (Control != null)
			{
				string confirm = 
					string.Format(
						CultureInfo.InvariantCulture,
						Strings.JSConfirm,
						Message);
				Control.Attributes.Add(Strings.OnClick, confirm);
			}
		}

		/// <summary>
		/// Will delete a readonly folder and all sub-folders and files contained therein.
		/// </summary>
		/// <param name="Path">The path.</param>
		static void ReadOnlyFolderDelete(string Path)
		{
			DirectoryInfo di = new DirectoryInfo(Path);
			Stack<DirectoryInfo> folders = new Stack<DirectoryInfo>();
			DirectoryInfo folder;

			// Add to the stack.
			folders.Push(di);

			while (folders.Count > 0)
			{ 
				// Get the folder and set all attributes to normal.
				folder = folders.Pop();
				folder.Attributes = FileAttributes.Normal;

				// Add to the stack.
				foreach (DirectoryInfo dir in folder.GetDirectories())
				{
					folders.Push(dir);
				}

				// Set and delete all of the files.
				foreach (FileInfo fi in folder.GetFiles())
				{
					fi.Attributes = FileAttributes.Normal;
					fi.Delete();
				}
			}

			// Delete the folder and all sub-folders.
			di.Delete(true);
		}
		#endregion
	}
}