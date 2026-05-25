using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Managers
{
    public class Manager
    {
        IVragenRepository _repository;

        public Manager(IVragenRepository repository)
        {
            _repository = repository;   
        }

        public Manager()
        {
        }

        public void VoegUserToe(string naam, string achternaam)
        {
            _repository.voegUserToe(naam, achternaam);
        }

      
        public List<Onderwerpen> GeefAlleOnderwerpen()
        {
            return _repository.GeefAlleOnderwerpen();
        }

    }
}
