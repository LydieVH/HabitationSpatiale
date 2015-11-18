using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppliMartienneTest
{
    class Mission
    {
        public string _nomMission
        {
            get;
            set;
        }

        public DateTime _dateDebut
        {
            get;
            set;
        }

        public int _dureeMission
        {
            get;
            set;
        }

        public DateTime _dateFin
        {
            get;
            set;
        }

        public DateTime _jourJ
        {
            get;
            set;
        }

        public List<Astronaute> _astronautes
        {
            get;
            set;
        }

        public int _nbAstronautes
        {
            get;
            set;
        }

        public string _cheminGeneralXML
        {
            get;
            set;
        }

        public string _cheminPlanningXML
        {
            get;
            set;
        }

        public Planning _planning
        {
            get;
            set;
        }


        // Création d'une Mission à partir d'un fichier XML
        public Mission (string cheminXMLGeneral)
        {
            // chargement du XML général 
            XDocument _generalXML = XDocument.Load(cheminXMLGeneral);
            _nomMission = _generalXML.Element("Mission").Element("NomMission").Value;
            _dateDebut = DateTime.Parse(_generalXML.Element("Mission").Element("DateDebut").Value);
            _dureeMission = int.Parse(_generalXML.Element("Mission").Element("Duree").Value);
            // Détermination de la date de fin de la mission
            _jourJ = _dateDebut;
            _nbAstronautes = 0;
            _astronautes = new List<Astronaute>();
            var astronautes = from astronaute in _generalXML.Descendants("Astronautes") select astronaute;
            foreach (XElement a in astronautes.Elements("Astronaute"))
            {
                string nomAstronaute = a.Value;
                _astronautes.Add(new Astronaute(nomAstronaute));
                _nbAstronautes++;
            }
            // Création du planning
        }
    }
}
