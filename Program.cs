using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace JsonParse
{
    class Program
    {
        static void Main(string[] args)
        {

            Dictionary<string,string> settings = new Dictionary<string,string>();

            //var json = "{\"Insurer\": {\"FirstName\": \"Петрищенко\",\"LastName\": \"Федор\"},\"Vehicle\": {\"Mark\": \"Volvo\",\"Model\": \"XC90\"},\"DateBegin\": \"2016-06-06\",\"DateEnd\": \"2017-06-05\"} ";
            //settings.Add("InsurerFirstName", "Insurer/FirstName");
            //settings.Add("InsurerLastName", "Insurer/LastName");
            //settings.Add("MarkName", "Vehicle/Mark");
            //settings.Add("ModelName", "Vehicle/Model");
            //settings.Add("EffectiveDate", "DateBegin");
            //settings.Add("ExpirationDate", "DateEnd");
            //settings.Add("DocumentDate", "");
            //settings.Add("AcceptationDate", "");

            //var json = "{\"InsurerFirstName\": \"Петрищенко\",\"InsurerLastName\": \"Федор\",\"Vehicle\": {\"Mark\": \"Volvo\",\"Model\": \"XC90\"},\"EffectiveDate\": \"2016 - 06 - 06\",\"ExpirationDate\": \"2017 - 06 - 05\"}";
            //settings.Add("InsurerFirstName", "InsurerFirstName");
            //settings.Add("InsurerLastName", "InsurerLastName");
            //settings.Add("MarkName", "Vehicle/Mark");
            //settings.Add("ModelName", "Vehicle/Model");
            //settings.Add("EffectiveDate", "EffectiveDate");
            //settings.Add("ExpirationDate", "ExpirationDate");
            //settings.Add("DocumentDate", "");
            //settings.Add("AcceptationDate", "");

            //============Входные данные и параметры============
            //если параметры не задать, то имеется ввиду что данные в JSON совпадают по структуре с базовым классом.
            var json = "{\"Insurer\": {\"Type\": \"Person\",\"Person\": {\"InsurerFirstName\": \"Петрищенко\",\"InsurerLastName\": \"Федор\"}},\"VehicleMark\": \"Volvo\",\"VehicleModel\": \"XC90\",\"DateBegin\": \"2016-06-06\",\"DateEnd\": \"2017-06-05\"}";
            settings.Add("InsurerFirstName", "Insurer/Person/InsurerFirstName");
            settings.Add("InsurerLastName", "Insurer/Person/InsurerLastName");
            settings.Add("MarkName", "VehicleMark");
            settings.Add("ModelName", "VehicleModel");
            settings.Add("EffectiveDate", "DateBegin");
            settings.Add("ExpirationDate", "DateEnd");
            settings.Add("DocumentDate", "");
            settings.Add("AcceptationDate", "");
            //==================================================
            BasePolicy bp;
            if (settings.Count != 0)
            {
                JObject t = JsonConvert.DeserializeObject<dynamic>(json);
                bp = new BasePolicy();
                bp.Insurer = new Person();
                bp.Vehicle = new Vehicle();

                foreach (var item in settings)
                {
                    PropertyInfo prop = null;
                    var prValue = getObjectPath(item.Value, t);

                    if (item.Key.Contains("Date"))
                    {
                        prop = bp.GetType().GetProperty(item.Key);
                        prop.SetValue(bp, Convert.ToDateTime(prValue), null);
                    }
                    else if (item.Key == "MarkName" || item.Key == "ModelName")
                    {
                        prop = bp.Vehicle.GetType().GetProperty(item.Key);
                        prop.SetValue(bp.Vehicle, prValue.ToString(), null);
                    }
                    else if (item.Key == "InsurerFirstName" || item.Key == "InsurerLastName")
                    {
                        prop = bp.Insurer.GetType().GetProperty("Name");
                        prop.SetValue(bp.Insurer, bp.Insurer.Name + prValue.ToString() + " ", null);
                    }
                }
            }
            else {
                 bp = JsonConvert.DeserializeObject<BasePolicy>(json);
            }

            var result = bp; // Результат
        }
        
        public static JToken getObjectPath(string path, JObject jsn) {
            string[] curpath = path.Split("/");
            JToken j= jsn;
                for (int i = 0; i < curpath.Length; i++)
                {
                    j = j[curpath[i]];
                }
            return j;
        }
    }


  
class BasePolicy
    {
        public DateTime DocumentDate { get; set; } // Дата создания документа
        public DateTime EffectiveDate { get; set; } // Начало действия ДС
        public DateTime ExpirationDate { get; set; } // Окончание действия ДС
        public DateTime AcceptationDate { get; set; } // Дата акцептации ДС
        public Person Insurer { get; set; } // Страхователь
        public Vehicle Vehicle { get; set; } // Данные Автомобиля
    }

    // Класс субъекта Физ. лица
    class Person
    {
        public string Name { get; set; } // Наименование субъекта (ФИО - если ФЛ, Название организации - если ЮЛ)
    }

    // Класс автомобиля
    class Vehicle
    {
        public string MarkName { get; set; } // Наименование марки
        public string ModelName { get; set; } // Наименование модели

    }
}
