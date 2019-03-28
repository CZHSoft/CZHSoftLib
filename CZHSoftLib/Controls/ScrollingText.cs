using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Threading;

namespace LTControls
{
	/// <summary>
	/// Summary description for ScrollingTextControl.
	/// </summary>
	[
    ToolboxBitmapAttribute(typeof(LTControls.ScrollingText), "ScrollingText.bmp"),
	DefaultEvent("TextClicked")
	]
	public class ScrollingText : System.Windows.Forms.Control
	{
        //private System.Windows.Forms.Timer timer;		// Timer for text animation.
        private Thread animationThread;                 // 2013.01.24 �����ж�
		private string text = "Text";					// Scrolling text
		private float staticTextPos = 0;				// The running x pos of the text
		private float yPos = 0;							// The running y pos of the text
		private ScrollDirection scrollDirection = ScrollDirection.RightToLeft;				// The direction the text will scroll
		private ScrollDirection currentDirection = ScrollDirection.LeftToRight;				// Used for text bouncing 
		private VerticleTextPosition verticleTextPosition = VerticleTextPosition.Center;	// Where will the text be vertically placed
		private int scrollPixelDistance = 2;			// How far the text scrolls per timer event
		private bool showBorder = true;					// Show a border or not
		private bool stopScrollOnMouseOver = false;		// Flag to stop the scroll if the user mouses over the text
		private bool scrollOn = true;					// Internal flag to stop / start the scrolling of the text
		private Brush foregroundBrush = null;			// Allow the user to set a custom Brush to the text Font
		private Brush backgroundBrush = null;			// Allow the user to set a custom Brush to the background
		private Color borderColor = Color.Black;		// Allow the user to set the color of the control border
		private RectangleF lastKnownRect;				// The last known position of the text
        private bool manualMode = false;                // 2013.01.24 �����ж�
        private int textScrollSpeed = 25;            

		public ScrollingText()
		{
			// Setup default properties for ScrollingText control
			InitializeComponent();
			
			//This turns off internal double buffering of all custom GDI+ drawing
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);

			//setup the timer object
            //timer = new Timer();
            //timer.Interval = 25;	//default timer interval
            //timer.Enabled = true;
            //timer.Tick += new EventHandler(Tick);

            animationThread = new Thread(Tick);
            animationThread.IsBackground = true;
            animationThread.Start();

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				//Make sure our brushes are cleaned up
				if (foregroundBrush != null)
					foregroundBrush.Dispose();

				//Make sure our brushes are cleaned up
				if (backgroundBrush != null)
					backgroundBrush.Dispose();

				//Make sure our timer is cleaned up
                //if (timer != null)
                //    timer.Dispose();
                if (animationThread != null)
                {
                    animationThread.Abort();
                    animationThread = null;
                }
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			//ScrollingText			
			this.Name = "ScrollingText";
			this.Size = new System.Drawing.Size(216, 40);
			this.Click += new System.EventHandler(this.ScrollingText_Click);					
		}
		#endregion
	
		//Controls the animation of the text.
		private void Tick(object sender, EventArgs e)
		{
			//update rectangle to include where to paint for new position			
			//lastKnownRect.X -= 10;
			//lastKnownRect.Width += 20;
			
			lastKnownRect.Inflate(10, 5);

			//create region based on updated rectangle
			Region updateRegion = new Region(lastKnownRect);			
			
			//repaint the control			
			Invalidate(updateRegion);
			Update();
		}

        /// <summary>
        /// �߳�
        /// </summary>
        private void Tick()
        {
            while (true)
            {
                lastKnownRect.Inflate(10, 5);

                //create region based on updated rectangle
                Region updateRegion = new Region(lastKnownRect);

                //repaint the control	
                if (this.InvokeRequired)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        Invalidate(updateRegion);
                        Update();
                    }));
                }
                else
                {
                    Invalidate(updateRegion);
                    Update();
                }

                Thread.Sleep(TextScrollSpeed);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag">T:�˹�;F:�Զ�</param>
        public void SetMode(bool flag)
        {
            manualMode = flag;
        }

		//Paint the ScrollingTextCtrl.
		protected override void OnPaint(PaintEventArgs pe)
		{
			//Console.WriteLine(pe.ClipRectangle.X + ",  " + pe.ClipRectangle.Y + ",  " + pe.ClipRectangle.Width + ",  " + pe.ClipRectangle.Height);

			//Paint the text to its new position
			DrawScrollingText(pe.Graphics);

			//pass on the graphics obj to the base Control class
			base.OnPaint(pe);
		}

		//Draw the scrolling text on the control		
		public void DrawScrollingText(Graphics canvas)
		{
			//measure the size of the string for placement calculation
			SizeF stringSize = canvas.MeasureString(this.text, this.Font);
		
			//Calculate the begining x position of where to paint the text
			if (scrollOn)
			{
				CalcTextPosition(stringSize);	
			}

			//Clear the control with user set BackColor
			if (backgroundBrush != null)
			{
				canvas.FillRectangle(backgroundBrush, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
			}
			else
				canvas.Clear(this.BackColor);					

			// Draw the border
			if (showBorder)
				using (Pen borderPen = new Pen(borderColor))
					canvas.DrawRectangle(borderPen, 0, 0, this.ClientSize.Width-1, this.ClientSize.Height-1);					

			// Draw the text string in the bitmap in memory
			if (foregroundBrush == null)					
			{
				using (Brush tempForeBrush = new System.Drawing.SolidBrush(this.ForeColor))																								
					canvas.DrawString(this.text, this.Font, tempForeBrush, staticTextPos, yPos);	
			}
			else
				canvas.DrawString(this.text, this.Font, foregroundBrush, staticTextPos, yPos);						

			lastKnownRect = new RectangleF(staticTextPos, yPos, stringSize.Width, stringSize.Height);			
			EnableTextLink(lastKnownRect);
		}

		private void CalcTextPosition(SizeF stringSize)
		{
			switch (scrollDirection)
			{
				case ScrollDirection.RightToLeft:
                    if (staticTextPos < (-1 * (stringSize.Width)))
                    {
                        staticTextPos = this.ClientSize.Width - 1;

                        if (manualMode)
                        {
                            scrollOn = false;
                        }
                    }
                    else
                        staticTextPos -= scrollPixelDistance;
					break;
				case ScrollDirection.LeftToRight:
                    if (staticTextPos > this.ClientSize.Width)
                    {
                        staticTextPos = -1 * stringSize.Width;

                        if (manualMode)
                        {
                            scrollOn = false;
                        }
                    }
                    else
                        staticTextPos += scrollPixelDistance;
					break;
				case ScrollDirection.Bouncing:
					if (currentDirection == ScrollDirection.RightToLeft)
					{
						if (staticTextPos < 0)
							currentDirection = ScrollDirection.LeftToRight;
						else
							staticTextPos -= scrollPixelDistance;						
					}
					else if (currentDirection == ScrollDirection.LeftToRight)
					{
						if (staticTextPos > this.ClientSize.Width - stringSize.Width)
							currentDirection = ScrollDirection.RightToLeft;
						else
							staticTextPos += scrollPixelDistance;						
					}
					break;
			}				

			//Calculate the vertical position for the scrolling text				
			switch (verticleTextPosition)
			{
				case VerticleTextPosition.Top:
					yPos = 2;

                    if (manualMode)
                    {
                        scrollOn = false;
                    }

					break;
				case VerticleTextPosition.Center:
					yPos = (this.ClientSize.Height / 2) - (stringSize.Height / 2);
					break;
				case VerticleTextPosition.Botom:
					yPos = this.ClientSize.Height - stringSize.Height;

                    if (manualMode)
                    {
                        scrollOn = false;
                    }

					break;
			}
		}

		#region Mouse over, text link logic
		private void EnableTextLink(RectangleF textRect)
		{
			Point curPt = this.PointToClient(Cursor.Position);

			//if (curPt.X > textRect.Left && curPt.X < textRect.Right
			//	&& curPt.Y > textRect.Top && curPt.Y < textRect.Bottom)			
			if (textRect.Contains(curPt))
			{
				//Stop the text of the user mouse's over the text
				if (stopScrollOnMouseOver)
					scrollOn = false;				

				this.Cursor = Cursors.Hand;									
			}
			else
			{
				//Make sure the text is scrolling if user's mouse is not over the text

                //scrollOn = true;
                if (!manualMode)
                {
                    scrollOn = true;
                }
				this.Cursor = Cursors.Default;
			}
		}		

		private void ScrollingText_Click(object sender, System.EventArgs e)
		{
			//Trigger the text clicked event if the user clicks while the mouse 
			//is over the text.  This allows the text to act like a hyperlink
			if (this.Cursor == Cursors.Hand)
				OnTextClicked(this, new EventArgs());
		}

		public delegate void TextClickEventHandler(object sender, EventArgs args);	
		public event TextClickEventHandler TextClicked;

		private void OnTextClicked(object sender, EventArgs args)
		{
			//Call the delegate
            if (TextClicked != null)
            {
                TextClicked(sender, args);
                scrollOn = true;
            }
		}
		#endregion
		

		#region Properties
        [
        Browsable(true),
        CategoryAttribute("Scrolling Text"),
        Description("The timer interval that determines how often the control is repainted")
        ]
        public int TextScrollSpeed
        {
            set
            {
                textScrollSpeed = value;
            }
            get
            {
                return textScrollSpeed;
            }
        }

		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("How many pixels will the text be moved per Paint")
		]
		public int TextScrollDistance
		{
			set
			{
				scrollPixelDistance = value;
			}
			get
			{
				return scrollPixelDistance;
			}
		}

		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("The text that will scroll accros the control")
		]
		public string ScrollText
		{
			set
			{
				text = value;
				this.Invalidate();
				this.Update();
                scrollOn = true;
			}
			get
			{
				return text;
			}
		}

		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("What direction the text will scroll: Left to Right, Right to Left, or Bouncing")
		]
		public ScrollDirection ScrollDirection
		{
			set
			{
				scrollDirection = value;
			}
			get
			{
				return scrollDirection;
			}
		}

		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("The verticle alignment of the text")
		]
		public VerticleTextPosition VerticleTextPosition
		{
			set
			{
				verticleTextPosition = value;
			}
			get
			{
				return verticleTextPosition;
			}
		}

		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("Turns the border on or off")
		]
		public bool ShowBorder
		{
			set
			{
				showBorder = value;
			}
			get
			{
				return showBorder;
			}
		}

		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("The color of the border")
		]
		public Color BorderColor
		{
			set
			{
				borderColor = value;
			}
			get
			{
				return borderColor;
			}
		}

		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("Determines if the text will stop scrolling if the user's mouse moves over the text")
		]
		public bool StopScrollOnMouseOver
		{
			set
			{
				stopScrollOnMouseOver = value;
			}
			get
			{
				return stopScrollOnMouseOver;
			}
		}

		[
		Browsable(false)
		]
		public Brush ForegroundBrush
		{
			set
			{
				foregroundBrush = value;
			}
			get
			{
				return foregroundBrush;
			}
		}
		
		[
		ReadOnly(true)
		]
		public Brush BackgroundBrush
		{
			set
			{
				backgroundBrush = value;
			}
			get
			{
				return backgroundBrush;
			}
		}
		#endregion		
	}

	public enum ScrollDirection
	{
		RightToLeft,
        LeftToRight,
        Bouncing
	}

	public enum VerticleTextPosition
	{
		Top,
        Center,
        Botom
	}
}
