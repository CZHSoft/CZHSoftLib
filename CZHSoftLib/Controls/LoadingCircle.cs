using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace LTControls
{
    public partial class LoadingCircle : Control
    {
        #region ����

        private const double NumberOfDegreesInCircle = 360;
        private const double NumberOfDegreesInHalfCircle = NumberOfDegreesInCircle / 2;
        private const int DefaultInnerCircleRadius = 8;
        private const int DefaultOuterCircleRadius = 10;
        private const int DefaultNumberOfSpoke = 10;
        private const int DefaultSpokeThickness = 4;
        private readonly Color DefaultColor = Color.DarkGray;

        private const int MacOSXInnerCircleRadius = 5;
        private const int MacOSXOuterCircleRadius = 15;
        private const int MacOSXNumberOfSpoke = 12;
        private const int MacOSXSpokeThickness = 2;

        private const int FireFoxInnerCircleRadius = 6;
        private const int FireFoxOuterCircleRadius = 7;
        private const int FireFoxNumberOfSpoke = 9;
        private const int FireFoxSpokeThickness = 4;

        private const int IE7InnerCircleRadius = 8;
        private const int IE7OuterCircleRadius = 9;
        private const int IE7NumberOfSpoke = 24;
        private const int IE7SpokeThickness = 4;

        #endregion

        #region ö��

        public enum StylePresets
        {
            MacOSX,
            Firefox,
            IE7,
            Custom
        }

        #endregion

        #region �ֲ�����

        private Timer m_Timer;
        private bool m_IsTimerActive;
        private int m_NumberOfSpoke;
        private int m_SpokeThickness;
        private int m_ProgressValue;
        private int m_OuterCircleRadius;
        private int m_InnerCircleRadius;
        private PointF m_CenterPoint;
        private Color m_Color;
        private Color[] m_Colors;
        private double[] m_Angles;
        private StylePresets m_StylePreset;

        #endregion

        #region ����

        /// <summary>
        /// ��ȡ�����ÿؼ�����ɫ
        /// </summary>
        /// <value>����ɫ</value>
        [TypeConverter("System.Drawing.ColorConverter"),
        Category("LoadingCircle"),
        Description("��ȡ�����ÿؼ�����ɫ")]
        public Color Color
        {
            get
            {
                return m_Color;
            }
            set
            {
                m_Color = value;

                GenerateColorsPallet();
                Invalidate();
            }
        }

        /// <summary>
        /// ��ȡ��������Χ�뾶
        /// </summary>
        /// <value>��Χ�뾶</value>
        [System.ComponentModel.Description("��ȡ��������Χ�뾶"),
         System.ComponentModel.Category("LoadingCircle")]
        public int OuterCircleRadius
        {
            get
            {
                if (m_OuterCircleRadius == 0)
                    m_OuterCircleRadius = DefaultOuterCircleRadius;

                return m_OuterCircleRadius;
            }
            set
            {
                m_OuterCircleRadius = value;
                Invalidate();
            }
        }

        /// <summary>
        /// ��ȡ��������Բ�뾶
        /// </summary>
        /// <value>��Բ�뾶</value>
        [System.ComponentModel.Description("��ȡ��������Բ�뾶"),
         System.ComponentModel.Category("LoadingCircle")]
        public int InnerCircleRadius
        {
            get
            {
                if (m_InnerCircleRadius == 0)
                    m_InnerCircleRadius = DefaultInnerCircleRadius;

                return m_InnerCircleRadius;
            }
            set
            {
                m_InnerCircleRadius = value;
                Invalidate();
            }
        }

        /// <summary>
        /// ��ȡ�����÷�������
        /// </summary>
        /// <value>��������</value>
        [System.ComponentModel.Description("��ȡ�����÷�������"),
        System.ComponentModel.Category("LoadingCircle")]
        public int NumberSpoke
        {
            get
            {
                if (m_NumberOfSpoke == 0)
                    m_NumberOfSpoke = DefaultNumberOfSpoke;

                return m_NumberOfSpoke;
            }
            set
            {
                if (m_NumberOfSpoke != value && m_NumberOfSpoke > 0)
                {
                    m_NumberOfSpoke = value;
                    GenerateColorsPallet();
                    GetSpokesAngles();

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// ��ȡ������һ������ֵ����ʾ��ǰ�ؼ�<see cref="T:LoadingCircle"/>�Ƿ񼤻
        /// </summary>
        /// <value><c>true</c> ��ʾ�������Ϊ<c>false</c>��</value>
        [System.ComponentModel.Description("��ȡ������һ������ֵ����ʾ��ǰ�ؼ��Ƿ񼤻"),
        System.ComponentModel.Category("LoadingCircle")]
        public bool Active
        {
            get
            {
                return m_IsTimerActive;
            }
            set
            {
                m_IsTimerActive = value;
                ActiveTimer();
            }
        }

        /// <summary>
        /// ��ȡ�����÷�����ϸ�̶ȡ�
        /// </summary>
        /// <value>������ϸֵ</value>
        [System.ComponentModel.Description("��ȡ�����÷�����ϸ�̶ȡ�"),
        System.ComponentModel.Category("LoadingCircle")]
        public int SpokeThickness
        {
            get
            {
                if (m_SpokeThickness <= 0)
                    m_SpokeThickness = DefaultSpokeThickness;

                return m_SpokeThickness;
            }
            set
            {
                m_SpokeThickness = value;
                Invalidate();
            }
        }

        /// <summary>
        /// ��ȡ��������ת�ٶȡ�
        /// </summary>
        /// <value>��ת�ٶ�</value>
        [System.ComponentModel.Description("��ȡ��������ת�ٶȡ�"),
        System.ComponentModel.Category("LoadingCircle")]
        public int RotationSpeed
        {
            get
            {
                return m_Timer.Interval;
            }
            set
            {
                if (value > 0)
                    m_Timer.Interval = value;
            }
        }

        /// <summary>
        /// ��������Ԥ������
        /// </summary>
        /// <value>����ֵ</value>
        [Category("LoadingCircle"),
        Description("��������Ԥ������"),
         DefaultValue(typeof(StylePresets), "Custom")]
        public StylePresets StylePreset
        {
            get { return m_StylePreset; }
            set
            {
                m_StylePreset = value;

                switch (m_StylePreset)
                {
                    case StylePresets.MacOSX:
                        SetCircleAppearance(MacOSXNumberOfSpoke,
                            MacOSXSpokeThickness, MacOSXInnerCircleRadius,
                            MacOSXOuterCircleRadius);
                        break;
                    case StylePresets.Firefox:
                        SetCircleAppearance(FireFoxNumberOfSpoke,
                            FireFoxSpokeThickness, FireFoxInnerCircleRadius,
                            FireFoxOuterCircleRadius);
                        break;
                    case StylePresets.IE7:
                        SetCircleAppearance(IE7NumberOfSpoke,
                            IE7SpokeThickness, IE7InnerCircleRadius,
                            IE7OuterCircleRadius);
                        break;
                    case StylePresets.Custom:
                        SetCircleAppearance(DefaultNumberOfSpoke,
                            DefaultSpokeThickness,
                            DefaultInnerCircleRadius,
                            DefaultOuterCircleRadius);
                        break;
                }
            }
        }

        #endregion

        #region ���캯�����¼�����

        public LoadingCircle()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            m_Color = DefaultColor;

            GenerateColorsPallet();
            GetSpokesAngles();
            GetControlCenterPoint();

            m_Timer = new Timer();
            m_Timer.Tick += new EventHandler(aTimer_Tick);
            ActiveTimer();

            this.Resize += new EventHandler(LoadingCircle_Resize);
        }

        void LoadingCircle_Resize(object sender, EventArgs e)
        {
            GetControlCenterPoint();
        }

        void aTimer_Tick(object sender, EventArgs e)
        {
            m_ProgressValue = ++m_ProgressValue % m_NumberOfSpoke;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_NumberOfSpoke > 0)
            {
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                int intPosition = m_ProgressValue;
                for (int intCounter = 0; intCounter < m_NumberOfSpoke; intCounter++)
                {
                    intPosition = intPosition % m_NumberOfSpoke;
                    DrawLine(e.Graphics,
                             GetCoordinate(m_CenterPoint, m_InnerCircleRadius, m_Angles[intPosition]),
                             GetCoordinate(m_CenterPoint, m_OuterCircleRadius, m_Angles[intPosition]),
                             m_Colors[intCounter], m_SpokeThickness);
                    intPosition++;
                }
            }

            base.OnPaint(e);
        }

        #endregion

        #region �ֲ�����

        private Color Darken(Color _objColor, int _intPercent)
        {
            int intRed = _objColor.R;
            int intGreen = _objColor.G;
            int intBlue = _objColor.B;
            return Color.FromArgb(_intPercent, Math.Min(intRed, byte.MaxValue), Math.Min(intGreen, byte.MaxValue), Math.Min(intBlue, byte.MaxValue));
        }

        private void GenerateColorsPallet()
        {
            m_Colors = GenerateColorsPallet(m_Color, Active, m_NumberOfSpoke);
        }

        private Color[] GenerateColorsPallet(Color _objColor, bool _blnShadeColor, int _intNbSpoke)
        {
            Color[] objColors = new Color[NumberSpoke];

            byte bytIncrement = (byte)(byte.MaxValue / NumberSpoke);

            byte PERCENTAGE_OF_DARKEN = 0;

            for (int intCursor = 0; intCursor < NumberSpoke; intCursor++)
            {
                if (_blnShadeColor)
                {
                    if (intCursor == 0 || intCursor < NumberSpoke - _intNbSpoke)
                        objColors[intCursor] = _objColor;
                    else
                    {
                        PERCENTAGE_OF_DARKEN += bytIncrement;

                        if (PERCENTAGE_OF_DARKEN > byte.MaxValue)
                            PERCENTAGE_OF_DARKEN = byte.MaxValue;

                        objColors[intCursor] = Darken(_objColor, PERCENTAGE_OF_DARKEN);
                    }
                }
                else
                    objColors[intCursor] = _objColor;
            }

            return objColors;
        }

        private void GetControlCenterPoint()
        {
            m_CenterPoint = GetControlCenterPoint(this);
        }

        private PointF GetControlCenterPoint(Control _objControl)
        {
            return new PointF(_objControl.Width / 2, _objControl.Height / 2 - 1);
        }

        private void DrawLine(Graphics _objGraphics, PointF _objPointOne, PointF _objPointTwo,
                              Color _objColor, int _intLineThickness)
        {
            using (Pen objPen = new Pen(new SolidBrush(_objColor), _intLineThickness))
            {
                objPen.StartCap = LineCap.Round;
                objPen.EndCap = LineCap.Round;
                _objGraphics.DrawLine(objPen, _objPointOne, _objPointTwo);
            }
        }

        private PointF GetCoordinate(PointF _objCircleCenter, int _intRadius, double _dblAngle)
        {
            double dblAngle = Math.PI * _dblAngle / NumberOfDegreesInHalfCircle;

            return new PointF(_objCircleCenter.X + _intRadius * (float)Math.Cos(dblAngle),
                              _objCircleCenter.Y + _intRadius * (float)Math.Sin(dblAngle));
        }

        private void GetSpokesAngles()
        {
            m_Angles = GetSpokesAngles(NumberSpoke);
        }

        private double[] GetSpokesAngles(int _intNumberSpoke)
        {
            double[] Angles = new double[_intNumberSpoke];
            double dblAngle = (double)NumberOfDegreesInCircle / _intNumberSpoke;

            for (int shtCounter = 0; shtCounter < _intNumberSpoke; shtCounter++)
                Angles[shtCounter] = (shtCounter == 0 ? dblAngle : Angles[shtCounter - 1] + dblAngle);

            return Angles;
        }

        private void ActiveTimer()
        {
            if (m_IsTimerActive)
                m_Timer.Start();
            else
            {
                m_Timer.Stop();
                m_ProgressValue = 0;
            }

            GenerateColorsPallet();
            Invalidate();
        }

        #endregion

        #region ȫ�ַ���

        /// <summary>
        /// ��ȡ�ʺϿؼ�����ľ��δ�С��
        /// </summary>
        /// <param name="proposedSize">The custom-sized area for a control.</param>
        /// <returns>
        /// An ordered pair of type <see cref="T:System.Drawing.Size"></see> representing the width and height of a rectangle.
        /// </returns>
        public override Size GetPreferredSize(Size proposedSize)
        {
            proposedSize.Width =
                (m_OuterCircleRadius + m_SpokeThickness) * 2;

            return proposedSize;
        }

        /// <summary>
        /// ���ÿؼ������
        /// </summary>
        /// <param name="numberSpoke">����</param>
        /// <param name="spokeThickness">��ϸ</param>
        /// <param name="innerCircleRadius">��Բ�뾶</param>
        /// <param name="outerCircleRadius">��Բ�뾶</param>
        public void SetCircleAppearance(int numberSpoke, int spokeThickness,
            int innerCircleRadius, int outerCircleRadius)
        {
            NumberSpoke = numberSpoke;
            SpokeThickness = spokeThickness;
            InnerCircleRadius = innerCircleRadius;
            OuterCircleRadius = outerCircleRadius;

            Invalidate();
        }

        #endregion
    }
}