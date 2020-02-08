using System;
using System.Collections.Generic;
using Serilog;
using Fo76_Communication.Packets;
using Fo76_Communication.Packets.Types;

namespace Fo76_Communication
{
    public class PacketProcessor
    {
        public List<Client> Clients = new List<Client>();
        public void ProcessPacket(byte[] data)
        {
            Packet packet = Packet.ParsePacket(data);
            Client client = GetClient(packet.Head.Session);

            if (client == null)
            {
                client = new Client(packet.Head.Session);
                Clients.Add(client);
                Log.Information("New client connected with id {@SessionId}", client.SessionId);
            }

            switch (packet.Head.Type)
            {
                case PacketType.Token:
                    this.HandleToken(client, (Token)packet);
                    break;
                case PacketType.PingRequest:
                    this.HandlePingRequest(client, (PingRequest)packet);
                    break;
                case PacketType.PingReply:
                    this.HandlePingReply(client, (PingReply)packet);
                    break;
                case PacketType.Message:
                    this.HandleMessage(client, (Message)packet);
                    break;
                default:
                    throw (new Exception("Unable to handle Packet with Type " + packet.Head.Type));
            }
        }

        private void HandleToken(Client client, Token token)
        {
            if (!Config.IgnoreToken)
            {
                throw (new Exception("Token handling not implemented."));
            }

            client.Token = "StubToken";
        }

        private void HandlePingRequest(Client client, PingRequest pingRequest)
        {

        }

        private void HandlePingReply(Client client, PingReply pingReply)
        {

        }

        private void HandleMessage(Client client, Message message)
        {
            if (client.Token == null)
            {
                Log.Warning("Client {@SessionId} tried to send message packet before setting the token ", client.SessionId);

                if (Config.IgnoreToken)
                {
                    client.Token = "StubToken";
                }
            }

            client.ProcessMessage(message);
        }

        private Client GetClient(ushort sessionId) => Clients.Find(x => x.SessionId == sessionId);
    }
}
