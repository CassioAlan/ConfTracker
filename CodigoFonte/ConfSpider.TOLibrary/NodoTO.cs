using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.TOLibrary
{
    public class NodeTO
    {
        public Tuple<double, double> PointTL { get; protected set; }
        public Tuple<double, double> PointTR { get; protected set; }
        public Tuple<double, double> PointBL { get; protected set; }
        
        public Tuple<double, double> PointBR { get; protected set; }
        //public Tuple<double, double> PointBR { get { return new Tuple<double, double>(PointBL.Item1, PointTR.Item2); } }
        public Tuple<double, double> PointC { get; protected set; }
        public string Text { get; protected set; }

        protected static Tuple<double, double> StringToTuple(string piece)
        {
            var _subPieces = piece.Split(',');
            return new Tuple<double, double>(Convert.ToDouble(_subPieces[0].Replace('.', ',')), Convert.ToDouble(_subPieces[1].Replace('.', ',')));
        }
    }

    public class LabelNodeTO : NodeTO
    {
        public EnumTipoDataPai TipoDataPai { get; }

        /*
        public LabelNodeTO(Tuple<double, double> pointTL, Tuple<double, double> pointTR, Tuple<double, double> pointBL, 
                            Tuple<double, double> pointBR, Tuple<double, double> PointC, string text, EnumTipoDataPai tipoDataPai)
        {
            this.PointTL = pointTL;
            this.PointTR = pointTR;
            this.PointBL = pointBL;
            //this.PointBR = pointBR;
            this.PointC = PointC;

            this.Text = text;
            this.TipoDataPai = tipoDataPai;
        }
        */

        public LabelNodeTO(string linha, EnumTipoDataPai tipoDataPai)
        {
            var _pieces = linha.Split(new string[] { "#|||#" }, StringSplitOptions.None);
            
            this.PointTL = StringToTuple(_pieces[1]);
            this.PointTR = StringToTuple(_pieces[2]);
            this.PointBL = StringToTuple(_pieces[3]);
            this.PointBR = StringToTuple(_pieces[4]);
            this.PointC = StringToTuple(_pieces[5]);

            this.Text = _pieces[0];
            this.TipoDataPai = tipoDataPai;
        }
    }

    public class DateNodeTO : NodeTO
    {
        public EnumDateType DateType { get; }
        public DateTime Date1 { get; }
        public DateTime Date2 { get; }

        /*
        public DateNodeTO(Tuple<double, double> pointTL, Tuple<double, double> pointTR, Tuple<double, double> pointBL,
                            Tuple<double, double> pointBR, Tuple<double, double> PointC, string text)
        {
            this.PointTL = pointTL;
            this.PointTR = pointTR;
            this.PointBL = pointBL;
            //this.PointBR = pointBR;
            this.PointC = PointC;

            this.Text = text;

            var dates = DateUtilTO.FindIntervalDatesOnText(text);
            if (dates.Count > 0)
            {
                Date1 = dates.First().Item1;
                Date2 = dates.First().Item2;
                DateType = EnumDateType.IntervalDate;
            }
            else
            {
                var date = DateUtilTO.FindDatesOnText(text);
                if (dates.Count > 0)
                {
                    //Pegar a última do nodo, pois pode acontecer de haver prorrogação, entao a data original é riscada e colocada a atual em seguida
                    Date1 = date.Last();
                    DateType = EnumDateType.SingleDate;
                }
            }
        }
        */
        public DateNodeTO(string linha)
        {
            var _pieces = linha.Split(new string[] { "#|||#" }, StringSplitOptions.None);
            
            this.PointTL = StringToTuple(_pieces[1]);
            this.PointTR = StringToTuple(_pieces[2]);
            this.PointBL = StringToTuple(_pieces[3]);
            this.PointBR = StringToTuple(_pieces[4]);
            this.PointC = StringToTuple(_pieces[5]);

            this.Text = _pieces[0];

            var dates = DateUtilTO.FindIntervalDatesOnText(_pieces[0]);
            if (dates.Count > 0)
            {
                this.Date1 = dates.First().Item1;
                this.Date2 = dates.First().Item2;
                this.DateType = EnumDateType.IntervalDate;
            }
            else
            {
                var date = DateUtilTO.FindDatesOnText(_pieces[0]);
                if (date.Count > 0)
                {
                    //Pegar a maior do nodo, pois pode acontecer de haver prorrogação, entao a data original é riscada e colocada a atual
                    this.Date1 = date.Max();
                    this.DateType = EnumDateType.SingleDate;
                }
            }
        }
    }
}
