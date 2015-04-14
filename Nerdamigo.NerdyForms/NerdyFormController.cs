using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.Web.WebPages;

namespace Nerdamigo.NerdyForms
{
	public class NerdyFormController : Controller
	{
		private INerdyFormHandler mFormHandler;
		public NerdyFormController(INerdyFormHandler aFormHandler)
		{
			if (aFormHandler == null)
			{
				throw new ArgumentException("aFormHandlerList");
			}

			mFormHandler = aFormHandler;
		}

		public virtual ActionResult Handle(string FormName)
		{
			if (String.IsNullOrWhiteSpace(FormName))
			{
				throw new ArgumentException("Form Name is Required");
			}


			dynamic tData = ProcessFormData();

			List<Exception> tEncounteredExceptions = new List<Exception>();

			tData.FormName = FormName;
			tData._Metadata = new NerdyFormDynamic();
			tData._Metadata.SubmissionID = Guid.NewGuid();
			tData._Metadata.SubmissionDttmUtc = DateTime.UtcNow;
			tData._Metadata.Request = new NerdyFormDynamic();
			tData._Metadata.Request.RawUrl = Request.RawUrl;
			tData._Metadata.Request.Headers = new Dictionary<string, string>();

			foreach (var iHeaderKey in Request.Headers.AllKeys)
			{
				tData._Metadata.Request.Headers.Add(iHeaderKey, Request.Headers[iHeaderKey]);
			}

			mFormHandler.Handle(tData);

			if (Request.IsAjaxRequest())
			{
				return new EmptyResult();
			}
			else
			{
				string tReferringUrl = Request.UrlReferrer != null ? Request.UrlReferrer.PathAndQuery : null;
				string tRedirectToUrl = Request.Form["RedirectTo"];

				//post-redirect-get and watch out for open redirects
				if (!String.IsNullOrEmpty(tRedirectToUrl) && RequestExtensions.IsUrlLocalToHost(Request, tRedirectToUrl))
				{
					return new RedirectResult(tRedirectToUrl, false);
				}
				else if (!String.IsNullOrEmpty(tReferringUrl) && RequestExtensions.IsUrlLocalToHost(Request, tReferringUrl))
				{
					return new RedirectResult(tReferringUrl, false);
				}
				else
				{
					return new RedirectResult("/");
				}
			}
		}

		protected virtual dynamic ProcessFormData()
		{
			dynamic tData = new NerdyFormDynamic();

			foreach (string iFormKey in Request.Form.AllKeys.OrderBy(s => s))
			{
				try
				{
					ProcessKey(iFormKey, tData, Request.Form[iFormKey]);
				}
				catch (NerdyFormKeyDecodeException)
				{
				}
			}
			return tData;
		}

		
		private void ProcessKey(string aFormKey, dynamic aData, string aFormValue)
		{
			string[] tDotSplits = aFormKey.Split('.');
			if (tDotSplits[0].IndexOf('[') > -1)
			{
				ProcessList(aFormKey, aData, aFormValue);
				return;
			}

			if (tDotSplits[0] == aFormKey)
			{
				aData[aFormKey] = aFormValue;
			}
			else
			{
				string tSubkey = String.Join(".", tDotSplits.Skip(1));
				dynamic tSubData = aData[tDotSplits[0]];

				if (tSubData == null)
				{
					tSubData = new NerdyFormDynamic();
					aData[tDotSplits[0]] = tSubData;
				}


				ProcessKey(tSubkey, tSubData, aFormValue);
			}
		}

		private void ProcessList(string aFormKey, dynamic aData, string aFormValue)
		{
			int tStartIdx = aFormKey.IndexOf('[');
			int tEndIdx = aFormKey.IndexOf(']');

			if (tStartIdx == -1 || tEndIdx == -1 || tEndIdx - tStartIdx < 1)
			{
				throw new NerdyFormKeyDecodeException(String.Format("Invalid Form Key {0}", aFormKey));
			}

			string tIndexString = aFormKey.Substring(tStartIdx + 1, tEndIdx - tStartIdx - 1);
			int tIndex;
			if (!int.TryParse(tIndexString, out tIndex))
			{
				throw new NerdyFormKeyDecodeException(String.Format("Invalid Form Key List Index {0}", aFormKey));
			}

			string tPropertyName = null;
			tPropertyName = tStartIdx > 0 ? aFormKey.Substring(0, tStartIdx) : "";
			string tConstructedPropertyWithIndex = String.Format("{0}[{1}]", tPropertyName, tIndex);
			if(tConstructedPropertyWithIndex == aFormKey && tConstructedPropertyWithIndex[0] == '[')
			{
				//dealing with a multi-dimensional list of strings
			}
			else if (tConstructedPropertyWithIndex == aFormKey)
			{
				//we are dealing with a list of strings directly
				List<string> tItems;
				if (aData[tPropertyName] == null || aData[tPropertyName].GetType() != typeof(List<string>))
				{
					tItems = new List<string>();
					aData[tPropertyName] = tItems;
				}
				else
				{
					tItems = aData[tPropertyName];
				}

				while (tItems.Count < tIndex) tItems.Add(null);
				tItems.Add(aFormValue);
			}
			else if(aFormKey.IndexOf(String.Format("{0}.", tConstructedPropertyWithIndex)) == 0)
			{
				//dealing with a List<dynamic>
				List<NerdyFormDynamic> tItems;
				if (aData[tPropertyName] == null || aData[tPropertyName].GetType() != typeof(List<NerdyFormDynamic>))
				{
					tItems = new List<NerdyFormDynamic>();
					aData[tPropertyName] = tItems;
				}
				else
				{
					tItems = aData[tPropertyName];
				}

				while (tItems.Count < tIndex) tItems.Add(null);

				if (tItems.Count <= tIndex || tItems[tIndex] == null)
				{
					tItems.Add(new NerdyFormDynamic());
				}
				dynamic tSubData = tItems[tIndex];
				ProcessKey(aFormKey.Substring(tConstructedPropertyWithIndex.Length + 1), tSubData, aFormValue);
			}
			else if (aFormKey.IndexOf(String.Format("{0}[", tConstructedPropertyWithIndex)) == 0)
			{
				//dealing with a List<List<something>>
				List<NerdyFormDynamic> tItems;
				if (aData[tPropertyName] == null || aData[tPropertyName].GetType() != typeof(List<NerdyFormDynamic>))
				{
					tItems = new List<NerdyFormDynamic>();
					aData[tPropertyName] = tItems;
				}
				else
				{
					tItems = aData[tPropertyName];
				}

				while (tItems.Count < tIndex) tItems.Add(null);

				if (tItems.Count <= tIndex || tItems[tIndex] == null)
				{
					tItems.Add(new NerdyFormDynamic());
				}
				dynamic tSubData = tItems[tIndex];
				ProcessList(aFormKey.Substring(tConstructedPropertyWithIndex.Length), tSubData, aFormValue);
			}
		}

		private static Regex tInvalidKeyChars = new Regex(@"[^a-z0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static Regex tValidKey = new Regex(@"^[a-z][a-z0-9]*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private string CreateValidKey(string iToken)
		{
			string tSanitized = tInvalidKeyChars.Replace(iToken, "");
			if (!tValidKey.IsMatch(tSanitized))
			{
				throw new NerdyFormKeyDecodeException(String.Format("Token could not be sanitized {0}", iToken));
			}

			return tSanitized;
		}
	}
}
