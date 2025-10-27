using System;
using System.Web;
using System.Collections.Generic;

/// <summary>
/// Block the response to attacking IP addresses.
/// </summary>
public class DosAttackModule : IHttpModule
{
    private static int MAX_REQUESTS_PER_SECOND = 10;
    private static int LOCK_PERIOD_IN_SECONDS = 5 * 60;
    private static int MAX_VALID_FILE_PATH_SIZE = 1000;

    private class Limiter
    {
        private long Second;
        private int Count;
        private long UnlockTime;

        public Limiter()
        {
            Second = DateTime.UtcNow.Ticks / 10000000;
            Count = 0;
            UnlockTime = 0;
        }

        public bool newRequest(HttpRequest req)
        {
            long s = DateTime.UtcNow.Ticks / 10000000;

            if (s < UnlockTime) return false;
            else if (s != Second)
            {
                Second = s;
                Count = 0;
                return true;
            }
            else
            {
                ++Count;

                bool isValid = Count <= MAX_REQUESTS_PER_SECOND;
                if (!string.IsNullOrEmpty(req.FilePath) && req.FilePath.Length > MAX_VALID_FILE_PATH_SIZE)
                    isValid = false;

                if (isValid) return true;
                else
                {
                    UnlockTime = s + LOCK_PERIOD_IN_SECONDS;
                    return false;
                }
            }
        }
    }

    void IHttpModule.Dispose()
    {
        // Nothing to dispose; 
    }

    void IHttpModule.Init(HttpApplication context)
    {
        context.BeginRequest += new EventHandler(beginRequest);
    }

    private static Dictionary<string, Limiter> Applicants = new Dictionary<string, Limiter>();

    private void beginRequest(object sender, EventArgs e)
    {
        try
        {
            string ip = HttpContext.Current.Request.UserHostAddress;

            //Ignore files
            int ind = string.IsNullOrEmpty(HttpContext.Current.Request.FilePath) ||
                HttpContext.Current.Request.FilePath.Length > MAX_VALID_FILE_PATH_SIZE ? -1 :
                HttpContext.Current.Request.FilePath.LastIndexOf('.');
            string ext = ind < 0 ? null : HttpContext.Current.Request.FilePath.Substring(ind).ToLower();
            if (!string.IsNullOrEmpty(ext) && ext.Length > 1 &&
                ".js,.css,.jpg,.png,.gif,.aspx,.ashx,.eot,.ttf,.woff,.otf,.svg,.woff2".IndexOf(ext) >= 0) return;
            //end of Ignore files

            if (!Applicants.ContainsKey(ip)) Applicants[ip] = new Limiter();

            if (!Applicants[ip].newRequest(HttpContext.Current.Request))
            {
                HttpContext.Current.Response.StatusCode = 403;
                HttpContext.Current.Response.End();
            }
        }
        catch { }
    }
}