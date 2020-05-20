﻿//#define THREADED_RENDERER

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading;

using Y2Snes.Core;
using SnesDebugger;

namespace WinFormRender
{
    public partial class RenderWindow : Form
    {
        SuperFamicom snes;
        SnesDebugConsole dbgConsole;
       
        ConsoleWindow consoleWindow;

        Stopwatch timer = new Stopwatch();
        long elapsedMs;
        long elapsedMsBgWin;
        int framesDrawn;
        int fps;

        Rectangle gameRect;
        Rectangle fpsRect;
        Point fpsPt;

        MenuStrip menu;

#if THREADED_RENDERER
        bool drawFrame = false;
        bool exitThread = false;
        Thread renderThread;
#endif 

        BufferedGraphicsContext gfxBufferedContext;
        BufferedGraphics gfxBuffer;


        public RenderWindow()
        {
            InitializeComponent();
            
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            snes = new SuperFamicom();
            snes.PowerOn();
            //dmg.OnFrame = () => this.Draw();

            dbgConsole = new SnesDebugConsole(snes);
            
            consoleWindow = new ConsoleWindow(snes, dbgConsole);
            consoleWindow.Show();


            this.Text = snes.rom.RomName;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;

      /*      
            Controls.Add(menu = new MenuStrip
            {
                Items =
                {
                    new ToolStripMenuItem("Emulator")
                    {
                        DropDownItems =
                        {
                            new ToolStripMenuItem("Load ROM", null, (sender, args) => { LoadRom(); }),
                            new ToolStripMenuItem("Pause", null, (sender, args) => {  }),
                            new ToolStripMenuItem("Quit", null, (sender, args) => { Application.Exit(); })
                        }
                    },
                    new ToolStripMenuItem("Window")
                    {
                        DropDownItems =
                        {
                            new ToolStripMenuItem("Console", null, (sender, args) => { consoleWindow.Visible = !consoleWindow.Visible; }),
                            new ToolStripMenuItem("Bg Viewer", null, (sender, args) => { bgWnd.Visible = !bgWnd.Visible; })
                        }
                    },
                     new ToolStripMenuItem("Help")
                    {
                        DropDownItems =
                        {
                            new ToolStripMenuItem("About", null, (sender, args) => {  })
                        }
                    }
                }
            });

    */
            // 4X gameboy resolution
            Width = 640;
            Height = 576; // + menu.Height;
            DoubleBuffered = true;
            

            System.Windows.Forms.Application.Idle += new EventHandler(OnApplicationIdle);

            timer.Start();

#if THREADED_RENDERER
            drawFrame = false;
            renderThread = new Thread(new ThreadStart(RenderThread));
            renderThread.Start();
#endif
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            consoleWindow.Location = new Point(Location.X + Width + 20, Location.Y);
            
            // Gets a reference to the current BufferedGraphicsContext
            gfxBufferedContext = BufferedGraphicsManager.Current;

            // Creates a BufferedGraphics instance associated with this form, and with dimensions the same size as the drawing surface of Form1.
            gfxBuffer = gfxBufferedContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            fpsRect = new Rectangle(ClientRectangle.Width - 75, 5, 55, 30);
            fpsPt = new Point(ClientRectangle.Width - 75, 10);

            gameRect = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);
            if(menu != null)
            {
                fpsPt.Y = 10 + menu.Height; 
                fpsRect.Y = 5 + menu.Height;
                gameRect.Y = menu.Height;
                gameRect.Height -= menu.Height;
            }

            if (gfxBufferedContext != null)
            {
                gfxBuffer = gfxBufferedContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);
            }
        }

        private void OnKeyDown(Object o, KeyEventArgs a)
        {
            /*
            if (InvokeRequired)
            {
                Invoke(new Action<object, KeyEventArgs>(OnKeyDown), o, a);
                return;
            }
            if (a.KeyCode == Keys.Up) dmg.pad.UpdateKeyState(Joypad.GbKey.Up, true);
            else if (a.KeyCode == Keys.Down) dmg.pad.UpdateKeyState(Joypad.GbKey.Down, true);
            else if (a.KeyCode == Keys.Left) dmg.pad.UpdateKeyState(Joypad.GbKey.Left, true);
            else if (a.KeyCode == Keys.Right) dmg.pad.UpdateKeyState(Joypad.GbKey.Right, true);
            else if (a.KeyCode == Keys.Z) dmg.pad.UpdateKeyState(Joypad.GbKey.B, true);
            else if (a.KeyCode == Keys.X) dmg.pad.UpdateKeyState(Joypad.GbKey.A, true);
            else if (a.KeyCode == Keys.Enter) dmg.pad.UpdateKeyState(Joypad.GbKey.Start, true);
            else if (a.KeyCode == Keys.Back) dmg.pad.UpdateKeyState(Joypad.GbKey.Select, true);
            */
        }

        private void OnKeyUp(Object o, KeyEventArgs a)
        {
            /*
            if (InvokeRequired)
            {
                Invoke(new Action<object, KeyEventArgs>(OnKeyUp), o, a);
                return;
            }

            if (a.KeyCode == Keys.Up) dmg.pad.UpdateKeyState(Joypad.GbKey.Up, false);
            else if (a.KeyCode == Keys.Down) dmg.pad.UpdateKeyState(Joypad.GbKey.Down, false);
            else if (a.KeyCode == Keys.Left) dmg.pad.UpdateKeyState(Joypad.GbKey.Left, false);
            else if (a.KeyCode == Keys.Right) dmg.pad.UpdateKeyState(Joypad.GbKey.Right, false);
            else  if (a.KeyCode == Keys.Z) dmg.pad.UpdateKeyState(Joypad.GbKey.B, false);
            else if (a.KeyCode == Keys.X) dmg.pad.UpdateKeyState(Joypad.GbKey.A, false);
            else if (a.KeyCode == Keys.Enter) dmg.pad.UpdateKeyState(Joypad.GbKey.Start, false);
            else if (a.KeyCode == Keys.Back) dmg.pad.UpdateKeyState(Joypad.GbKey.Select, false);
            */
        }

        
        private void OnApplicationIdle(object sender, EventArgs e)
        {

            while (IsApplicationIdle())
            {
                if (timer.ElapsedMilliseconds - elapsedMs >= 1000)
                {
                    elapsedMs = timer.ElapsedMilliseconds;

                    fps = framesDrawn;
                    framesDrawn = 0;
                }

               
                if (dbgConsole.SnesMode == SnesDebugConsole.Mode.Running)
                {
                    snes.Step();

                    if (dbgConsole.CheckForBreakpoints())
                    {
                        consoleWindow.RefreshDmgSnapshot();
                    }
                }

                else if (dbgConsole.SnesMode == SnesDebugConsole.Mode.BreakPoint &&
                            dbgConsole.BreakpointStepAvailable)
                {
                    dbgConsole.OnPreBreakpointStep();
                    snes.Step();
                    dbgConsole.PeekSequentialInstructions();
                    dbgConsole.OnPostBreakpointStep();
                    consoleWindow.RefreshDmgSnapshot();
                    consoleWindow.RefreshConsoleText();
                }              
                
            }
        }


#if THREADED_RENDERER
        private void RenderThread()
        {
            var whiteBrush = new SolidBrush(Color.White);
            var redBrush = new SolidBrush(Color.Red);
            var amberBrush = new SolidBrush(Color.Orange);
            var greenBrush = new SolidBrush(Color.Green);
            var font = new Font("Verdana", 8);                                   

            while (exitThread == false)
            {
                if (drawFrame)
                {
                    framesDrawn++;

                    lock (dmg.FrameBuffer)
                    {
                    
                        gfxBuffer.Graphics.DrawImage(dmg.FrameBuffer, gameRect);


                        // Only show fps if we are dipping and then use a colour code
                        if (fps < 58)
                        {
                            var brush = redBrush;
                            if (fps >= 50) brush = greenBrush;
                            else if (fps >= 40) brush = amberBrush;

                            gfxBuffer.Graphics.FillRectangle(brush, fpsRect);
                            gfxBuffer.Graphics.DrawString(String.Format("{0:D2} fps", fps), font, whiteBrush, fpsPt);
                        }

                        gfxBuffer.Render();
                    }
                    drawFrame = false;
                }
            }
        }


        private void Draw()
        {

            // If the Bg viewer is open, then update it at 5fps
            if (bgWnd.Visible &&
                timer.ElapsedMilliseconds - elapsedMsBgWin >= (200))
            {
                elapsedMsBgWin = timer.ElapsedMilliseconds;
                bgWnd.RenderBg();
            }             

            // Wait for previous frame to finish drawing while also locking to 60fps
            while (drawFrame)
            {
            }
            drawFrame = true;
        }

#else
        private void Draw()
        {
            /*
            framesDrawn++;                
            gfxBuffer.Graphics.DrawImage(dmg.FrameBuffer, new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height));
         

            gfxBuffer.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(ClientRectangle.Width -75, 5, 55, 30));
            gfxBuffer.Graphics.DrawString(String.Format("{0:D2} fps", fps), new Font("Verdana", 8),  new SolidBrush(Color.Black), new Point(ClientRectangle.Width - 75, 10));

            gfxBuffer.Render();  
            */
        }
#endif

        private void OnApplicationExit(object sender, EventArgs e)
        {
            //dmg.rom.SaveMbc1BatteryBackData();
            //exitThread = true;
            //Thread.Sleep(500);
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr Handle;
            public uint Message;
            public IntPtr WParameter;
            public IntPtr LParameter;
            public uint Time;
            public Point Location;
        }

        bool IsApplicationIdle()
        {
            NativeMessage result;
            return PeekMessage(out result, IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
        }

        [DllImport("user32.dll")]
        public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);
    }




}
