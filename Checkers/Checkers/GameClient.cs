﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Navigation;

namespace Checkers
{
    public class GameClient
    {
        private Boolean inGame = false;
        private Boolean isConnected = false;

        private String     userId;
        private IPEndPoint remote;
        private Socket     client;
        private byte[]     r_buff = new byte[16384];

        private static GameState parseFromString(String gString)
        {
            GameState ret = new GameState();

            return ret;
        }

        private static String parseToString(GameState gState)
        {
            return "";
        }

        public int Connect(String netAddress, String userName)
        {
            int ret = -1;

            if (isConnected)
                return ret;

            userId = userName;
            remote = new IPEndPoint(IPAddress.Parse(netAddress), 8080);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                int r_sz;
                String r;

                client.Connect(remote);

                client.Send(Encoding.ASCII.GetBytes(userId + ": HELLO"));

                r_sz = client.Receive(r_buff);
                r = Encoding.ASCII.GetString(r_buff, 0, r_sz);

                if (r.Equals("S:WELCOME", StringComparison.OrdinalIgnoreCase)) {
                    ret = 0;
                    isConnected = true;
                } else if (r.Equals("S:E:USER EXISTS", StringComparison.OrdinalIgnoreCase)) {
                    ret = -2;
                }
            } catch (Exception e) {Console.WriteLine(e.ToString());}

            return ret;
        }

        public int disconnect()
        {
            if (!isConnected)
                return -1;

            try
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                isConnected = false;
            } catch (Exception e) {Console.Write(e.ToString());}

            return 0;
        }

        public List<String> listPlayers()
        {
            List<String> ret = new List<string>();

            if (isConnected)
            {
                try
                {
                    int r_sz;
                    String r;

                    client.Send(Encoding.ASCII.GetBytes(userId + ": LIST PLAYERS"));

                    r_sz = client.Receive(r_buff);
                    r = Encoding.ASCII.GetString(r_buff, 0, r_sz);

                    if (r.StartsWith("S:OKAY", StringComparison.OrdinalIgnoreCase))
                    {
                        r = Encoding.ASCII.GetString(r_buff, 7, r_sz);
                        ret = r.Split('|').ToList();
                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }

            return ret;
        }

        public List<GameState> listGames()
        {
            List<GameState> ret = new List<GameState>();

            if (isConnected)
            {
                try
                {
                    int r_sz;
                    String r;

                    client.Send(Encoding.ASCII.GetBytes(userId + ": LIST GAMES"));

                    r_sz = client.Receive(r_buff);
                    r = Encoding.ASCII.GetString(r_buff, 0, r_sz);

                    if (r.StartsWith("S:OKAY", StringComparison.OrdinalIgnoreCase))
                    {
                        r = Encoding.ASCII.GetString(r_buff, 7, r_sz);
                        ret = r.Split('|').Select(gString => parseFromString(gString)).ToList();
                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }

            return ret;
        }

        public GameState joinGame(GameState remote)
        {
            if (!isConnected || inGame)
                return null;

            inGame = true;
            return new GameState();
        }

        public int quitGame()
        {
            if (!isConnected || !inGame)
                return -1;

            inGame = false;
            return 0;
        }

        public int sendMove(GameState state)
        {
            if (!isConnected || !inGame)
                return -1;

            return -1;
        }

        private int sendState(GameState remoteGame, GameState state)
        {
            return 0;
        }

        // Note: This call may block, call in async state or in a worker thread
        public GameState receiveMove()
        {
            if (!isConnected || !inGame)
                return null;

            return null;
        }

        private GameState receiveState(GameState game)
        {
            return new GameState();
        }

        public static implicit operator GameClient(NavigationEventArgs v)
        {
            throw new NotImplementedException();
        }
    }
}
