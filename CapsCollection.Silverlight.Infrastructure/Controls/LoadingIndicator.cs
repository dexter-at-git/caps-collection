using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CapsCollection.Silverlight.Infrastructure.Controls
{
    [TemplatePart(Name = PART_AnimationElementContainer, Type = typeof(Canvas))]
    [TemplatePart(Name = PART_AnimationElement, Type = typeof(FrameworkElement))]
    public class LoadingIndicator : Control
    {
        // Template parts
        private const string PART_AnimationElementContainer = "PART_AnimationElementContainer";
        private const string PART_AnimationElement = "PART_AnimationElement";

        // Default values
        private const double EndOpacity = 0.1;
        private const double DefaultDurationInSeconds = 1;
        private const int DefaultCount = 12;

        #region public TimeSpan Duration

        /// <summary>
        /// Gets or sets Duration of one animation cycle.
        /// </summary>
        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LoadingIndicator.Duration" /> dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="LoadingIndicator.Duration" /> dependency property.</value>
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register(
            "Duration",
            typeof(TimeSpan),
            typeof(LoadingIndicator),
            new PropertyMetadata(TimeSpan.FromSeconds(DefaultDurationInSeconds), new PropertyChangedCallback(OnDurationPropertyChanged)));

        /// <summary>
        /// Called when Duration property is changed.
        /// </summary>
        /// <param name="d">LoadingIndicator object whose Duration property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnDurationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LoadingIndicator ctl = (LoadingIndicator)d;
            ctl.CreateAnimation();
        }

        #endregion

        #region public int Count

        /// <summary>
        /// Gets or sets Count of animated elements.
        /// </summary>
        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LoadingIndicator.Count" /> dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="LoadingIndicator.Count" /> dependency property.</value>
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register(
            "Count",
            typeof(int),
            typeof(LoadingIndicator),
            new PropertyMetadata(DefaultCount, new PropertyChangedCallback(OnCountPropertyChanged)));

        /// <summary>
        /// Check Count property and redraw control with new parameters.
        /// </summary>
        /// <param name="d">LoadingIndicator object whose Count property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LoadingIndicator ctl = (LoadingIndicator)d;

            int count = (int)e.NewValue;
            if (count <= 0)
                ctl.Count = DefaultCount;

            ctl.CreateAnimation();
        }

        #endregion

        #region private bool ControlVisibility

        /// <summary>
        /// Gets or sets Control Visibility.
        /// </summary>
        private bool ControlVisibility
        {
            get { return (bool)GetValue(ControlVisibilityProperty); }
            set { SetValue(ControlVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LoadingIndicator.ControlVisibility" /> dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="LoadingIndicator.ControlVisibility" /> dependency property.</value>
        private static readonly DependencyProperty ControlVisibilityProperty =
            DependencyProperty.Register(
            "ControlVisibility",
            typeof(Visibility),
            typeof(LoadingIndicator),
            new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnControlVisibilityPropertyChanged)));

        /// <summary>
        /// Stop animation when control becomes collapsed and create it anew - when visible.
        /// </summary>
        /// <param name="d">LoadingIndicator object whose ControlVisibility property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnControlVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LoadingIndicator ctl = (LoadingIndicator)d;

            Visibility visibility = (Visibility)e.NewValue;
            if (ctl.animationElementContainer != null)
            {
                if (visibility == Visibility.Collapsed)
                {
                    ctl.StopAnimation();
                }
                else
                {
                    ctl.StartAnimation();
                }
            }
        }

        #endregion

        #region public DataTemplate AnimationElementTemplate

        /// <summary>
        /// Gets or sets AnimationElementTemplate.
        /// </summary>
        public DataTemplate AnimationElementTemplate
        {
            get { return (DataTemplate)GetValue(AnimationElementTemplateProperty); }
            set { SetValue(AnimationElementTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LoadingIndicator.AnimationElementTemplate" /> dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="LoadingIndicator.AnimationElementTemplate" /> dependency property.</value>
        public static readonly DependencyProperty AnimationElementTemplateProperty =
            DependencyProperty.Register(
            "AnimationElementTemplate",
            typeof(DataTemplate),
            typeof(LoadingIndicator), new PropertyMetadata(null));

        #endregion

        private Canvas animationElementContainer { get; set; }
        private FrameworkElement animationElement { get; set; }
        private List<Storyboard> storyboardList { get; set; }
        private double innerRadius { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingIndicator"/> class.
        /// </summary>
        public LoadingIndicator()
        {
            this.DefaultStyleKey = typeof(LoadingIndicator);
            storyboardList = new List<Storyboard>();
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        public void StartAnimation()
        {
            if (storyboardList.Count == 0)
                CreateAnimation();
            else
                storyboardList.ForEach(x => x.Begin());
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public void StopAnimation()
        {
            storyboardList.ForEach(x => x.Stop());
        }

        /// <summary>
        /// Builds the visual tree when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            animationElementContainer = GetTemplateChild(PART_AnimationElementContainer) as Canvas;

            if (animationElementContainer == null)
                throw new NotImplementedException("Template PART_AnimationElementContainer is required to display LoadingIndicator.");

            animationElement = (FrameworkElement)AnimationElementTemplate.LoadContent();
            if (animationElement == null)
                throw new NotImplementedException("AnimationElementTemplate is required to display LoadingIndicator.");

            // Calculate inner radius of the indicator
            double outerRadius = Math.Min(Width, Height);
            innerRadius = outerRadius / 2 - Math.Max(animationElement.Width, animationElement.Height);

            CreateAnimation();
        }

        /// <summary>
        /// Copy base animation element Count times and start animation.
        /// </summary>
        private void CreateAnimation()
        {
            if (animationElementContainer != null)
            {
                animationElementContainer.Children.Clear();
                storyboardList.Clear();

                double angle = 360.0 / this.Count;
                double width = animationElement.Width;
                double x = (Width - width) / 2;
                double y = Height / 2 + innerRadius;

                for (int i = 0; i < this.Count; i++)
                {
                    // Copy base element
                    FrameworkElement element = (FrameworkElement)AnimationElementTemplate.LoadContent();
                    element.Opacity = 0;

                    TranslateTransform tt = new TranslateTransform { X = x, Y = y };
                    RotateTransform rt = new RotateTransform { Angle = i * angle + 180, CenterX = (width / 2), CenterY = -innerRadius };
                    TransformGroup tg = new TransformGroup();
                    tg.Children.Add(rt);
                    tg.Children.Add(tt);
                    element.RenderTransform = tg;

                    animationElementContainer.Children.Add(element);

                    DoubleAnimation animation = new DoubleAnimation
                                                {
                        From = animationElement.Opacity,
                        To = EndOpacity,
                        Duration = this.Duration,
                        RepeatBehavior = RepeatBehavior.Forever,
                        BeginTime = TimeSpan.FromMilliseconds((this.Duration.TotalMilliseconds / this.Count) * i)
                    };

                    Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
                    Storyboard.SetTarget(animation, element);

                    Storyboard sb = new Storyboard();
                    sb.Children.Add(animation);
                    sb.Begin();

                    storyboardList.Add(sb);
                }

                // Bind ControlVisibilityProperty to the Visibility property 
                // in order to catch missing OnVisibilityChanged event
                Binding binding = new Binding
                                  {
                    Source = this,
                    Path = new PropertyPath("Visibility")
                };

                this.SetBinding(LoadingIndicator.ControlVisibilityProperty, binding);
            }
        }
    }
}
