using System;
using System.Drawing;
using System.Windows.Forms;

namespace MouseCheeseGame
{
    public partial class Form1 : Form
    {
        private const int MapSize = 21;
        private const int CellSize = 35;

        private PictureBox[,] map;
        private PictureBox mouse, cheese, cat, trap;

        private int mouseX, mouseY;
        private int cheeseX, cheeseY;
        private int catX, catY;
        private int trap1X, trap1Y, trap2X, trap2Y;

        private Random random = new Random();

        private Timer catTimer;

        // Лабиринт
        private bool[,] obstacleMap = new bool[MapSize, MapSize];
        private int cheeseFoundCount = 0;

        public Form1()
        {
            InitializeComponent();
            
            GenerateObstacles(1);
            InitializeMap(); // Генерируем лабиринт препятствий
            PlaceObjects(1);

            // Инициализируем таймер для движения кота
            catTimer = new Timer();
            catTimer.Interval = 500; // Интервал в миллисекундах
            catTimer.Tick += CatTimer_Tick;
            catTimer.Start();
        }

        private void CatTimer_Tick(object sender, EventArgs e)
        {
            MoveCat();
        }

        private void InitializeMap()
        {
            map = new PictureBox[MapSize, MapSize];

            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    map[i, j] = new PictureBox();
                    map[i, j].Size = new Size(CellSize, CellSize);
                    map[i, j].Location = new Point(j * CellSize, i * CellSize);
                    map[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
                    map[i, j].Click += Map_Click; // Добавляем обработчик клика на каждую ячейку

                    // Если текущая ячейка - препятствие, устанавливаем ей фоновое изображение зеленого цвета
                    if (obstacleMap[i, j])
                    {
                        //map[i, j].BackgroundImage = CreateGreenImage();
                        map[i, j].Image = Image.FromFile(@"C:\Users\jkruu\OneDrive\Рабочий стол\Задачи\MouseCheeseGame\MouseCheeseGame\Images\cheesebg.jpg");
                    }
                    else
                    {
                        map[i, j].BackgroundImage = CreateYellowImage();
                    }

                    Controls.Add(map[i, j]);
                }
            }

            mouse = new PictureBox();
            mouse.Image = Image.FromFile(@"Images\mouse.png");
            mouse.Size = new Size(CellSize, CellSize);
            mouse.BackColor = Color.FromArgb(255, 247, 153);

            cheese = new PictureBox();
            cheese.Image = Image.FromFile(@"Images\cheese.png");
            cheese.Size = new Size(CellSize, CellSize);

            cat = new PictureBox();
            cat.Image = Image.FromFile(@"Images\cat.png");
            cat.Size = new Size(CellSize, CellSize);

            trap = new PictureBox();
            trap.Image = Image.FromFile(@"Images\trap.png");
            trap.Size = new Size(CellSize, CellSize);

            // Добавляем обработчик нажатия клавиш
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }

        private void PlaceObjects(int level)
        {
            if (level == 1)
            {
                mouseX = 6;
                mouseY = 10;
                cheeseX = random.Next(1, MapSize);
                cheeseY = GenerateRandomValidPositionY();
                catX = 13;
                catY = 10;
                trap1X = 12;
                trap1Y = 2;
                trap2X = 12;
                trap2Y = 18;
            }

            map[mouseX, mouseY].Image = mouse.Image;
            map[cheeseX, cheeseY].Image = cheese.Image;
            map[catX, catY].Image = cat.Image;
            map[trap1X, trap1Y].Image = trap.Image;
            map[trap2X, trap2Y].Image = trap.Image;
        }

        // Generate a random X position for the cheese while avoiding forbidden locations
        private int GenerateRandomValidPositionX()
        {
            int x;
            do
            {
                x = random.Next(1, MapSize);
            } while (x == mouseX || x == catX || x == trap1X || x == trap2X || obstacleMap[x, mouseY]);
            return x;
        }

        // Generate a random Y position for the cheese while avoiding forbidden locations
        private int GenerateRandomValidPositionY()
        {
            int y;
            do
            {
                y = random.Next(1, MapSize);
            } while (y == mouseY || y == catY || y == trap1Y || y == trap2Y || obstacleMap[cheeseX, y]);
            return y;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W: // Нажата клавиша W (вверх)
                    MoveMouse(mouseX - 1, mouseY);
                    break;
                case Keys.S: // Нажата клавиша S (вниз)
                    MoveMouse(mouseX + 1, mouseY);
                    break;
                case Keys.A: // Нажата клавиша A (влево)
                    MoveMouse(mouseX, mouseY - 1);
                    break;
                case Keys.D: // Нажата клавиша D (вправо)
                    MoveMouse(mouseX, mouseY + 1);
                    break;
            }
        }

        private void MoveMouse(int newX, int newY)
        {
            // Проверяем, чтобы новые координаты находились в пределах карты
            if (newX >= 0 && newX < MapSize && newY >= 0 && newY < MapSize && !obstacleMap[newX, newY])
            {
                // Убираем мышку с текущей позиции на карте
                map[mouseX, mouseY].Image = null;

                // Устанавливаем новые координаты для мышки
                mouseX = newX;
                mouseY = newY;

                // Проверяем на столкновения
                CheckCollision();

                // Устанавливаем изображение для новой позиции мышки
                map[mouseX, mouseY].Image = mouse.Image;
            }
        }

        private bool cheeseFound = false;
        private bool catFound = false;
        private bool trapFound = false; // добавим переменную для отслеживания того, найден ли сыр

        private void CheckCollision()
        {
            if (!catFound && mouseX == catX && mouseY == catY)
            {
                catFound = true;
                MessageBox.Show("Вы встретили кота и проиграли!");
                ResetGame();
            }
            else if (!trapFound && (mouseX == trap1X && mouseY == trap1Y || mouseX == trap2X && mouseY == trap2Y))
            {
                trapFound = true;
                MessageBox.Show("Вы попали в мышеловку и проиграли!");
                ResetGame();
            }
            else if (!cheeseFound && mouseX == cheeseX && mouseY == cheeseY)
            {
                cheeseFoundCount++; // Increment the count of found cheese pieces
                map[cheeseX, cheeseY].Image = null; // Remove the cheese from the map
                if (cheeseFoundCount >= 5) // Check if 5 cheese pieces are found
                {
                    cheeseFound = true;
                    MessageBox.Show("Вы нашли все кусочки сыра и выиграли!");
                    ResetGame();
                }
                else
                {
                    cheeseX = random.Next(1, MapSize); // Place a new cheese randomly
                    cheeseY = GenerateRandomValidPositionY();
                    map[cheeseX, cheeseY].Image = cheese.Image; // Show the new cheese on the map
                }
            }
        }


        private void ResetGame()
        {
            cheeseFound = false;
            catFound = false;
            trapFound = false;
            cheeseFoundCount = 0;
            map[catX, catY].Image = null;
            map[cheeseX, cheeseY].Image = null;
            /*for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    map[i, j].Image = null;
                }
            }*/



            GenerateObstacles(1); // Генерируем новый лабиринт препятствий
            PlaceObjects(1);
        }

        private void Map_Click(object sender, EventArgs e)
        {
            PictureBox clickedCell = sender as PictureBox;
            int clickedRow = clickedCell.Location.Y / CellSize;
            int clickedCol = clickedCell.Location.X / CellSize;

            int dx = Math.Abs(clickedRow - mouseX);
            int dy = Math.Abs(clickedCol - mouseY);

            if ((dx == 1 && dy == 0) || (dx == 0 && dy == 1))
            {
                MoveMouse(clickedRow, clickedCol);
            }
        }

        private void MoveCat()
        {
            int dx = mouseX - catX;
            int dy = mouseY - catY;

            // Определяем направление движения кота по горизонтали и вертикали
            int moveX = 0;
            int moveY = 0;

            // Если кот дальше по горизонтали, двигаемся в этом направлении
            if (Math.Abs(dx) > Math.Abs(dy))
                moveX = Math.Sign(dx);
            // Иначе двигаемся по вертикали
            else
                moveY = Math.Sign(dy);

            int newX = catX + moveX;
            int newY = catY + moveY;

            // Проверяем, чтобы новые координаты находились в пределах карты
            if (newX >= 0 && newX < MapSize && newY >= 0 && newY < MapSize && !obstacleMap[newX, newY])
            {
                // Перемещаем кота
                if (newX != cheeseX || newY != cheeseY)
                {
                    // Перемещаем кошку
                    map[catX, catY].Image = null;
                    catX = newX;
                    catY = newY;
                    map[catX, catY].Image = cat.Image;
                }
            }

            // Проверяем столкновения после каждого хода кота
            CheckCollision();
        }

        private void GenerateObstacles(int level)
        {
            // Очищаем лабиринт препятствий
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    obstacleMap[i, j] = false;
                }
            }

            // Генерируем препятствия в лабиринте
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                        if (i == 0 || j == 0 || i == MapSize - 1 || j == MapSize - 1)
                        obstacleMap[i, j] = true;
                }
            }

            if (level == 1)
            {
                obstacleMap[1,1] = true;
                obstacleMap[1,10] = true;
                obstacleMap[1,19] = true;
                obstacleMap[2,10] = true;
                obstacleMap[3,2] = true;
                obstacleMap[3,3] = true;
                obstacleMap[3,10] = true;
                obstacleMap[3,17] = true;
                obstacleMap[3,18] = true;
                obstacleMap[4,5] = true;
                obstacleMap[4,6] = true;
                obstacleMap[4,10] = true;
                obstacleMap[4,14] = true;
                obstacleMap[4,15] = true;
                obstacleMap[5,9] = true;
                obstacleMap[5,10] = true;
                obstacleMap[5,11] = true;
                obstacleMap[7,2] = true;
                obstacleMap[7,3] = true;
                obstacleMap[7,6] = true;
                obstacleMap[7,7] = true;
                obstacleMap[7,8] = true;
                obstacleMap[7,9] = true;
                obstacleMap[7,10] = true;
                obstacleMap[7,11] = true;
                obstacleMap[7,12] = true;
                obstacleMap[7,13] = true;
                obstacleMap[7,14] = true;
                obstacleMap[7,17] = true;
                obstacleMap[7,18] = true;
                obstacleMap[8,2] = true;
                obstacleMap[8,6] = true;
                obstacleMap[8,14] = true;
                obstacleMap[8,18] = true;
                obstacleMap[9,2] = true;
                obstacleMap[9,3] = true;
                obstacleMap[9,6] = true;
                obstacleMap[9,9] = true;
                obstacleMap[9,11] = true;
                obstacleMap[9,14] = true;
                obstacleMap[9, 17] = true;
                obstacleMap[9, 18] = true;
                obstacleMap[10, 6] = true;
                obstacleMap[10, 9] = true;
                obstacleMap[10, 11] = true;
                obstacleMap[10, 14] = true;
                obstacleMap[11, 6] = true;
                obstacleMap[11, 9] = true;
                obstacleMap[11, 10] = true;
                obstacleMap[11, 11] = true;
                obstacleMap[11, 14] = true;
                obstacleMap[12, 4] = true;
                obstacleMap[12, 5] = true;
                obstacleMap[12, 6] = true;
                obstacleMap[12, 14] = true;
                obstacleMap[12, 15] = true;
                obstacleMap[12, 16] = true;
                obstacleMap[15, 2] = true;
                obstacleMap[15, 3] = true;
                obstacleMap[15, 8] = true;
                obstacleMap[15, 9] = true;
                obstacleMap[15, 10] = true;
                obstacleMap[15, 11] = true;
                obstacleMap[15, 12] = true;
                obstacleMap[15, 17] = true;
                obstacleMap[15, 18] = true;
                obstacleMap[16, 3] = true;
                obstacleMap[16, 5] = true;
                obstacleMap[16, 10] = true;
                obstacleMap[16, 15] = true;
                obstacleMap[16, 17] = true;
                obstacleMap[17, 3] = true;
                obstacleMap[17, 5] = true;
                obstacleMap[17, 10] = true;
                obstacleMap[17, 15] = true;
                obstacleMap[17, 17] = true;
                obstacleMap[18, 5] = true;
                obstacleMap[18, 6] = true;
                obstacleMap[18, 10] = true;
                obstacleMap[18, 14] = true;
                obstacleMap[18, 15] = true;
                obstacleMap[19, 1] = true;
                obstacleMap[19, 19] = true;
            }
        }

        private Image CreateYellowImage()
        {
            Color pastelYellow = Color.FromArgb(255, 247, 153);
            Bitmap bitmap = new Bitmap(CellSize, CellSize);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(pastelYellow);
            }
            return bitmap;
        }

    }
}
