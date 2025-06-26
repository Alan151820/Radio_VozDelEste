using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using RestSharp;
using RadioVozDelEste.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace RadioVozDelEste.Controllers
{

    public class ClimaController : Controller
    {
        private readonly string apiUrl = "https://api.openweathermap.org/data/2.5/forecast?lat=-34.9&lon=-54.94&appid=a54ba4791103121fb778a7e957021412&units=metric";

        public async Task<ActionResult> ClimaIndex()
        {
            WeatherResponse datosClima = new WeatherResponse();

            using (HttpClient cliente = new HttpClient())
            {
                HttpResponseMessage respuesta = await cliente.GetAsync(apiUrl);

                if (respuesta.IsSuccessStatusCode)
                {
                    string json = await respuesta.Content.ReadAsStringAsync();
                    datosClima = JsonConvert.DeserializeObject<WeatherResponse>(json);
                }
                else
                {
                    ViewBag.Error = "No se pudo obtener el pronóstico.";
                }
            }

            return View(datosClima);
        }
    }
}
