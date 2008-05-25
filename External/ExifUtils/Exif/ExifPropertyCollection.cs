using System;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;

using ExifUtils.Exif.TypeConverters;

namespace ExifUtils.Exif
{
	/// <summary>
	/// Collection of ExifProperty items
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(ExifCollectionConverter))]
	public class ExifPropertyCollection : ICollection<ExifProperty>, ICollection
	{
		#region Fields

		private SortedDictionary<Int32, ExifProperty> items = new SortedDictionary<int, ExifProperty>();

		#endregion Fields

		#region Init

		/// <summary>
		/// Ctor.
		/// </summary>
		public ExifPropertyCollection()
		{
		}

		/// <summary>
		/// Creates a ExifPropertyCollection from a collection of ExifProperties.
		/// Also works as a copy constructor.
		/// </summary>
		/// <param name="properties"></param>
		public ExifPropertyCollection(IEnumerable<ExifProperty> properties)
		{
			if (properties == null)
			{
				return;
			}

			// add all the Exif properties
			foreach (ExifProperty property in properties)
			{
				this.Add(property);
			}
		}

		/// <summary>
		/// Creates a ExifPropertyCollection from a collection of PropertyItems.
		/// </summary>
		/// <param name="propertyItems"></param>
		public ExifPropertyCollection(IEnumerable<PropertyItem> propertyItems)
		{
			if (propertyItems == null)
			{
				return;
			}

			// copy all the Exif properties
			foreach (PropertyItem property in propertyItems)
			{
				if (property.Value != null)
				{
					this.Add(new ExifProperty(property));
				}
			}
		}

		/// <summary>
		/// Creates a ExifPropertyCollection from a collection of PropertyItems only including explicitly named ExifTags.
		/// </summary>
		/// <param name="propertyItems"></param>
		/// <param name="exifTags">additional EXIF tags to include</param>
		public ExifPropertyCollection(IEnumerable<PropertyItem> propertyItems, ICollection<ExifTag> exifTags)
		{
			if (propertyItems == null)
			{
				return;
			}

			// copy all the Exif properties
			foreach (PropertyItem property in propertyItems)
			{
				if (exifTags != null &&
					(!Enum.IsDefined(typeof(ExifTag), property.Id) || !exifTags.Contains((ExifTag)property.Id)))
				{
					continue;
				}

				if (property.Value != null)
				{
					this.Add(new ExifProperty(property));
				}
			}
		}

		#endregion Init

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tagID"></param>
		/// <returns></returns>
		public ExifProperty this[ExifTag tagID]
		{
			get
			{
				if (!this.items.ContainsKey((int)tagID))
				{
					ExifProperty property = new ExifProperty();
					property.Tag = tagID;
					property.Type = ExifDataTypeAttribute.GetExifType(tagID);
					this.items[(int)tagID] = property;
				}
				return this.items[(int)tagID];
			}
			set { this.items[(int)tagID] = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		/// <remarks>
		/// Warning: inefficient, used only for serialization
		/// </remarks>
		public ExifProperty this[int index]
		{
			get
			{
				int[] keys = new int[this.items.Keys.Count];
				this.items.Keys.CopyTo(keys, 0);
				return this.items[keys[index]];
			}
			set { throw new NotSupportedException("This operation is not supported."); }
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public bool Remove(ExifTag tag)
		{
			if (!this.items.ContainsKey((int)tag))
				return false;

			this.items.Remove((int)tag);
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public bool Contains(ExifTag tag)
		{
			return this.items.ContainsKey((int)tag);
		}

		#endregion Methods

		#region ICollection Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Array array, int index)
		{
			((ICollection)this.items).CopyTo(array, index);
		}

		/// <summary>
		/// 
		/// </summary>
		public int Count
		{
			get { return ((ICollection)this.items).Count; }
		}

		bool ICollection.IsSynchronized
		{
			get { return ((ICollection)this.items).IsSynchronized; }
		}

		object ICollection.SyncRoot
		{
			get { return ((ICollection)this.items).SyncRoot; }
		}

		#endregion ICollection Members

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.items.Values).GetEnumerator();
		}

		#endregion IEnumerable Members

		#region ICollection<ExifProperty> Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public void Add(ExifProperty item)
		{
			if (item == null)
				return;

			if (item.Value == null)
			{
				if (this.Contains(item.Tag))
					this.Remove(item.Tag);
				return;
			}

			this.items[item.ID] = item;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Clear()
		{
			this.items.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(ExifProperty item)
		{
			return this.items.ContainsValue(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(ExifProperty[] array, int index)
		{
			this.items.Values.CopyTo(array, index);
		}

		/// <summary>
		/// 
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsReadOnly
		{
			get { return ((ICollection<KeyValuePair<int,ExifProperty>>)this.items).IsReadOnly; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(ExifProperty item)
		{
			if (!this.items.ContainsKey(item.ID))
				return false;

			this.items.Remove(item.ID);
			return true;
		}

		#endregion ICollection<ExifProperty> Members

		#region IEnumerable<ExifProperty> Members

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ExifProperty> GetEnumerator()
		{
			return ((IEnumerable<ExifProperty>)this.items.Values).GetEnumerator();
		}

		#endregion IEnumerable<ExifProperty> Members
	}
}