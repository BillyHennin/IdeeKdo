using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Android.OS;
using Android.Util;

namespace IdeeKdo.Assets.Tools
{
    /// <summary>
    ///     Classe static qui permet de normaliser le systeme de log
    /// </summary>
    public static class XLog
    {
        public static List<XadiaLog> XadiaLogs;

        public static void Write(LogPriority logPrio, string strMessage)
        {
            var xlog = new XadiaLog(logPrio, strMessage);
            if (XadiaLogs.Equals(null))
                XadiaLogs = new List<XadiaLog>();
            XadiaLogs.Add(xlog);
            Log.WriteLine(logPrio, xlog.CallerMemberName, strMessage);
        }

        public static string GetLogs(List<XadiaLog> listXadiaLog = null)
            => (listXadiaLog ?? XadiaLogs).Aggregate(GetPhoneInfos(), (current, log) => current + "\n" + log.GetItem());

        public static List<XadiaLog> GetXadiaLogsByPrio(LogPriority priority, bool pAndAbove = false)
            => GetXadiaLogs(log => pAndAbove ? log.Priority <= priority : log.Priority == priority);

        public static List<XadiaLog> GetXadiaLogs(Predicate<XadiaLog> conditionPredicate)
            => XadiaLogs.FindAll(conditionPredicate);

        private static string GetPhoneInfos()
            =>
                $"Android version : \"{Build.VERSION.Release}\".\n Téléphone : {Build.Brand.ToUpper()} {Build.Manufacturer} {Build.Model}\n\r";
    }

    public class XadiaLog
    {
        private readonly string _callerFilePath;
        private readonly int _callerLineNumber;
        private readonly DateTime _date;
        private readonly string _message;
        public readonly string CallerMemberName;
        public readonly LogPriority Priority;

        public XadiaLog(LogPriority logPrio, string strMessage,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            _date = DateTime.Now;
            Priority = logPrio;
            _message = strMessage;
            CallerMemberName = callerMemberName;
            _callerFilePath = callerFilePath;
            _callerLineNumber = callerLineNumber;
        }

        public string GetItem()
            =>
                $"{_date} - {Enum.GetName(typeof(LogPriority), Priority)} : From \"{CallerMemberName}\" - \"{_callerFilePath}\" - line {_callerLineNumber} \n Message : {_message} \n\r";
    }
}