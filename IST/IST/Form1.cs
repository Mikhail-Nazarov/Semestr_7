using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IST
{
    struct Point
    {
        public double x1 { get; set; }
        public double x2 { get; set; }
        public double y { get; set; }
        public Point(double y, double x1,double x2)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y = y;
        }
    }

    public partial class Form1 : Form
    {
        List<Point> points1 = new List<Point>();
        List<Point> points2=new List<Point>();
        List<Point> points3= new List<Point>();

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            dataGridView2.RowCount = Convert.ToInt32(numericUpDown2.Value) + 1;
            dataGridView1.RowCount =Convert.ToInt32(numericUpDown1.Value)+1;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView3.ReadOnly = true;

            comboBox1.SelectedIndex = 0;

            CompareResult.Text = "";

            chart1.Series.Add("2");
            chart1.Series.Add("3");
        }
        void CheckTableValues(DataGridView table, Button button)
        {
            try
            {
                Convert.ToDouble(table.CurrentCell.Value);
            }
            catch
            {
                table.CurrentCell.Value = string.Empty;
                MessageBox.Show("Таблицы должны содержать только числа", "Ошибка ввода данных!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if ((Convert.ToDouble(table.CurrentCell.Value) < 0 || Convert.ToDouble(table.CurrentCell.Value) > 1) && table.CurrentCell.ColumnIndex == 0)
            {
                table.CurrentCell.Value = string.Empty;
                MessageBox.Show("Альфа-уровни должны быть в диапазоне от 0 до 1", "Ошибка ввода данных!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            for (int i = 0; i < table.RowCount; i++)
                for (int j = 0; j < table.ColumnCount; j++)
                {
                    if (table.Rows[i].Cells[j].Value == null)
                    {
                        button.Enabled = false;
                        return;
                    }
                }
            button.Enabled = true;
            if (button1.Enabled == true && button2.Enabled == true)
            {
                StartBt.Enabled = true;
                CompareBt.Enabled = true;
            }
        }

        void Draw(DataGridView table, int seriesNumber)
        {
            List<Point> points = new List<Point>();
            chart1.Series[seriesNumber] = new System.Windows.Forms.DataVisualization.Charting.Series();
            if(seriesNumber==2)
                chart1.Series[seriesNumber].Name = "Результат";
            else
                chart1.Series[seriesNumber].Name = "Число "+(seriesNumber+1).ToString();
            chart1.Series[seriesNumber].BorderWidth = 3;
            chart1.Series[seriesNumber].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            for (int i = 0; i < table.RowCount; i++)
            {
                points.Add(new Point(Convert.ToDouble(table.Rows[i].Cells[0].Value), 
                    Convert.ToDouble(table.Rows[i].Cells[1].Value),
                    Convert.ToDouble(table.Rows[i].Cells[2].Value)));
            }
            points = Sort(points);
            for (int i = 0; i < table.RowCount; i++)
            {
                chart1.Series[seriesNumber].Points.AddXY(points[i].x1, points[i].y);
            }
            for (int i = table.RowCount - 1; i > -1; i--)
            {
                chart1.Series[seriesNumber].Points.AddXY(points[i].x2, points[i].y);
            }
        }

        bool BulgeCheck(List<Point> points, DataGridView dataGrid)
        {
            bool chek0 = false, chek1 = false;
            points.Clear();
            for (int i = 0; i < dataGrid.RowCount; i++)
            {
                points.Add(new Point(Convert.ToDouble(dataGrid.Rows[i].Cells[0].Value), 
                    Convert.ToDouble(dataGrid.Rows[i].Cells[1].Value), 
                    Convert.ToDouble(dataGrid.Rows[i].Cells[2].Value)));
                if (points[i].y == 0) chek0 = true;
                if (points[i].y == 1) chek1 = true;
            }
            points = Sort(points);
            for(int i=0;i<points.Count-1;i++)
            {
                if(points[i].x1 > points[i+1].x1 || points[i].x2 < points[i + 1].x2)
                {
                    MessageBox.Show("Функция принадлежности должна быть выпуклой", "Ошибка данных!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            if(!chek0 || !chek1)
            {
                MessageBox.Show("Обязательно должны присутствовать 0-й и 1-й срезы", "Ошибка данных!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        List<Point> Sort(List<Point> points)
        {
            Point temp;
            for (int i=0;i<points.Count;i++)
                for(int j=i+1; j<points.Count;j++)
                {
                    if(points[i].y>points[j].y)
                    {
                        temp = new Point(points[j].y, points[j].x1, points[j].x2);
                        points[j]= new Point(points[i].y, points[i].x1, points[i].x2);
                        points[i] = temp;
                    }
                }
            return points;
        }

        Point StraightLine(Point requiredPoint, List<Point> points)
        {
            Point point1 = new Point(), point2 = new Point();
            double y = requiredPoint.y;
            for (int i=0;i<points.Count-1;i++)
            {
                if(points[i+1].y>y)
                {
                    point1 = points[i];
                    point2 = points[i+1];
                    break;
                }
            }
            y = requiredPoint.y;
            double k1 = (point2.y - point1.y) / (point2.x1 - point1.x1);
            double k2 = (point2.y - point1.y) / (point2.x2 - point1.x2);
            double b1 = -(point1.x1 * point2.y - point2.x1 * point1.y) / (point2.x1 - point1.x1);
            double b2 = -(point1.x2 * point2.y - point2.x2 * point1.y) / (point2.x2 - point1.x2);
            Point result = new Point(y,(y-b1)/k1, (y - b2) / k2);
            return result;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CheckTableValues(dataGridView1, button1);
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CheckTableValues(dataGridView2, button2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Draw(dataGridView1, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Draw(dataGridView2, 1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Draw(dataGridView3, 2);
        }

        void SetResulTable(List<Point> result)
        {
            dataGridView3.RowCount = result.Count;
            for (int i=0;i<result.Count;i++)
            {
                dataGridView3.Rows[i].Cells[0].Value = result[i].y;
                dataGridView3.Rows[i].Cells[1].Value = result[i].x1;
                dataGridView3.Rows[i].Cells[2].Value = result[i].x2;
            }
        }
        List<Point> Division(List<Point> points1, List<Point> points2)
        {
            int j;
            Point p;
            bool flag = false;
            List<Point> result = new List<Point>();
            for (int i = 0; i < points1.Count; i++)
            {
                for (j = 0; j < points2.Count; j++)
                {
                    if (points1[i].y == points2[j].y)
                    {
                        result.Add(new Point(points1[i].y, points1[i].x1 / points2[j].x2, points1[i].x2 / points2[j].x1));
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    p = StraightLine(points1[i], points2);
                    result.Add(new Point(points1[i].y, points1[i].x1 / p.x2, points1[i].x2 / p.x1));
                }
                flag = false;
            }

            for (int i = 0; i < points2.Count; i++)
            {
                for (j = 0; j < result.Count; j++)
                    if (points2[i].y == result[j].y)
                    {
                        flag = true;
                        break;
                    }
                if (!flag)
                {
                    p = StraightLine(points2[i], points1);
                    result.Add(new Point(points2[i].y, p.x1 / points2[i].x2, p.x2 / points2[i].x1));
                }
                flag = false;
            }
            result = Sort(result);
            SetResulTable(result);
            return result;
        }
        List<Point> Difference(List<Point> points1, List<Point> points2)
        {
            int j;
            Point p;
            bool flag = false;
            List<Point> result = new List<Point>();
            for (int i = 0; i < points1.Count; i++)
            {
                for (j = 0; j < points2.Count; j++)
                {
                    if (points1[i].y == points2[j].y)
                    {
                        result.Add(new Point(points1[i].y, points1[i].x1 - points2[j].x2, points1[i].x2 - points2[j].x1));
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    p = StraightLine(points1[i], points2);
                    result.Add(new Point(points1[i].y, points1[i].x1 - p.x2, points1[i].x2 - p.x1));
                }
                flag = false;
            }

            for (int i = 0; i < points2.Count; i++)
            {
                for (j = 0; j < result.Count; j++)
                    if (points2[i].y == result[j].y)
                    {
                        flag = true;
                        break;
                    }
                if (!flag)
                {
                    p = StraightLine(points2[i], points1);
                    result.Add(new Point(points2[i].y, p.x1 - points2[i].x2, p.x2 - points2[i].x1));
                }
                flag = false;
            }
            result = Sort(result);
            SetResulTable(result);
            return result;
        }
        List<Point> Multiplication(List<Point> points1, List<Point> points2)
        {
            int j;
            Point p;
            bool flag = false;
            List<Point> result = new List<Point>();
            for (int i = 0; i < points1.Count; i++)
            {
                for (j = 0; j < points2.Count; j++)
                {
                    if (points1[i].y == points2[j].y)
                    {
                        result.Add(new Point(points1[i].y, points1[i].x1 * points2[j].x1, points1[i].x2 * points2[j].x2));
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    p = StraightLine(points1[i], points2);
                    result.Add(new Point(points1[i].y, points1[i].x1 * p.x1, points1[i].x2 * p.x2));
                }
                flag = false;
            }

            for (int i = 0; i < points2.Count; i++)
            {
                for (j = 0; j < result.Count; j++)
                    if (points2[i].y == result[j].y)
                    {
                        flag = true;
                        break;
                    }
                if (!flag)
                {
                    p = StraightLine(points2[i], points1);
                    result.Add(new Point(points2[i].y, points2[i].x1 * p.x1, points2[i].x2 * p.x2));
                }
                flag = false;
            }
            result = Sort(result);
            SetResulTable(result);
            return result;
        }
        List<Point> Sum(List<Point> points1, List<Point> points2)
        {
            int j;
            Point p;
            bool flag = false;
            List<Point> result = new List<Point>();
            for (int i = 0; i < points1.Count; i++)
            {
                for (j = 0; j < points2.Count; j++)
                {
                    if (points1[i].y == points2[j].y)
                    {
                        result.Add(new Point(points1[i].y, points1[i].x1 + points2[j].x1, points1[i].x2 + points2[j].x2));
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    p = StraightLine(points1[i], points2);
                    result.Add(new Point(points1[i].y, points1[i].x1 + p.x1, points1[i].x2 + p.x2));
                }
                flag = false;
            }

            for (int i = 0; i < points2.Count; i++)
            {
                for(j=0;j<result.Count;j++)
                    if (points2[i].y == result[j].y)
                    {
                        flag = true;
                        break;
                    }
                if (!flag)
                {
                    p = StraightLine(points2[i], points1);
                    result.Add(new Point(points2[i].y, points2[i].x1 + p.x1, points2[i].x2 + p.x2));
                }
                flag = false;
            }
            result = Sort(result);
            SetResulTable(result);
            return result;
        }

        int Compare(List<Point> points1, List<Point> points2)
        {
            double sum = 0;
            double val1, val2;
            for (int i = 0; i < points1.Count; i++)
                sum += points1[i].x1 + points1[i].x2;
            val1 = 1.0/points1.Count * sum;
            sum = 0;
            for (int i = 0; i < points2.Count; i++)
                sum += points2[i].x1 + points2[i].x2;
            val2 = 1.0 / points2.Count * sum; 
            if (val1 > val2) return 0;
            if (val1 < val2) return 1;
            else return 2;
        }

        private void StartBt_Click(object sender, EventArgs e)
        {
            if (BulgeCheck(points1, dataGridView1) && BulgeCheck(points2, dataGridView2))
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0: points3 = Sum(points1, points2); break;
                    case 1: points3 = Difference(points1, points2); break;
                    case 2: points3 = Multiplication(points1, points2); break;
                    case 3: points3 = Division(points1, points2); break;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (BulgeCheck(points1, dataGridView1) && BulgeCheck(points2, dataGridView2))
            {
                switch (Compare(points1,points2))
                {
                    case 0: CompareResult.Text=">"; break;
                    case 1: CompareResult.Text = "<"; break;
                    case 2: CompareResult.Text = "="; break;
                }
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.RowCount = Convert.ToInt32(numericUpDown1.Value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            dataGridView2.RowCount = Convert.ToInt32(numericUpDown2.Value);
        }
    }
}
