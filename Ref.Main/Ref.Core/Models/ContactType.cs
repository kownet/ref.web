using System.ComponentModel;

namespace Ref.Core.Models
{
    public enum ContactType
    {
        [Description("Rejestracja wersji Demo")]
        Demo = 0,

        [Description("Pytanie o wersje Standard")]
        Standard = 1,

        [Description("Pytanie o wersje Premium")]
        Premium = 2
    }
}