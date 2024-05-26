using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WMPLib;

namespace MouseCheeseGame
{
    public partial class Form1 : Form
    {
        WindowsMediaPlayer player = new WindowsMediaPlayer();

        private const int MapSize = 21;
        private const int CellSize = 35;

        private PictureBox[,] map;
        private PictureBox mouse, cheese, cat, trap, gameTitle, cheeseTitle;
        private Button playButton, rulesButton;

        private int mouseX, mouseY;
        private int cheeseX, cheeseY;
        private int catX, catY;
        private int trap1X, trap1Y, trap2X, trap2Y;

        private bool cheeseFound = false;
        private bool catFound = false;
        private bool trapFound = false; // добавим переменную для отслеживания того, найден ли сыр

        private Random random = new Random();

        private Timer catTimer;

        // Лабиринт
        private bool[,] obstacleMap = new bool[MapSize, MapSize];
        private int cheeseFoundCount = 0;

        public Form1()
        {
            InitializeComponent();

            InitializeMenu(); // Добавляем входное меню

            player.URL = @"Music\music.mp3";
            player.controls.play();
            player.settings.volume = 4;
            player.settings.setMode("loop", true);
        }

        private void InitializeMenu()
        {
            playButton = new Button();
            playButton.BackgroundImage = Image.FromFile(@"Images\playButton.png"); // Установка нового изображения для кнопки
            playButton.BackgroundImageLayout = ImageLayout.Stretch;
            playButton.Size = new Size(230, 80);
            playButton.Location = new Point(250, 400);
            playButton.FlatStyle = FlatStyle.Flat; // Убираем рамку
            playButton.FlatAppearance.BorderSize = 0; // Убираем границу
            playButton.Click += PlayButton_Click;
            this.Controls.Add(playButton);

            rulesButton = new Button();
            rulesButton.BackgroundImage = Image.FromFile(@"Images\rulesButton.png"); // Установка нового изображения для кнопки
            rulesButton.BackgroundImageLayout = ImageLayout.Stretch;
            rulesButton.Size = new Size(230, 80);
            rulesButton.Location = new Point(250, 500);
            rulesButton.FlatStyle = FlatStyle.Flat; // Убираем рамку
            rulesButton.FlatAppearance.BorderSize = 0; // Убираем границу
            rulesButton.Click += RulesButton_Click;
            this.Controls.Add(rulesButton);

            // Создаем PictureBox для отображения названия игры
            gameTitle = new PictureBox();
            gameTitle.Image = Image.FromFile(@"Images\gameTitle.png"); // Замените "Images\game_title.png" на путь к вашему изображению
            gameTitle.Size = new Size(550, 100); // Устанавливаем размер изображения
            gameTitle.SizeMode = PictureBoxSizeMode.StretchImage; // Масштабируем изображение по размеру PictureBox
            gameTitle.Location = new Point(80, 50); // Устанавливаем расположение изображения
            this.Controls.Add(gameTitle); // Добавляем PictureBox на форму

            cheeseTitle = new PictureBox();
            cheeseTitle.Image = Image.FromFile(@"Images\cheese.png");
            cheeseTitle.Size = new Size(250, 600);
            cheeseTitle.SizeMode = PictureBoxSizeMode.StretchImage;
            cheeseTitle.Location = new Point(250, 200);
            this.Controls.Add(cheeseTitle);

            // Устанавливаем размеры формы
            this.Width = 750; // Здесь задайте ширину в пикселях
            this.Height = 775; // Здесь задайте высоту в пикселях
            this.BackColor = Color.FromArgb(255, 247, 153);

            this.MinimumSize = new Size(this.Width, this.Height);
            this.MaximumSize = new Size(this.Width, this.Height);
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            // Здесь вызовите метод или откройте форму для начала игры
            StartGame();
        }

        private void RulesButton_Click(object sender, EventArgs e)
        {
            // Здесь отобразите правила игры (можно использовать MessageBox или отдельную форму)
            MessageBox.Show("Правила игры:\n\n1. Ваша задача - добраться до сыра, избегая кота и ловушек.\n2. Используйте клавиши W, A, S, D для перемещения мыши.\n3. Соберите 5 кусочков сыра, чтобы победить!");
        }

        private void StartGame()
        {
            playButton.Visible = false;
            rulesButton.Visible = false;
            gameTitle.Visible = false;
            cheeseTitle.Visible = false;

            GenerateObstacles(1);
            InitializeMap(); // Генерируем лабиринт препятствий
            PlaceObjects(1);

            // Инициализируем таймер для движения кота
            catTimer = new Timer();
            catTimer.Interval = 400; // Интервал в миллисекундах
            catTimer.Tick += CatTimer_Tick;
            catTimer.Start();
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
                obstacleMap[1, 1] = true;
                obstacleMap[1, 10] = true;
                obstacleMap[1, 19] = true;
                obstacleMap[2, 10] = true;
                obstacleMap[3, 2] = true;
                obstacleMap[3, 3] = true;
                obstacleMap[3, 10] = true;
                obstacleMap[3, 17] = true;
                obstacleMap[3, 18] = true;
                obstacleMap[4, 5] = true;
                obstacleMap[4, 6] = true;
                obstacleMap[4, 10] = true;
                obstacleMap[4, 14] = true;
                obstacleMap[4, 15] = true;
                obstacleMap[5, 9] = true;
                obstacleMap[5, 10] = true;
                obstacleMap[5, 11] = true;
                obstacleMap[7, 2] = true;
                obstacleMap[7, 3] = true;
                obstacleMap[7, 6] = true;
                obstacleMap[7, 7] = true;
                obstacleMap[7, 8] = true;
                obstacleMap[7, 9] = true;
                obstacleMap[7, 10] = true;
                obstacleMap[7, 11] = true;
                obstacleMap[7, 12] = true;
                obstacleMap[7, 13] = true;
                obstacleMap[7, 14] = true;
                obstacleMap[7, 17] = true;
                obstacleMap[7, 18] = true;
                obstacleMap[8, 2] = true;
                obstacleMap[8, 6] = true;
                obstacleMap[8, 14] = true;
                obstacleMap[8, 18] = true;
                obstacleMap[9, 2] = true;
                obstacleMap[9, 3] = true;
                obstacleMap[9, 6] = true;
                obstacleMap[9, 9] = true;
                obstacleMap[9, 11] = true;
                obstacleMap[9, 14] = true;
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
                        map[i, j].Image = Image.FromFile(@"Images\cheesebg.png");
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

        private void CatTimer_Tick(object sender, EventArgs e)
        {
            MoveCat();
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

        private void MoveCat()
        {
            // Создаем двумерный массив для хранения информации о посещенных клетках
            bool[,] visited = new bool[MapSize, MapSize];
            // Массив для хранения предков клеток, чтобы можно было восстановить путь
            (int, int)[,] previous = new (int, int)[MapSize, MapSize];

            // Очистка массивов
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    visited[i, j] = false;
                    previous[i, j] = (-1, -1);
                }
            }

            // Очередь для обхода клеток в ширину
            Queue<(int, int)> queue = new Queue<(int, int)>();

            // Начальная позиция кота
            queue.Enqueue((catX, catY));
            visited[catX, catY] = true;

            bool foundPath = false;

            while (queue.Count > 0)
            {
                // Извлекаем клетку из очереди
                var (currentX, currentY) = queue.Dequeue();

                // Если кот добрался до мыши, завершаем поиск
                if (currentX == mouseX && currentY == mouseY)
                {
                    foundPath = true;
                    break;
                }

                // Проверяем соседние клетки на доступность и добавляем их в очередь, если они не посещены
                int[] dx = { -1, 1, 0, 0 };
                int[] dy = { 0, 0, -1, 1 };

                for (int i = 0; i < 4; i++)
                {
                    int nextX = currentX + dx[i];
                    int nextY = currentY + dy[i];

                    // Проверяем, находится ли следующая клетка в пределах карты и не является ли препятствием, ловушкой или сыром
                    if (nextX >= 0 && nextX < MapSize && nextY >= 0 && nextY < MapSize &&
                        !obstacleMap[nextX, nextY] && !visited[nextX, nextY])
                    {
                        // Избегаем ловушки и сыр, но продолжаем искать пути
                        if ((nextX == trap1X && nextY == trap1Y) || (nextX == trap2X && nextY == trap2Y) ||
                            (nextX == cheeseX && nextY == cheeseY))
                        {
                            continue;
                        }

                        queue.Enqueue((nextX, nextY));
                        visited[nextX, nextY] = true;
                        previous[nextX, nextY] = (currentX, currentY);
                    }
                }
            }

            if (foundPath)
            {
                // Восстанавливаем путь от мыши к коту
                int targetX = mouseX;
                int targetY = mouseY;

                Stack<(int, int)> path = new Stack<(int, int)>();
                while (previous[targetX, targetY] != (-1, -1))
                {
                    path.Push((targetX, targetY));
                    (targetX, targetY) = previous[targetX, targetY];
                }

                // Двигаем кота по найденному пути
                if (path.Count > 0)
                {
                    var (nextX, nextY) = path.Peek();
                    MoveCatAlongPath(nextX, nextY);
                }
            }
            else
            {
                // Если кот не может найти путь к мыши, просто перемещаем его в случайном направлении
                MoveCatRandomly();
            }
        }

        private void MoveCatAlongPath(int targetX, int targetY)
        {
            // Определяем направление движения кота по горизонтали и вертикали
            int moveX = 0;
            int moveY = 0;

            // Находим направление движения кота
            if (targetX < catX && !obstacleMap[catX - 1, catY] && !(catX - 1 == trap1X && catY == trap1Y) && !(catX - 1 == trap2X && catY == trap2Y) && !(catX - 1 == cheeseX && catY == cheeseY))
                moveX = -1;
            else if (targetX > catX && !obstacleMap[catX + 1, catY] && !(catX + 1 == trap1X && catY == trap1Y) && !(catX + 1 == trap2X && catY == trap2Y) && !(catX + 1 == cheeseX && catY == cheeseY))
                moveX = 1;
            if (targetY < catY && !obstacleMap[catX, catY - 1] && !(catX == trap1X && catY - 1 == trap1Y) && !(catX == trap2X && catY - 1 == trap2Y) && !(catX == cheeseX && catY - 1 == cheeseY))
                moveY = -1;
            else if (targetY > catY && !obstacleMap[catX, catY + 1] && !(catX == trap1X && catY + 1 == trap1Y) && !(catX == trap2X && catY + 1 == trap2Y) && !(catX == cheeseX && catY + 1 == cheeseY))
                moveY = 1;

            // Перемещаем кота
            int newX = catX + moveX;
            int newY = catY + moveY;

            // Проверяем, чтобы новые координаты находились в пределах карты
            if (newX >= 0 && newX < MapSize && newY >= 0 && newY < MapSize && !obstacleMap[newX, newY] &&
                !(newX == trap1X && newY == trap1Y) && !(newX == trap2X && newY == trap2Y) && !(newX == cheeseX && newY == cheeseY))
            {
                // Перемещаем кота
                map[catX, catY].Image = null;
                catX = newX;
                catY = newY;
                map[catX, catY].Image = cat.Image;
            }

            // Проверяем столкновения после каждого хода кота
            CheckCollision();
        }

        private void MoveCatRandomly()
        {
            // Создаем список доступных направлений для перемещения
            List<(int, int)> availableDirections = new List<(int, int)>();

            // Проверяем доступность каждого направления и добавляем его в список, если клетка свободна и не содержит ловушки или сыра
            if (catX - 1 >= 0 && !obstacleMap[catX - 1, catY] && !(catX - 1 == trap1X && catY == trap1Y) && !(catX - 1 == trap2X && catY == trap2Y) && !(catX - 1 == cheeseX && catY == cheeseY))
                availableDirections.Add((-1, 0)); // Вверх
            if (catX + 1 < MapSize && !obstacleMap[catX + 1, catY] && !(catX + 1 == trap1X && catY == trap1Y) && !(catX + 1 == trap2X && catY == trap2Y) && !(catX + 1 == cheeseX && catY == cheeseY))
                availableDirections.Add((1, 0)); // Вниз
            if (catY - 1 >= 0 && !obstacleMap[catX, catY - 1] && !(catX == trap1X && catY - 1 == trap1Y) && !(catX == trap2X && catY - 1 == trap2Y) && !(catX == cheeseX && catY - 1 == cheeseY))
                availableDirections.Add((0, -1)); // Влево
            if (catY + 1 < MapSize && !obstacleMap[catX, catY + 1] && !(catX == trap1X && catY + 1 == trap1Y) && !(catX == trap2X && catY + 1 == trap2Y) && !(catX == cheeseX && catY + 1 == cheeseY))
                availableDirections.Add((0, 1)); // Вправо

            // Если доступны какие-либо направления, выбираем случайное направление и перемещаем кота
            if (availableDirections.Count > 0)
            {
                // Выбираем случайное доступное направление
                Random rnd = new Random();
                int randomIndex = rnd.Next(0, availableDirections.Count);
                var (moveX, moveY) = availableDirections[randomIndex];

                // Перемещаем кота
                int newX = catX + moveX;
                int newY = catY + moveY;

                map[catX, catY].Image = null;
                catX = newX;
                catY = newY;
                map[catX, catY].Image = cat.Image;

                // Проверяем столкновения после перемещения кота
                CheckCollision();
            }
            // Если все направления заблокированы, кот остается на месте
        }

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
                    ShowCongratulations();
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

            GenerateObstacles(1); // Генерируем новый лабиринт препятствий
            PlaceObjects(1);
        }

        private void ShowCongratulations()
        {
            // Создание окна с поздравлением
            Form congratsForm = new Form();
            congratsForm.Text = "Победа!";
            congratsForm.Size = new Size(400, 400);
            congratsForm.StartPosition = FormStartPosition.CenterScreen;

            // Создание PictureBox для отображения изображения
            PictureBox congratsPictureBox = new PictureBox();
            congratsPictureBox.Image = Image.FromFile(@"Images\mousecheese.png"); // Путь к изображению поздравления
            congratsPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            congratsPictureBox.Dock = DockStyle.Fill;

            // Добавление PictureBox на форму
            congratsForm.Controls.Add(congratsPictureBox);

            // Отображение окна с поздравлением
            congratsForm.ShowDialog();
        }


    }
}
