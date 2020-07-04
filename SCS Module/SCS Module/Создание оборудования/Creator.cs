using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Drawing.Imaging;

namespace SCS_Module
{
    public partial class Creator : Form
    {
        public Creator()
        {
            InitializeComponent();
            menu = new ContextMenu(new MenuItem[] { new MenuItem("Удалить", hadler) });
            menu2 = new ContextMenu(new MenuItem[] { new MenuItem("Удалить", handler2) });
            buttons_for_files  = new Button[] { button11, button12, button13, button14 };
            buttons_for_links = new Button[] { button3, button4, button5, button6 };
            radioButton2.Checked = true;
        }
       
        void status()
        {
            bool started = false;
            label7.Text = "";
            //name
            if (string.IsNullOrEmpty(textBox1.Text.Trim().Replace(" ", "")))
            {
                started = true;
                label7.Text = "Отсутствует: Имя";
            }
            //properties
            bool True = true;
            try
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells[0].Value.ToString()))
                        if (dataGridView1.Rows[i].Cells[1].Value==null || string.IsNullOrEmpty(dataGridView1.Rows[i].Cells[1].Value.ToString()))
                        {
                            True = false;
                            break;
                        }

            }
            }
            catch (Exception ex) { }
            if (!True)
            {
               if(started) label7.Text += ", Один из параметров";
                else label7.Text = "Отсутствует: Один из параметров";
                started = true;
            }




            if (started) label7.ForeColor = Color.DarkRed;
            else
            {
                label7.Text = "Все отлично";
                label7.ForeColor = Color.DarkGreen;
                
            }
        }


        Equipment result = null;
        List<Equipment.Compatibility> list = new List<Equipment.Compatibility>();
        bool aaaa = false;
        int index;
        Bitmap[] images = new Bitmap[4] { null, null, null, null };
        List<Image> photos = new List<Image>();
        Equipment forFamilyId = null;
        Button for_photos;
        ContextMenu menu, menu2;
        PictureBox[] for_plans = new PictureBox[4] { null, null, null, null };
        Button[] buttons_for_files, buttons_for_links;
        Equipment.VectorPic[] vectorImages = new Equipment.VectorPic[] { null, null, null, null }, vectorImagesBuff = new Equipment.VectorPic[] { null, null, null, null };
        private void button2_Click(object sender, EventArgs e)
        {
            CreateInterface c = new CreateInterface(this);
            c.ShowDialog();
            status();
        }
 
        public void add_interface(Equipment.Compatibility com)
        {
            list.Add(com);
            Restore();
            status();
        }
     
        TableLayoutPanel CreateInterfacePanel(Equipment.Compatibility com)
        {
            TableLayoutPanel result = new TableLayoutPanel();
            if (aaaa) result.BackColor = Color.LightGreen;
            else result.BackColor = Color.LightSalmon;
            aaaa = !aaaa;
            result.Dock = DockStyle.Fill;
            result.RowCount = 4;
            result.ColumnCount = 1;
            for (int i = 0; i < 4; i++) result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            result.Controls.AddRange(new Control[] {
                new Label() { Text = com.interfaceType.name, TextAlign = ContentAlignment.MiddleCenter },
                new Label() { Text=com.count+" Шт.", TextAlign = ContentAlignment.MiddleCenter},
                new Button() { Text="Изменить", Width = 300, Height = 80, Name=com.interfaceType.id.ToString() },
                new Button() { Text="Удалить", Width = 300, Height = 80, Name =com.interfaceType.id.ToString() }
            });
            result.Controls[2].Click += change_click;
            result.Controls[3].Click += delete_click;
            foreach (var i in result.Controls) ((Control)i).Anchor = AnchorStyles.None;
            return result;
        }

        private void delete_click(object sender, EventArgs e)
        {
            Button pressed = (Button)sender;
            list.RemoveAll(x => x.interfaceType.id == Convert.ToInt32(pressed.Name));
            Restore();
            status();
        }
       public void replacer(Equipment.Compatibility com)
        {
            list.RemoveAt(index);
            list.Insert(index, com);
            Restore();
            status();
        }
    
        private void change_click(object sender, EventArgs e)
        {
            Button pressed = (Button)sender;
            index = list.FindIndex(x => x.interfaceType.id == Convert.ToInt32(pressed.Name));
            new CreateInterface(this, list[index]).ShowDialog();
            status();
        }
        void Restore()
        {
            aaaa = false;
            Button b = button2;
            while (tableLayoutPanel3.Controls.Count!=0)
                tableLayoutPanel3.Controls.RemoveAt(0);
            tableLayoutPanel3.ColumnStyles.Clear();
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tableLayoutPanel3.RowStyles.Clear();
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.RowCount = list.Count / 3 + 1;
            tableLayoutPanel3.Height = 201 * tableLayoutPanel3.RowCount + 1;
            int i=0, j=0, n=0;
            for (i=0, n=0;i< tableLayoutPanel3.RowCount & n < list.Count; i++)
            {
                tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));
                for (j = 0; j < 3 & n<list.Count; j++,n++)
                {
                    tableLayoutPanel3.Controls.Add(CreateInterfacePanel(list[n]));
                }
            }
            i--;
            tableLayoutPanel3.Controls.Add(b,j,i);
            Refresh();
        }
        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        { 
        }



        Bitmap Normalizer(Bitmap b, bool color_replace = true)
        {
            Rectangle rect = new Rectangle();
            bool stopper = false;
            for (int i = 0; i < b.Width; i++)
            {
                if (stopper) break;
                for (int j = 0; j < b.Height; j++)
                    if (b.GetPixel(i, j) != Color.FromArgb(255, 255, 255, 255))
                    {
                        rect.X = i;
                        stopper = true;
                        break;
                    }
            }
            stopper = false;
            for (int i = 0; i < b.Height; i++)
            {
                if (stopper) break;
                for (int j = 0; j < b.Width; j++)
                    if (b.GetPixel(j, i) != Color.FromArgb(255, 255, 255, 255))
                    {
                        rect.Y = i;
                        stopper = true;
                        break;
                    }
            }
            stopper = false;
            for (int i = b.Width - 1; i > -1; i--)
            {
                if (stopper) break;
                for (int j = b.Height - 1; j > -1; j--)
                    if (b.GetPixel(i, j) != Color.FromArgb(255, 255, 255, 255))
                    {
                        rect.Width = i - rect.X + 1;
                        stopper = true;
                        break;
                    }
            }
            stopper = false;
            for (int i = b.Height - 1; i > -1; i--)
            {
                if (stopper) break;
                for (int j = b.Width - 1; j > -1; j--)
                    if (b.GetPixel(j, i) != Color.FromArgb(255, 255, 255, 255))
                    {
                        rect.Height = i - rect.Y + 1;
                        stopper = true;
                        break;
                    }
            }
            Bitmap bnew = new Bitmap(rect.Width, rect.Height);
            if (color_replace)
            {
                for (int i = rect.X, ii = 0; i < rect.X + rect.Width; i++, ii++)
                    for (int j = rect.Y, jj = 0; j < rect.Y + rect.Height; j++, jj++)
                    {
                        if (b.GetPixel(i, j) != Color.FromArgb(255, 255, 255, 255))
                            bnew.SetPixel(ii, jj, Color.Black);
                        else
                            bnew.SetPixel(ii, jj, Color.White);
                    }
            }
            else
            {
                for (int i = rect.X, ii = 0; i < rect.X + rect.Width; i++, ii++)
                    for (int j = rect.Y, jj = 0; j < rect.Y + rect.Height; j++, jj++)
                    {
                        bnew.SetPixel(ii, jj, b.GetPixel(i, j));
                    }
            }
            return bnew;
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            result = new Equipment();
            result.id = Convert.ToInt32(textBox3.Text);
            result.name = textBox1.Text;
            result.description = textBox2.Text;
            if (forFamilyId == null)
            {
                try
                {
                    result.bytedFile = File.ReadAllBytes(label12.Text);
                }
                catch (Exception ex) { }
            }
            else
            {
                result.familyID = forFamilyId.familyID;
            }
            result.compatibilities = list;
            Dictionary<string, string> slov = new Dictionary<string, string>();
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                slov.Add(dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString());
            result.properties = slov;
            result.PICS = new List<string>();
            MemoryStream ms;
            for (int i = 0; i < photos.Count; i++)
            {
                if (photos[i] == null)
                    break;
                ms = new MemoryStream();
                photos[i].Save(ms, ImageFormat.Png);
                result.PICS.Add(Convert.ToBase64String(ms.ToArray()));
            }
            try
            {
                //result.inBox_v = Equipment.vectorization(images[0]);
                //result.inConnectionScheme_v = Equipment.vectorization(images[1]);
                //result.inStructural_v = Equipment.vectorization(images[2]);
                //result.placementScheme_v = Equipment.vectorization(images[3]);
                result.inBox = vectorImages[0];
                result.inConnectionScheme = vectorImages[1];
                result.inStructural = vectorImages[2];
                result.inPlacementScheme = vectorImages[3];
            }
            catch (Exception ex) { }

            result.isWire = radioButton1.Checked;
            result.isBox = radioButton3.Checked;
            result.isInBox = radioButton5.Checked;
            Internet.send(new ExecutiveToServer() { equipment = result, requestType = RequestType.AddEquipment });
            if (Internet.receive().success)
            {
                DialogResult = DialogResult.OK;
                MessageBox.Show("Все прошло успешно");
                Close();
            }
            status();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            FamilySearch FS = new FamilySearch();
            if (FS.DialogResult == DialogResult.OK)
            {
                forFamilyId = FS.equipment;
                label12.Text = $"Выбрано семейство оборудования '{forFamilyId.name}'";
            }
        }
        Bitmap squarezer(Bitmap b, ProgressBar bar)
        {
            Bitmap res1 = Normalizer(b, false);
            if (res1.Width > res1.Height)
            {
                Bitmap res2 = new Bitmap(res1.Width, res1.Width);
                //for (int i = 0; i < (res1.Width - res1.Height) / 2 - 1; i++)
                //    for (int j = 0; j < res2.Width; j++)
                //        res2.SetPixel(j, i, Color.White);
                //for (int i = res2.Height - 1; i > (res1.Width - res1.Height) / 2 - 1 + res1.Height; i--)
                //    for (int j = 0; j < res2.Width; j++)
                //        res2.SetPixel(j, i, Color.White);
                double progress = 0, 
                    step = 1000 / (1.0*((res1.Width - res1.Height) / 2 - 1 + res1.Height - ((res1.Width - res1.Height) / 2 - 1)));
                for (int i = (res1.Width - res1.Height) / 2 - 1; i < (res1.Width - res1.Height) / 2 - 1 + res1.Height; i++)
                {
                    for (int j = 0; j < res2.Width; j++)
                        res2.SetPixel(j, i, res1.GetPixel(j, i - ((res1.Width - res1.Height) / 2 - 1)));
                    progress += step;
                    bar.Value = (int)progress;
                    bar.Refresh();
                }
                return res2;
            }
            else if (res1.Width < res1.Height)
            {
                Bitmap res2 = new Bitmap(res1.Height, res1.Height);
                //for (int i = 0; i < (res1.Height - res1.Width) / 2 - 1; i++)
                //    for (int j = 0; j < res2.Height; j++)
                //        res2.SetPixel(i, j, Color.White);
                //for (int i = res2.Width - 1; i > (res1.Height - res1.Width) / 2 - 1 + res1.Width; i--)
                //    for (int j = 0; j < res2.Height; j++)
                //        res2.SetPixel(i, j, Color.White);
                double progress = 0, step = 1000 / (1.0 * ((res1.Height - res1.Width) / 2 - 1 + res1.Width - ((res1.Height - res1.Width) / 2 - 1)));
                for (int i = (res1.Height - res1.Width) / 2 - 1; i < (res1.Height - res1.Width) / 2 - 1 + res1.Width; i++)
                {
                    for (int j = 0; j < res2.Height; j++)
                        res2.SetPixel(i, j, res1.GetPixel(i - ((res1.Height - res1.Width) / 2 - 1), j));
                    progress += step;
                    bar.Value = (int)progress;
                    bar.Refresh();
                }
                    return res2;
            }
            else return res1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            for_photos = button1;
            tableLayoutPanel5.Controls.Remove(button1);
            ProgressBar bar = new ProgressBar();
            bar.Minimum = 0;bar.Maximum = 1000;
            bar.Anchor = AnchorStyles.None;
            tableLayoutPanel5.Controls.Add(bar);
            if (f.ShowDialog() == DialogResult.OK)
            {
                Image img = new Bitmap(squarezer(new Bitmap(Image.FromFile(f.FileName)), bar), new Size(500, 500));
                photos.Add(img);
                bulid_photos();

            }
            status();
        }
        

        private void hadler(object sender, EventArgs e)
        {
            int fordel = Convert.ToInt32(((PictureBox)((MenuItem)sender).GetContextMenu().SourceControl).Name);
            photos.RemoveAt(fordel);
            bulid_photos();
        }

        void bulid_photos()
        {
            tableLayoutPanel5.Controls.Clear();
            int index = 0;
            foreach(var img in photos)
            {
                PictureBox box = new PictureBox();
                tableLayoutPanel5.Controls.Add(box,index, 0);
                box.SizeMode = PictureBoxSizeMode.Zoom;
                box.Dock = DockStyle.Fill;
                box.Image = img;
                box.Refresh();
                box.ContextMenu = menu;
                box.Name = index.ToString();
                index++;
            }
            if (photos.Count != 4)
            {
                tableLayoutPanel5.Controls.Add(for_photos);
                tableLayoutPanel5.SetColumn(for_photos, tableLayoutPanel5.Controls.Count - 1);
            }

        }

        private void tableLayoutPanel2_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.LightSteelBlue, e.CellBounds);
        }
       
        //private void button3_Click(object sender, EventArgs e)
        //{
        //    pickSetter(0);
        //    status();
        //}

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    pickSetter(1); status();
        //}
        //private void button5_Click(object sender, EventArgs e)
        //{
        //    pickSetter(2); status();
        //}
        //private void button6_Click(object sender, EventArgs e)
        //{
        //    pickSetter(3); status();
        //}
        //void pickSetter(int num)
        //{
        //    if (!Clipboard.ContainsImage()) return;
        //    for_plans[num] = new PictureBox();
        //    for_plans[num].Name = num.ToString();
        //    for_plans[num].Dock = DockStyle.Fill;
        //    for_plans[num].ContextMenu = menu2;
        //    tableLayoutPanel6.Controls.Remove(buttons_for_files[num]);
        //    tableLayoutPanel6.Controls.Add(for_plans[num], num, 1);
        //    for_plans[num].SizeMode = PictureBoxSizeMode.Zoom;
        //    try
        //    {
        //        Bitmap b = Normalizer(new Bitmap(Clipboard.GetImage()));
        //        double k = Math.Sqrt(50000 / (b.Width * b.Height * 1.0));

        //        images[num] = new Bitmap(b, new Size((int)(b.Width * k), (int)(b.Height * k)));

        //        vectorImages[num] = Equipment.vectorization(images[num]);

        //        Bitmap bit = new Bitmap(for_plans[num].Width, for_plans[num].Height);
        //        Graphics gg = Graphics.FromImage(bit);
        //        gg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //        float koefX = for_plans[num].Width / (images[num].Width*1.0f), 
        //            koefY = for_plans[num].Height /( images[num].Height*1.0f);
        //        foreach (var i in vectorImages[num].hatching)
        //        {
        //            List<Point> list1 = new List<Point>();
        //            List<Point> list2 = new List<Point>();
        //            foreach (var j in i[0])
        //                list1.Add(new Point() { X =(int) (j.X*koefX), Y = (int)(j.Y *koefY) });
        //            if (i.Count == 1)
        //            {
        //              list1.Add(list1[0]);
        //                gg.FillPolygon(Brushes.Black, list1.ToArray());
        //            }
        //            else
        //            {              
        //                foreach (var j in i[1])
        //                    list2.Add(new Point() { X = (int)(j.X * koefX), Y = (int)(j.Y * koefY) });
        //                list2.Add(list2[0]);
        //                gg.FillPolygon(Brushes.White, list2.ToArray());
        //            }
        //        }
        //        foreach (var i in vectorImages[num].polyLines)
        //        {
        //            List<Point> list1 = new List<Point>();
        //            foreach (var j in i)
        //                list1.Add(new Point() { X = (int)(j.X * koefX), Y = (int)(j.Y * koefY) });
        //            gg.DrawLines(Pens.Blue, list1.ToArray());
        //        }
        //        for_plans[num].Image = bit;
        //    }
        //    catch (Exception ex) { }
        //    for_plans[num].Refresh();
        //}


        private void bull(object sender, EventArgs e)
        {
            status();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            status();
        }

        private void WireNotWire(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            if (radioButton1.Checked)
            {
                tableLayoutPanel10.Enabled = false;
                tableLayoutPanel11.Enabled = false;

                radioButton4.Checked = true;
                radioButton6.Checked = true;

                tableLayoutPanel8.Enabled = false;
                tableLayoutPanel6.Enabled = false;

                buttons_for_files[0].Enabled = true;
                buttons_for_links[0].Enabled = true;

            }
            else
            {
                tableLayoutPanel10.Enabled = true;
                tableLayoutPanel11.Enabled = true;

                tableLayoutPanel8.Enabled = true;
                tableLayoutPanel6.Enabled = true;

                buttons_for_files[0].Enabled = false;
                buttons_for_links[0].Enabled = false;
            }
        }

        private void BoxNotBox(object sender, EventArgs e)
        {
            if (!tableLayoutPanel10.Enabled) return;
            if (radioButton3.Checked)
            {
                tableLayoutPanel11.Enabled = false;
                radioButton6.Checked = true;
                dataGridView1.Rows.Clear();
                //dataGridView1.Rows.Add(new object[] { "Ширина (мм)", "" });
                //dataGridView1.Rows.Add(new object[] { "Высота (мм)", "" });
                //dataGridView1.Rows.Add(new object[] { "Глубина (мм)", "" });
                //dataGridView1.Rows.Add(new object[] { "Вес (кг)", "" });
                dataGridView1.Rows.Add(new object[] { "Количество юнитов (шт)", "" });
                //dataGridView1.Rows.Add(new object[] { "Ширина юнитов (дюйм)", "" });
                for (int i = 0; i < 3; i++) {
                    buttons_for_files[i].Enabled = false;
                    buttons_for_links[i].Enabled = false;
                }
            }
            else
            {
                dataGridView1.Rows.Clear();
                for (int i = 0; i < 3; i++)
                {
                    buttons_for_files[i].Enabled = true;
                    buttons_for_links[i].Enabled = true;
                }
                tableLayoutPanel11.Enabled = true;
            }
        }
        private void inBoxNotinBox(object sender, EventArgs e)
        {
            if (!tableLayoutPanel11.Enabled) return;
            if (radioButton5.Checked)
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Rows.Add(new object[] { "Занимаемых юнитов (шт)", "" });
                //dataGridView1.Rows.Add(new object[] { "Мин. ширина юнита (дюйм)", "" });
                //dataGridView1.Rows.Add(new object[] { "Макс. ширина юнита (дюйм)", "" });
                for (int i = 0; i < 4; i++)
                {
                    buttons_for_files[i].Enabled = true;
                    buttons_for_links[i].Enabled = true;
                }
                buttons_for_files[2].Enabled = false;
                buttons_for_links[2].Enabled = false;

                buttons_for_files[3].Enabled = false;
                buttons_for_links[3].Enabled = false;
            }
            else
            {
                buttons_for_files[2].Enabled = true;
                buttons_for_links[2].Enabled = true;


                buttons_for_files[3].Enabled = true;
                buttons_for_links[3].Enabled = true;
                dataGridView1.Rows.Clear();
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            if (f.ShowDialog() == DialogResult.OK)
            {
                forFamilyId = null;
                label12.Text = f.FileName;
            }
            status();
        }
        void rebuild_images()
        {
            for (int num = 0; num < 4; num++)
            {
                if (vectorImages[num] == null) continue;
                vectorImagesBuff[num] = vectorImages[num].copy();
                if (for_plans[num].Width == 0) break;
                //calculating width and height
                // for_plans[num].Width = 1000; for_plans[num].Height = 900;

                int BoxWidth = for_plans[num].Width - 2, BoxHeight = for_plans[num].Height - 2;

                    double LeftUpperX = 0, LeftUpperY = 0,
                RightLowerX = 0, RightLowerY = 0;

                calculateBorders(ref LeftUpperX, ref LeftUpperY, ref RightLowerX, ref RightLowerY, vectorImagesBuff[num], true);
                //
                  Bitmap bit = new Bitmap(for_plans[num].Width, for_plans[num].Height);
                //Bitmap bit = new Bitmap(1000, 900);
                Graphics gg = Graphics.FromImage(bit);
                gg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                float proportion = 1;
                //first alignment
                if (RightLowerX - LeftUpperX > RightLowerY - LeftUpperY)
                    proportion = Convert.ToSingle((RightLowerX - LeftUpperX) / (BoxWidth*1.0f));
                else
                    proportion = Convert.ToSingle((RightLowerY - LeftUpperY) / (BoxHeight*1.0f));
                //applying
                foreach (var i in vectorImagesBuff[num].polyLines)
                    for (int j = 0; j < i.Count; j++)
                        i[j].divide(proportion);
                //foreach (var i in vectorImagesBuff[num].arcs)
                //    i.divide(proportion);
                foreach (var i in vectorImagesBuff[num].circles)
                    i.divide(proportion);
                //foreach (var i in vectorImagesBuff[num].hatching)
                //    i.divide(proportion);
                //second alignment
                calculateBorders(ref LeftUpperX, ref LeftUpperY, ref RightLowerX, ref RightLowerY, vectorImagesBuff[num], true);
                proportion = 1;
                if (RightLowerX - LeftUpperX > RightLowerY - LeftUpperY)
                {
                    if (RightLowerY - LeftUpperY > BoxHeight)
                        proportion = Convert.ToSingle((RightLowerY - LeftUpperY) / (BoxHeight*1.0f));
                }
                else
                {
                    if (RightLowerX - LeftUpperX > BoxWidth)
                        proportion = Convert.ToSingle((RightLowerX - LeftUpperX) / (BoxWidth * 1.0f));
                }
                if (proportion != 1)
                {
                    //applying
                    foreach (var i in vectorImagesBuff[num].polyLines)
                        for (int j = 0; j < i.Count; j++)
                            i[j].divide(proportion);
                    //foreach (var i in vectorImagesBuff[num].arcs)
                    //    i.divide(proportion);
                    foreach (var i in vectorImagesBuff[num].circles)
                        i.divide(proportion);
                    //foreach (var i in vectorImagesBuff[num].hatching)
                    //    i.divide(proportion);
                }


                //painting
                LeftUpperX =0; LeftUpperY = 0;
                RightLowerX = 0; RightLowerY = 0;
                calculateBorders(ref LeftUpperX, ref LeftUpperY, ref RightLowerX, ref RightLowerY, vectorImagesBuff[num], true);


                float width = Convert.ToSingle(RightLowerX - LeftUpperX), height = Convert.ToSingle(RightLowerY - LeftUpperY);
                int xOffset = (int)(BoxWidth - width) / 2+1,
                    yOffset = (int)(BoxHeight - height) / 2+1;


                //foreach (var i in vectorImagesBuff[num].hatching)
                //{

                //}
                foreach (var i in vectorImagesBuff[num].polyLines)
                {
                    List<Point> list1 = new List<Point>();
                    foreach (var j in i)
                        list1.Add(new Point() { X = (int)(j.X + xOffset), Y = (int)(j.Y + yOffset) });
                    gg.DrawLines(Pens.Blue, list1.ToArray());
                }
                foreach (var i in vectorImagesBuff[num].circles)
                {
                    gg.DrawEllipse(Pens.Black, (float)i.center.X + xOffset - (float)i.radius, (float)i.center.Y + yOffset - (float)i.radius, (float)i.radius * 2, (float)i.radius * 2);
                }
                int count=0;
                //foreach (var i in vectorImagesBuff[num].arcs)
                //{
                //    //gg.DrawLine(Pens.Green, 0, 0, (float)i.UpperLeft.X + xOffset, (float)i.UpperLeft.Y + yOffset);
                //    gg.DrawArc(Pens.Red, (float)i.UpperLeft.X + xOffset, (float)i.UpperLeft.Y + yOffset, (float)i.radius * 2, (float)i.radius * 2,360- (float)i.EndAngle, Math.Abs((float)i.EndAngle-(float)i.startAngle));
                //    //count++;
                //    //if (count == 1) break;
                //}
                for_plans[num].Image = bit;
               // bit.Save("hui.png");
            }
        }
        private void Creator_Resize(object sender, EventArgs e)
        {
            rebuild_images();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            pickFile(0);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            pickFile(1);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            pickFile(2);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            pickFile(3);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            getFromAnother(0);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            getFromAnother(1);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            getFromAnother(2);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            getFromAnother(3);
        }
        
        void pickFile(int num)
        {
            OpenFileDialog file = new OpenFileDialog();
            //new FirstOpenAddUGO().ShowDialog();
            if(file.ShowDialog() == DialogResult.OK)
            {
                if (file.FileName.Split('.')[file.FileName.Split('.').Length - 1] == "txt")
                {
                    image img = JsonConvert.DeserializeObject<image> (File.ReadAllText(file.FileName));
                    vectorImages[num] = new Equipment.VectorPic();
                    vectorImages[num].polyLines = new List<List<Equipment.Point>>();
                    //vectorImages[num].hatching = new List<Equipment.Point>();
                    //vectorImages[num].arcs = new List<arc>();
                    vectorImages[num].circles = new List<circle>();

                    foreach (var i in img.plines)
                    {
                        vectorImages[num].polyLines.Add(new List<Equipment.Point>());
                        foreach (var j in i.list)
                            vectorImages[num].polyLines[vectorImages[num].polyLines.Count - 1].Add(new Equipment.Point() { X = Convert.ToSingle(j.X), Y = Convert.ToSingle(j.Y) });
                    }
                    //foreach (var i in img.arcs)
                    //{
                    //    vectorImages[num].arcs.Add(new arc() { EndAngle = (i.EndAngle/Math.PI)*180, radius=i.radius, startAngle=(i.startAngle / Math.PI) * 180, UpperLeft = i.UpperLeft, radiusX = i.radius, radiusY = i.radius });
                    //}
                    foreach (var i in img.circles)
                    {
                        vectorImages[num].circles.Add(new circle() { center = i.center, radius=i.radius, radiusX = i.radiusX, radiusY = i.radiusY });
                    }

                    tableLayoutPanel6.Controls.Remove(buttons_for_files[num]);

                    for_plans[num] = new PictureBox();
                    for_plans[num].Name = num.ToString();
                    for_plans[num].Dock = DockStyle.Fill;
                    for_plans[num].ContextMenu = menu2;
                    tableLayoutPanel6.Controls.Remove(buttons_for_links[num]);

                    tableLayoutPanel6.Controls.Add(for_plans[num], num, 1);
                    tableLayoutPanel6.SetRowSpan(for_plans[num], 2);
                    Refresh();
                    //normalization. Part 1 - calcualiting proportions

                    double LeftUpperX = 0, LeftUpperY = 0,
                    RightLowerX = 0, RightLowerY = 0;

                    LeftUpperX = 0; LeftUpperY = 0;
                    RightLowerX = 0; RightLowerY = 0;

                    calculateBorders(ref LeftUpperX, ref LeftUpperY, ref RightLowerX, ref RightLowerY, vectorImages[num]);


                    ////
                    foreach (var i in vectorImages[num].polyLines)
                        for (int j = 0; j < i.Count; j++)
                        {
                            i[j].X -= (float)LeftUpperX;
                            i[j].Y = (float)LeftUpperY- i[j].Y;
                        }
                    //foreach (var i in vectorImages[num].arcs)
                    //{
                    //    i.UpperLeft.X -= (float)LeftUpperX;
                    //    i.UpperLeft.Y = (float)LeftUpperY - i.UpperLeft.Y;
                    //}
                    foreach (var i in vectorImages[num].circles)
                    {
                        i.center.X -= (float)LeftUpperX;
                        i.center.Y = (float)LeftUpperY - i.center.Y;
                    }

                    rebuild_images();
                }
            }
        }


        void getFromAnother(int num)
        {
            Concrete_Searcher ser = new Concrete_Searcher(num);
            ser.ShowDialog();
            vectorImages[num] = ser.result;
            rebuild_images();
        }

        public void calculateBorders(ref double LeftUpperX, ref double LeftUpperY,  ref double RightLowerX , ref double RightLowerY, Equipment.VectorPic vectorpic, bool normalized = false)
        {
            if (normalized)
            {
                //if (vectorpic.arcs.Count != 0)
                //{
                //    LeftUpperX = vectorpic.arcs[0].UpperLeft.X; LeftUpperY = vectorpic.arcs[0].UpperLeft.Y;
                //    RightLowerX = vectorpic.arcs[0].UpperLeft.X; RightLowerY = vectorpic.arcs[0].UpperLeft.Y;
                //}
                if (vectorpic.polyLines.Count != 0)
                {
                    LeftUpperX = vectorpic.polyLines[0][0].X; LeftUpperY = vectorpic.polyLines[0][0].Y;
                    RightLowerX = vectorpic.polyLines[0][0].X; RightLowerY = vectorpic.polyLines[0][0].Y;
                }
                else if (vectorpic.circles.Count != 0)
                {
                    LeftUpperX = vectorpic.circles[0].center.X; LeftUpperY = vectorpic.circles[0].center.Y;
                    RightLowerX = vectorpic.circles[0].center.X; RightLowerY = vectorpic.circles[0].center.Y;
                }
                else
                {
                    return;
                }
                foreach (var i in vectorpic.polyLines)
                {
                    foreach (var j in i)
                    {
                        if (j.X < LeftUpperX)
                            LeftUpperX = j.X;
                        if (j.X > RightLowerX)
                            RightLowerX = j.X;

                        if (j.Y < LeftUpperY)
                            LeftUpperY = j.Y;
                        if (j.Y > RightLowerY)
                            RightLowerY = j.Y;
                    }
                }
                foreach (var i in vectorpic.circles)
                {
                    if (i.center.X - i.radius < LeftUpperX)
                        LeftUpperX = i.center.X - i.radius;
                    if (i.center.X + i.radius > RightLowerX)
                        RightLowerX = i.center.X + i.radius;

                    if (i.center.Y - i.radius < LeftUpperY)
                        LeftUpperY = i.center.Y - i.radius;
                    if (i.center.Y + i.radius > RightLowerY)
                        RightLowerY = i.center.Y + i.radius;
                }
                //foreach (var i in vectorpic.arcs)
                //{
                //    if (i.UpperLeft.X < LeftUpperX)
                //        LeftUpperX = i.UpperLeft.X;
                //    if (i.UpperLeft.X + i.radius * 2 > RightLowerX)
                //        RightLowerX = i.UpperLeft.X + i.radius * 2;

                //    if (i.UpperLeft.Y < LeftUpperY)
                //        LeftUpperY = i.UpperLeft.Y;
                //    if (i.UpperLeft.Y + i.radius * 2 > RightLowerY)
                //        RightLowerY = i.UpperLeft.Y + i.radius * 2;
                //}
                //LeftUpperX--; LeftUpperY--;
                //RightLowerX++; RightLowerY++;
            }
            else
            {
                //if (vectorpic.arcs.Count != 0)
                //{
                //    LeftUpperX = vectorpic.arcs[0].UpperLeft.X; LeftUpperY = vectorpic.arcs[0].UpperLeft.Y;
                //    RightLowerX = vectorpic.arcs[0].UpperLeft.X; RightLowerY = vectorpic.arcs[0].UpperLeft.Y;
                //}
                if (vectorpic.polyLines.Count != 0)
                {
                    LeftUpperX = vectorpic.polyLines[0][0].X; LeftUpperY = vectorpic.polyLines[0][0].Y;
                    RightLowerX = vectorpic.polyLines[0][0].X; RightLowerY = vectorpic.polyLines[0][0].Y;
                }
                else if (vectorpic.circles.Count != 0)
                {
                    LeftUpperX = vectorpic.circles[0].center.X; LeftUpperY = vectorpic.circles[0].center.Y;
                    RightLowerX = vectorpic.circles[0].center.X; RightLowerY = vectorpic.circles[0].center.Y;
                }
                else
                {
                    return;
                }
                foreach (var i in vectorpic.polyLines)
                {
                    foreach (var j in i)
                    {
                        if (j.X < LeftUpperX)
                            LeftUpperX = j.X;
                        if (j.X > RightLowerX)
                            RightLowerX = j.X;

                        if (j.Y > LeftUpperY)
                            LeftUpperY = j.Y;
                        if (j.Y < RightLowerY)
                            RightLowerY = j.Y;
                    }
                }
                foreach (var i in vectorpic.circles)
                {
                    if (i.center.X - i.radius < LeftUpperX)
                        LeftUpperX = i.center.X - i.radius;
                    if (i.center.X + i.radius > RightLowerX)
                        RightLowerX = i.center.X + i.radius;

                    if (i.center.Y + i.radius > LeftUpperY)
                        LeftUpperY = i.center.Y + i.radius;
                    if (i.center.Y - i.radius < RightLowerY)
                        RightLowerY = i.center.Y - i.radius;
                }
                //foreach (var i in vectorpic.arcs)
                //{
                //    if (i.UpperLeft.X < LeftUpperX)
                //        LeftUpperX = i.UpperLeft.X;
                //    if (i.UpperLeft.X + i.radius * 2 > RightLowerX)
                //        RightLowerX = i.UpperLeft.X + i.radius * 2;

                //    if (i.UpperLeft.Y > LeftUpperY)
                //        LeftUpperY = i.UpperLeft.Y;
                //    if (i.UpperLeft.Y - i.radius * 2 < RightLowerY)
                //        RightLowerY = i.UpperLeft.Y - i.radius * 2;
                //}
                //LeftUpperX--; LeftUpperY++;
                //    RightLowerX++; RightLowerY--;
            }
        }
        private void handler2(object sender, EventArgs e)
        {
            int fordel = Convert.ToInt32(((PictureBox)((MenuItem)sender).GetContextMenu().SourceControl).Name);
            images[fordel] = null;
            vectorImages[fordel] = null;
            tableLayoutPanel6.Controls.Remove(for_plans[fordel]);
            tableLayoutPanel6.Controls.Add(buttons_for_files[fordel], fordel, 1);
            tableLayoutPanel6.Controls.Add(buttons_for_links[fordel], fordel, 2);
        }

    }
}
