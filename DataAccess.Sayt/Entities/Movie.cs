using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Sayt.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string VideoPath { get; set; }
    public string SubtitlePath { get; set; }
}
