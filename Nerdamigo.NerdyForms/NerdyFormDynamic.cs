using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nerdamigo.NerdyForms
{
	public class NerdyFormDynamic : DynamicObject
	{
		private Dictionary<string, object> mData;

		public NerdyFormDynamic()
		{
			mData = new Dictionary<string, object>();
		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return mData.Keys;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			if (!mData.ContainsKey(binder.Name))
			{
				mData.Add(binder.Name, value);
			} 
			else
			{
				mData[binder.Name] = value;
			}

			return true;
		}

		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
		{
			if (indexes.Length != 1)
			{
				throw new ArgumentException("Multi-Valued indexes not supported");
			}

			string tKey = indexes[0].ToString();

			if (String.IsNullOrEmpty(tKey))
			{
				throw new ArgumentException("Null or empty index not supported");
			}

			if (!mData.ContainsKey(tKey))
			{
				mData.Add(tKey, value);
			}
			else
			{
				mData[tKey] = value;
			}

			return true;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			mData.TryGetValue(binder.Name, out result);
			return true;
		}
	}
}
