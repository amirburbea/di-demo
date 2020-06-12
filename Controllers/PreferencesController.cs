using System;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreferencesController : ControllerBase
    {
        private static readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "prefs.txt");

        [HttpGet]
        public ActionResult<Preferences> GetPreferences()
        {
            if (!System.IO.File.Exists(_filePath))
            {
                return this.NotFound();
            }
            string text = System.IO.File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<Preferences>(text);
        }

        [HttpPost]
        public ActionResult SetPrefences([FromBody] Preferences preferences)
        {
            System.IO.File.WriteAllText(_filePath, JsonSerializer.Serialize(preferences));
            return this.NoContent();
        }

        public class Preferences
        {
            public string ThemeName { get; set; }
        }
    }
}
