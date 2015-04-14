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
		private static ConcurrentDictionary<string, ConcurrentQueue<object>> mSubmissions;
		static InMemoryNerdyFormHandler()
		{
			mSubmissions = new ConcurrentDictionary<string, ConcurrentQueue<object>>();
		}

		public static List<object> GetSubmissions(string aFormName)
		{
			List<object> tSubmissions = new List<object>();
			ConcurrentQueue<object> tFormSumbissions;
			if(mSubmissions.TryGetValue(aFormName, out tFormSumbissions)) 
			{
				foreach (object tSubmission in tFormSumbissions)
				{
					object tClone = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(tSubmission));
					tSubmissions.Add(tClone);
				}

				tSubmissions.Reverse();
			}

			return tSubmissions;
		}

		public void Handle(dynamic aFormData)
		{
			ConcurrentQueue<object> tFormSumbissions;
			if (!mSubmissions.TryGetValue(aFormData.FormName, out tFormSumbissions))
			{
				tFormSumbissions = new ConcurrentQueue<object>();
				mSubmissions.TryAdd(aFormData.FormName, tFormSumbissions);
			}

			tFormSumbissions.Enqueue(aFormData);
			object tWasted;
			while (mSubmissions.Count > 10)
			{
				tFormSumbissions.TryDequeue(out tWasted);
			}
		}
	}
}
