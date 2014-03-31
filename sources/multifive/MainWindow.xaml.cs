using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace multifive
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Case[,] board = new Case[8, 8];
        Rectangle[,] rectBoard = new Rectangle[8, 8];

        Case[] hand = new Case[8];
        List<Label> numberList = new List<Label>();
        List<Label> numberListForHand = new List<Label>();
        List<Case> groupList = new List<Case>();
        int size = 8;
        int boilUpTurn = 4;
        Random rnd = new Random();
        int score = 0;
        Label scoreLabel = new Label();
        Label turnsLeftLabel = new Label();
        Label gameOverLabel = new Label();
        Rectangle wible = new Rectangle();  // the wible is the block that indicates in which position the next block will be placed
        Canvas wibleCanvas = new Canvas();
        Label wibleLabel = new Label();
        Canvas handCanvas = new Canvas();
        Canvas boardCanvas = new Canvas();
        int selectedCase = 1000;
        bool gameOver = false;
        DispatcherTimer dispatcherTimer;
        DispatcherTimer groupRemovalTimer;
        int dontCheck = 2;
        bool firstGame = true;
        TextBox textBox;

        public MainWindow()
        {
            InitializeComponent();
        }

        //init board
        private void Canvas_Loaded_1(object sender, RoutedEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            boardCanvas = canvas;
            int size = 8;
            Random rnd = new Random();

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dispatcherTimer.Start();

            groupRemovalTimer = new System.Windows.Threading.DispatcherTimer();
            groupRemovalTimer.Tick += new EventHandler(checkGroups);
            groupRemovalTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            groupRemovalTimer.Start();



            initiateRandomly(canvas, size, rnd);


        }

        //fills the board with empty and colored blocks
        private void initiateRandomly(Canvas canvas, int size, Random rnd)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Case rect = new Case(canvas.Height, canvas.Width, size, rnd);

                    Canvas.SetLeft(rect.getRect(), i * canvas.Width / size);
                    Canvas.SetTop(rect.getRect(), j * canvas.Width / size);



                    board[i, j] = rect;
                    rectBoard[i, j] = rect.getRect();
                    canvas.Children.Add(rect.getRect());

                }

            }
        }

        //every tick, this method looks for a clock that should fall and moves it down by one square, it will also check if groups have been created
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            drop(boardCanvas, size);
        }

        private void checkGroups(object sender, EventArgs e)
        {
            if (dontCheck == 0)
            {
                for (int i = 0; i < size; i++)
                {
                    findGroup(i);
                }
                if (firstGame == true)
                {
                    textBox.Visibility = Visibility.Visible;
                }

                showScore();

            }
            dontCheck--;
        }

        private void Canvas_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            
            
        }

        //this method is called by the timer, it detects the blocks that can move
        private void drop(Canvas canvas, int size)
        {
            
                for (int i = size-1; i >= 0; i--)
                {
                    for (int j = size-1; j >= 0; j--)
                    {
                        if (!board[i, j].isEmpty() && j < size -1)
                        {
                         

                            
                            if (board[i, j+1].isEmpty())
                            {

                                swap(canvas, i, j, i, j+1);
                                dontCheck = 2;
                            }
                            
                        }
                    }
                }
        }

        //swaps two blocks, an empty block CAN be swaped. the swap is done graphically and logically(these are seperate parts)
        private void swap(Canvas canvas, int x1, int y1, int x2, int y2)
        {
            Canvas.SetLeft(board[x2,y2].getRect(), x1 * canvas.Width / size);
            Canvas.SetTop(board[x2, y2].getRect(), y1 * canvas.Width / size);


            Canvas.SetLeft(board[x1, y1].getRect(), (x2) * canvas.Width / size);
            Canvas.SetTop(board[x1, y1].getRect(), (y2) * canvas.Width / size);

            

            Rectangle tempRect = rectBoard[x1, y1];
            rectBoard[x1, y1] = rectBoard[x2, y2];
            rectBoard[x2, y2] = tempRect;

            Case tempCase = board[x1, y1];
            board[x1, y1] = board[x2, y2];
            board[x2, y2] = tempCase;

            

            drawLabelsForBoard();
        }

        //as the numbers and blocks are seperate, this will detect where to draw labels
        private void drawLabelsForBoard()
        {
            foreach(Label label in numberList)
            {
                boardCanvas.Children.Remove(label);
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (!board[i, j].isEmpty())
                    {
                        drawLabel(size, boardCanvas, i, j);

                    }
                }
            }
        }

        private void drawLabel(int size, Canvas canvas, int i, int j)
        {

            Label number = new Label();
            number.Content = board[i, j].getNumber().ToString();
            number.FontSize = 30;
            number.Foreground =  new SolidColorBrush(Colors.White);

            Canvas.SetLeft(number, 10 + i * canvas.Width / size);
            Canvas.SetTop(number, j * canvas.Height / size);

            numberList.Add(number);
            canvas.Children.Add(number);
        }


        private void drawLabelsForHand(int size, Canvas canvas)
        {
            foreach(Label label in numberListForHand)
            {
                canvas.Children.Remove(label);
            }
            for (int i = 0; i < size; i++)
            {
                Label number = new Label();
                number.Content = hand[i].getNumber().ToString();
                number.FontSize = 30;

                Canvas.SetLeft(number, 10 + i * canvas.Width / size);

                numberListForHand.Add(number);
                canvas.Children.Add(number);
            }
        }

        //init hand
        private void Canvas_Loaded_2(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            rnd = random;
            Canvas canvas = sender as Canvas;
            handCanvas = canvas;
            for (int i = 0; i < size; i++)
            {
                Case handCase = new Case(canvas.Height*6, canvas.Width, size, rnd, false);

                Canvas.SetLeft(handCase.getRect(), i * canvas.Width / size);
                Canvas.SetTop(handCase.getRect(), 0);

                hand[i] = handCase;
                canvas.Children.Add(handCase.getRect());
            }

            drawLabelsForHand(size, canvas);
        }




        //the event when the hand is clicked
        private void Canvas_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            int columnNumber = (int)(e.GetPosition(handCanvas).X / (handCanvas.Width / 8));
            
            selectHand(columnNumber);

            wible.Fill = hand[selectedCase].getRect().Fill;
            wible.Stroke = new SolidColorBrush(Colors.Black);

            wible.Visibility = Visibility.Visible;
            textBox.Visibility = Visibility.Collapsed;
            firstGame = false;

        }

        //this highlights the selected block in the hand
        private void selectHand(int columnNumber)
        {
            if (selectedCase != 1000)
            {
                hand[selectedCase].getRect().Stroke = new SolidColorBrush(Colors.Black);
            }
            hand[columnNumber].getRect().Stroke = new SolidColorBrush(Colors.Red);
            selectedCase = columnNumber;
        }
        
        //the event when the board is clicked
        private void Canvas_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            dispatcherTimer.IsEnabled = false;
            if (gameOver == false)
            {
                int columnNumber = (int)(e.GetPosition(boardCanvas).X / (boardCanvas.Width / 8));


                if (selectedCase != 1000)
                {
                    copyRectangleInHandAt(columnNumber);
                    drawLabelsForBoard();

                    dispatcherTimer.IsEnabled = true;

                    Random rnd = new Random();
                    hand[selectedCase] = new Case(boardCanvas.Height, boardCanvas.Width, size, rnd, false);
                    hand[selectedCase].getRect().Stroke = new SolidColorBrush(Colors.Red);
                    wible.Fill = hand[selectedCase].getRect().Fill;

                    Canvas.SetLeft(hand[selectedCase].getRect(), selectedCase * boardCanvas.Width / size);
                    Canvas.SetTop(hand[selectedCase].getRect(), 0);

                    handCanvas.Children.Add(hand[selectedCase].getRect());
                    drawLabelsForHand(size, handCanvas);

               
                    if (boilUpTurn == 0)
                    {
                        boilUp();
                        score += 50;
                        showScore();
                        boilUpTurn = 4;
                    }
                    else
                    {
                        boilUpTurn--;
                    }

                    updateTurnsLeft();
                }
            }
            else
            {
                initiateRandomly(boardCanvas, size, rnd);
                gameOverLabel.Visibility = Visibility.Collapsed;
                gameOver = false;
                dispatcherTimer.IsEnabled = true;
                drawLabelsForBoard();
                score = 0;
                wible.Visibility = Visibility.Visible;
                wibleLabel.Visibility = Visibility.Visible;
            }
            
            
        }

        private void showScore()
        {
            scoreLabel.Content = "Score: " + score.ToString();
        }

        private void calculateScore(int addition)
        {
            score += addition;
        }

        private void updateTurnsLeft()
        {
            turnsLeftLabel.Content = (boilUpTurn +1).ToString() + " turns until boil up";
        }

        //the method that checks if groups have been formed
        private void findGroup(int x)
        {
            dispatcherTimer.IsEnabled = false;
            groupList.Clear();

            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    board[i, k].searched = false;
                }
            }

            int j = 0;
            while (board[x, j].isEmpty() && j<size-1)
            {
                j++;
            }
            searchSimilarNeighbors(x,j);


            int addition = 0;
            foreach (Case number in groupList)
            {
                addition += number.getNumber();
            }

            if (addition % 5 == 0)
            {
                foreach (Case square in groupList)
                {
                    empty(square);
                }

                dispatcherTimer.IsEnabled = true;
                calculateScore(addition);

            }
            dispatcherTimer.IsEnabled = true;
            
        }

        //this method sets a block so that it will return isEmpty == true
        private void empty(Case square)
        {
            square.getRect().Fill = new SolidColorBrush(Colors.White);
            square.takeColor();
        }

        //recursive method that checks if the current block forms a group with adjacent blocks
        private void searchSimilarNeighbors(int x, int y)
        {

            board[x, y].searched = true;
            groupList.Add(board[x, y]);
            if (x < 7 && !board[x+1,y].searched && board[x,y].getRect().Fill.ToString() == board[x+1,y].getRect().Fill.ToString())
            {
                searchSimilarNeighbors(x+1, y);
            }
            if (y < 7 && !board[x, y+1].searched && board[x,y].getRect().Fill.ToString() == board[x,y+1].getRect().Fill.ToString())
            {
                searchSimilarNeighbors(x, y+1);
            }
            if (x > 0 && !board[x - 1, y].searched && board[x, y].getRect().Fill.ToString() == board[x-1, y].getRect().Fill.ToString())
            {
                searchSimilarNeighbors(x-1, y);
            }
            if (y > 0 && !board[x, y - 1].searched && board[x, y].getRect().Fill.ToString() == board[x, y-1].getRect().Fill.ToString())
            {
                searchSimilarNeighbors(x, y-1);
            }

        }

        private void copyRectangleInHandAt(int columnNumber)
        {
            rectBoard[columnNumber, 0].Fill = hand[selectedCase].getRect().Fill;
            board[columnNumber, 0].giveColor();
            board[columnNumber, 0].setNumber(hand[selectedCase].getNumber());
        }

        //this pushes all the blocks up one. It will check for game lost and call for filling the bottom line
        private void boilUp()
        {
            drop(boardCanvas, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (!board[i, j].isEmpty())
                    {
                        if (j > 0)
                        {
                            swap(boardCanvas, i, j, i, j - 1);
                        }
                        else
                        {
                            gameLost();
                        }
                    }
                }
            }
            fillBottomLine();
            drawLabelsForBoard();
        }


        private void gameLost()
        {
            
            gameOverLabel.Visibility = Visibility.Visible;
            gameOver = true;
            wible.Visibility = Visibility.Collapsed;
            wibleLabel.Visibility = Visibility.Collapsed;
        }

        //fills bottom line after all the blocks have been pushed up
        private void fillBottomLine()
        {
            for (int i = 0; i < size; i++)
            {

                board[i, size-1].giveColor();
                rectBoard[i, size - 1].Fill = board[i, size - 1].getRandomColor(rnd);
            }
        }

        //turns left label
        private void Label_Loaded_1(object sender, RoutedEventArgs e)
        {
            Label label = sender as Label;
            turnsLeftLabel = label;
            label.FontSize = 20;
            label.Content = "5 turns until boil up";
        }

        //score
        private void Label_Loaded_2(object sender, RoutedEventArgs e)
        {
            Label label = sender as Label;
            scoreLabel = label;
            label.FontSize = 40;
            label.Content = "Score: 0";
        }

        private void Canvas_MouseMove_1(object sender, MouseEventArgs e)
        {
            
        }

        //makes the wible follow the mouse position
        private void setWiblePosition(double position)
        {
            Canvas.SetLeft(wible, position - boardCanvas.Width/16 - 35);

            Label number = new Label();
            number.FontSize = 30;
            number.Foreground = new SolidColorBrush(Colors.White);
            if (selectedCase != 1000)
            {
                wibleCanvas.Children.Remove(wibleLabel);
                number.Content = hand[selectedCase].getNumber().ToString();

                Canvas.SetLeft(number, -25 + position - boardCanvas.Width / 16);
                wibleCanvas.Children.Add(number);
            }
            wibleLabel = number;
        }

        //wible canvas
        private void Canvas_Loaded_3(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            Canvas canvas = sender as Canvas;
            wibleCanvas = canvas;
            Case wibleCase = new Case(canvas.Height*6, canvas.Width, size, rnd, false);
            wible = wibleCase.getRect();
            Canvas.SetLeft(wibleCase.getRect(), 0);
            Canvas.SetTop(wibleCase.getRect(), 0);
            wible.Fill = new SolidColorBrush(Colors.White);
            wible.Stroke = new SolidColorBrush(Colors.White);
            wible.Visibility = Visibility.Collapsed;
            canvas.Children.Add(wibleCase.getRect());
        }

        private void Grid_MouseMove_1(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            setWiblePosition(e.GetPosition(grid).X - boardCanvas.Width/(size*2));
        }

        private void Label_Loaded_3(object sender, RoutedEventArgs e)
        {
            Label label = sender as Label;
            gameOverLabel = label;
            gameOverLabel.Visibility = Visibility.Collapsed;
        }

        private void TextBox_Loaded_1(object sender, RoutedEventArgs e)
        {
            TextBox box = sender as TextBox;
            box.Visibility = Visibility.Collapsed;
            textBox = box;
        }


    }
}
