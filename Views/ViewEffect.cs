using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace BaseUI
{

    /// <summary>
    /// WPF View의 Grid 컨트롤의 Row 또는 Column의 크기를 애니메이션으로 변경하기 위한 클래스.
    /// </summary>
    public class GridResizeAnimation : AnimationTimeline
    {
        public override Type TargetPropertyType => typeof(GridLength);
        
        public static readonly DependencyProperty dpFromProperty 
        = DependencyProperty.Register(nameof(From), typeof(GridLength?), typeof(GridResizeAnimation));

        public static readonly DependencyProperty dpToProperty
        = DependencyProperty.Register(nameof(To), typeof(GridLength?), typeof(GridResizeAnimation));

        public GridLength? From
        {
            get => (GridLength?)GetValue(dpFromProperty);
            set => SetValue(dpFromProperty, value);
        }
        
        public GridLength? To
        {
            get => (GridLength?)GetValue(dpToProperty);
            set => SetValue(dpToProperty, value);
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            if(animationClock.CurrentProgress == null)
            {
                return defaultOriginValue;
            }

            double fromValue = From?.Value ?? ((GridLength)defaultOriginValue).Value;
            double toValue = To?.Value ?? ((GridLength)defaultDestinationValue).Value;
            double progress = animationClock.CurrentProgress.Value;
            double currentValue = ((toValue - fromValue) * progress) + fromValue;

            return new GridLength(currentValue, GridUnitType.Pixel);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new GridResizeAnimation();
        }
    }
}
