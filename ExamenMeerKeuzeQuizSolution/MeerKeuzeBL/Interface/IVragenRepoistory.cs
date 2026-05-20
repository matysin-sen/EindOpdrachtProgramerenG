using MeerKeuzeBL.Domein;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Interface
{
    public interface IVragenRepoistory
    {
        public void VoegVraagToe(Vragen vraag);
    }
}
