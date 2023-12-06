﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prova2_PDMI
{
    public class Livro
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string NomeAutor { get; set; }
        public string EmailAutor { get; set; }
        public string ISBN { get; set; }
    }
}
