using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using RemiBou.Blazor.BrowserInterop.Credentials;

namespace RemiBou.Blazor.BrowserInterop
{
    public class NavigatorInterop
    {
        private IJSRuntime jsRuntime;

        public NavigatorInterop()
        {
            _lazyCredentialContainer = new Lazy<CredentialsContainer>(() => new CredentialsContainer(jsRuntime));
        }

        internal void SetJSRuntime(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }


        /// <summary>
        /// Returns the internal "code" name of the current browser. Do not rely on this property to return the correct value.
        /// </summary>
        /// <returns></returns>
        public string AppCodeName { get; set; }

        /// <summary>
        /// Returns  the official name of the browser. Do not rely on this property to return the correct value.
        /// </summary>
        /// <returns></returns>
        public string AppName { get; set; }

        /// <summary>
        /// Returns  the official name of the browser. Do not rely on this property to return the correct value.
        /// </summary>
        /// <returns></returns>
        public string AppVersion { get; set; }

        /// <summary>
        /// NON STANDARD Returns the build identifier of the browser. In modern browsers this property now returns a fixed timestamp as a privacy measure, e.g. 20181001000000 in Firefox 64 onwards.
        /// </summary>
        /// <value></value>
        public string BuildID { get; set; }



        /// <summary>
        ///  provides information about the system's battery
        /// </summary>
        /// <returns></returns>
        public async Task<BatteryManager> GetBattery()
        {
            return await jsRuntime.InvokeAsync<BatteryManager>("browserInterop.getBattery");

        }



        /// <summary>
        /// Return a JS Interop wrapper for getting information about the network connection of a device.
        /// </summary>
        /// <returns></returns>
        public NetworkInformationInterop Connection { get; set; }

        /// <summary>
        /// Returns false if setting a cookie will be ignored and true otherwise.
        /// </summary>
        /// <returns></returns>
        public bool CookieEnabled { get; set; }
        private Lazy<CredentialsContainer> _lazyCredentialContainer;

        /// <summary>
        /// Returns the CredentialsContainer interface which exposes methods to request credentials and notify the user agent when interesting events occur such as successful sign in or sign out. 
        /// </summary>
        public CredentialsContainer Credentials => _lazyCredentialContainer.Value;

        /// <summary>
        /// Returns the number of logical processor cores available.
        /// </summary>
        /// <returns></returns>
        public int HardwareConcurrency { get; set; }

        /// <summary>
        /// Returns false if the browser enables java
        /// </summary>
        /// <returns></returns>
        public async Task<bool> JavaEnabled()
        {
            return await this.jsRuntime.InvokeAsync<bool>("window.navigator.javaEnabled");
        }

        /// <summary>
        /// The user prefred language
        /// </summary>
        /// <returns></returns>
        public string Language { get; set; }

        /// <summary>
        /// Return all the user set languages
        /// </summary>
        /// <returns></returns>
        public string[] Languages { get; set; }

        /// <summary>
        /// Returns the maximum number of touch point supported by the current device
        /// </summary>
        /// <returns></returns>
        public int MaxTouchPoints { get; set; }

        /// <summary>
        /// Returns the mime types supported by the browser
        /// </summary>
        /// <returns></returns>
        public MimeTypeInterop[] MimeTypes { get; set; }

        /// <summary>
        /// Returns true if the user is online
        /// </summary>
        /// <returns></returns>
        public bool Online { get; set; }

        /// <summary>
        /// Returns a string representing the platform of the browser.
        /// </summary>
        /// <value></value>
        public string Platform { get; set; }


        /// <summary>
        /// Returns the plugins installed in this browser
        /// </summary>
        /// <returns></returns>

        public PluginInterop[] Plugins { get; set; }

        /// <summary>
        /// Return the user agent string for the browser
        /// </summary>
        /// <value></value>
        public string UserAgent { get; set; }

    }
    //from https://github.com/dotnet/corefx/issues/41442#issuecomment-553196880
    internal class HandleSpecialDoublesAsStrings : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string specialDouble = reader.GetString();
                if (specialDouble == "Infinity")
                {
                    return double.PositiveInfinity;
                }
                else if (specialDouble == "-Infinity")
                {
                    return double.NegativeInfinity;
                }
                else
                {
                    return double.NaN;
                }
            }
            return reader.GetDouble();
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            if (double.IsFinite(value))
            {
                writer.WriteNumberValue(value);
            }
            else
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }

}
