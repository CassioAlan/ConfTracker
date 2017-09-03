using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConfSpider.TOLibrary
{
    public static class DateUtilTO
    {
        public static string DayPattern { get { return @"([1-2]\d|3[0-1]|0?[1-9])(st|nd|rd|th)*"; } }
        public static string SpacePattern { get { return @"(, |\.|-| de |\/|\s+)"; } }
        public static string YearPattern { get { return @"(\d{4}|\d{2})"; } }
        public static string MonthPattern { get { return @"(1[0-2]|[1-9]|0[1-9]|january|february|march|april|may|june|july|august|september|october|november|december|jan\.?|feb\.?|mar\.?|apr\.?|may\.?|jun\.?|jul\.?|aug\.?|sep\.?|oct\.?|nov\.?|dec\.?|janeiro|fevereiro|março|abril|maio|junho|julho|agosto|setembro|outubro|novembro|dezembro|jan\.?|fev\.?|mar\.?|abr\.?|mai\.?|jun\.?|jul\.?|ago\.?|set\.?|out\.?|nov\.?|dez\.?)"; } }
        public static string IntervalPattern { get { return @"( - | -|- |-| – | –|– |–| to | and | a )"; } }

        public static string PatternDMY { get { return DayPattern + SpacePattern + MonthPattern + SpacePattern + YearPattern; } }
        public static string PatternMDY { get { return MonthPattern + SpacePattern + DayPattern + SpacePattern + YearPattern; } }
        public static string PatternYDM { get { return YearPattern + SpacePattern + DayPattern + SpacePattern + MonthPattern; } }
        public static string PatternYMD { get { return YearPattern + SpacePattern + MonthPattern + SpacePattern + DayPattern; } }

        public static string PatternDM { get { return DayPattern + SpacePattern + MonthPattern; } }
        public static string PatternMD { get { return MonthPattern + SpacePattern + DayPattern; } }

        public static string SingleDatePattern { get { return "(" + PatternDMY + ")|" +
                                                                "(" + PatternMDY + ")|" +
                                                                "(" + PatternYDM + ")|" +
                                                                "(" + PatternYMD + ")|" +
                                                                "(" + PatternDM + ")|" +
                                                                "(" + PatternMD + ")"; } }

        public static string PatternIMY { get { return DayPattern + IntervalPattern + DayPattern + SpacePattern + MonthPattern + SpacePattern + YearPattern; } }
        public static string PatternMIY { get { return MonthPattern + SpacePattern + DayPattern + IntervalPattern + DayPattern + SpacePattern + YearPattern; } }
        public static string PatternYIM { get { return YearPattern + SpacePattern + DayPattern + IntervalPattern + DayPattern + SpacePattern + MonthPattern; } }
        public static string PatternYMI { get { return YearPattern + SpacePattern + MonthPattern + SpacePattern + DayPattern + IntervalPattern + DayPattern; } }

        public static string PatternDMDMY { get { return DayPattern + SpacePattern + MonthPattern + IntervalPattern + DayPattern + SpacePattern + MonthPattern + SpacePattern + YearPattern; } }
        public static string PatternMDMDY { get { return MonthPattern + SpacePattern + DayPattern + IntervalPattern + MonthPattern + SpacePattern + DayPattern + SpacePattern + YearPattern; } }
        public static string PatternYDMDM { get { return YearPattern + SpacePattern + DayPattern + SpacePattern + MonthPattern + IntervalPattern + DayPattern + SpacePattern + MonthPattern; } }
        public static string PatternYMDMD { get { return YearPattern + SpacePattern + MonthPattern + SpacePattern + DayPattern + IntervalPattern + MonthPattern + SpacePattern + DayPattern; } }

        public static string PatternIM { get { return DayPattern + IntervalPattern + DayPattern + SpacePattern + MonthPattern; } }
        public static string PatternMI { get { return MonthPattern + SpacePattern + DayPattern + IntervalPattern + DayPattern; } }

        public static string IntervalDatesPattern { get { return    "(" + PatternIMY + ")|" +
                                                                    "(" + PatternMIY + ")|" +
                                                                    "(" + PatternYIM + ")|" + 
                                                                    "(" + PatternYMI + ")|" +
                                                                    "(" + PatternDMDMY + ")|" +
                                                                    "(" + PatternMDMDY + ")|" +
                                                                    "(" + PatternYDMDM + ")|" +
                                                                    "(" + PatternYMDMD + ")|" +
                                                                    "(" + PatternIM + ")|" +
                                                                    "(" + PatternMI + ")"; } }

        public static string AllDatesPattern { get { return "(" + IntervalDatesPattern + ")|(" + SingleDatePattern + ")"; } }
        
        public static List<DateTime> FindDatesOnText(string text)
        {
            var _response = new List<DateTime>();
            Regex _regex;
            MatchCollection _matches;

            #region DMY
            _regex = new Regex(PatternDMY, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _spaces = SpacePattern.Replace("(", "").Replace(")", "").Replace("\\", "").Split('|');
                //var _pieces = _m.Value.Split(_spaces, StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, SpacePattern.Replace("(", "").Replace(")", ""), RegexOptions.IgnoreCase);

                if (_pieces.Length == 3)
                {
                    int _day = stringDayToInt(_pieces[0]);
                    int _month = stringMonthToInt(_pieces[1]);
                    int _year = stringYearToInt(_pieces[2]);

                    try
                    {
                        _response.Add(new DateTime(_year, _month, _day));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region MDY
            _regex = new Regex(PatternMDY, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _spaces = SpacePattern.Replace("(", "").Replace(")", "").Replace("\\", "").Split('|');
                //var _pieces = _m.Value.Split(_spaces, StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, SpacePattern.Replace("(", "").Replace(")", ""), RegexOptions.IgnoreCase);

                if (_pieces.Length == 3)
                {
                    int _month = stringMonthToInt(_pieces[0]);
                    int _day = stringDayToInt(_pieces[1]);
                    int _year = stringYearToInt(_pieces[2]);

                    try
                    {
                        _response.Add(new DateTime(_year, _month, _day));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region YDM
            _regex = new Regex(PatternYDM, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _spaces = SpacePattern.Replace("(", "").Replace(")", "").Replace("\\", "").Split('|');
                //var _pieces = _m.Value.Split(_spaces, StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, SpacePattern.Replace("(", "").Replace(")", ""), RegexOptions.IgnoreCase);

                if (_pieces.Length == 3)
                {
                    int _year = stringYearToInt(_pieces[0]);
                    int _day = stringDayToInt(_pieces[1]);
                    int _month = stringMonthToInt(_pieces[2]);

                    try
                    {
                        _response.Add(new DateTime(_year, _month, _day));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region YMD
            _regex = new Regex(PatternYMD, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _spaces = SpacePattern.Replace("(", "").Replace(")", "").Replace("\\", "").Split('|');
                //var _pieces = _m.Value.Split(_spaces, StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, SpacePattern.Replace("(", "").Replace(")", ""), RegexOptions.IgnoreCase);

                if (_pieces.Length == 3)
                {
                    int _year = stringYearToInt(_pieces[0]);
                    int _month = stringMonthToInt(_pieces[1]);
                    int _day = stringDayToInt(_pieces[2]);

                    try
                    {
                        _response.Add(new DateTime(_year, _month, _day));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region DM
            _regex = new Regex(PatternDM, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _spaces = SpacePattern.Replace("(", "").Replace(")", "").Replace("\\", "").Split('|');
                //var _pieces = _m.Value.Split(_spaces, StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, SpacePattern.Replace("(", "").Replace(")", ""), RegexOptions.IgnoreCase);

                if (_pieces.Length == 2)
                {
                    int _day = stringDayToInt(_pieces[0]);
                    int _month = stringMonthToInt(_pieces[1]);
                    int _year = DateTime.Now.Year;

                    try
                    {
                        _response.Add(new DateTime(_year, _month, _day));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region MD
            _regex = new Regex(PatternMD, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _spaces = SpacePattern.Replace("(", "").Replace(")", "").Replace("\\", "").Split('|');
                //var _pieces = _m.Value.Split(_spaces, StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, SpacePattern.Replace("(", "").Replace(")", ""), RegexOptions.IgnoreCase);

                if (_pieces.Length == 2)
                {
                    int _month = stringMonthToInt(_pieces[0]);
                    int _day = stringDayToInt(_pieces[1]);
                    int _year = DateTime.Now.Year;

                    try
                    {
                        _response.Add(new DateTime(_year, _month, _day));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            return _response;
        }

        public static List<Tuple<DateTime, DateTime>> FindIntervalDatesOnText(string text)
        {
            var _response = new List<Tuple<DateTime, DateTime>>();
            Regex _regex;
            MatchCollection _matches;

            //List<string> _separetors = IntervalPattern.Replace("(", "").Replace(")", "").Replace("\\", "").Split('|').ToList();
            //_separetors.AddRange(SpacePattern.Replace("(", "").Replace(")", "").Replace("\\", "").Split('|').ToList());
            var _separator = IntervalPattern.Replace("(", "").Replace(")", "") + "|" + SpacePattern.Replace("(", "").Replace(")", "");

            #region DMDMY
            _regex = new Regex(PatternDMDMY, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _pieces = _m.Value.Split(_separetors.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, _separator, RegexOptions.IgnoreCase);

                if (_pieces.Length == 5)
                {
                    int _day1 = stringDayToInt(_pieces[0]);
                    int _month1 = stringMonthToInt(_pieces[1]);
                    int _day2 = stringDayToInt(_pieces[2]);
                    int _month2 = stringMonthToInt(_pieces[3]);
                    int _year = stringYearToInt(_pieces[4]);

                    try
                    {
                        _response.Add(new Tuple<DateTime, DateTime>(new DateTime(_year, _month1, _day1), new DateTime(_year, _month2, _day2)));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region MDMDY
            _regex = new Regex(PatternMDMDY, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _pieces = _m.Value.Split(_separetors.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, _separator, RegexOptions.IgnoreCase);

                if (_pieces.Length == 5)
                {
                    int _month1 = stringMonthToInt(_pieces[0]);
                    int _day1 = stringDayToInt(_pieces[1]);
                    int _month2 = stringMonthToInt(_pieces[2]);
                    int _day2 = stringDayToInt(_pieces[3]);
                    int _year = stringYearToInt(_pieces[4]);

                    try
                    {
                        _response.Add(new Tuple<DateTime, DateTime>(new DateTime(_year, _month1, _day1), new DateTime(_year, _month2, _day2)));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region YDMDM
            _regex = new Regex(PatternYDMDM, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _pieces = _m.Value.Split(_separetors.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, _separator, RegexOptions.IgnoreCase);

                if (_pieces.Length == 5)
                {
                    int _year = stringYearToInt(_pieces[0]);
                    int _day1 = stringDayToInt(_pieces[1]);
                    int _month1 = stringMonthToInt(_pieces[2]);
                    int _day2 = stringDayToInt(_pieces[3]);
                    int _month2 = stringMonthToInt(_pieces[4]);

                    try
                    {
                        _response.Add(new Tuple<DateTime, DateTime>(new DateTime(_year, _month1, _day1), new DateTime(_year, _month2, _day2)));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region YMDMD
            _regex = new Regex(PatternYMDMD, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _pieces = _m.Value.Split(_separetors.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, _separator, RegexOptions.IgnoreCase);

                if (_pieces.Length == 5)
                {
                    int _year = stringYearToInt(_pieces[0]);
                    int _month1 = stringMonthToInt(_pieces[1]);
                    int _day1 = stringDayToInt(_pieces[2]);
                    int _month2 = stringMonthToInt(_pieces[3]);
                    int _day2 = stringDayToInt(_pieces[4]);

                    try
                    {
                        _response.Add(new Tuple<DateTime, DateTime>(new DateTime(_year, _month1, _day1), new DateTime(_year, _month2, _day2)));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region IMY
            _regex = new Regex(PatternIMY, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _pieces = _m.Value.Split(_separetors.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, _separator, RegexOptions.IgnoreCase);

                if (_pieces.Length == 4)
                {
                    int _day1 = stringDayToInt(_pieces[0]);
                    int _day2 = stringDayToInt(_pieces[1]);
                    int _month = stringMonthToInt(_pieces[2]);
                    int _year = stringYearToInt(_pieces[3]);

                    try
                    {
                        _response.Add(new Tuple<DateTime, DateTime>(new DateTime(_year, _month, _day1), new DateTime(_year, _month, _day2)));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region MIY
            _regex = new Regex(PatternMIY, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _pieces = _m.Value.Split(_separetors.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, _separator, RegexOptions.IgnoreCase);

                if (_pieces.Length == 4)
                {
                    int _month = stringMonthToInt(_pieces[0]);
                    int _day1 = stringDayToInt(_pieces[1]);
                    int _day2 = stringDayToInt(_pieces[2]);
                    int _year = stringYearToInt(_pieces[3]);

                    try
                    {
                        _response.Add(new Tuple<DateTime, DateTime>(new DateTime(_year, _month, _day1), new DateTime(_year, _month, _day2)));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region YIM
            _regex = new Regex(PatternYIM, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _pieces = _m.Value.Split(_separetors.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, _separator, RegexOptions.IgnoreCase);

                if (_pieces.Length == 4)
                {
                    int _year = stringYearToInt(_pieces[0]);
                    int _day1 = stringDayToInt(_pieces[1]);
                    int _day2 = stringDayToInt(_pieces[2]);
                    int _month = stringMonthToInt(_pieces[3]);

                    try
                    {
                        _response.Add(new Tuple<DateTime, DateTime>(new DateTime(_year, _month, _day1), new DateTime(_year, _month, _day2)));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region YMI
            _regex = new Regex(PatternYMI, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _pieces = _m.Value.Split(_separetors.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, _separator, RegexOptions.IgnoreCase);

                if (_pieces.Length == 4)
                {
                    int _year = stringYearToInt(_pieces[0]);
                    int _month = stringMonthToInt(_pieces[1]);
                    int _day1 = stringDayToInt(_pieces[2]);
                    int _day2 = stringDayToInt(_pieces[3]);

                    try
                    {
                        _response.Add(new Tuple<DateTime, DateTime>(new DateTime(_year, _month, _day1), new DateTime(_year, _month, _day2)));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region IM
            _regex = new Regex(PatternIM, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _pieces = _m.Value.Split(_separetors.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, _separator, RegexOptions.IgnoreCase);

                if (_pieces.Length == 3)
                {
                    int _day1 = stringDayToInt(_pieces[0]);
                    int _day2 = stringDayToInt(_pieces[1]);
                    int _month = stringMonthToInt(_pieces[2]);
                    int _year = DateTime.Now.Year;

                    try
                    {
                        _response.Add(new Tuple<DateTime, DateTime>(new DateTime(_year, _month, _day1), new DateTime(_year, _month, _day2)));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            #region MI
            _regex = new Regex(PatternMI, RegexOptions.IgnoreCase);
            _matches = _regex.Matches(text);

            foreach (Match _m in _matches)
            {
                //var _pieces = _m.Value.Split(_separetors.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                var _pieces = Regex.Split(_m.Value, _separator, RegexOptions.IgnoreCase);

                if (_pieces.Length == 3)
                {
                    int _month = stringMonthToInt(_pieces[0]);
                    int _day1 = stringDayToInt(_pieces[1]);
                    int _day2 = stringDayToInt(_pieces[2]);
                    int _year = DateTime.Now.Year;

                    try
                    {
                        _response.Add(new Tuple<DateTime, DateTime>(new DateTime(_year, _month, _day1), new DateTime(_year, _month, _day2)));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (_response.Any())
            {
                return _response;
            }
            #endregion

            return _response;
        }

        public static bool IsDay(string text)
        {
            //var _dayPattern = DayPattern.Replace("(", "(^");
            //_dayPattern = _dayPattern.Replace("|", "$|^");
            //_dayPattern = _dayPattern.Replace(")", "$)");
            var _dayPattern = "^" + DayPattern + "$";
            return Regex.IsMatch(text, _dayPattern);
        }
        public static bool IsMonth(string text)
        {
            var _monthPattern = MonthPattern.Replace("(", "(^");
            _monthPattern = _monthPattern.Replace("|", "$|^");
            _monthPattern = _monthPattern.Replace(")", "$)");
            return Regex.IsMatch(text, _monthPattern);
        }
        public static bool IsYear(string text)
        {
            var _yearPattern = YearPattern.Replace("(", "(^");
            _yearPattern = _yearPattern.Replace("|", "$|^");
            _yearPattern = _yearPattern.Replace(")", "$)");
            return Regex.IsMatch(text, _yearPattern);
        }

        private static int stringDayToInt(string day)
        {
            var _day = 0;

            if (!int.TryParse(day, out _day))
            {
                int.TryParse(Regex.Replace(day, @"st|nd|rd|th", ""), out _day);
            }

            return _day;
        }

        private static int stringMonthToInt(string month)
        {
            var _month = 0;
            if (!int.TryParse(month, out _month))
            {
                switch (month.ToLower())
                {
                    case "jan":
                    case "jan.":
                    case "january":
                    case "janeiro":
                        _month = 1;
                        break;
                    case "feb":
                    case "feb.":
                    case "fev":
                    case "fev.":
                    case "february":
                    case "fevereiro":
                        _month = 2;
                        break;
                    case "mar":
                    case "mar.":
                    case "march":
                    case "março":
                        _month = 3;
                        break;
                    case "apr":
                    case "apr.":
                    case "abr":
                    case "abr.":
                    case "april":
                    case "abril":
                        _month = 4;
                        break;
                    case "may":
                    case "may.":
                    case "mai":
                    case "mai.":
                    case "maio":
                        _month = 5;
                        break;
                    case "jun":
                    case "jun.":
                    case "june":
                    case "junho":
                        _month = 6;
                        break;
                    case "jul":
                    case "jul.":
                    case "july":
                    case "julho":
                        _month = 7;
                        break;
                    case "aug":
                    case "aug.":
                    case "ago":
                    case "ago.":
                    case "august":
                    case "agosto":
                        _month = 8;
                        break;
                    case "sep":
                    case "sep.":
                    case "set":
                    case "set.":
                    case "september":
                    case "setembro":
                        _month = 9;
                        break;
                    case "oct":
                    case "oct.":
                    case "out":
                    case "out.":
                    case "october":
                    case "outubro":
                        _month = 10;
                        break;
                    case "nov":
                    case "nov.":
                    case "november":
                    case "novembro":
                        _month = 11;
                        break;
                    case "dec":
                    case "dec.":
                    case "dez":
                    case "dez.":
                    case "december":
                    case "dezembro":
                        _month = 12;
                        break;
                    default:
                        return 0;
                }
            }

            return _month;
        }

        private static int stringYearToInt(string year)
        {
            if (year.Length == 2)
            {
                year = "20" + year;
            }

            int _year = 0;

            int.TryParse(year, out _year);

            return _year;
        }
    }
}
