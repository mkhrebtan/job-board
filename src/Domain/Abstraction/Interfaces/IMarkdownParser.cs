using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstraction.Interfaces;

public interface IMarkdownParser
{
    string ToPlainText(string markdown);
}
