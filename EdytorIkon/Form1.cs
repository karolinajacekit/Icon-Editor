using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 
namespace IconEditor
{
    public partial class Form1 : Form
    {
        DBDataContext db = new DBDataContext();
        private const int pixelSize = 20;
        private Graphics graph;
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if(textBoxName.Text.Length > 3 && numericUpDownWidth.Value > 10 && numericUpDownHeight.Value > 10)
            {
                Icon i = new Icon();
                i.Name = textBoxName.Text;
                i.Height = (int)numericUpDownHeight.Value;
                i.Width = (int)numericUpDownWidth.Value;

                db.Icons.InsertOnSubmit(i);
                db.SubmitChanges();
                LoadIconsFromDatabase();
            }
            else
            {
                MessageBox.Show("Please provide valid data! (Name > 3 chars, Dimensions > 10)");
            }
        }
        private void LoadIconsFromDatabase()
        {
            listBoxIcons.Items.Clear();
            listBoxIcons.Items.AddRange(db.Icons.ToArray());
        }
        private void listBoxIcons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBoxIcons.SelectedItem is Icon)
            {
                Icon i = listBoxIcons.SelectedItem as Icon;
                pictureBox1.Image = new Bitmap(i.Width * pixelSize+1, i.Height * pixelSize+1);
                graph = Graphics.FromImage(pictureBox1.Image);
                graph.Clear(Color.LightYellow);
                RenderIconCanvas();
            }
        }
        private void RenderIconCanvas()
        {
            if (listBoxIcons.SelectedItem is Icon)
            {
                Icon i = listBoxIcons.SelectedItem as Icon;
                graph.Clear(Color.White);
                foreach(IconPoint ip in i.IconPoints)
                {
                    graph.FillRectangle(new SolidBrush(Color.FromArgb(ip.Color)), ip.X*pixelSize, ip.Y*pixelSize, pixelSize, pixelSize);
                }
                for(int x=0; x <= i.Height; x++)
                {
                    graph.DrawLine(new Pen(Color.Black), 0, x*pixelSize, i.Width*pixelSize, x*pixelSize);
                }
                for (int y = 0; y <= i.Width; y++)
                {
                    graph.DrawLine(new Pen(Color.Black), y * pixelSize, 0, y* pixelSize, i.Height * pixelSize);
                }
                pictureBox1.Refresh();
            }
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (listBoxIcons.SelectedItem is Icon)
            {
                Icon i = listBoxIcons.SelectedItem as Icon;
                if (e.Button == MouseButtons.Left)
                {
                    IconPoint ic;
                    ic = i.IconPoints.Where(x => x.X == e.X / pixelSize && x.Y == e.Y / pixelSize).FirstOrDefault();
   
                    if(ic==null)
                    {
                    ic = new IconPoint();
                    ic.X = e.X / pixelSize;
                    ic.Y = e.Y / pixelSize;
                        ic.Icon = i;
                    }

                    ic.Color = Color.Purple.ToArgb();
                    db.SubmitChanges();
                    RenderIconCanvas();
                }
            }
        }
    }
    partial class Icon
    {
        public override string ToString()
        {
            return $"{Name} ({Width} x {Height})";
        }
    }
}
