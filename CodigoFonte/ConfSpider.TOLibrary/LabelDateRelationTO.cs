using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.TOLibrary
{
    public class LabelDateRelationTO
    {
        //O quanto de relevância há na posição em relação à distância.
        private const int POSITIONRELEVANCE = 1;
        private const int DISTANCERELEVANCE = 1;

        public LabelNodeTO LabelNode { get; }
        public DateNodeTO DateNode { get; }

        public double MinDistance { get; }
        public EnumPositionBlockWeight Position { get; set; }
        public double Relevance { get; }

        public LabelDateRelationTO(LabelNodeTO lNode, DateNodeTO dNode)
        {
            this.LabelNode = lNode;
            this.DateNode = dNode;

            //if (LabelNode.PointC.Item1 == DateNode.PointC.Item1 &&
            //    LabelNode.PointC.Item2 == DateNode.PointC.Item2)
            if (LabelNode.PointTL.Item1 <= DateNode.PointTL.Item1 && LabelNode.PointTL.Item2 <= DateNode.PointTL.Item2 &&
                LabelNode.PointTR.Item1 <= DateNode.PointTR.Item1 && LabelNode.PointTR.Item2 >= DateNode.PointTR.Item2 &&
                LabelNode.PointBL.Item1 >= DateNode.PointBL.Item1 && LabelNode.PointBL.Item2 <= DateNode.PointBL.Item2 &&
                LabelNode.PointBR.Item1 >= DateNode.PointBR.Item1 && LabelNode.PointBR.Item2 >= DateNode.PointBR.Item2)
            {
                // um nodo sobreposto ao outro ou DateNode contido no espaço do LabelNode
                Position = EnumPositionBlockWeight.B0;
                MinDistance = 0;
            }
            else
            {
                //data na Top Left (B1)
                if (LabelNode.PointTL.Item1 >= DateNode.PointBR.Item1 && //a cima
                    LabelNode.PointTL.Item2 >= DateNode.PointBR.Item2)   //a esquerda
                {
                    Position = EnumPositionBlockWeight.B1;
                    MinDistance = DistanceBetweenTwoPoints(LabelNode.PointTL, DateNode.PointBR);
                }
                else
                {
                    //data na Top Left (B2)
                    if (LabelNode.PointTL.Item1 >= DateNode.PointBL.Item1 && //a cima
                        LabelNode.PointTL.Item2 <= DateNode.PointBL.Item2 && //nao estar a esquerda
                        LabelNode.PointTR.Item2 > DateNode.PointBL.Item2)   //nao estar a direita
                    {
                        Position = EnumPositionBlockWeight.B1;
                        MinDistance = DistanceBetweenTwoPoints(LabelNode.PointTL, DateNode.PointBL);
                    }
                    else
                    {
                        //data na Top Right (B3)
                        if (LabelNode.PointTR.Item1 >= DateNode.PointBL.Item1 && //a cima
                            LabelNode.PointTR.Item2 <= DateNode.PointBL.Item2)   //a direita
                        {
                            Position = EnumPositionBlockWeight.B3;
                            MinDistance = DistanceBetweenTwoPoints(LabelNode.PointTR, DateNode.PointBL);
                        }
                        else
                        {
                            //data na Left (B4)
                            if (LabelNode.PointTL.Item1 <= DateNode.PointTR.Item1 && //nao estar a cima
                                LabelNode.PointTL.Item2 > DateNode.PointTR.Item2 &&  //a esquerda
                                LabelNode.PointBL.Item1 > DateNode.PointTR.Item1)    //nao estar a baixo
                            {
                                Position = EnumPositionBlockWeight.B4;
                                MinDistance = DistanceBetweenTwoPoints(LabelNode.PointTL, DateNode.PointTR);
                            }
                            else
                            {
                                //data na Right (B5)
                                if (LabelNode.PointTR.Item1 <= DateNode.PointTL.Item1 && //nao estar a cima
                                    LabelNode.PointTR.Item2 <= DateNode.PointTL.Item2 &&  //a direita
                                    LabelNode.PointBR.Item1 > DateNode.PointTL.Item1)    //nao estar a baixo
                                {
                                    Position = EnumPositionBlockWeight.B5;
                                    MinDistance = DistanceBetweenTwoPoints(LabelNode.PointTR, DateNode.PointTL);
                                }
                                else
                                {
                                    //data na Bottom Left (B6)
                                    if (LabelNode.PointBL.Item1 <= DateNode.PointTR.Item1 && //a baixo
                                        LabelNode.PointBL.Item2 > DateNode.PointTR.Item2)    //a esquerda
                                    {
                                        Position = EnumPositionBlockWeight.B6;
                                        MinDistance = DistanceBetweenTwoPoints(LabelNode.PointBL, DateNode.PointTR);
                                    }
                                    else
                                    {
                                        //data na Bottom (B7)
                                        if (LabelNode.PointBL.Item1 <= DateNode.PointTL.Item1 && //a baixo
                                            LabelNode.PointBL.Item2 <= DateNode.PointTL.Item2 && //nao estar a esquerda
                                            LabelNode.PointBR.Item2 > DateNode.PointTL.Item2)    //nao estar a direita
                                        {
                                            Position = EnumPositionBlockWeight.B7;
                                            MinDistance = DistanceBetweenTwoPoints(LabelNode.PointBL, DateNode.PointTL);
                                        }
                                        else
                                        {
                                            //data na Right (B8)
                                            if (LabelNode.PointBR.Item1 <= DateNode.PointTL.Item1 && //a baixo
                                                LabelNode.PointBR.Item2 <= DateNode.PointTL.Item2)   //a direita
                                            {
                                                Position = EnumPositionBlockWeight.B8;
                                                MinDistance = DistanceBetweenTwoPoints(LabelNode.PointBR, DateNode.PointTL);
                                            }
                                            else
                                            {
                                                //relação a desconsiderar
                                                Position = EnumPositionBlockWeight.B9;
                                                MinDistance = double.MaxValue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            if (MinDistance == 0)
            {
                MinDistance = 1;
            }

            //Relevance = (POSITIONRELEVANCE * ((int)Position)) * (DISTANCERELEVANCE * (1 / MinDistance));

            Relevance = ((int)Position) - MinDistance;
        }

        private double DistanceBetweenTwoPoints(Tuple<double, double> point1, Tuple<double, double> point2)
        {
            return Math.Sqrt(Math.Pow(point1.Item1 - point2.Item1, 2) + Math.Pow(point1.Item2 - point2.Item2, 2));
        }
    }
}
