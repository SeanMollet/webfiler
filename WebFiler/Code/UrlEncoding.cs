using System.Web;

namespace WebFiler
{
	#region Comments
	/// <summary>
	/// Url Decode/Encode.
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

	sealed class UrlEncoding
	{
		/// <summary>
		/// Returns a UrlDecoded object.
		/// </summary>
		/// <param name="Data">The object to decode.</param>
		/// <returns>string</returns>
		public static string Decode(string Data)
		{
			string decode = (string.IsNullOrEmpty(Data)) ? string.Empty : Data;
			return HttpContext.Current.Server.UrlDecode(decode);
		}

		/// <summary>
		/// Returns a UrlEncoded string.
		/// </summary>
		/// <param name="Data">The string to encode.</param>
		/// <returns>string</returns>
		public static string Encode(string Data)
		{
			return HttpContext.Current.Server.UrlEncode(Data);
		}
	}
}