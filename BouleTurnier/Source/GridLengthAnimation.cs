using System;
using System.Windows;
using System.Windows.Media.Animation;
public class GridLengthAnimation : AnimationTimeline
{
    public override Type TargetPropertyType
    {
        get { return typeof(GridLength); }
    }

    protected override System.Windows.Freezable CreateInstanceCore()
    {
        return new GridLengthAnimation();
    }

    public static readonly DependencyProperty FromProperty =
        DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));

    public GridLength From
    {
        get
        {
            return (GridLength)GetValue(GridLengthAnimation.FromProperty);
        }
        set
        {
            SetValue(GridLengthAnimation.FromProperty, value);
        }
    }

    private double FromAsDouble
    {
        get
        {
            return ((GridLength)From).Value;
        }
    }

    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));

    public GridLength To
    {
        get
        {
            return (GridLength)GetValue(GridLengthAnimation.ToProperty);
        }
        set
        {
            SetValue(GridLengthAnimation.ToProperty, value);
        }
    }

    private double ToAsDouble
    {
        get
        {
            return ((GridLength)To).Value;
        }
    }

    public GridUnitType GridUnitType
    {
        get { return (GridUnitType)GetValue(GridUnitTypeProperty); }
        set { SetValue(GridUnitTypeProperty, value); }
    }

    public static readonly DependencyProperty GridUnitTypeProperty =
        DependencyProperty.Register("GridUnitType", typeof(GridUnitType), typeof(GridLengthAnimation), new UIPropertyMetadata(GridUnitType.Pixel));

    public double CurrentProgress(AnimationClock clock)
    {
        return Math.Abs(1f-((1f-clock.CurrentProgress.Value) * (1f-clock.CurrentProgress.Value)));
    }
    public double CurrentProgressQuad(AnimationClock clock)
    {
        return Math.Abs(1f - Math.Pow(1f - clock.CurrentProgress.Value, 4));
    }

    public double CurrentProgressQuadPow(AnimationClock clock)
    {
        return Math.Pow(clock.CurrentProgress.Value, 4);
    }

    public override object GetCurrentValue(object defaultOriginValue,
                                            object defaultDestinationValue,
                                            AnimationClock animationClock)
    {
        if (FromAsDouble > ToAsDouble)
            return new GridLength((1 - CurrentProgressQuad(animationClock)) *
                (FromAsDouble - ToAsDouble) + ToAsDouble, this.GridUnitType);

        return new GridLength(CurrentProgressQuad(animationClock) *
            (ToAsDouble - FromAsDouble) + FromAsDouble, this.GridUnitType);
    }
}