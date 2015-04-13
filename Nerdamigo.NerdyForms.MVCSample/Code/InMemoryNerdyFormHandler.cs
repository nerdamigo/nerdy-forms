using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace Nerdamigo.NerdyForms.MVCSample.Code
{
	public class InMemoryNerdyFormHandler : INerdyFormHandler
	{
		private static ConcurrentQueue<object> mSubmissions;
		static InMemoryNerdyFormHandler()
		{
			mSubmissions = new ConcurrentQueue<object>();
		}

		public static List<object> Submissions
		{
			get
			{
				List<object> tSubmissions = new List<object>();
				foreach(object tSubmission in mSubmissions)
				{
					object tClone = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(tSubmission));
					tSubmissions.Add(tClone);
				}

				tSubmissions.Reverse();
				return tSubmissions;
			}
		}

		public void Handle(dynamic aFormData)
		{
			mSubmissions.Enqueue(aFormData);
			object tWasted;
			while (mSubmissions.Count > 10)
			{
				mSubmissions.TryDequeue(out tWasted);
			}
		}
	}
}
