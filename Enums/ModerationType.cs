using System.ComponentModel;

namespace BlogProjectPrac7.Enums
{
    public enum ModerationType
    {

        [Description("Political propaganda")]
        Political,
        [Description("Offensive anguage")]
        Language,
        [Description("Drug references")]
        Drugs,
        [Description("Threatening speech")]
        Threatening,
        [Description("Sexual content")]
        Sexual,
        [Description("Hate speech")]
        HateSpeech,
        [Description("Targeted shaming")]
        Shaming
    }
}
