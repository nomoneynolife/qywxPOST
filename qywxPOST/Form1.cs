using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace qywxPOST
{
    public partial class Form1 : Form
    {
        //private const string WebhookUrl = "https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key=a5ac3a59-1ac3-4fb4-adcc-001fe1fe92eb";
        private string WebhookUrl; //从配置文件读取，写死太麻烦了
        private System.Threading.Timer timer;
        private List<ScheduledTask> scheduledTasks = new List<ScheduledTask>();
        private string tasksFilePath = "ScheduledTasks.txt"; // 存储任务的文件路径

        private System.Windows.Forms.Timer updateTimer; //更新listView1的过期任务标红

        private bool isPlaying = false; // 标记是否正在播放
        private bool isFullScreenFormClosed = true; // 标记全屏窗体是否关闭
        private PictureBox pictureBox1; // PictureBox 控件用于显示 GIF
        private Form gifForm; // 用于显示 GIF 的窗体

        // 添加 Mutex 以确保只有一个实例在运行
        private Mutex mutex;

        // 添加 NotifyIcon 和 ContextMenu 以处理系统托盘相关的操作
        private NotifyIcon notifyIcon;
        private ContextMenu contextMenu;
        private MenuItem showMenuItem;
        private MenuItem exitMenuItem;

        public Form1()
        {
            // 使用 Mutex 确保只有一个实例在运行
            bool createdNew;
            mutex = new Mutex(true, "SingleInstanceAppMutex", out createdNew);

            if (!createdNew)
            {
                //MessageBox.Show("只能运行一个窗口", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close(); // 关闭当前窗体
                Application.Exit();
                return;
            }

            InitializeComponent();
            InitializePictureBox(); // 初始化 PictureBox

            // 为listView1添加列
            listView1.View = View.Details;
            listView1.Columns.Add("预定时间", 130);
            listView1.Columns.Add("消息内容", 180);

            // 创建用于删除任务的右键菜单项
            ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("删除定时");
            deleteMenuItem.Click += DeleteTask_Click;

            // 将删除菜单项添加到右键菜单中
            contextMenuStrip1.Items.Add(deleteMenuItem);

            // 将右键菜单关联到listView1
            listView1.ContextMenuStrip = contextMenuStrip1;

            // 附加listView1的MouseClick事件处理程序
            listView1.MouseClick += listView1_MouseClick;

            // 从文件加载任务
            LoadTasksFromFile();

            InitializeSystemTray(); // 初始化系统托盘

            // 初始化定时器，listView1的
            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 1000; // 1秒钟更新一次
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();

            // 从配置文件读取 WebhookUrl
            string configFile = "config.ini";
            XDocument doc = XDocument.Load(configFile);

            // 使用 LINQ to XML 从 XML 文件中检索值
            this.WebhookUrl = doc.Descendants("WebhookUrl").FirstOrDefault()?.Value;
            //textBoxOutput.AppendText($"WebhookUrl: {this.WebhookUrl}" + Environment.NewLine); //打印查看是否异常

            string isForcePushString = doc.Descendants("ForcePush").FirstOrDefault()?.Value;
            bool isForcePush;
            if (bool.TryParse(isForcePushString, out isForcePush))
            {
                checkBoxForcePush.Checked = isForcePush;
            }

            checkBoxForcePush.CheckedChanged += checkBoxForcePush_CheckedChanged;

            string isTodayTasksString = doc.Descendants("TodayTasks").FirstOrDefault()?.Value;
            bool isTodayTasks;
            if (bool.TryParse(isTodayTasksString, out isTodayTasks))
            {
                checkBoxShowTodayTasks.Checked = isTodayTasks;
            }

            checkBoxShowTodayTasks.CheckedChanged += checkBoxShowTodayTasks_CheckedChanged;

        }

        private void UpdateTimer_Tick(object sender, EventArgs e) //更新listView1的过期任务标红
        {
            // 检查并更新列表
            UpdateListView();
        }

        private void UpdateListView() //更新listView1的过期任务标红
        {
            // 获取当前时间
            DateTime currentTime = DateTime.Now;

            // 遍历列表项，更新颜色
            foreach (ListViewItem item in listView1.Items)
            {
                DateTime scheduledTime = DateTime.Parse(item.SubItems[0].Text);

                // 如果任务已过期，将字体颜色设置为红色
                if (scheduledTime < currentTime)
                {
                    item.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    item.ForeColor = System.Drawing.SystemColors.WindowText; // 恢复默认颜色
                }
            }
        }

        private void InitializeSystemTray() //系统托盘
        {
            // 从资源加载自定义图标
            Icon customIcon = Properties.Resources.ico1;

            // 创建 NotifyIcon
            notifyIcon = new NotifyIcon
            {
                Icon = customIcon,
                Text = "qywxPOST",
                Visible = true
            };

            // 创建 ContextMenu 和 MenuItem
            contextMenu = new ContextMenu();
            showMenuItem = new MenuItem("Show", ShowMenuItem_Click);
            exitMenuItem = new MenuItem("Exit", ExitMenuItem_Click);

            // 添加 MenuItem 到 ContextMenu
            contextMenu.MenuItems.Add(showMenuItem);
            contextMenu.MenuItems.Add(exitMenuItem);

            // 设置 ContextMenu 到 NotifyIcon
            notifyIcon.ContextMenu = contextMenu;

            // 添加双击事件处理
            notifyIcon.DoubleClick += ShowMenuItem_Click;
        }

        private void ShowMenuItem_Click(object sender, EventArgs e) //系统托盘-显示
        {
            // 显示窗体
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void ExitMenuItem_Click(object sender, EventArgs e) //系统托盘-退出
        {
            // 退出应用程序
            Application.Exit();
        }

        private void InitializePictureBox() //初始化PictureBox以播放全屏gif
        {
            // 创建 PictureBox 以显示 GIF
            pictureBox1 = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.AutoSize,
                Location = new System.Drawing.Point(10, 10),  // 根据需要调整位置
            };

            // 将 PictureBox 添加到窗体
            this.Controls.Add(pictureBox1);
        }

        private void button1_Click(object sender, EventArgs e) // 新增定时任务
        {
            string textToSchedule = textDate.Text;
            string messageContent = textData.Text;

            if (!string.IsNullOrEmpty(textToSchedule))
            {
                DateTime scheduledTime;
                if (DateTime.TryParse(textToSchedule, out scheduledTime))
                {
                    if (scheduledTime > DateTime.Now)
                    {
                        // 检查是否已存在与该任务关联的定时器
                        if (!scheduledTasks.Any(task => task.ScheduledTime == scheduledTime && task.MessageContent == messageContent))
                        {
                            long dueTime = (long)(scheduledTime - DateTime.Now).TotalMilliseconds;
                            timer = new System.Threading.Timer(SendScheduledMessage, messageContent, dueTime, Timeout.Infinite);

                            // 将任务添加到 listView1
                            ListViewItem listItem = new ListViewItem(scheduledTime.ToString());
                            listItem.SubItems.Add(messageContent);
                            listView1.Items.Add(listItem);

                            // 将任务添加到列表中
                            scheduledTasks.Add(new ScheduledTask(scheduledTime, messageContent, timer));

                            // 在 textBoxOutput 中显示消息
                            textBoxOutput.AppendText($"定时任务已创建，将在 {scheduledTime} 推送消息" + Environment.NewLine);
                        }
                        else
                        {
                            MessageBox.Show("相同的定时任务已存在", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("预定时间应该在当前时间之后", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("无效的日期和时间格式", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("请输入文本内容", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // 保存任务到文件
            SaveTasksToFile();
        }

        private async void SendScheduledMessage(object state) //推送消息和gif
        {
            try
            {
                // 尝试发送预定消息
                string message = state as string;

                using (HttpClient client = new HttpClient())
                {
                    // 构造 JSON 请求体
                    string jsonBody = $"{{\"msgtype\": \"text\", \"text\": {{\"content\": \"{message}\"}}}}";
                    HttpResponseMessage response = await client.PostAsync(WebhookUrl, new StringContent(jsonBody, Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);

                    // 使用 Invoke 更新主线程上的 UI 组件
                    Invoke((MethodInvoker)delegate
                    {
                        // 找到与发送消息对应的 ListViewItem
                        ListViewItem scheduledItem = listView1.Items
                            .OfType<ListViewItem>()
                            .FirstOrDefault(item => item.SubItems[1].Text == message);

                        if (scheduledItem != null)
                        {
                            // 找到与发送消息对应的 ScheduledTask
                            ScheduledTask completedTask = scheduledTasks.Find(task =>
                                task.MessageContent == message);

                            if (completedTask != null)
                            {
                                // 停止和释放定时器
                                completedTask.Timer?.Dispose();
                            }
                        }

                        // 在 textBoxOutput 中显示消息
                        textBoxOutput.AppendText("消息成功发送到企业微信机器人" + Environment.NewLine);

                        // 如果 checkBox1 被选中
                        if (checkBoxForcePush.Checked)
                        {
                            // 开始播放 GIF
                            PlayGif();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                // 使用 Invoke 更新主线程上的 UI 组件
                Invoke((MethodInvoker)delegate
                {
                    // 在 textBoxOutput 中显示错误消息
                    textBoxOutput.AppendText($"消息发送失败：{ex.Message}" + Environment.NewLine);
                });
                // 如果 checkBox1 被选中
                if (checkBoxForcePush.Checked)
                {
                    // 开始播放 GIF
                    PlayGif();
                }
            }
        }

        private void PlayGif() //播放GIF
        {
            // 检查是否已经在播放
            if (!isPlaying)
            {
                Invoke((MethodInvoker)delegate
                {
                    // 创建一个新窗体以显示 GIF
                    gifForm = new Form();
                    gifForm.FormBorderStyle = FormBorderStyle.None;
                    gifForm.WindowState = FormWindowState.Maximized;

                    // 在新窗体上创建一个 PictureBox
                    PictureBox gifPictureBox = new PictureBox
                    {
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Dock = DockStyle.Fill
                    };
                    gifForm.Controls.Add(gifPictureBox);

                    // 在 PictureBox 中加载并显示 GIF
                    gifPictureBox.Image = Properties.Resources.img1;

                    // 订阅 KeyPress 事件以在按任意键时关闭窗体
                    gifForm.KeyPress += GifForm_KeyPress;

                    // 显示窗体
                    gifForm.Show();

                    // 设置标记以指示全屏窗体已打开
                    isFullScreenFormClosed = false;

                    // 设置标记以指示正在播放 GIF
                    isPlaying = true;
                });
            }
        }

        private void StopGif() //停止GIF
        {
            // 设置标记以指示 GIF 未在播放
            isPlaying = false;

            Invoke((MethodInvoker)delegate
            {
                // 重置标记以指示全屏窗体已关闭
                isFullScreenFormClosed = true;

                // 关闭 GIF 窗体
                gifForm?.Close();
            });
        }

        private void GifForm_KeyPress(object sender, KeyPressEventArgs e) //任意键关闭GIF
        {
            // 检查按下的键是否为 ESC 键
            if (e.KeyChar == (char)Keys.Escape)
            {
                // 处理按下 ESC 键的事件以关闭窗体
                isPlaying = false; // 将 isPlaying 设置为 false 以允许再次播放
                gifForm.Close();
            }
        }

        private void getTimeNow_Click(object sender, EventArgs e) //获取当前时间
        {
            // 获取当前时间
            DateTime currentTime = DateTime.Now;

            // 在textDate中显示当前时间
            textDate.Text = currentTime.ToString();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e) //listView1设定
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem != null && listView1.FocusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void DeleteTask_Click(object sender, EventArgs e) //删除listView1定时任务并保存到指定文件
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // 遍历选中的项并逐个删除
                foreach (ListViewItem selectedItem in listView1.SelectedItems)
                {
                    // 获取所选项的预定时间和消息内容
                    string scheduledTime = selectedItem.SubItems[0].Text;
                    string messageContent = selectedItem.SubItems[1].Text;

                    // 从 listView1 中移除所选项
                    selectedItem.Remove();

                    // 从列表中移除任务
                    ScheduledTask deletedTask = scheduledTasks.Find(task =>
                        task.ScheduledTime.ToString() == scheduledTime &&
                        task.MessageContent == messageContent);

                    if (deletedTask != null)
                    {
                        scheduledTasks.Remove(deletedTask);

                        // 在 textBoxOutput 中显示消息
                        textBoxOutput.AppendText($"已删除定时任务：{scheduledTime}，消息内容：{messageContent}" + Environment.NewLine);
                    }
                }

                // 保存任务到文件
                SaveTasksToFile();
            }
        }

        private void LoadTasksFromFile() //加载ScheduledTasks.txt到listView1中
        {
            if (File.Exists(tasksFilePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(tasksFilePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 2)
                        {
                            DateTime scheduledTime = DateTime.Parse(parts[0]);
                            string messageContent = parts[1];

                            // 检查任务是否已经存在于 scheduledTasks 列表中
                            // if (!scheduledTasks.Any(task => task.ScheduledTime == scheduledTime && task.MessageContent == messageContent))
                            // {
                            // 查找已存在的任务，如果存在则将其定时器设为 null
                            ScheduledTask existingTask = scheduledTasks.FirstOrDefault(t =>
                                t.ScheduledTime == scheduledTime && t.MessageContent == messageContent);

                            if (existingTask != null)
                            {
                                // 将任务添加到列表中，提供现有定时器作为参数
                                scheduledTasks.Add(new ScheduledTask(scheduledTime, messageContent, existingTask.Timer));

                                // 将任务添加到 listView1
                                ListViewItem listItem = new ListViewItem(scheduledTime.ToString());
                                listItem.SubItems.Add(messageContent);
                                listView1.Items.Add(listItem);
                            }
                            else
                            {
                                // 将任务添加到列表中，提供 null 作为定时器参数
                                scheduledTasks.Add(new ScheduledTask(scheduledTime, messageContent, null));

                                // 将任务添加到 listView1
                                ListViewItem listItem = new ListViewItem(scheduledTime.ToString());
                                listItem.SubItems.Add(messageContent);

                                // 如果任务未过期，为任务创建新的定时器
                                if (scheduledTime >= DateTime.Now)
                                {
                                    long dueTime = (long)(scheduledTime - DateTime.Now).TotalMilliseconds;
                                    System.Threading.Timer newTimer = new System.Threading.Timer(SendScheduledMessage, messageContent, dueTime, Timeout.Infinite);
                                    scheduledTasks.Last().Timer = newTimer;
                                }

                                listView1.Items.Add(listItem);
                                UpdateListView(); // 更新定时器
                            }
                            //}
                            // 如果任务已存在，则不进行任何处理
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"从文件加载任务时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveTasksToFile() //通过按钮1新增的任务保存到文件
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (ScheduledTask task in scheduledTasks)
                {
                    lines.Add($"{task.ScheduledTime}|{task.MessageContent}");
                }

                File.WriteAllLines(tasksFilePath, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存任务到文件时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class ScheduledTask //通过定时器触发的消息发送任务
        {
            public DateTime ScheduledTime { get; }
            public string MessageContent { get; }
            public System.Threading.Timer Timer { get; set; }

            public ScheduledTask(DateTime scheduledTime, string messageContent, System.Threading.Timer timer)
            {
                ScheduledTime = scheduledTime;
                MessageContent = messageContent;
                Timer = timer;
            }
        }

        protected override void OnResize(EventArgs e) //最小化时隐藏到托盘
        {
            // 捕捉窗体最小化事件，隐藏到系统托盘
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }

            base.OnResize(e);
        }

        private void buttonFileUpdata_Click(object sender, EventArgs e) //清空listView1列表后重载
        {
            listView1.Items.Clear(); //清空listView1
            LoadTasksFromFile();
        }

        private void checkBoxShowTodayTasks_CheckedChanged(object sender, EventArgs e) //只显示当天任务
        {
            if (checkBoxShowTodayTasks.Checked)
            {
                ShowTodayTasks();
            }
            else
            {
                // 在加载任务之前清空 listView1
                listView1.Items.Clear();

                // 只加载符合当前筛选条件的任务
                foreach (ScheduledTask task in scheduledTasks)
                {
                    // 检查任务是否应根据筛选条件显示
                    if (checkBoxShowTodayTasks.Checked || task.ScheduledTime.Date != DateTime.Today)
                    {
                        AddTaskToListView(task.ScheduledTime, task.MessageContent);
                    }
                }
                // 保存更新后的配置到文件
                SaveConfiguration();
                // 当 checkBox1 状态改变时保存到配置文件
                string configFile = "config.ini";
                XDocument doc = XDocument.Load(configFile);

                // 更新 IsCheckBoxChecked 节点的值
                doc.Descendants("TodayTasks").FirstOrDefault()?.SetValue(checkBoxShowTodayTasks.Checked.ToString());

                // 保存配置文件
                doc.Save(configFile);
            }
        }

        private void ShowTodayTasks() //只显示当天任务
        {
            // 获取今天的日期
            DateTime today = DateTime.Today;

            // 清空 listView1 中的项
            listView1.Items.Clear();

            // 添加今天的任务到 listView1
            foreach (ScheduledTask task in scheduledTasks)
            {
                if (task.ScheduledTime.Date == today)
                {
                    ListViewItem listItem = new ListViewItem(task.ScheduledTime.ToString());
                    listItem.SubItems.Add(task.MessageContent);
                    listView1.Items.Add(listItem);
                }
            }
        }

        private void AddTaskToListView(DateTime scheduledTime, string messageContent) //添加任务到view1
        {
            ListViewItem listItem = new ListViewItem(scheduledTime.ToString());
            listItem.SubItems.Add(messageContent);
            listView1.Items.Add(listItem);
        }

        private void SaveConfiguration() //保存checkbox2的状态到配置文件
        {
            // 当 checkBox1 状态改变时保存到配置文件
            string configFile = "config.ini";
            XDocument doc = XDocument.Load(configFile);

            // 更新 IsCheckBoxChecked 节点的值
            doc.Descendants("TodayTasks").FirstOrDefault()?.SetValue(checkBoxShowTodayTasks.Checked.ToString());

            // 保存配置文件
            doc.Save(configFile);
        }

        private void checkBoxForcePush_CheckedChanged(object sender, EventArgs e) //保存checkbox1的状态到配置文件
        {
            // 当 checkBox1 状态改变时保存到配置文件
            string configFile = "config.ini";
            XDocument doc = XDocument.Load(configFile);

            // 更新 IsCheckBoxChecked 节点的值
            doc.Descendants("ForcePush").FirstOrDefault()?.SetValue(checkBoxForcePush.Checked.ToString());

            // 保存配置文件
            doc.Save(configFile);
        }




    }
}
