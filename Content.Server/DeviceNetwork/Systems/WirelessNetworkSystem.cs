using Content.Server.DeviceNetwork.Components;
using Content.Shared._NF.Shipyard.Components; // Frontier
using Content.Shared.DeviceNetwork.Events;
using JetBrains.Annotations;
using Content.Shared.DeviceLinking;

namespace Content.Server.DeviceNetwork.Systems
{
    [UsedImplicitly]
    public sealed class WirelessNetworkSystem : EntitySystem
    {
        [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
        [Dependency] private readonly SharedDeviceLinkSystem _device = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<WirelessNetworkComponent, BeforePacketSentEvent>(OnBeforePacketSent);
        }

        /// <summary>
        /// Gets the position of both the sending and receiving entity and checks if the receiver is in range of the sender.
        /// </summary>
        private void OnBeforePacketSent(EntityUid uid, WirelessNetworkComponent component, BeforePacketSentEvent args)
        {
            var ownPosition = args.SenderPosition;
            var xform = Transform(uid);

            // not a wireless to wireless connection, just let it happen
            if (!TryComp<WirelessNetworkComponent>(args.Sender, out var sendingComponent))
                return;

            // Frontier - Unlimited device range on ships. Using code from device linking to save copypasting here.
            if (!_device.InRange(args.Sender, uid, sendingComponent.Range))
                args.Cancel();
        }
    }
}
