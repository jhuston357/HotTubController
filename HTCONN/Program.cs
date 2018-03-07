using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HTCONN
{

    class Display
    {

        int avalue = 888;
        int digit1 = 8;
        int digit2 = 8;
        int digit3 = 8;

        public Display()
	    {
	
		avalue = 888;
		digit1 = 8;
		digit2 = 8;
		digit3 = 8;
	
	     }

        public void set_value(double num)
        {

            int number = 0;
            int value = 0;

            avalue = (int)num;

            digit3 = value % 10;
            number = value / 10;

            digit2 = value % 10;
            number = value / 10;

            digit1 = value % 10;

        }

        public void error()
        {

            TcpClient client = new TcpClient("192.168.1.140", 5003);
            string data = "ERR";

            int byteCount = Encoding.ASCII.GetByteCount(data); ;

            byte[] sendData = new byte[byteCount];

            sendData = Encoding.ASCII.GetBytes(data);

            NetworkStream stream = client.GetStream();

            stream.Write(sendData, 0, sendData.Length);

            stream.Close();
            client.Close();

        }

        public void display_num()
        {

            TcpClient client = new TcpClient("192.168.1.140", 5003);
            string data = Convert.ToString(avalue);
            char pad = '0';
            data = data.PadLeft(3, pad);

            int byteCount = Encoding.ASCII.GetByteCount(data); ;

            byte[] sendData = new byte[byteCount];

            sendData = Encoding.ASCII.GetBytes(data);

            NetworkStream stream = client.GetStream();

            stream.Write(sendData, 0, sendData.Length);

            stream.Close();
            client.Close();

        }
    }

    class Hottub
    {

        private double Ctemp; //current temperature
        private double Stemp; //temperature setting

        private bool powersavemode;
        private bool partymode;

        private bool heater; // is heater on or off
        private bool mainpump; // is main pump on or off // main pump is the single speed connected to the heat
        private bool auxilarypump; // is auxilary pump on // auxilary pump is the 2 speed
        private bool auxsetting; // is auxilary pump set to low or high speed
        private bool lighting; // are the lights on

        public Hottub()
        {

            Stemp = 65.0;
            Ctemp = 80.0;
            powersavemode = false;
            partymode = false;
            heater = false;
            mainpump = false;
            auxilarypump = false;
            auxsetting = false;
            lighting = false;


        }


        void update()
        {
            String acommand = status();
            String location = "/home/pi/Debug/Relays.py " + Convert.ToString(acommand);
            //run python script
            ProcessStartInfo info = new ProcessStartInfo("python", location);
            Process.Start(info);

        }

        public String status()
        {
            String statusval = "";

            if (heater)
            {
                statusval = statusval + "1 ";
    
            }
            else
            {

                statusval = statusval + "0 ";
    

            }

            if (auxsetting)
            {
                statusval = statusval + "1 ";
    
            }
            else
            {

                statusval = statusval + "0 ";
    

            }

            if (mainpump)
            {
                statusval = statusval + "1 ";
    
            }
            else
            {

                statusval = statusval + "0 ";
    

            }

            if (auxilarypump)
            {
                statusval = statusval + "1 ";
    
            }
            else
            {

                statusval = statusval + "0 ";
    

            }

            if (lighting)
            {
                statusval = statusval + "1 ";
    
            }
            else
            {

                statusval = statusval + "0 ";
    

            }

            return statusval;

        }

        public void toggle_heater()
        {

            if (heater == true)
            {

                heater = false;

                if (powersavemode == true)
                {

                    mainpump = false;

                }

            }
            else
            {

                heater = true;
                mainpump = true;

            }

            update();

        }


        public void toggle_main()
        {


            if (heater == true)
            {

                // pump must stay on when heater is on
                mainpump = true;

            }
            else
            {


                if (mainpump == true)
                {

                    mainpump = false;

                }
                else
                {

                    mainpump = true;

                }

            }

            update();

        }

        public void toggle_auxilary()
        {

            if (auxilarypump == true)
            {

                auxilarypump = false;
              
            }
            else
            {

                auxilarypump = true;

            }

            update();
        }

        public void toggle_aux_speed()
        {


            if (auxsetting == true)
            {

                auxsetting = false;

            }
            else
            {

                auxsetting = true;

            }

            update();

        }

        public void toggle_lighting()
        {

            if (lighting == true)
            {

                lighting = false;

            }
            else
            {

                lighting = true;

            }

            update();

        }

        public void toggle_partymode()
        {

            if (partymode == true)
            {
                partymode = false;
                auxilarypump = false;
                auxsetting = false;
                powersavemode = true;
                toggle_main();
                lighting = false;

            }
            else
            {

                partymode = true;
                auxilarypump = true;
                auxsetting = true;
                powersavemode = false;
                mainpump = true;
                lighting = true;


            }

            update();

        }

        public void toggle_powersave()
        {

            if (powersavemode == true)
            {

                powersavemode = false;

            }
            else
            {

                powersavemode = true;

            }

        }

        public void Shutdown()
        {

            powersavemode = false;
            mainpump = true;
            heater = false;

            update();

        }

        public void error_antifreeze()
        {

            powersavemode = false;
            mainpump = true;
            heater = true;

            update();

        }

        public void set_Stemp(int newtemp)
        {

            if (newtemp < 110 && newtemp > 55)
            {
                Stemp = newtemp;
            }

        }

        public double get_Stemp()
        {
		
		    return Stemp;

        }

        public double get_Ctemp()
        {

            return Ctemp;

        }

        public void set_Ctemp(double temp)
        {

            Ctemp = temp;

        }

        public bool get_heater_status()
        {

            return heater;

        }

        public bool get_main_status()
        {

            return mainpump;

        }

        public bool get_main_speed_status()
        {

            return auxsetting;

        }

        public bool get_aux_status()
        {

            return auxsetting;
            
        }

        public bool get_lighting_status()
        {

            return lighting;

        }

    }


    class Program
    {

        private readonly static object myLock = new object();

        public static void Log(string logMessage, TextWriter a)
        {
            lock (myLock)
            {
                StreamWriter w = File.AppendText("/home/pi/Debug/Log2.txt");
                w.AutoFlush = true;
                w.Write("\r\nLog Entry : ");
                w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                w.WriteLine("  :");
                w.WriteLine("  :{0}", logMessage);
                w.WriteLine("-------------------------------");
                w.Close();
            }
        }

        static void Main(string[] args)
        {

            StreamWriter w = File.AppendText("log.txt");
            w.AutoFlush = true;

            Hottub TD = new Hottub();
            Display LED = new Display();

            bool Smode = false; // mode to see if we are displaying current temp or setting temp
            bool error = false; //true when an error is detected

            bool btn1 = false; // Up temp button
            bool btn2 = false; // Down temp button
            bool btn3 = false; // Menu scroll air switch
            bool btn4 = false; // Setting scroll air switch

            //IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
            IPAddress ip = IPAddress.Parse("192.168.1.140");
            TcpListener server = new TcpListener(ip, 5001);
            TcpClient client = default(TcpClient);

            ProcessStartInfo info1 = new ProcessStartInfo("python", "/home/pi/Debug/LEDC.py 8 8 8");
            Process.Start(info1);
            Log("StartedLEDs", w);

            ProcessStartInfo info2 = new ProcessStartInfo("python", "/home/pi/Debug/inputs.py");
            Process.Start(info2);
            Log("Now checking for inputs", w);

            Thread.Sleep(2000);


            new Thread(() =>
            {

                //thread to comunicate with button pushing program
                try
                {

                    server.Start();
                    //Console.WriteLine("Server started.... ");
                    Log("input server started", w);

                }
                catch (Exception ex)
                {
                    Log("input server failure", w);
                    Console.WriteLine(ex.ToString());
                    Console.Read();

                }

                while (true)
                {

                    client = server.AcceptTcpClient();

                    Log("client accepted", w);

                    byte[] receivedBuffer = new byte[100];
                    NetworkStream stream = client.GetStream();

                    stream.Read(receivedBuffer, 0, receivedBuffer.Length);

                    string msg = Encoding.ASCII.GetString(receivedBuffer, 0, receivedBuffer.Length);

                    Log("Message received and converted", w);
                    Log(msg, w);

                    try
                    {
                        int count = 0;
                        foreach (char c in msg.ToCharArray())
                        {
                            count++;

                            switch (count)
                            {
                                case 1:
                                    if (c == '1')
                                        btn1 = true;
                                    break;
                                case 2:
                                    if (c == '1')
                                        btn2 = true;
                                    break;
                                case 3:
                                    if (c == '1')
                                        btn3 = true;
                                    break;
                                case 4:
                                    if (c == '1')
                                        btn4 = true;
                                    break;
                                default:
                                    break;
                            }

                        }
                        count = 0;

                        //Console.WriteLine("response " + msg);
                        //Console.WriteLine(btn1 + " " + btn2 + " " + btn3 + " " + btn4);

                    }
                    catch
                    {

                        Log("failed to interpret message", w);

                    }

                }


            }).Start();

            new Thread(() =>
            {

                int prev = 0;
                int current = 0;

                //thread for checking temperature
                while (true)
                {
                    double probe = 0;

                    if (prev != current)
                    {

                        Log("prev: " + prev + " cur: " + current, w);

                    }
                    prev = current;

                    DirectoryInfo devicesDir = new DirectoryInfo("/sys/bus/w1/devices");

                    foreach (var deviceDir in devicesDir.EnumerateDirectories("28*"))
                    {

                        var w1slavetext =
                        deviceDir.GetFiles("w1_slave").FirstOrDefault().OpenText().ReadToEnd();
                        string temptext = w1slavetext.Split(new string[] { "t=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        probe = double.Parse(temptext) / 1000;
                        probe = probe * 1.8 + 32;
                        current = (int)probe;

                    }

                    if ((int)probe != TD.get_Ctemp())
                    {

                        TD.set_Ctemp(probe);

                    }

                    if (probe >= 115)
                    {
                        error = true;
                        TD.Shutdown();
                        Log("overheat detected in probe thread ... activating shutdown protocol", w);
                        //first case shutdown
                    }

                }

            }).Start();

            new Thread(() =>
            {
                //thread listen for button press
                while (true)
                {

                    if (btn1)
                    {
                        btn1 = false;
                        TD.set_Stemp((int)TD.get_Stemp() + 1);
        
                        Smode = true;

                        Log("button one pressed", w);

                    }

                    if (btn2)
                    {
                        btn2 = false;
                        TD.set_Stemp((int)TD.get_Stemp() - 1);
        
                        Smode = true;

                        Log("button 2 pressed", w);

                    }

                    //Log

                }

            }).Start();

            new Thread(() =>
            {

                //display thread
                int prev = 0;
                int cur = 0;

                while (true)
                {

                    prev = cur;
                    cur = (int)TD.get_Ctemp();

                    if (error)
                    {

                        LED.error();
                        Log("LED sending error", w);
                        Thread.Sleep(500);
                    }
                    else
                    {
                        if (!Smode)
                        {
                            LED.set_value(TD.get_Ctemp());
                            if (prev != cur)
                            {
                                LED.display_num();
                            }
                            Thread.Sleep(500);
                        }
                        else
                        {
                            Log("Displaying temp setting", w);
                            Smode = false;
                            int counter = 0;
                           
                            while (counter < 3500)
                            {
                                Thread.Sleep(1);
                                prev = cur;
                                cur = (int)TD.get_Stemp();

                                if (Smode == true)
                                {

                                   counter = 0;
                                   Smode = false;

                                }
                                if (prev != cur)
                                {
                                    LED.set_value(TD.get_Stemp());
                                    LED.display_num();
                                    
                                }
                               counter++;

                            }

                         }

                    }
		
		        }
	
	        }).Start();

            new Thread(() =>
            {
             //thread to handle temp change
             while (true)
             {
               //Log
                if (TD.get_Ctemp() >= 115)
                {

                    Log("temp handler detected temp over 115 .. error code activated ... shutdown initiated", w);
                    error = true;
                    TD.Shutdown();

                    while (error)
                    {

                        if (TD.get_Ctemp() <= 55)
                        {
                            Log("temp is less than 55 in error mode activating antifreeze", w);
                            TD.error_antifreeze();
                            Thread.Sleep(60000);

                        }
                        else
                        {
                            Log("temp restored to above 55 in error mode .. shutting down", w);
                            TD.Shutdown();
                            Thread.Sleep(60000);

                        }

                    }

                    //Log 

                }

                if (TD.get_Ctemp() >= TD.get_Stemp())
                {
                    //Log("temp has reached setting checking heat status", w);
                    if (TD.get_heater_status())
                    {
                        Log("heat status is on shutting heat off", w);
                        TD.toggle_heater();
                        Thread.Sleep(120000);

                    }
                    //Log

                }
                else
                {
                    //Log("temp is below setting ", w);
                    if (!TD.get_heater_status())
                    {
                        Log("turning heat on", w);
                        TD.toggle_heater();
                        Thread.Sleep(120000);

                    }


                }


             }


            }).Start();

            new Thread(() =>
            {

                //thread for pump buttons
                int setting = 0;
                while (true)
                {

                    if (btn3)
                    {
                        Log("setting button pressed", w);
                        btn3 = false;

                        if (setting >= 2)
                        {

                            setting = 0;


                        }
                        else
                        {

                            setting = setting + 1;

                        }


                    }

                    if (btn4)
                    {
                        Log("ativation button pressed", w);
                        btn4 = false;

                        if (setting == 0)
                        {

                            TD.toggle_lighting();

                        }

                        if (setting == 1)
                        {

                            TD.toggle_auxilary();

                        }

                        if (setting == 2)
                        {

                            TD.toggle_aux_speed();

                        }



                    }


                }

            }).Start();


            //CONSOLE DISPLAY
            string choice = "";
            int check = 0;
            while (true)
            {

                Log("Displaying Console", w);
                Console.WriteLine("");
                Console.WriteLine("Enter 1 to see the Ctemp 2 to set the temp 3 to change settings, 4 for a status update and 5 to end the program!");
                choice = "";
                check = 0;
                choice = Console.ReadLine();
                try
                {
                    check = Convert.ToInt32(choice);
                }
                catch
                {

                    check = 0;

                }
               


                if (check == 1)
                {

                    var Key = 1;
                    new Thread(() =>
                    {

                        Console.Clear();
                        while (Key == 1)
                        {
                            Console.SetCursorPosition(0, 0);
                            Console.WriteLine("----------------------------------------------------");
                            Console.WriteLine("Current Reading: " + (int)TD.get_Ctemp() + " !");
                            Console.WriteLine("Current Setting: " + (int)TD.get_Stemp() + " !");
                            Console.WriteLine("Heater Status: " + TD.get_heater_status() + " !");
                            Console.WriteLine("Main Pump Status: " + TD.get_main_status() + " !");
                            Console.WriteLine("----------------------------------------------------");
                            Console.WriteLine("                                                    ");
                            Thread.Sleep(3500);
                            
                            
                        }

                    }).Start();

                    Console.ReadKey();
                    Key = 2;
                    Console.Clear();
                }
                else if (check == 2)
                {

                    Console.WriteLine("Please enter a temp");
                    choice = Console.ReadLine();
                    double checkt = Convert.ToInt32(choice);
                    TD.set_Stemp((int)checkt);
                    Console.WriteLine("Current Setting: " + TD.get_Stemp() + " !");

                }
                else if (check == 3)
                {
                    char[] commands = { '0', '0', '0', '0', '0' };
                    Console.WriteLine("Please insert a binary commandfor the following settings");
                    Console.WriteLine("PartyMode \n PowerSaveMode \n Auxpump \n Auxsetting \n lights");
                    Console.WriteLine("00000 for toggle none and 11111 for toggle all");
                    string command = Console.ReadLine();
                    int count = 0;
                    foreach (char c in command)
                    {

                        if ((c == '1' || c == '0') && count <= 4)
                        {

                            commands[count] = c;
                            
                        }
                        else
                        {

                            break;

                        }
                        count++;

                    }
                    count = 0;

                    if (commands[0] == '1')
                    {

                        TD.toggle_partymode();

                    }

                    if (commands[1] == '1')
                    {

                        TD.toggle_powersave();

                    }

                    if (commands[2] == '1')
                    {

                        TD.toggle_auxilary();

                    }

                    if (commands[3] == '1')
                    {

                        TD.toggle_aux_speed();

                    }

                    if (commands[4] == '1')
                    {

                        TD.toggle_lighting();

                    }


                }
                else if (check == 4)
                {

                    Console.WriteLine("----------------------------------------------------");
                    Console.WriteLine("HeaterStatus = " + TD.get_heater_status());
                    Console.WriteLine("MainStatus = " + TD.get_main_status());
                    Console.WriteLine("MainSpeedStatus = " + TD.get_main_speed_status());
                    Console.WriteLine("AuxStatus = " + TD.get_aux_status());
                    Console.WriteLine("LightStatus = " + TD.get_lighting_status());
                    Console.WriteLine("----------------------------------------------------");
                    Console.WriteLine("                                                    ");
                    Console.Read();


                }
                else if (check == 5)
                {

                    Console.WriteLine("Press any key to end!");
                    Console.ReadKey();
                    Environment.Exit(0);

                }
                else
                {

                    Console.WriteLine("Bad Choice Try Again");

                }
            }
	
	        Console.ReadLine();
	        Console.ReadLine();
	
}

    }
}
