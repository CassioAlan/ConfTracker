using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.TOLibrary
{
    public enum EnumTipoExtracao
    {
        Manual = 1,

        AnaliseTexto = 2,
        AnaliseTexto_URLsGoldstandard = 3,

        Posicional = 4,
        Posicional_URLsGoldstandard = 5,

        Texto_Posicional_URLsGoldstandard = 6,

        CRF = 7,

        CRF_Posicional = 8,

        Google = 99
    }

    public enum EnumTipoDataPai
    {
        AbstractSubmission = 1,
        PaperSubmission = 5,
        AcceptanceNotification = 17,
        CameraReadyDue = 26,
        ConferenceStart = 33,
        ConferenceEnd = 37
    }

    public enum EnumDateType
    {
        SingleDate = 1,
        IntervalDate = 2
    }

    public enum EnumPositionBlockWeight
    {
        [Description("Same block")]
        B0 = 1000,
        [Description("Top Left")]
        B1 = 100,
        [Description("Top")]
        B2 = 400,
        [Description("Top Right")]
        B3 = 100,
        [Description("Left")]
        B4 = 800,
        [Description("Right")]
        B5 = 800,
        [Description("Botton Left")]
        B6 = 100,
        [Description("Botton")]
        B7 = 500,
        [Description("Botton Right")]
        B8 = 100,
        [Description("Nodo que pode ser desconsiderado")]
        B9 = 0
    }
}
