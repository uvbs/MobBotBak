#region using directives

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PoGo.PokeMobBot.Logic.Logging;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using System.Linq;
using GeoCoordinatePortable;
using PoGo.PokeMobBot.Logic.API;

#endregion

namespace PoGo.PokeMobBot.Logic
{
    internal class AuthSettings
    {
        [JsonIgnore]
        private string _filePath;
        public AuthType AuthType;
        public string GoogleRefreshToken = "";
        public string GoogleUsername;
        public string GooglePassword;
        public string PtcUsername;
        public string PtcPassword;
        public bool UseProxy;
        public string ProxyLogin;
        public string ProxyPass;
        public string ProxyUri;

        public void Load(string path)
        {
            try
            {
                _filePath = path;

                if (File.Exists(_filePath))
                {
                    //if the file exists, load the settings
                    var input = File.ReadAllText(_filePath);

                    var settings = new JsonSerializerSettings();
                    settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });

                    JsonConvert.PopulateObject(input, this, settings);
                }
                else
                {
                    Save(_filePath);
                }
            }
            catch (JsonReaderException exception)
            {
                if (exception.Message.Contains("Unexpected character") && exception.Message.Contains("PtcUsername"))
                    Logger.Write("JSON Exception: You need to properly configure your PtcUsername using quotations.",
                        LogLevel.Error);
                else if (exception.Message.Contains("Unexpected character") && exception.Message.Contains("PtcPassword"))
                    Logger.Write(
                        "JSON Exception: You need to properly configure your PtcPassword using quotations.",
                        LogLevel.Error);
                else if (exception.Message.Contains("Unexpected character") &&
                         exception.Message.Contains("GoogleUsername"))
                    Logger.Write(
                        "JSON Exception: You need to properly configure your GoogleUsername using quotations.",
                        LogLevel.Error);
                else if (exception.Message.Contains("Unexpected character") &&
                         exception.Message.Contains("GooglePassword"))
                    Logger.Write(
                        "JSON Exception: You need to properly configure your GooglePassword using quotations.",
                        LogLevel.Error);
                else
                    Logger.Write("JSON Exception: " + exception.Message, LogLevel.Error);
            }
        }

        public void Save(string path)
        {
            var output = JsonConvert.SerializeObject(this, Formatting.Indented,
                new StringEnumConverter { CamelCaseText = true });

            var folder = Path.GetDirectoryName(path);
            if (folder != null && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllText(path, output);
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(_filePath))
            {
                Save(_filePath);
            }
        }
    }

    public static class RuntimeSettings
    {
        public static int StopsHit = 0;
        public static DateTime startTime = DateTime.Now;
        public static bool DelayingScan = false;
        public static int PokemonScanDelay = 10000;// in ms
        public static bool BreakOutOfPathing = false;
        public static string lastPokeStopId = "69694201337";//these don't matter
        public static string TargetStopID = "420Ayylmao";//so keep them funny. Stop PRing lame changes to these. Go crazy
        public static GeoCoordinatePortable.GeoCoordinate lastPokeStopCoordinate = new GeoCoordinate(0,0);
        public static bool CheckScan()
        {
            if (DelayingScan)
            {
                if ((DateTime.Now.Subtract(startTime).TotalMilliseconds > PokemonScanDelay) && DelayingScan)
                {
                    DelayingScan = false;
                }
                return false;
            }
            else
            {
                startTime = DateTime.Now;
                DelayingScan = true;
                return true;
            }
        }
    }


    public class DeviceSettings
    {

        private static Random random = new Random();
        public static IDictionary<string, string> phone_item = RandomPhone();
        #region DeviceSettingsTemplate
        //public string DeviceId = RandomString(16,"0123456789abcdef"); // "ro.build.id";
        //public string AndroidBoardName = "thunderc"; // "ro.product.board";
        //public string AndroidBootLoader = "unknown"; //"ro.product.bootloader; //I think
        //public string DeviceBrand = "LGE";// "product.brand";
        //public string DeviceModel = "thunderc"; //"product.device";
        //public string DeviceModelIdentifier = "GWK74 10282011";// "build.display.id";
        //public string DeviceModelBoot = "thunderc"; //"boot.hardware";
        //public string HardwareManufacturer = "LGE"; //"product.manufacturer";
        //public string HardWareModel = "LG-VS660"; //"product.model";
        //public string FirmwareBrand = "thunderc"; //"product.name"; //iOS is "iPhone OS"
        //public string FirmwareTags = "test-keys"; //"build.tags";
        //public string FirmwareType = "eng"; //"build.type"; //iOS is "iOS version"
        //public string FirmwareFingerprint = "lge/lge_gelato/VM701:2.3.4/GRJ22/ZV4.19cd75186d:user/release-keys"; //"build.fingerprint";
        #endregion
        #region New Format by Lars
        /*
        public string DeviceId = RandomString(16, "0123456789abcdef"); // "ro.build.id";
        public string AndroidBoardName = phone_item["AndriodBoardName"]; // "ro.product.board";
        public string AndroidBootLoader = phone_item["AndroidBootLoader"]; //"ro.product.bootloader; //I think
        public string DeviceBrand = phone_item["DeviceBrand"];// "product.brand";
        public string DeviceModel = phone_item["DeviceModel"]; //"product.device";
        public string DeviceModelIdentifier = phone_item["DeviceModelIdentifier"] + "_" + RandomString(random.Next(4, 10), "0123456789abcdef");// "build.display.id";
        public string DeviceModelBoot = phone_item["DeviceModelBoot"]; //"boot.hardware";
        public string HardwareManufacturer = phone_item["DeviceModelBoot"]; //"product.manufacturer";
        public string HardWareModel = phone_item["DeviceModelIdentifier"]; //"product.model";
        public string FirmwareBrand = phone_item["DeviceModel"]; //"product.name"; //iOS is "iPhone OS"
        public string FirmwareTags = "test-keys"; //"build.tags";
        public string FirmwareType = "en"; //"build.type"; //iOS is "iOS version"
        public string FirmwareFingerprint = phone_item["DeviceBrand"] + "/" + 
                                                    phone_item["DeviceModel"] + "/" +
                                                    phone_item["FirmwareBrand"] + ":" + 
                                                    RandomAndroidVersion() + "/" +
                                                    RandomString(random.Next(4, 10), "0123456789abcdef") + 
                                                    ":user/release-keys";

        */
        #endregion
        public string DeviceId = RandomString(16, "0123456789abcdef"); // "ro.build.id";
        public string AndroidBoardName = phone_item["board"]; // "ro.product.board";
        public string AndroidBootLoader = "unknown"; //"ro.product.bootloader; //I think
        public string DeviceBrand = phone_item["mft"];// "product.brand";
        public string DeviceModel = phone_item["model"]; //"product.device";
        public string DeviceModelIdentifier = phone_item["name"] + "_" + RandomString(random.Next(4, 10), "0123456789abcdef");// "build.display.id";
        public string DeviceModelBoot = phone_item["board"]; //"boot.hardware";
        public string HardwareManufacturer = phone_item["mft"]; //"product.manufacturer";
        public string HardWareModel = phone_item["model"]; //"product.model";
        public string FirmwareBrand = phone_item["board"]; //"product.name"; //iOS is "iPhone OS"
        public string FirmwareTags = "test-keys"; //"build.tags";
        public string FirmwareType = "eng"; //"build.type"; //iOS is "iOS version"
        public string FirmwareFingerprint =
           phone_item["mft"] + "/" +
            phone_item["mft"] + "_" + phone_item["board"] + ":" +
                                                    RandomAndroidVersion() + "/" +
                                                    RandomString(random.Next(4, 10), "0123456789abcdef") +
                                                    ":user/release-keys";



        private static string RandomString(int length, string chars)
        {
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private static string RandomAndroidVersion()
        {
            //possible android versions based on PokemonGo requirements
            List<string> possibleAndroidVersions = new List<string>() { "4.4", "4.4.1", "4.4.2", "4.4.3", "4.4.4", "5.0", "5.0.1", "5.0.2", "5.1", "5.1.1", "6.0", "6.0.1" };
            //generate a random index to choose version
            int index = random.Next(0, possibleAndroidVersions.Count);
            //return random vserion
            return possibleAndroidVersions[index];
        }
        public static IDictionary<string, string> RandomPhone()
        {
            List<IDictionary<string, string>> phone_list = GetPhoneList();
            Random rnd = new Random();

            int phone_index = rnd.Next(0, phone_list.Count);
            IDictionary<string, string> phone_item = phone_list[phone_index];
            return phone_item;
        }

        #region GetPhoneList
        // data from https://conf.skype.com/whitelist26.txt
        private static List<IDictionary<string, string>> GetPhoneList()
        {
            List<IDictionary<string, string>> phone_list = new List<IDictionary<string, string>>();


            IDictionary<string, string> phone_item = new Dictionary<string, string>();


            // ******* Samsung ******* //

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Nexus S";
            phone_item["mft"] = "samsung";
            phone_item["board"] = "herring";
            phone_item["model"] = "Nexus S";
            phone_item["product"] = "soju.*";
            phone_item["device"] = "crespo.*";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Galaxy Tab 10.1 (Wifi)";
            phone_item["mft"] = "samsung";
            phone_item["board"] = "samsung";
            phone_item["board"] = "GT-P7510";
            phone_item["model"] = "GT-P7510";
            phone_item["product"] = "GT-P7510";
            phone_item["device"] = "GT-P7510";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Galaxy Nexus";
            phone_item["mft"] = "samsung";
            phone_item["board"] = "google";
            phone_item["board"] = "tuna";
            phone_item["model"] = "Galaxy Nexus";
            phone_item["product"] = "mysid";
            phone_item["device"] = "toro";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Galaxy Nexus";
            phone_item["mft"] = "samsung";
            phone_item["board"] = "google";
            phone_item["board"] = "tuna";
            phone_item["model"] = "Galaxy Nexus";
            phone_item["product"] = "yakju";
            phone_item["device"] = "maguro";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Samsung Galaxy S 4G";
            phone_item["mft"] = "Samsung";
            phone_item["board"] = "TMOUS";
            phone_item["board"] = "SGH-T959V";
            phone_item["model"] = "SGH-T959V";
            phone_item["product"] = "SGH-T959V";
            phone_item["device"] = "SGH-T959V";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Samsung Galaxy S";
            phone_item["mft"] = "samsung";
            phone_item["board"] = "sprint";
            phone_item["board"] = "GT-I9000.*";
            phone_item["model"] = "GT-I9000.*";
            phone_item["product"] = "GT-I9000.*";
            phone_item["device"] = "GT-I9000.*";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Samsung Galaxy S Fascinate";
            phone_item["mft"] = "samsung";
            phone_item["board"] = "verizon";
            phone_item["board"] = "SCH-I500";
            phone_item["model"] = "SCH-I500";
            phone_item["product"] = "SCH-I500";
            phone_item["device"] = "SCH-I500";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Samsung Droid Charge";
            phone_item["mft"] = "Samsung";
            phone_item["board"] = "verizon";
            phone_item["board"] = "SCH-I510";
            phone_item["model"] = "SCH-I510";
            phone_item["product"] = "SCH-I510";
            phone_item["device"] = "SCH-I510";


            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Samsung Galaxy S II";
            phone_item["mft"] = "samsung";
            phone_item["board"] = "samsung";
            phone_item["board"] = "GT-I9100";
            phone_item["model"] = "GT-I9100";
            phone_item["product"] = "GT-I9100";
            phone_item["device"] = "GT-I9100";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Samsung Galaxy S II (Sprint)";
            phone_item["mft"] = "samsung";
            phone_item["board"] = "samsung";
            phone_item["board"] = "SPH-D710";
            phone_item["model"] = "SPH-D710";
            phone_item["product"] = "SPH-D710";
            phone_item["device"] = "SPH-D710";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Samsung Galaxy Tab 7 WIFI";
            phone_item["mft"] = "samsung";
            phone_item["board"] = "samsung";
            phone_item["board"] = "GT-P10.*";
            phone_item["model"] = "GT-P10.*";
            phone_item["product"] = "GT-P10.*";
            phone_item["device"] = "GT-P10.*";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Samsung Galaxy Tab 7 Verizon";
            phone_item["mft"] = "samsung";
            phone_item["board"] = "verizon";
            phone_item["board"] = "SCH-I800";
            phone_item["model"] = "SCH-I800";
            phone_item["product"] = "SCH-I800";
            phone_item["device"] = "SCH-I800";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Samsung Galaxy Tab 7 Sprint";
            phone_item["mft"] = "samsung";
            phone_item["board"] = "sprint";
            phone_item["board"] = "SPH-P100";
            phone_item["model"] = "SPH-P100";
            phone_item["product"] = "SPH-P100";
            phone_item["device"] = "SPH-P100";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Galaxy Tab 10.1 (T-Mo)";
            phone_item["mft"] = "samsung";
            phone_item["board"] = "samsung";
            phone_item["board"] = "SGH-T859";
            phone_item["model"] = "SGH-T859";
            phone_item["product"] = "SGH-T859";
            phone_item["device"] = "SGH-T859";
            phone_list.Add(phone_item);


            // ******* HTC ******* //

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Nexus One";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "google";
            phone_item["board"] = "mahimahi";
            phone_item["model"] = "Nexus One";
            phone_item["product"] = "passion";
            phone_item["device"] = "passion";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "HTC Amaze 4G";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "telus_wwe";
            phone_item["board"] = "ruby";
            phone_item["model"] = "HTC Ruby";
            phone_item["product"] = "htc_ruby";
            phone_item["device"] = "ruby";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "HTC Desire";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "bravo";
            phone_item["model"] = "HTC Desire";
            phone_item["product"] = "htc_bravo";
            phone_item["device"] = "bravo";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "HTC Desire S";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "saga";
            phone_item["model"] = "HTC Desire S";
            phone_item["product"] = "htc_saga";
            phone_item["device"] = "saga";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "HTC Incredible S";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "htc_wwe";
            phone_item["board"] = "vivo";
            phone_item["model"] = "HTC Incredible S";
            phone_item["product"] = "htc_vivo";
            phone_item["device"] = "vivo";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "HTC Desire HD";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "htc_wwe";
            phone_item["board"] = "spade";
            phone_item["model"] = "Desire HD";
            phone_item["product"] = "htc_ace";
            phone_item["device"] = "ace";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "HTC EVO 4G";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "sprint";
            phone_item["board"] = "supersonic";
            phone_item["model"] = "PC36100";
            phone_item["product"] = "htc_supersonic";
            phone_item["device"] = "supersonic";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "HTC EVO 3D";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "sprint";
            phone_item["board"] = "shooter.*";
            phone_item["model"] = "PG86100";
            phone_item["product"] = "htc_shooter.*";
            phone_item["device"] = "shooter.*";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "HTC Sensation 4G";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "tmous";
            phone_item["board"] = "pyramid";
            phone_item["model"] = "HTC Sensation 4G";
            phone_item["product"] = "htc_pyramid";
            phone_item["device"] = "pyramid";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "HTC Thunderbolt";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "verizon_wwe";
            phone_item["board"] = "mecha";
            phone_item["model"] = "ADR6400L";
            phone_item["product"] = "htc_mecha";
            phone_item["device"] = "mecha";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "HTC Flyer Wifi HC";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "HTC";
            phone_item["board"] = "flyer";
            phone_item["model"] = "HTC P510e";
            phone_item["product"] = "htc_flyer";
            phone_item["device"] = "flyer";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "HTC Flyer Wifi";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "HTC";
            phone_item["board"] = "flyer";
            phone_item["model"] = "HTC P510e";
            phone_item["product"] = "htc_flyer";
            phone_item["device"] = "flyer";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "HTC Flyer";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "htc_wwe_wifi";
            phone_item["board"] = "flyer";
            phone_item["model"] = "HTC Flyer P512";
            phone_item["product"] = "htc_flyer";
            phone_item["device"] = "flyer";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "HTC Flyer Wifi 2";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "HTC";
            phone_item["board"] = "flyer";
            phone_item["model"] = "HTC Flyer";
            phone_item["product"] = "htc_flyer";
            phone_item["device"] = "flyer";


            phone_list.Add(phone_item);


            // ******* Lenovo ******* //

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Lenovo IdeaPad K1";
            phone_item["mft"] = "LENOVO";
            phone_item["board"] = "LENOVO";
            phone_item["board"] = "ventana";
            phone_item["model"] = "K1";
            phone_item["product"] = "IdeaPad_Tablet_K1";
            phone_item["device"] = "K1";
            phone_list.Add(phone_item);


            // ******* MOTOROLA ******* //   
            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Motorola Droid 4";
            phone_item["mft"] = "motorola";
            phone_item["board"] = "verizon";
            phone_item["board"] = "maserati";
            phone_item["model"] = "DROID4";
            phone_item["product"] = "maserati_vzw";
            phone_item["device"] = "cdma_maserati";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Motorola Droid RAZR Verizon";
            phone_item["mft"] = "motorola";
            phone_item["board"] = "verizon";
            phone_item["board"] = "spyder";
            phone_item["model"] = "DROID RAZR";
            phone_item["product"] = "spyder_vzw";
            phone_item["device"] = "cdma_spyder";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Motorola Droid RAZR";
            phone_item["mft"] = "motorola";
            phone_item["board"] = "MOTO";
            phone_item["board"] = "spyder";
            phone_item["model"] = "XT910";
            phone_item["product"] = "XT910_O2GB";
            phone_item["device"] = "umts_spyder";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Motorola Xoom2";
            phone_item["mft"] = "Motorola";
            phone_item["board"] = "Motorola";
            phone_item["board"] = "ventana";
            phone_item["model"] = "MZ505";
            phone_item["product"] = "MZ505";
            phone_item["device"] = "Graham";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Motorola Atrix 2";
            phone_item["mft"] = "motorola";
            phone_item["board"] = "MOTO";
            phone_item["board"] = "p3";
            phone_item["model"] = "MB865";
            phone_item["product"] = "edison_att_us";
            phone_item["device"] = "edison";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Motorola Atrix";
            phone_item["mft"] = "motorola";
            phone_item["board"] = "MOTO";
            phone_item["board"] = "olympus";
            phone_item["model"] = "MB860";
            phone_item["product"] = "oly.*";
            phone_item["device"] = "olympus";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Motorola Photon";
            phone_item["mft"] = "motorola";
            phone_item["board"] = "sprint";
            phone_item["board"] = "sunfire";
            phone_item["model"] = "MB855";
            phone_item["product"] = "moto_sunfire";
            phone_item["device"] = "sunfire";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Motorola Droid 3";
            phone_item["mft"] = "motorola";
            phone_item["board"] = "verizon";
            phone_item["board"] = "solana";
            phone_item["model"] = "DROID3";
            phone_item["product"] = "solana_vzw";
            phone_item["device"] = "cdma_solana";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Motorola Bionic";
            phone_item["mft"] = "motorola";
            phone_item["board"] = "verizon";
            phone_item["board"] = "targa";
            phone_item["model"] = "DROID BIONIC";
            phone_item["product"] = "targa_vzw";
            phone_item["device"] = "cdma_targa";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Motorola Xoom";
            phone_item["mft"] = "motorola";
            phone_item["board"] = "verizon";
            phone_item["board"] = "unknown";
            phone_item["model"] = "Xoom";
            phone_item["product"] = "trygon";
            phone_item["device"] = "stingray";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Motorola Pasteur";
            phone_item["mft"] = "Motorola";
            phone_item["board"] = "verizon";
            phone_item["board"] = "pasteur";
            phone_item["model"] = "MZ617";
            phone_item["product"] = "pasteur";
            phone_item["device"] = "pasteur";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Motorola Fleming";
            phone_item["mft"] = "Motorola";
            phone_item["board"] = "Motorola";
            phone_item["board"] = "fleming";
            phone_item["model"] = "XOOM 2 ME";
            phone_item["product"] = "RTCOREEU";
            phone_item["device"] = "fleming";
            phone_list.Add(phone_item);


            // ******* Sony Ericsson ******* //

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Sony Ericsson Xperia Neo";
            phone_item["mft"] = "Sony Ericsson";
            phone_item["board"] = "SEMC";
            phone_item["board"] = "unknown";
            phone_item["model"] = "MT15[ai]";
            phone_item["product"] = "MT15[ai]_.*";
            phone_item["device"] = "MT15[ai]";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Sony Ericsson Xperia Pro";
            phone_item["mft"] = "Sony Ericsson";
            phone_item["board"] = "SEMC";
            phone_item["board"] = "unknown";
            phone_item["model"] = "MK16[ai]";
            phone_item["product"] = "MK16[ai]_.*";
            phone_item["device"] = "MK16[ai]";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Sony Ericsson Xperia Play ROW";
            phone_item["mft"] = "Sony Ericsson";
            phone_item["board"] = "SEMC";
            phone_item["board"] = "unknown";
            phone_item["model"] = "R800.*";
            phone_item["product"] = "R800.*";
            phone_item["device"] = "R800.*";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Sony Ericsson Xperia Play China";
            phone_item["mft"] = "Sony Ericsson";
            phone_item["board"] = "SEMC";
            phone_item["board"] = "unknown";
            phone_item["model"] = "Z1.*";
            phone_item["product"] = "Z1.*";
            phone_item["device"] = "Z1.*";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Sony Ericsson Xperia Ray";
            phone_item["mft"] = "Sony Ericsson";
            phone_item["board"] = "SEMC";
            phone_item["board"] = "unknown";
            phone_item["model"] = "ST18[ai]";
            phone_item["product"] = "ST18[ai]_.*";
            phone_item["device"] = "ST18[ai]";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Sony Ericsson Xperia Mini Pro2";
            phone_item["mft"] = "Sony Ericsson";
            phone_item["board"] = "SEMC";
            phone_item["board"] = "unknown";
            phone_item["model"] = "SK17[ai]";
            phone_item["product"] = "SK17[ai]_.*";
            phone_item["device"] = "SK17[ai]";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Sony Ericsson Xperia Walkman";
            phone_item["mft"] = "Sony Ericsson";
            phone_item["board"] = "SEMC";
            phone_item["board"] = "unknown";
            phone_item["model"] = "WT19[ai]";
            phone_item["product"] = "WT19[ai]_.*";
            phone_item["device"] = "WT19[ai]";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Sony Ericsson Xperia NeoV";
            phone_item["mft"] = "Sony Ericsson";
            phone_item["board"] = "SEMC";
            phone_item["board"] = "unknown";
            phone_item["model"] = "MT11[ai]";
            phone_item["product"] = "MT11[ai]_.*";
            phone_item["device"] = "MT11[ai]";
            phone_list.Add(phone_item);

            // ******* Acer ******* //

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Acer A5"; phone_item["mft"] = "Acer";
            phone_item["board"] = "acer";
            phone_item["board"] = "jazz";
            phone_item["model"] = "S300";
            phone_item["product"] = "a5_generic.*";
            phone_item["device"] = "a5";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "Acer Iconia Tablet";
            phone_item["mft"] = "Acer";
            phone_item["board"] = "acer";
            phone_item["board"] = "picasso";
            phone_item["model"] = "A500";
            phone_item["product"] = "picasso_comgen.*";
            phone_item["device"] = "picasso";
            phone_list.Add(phone_item);


            // ******* LG ******* //
            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "LG Revolution";
            phone_item["mft"] = "LGE";
            phone_item["board"] = "Verizon";
            phone_item["board"] = "bryce";
            phone_item["model"] = "VS910 4G";
            phone_item["product"] = "bryce";
            phone_item["device"] = "bryce";

            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "LG Optimus Black";
            phone_item["mft"] = "lge";
            phone_item["board"] = "lge";
            phone_item["board"] = "lgp970";
            phone_item["model"] = "LG-P970";
            phone_item["product"] = "lge_bprj";
            phone_item["device"] = "lgp970";


            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "LG Optimus 3D";
            phone_item["mft"] = "LGE";
            phone_item["board"] = "lge";
            phone_item["board"] = "omap4sdp";
            phone_item["model"] = "LG-P920";
            phone_item["product"] = "lge_Cosmopolitan";
            phone_item["device"] = "p920";


            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "LG Optimus 2x";
            phone_item["mft"] = "lge";
            phone_item["board"] = "lge";
            phone_item["board"] = "p990";
            phone_item["model"] = "LG-P990";
            phone_item["product"] = "lge_star";
            phone_item["device"] = "p990";


            phone_list.Add(phone_item);


            // ******* ASUS ******* //
            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "ASUS Transfomer Prime";
            phone_item["mft"] = "asus";
            phone_item["board"] = "asus";
            phone_item["board"] = "EeePad";
            phone_item["model"] = "Transformer Prime TF201";
            phone_item["product"] = "TW_epad";
            phone_item["device"] = "TF201";
            phone_list.Add(phone_item);


            // ******* KDDI ******* //
            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "ISW11M";
            phone_item["mft"] = "motorola";
            phone_item["board"] = "KDDI";
            phone_item["board"] = "sunfire";
            phone_item["model"] = "ISW11M";
            phone_item["product"] = "MOI11";
            phone_item["device"] = "sunfire";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "IS05";
            phone_item["mft"] = "SHARP";
            phone_item["board"] = "KDDI";
            phone_item["board"] = "SHI05";
            phone_item["model"] = "IS05";
            phone_item["product"] = "SHI05";
            phone_item["device"] = "SHI05";
            phone_list.Add(phone_item);

            phone_item = new Dictionary<string, string>();
            phone_item["name"] = "ISW12HT";
            phone_item["mft"] = "HTC";
            phone_item["board"] = "KDDI";
            phone_item["board"] = "shooterk";
            phone_item["model"] = "ISW12HT";
            phone_item["product"] = "HTI12";
            phone_item["device"] = "shooterk";
            phone_list.Add(phone_item);

            return phone_list;

        }
        #endregion




    }
    public class DelaySettings
    {
        [JsonIgnore]
        private static Random r = new Random();
        [JsonIgnore]
        private static int FirstRunMin = 2141;
        [JsonIgnore]
        private static int FirstRunMax = 6736;

        //delays
        public int MinRandomizeDelayMilliseconds = FirstRunMin;
        public int MaxRandomizeDelayMilliseconds = FirstRunMax;
        public bool ReRandomizeDelayOnStart = true;
        public int DelayBetweenPlayerActions = r.Next(FirstRunMin, FirstRunMax);
        public int DelayPositionCheckState = r.Next(FirstRunMin, FirstRunMax);
        public int DelayPokestop = r.Next(FirstRunMin, FirstRunMax);
        public int DelayCatchPokemon = r.Next(FirstRunMin, FirstRunMax);
        public int DelayBetweenPokemonCatch = r.Next(FirstRunMin, FirstRunMax);
        public int DelayCatchNearbyPokemon = r.Next(FirstRunMin, FirstRunMax);
        public int DelayCatchLurePokemon = r.Next(FirstRunMin, FirstRunMax);
        public int DelayCatchIncensePokemon = r.Next(FirstRunMin, FirstRunMax);
        //public int DelayEvolvePokemon = r.Next(FirstRunMin, FirstRunMax); //reports say this takes ~25seconds give or take.
        public int DelayEvolvePokemon = r.Next(24000, 26000); //hardcoded to allow for maximum humanization
        public double DelayEvolveVariation = 0.3;
        public int DelayTransferPokemon = r.Next(FirstRunMin, FirstRunMax);
        public int DelayDisplayPokemon = r.Next(FirstRunMin, FirstRunMax);
        public int DelayUseLuckyEgg = r.Next(FirstRunMin, FirstRunMax);
        public int DelaySoftbanRetry = r.Next(FirstRunMin, FirstRunMax);
        public int DelayRecyleItem = r.Next(FirstRunMin, FirstRunMax);
        public int DelaySnipePokemon = r.Next(FirstRunMin, FirstRunMax);
        public int MinDelayBetweenSnipes = 10000;
        public double SnipingScanOffset = 0.003;
    }

    public class StartUpSettings
    {
        //bot start
        public bool AutoUpdate = false;
        public bool TransferConfigAndAuthOnUpdate = true;
        public bool DumpPokemonStats = false;
        public int AmountOfPokemonToDisplayOnStartCp = 10;
        public int AmountOfPokemonToDisplayOnStartIv = 10;
        public bool StartupWelcomeDelay = false;
        public string TranslationLanguageCode = "en";
        public int WebSocketPort = 14251;
        //display
        public bool DisplayPokemonMaxPoweredCp = true;
        public bool DisplayPokemonMovesetRank = true;
        //Login exception
        public bool StopBotToAvoidBanOnUnknownLoginError = true;

        //humanized pathing
        public bool UseHumanPathing = true;
    }

    public class PokemonConfig
    {
        //incubator
        public bool UseEggIncubators = true;



        //rename
        public bool RenamePokemon = false;
        public bool RenameOnlyAboveIv = true;
        public string RenameTemplate = "{1}_{0}";

        //transfer
        public bool TransferDuplicatePokemon = true;
        public bool PrioritizeIvOverCp = true;
        public bool PrioritizeBothIvAndCpForTransfer = false;
        public int KeepMinCp = 2000;
        public float KeepMinIvPercentage = 97;
        public int KeepMinDuplicatePokemon = 1;
        public bool KeepPokemonsThatCanEvolve = false;

        //evolve
        public bool EvolveAllPokemonWithEnoughCandy = false;
        public bool EvolveAllPokemonAboveIv = true;
        public float EvolveAboveIvValue = 98;
        public bool UseLuckyEggsWhileEvolving = true;
        public int UseLuckyEggsMinPokemonAmount = 15;

        //levelup
        public bool AutomaticallyLevelUpPokemon = false;
        public string LevelUpByCPorIv = "iv";
        public float UpgradePokemonCpMinimum = 1000;
        public float UpgradePokemonIvMinimum = 95;

        //favorite
        public bool AutoFavoritePokemon = true;
        public float FavoriteMinIvPercentage = 98;

        
    }

    public class LocationSettings
    {

        //coords and movement
        private static Random random = new Random();
        public bool Teleport = false;
        public double DefaultLatitude = 40.782425 + double.Parse( "0.00" + random.Next(0, 900).ToString());
        public double DefaultLongitude = -73.964654 + double.Parse( "0.00" + random.Next(0, 900).ToString());
        public double DefaultAltitude = random.Next(8, 12);
        public double WalkingSpeedInKilometerPerHour = random.Next(30, 50);
        public int MaxSpawnLocationOffset = 10;
        public int MaxTravelDistanceInMeters = 5000;
        public bool UseGpxPathing = false;
        public string GpxFile = "GPXPath.GPX";
        public bool UsePokeStopLuckyNumber = true;
        public int PokestopSkipLuckyNumberMinUse = 3;
        public int PokestopSkipLuckyNumber = 1;
        public int PokestopSkipLuckyMin = 0;
        public int PokestopSkipLuckyMax = 4;
        //random walking speed
        public bool RandomizeWalkingSpeed = true;
        public int MinutesUntilRandomizeWalkingSpeed = 5;
        public double MinRandomizeWalkingSpeedInKph = 7;
        public double MaxRandomizeWalkingSpeedInKph = 11;
        //mapzen api
        public bool RandomizeSpawnOnStart = true;
        public bool UseMapzenApiElevation = false;
        public string MapzenApiElevationKey = "";
    }

    public class CatchSettings
    {
        public bool CatchWildPokemon = true;

        //catch
        public bool HumanizeThrows = true;
        public double ThrowAccuracyMin = 0.80;
        public double ThrowAccuracyMax = 1.00;
        public double ThrowSpinFrequency = 0.80;
        public int MaxPokeballsPerPokemon = 6;
        public int UseGreatBallAboveIv = 80;
        public int UseUltraBallAboveIv = 90;
        public double UseGreatBallBelowCatchProbability = 0.35;
        public double UseUltraBallBelowCatchProbability = 0.2;
        public bool UsePokemonToNotCatchFilter = false;

        //berries
        public int UseBerryMinCp = 1000;
        public float UseBerryMinIv = 95;
        public double UseBerryBelowCatchProbability = 0.25;
    }

    public class RecycleSettings
    {
        //recycle
        public bool AutomaticInventoryManagement = true;
        public int AutomaticMaxAllPokeballs = 200;
        public int AutomaticMaxAllPotions = 10;
        public int AutomaticMaxAllRevives = 20;
        public int AutomaticMaxAllBerries = 50;
        public int TotalAmountOfPokeballsToKeep = 40;
        public int TotalAmountOfGreatballsToKeep = 80;
        public int TotalAmountOfUltraballsToKeep = 120;
        public int TotalAmountOfMasterballsToKeep = 100;
        public int TotalAmountOfPotionsToKeep = 3;
        public int TotalAmountOfSuperPotionsToKeep = 5;
        public int TotalAmountOfHyperPotionsToKeep = 10;
        public int TotalAmountOfMaxPotionsToKeep = 30;
        public int TotalAmountOfRevivesToKeep = 20;
        public int TotalAmountOfMaxRevivesToKeep = 40;
        public int TotalAmountOfRazzToKeep = 50;
        //public int TotalAmountOfBlukToKeep = 50;
        //public int TotalAmountOfNanabToKeep = 50;
        //public int TotalAmountOfPinapToKeep = 50;
        //public int TotalAmountOfWeparToKeep = 50;
        public double RecycleInventoryAtUsagePercentage = 0.85;
    }

    public class SnipeConfig
    {
        //snipe
        public bool SnipeAtPokestops = false;
        public bool SnipeIgnoreUnknownIv = false;
        public bool UseTransferIvForSnipe = false;
        public int MinPokeballsToSnipe = 20;
        public int MinPokeballsWhileSnipe = 5;
        public bool UseSnipeLocationServer = false;
        public bool UsePokeSnipersLocationServer = false;
        public string SnipeLocationServer = "localhost";
        public int SnipeLocationServerPort = 16969;
        public int SnipeRequestTimeoutSeconds = 10;
    }

    public class GlobalSettings
    {
        [JsonIgnore]
        internal AuthSettings Auth = new AuthSettings();
        [JsonIgnore]
        public string GeneralConfigPath;
        [JsonIgnore]
        public string ProfilePath;
        [JsonIgnore]
        public string ProfileConfigPath;

        public DeviceSettings DeviceSettings = new DeviceSettings();

        public StartUpSettings StartUpSettings = new StartUpSettings();

        public LocationSettings LocationSettings = new LocationSettings();

        public DelaySettings DelaySettings = new DelaySettings();

        public PokemonConfig PokemonSettings = new PokemonConfig();

        public CatchSettings CatchSettings = new CatchSettings();

        public RecycleSettings RecycleSettings = new RecycleSettings();

        public SnipeConfig SnipeSettings = new SnipeConfig();
        [JsonIgnore]
        public MapzenAPI MapzenAPI = new MapzenAPI();

        private static Random random = new Random();


        public List<KeyValuePair<ItemId, int>> ItemRecycleFilter = new List<KeyValuePair<ItemId, int>>
        {
            new KeyValuePair<ItemId, int>(ItemId.ItemUnknown, 0),
            new KeyValuePair<ItemId, int>(ItemId.ItemLuckyEgg, 200),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncenseOrdinary, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncenseSpicy, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncenseCool, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncenseFloral, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemTroyDisk, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemXAttack, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemXDefense, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemXMiracle, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemSpecialCamera, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncubatorBasicUnlimited, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncubatorBasic, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemPokemonStorageUpgrade, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemItemStorageUpgrade, 100)
        };

        public List<PokemonId> PokemonsNotToTransfer = new List<PokemonId>
        {
            //criteria: from SS Tier to A Tier + Regional Exclusive
            //PokemonId.Venusaur,
            //PokemonId.Charizard,
            //PokemonId.Blastoise,
            //PokemonId.Nidoqueen,
            //PokemonId.Nidoking,
            //PokemonId.Clefable,
            //PokemonId.Vileplume,
            //PokemonId.Golduck,
            //PokemonId.Arcanine,
            //PokemonId.Poliwrath,
            //PokemonId.Machamp,
            //PokemonId.Victreebel,
            //PokemonId.Golem,
            //PokemonId.Slowbro,
            //PokemonId.Farfetchd,
            //PokemonId.Muk,
            //PokemonId.Exeggutor,
            //PokemonId.Lickitung,
            //PokemonId.Chansey,
            //PokemonId.Kangaskhan,
            //PokemonId.MrMime,
            //PokemonId.Tauros,
            //PokemonId.Gyarados,
            //PokemonId.Lapras,
            PokemonId.Ditto,
            //PokemonId.Vaporeon,
            //PokemonId.Jolteon,
            //PokemonId.Flareon,
            //PokemonId.Porygon,
            //PokemonId.Snorlax,
            PokemonId.Articuno,
            PokemonId.Zapdos,
            PokemonId.Moltres,
            //PokemonId.Dragonite,
            PokemonId.Mewtwo,
            PokemonId.Mew
        };

        public List<PokemonId> PokemonsToEvolve = new List<PokemonId>
        {
            /*NOTE: keep all the end-of-line commas exept for the last one or an exception will be thrown!
            criteria: 12 candies*/
            PokemonId.Caterpie,
            PokemonId.Weedle,
            PokemonId.Pidgey,
            /*criteria: 25 candies*/
            //PokemonId.Bulbasaur,
            //PokemonId.Charmander,
            //PokemonId.Squirtle,
            PokemonId.Rattata,
            //PokemonId.NidoranFemale,
            //PokemonId.NidoranMale,
            //PokemonId.Oddish,
            //PokemonId.Poliwag,
            //PokemonId.Abra,
            //PokemonId.Machop,
            //PokemonId.Bellsprout,
            //PokemonId.Geodude,
            //PokemonId.Gastly,
            //PokemonId.Eevee,
            //PokemonId.Dratini,
            /*criteria: 50 candies commons*/
            PokemonId.Spearow,
            PokemonId.Ekans,
            PokemonId.Zubat,
            //PokemonId.Paras,
            //PokemonId.Venonat,
            //PokemonId.Psyduck,
            //PokemonId.Slowpoke,
            PokemonId.Doduo,
            //PokemonId.Drowzee,
            //PokemonId.Krabby,
            //PokemonId.Horsea,
            //PokemonId.Goldeen,
            //PokemonId.Staryu
            PokemonId.Pikachu,
            PokemonId.Sandshrew,
            PokemonId.Clefairy,
            PokemonId.Vulpix,
            PokemonId.Jigglypuff,
            PokemonId.Zubat,
            PokemonId.Oddish,
            PokemonId.Paras,
            PokemonId.Venonat,
            PokemonId.Diglett,
            PokemonId.Meowth,
            PokemonId.Psyduck,
            PokemonId.Mankey,
            PokemonId.Poliwag,
            PokemonId.Abra,
            PokemonId.Machop,
            PokemonId.Bellsprout,
            PokemonId.Tentacool,
            PokemonId.Geodude,
            PokemonId.Ponyta,
            PokemonId.Slowpoke,
            PokemonId.Magnemite,
            PokemonId.Doduo,
            PokemonId.Seel,
            PokemonId.Grimer,
            PokemonId.Shellder,
            PokemonId.Gastly,
            PokemonId.Drowzee,
            PokemonId.Krabby,
            PokemonId.Voltorb,
            PokemonId.Cubone,
            PokemonId.Koffing,
            PokemonId.Horsea,
            PokemonId.Goldeen,
            PokemonId.Staryu
        };

        public List<PokemonId> PokemonsToIgnore = new List<PokemonId>
        {
            //criteria: most common
            PokemonId.Caterpie,
            PokemonId.Weedle,
            PokemonId.Pidgey,
            PokemonId.Rattata,
            PokemonId.Spearow,
            PokemonId.Zubat,
            PokemonId.Doduo
        };

        public Dictionary<PokemonId, TransferFilter> PokemonsTransferFilter = new Dictionary<PokemonId, TransferFilter>
        {
            //criteria: based on NY Central Park and Tokyo variety + sniping optimization v4.1
            {PokemonId.Venusaur, new TransferFilter(1750, 80, 1)},
            {PokemonId.Charizard, new TransferFilter(1750, 20, 1)},
            {PokemonId.Blastoise, new TransferFilter(1750, 50, 1)},
            {PokemonId.Nidoqueen, new TransferFilter(1750, 80, 1)},
            {PokemonId.Nidoking, new TransferFilter(1750, 80, 1)},
            {PokemonId.Clefable, new TransferFilter(1500, 60, 1)},
            {PokemonId.Vileplume, new TransferFilter(1750, 80, 1)},
            {PokemonId.Golduck, new TransferFilter(1750, 80, 1)},
            {PokemonId.Arcanine, new TransferFilter(2250, 90, 1)},
            {PokemonId.Poliwrath, new TransferFilter(1750, 80, 1)},
            {PokemonId.Machamp, new TransferFilter(1250, 80, 1)},
            {PokemonId.Victreebel, new TransferFilter(1250, 60, 1)},
            {PokemonId.Golem, new TransferFilter(1500, 80, 1)},
            {PokemonId.Slowbro, new TransferFilter(1750, 90, 1)},
            {PokemonId.Farfetchd, new TransferFilter(1250, 90, 1)},
            {PokemonId.Muk, new TransferFilter(2000, 80, 1)},
            {PokemonId.Exeggutor, new TransferFilter(2250, 80, 1)},
            {PokemonId.Lickitung, new TransferFilter(1500, 80, 1)},
            {PokemonId.Chansey, new TransferFilter(1500, 95, 1)},
            {PokemonId.Kangaskhan, new TransferFilter(1500, 60, 1)},
            {PokemonId.MrMime, new TransferFilter(1250, 80, 1)},
            {PokemonId.Scyther, new TransferFilter(1750, 90, 1)},
            {PokemonId.Jynx, new TransferFilter(1250, 90, 1)},
            {PokemonId.Electabuzz, new TransferFilter(1500, 80, 1)},
            {PokemonId.Magmar, new TransferFilter(1750, 90, 1)},
            {PokemonId.Pinsir, new TransferFilter(2000, 98, 1)},
            {PokemonId.Tauros, new TransferFilter(500, 90, 1)},
            {PokemonId.Gyarados, new TransferFilter(2000, 90, 1)},
            {PokemonId.Lapras, new TransferFilter(2250, 90, 1)},
            {PokemonId.Eevee, new TransferFilter(1500, 98, 1)},
            {PokemonId.Vaporeon, new TransferFilter(2250, 98, 1)},
            {PokemonId.Jolteon, new TransferFilter(2250, 95, 1)},
            {PokemonId.Flareon, new TransferFilter(2250, 95, 1)},
            {PokemonId.Porygon, new TransferFilter(1500, 95, 1)},
            {PokemonId.Aerodactyl, new TransferFilter(2000, 95, 1)},
            {PokemonId.Snorlax, new TransferFilter(2750, 96, 1)},
            {PokemonId.Dragonite, new TransferFilter(2750, 90, 1)}
        };

        public SnipeSettings PokemonToSnipe = new SnipeSettings
        {
            Locations = new List<Location>
            {
                new Location(38.55680748646112, -121.2383794784546), //Dratini Spot
                new Location(-33.85901900, 151.21309800), //Magikarp Spot
                new Location(47.5014969, -122.0959568), //Eevee Spot
                new Location(51.5025343, -0.2055027) //Charmender Spot
            },
            Pokemon = new List<PokemonId>
            {
                PokemonId.Bulbasaur,
                PokemonId.Ivysaur,
                PokemonId.Venusaur,
                PokemonId.Charmander,
                PokemonId.Charmeleon,
                PokemonId.Charizard,
                PokemonId.Squirtle,
                PokemonId.Wartortle,
                PokemonId.Blastoise,
                PokemonId.Butterfree,
                PokemonId.Beedrill,
                PokemonId.Pidgeot,
                PokemonId.Raticate,
                PokemonId.Fearow,
                PokemonId.Arbok,
                PokemonId.Pikachu,
                PokemonId.Raichu,
                PokemonId.Sandslash,
                PokemonId.Nidoqueen,
                PokemonId.Nidoking,
                PokemonId.Clefable,
                PokemonId.Ninetales,
                PokemonId.Wigglytuff,
                PokemonId.Golbat,
                PokemonId.Vileplume,
                PokemonId.Parasect,
                PokemonId.Venomoth,
                PokemonId.Dugtrio,
                PokemonId.Persian,
                PokemonId.Golduck,
                PokemonId.Primeape,
                PokemonId.Growlithe,
                PokemonId.Arcanine,
                PokemonId.Poliwag,
                PokemonId.Poliwhirl,
                PokemonId.Poliwrath,
                PokemonId.Abra,
                PokemonId.Kadabra,
                PokemonId.Alakazam,
                PokemonId.Machop,
                PokemonId.Machoke,
                PokemonId.Machamp,
                PokemonId.Victreebel,
                PokemonId.Tentacruel,
                PokemonId.Golem,
                PokemonId.Rapidash,
                PokemonId.Slowbro,
                PokemonId.Magneton,
                PokemonId.Farfetchd,
                PokemonId.Dodrio,
                PokemonId.Dewgong,
                PokemonId.Grimer,
                PokemonId.Muk,
                PokemonId.Cloyster,
                PokemonId.Gastly,
                PokemonId.Haunter,
                PokemonId.Gengar,
                PokemonId.Onix,
                PokemonId.Hypno,
                PokemonId.Kingler,
                PokemonId.Electrode,
                PokemonId.Exeggutor,
                PokemonId.Marowak,
                PokemonId.Hitmonlee,
                PokemonId.Hitmonchan,
                PokemonId.Lickitung,
                PokemonId.Koffing,
                PokemonId.Weezing,
                PokemonId.Rhyhorn,
                PokemonId.Rhydon,
                PokemonId.Chansey,
                PokemonId.Tangela,
                PokemonId.Kangaskhan,
                PokemonId.Seadra,
                PokemonId.Seaking,
                PokemonId.Starmie,
                PokemonId.MrMime,
                PokemonId.Scyther,
                PokemonId.Jynx,
                PokemonId.Electabuzz,
                PokemonId.Magmar,
                PokemonId.Pinsir,
                PokemonId.Tauros,
                PokemonId.Magikarp,
                PokemonId.Gyarados,
                PokemonId.Lapras,
                PokemonId.Ditto,
                PokemonId.Eevee,
                PokemonId.Vaporeon,
                PokemonId.Jolteon,
                PokemonId.Flareon,
                PokemonId.Porygon,
                PokemonId.Omanyte,
                PokemonId.Omastar,
                PokemonId.Kabuto,
                PokemonId.Kabutops,
                PokemonId.Aerodactyl,
                PokemonId.Snorlax,
                PokemonId.Articuno,
                PokemonId.Zapdos,
                PokemonId.Moltres,
                PokemonId.Dratini,
                PokemonId.Dragonair,
                PokemonId.Dragonite,
                PokemonId.Mewtwo,
                PokemonId.Mew
            }
        };

        public List<PokemonId> PokemonToUseMasterball = new List<PokemonId>
        {
            PokemonId.Articuno,
            PokemonId.Zapdos,
            PokemonId.Moltres,
            PokemonId.Mew,
            PokemonId.Mewtwo
        };

        public static GlobalSettings Default => new GlobalSettings();

        public static GlobalSettings Load(string path)
        {
            GlobalSettings settings;
            var profilePath = Path.Combine(Directory.GetCurrentDirectory(), path);
            var profileConfigPath = Path.Combine(profilePath, "config");
            var configFile = Path.Combine(profileConfigPath, "config.json");

            if (File.Exists(configFile))
            {
                try
                {
                    //if the file exists, load the settings
                    var input = File.ReadAllText(configFile);

                    var jsonSettings = new JsonSerializerSettings();
                    jsonSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                    jsonSettings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    jsonSettings.DefaultValueHandling = DefaultValueHandling.Populate;

                    settings = JsonConvert.DeserializeObject<GlobalSettings>(input, jsonSettings);
                    if (settings.DelaySettings.ReRandomizeDelayOnStart)
                    {
                        settings.DelaySettings.DelayBetweenPlayerActions = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                        settings.DelaySettings.DelayPositionCheckState = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                        settings.DelaySettings.DelayPokestop = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                        settings.DelaySettings.DelayCatchPokemon = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                        settings.DelaySettings.DelayBetweenPokemonCatch = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                        settings.DelaySettings.DelayCatchNearbyPokemon = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                        settings.DelaySettings.DelayCatchLurePokemon = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                        settings.DelaySettings.DelayCatchIncensePokemon = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                        //settings.DelaySettings.DelayEvolvePokemon = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds); unused since we know how long it takes to evovle a pokemon
                        settings.DelaySettings.DelayTransferPokemon = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                        settings.DelaySettings.DelayDisplayPokemon = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                        settings.DelaySettings.DelayUseLuckyEgg = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                        settings.DelaySettings.DelaySoftbanRetry = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                        settings.DelaySettings.DelayRecyleItem = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                        settings.DelaySettings.DelaySnipePokemon = random.Next(settings.DelaySettings.MinRandomizeDelayMilliseconds, settings.DelaySettings.MaxRandomizeDelayMilliseconds);
                    }
                    if (settings.LocationSettings.UseMapzenApiElevation)
                    {
                        string possiblePath = Path.Combine(Directory.GetCurrentDirectory(), "MapzenAPI/" + settings.LocationSettings.DefaultLatitude.ToString().Replace(".", ",") + "_" + settings.LocationSettings.DefaultLongitude.ToString().Replace(".", ",") + ".json");
                        if (settings.MapzenAPI.checkForExistingAltitude(possiblePath))
                        {
                            settings.LocationSettings.DefaultAltitude = settings.MapzenAPI.getExistingAltitude(possiblePath);
                        }
                        else if (!settings.LocationSettings.MapzenApiElevationKey.Equals(""))
                        {
                            settings.LocationSettings.DefaultAltitude = settings.MapzenAPI.getAltitude(settings.LocationSettings.DefaultLatitude,
                                                                                                    settings.LocationSettings.DefaultLongitude,
                                                                                                    settings.LocationSettings.MapzenApiElevationKey);
                        }
                        else
                        {
                            Logger.Write("No MapzenAPIElevationKey? (Check LocationSettings)");
                        }
                    }
                }
                catch (JsonReaderException exception)
                {
                    Logger.Write("JSON Exception: " + exception.Message, LogLevel.Error);
                    return null;
                }
            }
            else
            {
                settings = new GlobalSettings();
            }

            if (settings.StartUpSettings.WebSocketPort == 0)
            {
                settings.StartUpSettings.WebSocketPort = 14251;
            }

            if (settings.PokemonToSnipe == null)
            {
                settings.PokemonToSnipe = Default.PokemonToSnipe;
            }

            if (settings.PokemonSettings.RenameTemplate == null)
            {
                settings.PokemonSettings.RenameTemplate = Default.PokemonSettings.RenameTemplate;
            }

            if (settings.SnipeSettings.SnipeLocationServer == null)
            {
                settings.SnipeSettings.SnipeLocationServer = Default.SnipeSettings.SnipeLocationServer;
            }

            settings.ProfilePath = profilePath;
            settings.ProfileConfigPath = profileConfigPath;
            settings.GeneralConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config");

            var firstRun = !File.Exists(configFile);

            settings.Save(configFile);
            settings.Auth.Load(Path.Combine(profileConfigPath, "auth.json"));

            if (firstRun)
            {
                return null;
            }

            return settings;
        }

        public void Save(string fullPath)
        {
            var output = JsonConvert.SerializeObject(this, Formatting.Indented,
                new StringEnumConverter { CamelCaseText = true });

            var folder = Path.GetDirectoryName(fullPath);
            if (folder != null && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllText(fullPath, output);
        }
    }

    public class ClientSettings : ISettings
    {
        // Never spawn at the same position.
        private readonly Random _rand = new Random();
        private readonly GlobalSettings _settings;

        public ClientSettings(GlobalSettings settings)
        {
            _settings = settings;
        }


        public string GoogleUsername => _settings.Auth.GoogleUsername;
        public string GooglePassword => _settings.Auth.GooglePassword;

        public string GoogleRefreshToken
        {
            get { return null; }
            set { GoogleRefreshToken = null; }
        }

        AuthType ISettings.AuthType
        {
            get { return _settings.Auth.AuthType; }

            set { _settings.Auth.AuthType = value; }
        }

        double ISettings.DefaultLatitude
        {
            get
            {
                if (_settings.LocationSettings.RandomizeSpawnOnStart)
                {
                    return _settings.LocationSettings.DefaultLatitude + _rand.NextDouble() * ((double)_settings.LocationSettings.MaxSpawnLocationOffset / 111111);
                }
                else
                {
                    return _settings.LocationSettings.DefaultLatitude;
                }
            }

            set { _settings.LocationSettings.DefaultLatitude = value; }
        }

        double ISettings.DefaultLongitude
        {
            get
            {
                if (_settings.LocationSettings.RandomizeSpawnOnStart)
                {
                    return _settings.LocationSettings.DefaultLongitude +
                       _rand.NextDouble() *
                       ((double)_settings.LocationSettings.MaxSpawnLocationOffset / 111111 / Math.Cos(_settings.LocationSettings.DefaultLatitude));
                }
                else
                {
                    return _settings.LocationSettings.DefaultLongitude;
                }
            }

            set { _settings.LocationSettings.DefaultLongitude = value; }
        }

        double ISettings.DefaultAltitude
        {
            get { return _settings.LocationSettings.DefaultAltitude; }

            set { _settings.LocationSettings.DefaultAltitude = value; }
        }

        string ISettings.PtcPassword
        {
            get { return _settings.Auth.PtcPassword; }

            set { _settings.Auth.PtcPassword = value; }
        }

        string ISettings.PtcUsername
        {
            get { return _settings.Auth.PtcUsername; }

            set { _settings.Auth.PtcUsername = value; }
        }

        string ISettings.GoogleUsername
        {
            get { return _settings.Auth.GoogleUsername; }

            set { _settings.Auth.GoogleUsername = value; }
        }

        string ISettings.GooglePassword
        {
            get { return _settings.Auth.GooglePassword; }

            set { _settings.Auth.GooglePassword = value; }
        }

        //string DevicePackageName
        //{
        //    get { return _settings.de; }
        //    set { _settings.DevicePackageName = value; }
        //}
        string ISettings.DeviceId
        {
            get { return _settings.DeviceSettings.DeviceId; }
            set { _settings.DeviceSettings.DeviceId = value; }
        }
        string ISettings.AndroidBoardName
        {
            get { return _settings.DeviceSettings.AndroidBoardName; }
            set { _settings.DeviceSettings.AndroidBoardName = value; }
        }
        string ISettings.AndroidBootloader
        {
            get { return _settings.DeviceSettings.AndroidBootLoader; }
            set { _settings.DeviceSettings.AndroidBootLoader = value; }
        }
        string ISettings.DeviceBrand
        {
            get { return _settings.DeviceSettings.DeviceBrand; }
            set { _settings.DeviceSettings.DeviceBrand = value; }
        }
        string ISettings.DeviceModel
        {
            get { return _settings.DeviceSettings.DeviceModel; }
            set { _settings.DeviceSettings.DeviceModel = value; }
        }
        string ISettings.DeviceModelIdentifier
        {
            get { return _settings.DeviceSettings.DeviceModelIdentifier; }
            set { _settings.DeviceSettings.DeviceModelIdentifier = value; }
        }
        string ISettings.DeviceModelBoot
        {
            get { return _settings.DeviceSettings.DeviceModelBoot; }
            set { _settings.DeviceSettings.DeviceModelBoot = value; }
        }
        string ISettings.HardwareManufacturer
        {
            get { return _settings.DeviceSettings.HardwareManufacturer; }
            set { _settings.DeviceSettings.HardwareManufacturer = value; }
        }
        string ISettings.HardwareModel
        {
            get { return _settings.DeviceSettings.HardWareModel; }
            set { _settings.DeviceSettings.HardWareModel = value; }
        }
        string ISettings.FirmwareBrand
        {
            get { return _settings.DeviceSettings.FirmwareBrand; }
            set { _settings.DeviceSettings.FirmwareBrand = value; }
        }
        string ISettings.FirmwareTags
        {
            get { return _settings.DeviceSettings.FirmwareTags; }
            set { _settings.DeviceSettings.FirmwareTags = value; }
        }
        string ISettings.FirmwareType
        {
            get { return _settings.DeviceSettings.FirmwareType; }
            set { _settings.DeviceSettings.FirmwareType = value; }
        }
        string ISettings.FirmwareFingerprint
        {
            get { return _settings.DeviceSettings.FirmwareFingerprint; }
            set { _settings.DeviceSettings.FirmwareFingerprint = value; }
        }

        bool ISettings.UseProxy
        {
            get { return _settings.Auth.UseProxy; }
            set { _settings.Auth.UseProxy = value; }
        }
        string ISettings.ProxyLogin
        {
            get { return _settings.Auth.ProxyLogin; }
            set { _settings.Auth.ProxyLogin = value; }
        }
        string ISettings.ProxyPass
        {
            get { return _settings.Auth.ProxyPass; }
            set { _settings.Auth.ProxyPass = value; }
        }
        string ISettings.ProxyUri
        {
            get { return _settings.Auth.ProxyUri; }
            set { _settings.Auth.ProxyUri = value; }
        }
    }

    public class LogicSettings : ILogicSettings
    {
        private readonly GlobalSettings _settings;

        public LogicSettings(GlobalSettings settings)
        {
            _settings = settings;
        }

        public string ProfilePath => _settings.ProfilePath;
        public string ProfileConfigPath => _settings.ProfileConfigPath;
        public int SnipeRequestTimeoutSeconds => _settings.SnipeSettings.SnipeRequestTimeoutSeconds * 1000;
        public string GeneralConfigPath => _settings.GeneralConfigPath;
        public bool AutoUpdate => _settings.StartUpSettings.AutoUpdate;
        public bool TransferConfigAndAuthOnUpdate => _settings.StartUpSettings.TransferConfigAndAuthOnUpdate;
        public float KeepMinIvPercentage => _settings.PokemonSettings.KeepMinIvPercentage;
        public int KeepMinCp => _settings.PokemonSettings.KeepMinCp;
        public bool AutomaticallyLevelUpPokemon => _settings.PokemonSettings.AutomaticallyLevelUpPokemon;
        public string LevelUpByCPorIv => _settings.PokemonSettings.LevelUpByCPorIv;
        public float UpgradePokemonIvMinimum => _settings.PokemonSettings.UpgradePokemonIvMinimum;
        public float UpgradePokemonCpMinimum => _settings.PokemonSettings.UpgradePokemonCpMinimum;
        public double WalkingSpeedInKilometerPerHour => _settings.LocationSettings.WalkingSpeedInKilometerPerHour;
        public bool EvolveAllPokemonWithEnoughCandy => _settings.PokemonSettings.EvolveAllPokemonWithEnoughCandy;
        public bool KeepPokemonsThatCanEvolve => _settings.PokemonSettings.KeepPokemonsThatCanEvolve;
        public bool TransferDuplicatePokemon => _settings.PokemonSettings.TransferDuplicatePokemon;
        public bool UseEggIncubators => _settings.PokemonSettings.UseEggIncubators;
        public int UseGreatBallAboveIv => _settings.CatchSettings.UseGreatBallAboveIv;
        public int UseUltraBallAboveIv => _settings.CatchSettings.UseUltraBallAboveIv;
        public double UseUltraBallBelowCatchProbability => _settings.CatchSettings.UseUltraBallBelowCatchProbability;
        public double UseGreatBallBelowCatchProbability => _settings.CatchSettings.UseGreatBallBelowCatchProbability;
        public int DelayBetweenPokemonCatch => _settings.DelaySettings.DelayBetweenPokemonCatch;
        public int DelayBetweenPlayerActions => _settings.DelaySettings.DelayBetweenPlayerActions;
        public bool UsePokemonToNotCatchFilter => _settings.CatchSettings.UsePokemonToNotCatchFilter;
        public int KeepMinDuplicatePokemon => _settings.PokemonSettings.KeepMinDuplicatePokemon;
        public bool PrioritizeIvOverCp => _settings.PokemonSettings.PrioritizeIvOverCp;
        public int MaxTravelDistanceInMeters => _settings.LocationSettings.MaxTravelDistanceInMeters;
        public string GpxFile => _settings.LocationSettings.GpxFile;
        public bool UseGpxPathing => _settings.LocationSettings.UseGpxPathing;
        public bool UsePokeStopLuckyNumber => _settings.LocationSettings.UsePokeStopLuckyNumber;
        public int PokestopSkipLuckyNumberMinUse => _settings.LocationSettings.PokestopSkipLuckyNumberMinUse;
        public int PokestopSkipLuckyNumber => _settings.LocationSettings.PokestopSkipLuckyNumber;
        public int PokestopSkipLuckyMin => _settings.LocationSettings.PokestopSkipLuckyMin;
        public int PokestopSkipLuckyMax => _settings.LocationSettings.PokestopSkipLuckyMax;
        public bool UseLuckyEggsWhileEvolving => _settings.PokemonSettings.UseLuckyEggsWhileEvolving;
        public int UseLuckyEggsMinPokemonAmount => _settings.PokemonSettings.UseLuckyEggsMinPokemonAmount;
        public bool EvolveAllPokemonAboveIv => _settings.PokemonSettings.EvolveAllPokemonAboveIv;
        public float EvolveAboveIvValue => _settings.PokemonSettings.EvolveAboveIvValue;
        public bool RenamePokemon => _settings.PokemonSettings.RenamePokemon;
        public bool RenameOnlyAboveIv => _settings.PokemonSettings.RenameOnlyAboveIv;
        public float FavoriteMinIvPercentage => _settings.PokemonSettings.FavoriteMinIvPercentage;
        public bool AutoFavoritePokemon => _settings.PokemonSettings.AutoFavoritePokemon;
        public string RenameTemplate => _settings.PokemonSettings.RenameTemplate;
        public int AmountOfPokemonToDisplayOnStartCp => _settings.StartUpSettings.AmountOfPokemonToDisplayOnStartCp;
        public int AmountOfPokemonToDisplayOnStartIv => _settings.StartUpSettings.AmountOfPokemonToDisplayOnStartIv;
        public bool DisplayPokemonMaxPoweredCp => _settings.StartUpSettings.DisplayPokemonMaxPoweredCp;
        public bool DisplayPokemonMovesetRank => _settings.StartUpSettings.DisplayPokemonMovesetRank;
        public bool DumpPokemonStats => _settings.StartUpSettings.DumpPokemonStats;
        public string TranslationLanguageCode => _settings.StartUpSettings.TranslationLanguageCode;
        public ICollection<KeyValuePair<ItemId, int>> ItemRecycleFilter => _settings.ItemRecycleFilter;
        public ICollection<PokemonId> PokemonsToEvolve => _settings.PokemonsToEvolve;
        public ICollection<PokemonId> PokemonsNotToTransfer => _settings.PokemonsNotToTransfer;
        public ICollection<PokemonId> PokemonsNotToCatch => _settings.PokemonsToIgnore;
        public ICollection<PokemonId> PokemonToUseMasterball => _settings.PokemonToUseMasterball;
        public Dictionary<PokemonId, TransferFilter> PokemonsTransferFilter => _settings.PokemonsTransferFilter;
        public bool StartupWelcomeDelay => _settings.StartUpSettings.StartupWelcomeDelay;
        public bool SnipeAtPokestops => _settings.SnipeSettings.SnipeAtPokestops;
        public int MinPokeballsToSnipe => _settings.SnipeSettings.MinPokeballsToSnipe;
        public int MinPokeballsWhileSnipe => _settings.SnipeSettings.MinPokeballsWhileSnipe;
        public int MaxPokeballsPerPokemon => _settings.CatchSettings.MaxPokeballsPerPokemon;
        public SnipeSettings PokemonToSnipe => _settings.PokemonToSnipe;
        public string SnipeLocationServer => _settings.SnipeSettings.SnipeLocationServer;
        public int SnipeLocationServerPort => _settings.SnipeSettings.SnipeLocationServerPort;
        public bool UseSnipeLocationServer => _settings.SnipeSettings.UseSnipeLocationServer;
        public bool UsePokeSnipersLocationServer => _settings.SnipeSettings.UsePokeSnipersLocationServer;
        public bool UseTransferIvForSnipe => _settings.SnipeSettings.UseTransferIvForSnipe;
        public bool SnipeIgnoreUnknownIv => _settings.SnipeSettings.SnipeIgnoreUnknownIv;
        public int MinDelayBetweenSnipes => _settings.DelaySettings.MinDelayBetweenSnipes;
        public double SnipingScanOffset => _settings.DelaySettings.SnipingScanOffset;
        public bool AutomaticInventoryManagement => _settings.RecycleSettings.AutomaticInventoryManagement;
        public int AutomaticMaxAllPokeballs => _settings.RecycleSettings.AutomaticMaxAllPokeballs;
        public int AutomaticMaxAllPotions => _settings.RecycleSettings.AutomaticMaxAllPotions;
        public int AutomaticMaxAllRevives => _settings.RecycleSettings.AutomaticMaxAllRevives;
        public int AutomaticMaxAllBerries => _settings.RecycleSettings.AutomaticMaxAllBerries;
        public int TotalAmountOfPokeballsToKeep => _settings.RecycleSettings.TotalAmountOfPokeballsToKeep;
        public int TotalAmountOfGreatballsToKeep => _settings.RecycleSettings.TotalAmountOfGreatballsToKeep;
        public int TotalAmountOfUltraballsToKeep => _settings.RecycleSettings.TotalAmountOfUltraballsToKeep;
        public int TotalAmountOfMasterballsToKeep => _settings.RecycleSettings.TotalAmountOfMasterballsToKeep;
        public int TotalAmountOfRazzToKeep => _settings.RecycleSettings.TotalAmountOfRazzToKeep;
        //public int TotalAmountOfBlukToKeep => _settings.RecycleSettings.TotalAmountOfBlukToKeep;
        //public int TotalAmountOfNanabToKeep => _settings.RecycleSettings.TotalAmountOfNanabToKeep;
        //public int TotalAmountOfPinapToKeep => _settings.RecycleSettings.TotalAmountOfPinapToKeep;
        //public int TotalAmountOfWeparToKeep => _settings.RecycleSettings.TotalAmountOfWeparToKeep;
        public int TotalAmountOfPotionsToKeep => _settings.RecycleSettings.TotalAmountOfPotionsToKeep;
        public int TotalAmountOfSuperPotionsToKeep => _settings.RecycleSettings.TotalAmountOfSuperPotionsToKeep;
        public int TotalAmountOfHyperPotionsToKeep => _settings.RecycleSettings.TotalAmountOfHyperPotionsToKeep;
        public int TotalAmountOfMaxPotionsToKeep => _settings.RecycleSettings.TotalAmountOfMaxPotionsToKeep;
        public int TotalAmountOfRevivesToKeep => _settings.RecycleSettings.TotalAmountOfRevivesToKeep;
        public int TotalAmountOfMaxRevivesToKeep => _settings.RecycleSettings.TotalAmountOfRevivesToKeep;
        public bool Teleport => _settings.LocationSettings.Teleport;
        public int DelayCatchIncensePokemon => _settings.DelaySettings.DelayCatchIncensePokemon;
        public int DelayCatchNearbyPokemon => _settings.DelaySettings.DelayCatchNearbyPokemon;
        public int DelayPositionCheckState => _settings.DelaySettings.DelayPositionCheckState;
        public int DelayCatchLurePokemon => _settings.DelaySettings.DelayCatchLurePokemon;
        public int DelayCatchPokemon => _settings.DelaySettings.DelayCatchPokemon;
        public int DelayDisplayPokemon => _settings.DelaySettings.DelayDisplayPokemon;
        public int DelayUseLuckyEgg => _settings.DelaySettings.DelayUseLuckyEgg;
        public int DelaySoftbanRetry => _settings.DelaySettings.DelaySoftbanRetry;
        public int DelayPokestop => _settings.DelaySettings.DelayPokestop;
        public int DelayRecyleItem => _settings.DelaySettings.DelayRecyleItem;
        public int DelaySnipePokemon => _settings.DelaySettings.DelaySnipePokemon;
        public int DelayTransferPokemon => _settings.DelaySettings.DelayTransferPokemon;
        public int DelayEvolvePokemon => _settings.DelaySettings.DelayEvolvePokemon;
        public double DelayEvolveVariation => _settings.DelaySettings.DelayEvolveVariation;
        public double RecycleInventoryAtUsagePercentage => _settings.RecycleSettings.RecycleInventoryAtUsagePercentage;
        public bool HumanizeThrows => _settings.CatchSettings.HumanizeThrows;
        public double ThrowAccuracyMin => _settings.CatchSettings.ThrowAccuracyMin;
        public double ThrowAccuracyMax => _settings.CatchSettings.ThrowAccuracyMax;
        public double ThrowSpinFrequency => _settings.CatchSettings.ThrowSpinFrequency;
        public int UseBerryMinCp => _settings.CatchSettings.UseBerryMinCp;
        public float UseBerryMinIv => _settings.CatchSettings.UseBerryMinIv;
        public double UseBerryBelowCatchProbability => _settings.CatchSettings.UseBerryBelowCatchProbability;

        public bool CatchWildPokemon => _settings.CatchSettings.CatchWildPokemon;

        public bool UseHumanPathing => _settings.StartUpSettings.UseHumanPathing;

        public bool RandomizeSpawnOnStart => _settings.LocationSettings.RandomizeSpawnOnStart;
        public bool RandomizeWalkingSpeed => _settings.LocationSettings.RandomizeWalkingSpeed;
        public bool UseMapzenApiElevation => _settings.LocationSettings.UseMapzenApiElevation;
        public string MapzenApiElevationKey => _settings.LocationSettings.MapzenApiElevationKey;
        public int MinutesUntilRandomizeWalkingSpeed => _settings.LocationSettings.MinutesUntilRandomizeWalkingSpeed;
        public double MaxRandomizeWalkingSpeedInKph => _settings.LocationSettings.MaxRandomizeWalkingSpeedInKph;
        public double MinRandomizeWalkingSpeedInKph => _settings.LocationSettings.MinRandomizeWalkingSpeedInKph;
        public bool PrioritizeBothIvAndCpForTransfer => _settings.PokemonSettings.PrioritizeBothIvAndCpForTransfer;
        public int MinRandomizeDelayMilliseconds => _settings.DelaySettings.MinRandomizeDelayMilliseconds;
        public int MaxRandomizeDelayMilliseconds => _settings.DelaySettings.MaxRandomizeDelayMilliseconds;
        public bool ReRandomizeDelayOnStart => _settings.DelaySettings.ReRandomizeDelayOnStart;

        public bool StopBotToAvoidBanOnUnknownLoginError => _settings.StartUpSettings.StopBotToAvoidBanOnUnknownLoginError;

    }


}