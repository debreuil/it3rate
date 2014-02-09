using System;
using System.Windows.Forms;
using DDW.Display;

namespace DDW
{
	public class LibraryItemDragPacket : IDataObject
	{
		public static string formatName = "DDW.LibraryItemDragPacket";
		public LibraryItem[] Items;
		public Type ContentsType { get { return Items[0].GetType(); } }

		public LibraryItemDragPacket (LibraryItem[] items)
		{
			Items = items;
		}

		// IDataObject
		public object GetData(Type format)
		{
			return Items[0];
		}

		public object GetData(string format)
		{
			return Items[0];
		}

		public object GetData(string format, bool autoConvert)
		{
			return Items[0];
		}

		public bool GetDataPresent(Type format)
		{
			return format == ContentsType;
		}

		public bool GetDataPresent(string format)
		{
			string contentType = LibraryItem.formatName;
			return format == contentType;
		}

		public bool GetDataPresent(string format, bool autoConvert)
		{
			string contentType = LibraryItem.formatName;
			return format == contentType;
		}

		public string[] GetFormats()
		{
			string contentType = LibraryItem.formatName;
			return new string[] { contentType };
		}

		public string[] GetFormats(bool autoConvert)
		{
			string contentType = LibraryItem.formatName;
			return new string[] { contentType };
		}

		public void SetData(object data)
		{
			throw new NotImplementedException();
		}

		public void SetData(Type format, object data)
		{
			throw new NotImplementedException();
		}

		public void SetData(string format, object data)
		{
			throw new NotImplementedException();
		}

		public void SetData(string format, bool autoConvert, object data)
		{
			throw new NotImplementedException();
		}
	}
			
}

