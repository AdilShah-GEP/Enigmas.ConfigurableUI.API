using System;
using System.Collections.Generic;
using System.Text;

namespace enigmas.ConfigurableUI.Core.Entity
{
    public class InputModel
    {
        public string DefaultValue { get; set; }

        public string Value { get; set; }

        public bool ShowIText { get; set; }

        public string ITextValue { get; set; }
    }
}
