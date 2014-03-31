using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Shapes;
using System.Windows.Media;

namespace multifive
{
    class Case
    {
        int limitNumber = 7;
        int number;
        public Rectangle rect;
        public Brush brush;
        int notInitiated;
        public bool searched;
        public int colorInt;

        Color[] input = { Colors.Firebrick, Colors.MediumAquamarine, Colors.Orange, Colors.RoyalBlue };


        //instantiates a block with a random color, including white(empty)
        public Case(double height, double width, int size, Random rnd)
        {
            rect = new Rectangle();
            number = rnd.Next(1, limitNumber);
            searched = false;

            List<Color> colors = new List<Color>(input);

            notInitiated = rnd.Next(1, 3);
            prepareCase(height, width, size, rnd, colors);

        }

        //creates an white block or a colored block
        public Case(double height, double width, int size, Random rnd, bool empty)
        {
            rect = new Rectangle();
            number = rnd.Next(1, limitNumber);
            searched = false;

            List<Color> colors = new List<Color>(input);

            if (empty == true)
            {
                notInitiated = 1;
            }
            else
            {
                notInitiated = 0;
            }

            prepareCase(height, width, size, rnd, colors);
            

       

        }

        //prepares the square to be drawn on the board
        private void prepareCase(double height, double width, int size, Random rnd, List<Color> colors)
        {
            rect.Height = height / size;
            rect.Width = width / size;

            if (notInitiated != 1)
            {
                rect.Height = height / size;
                rect.Width = width / size;

                colorInt = rnd.Next(0, colors.Count);
                brush = new SolidColorBrush(colors[colorInt]);
            }
            else
            {
                colorInt = 1000;
                brush = new SolidColorBrush(Colors.White);
            }

            rect.Fill = brush;
            rect.Stroke = new SolidColorBrush(Colors.Black);

        }

        public Rectangle getRect()
        {
            return rect;
        }

        public bool isEmpty()
        {
            return (notInitiated == 1) ? true : false;
        }


        internal void giveColor()
        {
            notInitiated = 0;
        }

        internal void takeColor()
        {
            notInitiated = 1;
        }

        public int getNumber()
        {
            return number;
        }

        public void setNumber(int change)
        {
            number = change;
        }


        internal Brush getRandomColor(Random rnd)
        {

            List<Color> colors = new List<Color>(input);
            

            return new SolidColorBrush(colors[rnd.Next(0, colors.Count)]);
        }
    }
}
