using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_250319
{
        internal class Program
        {
            struct Position
            {
                public int x;
                public int y;
            }
            struct Status
            {
                public bool gameOver;
                public bool gameClear;
                public bool isDamage;
                public bool isGoal;
                public int heart;
                public int moveNum;
            }
            struct Level
            {
                public int mapLevel;
                public int acheve;
            }

            static void Main(string[] args)
            {
                bool reGame = true;
                Level level = new Level();
                level.mapLevel = 1;
                while (reGame)
                {
                    Position playerPos = new Position();
                    Status status = new Status();
                    char[,] map;
                    char[,] visible;
                    status.gameOver = false;
                    status.gameClear = false;
                    status.isDamage = false;
                    status.isGoal = false;
                    status.heart = 3;


                    Start(level, out playerPos, out map, out visible, ref reGame);


                    while (!status.gameOver)
                    {
                        Render(playerPos, visible, ref status);
                        ConsoleKey key = Input();
                        Update(ref playerPos, key, map, visible, ref status, ref reGame, ref level);
                    }
                    //마지막 화면 render 진행
                    Render(playerPos, visible, ref status);
                    End(status, ref reGame, ref level);
                }
            }

            static void Start(Level level, out Position playerPos, out char[,] map, out char[,] visible, ref bool reGame)
            {
                // 게임 재시작 비활성
                reGame = false;

                //플레이어 시작 위치 정의 
                playerPos.x = 2;
                playerPos.y = 3;

                //mapLevel이 1일때의 맵
                if (level.mapLevel == 1)
                {
                    // 지뢰와 골 위치를 지도로 구현
                    map = new char[7, 12]
                        {
                    {'▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    {'▒', '*', ' ', ' ', '*', '*', ' ', ' ', '*', '*', ' ', '▒' },
                    {'▒', ' ', ' ', ' ', ' ', ' ', '*', ' ', ' ', ' ', ' ', '▒' },
                    {'▒', ' ', ' ', ' ', '*', ' ', ' ', ' ', ' ', '*', 'G', '▒' },
                    {'▒', ' ', ' ', ' ', ' ', '*', '*', ' ', '*', ' ', ' ', '▒' },
                    {'▒', ' ', '*', '*', ' ', ' ', ' ', ' ', ' ', ' ', '*', '▒' },
                    {'▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                        };
                }
                else if (level.mapLevel == 2)
                {
                    // 지뢰와 골 위치를 지도로 구현
                    map = new char[12, 12]
                        {
                    {'▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    {'▒', '*', ' ', '*', ' ', '*', ' ', '*', ' ', ' ', '*', '▒' },
                    {'▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '*', ' ', '▒' },
                    {'▒', ' ', ' ', ' ', '*', ' ', ' ', '*', ' ', ' ', ' ', '▒' },
                    {'▒', ' ', ' ', ' ', ' ', ' ', '*', '*', '*', ' ', '*', '▒' },
                    {'▒', ' ', '*', ' ', '*', ' ', ' ', ' ', '*', ' ', '*', '▒' },
                    {'▒', '*', ' ', ' ', ' ', ' ', '*', ' ', ' ', ' ', 'G', '▒' },
                    {'▒', ' ', '*', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '*', '▒' },
                    {'▒', ' ', ' ', ' ', '*', ' ', ' ', ' ', ' ', ' ', ' ', '▒' },
                    {'▒', '*', ' ', ' ', ' ', '*', ' ', '*', ' ', '*', ' ', '▒' },
                    {'▒', ' ', ' ', '*', ' ', '*', ' ', ' ', '*', ' ', '*', '▒' },
                    {'▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                        };
                }
                else
                {
                    // 지뢰와 골 위치를 지도로 구현
                    map = new char[12, 16]
                        {
                    {'▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    {'▒', ' ', ' ', '*', '*', ' ', ' ', ' ', ' ', '*', '*', ' ', ' ', ' ', ' ', '▒' },
                    {'▒', ' ', ' ', ' ', '*', '*', ' ', '*', ' ', ' ', ' ', '*', '*', ' ', '*', '▒' },
                    {'▒', ' ', ' ', ' ', ' ', ' ', '*', ' ', ' ', '*', ' ', ' ', '*', ' ', '*', '▒' },
                    {'▒', ' ', ' ', ' ', ' ', ' ', '*', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒' },
                    {'▒', ' ', '*', ' ', ' ', ' ', ' ', ' ', ' ', '*', '*', ' ', ' ', ' ', '*', '▒' },
                    {'▒', ' ', '*', ' ', '*', ' ', ' ', ' ', ' ', '*', ' ', ' ', '*', ' ', '*', '▒' },
                    {'▒', '*', ' ', ' ', '*', ' ', '*', '*', ' ', ' ', ' ', '*', ' ', ' ', 'G', '▒' },
                    {'▒', ' ', ' ', ' ', '*', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒' },
                    {'▒', ' ', '*', ' ', '*', ' ', '*', '*', '*', '*', '*', ' ', '*', '*', ' ', '▒' },
                    {'▒', ' ', '*', ' ', ' ', ' ', '*', ' ', ' ', '*', ' ', ' ', ' ', ' ', '*', '▒' },
                    {'▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                        };
                }


                // 지도에서 주변의 지뢰의 개수를 자동으로 해당 위치에 저장
                for (int y = 0; y < map.GetLength(0) - 1; y++)
                {
                    for (int x = 0; x < map.GetLength(1) - 1; x++)
                    {
                        // 현재 지도의 위치에 값이 ' ' (즉, 할당되지 않은 경우) 해당 지점 주변의 지뢰의 개수를 탐색
                        if (map[y, x] == ' ')
                        {
                            int mineNum = 0;
                            for (int i = -1; i <= 1; i++)
                            {
                                for (int j = -1; j <= 1; j++)
                                {   // 해당 지점 주변 3*3의 공간을 살펴서 지뢰의 개수를 더해줌 
                                    if (map[y + i, x + j] == '*')
                                    {
                                        mineNum++;
                                    }
                                }
                            }
                            // 지뢰의 개수를 현재 위치에 char의 형태로 할당해 줌
                            map[y, x] = (char)(mineNum + '0');
                        }

                    }
                }
                // 화면에 보여질 공개된 부분을 생성
                visible = new char[map.GetLength(0), map.GetLength(1)];
                for (int y = 0; y < visible.GetLength(0); y++)
                {
                    for (int x = 0; x < visible.GetLength(1); x++)
                    {
                        // 각 라인별로 첫 줄을 이동 불가능한 벽으로 생성
                        if (y == 0 || y == visible.GetLength(0) - 1 || x == 0 || x == visible.GetLength(1) - 1)
                        {
                            visible[y, x] = '▒';
                        }
                        // 해당 좌표가 골일 경우 공개 블럭 G로 설정
                        else if (map[y, x] == 'G')
                        {
                            visible[y, x] = 'G';
                        }
                        // 벽이 아닌 부분은 일단 유저에게 보여지지 않는 블럭으로 설정
                        else
                        {
                            visible[y, x] = '■';
                        }
                    }
                }
                for (int yPos = playerPos.y - 1; yPos <= playerPos.y + 1; yPos++)
                {
                    for (int xPos = playerPos.x - 1; xPos <= playerPos.x + 1; xPos++)
                    {
                        // 유저의 시작 위치 (3,2)에서부터 3*3의 칸을 공개 타일로 변경
                        visible[yPos, xPos] = map[yPos, xPos];
                    }
                }
                Console.CursorVisible = false;
                if (level.mapLevel == 1)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n     지뢰 피하기 게임에 오신 것을 환영합니다! ");
                    Console.WriteLine("     이 게임은 바닥에 표시된 숫자를 보고 지뢰를 피해");
                    Console.WriteLine("     골인 지점에 다다르는 게임입니다! ");
                    Console.WriteLine("     시작할 준비가 되셨다면 아무키나 눌러 게임을 시작해주세요!");
                    Console.ReadKey(true);
                }
            }

            static void Render(Position playerPos, char[,] visible, ref Status status)
            {
                // 공개 타일 부분을 화면에 표시
                PrintMap(visible, ref status);
                // 플레이어의 현재 위치를 화면에 출력
                PrintPlayer(playerPos, status);
            }

            static void PrintMap(char[,] visible, ref Status status)
            {
                //화면의 미리 표시되던 내용을 지워줌
                Console.Clear();
                for (int y = 0; y < visible.GetLength(0); y++)
                {
                    for (int x = 0; x < visible.GetLength(1); x++)
                    {
                        // 공개 타일 부분의 숫자가 0이면 회색
                        if (visible[y, x] == '0')
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        // 공개 타일 부분의 숫자가 1,5이면 노란색
                        else if (visible[y, x] == '1' || visible[y, x] == '5')
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                        // 공개 타일 부분의 숫자가 2,6이면 파란색
                        else if (visible[y, x] == '2' || visible[y, x] == '6')
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                        }
                        // 공개 타일 부분의 숫자가 3,7이면 초록색
                        else if (visible[y, x] == '3' || visible[y, x] == '7')
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        // 공개 타일의 부분의 숫자가 4면 빨간색
                        else if (visible[y, x] == '4')
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        // 공개 타일이 벽인 경우
                        else if (visible[y, x] == '▒')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }
                        // 공개 타일로 맵을 그림
                        Console.Write(visible[y, x]);
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                }
                // 맵 우측에 남은 체력 표시
                Console.SetCursorPosition(visible.GetLength(1) + 2, 0);
                for (int i = 0; i < 3; i++)
                {
                    // 체력은 3개부터 시작, 체력이 감소한 적이 없으면 3개를 빨간색으로 표시
                    if (i < status.heart)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("♥ ");
                        Console.ResetColor();
                    }
                    // 잃은 체력은 빈 하트를 회색으로 표시
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("♡ ");
                        Console.ResetColor();
                    }
                }
                // 체력을 잃었을 때 메시지를 띄움
                if (status.isDamage)
                {
                    Console.SetCursorPosition(visible.GetLength(1) + 2, 6);
                    Console.Write("생명력을 1 잃었습니다!");
                    status.isDamage = false;
                }
            }

            static void PrintPlayer(Position playerPos, Status status)
            {
                // 커서를 유저의 위치에 둠
                Console.SetCursorPosition(playerPos.x, playerPos.y);
                Console.ForegroundColor = ConsoleColor.White;
                if (!status.gameClear)
                {
                    // 기본적으로 플레이어의 위치를 출력함
                    Console.Write('▶');
                }
                else
                {
                    //골에 도착했을 때 플레이어 마크를 지우고 골을 표시함
                    Console.Write('G');
                }
                Console.ResetColor();
            }

            static ConsoleKey Input()
            {
                // 키보드 입력을 받아옴
                return Console.ReadKey(true).Key;
            }

            static void Update(ref Position playerPos, ConsoleKey key, char[,] map, char[,] visible, ref Status status, ref bool reGame, ref Level level)
            {
                // 이동할 위치를 position 구조체로 선언
                Position targetPos = new Position();
                targetPos = playerPos;

                switch (key)
                {
                    // 위 방향키, W를 눌렀을 때 y값에 -1 (Console이므로)
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        targetPos.y -= 1;
                        break;
                    // 아래 방향키, S를 눌렀을 때 y값에 +1
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        targetPos.y += 1;
                        break;
                    // 왼쪽 방향키, A를 눌렀을 때 x값에 -1
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        targetPos.x -= 1;
                        break;
                    // 오른쪽 방향키, D를 눌렀을 때 x값에 +1
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        targetPos.x += 1;
                        break;
                    // R키를 눌렀을 때 재시작을 구현한다. return을 통해 빠르게 재시작을 할 수 있도록 한다.
                    case ConsoleKey.R:
                        reGame = true;
                        status.gameOver = true;
                        return;
                }
                PositionCalculate(targetPos, map, ref playerPos, visible, ref status, ref level);
                status.gameOver = IsCleared(ref status);
            }

            static void PositionCalculate(Position targetPos, char[,] map, ref Position playerPos, char[,] visible, ref Status status, ref Level level)
            {
                // 1. 이동하려는 곳이 벽일 경우
                if (map[targetPos.y, targetPos.x] == '▒')
                {
                    //아무일도 일어나지 않는다. (이동 불가)
                }
                // 2. 이동하려는 곳이 지뢰인 경우
                else if (map[targetPos.y, targetPos.x] == '*')
                {
                    // 지뢰로 공개되지 않았던 지역이면 생명력을 1 잃는다.
                    if (visible[targetPos.y, targetPos.x] == '■')
                    {
                        status.heart--;
                        status.isDamage = true;
                        status.moveNum++;
                    }
                    // visible의 해당 타일을 공개 타일 *로 바꾼다.
                    visible[targetPos.y, targetPos.x] = '*';
                }
                // 3. 이동하려는 곳이 숫자인 경우
                else if (int.TryParse(map[targetPos.y, targetPos.x].ToString(), out int mineNum))
                {
                    // mineNum이 0인 경우 (주변 지뢰 갯수가 0인 경우)
                    if (mineNum == 0)
                    {
                        // 이동하고자하는 y, x좌표를 저장 
                        int yPos = targetPos.y;
                        int xPos = targetPos.x;
                        // 해당 좌표값을 가지고 openZero 함수를 실행한다.
                        OpenZero(yPos, xPos, ref visible, map);
                    }
                    else
                    {
                        // 아닐시 이동할 타일만 공개 타일로 변환
                        visible[targetPos.y, targetPos.x] = map[targetPos.y, targetPos.x];
                    }
                    // 숫자인 경우 이동가능한 타일이므로 플레이어의 위치를 변경
                    playerPos = targetPos;
                    status.moveNum++;
                }
                // 4. 이동하려는 곳이 골인 경우
                else if (map[targetPos.y, targetPos.x] == 'G')
                {
                    // 골에 도달했다는 내용을 true로 변경
                    status.isGoal = true;
                    // 골의 경우 이동가능한 타일이므로 플레이어의 위치를 변경
                    playerPos = targetPos;
                    status.moveNum++;
                    if (level.acheve == 0 || level.acheve > status.moveNum)
                    {
                        level.acheve = status.moveNum;
                    }

                }
            }

            // 값이 0인 타일을 열었을 때 처리 방법
            static void OpenZero(int yPos, int xPos, ref char[,] visible, char[,] map)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        // 주변 타일 중 비공개 타일을 찾아서
                        if (visible[yPos + i, xPos + j] == '■')
                        {
                            // 해당 타일을 공개 타일로 변환
                            visible[yPos + i, xPos + j] = map[yPos + i, xPos + j];
                            // 만약 열린 공개 타일 중 값이 0인 타일이 있다면
                            if (visible[yPos + i, xPos + j] == '0')
                            {
                                // 해당 타일을 중심으로 다시 공개타일을 전환 , 재귀함수 진행
                                OpenZero(yPos + i, xPos + j, ref visible, map);
                            }
                        }
                    }
                }
            }
            static bool IsCleared(ref Status status)
            {
                // 체력이 0인 경우
                if (status.heart == 0)
                {
                    return true;
                }
                // 골에 도달한 경우
                else if (status.isGoal)
                {
                    //gameClear 사실을 저장한다.
                    status.gameClear = true;
                    return true;
                }
                return false;
            }

            static void End(Status status, ref bool reGame, ref Level level)
            {
                // R을 눌러서 reGame이 활성화 되어있을 시에는 빠른 재시작을 위해 return한다.
                if (reGame == true)
                {
                    return;
                }
                //사용자에게 아무키나 입력받으면 결과창으로 이동함
                Console.ReadKey(true);
                Console.Clear();
                if (status.gameClear)
                {
                    Console.WriteLine("            Congraturations            ");
                    Console.WriteLine("지뢰 피하기 게임 클리어를 축하드립니다!");
                }
                else if (!status.gameClear)
                {
                    Console.WriteLine("아쉽게 체력이 다 떨어졌네요...");
                    Console.WriteLine("    다시 한번 도전해주세요!   ");
                }
                rePlay(status, ref reGame, ref level);
            }

            static void rePlay(Status status, ref bool reGame, ref Level level)
            {
                ConsoleKey key;
                Console.WriteLine("게임을 다시 즐겨보시겠어요? (Y/N)");
                do
                {
                    // 위의 writeLine을 토대로 키입력을 Y 혹은 N을 받을때까지 반복한다.
                    key = Console.ReadKey(true).Key;
                } while (key != ConsoleKey.Y && key != ConsoleKey.N);
                // Y키를 받았을 땐 loop를 돌릴 수 있게 reGame을 true로 만들어 준다.
                if (key == ConsoleKey.Y)
                {
                    reGame = true;
                    // 클리어의 경우 레벨을 올릴 것인지 물어본다.
                    if (status.gameClear)
                    {
                        Console.Clear();
                        Console.WriteLine("그럼 레벨을 올리시겠어요(Y/N)");
                        // 현재 레벨의 최단 이동 횟수를 보여준다.
                        Console.WriteLine("{0} 레벨 최단 이동 횟수: {1}", level.mapLevel, level.acheve);
                        do
                        {
                            // 위의 writeLine을 토대로 키입력을 Y 혹은 N을 받을때까지 반복한다.
                            key = Console.ReadKey(true).Key;
                        } while (key != ConsoleKey.Y && key != ConsoleKey.N);
                        if (key == ConsoleKey.Y)
                        {
                            //사용자가 Y를 누르면 맵 레벨을 1 높인다. (이후 나가서 레벨 1 높은 맵을 실행함)
                            level.mapLevel++;
                            level.acheve = 0;
                        }
                    }
                    status.moveNum = 0;
                }
                Console.Clear();
            }
        }
    }


