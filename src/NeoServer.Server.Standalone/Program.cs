﻿using Autofac;
using NeoServer.Data;
using NeoServer.Game.World.Spawns;
using NeoServer.Loaders.Interfaces;
using NeoServer.Loaders.Items;
using NeoServer.Loaders.Monsters;
using NeoServer.Loaders.Spawns;
using NeoServer.Loaders.Spells;
using NeoServer.Loaders.Vocations;
using NeoServer.Loaders.World;
using NeoServer.Networking.Listeners;
using NeoServer.Scripts.Lua;
using NeoServer.Server.Compiler;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Tasks;
using NeoServer.Server.Events;
using NeoServer.Server.Jobs.Creatures;
using NeoServer.Server.Jobs.Items;
using NeoServer.Server.Security;
using NeoServer.Server.Standalone;
using NeoServer.Server.Standalone.IoC;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class Program
{
    public static void Main()
    {
        Console.Title = "OpenCoreMMO Server";

        var sw = new Stopwatch();
        sw.Start();

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var (serverConfiguration, gameConfiguration, logConfiguration) = Container.LoadConfigurations();

        var (logger, loggerConfiguration) = Container.RegisterLogger();

        logger.Information("Welcome to OpenCoreMMO Server!");

        logger.Information("Log set to: {log}", logConfiguration.MinimumLevel);
        logger.Information("Environment: {env}", Environment.GetEnvironmentVariable("ENVIRONMENT"));

        logger.Information("Compiling scripts...");
        ScriptCompiler.Compile(serverConfiguration.Data);

        var container = Container.CompositionRoot();
        var databaseConfiguration = container.Resolve<DatabaseConfiguration2>();

        logger.Information("Loading database: {db}", databaseConfiguration.active);

        var context = container.Resolve<NeoContext>();
        context.Database.EnsureCreated();

        RSA.LoadPem(serverConfiguration.Data);

        container.Resolve<IEnumerable<IRunBeforeLoaders>>().ToList().ForEach(x => x.Run());

        container.Resolve<ItemTypeLoader>().Load();

        container.Resolve<WorldLoader>().Load();

        container.Resolve<SpawnLoader>().Load();

        container.Resolve<MonsterLoader>().Load();
        container.Resolve<VocationLoader>().Load();
        container.Resolve<SpellLoader>().Load();

        container.Resolve<IEnumerable<IStartupLoader>>().ToList().ForEach(x => x.Load());

        container.Resolve<SpawnManager>().StartSpawn();

        var listeningTask = StartListening(container, cancellationToken);

        var scheduler = container.Resolve<IScheduler>();
        var dispatcher = container.Resolve<IDispatcher>();

        dispatcher.Start(cancellationToken);
        scheduler.Start(cancellationToken);

        scheduler.AddEvent(new SchedulerEvent(1000, container.Resolve<GameCreatureJob>().StartChecking));
        scheduler.AddEvent(new SchedulerEvent(1000, container.Resolve<GameItemJob>().StartChecking));
        scheduler.AddEvent(new SchedulerEvent(1000, container.Resolve<GameChatChannelJob>().StartChecking));

        container.Resolve<EventSubscriber>().AttachEvents();

        container.Resolve<IGameServer>().Open();

        container.Resolve<LuaGlobalRegister>().Register();

        sw.Stop();

        logger.Information($"Running Garbage Collector");

        GC.Collect();
        GC.WaitForPendingFinalizers();

        logger.Information("Memory usage: {mem} MB", Math.Round((Process.GetCurrentProcess().WorkingSet64 / 1024f) / 1024f, 2));

        logger.Information("Server is {up}! {time} ms", "up", sw.ElapsedMilliseconds);

        listeningTask.Wait(cancellationToken);
    }

    static async Task StartListening(IContainer container, CancellationToken token)
    {
        container.Resolve<LoginListener>().BeginListening();
        container.Resolve<GameListener>().BeginListening();

        while (!token.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), token);
        }
    }
}

