using System.Collections.Specialized;
using System.Web;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework
{
    /// <summary>
    /// Represents a RemotePost helper class
    /// </summary>
    public partial class RemotePost
    {
        private readonly HttpContextBase _httpContext;
        private readonly IWebHelper _webHelper;
        private readonly NameValueCollection _inputValues;

        /// <summary>
        /// Gets or sets a remote URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets a form name
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// Gets or sets a form character-sets the server can handle for form-data.
        /// </summary>
        public string AcceptCharset { get; set; }

        /// <summary>
        /// A value indicating whether we should create a new "input" HTML element for each value (in case if there are more than one) for the same "name" attributes.
        /// </summary>
        public bool NewInputForEachValue { get; set; }

        public NameValueCollection Params => _inputValues;

        /// <summary>
        /// Creates a new instance of the RemotePost class
        /// </summary>
        public RemotePost()
            : this(EngineContext.Current.Resolve<HttpContextBase>(), EngineContext.Current.Resolve<IWebHelper>())
        {
        }

        /// <summary>
        /// Creates a new instance of the RemotePost class
        /// </summary>
        /// <param name="httpContext">HTTP Context</param>
        /// <param name="webHelper">Web helper</param>
        public RemotePost(HttpContextBase httpContext, IWebHelper webHelper)
        {
            this._inputValues = new NameValueCollection();
            this.Url = "http://www.someurl.com";
            this.Method = "post";
            this.FormName = "formName";

            this._httpContext = httpContext;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary (to be posted).
        /// </summary>
        /// <param name="name">The key of the element to add</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(string name, string value)
        {
            _inputValues.Add(name, value);
        }
        
        /// <summary>
        /// Post
        /// </summary>
        public void Post()
        {
            _httpContext.Response.Clear();
            _httpContext.Response.Write("<html><head>");
            _httpContext.Response.Write($"</head><body onload=\"document.{FormName}.submit()\">");
            _httpContext.Response.Write(!string.IsNullOrEmpty(AcceptCharset)
                ? $"<form name=\"{FormName}\" method=\"{Method}\" action=\"{Url}\" accept-charset=\"{AcceptCharset}\">"
                : $"<form name=\"{FormName}\" method=\"{Method}\" action=\"{Url}\" >");
            if (NewInputForEachValue)
            {
                foreach (string key in _inputValues.Keys)
                {
                    var values = _inputValues.GetValues(key);
                    if (values == null) continue;
                    foreach (var value in values)
                    {
                        _httpContext.Response.Write(
                            $"<input name=\"{HttpUtility.HtmlEncode(key)}\" type=\"hidden\" value=\"{HttpUtility.HtmlEncode(value)}\">");
                    }
                }
            }
            else
            {
                for (var i = 0; i < _inputValues.Keys.Count; i++)
                    _httpContext.Response.Write(
                        $"<input name=\"{HttpUtility.HtmlEncode(_inputValues.Keys[i])}\" type=\"hidden\" value=\"{HttpUtility.HtmlEncode(_inputValues[_inputValues.Keys[i]])}\">");
            }
            _httpContext.Response.Write("</form>");
            _httpContext.Response.Write("</body></html>");
            _httpContext.Response.End();
            //store a value indicating whether POST has been done
            _webHelper.IsPostBeingDone = true;
        }
    }
}