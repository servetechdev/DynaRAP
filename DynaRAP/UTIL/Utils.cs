﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DynaRAP.Data;
using log4net.Config;
using Newtonsoft.Json;

namespace DynaRAP.UTIL
{
    public static class Utils
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static DateTime GetDateFromJulian2(string julianDate)
        {
            string[] splits = julianDate.Split(':');
            //var arg = string.Format("{0} {1}:{2}:{3}", new DateTime().AddYears(DateTime.Now.Year - 1).AddDays(double.Parse(splits[0])).ToString("yyyy-MM-dd"), splits[1], splits[2], splits[3]);
            var arg = string.Format("{0} {1}:{2}:{3}", new DateTime(DateTime.Now.Year - 1, 1, 1).AddDays(double.Parse(splits[0])).ToString("yyyy-MM-dd"), splits[1], splits[2], splits[3]);

            DateTime dt = DateTime.ParseExact(arg, "yyyy-MM-dd HH:mm:ss.ffffff", null);

            return dt;

        }

        public static DateTime GetDateFromJulian(string julianDate)
        {
            DateTime dt = DateTime.Now;
            if (julianDate.Contains(":")) // 비행데이터
            {
                // 예시 352:12:36:20.941466
                int year = 21;
                int day = Convert.ToInt32(julianDate.Substring(0, 3));
                dt = new DateTime(1999 + year, 12, 18, new JulianCalendar());

                dt = dt.AddDays(day);
                dt = dt.AddHours(Convert.ToInt32(julianDate.Substring(4, 2)));
                dt = dt.AddMinutes(Convert.ToInt32(julianDate.Substring(7, 2)));
                dt = dt.AddSeconds(Convert.ToInt32(julianDate.Substring(10, 2)));
                dt = dt.AddMilliseconds(Convert.ToInt32(julianDate.Substring(13, 3)));
                dt = dt.AddTicks(Convert.ToInt32(julianDate.Substring(13, 6)) % TimeSpan.TicksPerMillisecond / 100);
                //dt = dt.AddTicks(-Convert.ToInt32(julianDate.Substring(13, 6)) % TimeSpan.TicksPerMillisecond / 100);
                ////myDateTime.AddTicks(-(myDateTime.Ticks % TimeSpan.TicksPerMillisecond) / 100);

                dt = DateTime.Parse(string.Format("{0}-{1}-{2} {3}:{4}:{5}.{6}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, julianDate.Substring(13, 6)));

            }
            else // 해석데이터
            {
                // 예시 1.98200
                dt = new DateTime(DateTime.Now.Year, 1, 1);
                dt = dt.AddSeconds(Convert.ToInt32(julianDate.Substring(0, 1)));
                dt = dt.AddMilliseconds(Convert.ToInt32(julianDate.Substring(2, 3)));
                //dt = dt.AddTicks(Convert.ToInt32(julianDate.Substring(2, 5)) % TimeSpan.TicksPerMillisecond / 100);
            }
            return dt;

        }

        public static string GetJulianFromDate(object obj)
        {
            //DateTime dt = (DateTime)obj;
            DateTime dt = Convert.ToDateTime(obj.ToString());
            string strDate = String.Format("{0}:{1:00}:{2:00}:{3:00}.{4:000000}", dt.DayOfYear, dt.Hour, dt.Minute, dt.Second, dt.TimeOfDay.Milliseconds * 1000);

            return strDate;
        }


        public static string GetJulianFromDate(DateTime dateTime)
        {
            string strDate = string.Empty;
            if (dateTime.Year == DateTime.Now.Year && dateTime.Month == 1 && dateTime.Day == 1)
            {
                double millise = Math.Round(dateTime.TimeOfDay.TotalSeconds - Math.Truncate(dateTime.TimeOfDay.TotalSeconds), 7) * 1000000;
                strDate = String.Format("{0}.{1:00000}", dateTime.DayOfYear, dateTime.Hour, dateTime.Minute, dateTime.Second, millise);
            } else
            {
                double millise = Math.Round(dateTime.TimeOfDay.TotalSeconds - Math.Truncate(dateTime.TimeOfDay.TotalSeconds), 7) * 1000000;
                strDate = String.Format("{0}:{1:00}:{2:00}:{3:00}.{4:000000}", dateTime.DayOfYear, dateTime.Hour, dateTime.Minute, dateTime.Second, millise);
            }

            return strDate;
        }

        public static string GetZaeroJulianFromDate(object obj)
        {
            DateTime dt = (DateTime)obj;
            //DateTime dt = Convert.ToDateTime(obj.ToString());
            string strDate = String.Format("{0}.{1:000}00",dt.Second, dt.Millisecond);

            return strDate;
        }

        public static DateTime ConvertFromJulian(int m_JulianDate)
        {

            long L = m_JulianDate + 68569;
            long N = (long)((4 * L) / 146097);
            L = L - ((long)((146097 * N + 3) / 4));
            long I = (long)((4000 * (L + 1) / 1461001));
            L = L - (long)((1461 * I) / 4) + 31;
            long J = (long)((80 * L) / 2447);
            int Day = (int)(L - (long)((2447 * J) / 80));
            L = (long)(J / 11);
            int Month = (int)(J + 2 - 12 * L);
            int Year = 2021;// (int)(100 * (N - 49) + I + L);

            DateTime dt = new DateTime(Year, Month, Day);
            return dt;
        }

        public static DateTime JulianToDateTime(double julianDate)
        {
            double unixTime = (julianDate - 2440587.5) * 86400;

            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();

            return dtDateTime;
        }

        public static string GetPostData(string url, string sendData)
        {
            try
            {
                log.Info("url : " + url);
                log.Info(sendData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 30 * 10000;
                //request.Headers.Add("Authorization", "BASIC SGVsbG8=");

                // POST할 데이타를 Request Stream에 쓴다
                byte[] bytes = Encoding.ASCII.GetBytes(sendData);
                request.ContentLength = bytes.Length; // 바이트수 지정

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

                return responseText;
            }
            catch (WebException e)
            {
                if(e.Response == null)
                {
                    log.Error(e.Message);
                    MessageBox.Show(e.Message);
                    return null;
                }
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    if (httpResponse == null)
                    {
                        log.Error(e.Message);
                        MessageBox.Show(e.Message);
                        return null;
                    }
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        JsonData result = JsonConvert.DeserializeObject<JsonData>(text);
                        if (result != null)
                        {
                            log.Error(e.Message);
                            log.Error("Code : " +result.code +" Message : "+ result.message);
                            MessageBox.Show(new Form { TopMost = true }, e.Message);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(new Form { TopMost = true }, ex.Message);
                return null;
            }

        }
        public static string GetPostDataNew(string url, string sendData)
        {
            try
            {
                log.Info("url : " + url);
                log.Info(sendData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 30 * 10000;
                //request.Headers.Add("Authorization", "BASIC SGVsbG8=");

                // POST할 데이타를 Request Stream에 쓴다
                byte[] bytes = Encoding.ASCII.GetBytes(sendData);
                request.ContentLength = bytes.Length; // 바이트수 지정

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

                return responseText;
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        JsonData result = JsonConvert.DeserializeObject<JsonData>(text);
                        if (result != null)
                        {
                            log.Error(e.Message);
                            log.Error("Code : " + result.code + " Message : " + result.message);
                            MessageBox.Show(string.Format("에러발생\r\nCode : {0} \r\nMessage : {1}", result.code, result.message));
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }

        }

        public static string base64StringDecoding(string byteData)
        {
            byte[] byte64 = Convert.FromBase64String(byteData);
            string deodingData = Encoding.UTF8.GetString(byte64);
            return deodingData;
        }

        public static T DeepClone<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Object cannot be null.");

            return (T)Process(obj, new Dictionary<object, object>() { });
        }


        private static object Process(object obj, Dictionary<object, object> circular)
        {
            if (obj == null)
                return null;


            Type type = obj.GetType();


            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }


            if (type.IsArray)
            {
                if (circular.ContainsKey(obj))
                    return circular[obj];


                string typeNoArray = type.FullName.Replace("[]", string.Empty);
                Type elementType = Type.GetType(typeNoArray + ", " + type.Assembly.FullName);
                var array = obj as Array;
                Array arrCopied = Array.CreateInstance(elementType, array.Length);
                circular[obj] = arrCopied;


                for (int i = 0; i < array.Length; i++)
                {
                    object element = array.GetValue(i);
                    object objCopy = null;


                    if (element != null && circular.ContainsKey(element))
                        objCopy = circular[element];
                    else
                        objCopy = Process(element, circular);


                    arrCopied.SetValue(objCopy, i);
                }


                return Convert.ChangeType(arrCopied, obj.GetType());
            }


            if (type.IsClass)
            {
                if (circular.ContainsKey(obj))
                    return circular[obj];


                object objValue = Activator.CreateInstance(obj.GetType());
                circular[obj] = objValue;
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);


                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                        continue;


                    object objCopy = circular.ContainsKey(fieldValue) ? circular[fieldValue] : Process(fieldValue, circular);
                    field.SetValue(objValue, objCopy);
                }
                return objValue;
            }
            else
                throw new ArgumentException("Unknown type");
        }
    }
}
