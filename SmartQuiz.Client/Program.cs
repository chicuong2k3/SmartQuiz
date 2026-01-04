using ActualLab.Fusion.Authentication;
using ActualLab.Fusion.Blazor;
using ActualLab.Fusion.Blazor.Authentication;
using ActualLab.Fusion.Client.Caching;
using ActualLab.Fusion.Extensions;
using ActualLab.Rpc;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartQuiz.Client;
using SmartQuiz.Client.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthServices();
builder.Services.RegisterSharedServices();

RpcSerializationFormatResolver.Default = new("msgpack5");
builder.Services.AddFusion(fusion =>
{
    fusion.AddBlazor()
        .AddAuthentication()
        .AddPresenceReporter();
    fusion.AddAuthClient();
    fusion.Rpc.AddWebSocketClient(builder.HostEnvironment.BaseAddress);
    fusion.AddFusionTime();

    fusion.AddClient<IFlashcardService>();
    fusion.AddClient<IFlashcardSetService>();
    fusion.AddClient<IQuizResultService>();
});

// builder.Services.AddSingleton<RpcPeerOptions>(_ => RpcPeerOptions.Default with {
//     PeerFactory = (hub, peerRef) => peerRef.IsServer
//         ? throw new NotSupportedException("No server peers on the client.")
//         : new RpcClientPeer(hub, peerRef) { CallLogLevel = LogLevel.Information },
// });

builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddSingleton(_ => LocalStorageRemoteComputedCache.Options.Default);
builder.Services.AddSingleton<IRemoteComputedCache>(c =>
{
    var options = c.GetRequiredService<LocalStorageRemoteComputedCache.Options>();
    return new LocalStorageRemoteComputedCache(options, c);
});

await builder.Build().RunAsync();