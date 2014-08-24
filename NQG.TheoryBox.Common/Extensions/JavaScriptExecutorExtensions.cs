namespace NQG.TheoryBox.Extensions
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenQA.Selenium;
    using System;
    using System.Threading;

    public static class JavaScriptExecutorExtensions
    {
        /// <summary>
        /// JavaScript to jQuerify the current page by retrieving the latest jQuery library from the web.
        /// </summary>
        private const string JQuerifyWeb = @"(function() {
if (typeof jQuery!='undefined') {
    return jQuery.fn.jquery;
}
    
function getScript(url,success) {
    var script=document.createElement('script');
    script.src=url;
    var head=document.getElementsByTagName('head')[0];
    var done=false;
        
    script.onload=script.onreadystatechange = function(){
        if ( !done && (!this.readyState
            || this.readyState == 'loaded'
            || this.readyState == 'complete') ) {
        done=true;
        
        script.onload = script.onreadystatechange = null;
        head.removeChild(script);
        if (typeof(success) != 'undefined') {
            setTimeout(success(), 50);
          }
        }
    };
    head.appendChild(script);
}

getScript('http://code.jquery.com/jquery-latest.min.js', function() {
    jQuery.noConflict();
    __jQueryVersion = (typeof jQuery=='undefined') ? null : jQuery.fn.jquery;
  });

})();
";

        private const string InjectScript = @"(function() {
function getScript(url,success) {
    var script=document.createElement('script');
    script.src=url;
    var head=document.getElementsByTagName('head')[0];
    var done=false;
        
    script.onload=script.onreadystatechange = function(){
        if ( !done && (!this.readyState
            || this.readyState == 'loaded'
            || this.readyState == 'complete') ) {
        done=true;
        
        script.onload = script.onreadystatechange = null;
        head.removeChild(script);
        if (typeof(success) != 'undefined') {
            setTimeout(success(), 50);
          }
        }
    };
    head.appendChild(script);
}

getScript('{{ScriptUri}}', function() { } );

})();
";

        /// <summary>
        /// Injects jQuery onto the current page and returns the version of jQuery injected.
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static string JQuerify(this IJavaScriptExecutor driver)
        {
            driver.ExecuteScript("__jQueryVersion = '';");

            driver.ExecuteScript(JQuerifyWeb);

            for (var i = 0; i < 10; i++)
            {
                var jQueryVersion = driver.ExecuteScript("return __jQueryVersion");
                if (jQueryVersion is string && String.IsNullOrWhiteSpace(jQueryVersion as string) == false)
                {
                    return jQueryVersion as string;
                }
                Thread.Sleep(500);
            }

            return null;
        }

        public static bool Uriify(this IJavaScriptExecutor driver)
        {
            var isUriJsDefined = driver.ExecuteScript(@"
if (typeof URI !== 'undefined') {
    return true;
}
return false;");

            if (isUriJsDefined is bool && (bool)isUriJsDefined)
                return true;

            driver.ExecuteScript(InjectScript.Replace("{{ScriptUri}}", "http://cdnjs.cloudflare.com/ajax/libs/URI.js/1.7.2/URI.min.js"));

            for (var i = 0; i < 10; i++)
            {
                var uriType = driver.ExecuteScript("return typeof(URI);");
                if (uriType is string && (uriType as string) != "undefined")
                {
                    return true;
                }
                Thread.Sleep(500);
            }

            return false;
        }

        public static bool Sugarify(this IJavaScriptExecutor driver)
        {
            var isSugarJsDefined = driver.ExecuteScript(@"
if (typeof Date.create !== 'undefined') {
    return true;
}
return false;");

            if (isSugarJsDefined is bool && (bool)isSugarJsDefined)
                return true;

            driver.ExecuteScript(InjectScript.Replace("{{ScriptUri}}", "http://cdnjs.cloudflare.com/ajax/libs/sugar/1.3.9/sugar.min.js"));

            for (var i = 0; i < 10; i++)
            {
                var uriType = driver.ExecuteScript("return typeof(Date.create);");
                if (uriType is string && (uriType as string) != "undefined")
                {
                    return true;
                }
                Thread.Sleep(500);
            }

            return false;
        }

        /// <summary>
        /// Returns a value that indicates if jQuery is enabled on the current page.
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static bool IsJQueryEnabled(this IJavaScriptExecutor driver)
        {
            var result = driver.ExecuteScript("return typeof jQuery;") as string;

            return result != null && result == "function";
        }

        /// <summary>
        /// Executes the specified script (That returns a json.stringified string) and returns a dynamic result.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="script"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static dynamic ExecuteScriptWithResult(this IJavaScriptExecutor driver, string script, params object[] args)
        {
            var result = driver.ExecuteScript(script, args);

            if (result == null)
                return null;

            if (result is IWebElement ||
                result is decimal ||
                result is long ||
                result is bool ||
                result is string) //TODO: Flesh this out...
                return result;

            //Using JSON.Net convert the key/value collection returned by the web driver to a dynamic object.
            result = JsonConvert.SerializeObject(result);

            var strResult = result as string;

            if (String.IsNullOrWhiteSpace(strResult))
                return strResult;

            return JToken.Parse(strResult);
        }

        /// <summary>
        /// Executes the specified script (That returns a json.stringified string) and returns a strongly-typed result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="driver"></param>
        /// <param name="script"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T ExecuteScriptWithResult<T>(this IJavaScriptExecutor driver, string script, params object[] args)
        {
            var result = driver.ExecuteScript(script, args);

            if (result == null)
                return default(T);

            if (result.GetType() != typeof(string))
                result = JsonConvert.SerializeObject(result);

            var strResult = result as string;

            if (String.IsNullOrWhiteSpace(strResult))
                return default(T);

            if (typeof(T) == typeof(string))
                return (T)result;

            return JsonConvert.DeserializeObject<T>(strResult);
        }

        /// <summary>
        /// Executes the specified script (That returns a json.stringified string) and populates the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="driver"></param>
        /// <param name="obj"></param>
        /// <param name="script"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void ExecuteScriptPopulateObject<T>(this IJavaScriptExecutor driver, T obj, string script, params object[] args)
        {
            var result = driver.ExecuteScript(script, args);

            if (result == null)
                return;

            if (result.GetType() != typeof(string))
                result = JsonConvert.SerializeObject(result);

            var strResult = result as string;

            if (strResult == null)
                return;

            JsonConvert.PopulateObject(strResult, obj);
        }
    }
}
