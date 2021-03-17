using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace mars_rover
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(RoverControl(
                new Point(5, 5),
                new List<Rover> {
                    new Rover(new Position(1, 2, "N"), new List<string>{"L","M","L","M", "L", "M", "L","M","M" }),
                    new Rover(new Position(3, 3, "E"), new List<string>{"M","M","R","M","M","R","M","R","R","M" })
                }));
        }

        /// <summary>
        /// get next direction for current direction and heading
        /// </summary>
        /// <param name="heading"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private static string getNextDirection(string heading, string current)
        {
            return heading switch
            {
                "L" => current switch
                {
                    "W" => "S",
                    "N" => "W",
                    "S" => "E",
                    "E" => "N",
                    _ => current,
                },
                "R" => current switch
                {
                    "W" => "N",
                    "N" => "E",
                    "S" => "W",
                    "E" => "S",
                    _ => current,
                },
                _ => current,
            };
        }

        internal class Position
        {
            internal int X;
            internal int Y;
            internal string CurrentDirection;

            internal Position(int x, int y, string direction)
            {
                X = x;
                Y = y;
                CurrentDirection = direction;
            }
        }

        internal class Rover
        {
            internal Position CurrentPosition;
            internal List<string> Movements;
            internal Rover(Position current, List<string> movements)
            {
                if (current != null && current.X >= 0 && current.Y >= 0)
                    CurrentPosition = current;
                else
                {
                    throw new Exception("invalid initial position");
                }
                Movements = movements ?? new List<string>();
            }
        }

        /// <summary>
        /// rover control function
        /// </summary>
        /// <param name="upperRight">upper right coordinates</param>
        /// <param name="rovers"> information for rovers</param>
        /// <returns></returns>
        public static string RoverControl(Point upperRight, List<Rover> rovers)
        {
            //genarate a map of tiles with boolean values to determine if a tile is accessible for movement.
            var map = new bool[upperRight.X + 1, upperRight.Y + 1];
            for (int i = 0; i <= upperRight.X; i++)
            {
                for (int j = 0; j <= upperRight.Y; j++)
                    map[i, j] = true;
            }

            //place rovers
            if (rovers == null || rovers.Count <= 0)
                return("No rovers found");

            var errMessage = "";
            rovers.ForEach(delegate (Rover rover)
            {
                if (rover.CurrentPosition.X > upperRight.X || rover.CurrentPosition.Y > upperRight.Y || map[rover.CurrentPosition.X, rover.CurrentPosition.Y] == false) //rovers cant be placed out of crater or on top of each other
                    errMessage = "Invalid rover placement at " + rover.CurrentPosition.X.ToString() + "," + rover.CurrentPosition.X.ToString();
                map[rover.CurrentPosition.X, rover.CurrentPosition.Y] = false;
            });
            if (!String.IsNullOrEmpty(errMessage))
                return errMessage;
            bool exploring = true;

            //start exploring. go perseverance! :)
            while (exploring)
            {
                exploring = false;
                rovers.ForEach(delegate (Rover rover)
                {
                    if (rover.Movements.Count > 0)
                    {
                        var curMove = rover.Movements[0];
                        if (curMove == "L" || curMove == "R")// change direction
                        {
                            rover.CurrentPosition.CurrentDirection = getNextDirection(curMove, rover.CurrentPosition.CurrentDirection);
                            rover.Movements.RemoveAt(0);
                        }
                        else if (curMove == "M")// try to move
                        {
                            int nextX = 0;
                            int nextY = 0;
                            //determine next tile
                            if (rover.CurrentPosition.CurrentDirection == "N")
                            {
                                nextX = rover.CurrentPosition.X;
                                nextY = rover.CurrentPosition.Y + 1;
                            }
                            else if (rover.CurrentPosition.CurrentDirection == "W")
                            {
                                nextX = rover.CurrentPosition.X - 1;
                                nextY = rover.CurrentPosition.Y;
                            }
                            else if (rover.CurrentPosition.CurrentDirection == "E")
                            {
                                nextX = rover.CurrentPosition.X + 1;
                                nextY = rover.CurrentPosition.Y;
                            }
                            else if (rover.CurrentPosition.CurrentDirection == "S")
                            {
                                nextX = rover.CurrentPosition.X;
                                nextY = rover.CurrentPosition.Y - 1;
                            }

                            if (nextX < 0 || nextY < 0 || nextX > upperRight.X || nextY > upperRight.Y) // if trying to move out of crater or if the blocking rover is  out of moves then skip to next move
                                rover.Movements.RemoveAt(0);
                            else if (rovers.FindIndex(x => x.CurrentPosition.X == nextX && x.CurrentPosition.Y == nextY && x.Movements.Count == 0) != -1)
                                rover.Movements.RemoveAt(0);

                            else if (map[nextX, nextY] == true) // move forward if the next tile is accesible
                            {
                                map[nextX, nextY] = false;
                                map[rover.CurrentPosition.X, rover.CurrentPosition.Y] = true;
                                rover.CurrentPosition.X = nextX;
                                rover.CurrentPosition.Y = nextY;
                                rover.Movements.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //invalid command , remove it
                            rover.Movements.RemoveAt(0);
                        }
                    }
                    // exploration should continue as long as some rovers have moves left
                    if (exploring == false)
                        exploring = rover.Movements.Count > 0;
                });
            }
            StringBuilder res = new StringBuilder("");
            rovers.ForEach(delegate (Rover rov)
            {
                res.Append(rov.CurrentPosition.X.ToString() + " " + rov.CurrentPosition.Y.ToString() + " " + rov.CurrentPosition.CurrentDirection + "\n");
            });
            return res.ToString();
        }
    }
}
