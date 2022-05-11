using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace lab7
{
        public partial class Form1 : Form
        {
            Mutex mut = new Mutex();
            Semaphore sem = new Semaphore(4, 10);
            Semaphore plat = new Semaphore(2, 3);
            List<Label> labels = new List<Label>();
            List<Label> labels1 = new List<Label>();
            int maxPersons = 10;

            public Form1()
            {
                InitializeComponent();
            }

            private void Form1_Load(object sender, EventArgs e)
            {
                Control.CheckForIllegalCrossThreadCalls = false; // вимикаємо перевірку в Debug

                labels.AddRange(new[] { label2, label3, label4, label5 });
                foreach (Label item in labels)
                {
                    item.Text = "X";
                }

                labels1.AddRange(new[] { label1, label6 });
                foreach(Label item1 in labels1)
                {
                   item1.Text = "X";
                }

                label_train.Text = "";
                label_street.Text = "";
                for (int i = 0; i < maxPersons; i++)
                {
                    label_street.Text += i.ToString() + ", ";
                }

                for (int i = 0; i < maxPersons; i++)
                {
                    Thread myThread = new Thread(changeLabelDoor);
                    string name = i.ToString();
                    myThread.Name = name + " людина біля дверей";
                    myThread.Start(name);
                }
            }

            void changeLabelDoor(object obj)
            {
                bool flag = false;
                while (true)
                {
                    if (obj is string str)
                    {
                        mut.WaitOne();

                        //Console.WriteLine($"    {Thread.CurrentThread.Name} заглянула до зали");
                        label_street.Text = label_street.Text.Replace(str, "x");
                        label_door.Text = str;
                        Thread.Sleep(1000);
                        Thread myThread = new Thread(changeLabelsHall);
                        Console.WriteLine($"    {Thread.CurrentThread.Name} зайшла до зали");
                        myThread.Name = str + " людина у залі";
                        label_door.Text = "X";
                        listBox1.Items.Add(str);
                        Thread.Sleep(1000);
                        myThread.Start(str);

                        flag = true;

                        mut.ReleaseMutex();

                        if (flag == true) return; //Thread.CurrentThread.Abort();
                    }
                }
            }

            void changeLabelsHall(object obj)
            {
                bool flag = false;
                while (true)
                {
                    sem.WaitOne();

                    if (obj is string str)
                    {
                        foreach (Label item in labels)
                        {
                            if (item.Text == "X")
                            {
                                Console.WriteLine($"{Thread.CurrentThread.Name} знайшла вільну касу");
                                listBox1.Items.Remove(str);
                                item.Text = str;
                                Thread.Sleep(10000);
                                //Console.WriteLine($"{Thread.CurrentThread.Name} вийшла з зали");
                                item.Text = "X";
                                Thread myThread1 = new Thread(changeLabelsPlatform);
                                listBox2.Items.Add(str);
                                myThread1.Start(str);
                                //label_train.Text += str + ", ";
                                Thread.Sleep(1000);
                                flag = true;
                                break;
                            }
                        }
                    }

                    sem.Release();

                    if (flag == true) return; // Thread.CurrentThread.Abort();
                }
            }

        void changeLabelsPlatform(object obj)
        {
            bool flag = false;
            while (true)
            {
                plat.WaitOne();

                if (obj is string str)
                {
                    foreach (Label item1 in labels1)
                    {
                        if (item1.Text == "X")
                        {
                            Console.WriteLine($"{Thread.CurrentThread.Name} знайшла відкриті двері");
                            item1.Text = str;
                            listBox2.Items.Remove(str);
                            Thread.Sleep(7500);
                            //Console.WriteLine($"{Thread.CurrentThread.Name} вийшла з зали");
                            item1.Text = "X";
                            label_train.Text += str + ", ";
                            Thread.Sleep(750);
                            flag = true;
                            break;
                        }
                    }
                }

                plat.Release();

                if (flag == true) return; // Thread.CurrentThread.Abort();
            }
        }


            private void label2_TextChanged(object sender, EventArgs e)
            {
                foreach (Label item in labels)
                {
                    item.BackColor = SystemColors.Control;
                    if (item == (Label)sender)
                    {
                        item.BackColor = Color.LightGreen;
                    }
                }
            }

        private void label3_TextChanged(object sender, EventArgs e)
        {
            foreach (Label item1 in labels1)
            {
                item1.BackColor = SystemColors.Control;
                if (item1 == (Label)sender)
                {
                    item1.BackColor = Color.LightGreen;
                }
            }
        }

        private void label_door_TextChanged(object sender, EventArgs e)
        {
            if (label_door.Text != "X")
            {
                pictureBox2.Visible = false;
                pictureBox1.Visible = true;
            }
            if (label_door.Text == "X")
            {
                pictureBox1.Visible = false;
                pictureBox2.Visible = true;
            }
        }

        
    }
    }
