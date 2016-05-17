using CertiPay.Common.Logging;
using StackExchange.Redis;
using System;
using System.Configuration;

namespace CertiPay.Common.Redis
{
    /// <summary>
    /// A base representing
    /// </summary>
    public class RedisConnection
    {
        private static readonly ILog Log = LogManager.GetLogger<RedisConnection>();

        // Note: I'm writing this for redis without clustering

        private readonly Lazy<ConnectionMultiplexer> _connectionManager;

        public String Host { get; private set; }

        public int Port { get; private set; }

        public int DefaultDb { get; private set; }

        public RedisConnection(string host = "localhost", int port = 6379, int defaultDb = 0, string password = null)
        {
            this.Host = host;
            this.Port = port;

            this.DefaultDb = defaultDb;

            var options = new ConfigurationOptions
            {
                Password = password,
                AbortOnConnectFail = false,
                ConnectRetry = 5,
                ConnectTimeout = 1000,
                KeepAlive = 10,
                AllowAdmin = false
            };

            options.EndPoints.Add(this.Host, this.Port);

            Log.Debug("Configured RedisConnection for {host}:{port} db {defaultDb}", host, port, defaultDb);

            this._connectionManager = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public IDatabase GetClient(int? db = null)
        {
            return _connectionManager.Value.GetDatabase(db ?? this.DefaultDb);
        }

        public ISubscriber GetSubscriber()
        {
            return _connectionManager.Value.GetSubscriber();
        }

        public IServer GetServer()
        {
            return _connectionManager.Value.GetServer(this.Host, this.Port);
        }

        public static RedisConnection FromConfig()
        {
            /*
                Pull the settings from the config file

                <add key="Redis.Enabled" value="true" />
                <add key="Redis.Host" value="cpotest.certipay.com" />
                <add key="Redis.Port" value="6379" />
                <add key="Redis.Db" value="1" />
            */

            return new RedisConnection(
                host: ConfigurationManager.AppSettings["Redis.Host"] ?? "localhost",
                port: int.Parse(ConfigurationManager.AppSettings["Redis.Port"] ?? "6379"),
                defaultDb: int.Parse(ConfigurationManager.AppSettings["Redis.Db"] ?? "0"),
                password: ConfigurationManager.AppSettings["Redis.Password"]
                );
        }
    }
}