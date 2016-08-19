﻿using System;
using System.Net;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.HttpClient;
using POGOProtos.Networking.Envelopes;

namespace PokemonGo.RocketAPI
{
    public class Client
    {
        public Rpc.Login Login;
        public Rpc.Player Player;
        public Rpc.Download Download;
        public Rpc.Inventory Inventory;
        public Rpc.Map Map;
        public Rpc.Fort Fort;
        public Rpc.Encounter Encounter;
        public Rpc.Misc Misc;
        public Random rnd = new Random();

        public IApiFailureStrategy ApiFailure { get; set; }
        public ISettings Settings { get; }
        public string AuthToken { get; set; }
        private IWebProxy proxy
        {
            get
            {
                if (Settings.UseProxy)
                {
                    NetworkCredential proxyCreds = new NetworkCredential(
                        Settings.ProxyLogin,
                        Settings.ProxyPass
                    );
                    WebProxy prox = new WebProxy(Settings.ProxyUri)
                    {
                        UseDefaultCredentials = false,
                        Credentials = proxyCreds,
                    };
                    return prox;
                }
                return null;
            }
        }

        public double CurrentLatitude { get; internal set; }
        public double CurrentLongitude { get; internal set; }
        public double CurrentAltitude { get; internal set; }

        public AuthType AuthType => Settings.AuthType;

        internal PokemonHttpClient PokemonHttpClient
        {
            get
            {
                return new PokemonHttpClient(proxy);
            }
        }
        internal string ApiUrl { get; set; }
        internal AuthTicket AuthTicket { get; set; }

        public Client(ISettings settings, IApiFailureStrategy apiFailureStrategy)
        {
            Settings = settings;
            ApiFailure = apiFailureStrategy;


            Login = new Rpc.Login(this);
            Player = new Rpc.Player(this);
            Download = new Rpc.Download(this);
            Inventory = new Rpc.Inventory(this);
            Map = new Rpc.Map(this);
            Fort = new Rpc.Fort(this);
            Encounter = new Rpc.Encounter(this);
            Misc = new Rpc.Misc(this);

            Player.SetCoordinates(Settings.DefaultLatitude, Settings.DefaultLongitude, Settings.DefaultAltitude);
        }
    }
}